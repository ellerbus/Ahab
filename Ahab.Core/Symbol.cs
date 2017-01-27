using System.Diagnostics;

namespace Ahab.Core
{
    #region Enum

    public enum MarketCaps
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// LT $50M
        /// </summary>
        Nano,
        /// <summary>
        /// LT $300M
        /// </summary>
        Micro,
        /// <summary>
        /// LT $2B
        /// </summary>
        Small,
        /// <summary>
        /// LT $10B
        /// </summary>
        Mid,
        /// <summary>
        /// LT $200B
        /// </summary>
        Large,
        /// <summary>
        /// 
        /// </summary>
        Mega
    }

    #endregion

    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public class Symbol
    {
        #region Methods

        private string DebuggerDisplay()
        {
            return $"{GetType().Name} StockId={StockId}";
        }

        public override string ToString()
        {
            return DebuggerDisplay();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string StockId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MarketCaps MarketCap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Sector { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Industry { get; set; }

        #endregion
    }
}
