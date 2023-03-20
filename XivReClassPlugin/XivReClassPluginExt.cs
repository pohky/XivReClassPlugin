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
using XivReClassPlugin.Resources;
using Module = ReClassNET.Memory.Module;

namespace XivReClassPlugin;

public sealed class XivReClassPluginExt : Plugin {
	private const string PluginName = "XivReClass";
	
	public XivPluginSettings Settings { get; private set; } = new();
	public Module MainModule { get; private set; } = new();
	public ClientStructsSymbols Symbols { get; private set; } = null!;
	public DataManager Data { get; private set; } = null!;

	public override bool Initialize(IPluginHost host) {
		Settings = XivPluginSettings.Load();
		GlobalWindowManager.WindowAdded += OnWindowAdded;
		Program.RemoteProcess.ProcessAttached += OnProcessAttached;

		Data = new DataManager(this);
		Symbols = new ClientStructsSymbols(this);
		
		HarmonyPatches.Setup(this);
		SetupMenu(host);

		return true;
	}

	public override void Terminate() {
		GlobalWindowManager.WindowAdded -= OnWindowAdded;
		Program.RemoteProcess.ProcessAttached -= OnProcessAttached;
		Settings.Save();
		HarmonyPatches.Remove();

		if (Settings.UseNamedAddresses)
			Program.RemoteProcess.NamedAddresses.Clear();
	}

	private void OnProcessAttached(RemoteProcess sender) {
		Symbols.Clear();
		sender.NamedAddresses.Clear();

		sender.EnumerateRemoteSectionsAndModules(out _, out var modules);
		MainModule = modules.Find(m => m.Name.Equals(sender.UnderlayingProcess.Name));

		if (sender.UnderlayingProcess.Name.Equals("ffxiv_dx11.exe", StringComparison.OrdinalIgnoreCase))
			Update();
	}

	public void Update() {
		Data.Reload();
		Symbols.Reload();

		Program.RemoteProcess.NamedAddresses.Clear();
		if (Settings.UseNamedAddresses) {
			foreach (var kv in Symbols.NamedAddresses)
				Program.RemoteProcess.NamedAddresses[kv.Key] = kv.Value;
		}
	}

	private void SetupMenu(IPluginHost host) {
		var menu = host.MainWindow.MainMenu.Items.OfType<ToolStripMenuItem>().FirstOrDefault(i => i.Text.Equals("Project"));
		if (menu != null) {
			var item = new ToolStripMenuItem("Generate ClientStructs Code", XivReClassResources.CSharpIcon);
			item.Click += (_, _) => Program.MainForm.ShowCodeGeneratorForm(new CsCodeGenerator());
			menu.DropDownItems.Add(item);
		}
	}

	private void OnWindowAdded(object sender, GlobalWindowManagerEventArgs e) {
		if (e.Form is not SettingsForm settingsForm)
			return;
		settingsForm.Shown += delegate {
			try {
				if (settingsForm.Controls.Find("settingsTabControl", true).FirstOrDefault() is not TabControl settingsTabControl)
					return;

				var settingsTab = new TabPage(PluginName) { UseVisualStyleBackColor = true };
				settingsTab.Controls.Add(new PluginSettingsTab(this) { Dock = DockStyle.Fill });
				settingsTabControl.TabPages.Add(settingsTab);
			} catch (Exception ex) {
				Program.ShowException(ex);
			}
		};
	}

	public override IReadOnlyList<INodeInfoReader> GetNodeInfoReaders() {
		return new List<INodeInfoReader> { new XivClassNodeReader(this) };
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
