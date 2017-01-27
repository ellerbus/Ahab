using CsvHelper.Configuration;

namespace Ahab.Core.Services
{
    class YahooPriceCsvMap : CsvClassMap<Price>
    {
        public YahooPriceCsvMap()
        {
            //Date,Open,High,Low,Close,Volume,Adj Close
            Map(x => x.Date).Name("DATE", "Date");
            Map(x => x.Open).Name("OPEN", "Open");
            Map(x => x.High).Name("HIGH", "High");
            Map(x => x.Low).Name("LOW", "Low");
            Map(x => x.Close).Name("CLOSE", "Close");
            Map(x => x.Volume).Name("VOLUME", "Volume");
            Map(x => x.AdjustedClose).Name("ADJ CLOSE", "Adj Close");

        }
    }
}
