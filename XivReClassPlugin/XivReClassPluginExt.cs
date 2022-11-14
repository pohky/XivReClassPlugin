using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MonoMod.RuntimeDetour;
using ReClassNET;
using ReClassNET.Extensions;
using ReClassNET.Forms;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using ReClassNET.Plugins;
using ReClassNET.UI;
using XivReClassPlugin.Data;
using XivReClassPlugin.Forms;
using XivReClassPlugin.NodeReaders;
using XivReClassPlugin.Nodes;

namespace XivReClassPlugin {
    public sealed class XivReClassPluginExt : Plugin {
        public static string PluginName => "XivReClass";
        
        public static XivPluginSettings Settings { get; private set; } = null!;
        
        public static readonly Dictionary<nint, string> InternalNamedAddresses = new();
        public static readonly Dictionary<string, nint> InternalInstanceNames = new();
        
        public static Module? MainModule { get; private set; }
        
        private static Detour? m_GetModuleByNameDetour;
        private static Detour? m_ReadRttiInfoDetour;

        public override bool Initialize(IPluginHost host) {
            Settings = XivPluginSettings.Load();
            GlobalWindowManager.WindowAdded += OnWindowAdded;
            Program.RemoteProcess.ProcessAttached += OnProcessAttached;
            var cfg = new DetourConfig {ManualApply = true};
            m_GetModuleByNameDetour = new Detour(typeof(RemoteProcess).GetMethod("GetModuleByName")!, typeof(XivReClassPluginExt).GetMethod(nameof(GetModuleByNameDetour)), cfg);
            m_ReadRttiInfoDetour = new Detour(typeof(RemoteProcess).GetMethod("ReadRemoteRuntimeTypeInformation")!, typeof(XivReClassPluginExt).GetMethod(nameof(ReadRttiInfoDetour)), cfg);

            var menu = host.MainWindow.MainMenu.Items.OfType<ToolStripMenuItem>().FirstOrDefault(i => i.Text.Equals("Project"));
            if (menu != null) {
                var item = new ToolStripMenuItem("Generate ClientStructs Code", Resources.XivReClassResources.B16x16_Page_Code_Csharp);
                item.Click += (_, _) => Program.MainForm.ShowCodeGeneratorForm(new CsCodeGenerator());
                menu.DropDownItems.Add(item);
            }

            return true;
        }

        public override void Terminate() {
            Settings.Save();
            Program.RemoteProcess.NamedAddresses.Clear();
            GlobalWindowManager.WindowAdded -= OnWindowAdded;
            Program.RemoteProcess.ProcessAttached -= OnProcessAttached;
            m_GetModuleByNameDetour?.Dispose();
            m_ReadRttiInfoDetour?.Dispose();
        }

        public string? ReadRttiInfoDetour(nint address) {
            if (address <= 0x10_000) return null;
            if (InternalNamedAddresses.TryGetValue(address, out var name))
                return name;
            return null;
        }

        public Module GetModuleByNameDetour(string name) {
            if (InternalInstanceNames.TryGetValue(name, out var address))
                return new Module {Start = address, Name = name};
            return Program.RemoteProcess.Modules.FirstOrDefault(m => m.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))!;
        }

        private void OnProcessAttached(RemoteProcess sender) {
            InternalNamedAddresses.Clear();
            InternalInstanceNames.Clear();
            sender.NamedAddresses.Clear();
            m_GetModuleByNameDetour?.Undo();
            m_ReadRttiInfoDetour?.Undo();

            sender.EnumerateRemoteSectionsAndModules(out _, out var modules);
            MainModule = modules.Find(m => m.Name.Equals(sender.UnderlayingProcess.Name));

            if (sender.UnderlayingProcess.Name.Equals("ffxiv_dx11.exe", StringComparison.OrdinalIgnoreCase))
                Update();
        }

        public static void Update() {
            DataManager.Reload();
            InternalNamedAddresses.Clear();
            InternalInstanceNames.Clear();
            Program.RemoteProcess.NamedAddresses.Clear();
            if (DataManager.Classes.Count == 0)
                return;
            SetupData();
        }

