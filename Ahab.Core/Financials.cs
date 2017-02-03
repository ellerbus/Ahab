using System.Collections.Generic;

namespace Ahab.Core
{
    public class Financials
    {
        public IDictionary<string, string> LineItems { get; } = new Dictionary<string, string>();
    }
}
