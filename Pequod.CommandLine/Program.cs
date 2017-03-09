using System;
using System.Linq;
using Augment;
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


            DateTime start = DateTime.Today.BeginningOfYear().AddYears(-1).FirstTradingDayOfMonth(DayOfWeek.Friday);

            DateTime end = DateTime.Today.EndOfYear().Date;

            //for (DateTime dt = start; dt < end; dt = dt.AddMonths(1))
            //{
            //    DateTime buy = dt.FirstTradingDayOfMonth(DayOfWeek.Friday);

            //    DateTime sell = buy.AddMonths(6).FirstTradingDayOfMonth(DayOfWeek.Friday).AddDays(-1);

            //    log.Debug($"{buy:yyyy-MM-dd} -> {sell:yyyy-MM-dd}");
            //}

            var mo = new TheStockSniper.Core.PortfolioModels.MomentumPortfolioModel(start);

            mo.FindSignals().ToList();
        }
    }
}