        private static void SetupData() {
            if (!Program.RemoteProcess.IsValid || MainModule == null) return;

            var pureCall = (nint)DataManager.Data.Functions.FirstOrDefault(kv => kv.Value.Equals("_purecall")).Key;
            if (pureCall != 0)
	            pureCall = (nint)((ulong)MainModule.Start + ((ulong)pureCall - DataManager.DataBaseAddress));

            foreach (var info in DataManager.Classes) {
                var className = Settings.ShowInheritance ?
                    info.GetInheritanceName(Settings.ShowNamespaces) :
                    Settings.ShowNamespaces ? info.FullName : info.Name;
                var classAddress = MainModule.Start + (nint)info.Offset;
                
                InternalNamedAddresses[classAddress] = className;

                foreach (var function in info.Functions)
                    InternalNamedAddresses[(nint)function.Key + MainModule.Start] = function.Value;
                
                foreach (var instance in info.Instances)
                    InternalInstanceNames[instance.Value] = MainModule.Start + (nint)instance.Key;

                foreach (var kv in DataManager.Data.Globals) {
                    var address = MainModule.Start + (nint)(kv.Key - DataManager.DataBaseAddress);
                    var name = kv.Value;
                    InternalInstanceNames[name] = address;
                }

                foreach (var kv in DataManager.Data.Functions) {
                    var address = MainModule.Start + (nint)(kv.Key - DataManager.DataBaseAddress);
                    var name = kv.Value;
                    InternalNamedAddresses[address] = name;
                }

                var list = new List<ClassInfo>();
                var classInfo = info;
                while (classInfo != null) {
                    list.Add(classInfo);
                    classInfo = classInfo.ParentClass;
                }

                list.Reverse();
                list.ForEach(ci => {
                    if (ci.Offset == 0) return;
                    var vftable = MainModule.Start + (nint)ci.Offset;
                    foreach (var vf in ci.VirtualFunctions) {
                        var addr = (nint)Program.RemoteProcess.ReadRemoteIntPtr(vftable + vf.Key * 8);
                        if (addr != 0 && addr != pureCall && !InternalNamedAddresses.ContainsKey(addr))
                            InternalNamedAddresses[addr] = $"{ci.Name}.{vf.Value}";
                    }
                });
            }

            if (InternalInstanceNames.Count > 0)
                m_GetModuleByNameDetour?.Apply();

            if (!Settings.UseNamedAddresses) {
                m_ReadRttiInfoDetour?.Apply();
                Program.RemoteProcess.NamedAddresses.Clear();
                return;
            }

            m_ReadRttiInfoDetour?.Undo();

            foreach (var kv in InternalNamedAddresses)
                Program.RemoteProcess.NamedAddresses[kv.Key] = kv.Value;
        }

        public override IReadOnlyList<INodeInfoReader> GetNodeInfoReaders() {
            return new List<INodeInfoReader> {new XivClassNodeReader()};
        }

        public override CustomNodeTypes GetCustomNodeTypes() {
            return new CustomNodeTypes {
                NodeTypes = new[] {typeof(Utf8StringNode)},
                Serializer = new Utf8StringSerializer(),
                CodeGenerator = new Utf8StringGenerator()
            };
        }

        private static void OnWindowAdded(object sender, GlobalWindowManagerEventArgs e) {
            if (e.Form is not SettingsForm settingsForm)
                return;
            settingsForm.Shown += delegate {
                try {
                    if (settingsForm.Controls.Find("settingsTabControl", true).FirstOrDefault() is not TabControl settingsTabControl)
                        return;

                    var settingsTab = new TabPage(PluginName) {UseVisualStyleBackColor = true};
                    settingsTab.Controls.Add(new PluginSettingsTab {Dock = DockStyle.Fill});
                    settingsTabControl.TabPages.Add(settingsTab);
                } catch (Exception ex) {
                    Program.ShowException(ex);
                }
            };
        }
    }
}