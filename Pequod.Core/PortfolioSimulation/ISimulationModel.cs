using System;
using System.Collections.Generic;
using Pequod.Core.Models;

namespace Pequod.Core.PortfolioSimulation
{
    public interface ISimulationModel
    {
        /// <summary>
        /// When implemented determines which symbols are a buy/sell.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Signal> FindSignals();

        /// <summary>
        /// Determine the total number of shares to purchase
        /// </summary>
        /// <param name="price"></param>
        /// <param name="balance">Current portfolio balance</param>
        /// <returns>Return less than 1 to ignore the request</returns>
        int GetSharesToPurchase(double price, double balance);

        /// <summary>
        /// Starting balance for this simulation
        /// </summary>
        double StartingBalance { get; }

        /// <summary>
        /// Commission costs per transaction
        /// </summary>
        double Commission { get; }

        /// <summary>
        /// Starting date for this simulation
        /// </summary>
        DateTime StartingDate { get; }

        /// <summary>
        /// Ending date for this simulation (today by default)
        /// </summary>
        DateTime EndingDate { get; }
    }
}
