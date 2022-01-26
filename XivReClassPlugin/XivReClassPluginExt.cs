using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MonoMod.RuntimeDetour;
using ReClassNET;
using ReClassNET.Forms;
using ReClassNET.Logger;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using ReClassNET.Plugins;
using ReClassNET.UI;
using XivReClassPlugin.Data;
using XivReClassPlugin.Forms;
using XivReClassPlugin.NodeReaders;
using XivReClassPlugin.Nodes;
using Module = ReClassNET.Memory.Module;

namespace XivReClassPlugin {
    public sealed class XivReClassPluginExt : Plugin {
        public static string PluginName => "XivReClass";
        public static XivPluginSettings Settings { get; private set; } = null!;
        public static readonly Dictionary<nint, string> InternalNamedAddresses = new();
        public static readonly Dictionary<string, nint> InternalInstanceNames = new();
        public static Module? MainModule { get; private set; }
        private Detour? m_GetModuleByNameDetour;
        private Detour? m_ReadRttiInfoDetour;

        public override bool Initialize(IPluginHost host) {
            Settings = XivPluginSettings.Load();
            GlobalWindowManager.WindowAdded += OnWindowAdded;
            Program.RemoteProcess.ProcessAttached += OnProcessAttached;
            DataManager.DataUpdated += OnDataUpdated;
            var cfg = new DetourConfig {ManualApply = true};
            m_GetModuleByNameDetour = new Detour(typeof(RemoteProcess).GetMethod("GetModuleByName")!, typeof(XivReClassPluginExt).GetMethod(nameof(GetModuleByNameDetour)), cfg);
            m_ReadRttiInfoDetour = new Detour(typeof(RemoteProcess).GetMethod("ReadRemoteRuntimeTypeInformation")!, typeof(XivReClassPluginExt).GetMethod(nameof(ReadRttiInfoDetour)), cfg);
            return true;
        }

        public override void Terminate() {
            Settings.Save();
            Program.RemoteProcess.NamedAddresses.Clear();
            DataManager.DataUpdated -= OnDataUpdated;
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

        public override IReadOnlyList<INodeInfoReader> GetNodeInfoReaders() {
            return new List<INodeInfoReader> {new XivClassNodeReader()};
        }

        public override CustomNodeTypes GetCustomNodeTypes() {
            return new CustomNodeTypes {
                NodeTypes = new[] { typeof(Utf8StringNode) },
                Serializer = new Utf8StringSerializer(),
                CodeGenerator = new Utf8StringGenerator()
            };
        }

        private void OnProcessAttached(RemoteProcess sender) {
            MainModule = null;
            if (!sender.UnderlayingProcess.Name.Equals("ffxiv_dx11.exe", StringComparison.OrdinalIgnoreCase)) {
                DataManager.Clear();
                InternalNamedAddresses.Clear();
                InternalInstanceNames.Clear();
                sender.NamedAddresses.Clear();
                m_GetModuleByNameDetour?.Undo();
                m_ReadRttiInfoDetour?.Undo();
            } else {
                Update();
            }
        }

        public static void Update() {
            DataManager.ReloadData();
        }

        private void OnDataUpdated() {
            var process = Program.RemoteProcess;
            if (!process.IsValid) return;

            InternalNamedAddresses.Clear();
            InternalInstanceNames.Clear();
            process.NamedAddresses.Clear();
            m_GetModuleByNameDetour?.Undo();

            process.EnumerateRemoteSectionsAndModules(out _, out var modules);
            MainModule = modules.Find(m => m.Name.Equals(process.UnderlayingProcess.Name));

            if (MainModule == null)
                return;
            
            foreach (var def in DataManager.Classes) {
                var name = Settings.ShowInheritance ? def.Value.FullName : def.Value.Name;
                name = Settings.ShowNamespaces ? name : Utils.RemoveNamespace(name);
                InternalNamedAddresses[(nint)(def.Key + (ulong)MainModule.Start)] = name;
                foreach (var instance in def.Value.Instances) {
                    InternalNamedAddresses[(nint)(instance.Key + (ulong)MainModule.Start)] = instance.Value;
                    //InternalInstanceNames[instance.Value] = (nint)(instance.Key + (ulong)MainModule.Start);
                }
            }

            foreach (var xivClass in DataManager.Data.Classes) {
                var data = xivClass.Value;
                if(data == null) continue;
                var idx = 0;
                foreach (var instance in data.Instances) {
                    var instanceName = string.IsNullOrEmpty(instance.Name) ? $"Instance{(idx++ == 0 ? string.Empty : $"{idx}")}" : instance.Name;
                    var addr = (nint)(instance.Address - DataManager.DataBaseAddress) + MainModule.Start;
                    InternalInstanceNames[$"{xivClass.Key}_{instanceName}"] = addr;
                }
            }
            
            if (InternalInstanceNames.Count > 0)
                m_GetModuleByNameDetour?.Apply();
            
            if (!Settings.UseNamedAddresses) {
                m_ReadRttiInfoDetour?.Apply();
                return;
            }

            m_ReadRttiInfoDetour?.Undo();

            foreach (var kv in InternalNamedAddresses)
                process.NamedAddresses[kv.Key] = kv.Value;
        }

        private static void OnWindowAdded(object sender, GlobalWindowManagerEventArgs e) {
            if (e.Form is SettingsForm settingsForm) {
                settingsForm.Shown += delegate {
                    try {
                        if (settingsForm.Controls.Find("settingsTabControl", true).FirstOrDefault() is not TabControl settingsTabControl)
                            return;
                        
                        var settingsTab = new TabPage(PluginName) { UseVisualStyleBackColor = true };
                        settingsTab.Controls.Add(new PluginSettingsTab {Dock = DockStyle.Fill});
                        settingsTabControl.TabPages.Add(settingsTab);
                    } catch (Exception ex) {
                        Program.ShowException(ex);
                    }
                };
            }
		}
    }
}