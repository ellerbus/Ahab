using Ahab.Core.Services;
using SimpleInjector;

namespace Ahab.Core
{
    public static class Injector
    {
        public static Container Register()
        {
            Container container = new Container();

            container.Register<IDownloaderService, DownloaderService>();

            container.Register<IPriceService, YahooPriceService>();

            container.Register<ISymbolService, NasdaqSymbolService>();

            container.Register<ISp500ConstituentService, Sp500ConstituentService>();

            container.Verify();

            return container;
        }
    }
}
