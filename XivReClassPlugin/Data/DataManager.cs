using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ReClassNET;
using YamlDotNet.Serialization;

namespace XivReClassPlugin.Data {
    public static class DataManager {
        public const ulong DataBaseAddress = 0x1_4000_0000;
        public static ClientStructsData Data { get; private set; } = new();

        public static List<ClassInfo> Classes { get; } = new();
        public static Dictionary<ulong, ClassInfo> ClassMap { get; } = new();

        public static bool TryGetClassByOffset(ulong offset, out ClassInfo info) {
            return ClassMap.TryGetValue(offset, out info);
        }
        
        public static void Reload() {
            var path = XivReClassPluginExt.Settings.ClientStructsDataPath;
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                Data = new ClientStructsData();
            else {
                try {
                    Data = new DeserializerBuilder().Build().Deserialize<ClientStructsData>(File.ReadAllText(path, Encoding.UTF8));
                } catch (Exception ex) {
                    Data = new ClientStructsData();
                    Program.ShowException(ex);
                }
            }
            UpdateClasses();
        }

        private static void UpdateClasses() {
            Classes.Clear();
            ClassMap.Clear();
            foreach (var kv in Data.Classes) {
                try {
                    if (kv.Value is {VirtualTables: {Count: > 1}}) {
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
    }
}