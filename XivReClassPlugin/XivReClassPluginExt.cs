using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReClassNET;
using ReClassNET.Extensions;
using ReClassNET.Forms;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using ReClassNET.Plugins;
using ReClassNET.UI;
using XivReClassPlugin.Data;
using XivReClassPlugin.Forms;

namespace XivReClassPlugin {
    public sealed class XivReClassPluginExt : Plugin {
        public static string PluginName => "XivReClass";
        public static XivPluginSettings Settings { get; private set; } = null!;
        public static readonly Dictionary<nint, string> InternalNamedAddresses = new();

        public override bool Initialize(IPluginHost host) {
            Settings = XivPluginSettings.Load();
            GlobalWindowManager.WindowAdded += OnWindowAdded;
            Program.RemoteProcess.ProcessAttached += OnProcessAttached;
            return true;
        }

        private static void OnProcessAttached(RemoteProcess sender) {
            if (!sender.UnderlayingProcess.Name.Equals("ffxiv_dx11.exe", StringComparison.OrdinalIgnoreCase)) {
                XivDataManager.Clear();
                InternalNamedAddresses.Clear();
                sender.NamedAddresses.Clear();
            } else Update();
        }

        public static void Update() {
            XivDataManager.Update();
            Task.Run(UpdateNamedAddresses);
        }

        public static async Task UpdateNamedAddresses() {
            var process = Program.RemoteProcess;
            if (!process.IsValid) return;
            var mod = process.GetModuleByName(Program.RemoteProcess.UnderlayingProcess.Name);
            while (mod == null) {
                mod = process.GetModuleByName(Program.RemoteProcess.UnderlayingProcess.Name);
                await Task.Delay(20);
            }

            InternalNamedAddresses.Clear();
            foreach (var classList in XivDataManager.Data.classes.Select(GetClasses)) {
                var vfdict = new Dictionary<int, string>();
                foreach (var (className, dataClass) in classList.Reverse()) {
                    if (dataClass.vtbl == 0)
                        continue;
                    var name = Settings.ShowNamespaces ? className : Utils.RemoveNamespace(className);
                    var address = ToModuleAddress(mod.Start, dataClass.vtbl);
                    InternalNamedAddresses[address] = name;
                    foreach (var kv in vfdict)
                        dataClass.vfuncs[kv.Key] = kv.Value;
                    foreach (var vfunc in dataClass.vfuncs) {
                        vfdict[vfunc.Key] = vfunc.Value;
                        var vaddr = process.ReadRemoteIntPtr(address + vfunc.Key * 8);
                        if (!InternalNamedAddresses.ContainsKey(vaddr))
                            InternalNamedAddresses[vaddr] = $"{name}.{vfunc.Value}";
                    }
                }
            }

            foreach (var dataGlobal in XivDataManager.Data.globals.Where(d => d.Key != 0)) {
                InternalNamedAddresses[ToModuleAddress(mod.Start, dataGlobal.Key)] = dataGlobal.Value;
            }

            foreach (var dataFunction in XivDataManager.Data.functions.Where(d => d.Key != 0 && !string.IsNullOrEmpty(d.Value))) {
                InternalNamedAddresses[ToModuleAddress(mod.Start, dataFunction.Key)] = dataFunction.Value!;
            }

            process.NamedAddresses.Clear();
            if (Settings.UseNamedAddresses) {
                foreach (var kv in InternalNamedAddresses) {
                    process.NamedAddresses[kv.Key] = kv.Value;
                }
            }

            static IntPtr ToModuleAddress(nint moduleAddress, long dataAddress) {
                return new IntPtr(moduleAddress + (dataAddress - XivDataManager.DataBaseAddress));
            }

            static IEnumerable<(string Name, XivClass DataClass)> GetClasses(KeyValuePair<string, XivClass?> kv) {
                var className = kv.Key;
                var xivClass = kv.Value;
                while (xivClass != null && !string.IsNullOrEmpty(className)) {
                    yield return (className, xivClass);
                    className = xivClass.inherits_from ?? string.Empty;
                    xivClass = string.IsNullOrEmpty(className) ? null : XivDataManager.Data.classes[className];
                }
            }
        }

        public override void Terminate() {
            Settings.Save();
            Program.RemoteProcess.NamedAddresses.Clear();
            GlobalWindowManager.WindowAdded -= OnWindowAdded;
            Program.RemoteProcess.ProcessAttached -= OnProcessAttached;
        }

        public override IReadOnlyList<INodeInfoReader> GetNodeInfoReaders() {
            return new List<INodeInfoReader> { new XivClassNodeReader() };
        }

        private static void OnWindowAdded(object sender, GlobalWindowManagerEventArgs e) {
            if (e.Form is SettingsForm settingsForm) {
                settingsForm.Shown += delegate {
                    try {
                        if (settingsForm.Controls.Find("settingsTabControl", true).FirstOrDefault() is not TabControl settingsTabControl)
                            return;
                        var newTab = new TabPage(PluginName) { UseVisualStyleBackColor = true };
                        newTab.Controls.Add(new PluginSettingsTab());
                        settingsTabControl.TabPages.Add(newTab);
                    } catch (Exception ex) {
                        Program.ShowException(ex);
                    }
                };
            }
		}
    }
}