using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ReClassNET;
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
        public static Module? MainModule { get; private set; }

        public override bool Initialize(IPluginHost host) {
            Settings = XivPluginSettings.Load();
            GlobalWindowManager.WindowAdded += OnWindowAdded;
            Program.RemoteProcess.ProcessAttached += OnProcessAttached;
            DataManager.DataUpdated += OnDataUpdated;
            return true;
        }

        public override void Terminate() {
            Settings.Save();
            Program.RemoteProcess.NamedAddresses.Clear();
            DataManager.DataUpdated -= OnDataUpdated;
            GlobalWindowManager.WindowAdded -= OnWindowAdded;
            Program.RemoteProcess.ProcessAttached -= OnProcessAttached;
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

        private static void OnProcessAttached(RemoteProcess sender) {
            MainModule = null;
            if (!sender.UnderlayingProcess.Name.Equals("ffxiv_dx11.exe", StringComparison.OrdinalIgnoreCase)) {
                DataManager.Clear();
                InternalNamedAddresses.Clear();
                sender.NamedAddresses.Clear();
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
            process.EnumerateRemoteSectionsAndModules(out _, out var modules);
            MainModule = modules.Find(m => m.Name.Equals(process.UnderlayingProcess.Name));

            InternalNamedAddresses.Clear();
            process.NamedAddresses.Clear();

            if (MainModule == null)
                return;
            
            foreach (var def in DataManager.Classes) {
                var name = Settings.ShowInheritance ? def.Value.FullName : def.Value.Name;
                name = Settings.ShowNamespaces ? name : Utils.RemoveNamespace(name);
                var address = def.Key + (ulong)MainModule.Start;
                InternalNamedAddresses[(nint)address] = name;
                foreach (var instance in def.Value.Instances)
                    InternalNamedAddresses[(nint)(instance.Key + (ulong)MainModule.Start)] = instance.Value;
            }

            if (!Settings.UseNamedAddresses)
                return;

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