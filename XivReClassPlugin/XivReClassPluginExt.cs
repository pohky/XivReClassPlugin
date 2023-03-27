using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HarmonyLib;
using ReClassNET;
using ReClassNET.Forms;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using ReClassNET.Plugins;
using ReClassNET.UI;
using XivReClassPlugin.Data;
using XivReClassPlugin.Forms;
using XivReClassPlugin.Forms.Controls;
using XivReClassPlugin.NodeReaders;
using XivReClassPlugin.Nodes;
using XivReClassPlugin.Resources;

namespace XivReClassPlugin;

public sealed class XivReClassPluginExt : Plugin {
	private const string HarmonyId = "reclass.plugin.ffxiv";
	private const string PluginName = "XivReClass";

	public static IPluginHost Host { get; private set; } = null!;
	public static Harmony Harmony { get; private set; } = null!;

	public override bool Initialize(IPluginHost host) {
		Host = host;
		GlobalWindowManager.WindowAdded += OnWindowAdded;
		Program.RemoteProcess.ProcessAttached += OnProcessAttached;

		Harmony = new Harmony(HarmonyId);
		Harmony.PatchAll(Assembly.GetExecutingAssembly());
		SetupMenu(host);

		return true;
	}

	public override void Terminate() {
		GlobalWindowManager.WindowAdded -= OnWindowAdded;
		Program.RemoteProcess.ProcessAttached -= OnProcessAttached;
		Ffxiv.Settings.Save();

		Harmony.UnpatchAll(HarmonyId);

		if (Ffxiv.Settings.UseNamedAddresses)
			Program.RemoteProcess.NamedAddresses.Clear();
	}

	private void OnProcessAttached(RemoteProcess sender) {
		sender.NamedAddresses.Clear();
		if (sender.UnderlayingProcess.Name.Equals("ffxiv_dx11.exe", StringComparison.OrdinalIgnoreCase)) {
			DataManager.Reload();
			Ffxiv.Update();
		}
	}

	private void SetupMenu(IPluginHost host) {
		var mainMenu = host.MainWindow.MainMenu;
		var projectMenu = mainMenu.Items.Cast<ToolStripMenuItem>().FirstOrDefault(m => m.Text.Equals("Project"));
		var xivMenu = new ToolStripMenuItem("FFXIV");
		mainMenu.Items.Add(xivMenu);
		
		var generatorItem = new ToolStripMenuItem("Generate ClientStructs Code", XivReClassResources.CSharpIcon);
		generatorItem.Click += (_, _) => Program.MainForm.ShowCodeGeneratorForm(new CsCodeGenerator());
		if (projectMenu == null) {
			xivMenu.DropDownItems.Add(generatorItem);
			xivMenu.DropDownItems.Add(new ToolStripSeparator());
		} else projectMenu.DropDownItems.Add(generatorItem);

		var agentsItem = new ToolStripMenuItem("Agents");
		agentsItem.Click += (_, _) => new AgentListForm().Show(host.MainWindow);
		xivMenu.DropDownItems.Add(agentsItem);
	}

	private void OnWindowAdded(object sender, GlobalWindowManagerEventArgs e) {
		if (e.Form is not SettingsForm settingsForm)
			return;
		settingsForm.Shown += delegate {
			try {
				if (settingsForm.Controls.Find("settingsTabControl", true).FirstOrDefault() is not TabControl settingsTabControl)
					return;

				var settingsTab = new TabPage(PluginName) { UseVisualStyleBackColor = true };
				settingsTab.Controls.Add(new PluginSettingsTab { Dock = DockStyle.Fill });
				settingsTabControl.TabPages.Add(settingsTab);
			} catch (Exception ex) {
				Program.ShowException(ex);
			}
		};
	}

	public override IReadOnlyList<INodeInfoReader> GetNodeInfoReaders() {
		return new List<INodeInfoReader> { new XivClassNodeReader() };
	}

	public override CustomNodeTypes GetCustomNodeTypes() {
		return new CustomNodeTypes {
			NodeTypes = new[] {
				typeof(Utf8StringNode),
				typeof(VectorNode),
				typeof(AtkValueNode)
			},
			Serializer = new XivNodeSerializer(),
			CodeGenerator = new XivNodeGenerator()
		};
	}
}
