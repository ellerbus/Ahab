using System.Collections.Generic;

namespace Ahab.Core.Services
{
    /// <summary>
    /// Represents the interface for getting symbols (names, sectors, etc)
    /// </summary>
    public interface ISymbolService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Symbol> GetAllSymbols();
    }
}
