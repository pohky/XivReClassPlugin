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

namespace XivReClassPlugin {
    public sealed class XivReClassPluginExt : Plugin {
        public static string PluginName => "XivReClass";
        public static XivPluginSettings Settings { get; private set; } = null!;

        public override bool Initialize(IPluginHost host) {
            Settings = XivPluginSettings.Load();
            XivDataManager.Update();
            GlobalWindowManager.WindowAdded += OnWindowAdded;
            Program.RemoteProcess.ProcessAttached += OnProcessAttached;
            return true;
        }

        private void OnProcessAttached(RemoteProcess sender) {
            XivDataManager.Update();
        }

        public override void Terminate() {
            Settings.Save();
            GlobalWindowManager.WindowAdded -= OnWindowAdded;
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