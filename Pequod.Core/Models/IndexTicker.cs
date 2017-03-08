namespace Pequod.Core.Models
{
    /// <summary>
    /// A Ticker that is available as part of an index
    /// </summary>
    public class IndexTicker
    {
        public string Ticker { get; set; }

        public string Name { get; set; }

        public string FreeCode { get; set; }

        public string PremiumCode { get; set; }
    }
}
