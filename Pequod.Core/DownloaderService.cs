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
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<string> GetStringAsync(string url);
    }

    public class DownloaderService : IDownloaderService
    {
        #region Methods

        public async Task<string> GetStringAsync(string url)
        {
            string results = await url.GetStringAsync();

            return results;
        }

        #endregion
    }
}
