using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Augment;

namespace Ahab.Core
{
    public class FinancialsCollection : Collection<Financials>
    {
        #region Constructors

        public FinancialsCollection() 
        {
        }

        public FinancialsCollection(IEnumerable<Financials> financials) : this()
        {
            foreach (Financials f in financials)
            {
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
    }
}
