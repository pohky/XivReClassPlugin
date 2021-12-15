using System.Collections.Generic;

namespace XivReClassPlugin {
    public class XivSymbolResolver {
        private readonly Dictionary<long, string> m_SymbolCache = new();
        private readonly HashSet<long> m_Blacklist = new();
        private XivPluginSettings m_Settings;

        public XivSymbolResolver(XivPluginSettings settings) {
            m_Settings = settings;
        }

        public bool GetSymbol(long address, out string symbol) {
            symbol = string.Empty;
            if (m_Blacklist.Contains(address))
                return false;
            if (m_SymbolCache.TryGetValue(address, out symbol)) {
                if (!m_Settings.ShowNamespaces)
                    symbol = Utils.RemoveNamespace(symbol);
                return true;
            }

            if (GetSymbolForOffset(address - 0, out symbol)) {
                m_SymbolCache[address] = symbol;
                return true;
            }

            m_Blacklist.Add(address);
            return false;
        }

        private bool GetSymbolForOffset(long offset, out string symbol) {
            symbol = string.Empty;
            return false;
        }
    }
}