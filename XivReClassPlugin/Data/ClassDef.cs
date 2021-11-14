namespace XivReClassPlugin.Data {
    public class ClassDef {
        private string? m_CachedName;

        public string Name { get; }
        public ulong Address { get; set; }
        public ClassDef? Parent { get; set; }

        public string FullName => m_CachedName ??= Parent == null ? $"{Name}" : $"{Name} : {Parent.FullName}";

        public ClassDef(string name, XivClass? data) {
            Name = name;
        }
    }
}