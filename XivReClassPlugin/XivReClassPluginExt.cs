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
        public static readonly Dictionary<nint, string> InternalNamedAddresses = new();

        public override bool Initialize(IPluginHost host) {
            Settings = XivPluginSettings.Load();
            GlobalWindowManager.WindowAdded += OnWindowAdded;
            Program.RemoteProcess.ProcessAttached += OnProcessAttached;
            return true;
        }

        private static void OnProcessAttached(RemoteProcess sender) {
            if (!sender.UnderlayingProcess.Name.Equals("ffxiv_dx11.exe", StringComparison.OrdinalIgnoreCase)) {
                DataManager.Clear();
                InternalNamedAddresses.Clear();
                sender.NamedAddresses.Clear();
            } else Update();
        }

        public static void Update() {
            DataManager.Update();
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
            process.NamedAddresses.Clear();

            if (!Settings.UseNamedAddresses)
                return;

            foreach (var def in DataManager.Classes) {
                var name = Settings.ShowInheritance ? def.Value.ToString() : def.Value.Name;
                name = Settings.ShowNamespaces ? name : Utils.RemoveNamespace(name);
                var address = def.Key + (ulong)mod.Start;
                InternalNamedAddresses[(nint)address] = name;
            }

            foreach (var function in DataManager.Functions) {
                var address = function.Key + (ulong)mod.Start;
                InternalNamedAddresses[(nint)address] = function.Value;
            }

            foreach (var kv in InternalNamedAddresses)
                process.NamedAddresses[kv.Key] = kv.Value;
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