using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HarmonyLib;
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
using Module = ReClassNET.Memory.Module;

namespace XivReClassPlugin; 

public sealed class XivReClassPluginExt : Plugin {
	public static string PluginName => "XivReClass";
	private const string HarmonyId = "reclass.plugin.ffxiv";
	
	public static XivPluginSettings Settings { get; private set; } = null!;
        
	public static readonly Dictionary<nint, string> InternalNamedAddresses = new();
	public static readonly Dictionary<string, nint> InternalInstanceNames = new();
        
	public static Module? MainModule { get; private set; }
	public static Harmony Harmony { get; private set; } = null!;
	
	public override bool Initialize(IPluginHost host) {
		Settings = XivPluginSettings.Load();
		GlobalWindowManager.WindowAdded += OnWindowAdded;
		Program.RemoteProcess.ProcessAttached += OnProcessAttached;
		
		Harmony = new Harmony(HarmonyId);
		Harmony.PatchAll(Assembly.GetExecutingAssembly());

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
		Harmony.UnpatchAll(HarmonyId);
		Program.RemoteProcess.NamedAddresses.Clear();
		GlobalWindowManager.WindowAdded -= OnWindowAdded;
		Program.RemoteProcess.ProcessAttached -= OnProcessAttached;
	}

	public static nint GetAgentModule(IRemoteMemoryReader process) {
		if (!InternalInstanceNames.TryGetValue("Client::System::Framework::Framework_InstancePointer2", out var fwPointer))
			return 0;
		var fwAddress = process.ReadRemoteIntPtr(fwPointer);
		if (fwAddress == IntPtr.Zero)
			return 0;

		var uiVf = InternalNamedAddresses.FirstOrDefault(kv => kv.Value.EndsWith("Framework.GetUIModule", StringComparison.OrdinalIgnoreCase));
		var agentVf = InternalNamedAddresses.FirstOrDefault(kv => kv.Value.EndsWith("UiModule.GetAgentModule", StringComparison.OrdinalIgnoreCase));

		var data = new byte[8];
		if (!process.ReadRemoteMemoryIntoBuffer(uiVf.Key + 8, ref data))
			return 0;
		var uiOffset = BitConverter.ToInt32(data, 3);

		if (!process.ReadRemoteMemoryIntoBuffer(agentVf.Key, ref data))
			return 0;
		var agentOffset = BitConverter.ToInt32(data, 3);
	        
		if (uiOffset <= 0 || agentOffset <= 0)
			return 0;

		var uiModule = process.ReadRemoteIntPtr(fwAddress + uiOffset);
		var agentModule = uiModule + agentOffset;

		return agentModule;
	}

	private void OnProcessAttached(RemoteProcess sender) {
		InternalNamedAddresses.Clear();
		InternalInstanceNames.Clear();
		sender.NamedAddresses.Clear();
		
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

		var agentModule = GetAgentModule(Program.RemoteProcess);
		if (agentModule != 0)
			InternalInstanceNames["AgentModule_Instance"] = agentModule;

		Program.RemoteProcess.NamedAddresses.Clear();
		if (!Settings.UseNamedAddresses)
			return;
		foreach (var kv in InternalNamedAddresses)
			Program.RemoteProcess.NamedAddresses[kv.Key] = kv.Value;
	}

	public override IReadOnlyList<INodeInfoReader> GetNodeInfoReaders() {
		return new List<INodeInfoReader> {new XivClassNodeReader()};
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
