using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Ahab.Core.Services
{
    public class Sp500ConstituentService : ISp500ConstituentService
    {
        #region Members

        private IDownloaderService _downloader;

        private const string BaseUrl = "http://data.okfn.org/data/core/s-and-p-500-companies/r/constituents.csv";

        #endregion

        #region Constructor

        public Sp500ConstituentService(IDownloaderService downloader)
        {
            _downloader = downloader;
        }

        #endregion

        #region Symbols

        public IEnumerable<string> GetConstituents()
        {
            string contents = _downloader.GetString(BaseUrl, "Symbols.sp500");

            StringReader sr = new StringReader(contents);

            CsvConfiguration cfg = new CsvConfiguration() { HasHeaderRecord = true, TrimFields = true };

            cfg.RegisterClassMap<NasdaqSymbolCsvMap>();

            CsvReader reader = new CsvReader(sr, cfg);

            while (reader.Read())
            {
                yield return reader.GetField(0);
            }
        }

        #endregion
    }
}
