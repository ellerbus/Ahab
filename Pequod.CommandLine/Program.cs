using System;
using System.Linq;
using log4net;
using log4net.Config;
using Pequod.Core;

namespace Pequod.CommandLine
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            IDownloaderService downloader = new DownloaderService();

            IDataService ds = new DataService(downloader);

            log.Debug($"SP500.Count={ds.GetComponentsOfSp500().Count()}");

            log.Debug($"EOD.Count={ds.GetEndOfDayPrices(DateTime.Today.AddDays(-1)).Count()}");

            log.Debug($"EOD.Count={ds.GetEndOfDayPrices("WDC", DateTime.Today.AddDays(-30), DateTime.Today.AddDays(-1)).Count()}");

            log.Debug($"EOD.WDC={ds.GetEndOfDayPrices("WDC", DateTime.Today.FirstTradingDayOfMonth(DayOfWeek.Monday)).First()}");

            //var ta = new TechnicalAnalysisPortfolioModel();

            //var simulator = new PortfolioSimulator();

            //var summary = simulator.RunSimulation(ta);

            //log.Debug(summary);
        }
    }
}
