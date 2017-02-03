using System;
using Ahab.Core;
using log4net.Config;

namespace Ahab.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            PortfolioSimulator ps = new PortfolioSimulator();

            PortfolioSummary summary = ps.RunSimulation(new TechnicalAnalysisPortfolioModel());

            Console.WriteLine(summary);
        }
    }
}
