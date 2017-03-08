using System;
using System.Diagnostics;
using Augment;

namespace Pequod.Core.Models
{
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public class Split
    {
        #region Constructors

        public Split() { }

        public Split(string stockId, DateTime date, string ratio) : this()
        {
            Ticker = stockId;

            Date = date;

            Ratio = ratio;
        }

        #endregion

        #region Methods

        private string DebuggerDisplay()
        {
            string s = $"{GetType().Name} Date={Date:MM/dd/yyyy} Split={Ratio}";

            return s;
        }

        public override string ToString()
        {
            return DebuggerDisplay();
        }

        private static int GCD(int p, int q)
        {
            if (q == 0)
            {
                return p;
            }

            int r = p % q;

            return GCD(q, r);
        }

        #endregion

        #region Properties

        ///	<summary>
        ///	
        ///	</summary>
        public string Ticker { get; set; }

        ///	<summary>
        ///	Date of split
        ///	</summary>
        public DateTime Date { get; set; }

        ///	<summary>
        ///	Ratio (ie. 2:1)
        ///	</summary>
        public string Ratio
        {
            get { return _ratio; }
            set
            {
                _ratio = value.AssertNotNull().Replace('-', ':');

                string sleft = _ratio.GetLeftOf(":");
                string sright = _ratio.GetRightOf(":");

                double left = double.Parse(sleft);
                double right = double.Parse(sright);

                int length = Math.Min(sleft.Length, sright.Length);

                if (length > 1)
                {
                    double divisor = Math.Pow(10, length - 1);

                    left = Math.Round(left / divisor, 0, MidpointRounding.AwayFromZero);

                    right = Math.Round(right / divisor, 0, MidpointRounding.AwayFromZero);

                    int gcd = GCD((int)left, (int)right);

                    left = Math.Round(left / gcd, 0, MidpointRounding.AwayFromZero);

                    right = Math.Round(right / gcd, 0, MidpointRounding.AwayFromZero);

                    _ratio = $"{left}:{right}";
                }

                Adjustment = right / left;
            }
        }
        private string _ratio;

        ///	<summary>
        ///	The multiplier value (for all prices before <see cref="Date"/>)
        ///	</summary>
        public double Adjustment { get; private set; }

        #endregion
    }
}
