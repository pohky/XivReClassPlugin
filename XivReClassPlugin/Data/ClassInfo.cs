﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XivReClassPlugin.Data;

public class ClassInfo {
    private static readonly Regex NameRegex = new("\\w+::", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex NamespaceSplitRegex = new("(?(?=.*<)::|::(?!.*>))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private string m_InheritanceNameCache = string.Empty;
    private string m_InheritanceNameFullCache = string.Empty;

    public string Name { get; set; }
    public string Namespace { get; set; }
    public string FullName { get; set; }
    public ulong Offset { get; set; }
    public ClassInfo? ParentClass { get; set; }
    public Dictionary<ulong, string> Functions { get; } = new();
    public Dictionary<int, string> VirtualFunctions { get; } = new();
    public Dictionary<ulong, string> Instances { get; } = new();

    public ClassInfo(ClientStructsData data, string rawName, XivClass? xivClass, XivVTable? baseClass) {
        if (string.IsNullOrWhiteSpace(rawName))
            rawName = string.Empty;
        Name = NameRegex.Replace(rawName, string.Empty);
        var nsSplit = NamespaceSplitRegex.Split(rawName);
        if (nsSplit.Length > 1) {
            nsSplit[nsSplit.Length - 1] = Name;
            Namespace = string.Join("::", nsSplit.Take(nsSplit.Length - 1));
        } else {
            Namespace = string.Empty;
        }

        FullName = string.IsNullOrEmpty(Namespace) ? Name : $"{Namespace}::{Name}";

        if (xivClass == null)
            return;

        var baseVtable = baseClass ?? xivClass.VirtualTables?.FirstOrDefault();

        Offset = baseVtable?.Address ?? 0;

        try {
            foreach (var func in xivClass.Functions) {
                if (func.Key == 0 || string.IsNullOrWhiteSpace(func.Value))
                    continue;
                Functions[func.Key] = $"{FullName}.{func.Value}";
            }
        } catch (NullReferenceException) {
            /* ignored, can happen with incomplete defs after patch */
        }

        try {
            foreach (var vf in xivClass.VirtualFunctions) {
                if (string.IsNullOrWhiteSpace(vf.Value))
                    continue;
                VirtualFunctions[vf.Key] = vf.Value;
            }
        } catch (NullReferenceException) {
            /* ignored, can happen with incomplete defs after patch */
        }

        var parentName = baseVtable?.Base;
        if (!string.IsNullOrEmpty(parentName) && data.Classes.TryGetValue(parentName!, out var parentClass)) {
            ParentClass = new ClassInfo(data, parentName!, parentClass, null);

            var parent = ParentClass;
            while (parent != null) {
                foreach (var vf in parent.VirtualFunctions.Where(vf => !VirtualFunctions.ContainsKey(vf.Key)))
                    VirtualFunctions[vf.Key] = vf.Value;
                parent = parent.ParentClass;
            }
        }

        var instanceIndex = 0;
        foreach (var instance in xivClass.Instances) {
            var name = !string.IsNullOrEmpty(instance.Name) ? instance.Name : $"Instance{(instanceIndex++ == 0 ? string.Empty : instanceIndex.ToString())}";
            Instances[instance.Address] = $"{FullName}_{name}";
        }
    }

    public string GetInheritanceName(bool includeNamespace) {
        if (includeNamespace) {
            if (!string.IsNullOrEmpty(m_InheritanceNameFullCache))
                return m_InheritanceNameFullCache;
            var name = FullName;
            var parent = ParentClass;
            while (parent != null) {
                name += $" : {parent.FullName}";
                parent = parent.ParentClass;
            }

            m_InheritanceNameFullCache = name;
            return name;
        } else {
            if (!string.IsNullOrEmpty(m_InheritanceNameCache))
                return m_InheritanceNameCache;
            var name = Name;
            var parent = ParentClass;
            while (parent != null) {
                name += $" : {parent.Name}";
                parent = parent.ParentClass;
            }

            m_InheritanceNameCache = name;
            return name;
        }
    }
}
