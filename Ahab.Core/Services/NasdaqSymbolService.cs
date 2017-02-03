using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Ahab.Core.Services
{
    class NasdaqSymbolService
    {
        #region Members

        private IDownloaderService _downloader;

        private const string BaseUrl = "http://www.nasdaq.com/screening/companies-by-region.aspx?region=North+America&country=United+States&render=download";

        #endregion

        #region Constructor

        public NasdaqSymbolService(IDownloaderService downloader)
        {
            _downloader = downloader;
        }

        #endregion

        #region Symbols

        public IEnumerable<Symbol> GetAllSymbols()
        {
            string contents = _downloader.GetString(BaseUrl, "Symbols.nasdaq");

            StringReader sr = new StringReader(contents);

            CsvConfiguration cfg = new CsvConfiguration() { HasHeaderRecord = true, TrimFields = true };

            cfg.RegisterClassMap<NasdaqSymbolCsvMap>();

            CsvReader reader = new CsvReader(sr, cfg);

            while (reader.Read())
            {
                yield return reader.GetRecord<Symbol>();
            }
        }

        #endregion
    }
}
