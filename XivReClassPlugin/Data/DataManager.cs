using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ReClassNET;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace XivReClassPlugin.Data;

public static class DataManager {
	private const ulong DataBaseAddress = 0x1_4000_0000;
	public static ClientStructsData Data { get; private set; } = new();

	public static List<ClassInfo> Classes { get; } = new();
	public static Dictionary<ulong, ClassInfo> ClassMap { get; } = new();
	
	public static void Reload() {
		Classes.Clear();
		ClassMap.Clear();
		var path = Ffxiv.Settings.ClientStructsDataPath;
		if (string.IsNullOrEmpty(path) || !File.Exists(path))
			Data = new ClientStructsData();
		else {
			try {
				Data = new DeserializerBuilder()
					.WithNodeDeserializer(new AddressDeserializer())
					.Build()
					.Deserialize<ClientStructsData>(File.ReadAllText(path, Encoding.UTF8));
			} catch (Exception ex) {
				Data = new ClientStructsData();
				Program.ShowException(ex);
			}
		}
		UpdateClasses();
	}

	private static void UpdateClasses() {
		foreach (var kv in Data.Classes) {
			try {
				if (kv.Value is { VirtualTables.Count: > 1 }) {
					foreach (var vTable in kv.Value.VirtualTables) {
						var vtInfo = new ClassInfo(Data, kv.Key, kv.Value, vTable);
						Classes.Add(vtInfo);
						if (vtInfo.Offset != 0)
							ClassMap[vtInfo.Offset] = vtInfo;
					}
				} else {
					var info = new ClassInfo(Data, kv.Key, kv.Value, null);
					Classes.Add(info);
					if (info.Offset == 0) continue;
					ClassMap[info.Offset] = info;
				}
			} catch (Exception ex) {
				Program.ShowException(ex);
			}
		}
	}

	private class AddressDeserializer : INodeDeserializer {
		public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value) {
			value = null;
			var underlyingType = Nullable.GetUnderlyingType(expectedType) ?? expectedType;
			if (underlyingType != typeof(ulong))
				return false;

			if (!reader.TryConsume<Scalar>(out var scalar) || !TryGetAddress(scalar.Value, out var address))
				return false;

			if (address < DataBaseAddress)
				return false;

			value = address - DataBaseAddress;
			return true;
		}

		private static bool TryGetAddress(string value, out ulong address) {
			address = 0;
			if (value.Length <= 2 || !value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
				return false;
			return ulong.TryParse(value.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out address);
		}
	}
}
