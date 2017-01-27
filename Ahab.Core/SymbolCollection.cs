using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Augment;

namespace Ahab.Core
{
    public class SymbolCollection : KeyedCollection<string, Symbol>
    {
        #region Members

        private readonly object _lock = new object();

        #endregion

        #region Constructors

        public SymbolCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public SymbolCollection(IEnumerable<Symbol> symbols) : this()
        {
            foreach (Symbol symbol in symbols)
            {
                if (!Contains(symbol.StockId))
                {
                    Add(symbol);
                }
            }
        }

        #endregion

        #region ToString/DebuggerDisplay

        ///	<summary>
        ///	DebuggerDisplay for this object
        ///	</summary>
        private string DebuggerDisplay
        {
            get { return "Count={0}".FormatArgs(Count); }
        }

        #endregion

        #region Collection Methods

        protected override string GetKeyForItem(Symbol item)
        {
            return item.StockId;
        }

        #endregion
    }
}
