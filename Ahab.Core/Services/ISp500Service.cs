using System.Collections.Generic;

namespace Ahab.Core.Services
{
    /// <summary>
    /// Represents the interface for getting symbols of the SP500
    /// </summary>
    public interface ISp500ConstituentService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetConstituents();
    }
}
