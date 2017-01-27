using System;
using System.IO;
using System.Net.Http;
using log4net;

namespace Ahab.Core.Services
{
    /// <summary>
    /// Represents the interface for downloading data
    /// </summary>
    public interface IDownloaderService
    {
        /// <summary>
        /// Gets the contents from a URL as a string
        /// (all contents are upper-cased for ease of use)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetString(string url);

        /// <summary>
        /// Provides a quick mechanism to cache the resulting
        /// string to a file (by default all results are cached
        /// for one week based on the previous friday from today)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileCacheName">The filename when caching contents to <see cref="Configuration.CacheDirectory"/></param>
        /// <returns></returns>
        string GetString(string url, string fileCacheName);
    }

    public class DownloaderService : IDownloaderService
    {
        #region Member

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Methods

        public string GetString(string url, string fileCacheName)
        {
            string fullpath = Path.Combine(Configuration.CacheDirectory, fileCacheName);

            string name = Path.GetFileName(fullpath);

            string contents = null;

            if (IsCacheStale(fullpath))
            {
                log.DebugFormat("Cache IsStale(or missing) File=[{0}]", name);

                contents = GetString(url);

                File.WriteAllText(fullpath, contents);
            }
            else
            {
                contents = File.ReadAllText(fullpath);
            }

            return contents;
        }

        public string GetString(string url)
        {
            try
            {
                log.DebugFormat("Downloading Url=[{0}]", url);

                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        string data = response.Content.ReadAsStringAsync().Result;

                        if (response.IsSuccessStatusCode)
                        {
                            return data.ToUpperInvariant();
                        }

                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex);

                throw;
            }
        }

        private static bool IsCacheStale(string fullpath)
        {
            if (!File.Exists(fullpath))
            {
                return true;
            }

            DateTime modified = File.GetLastWriteTime(fullpath);

            DateTime previousFriday = DateTime.Today;

            while (previousFriday.DayOfWeek != DayOfWeek.Friday)
            {
                previousFriday = previousFriday.AddDays(-1);
            }

            if (modified < previousFriday)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
