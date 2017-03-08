using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ahab.Core
{
    public sealed class PortfolioSummary
    {
        #region Constructors

        public PortfolioSummary(IPortfolioModel model, Portfolio portfolio, IList<Signal> signals)
        {
            Model = model;
            Portfolio = portfolio;
            Signals = signals;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            StringBuilder display = new StringBuilder();

            int trxs = Portfolio.Transactions.Count;

            display
                .AppendLine()
                .Append($"Model:       {Model.GetType().Name}").AppendLine()
                .Append($"Years:        {Model.StartingDate.Year} - {Model.EndingDate.Year}").AppendLine()
                .Append($"Starting:    {Model.StartingBalance,12:N2}").AppendLine()
                .Append($"Portfolio:   {Portfolio.Balance,12:N2}").AppendLine()
                .Append($"ROI:         {Return,12:0.0%}").AppendLine()
                .Append($"CAGR:        {Cagr,12:0.0%}").AppendLine()
                .Append($"# Triggers:  {Signals.Count(),9:0}").AppendLine()
                .Append($"# + ROI:     {Signals.Count(x => x.ReturnOnSell > 0),9:0}").AppendLine()
                .Append($"# - ROI:     {Signals.Count(x => x.ReturnOnSell < 0),9:0}").AppendLine();

            return display.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The model used
        /// </summary>
        public IPortfolioModel Model { get; private set; }

        /// <summary>
        /// The portfolio account
        /// </summary>
        public Portfolio Portfolio { get; private set; }

        /// <summary>
        /// The signals produced by the model
        /// </summary>
        public IList<Signal> Signals { get; private set; }

        /// <summary>
        /// The total number of years simulated
        /// </summary>
        public int Years
        {
            get { return Model.EndingDate.Year - Model.StartingDate.Year; }
        }

        /// <summary>
        /// The total portfolio return (profit or loss)
        /// </summary>
        public double Return
        {
            get { return (Portfolio.Balance - Model.StartingBalance) / Model.StartingBalance; }
        }

        /// <summary>
        /// The annualized growth rate per year
        /// </summary>
        public double Cagr
        {
            get { return Math.Pow(Portfolio.Balance / Model.StartingBalance, 1.0 / Years) - 1; }
        }

        #endregion
    }
}
