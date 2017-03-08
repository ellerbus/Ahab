using System;
using System.IO;
using System.Threading.Tasks;
using Flurl.Http;

namespace Pequod.Core
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
        Task<string> GetStringAsync(string url);

        /// <summary>
        /// Provides a quick mechanism to cache the resulting
        /// string to a file (by default all results are cached
        /// for one week based on the previous friday from today)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileCacheName">The filename when caching contents to <see cref="Configuration.CacheDirectory"/></param>
        /// <returns></returns>
        Task<string> GetStringAsync(string url, string fileCacheName);
    }

    public class DownloaderService : IDownloaderService
    {
        #region Methods

        public async Task<string> GetStringAsync(string url, string fileCacheName)
        {
            string fullpath = Path.Combine(Configuration.CacheDirectory, fileCacheName);

            string name = Path.GetFileName(fullpath);

            string contents = null;

            if (IsCacheStale(fullpath))
            {
                contents = await GetStringAsync(url);

                using (StreamWriter sw = new StreamWriter(fullpath))
                {
                    await sw.WriteAsync(contents);
                }
            }
            else
            {
                contents = File.ReadAllText(fullpath);

                using (StreamReader sr = new StreamReader(fullpath))
                {
                    contents = await sr.ReadToEndAsync();
                }
            }

            return contents;
        }

        public async Task<string> GetStringAsync(string url)
        {
            string results = await url.GetStringAsync();

            return results;
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
