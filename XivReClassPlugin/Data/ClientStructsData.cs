using System.Collections.Generic;
using YamlDotNet.Serialization;

// ReSharper disable CollectionNeverUpdated.Global

namespace XivReClassPlugin.Data;

public class ClientStructsData {
    [YamlMember(Alias = "version")] public string Version { get; set; } = string.Empty;
    [YamlMember(Alias = "globals")] public Dictionary<ulong, string> Globals { get; set; } = new();
    [YamlMember(Alias = "functions")] public Dictionary<ulong, string> Functions { get; set; } = new();
    [YamlMember(Alias = "classes")] public Dictionary<string, XivClass?> Classes { get; set; } = new();
}

public class XivClass {
    [YamlMember(Alias = "instances")] public List<XivInstance> Instances { get; set; } = new();
    [YamlMember(Alias = "funcs")] public Dictionary<ulong, string> Functions { get; set; } = new();
    [YamlMember(Alias = "vtbls")] public List<XivVTable>? VirtualTables { get; set; } = new();
    [YamlMember(Alias = "vfuncs")] public Dictionary<int, string> VirtualFunctions { get; set; } = new();
}

public class XivVTable {
    [YamlMember(Alias = "ea")] public ulong Address { get; set; }
    [YamlMember(Alias = "base")] public string Base { get; set; } = string.Empty;
}

public sealed class XivInstance {
    [YamlMember(Alias = "ea")] public ulong Address { get; set; }
    [YamlMember(Alias = "name")] public string? Name { get; set; } = string.Empty;
    [YamlMember(Alias = "pointer")] public bool IsPointer { get; set; } = false;
}
