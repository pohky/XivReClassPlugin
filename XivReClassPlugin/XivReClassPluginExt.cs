using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReClassNET;
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

        public override bool Initialize(IPluginHost host) {
            Settings = XivPluginSettings.Load();
            GlobalWindowManager.WindowAdded += OnWindowAdded;
            Program.RemoteProcess.ProcessAttached += OnProcessAttached;
            return true;
        }

        private static void OnProcessAttached(RemoteProcess sender) {
            if (!sender.UnderlayingProcess.Name.Equals("ffxiv_dx11.exe", StringComparison.OrdinalIgnoreCase)) {
                XivDataManager.Clear();
                sender.NamedAddresses.Clear();
            } else Update();
        }

        public static void Update() {
            XivDataManager.Update();
            if (Settings.UseNamedAddresses)
                Task.Run(UpdateNamedAddresses);
            else Program.RemoteProcess.NamedAddresses.Clear();
        }

        public static async Task UpdateNamedAddresses() {
            var process = Program.RemoteProcess;
            if (!process.IsValid) return;
            var mod = process.GetModuleByName(Program.RemoteProcess.UnderlayingProcess.Name);
            while (mod == null) {
                mod = process.GetModuleByName(Program.RemoteProcess.UnderlayingProcess.Name);
                await Task.Delay(20);
            }
            
            process.NamedAddresses.Clear();
            foreach (var dataClass in XivDataManager.Data.classes) {
                if (dataClass.Value == null || dataClass.Value.vtbl == 0)
                    continue;
                var name = Settings.ShowNamespaces ? dataClass.Key : Utils.RemoveNamespace(dataClass.Key);
                process.NamedAddresses[ToModuleAddress(mod.Start, dataClass.Value.vtbl)] = name;
            }

            foreach (var dataGlobal in XivDataManager.Data.globals.Where(d => d.Key != 0)) {
                process.NamedAddresses[ToModuleAddress(mod.Start, dataGlobal.Key)] = dataGlobal.Value;
            }

            foreach (var dataFunction in XivDataManager.Data.functions.Where(d => d.Key != 0 && !string.IsNullOrEmpty(d.Value))) {
                process.NamedAddresses[ToModuleAddress(mod.Start, dataFunction.Key)] = dataFunction.Value;
            }

            static IntPtr ToModuleAddress(nint moduleAddress, long dataAddress) {
                return new IntPtr(moduleAddress + (dataAddress - XivDataManager.DataBaseAddress));
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