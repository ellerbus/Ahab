using System;
using System.Configuration;
using System.IO;
using System.Threading;
using Augment;
using EnsureThat;

namespace Pequod.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class Configuration
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// 
        /// </summary>
        static string ApplicationDirectory
        {
            get
            {
                lock (_lock)
                {
                    if (_applicationDirectory.IsNullOrEmpty())
                    {
                        if (Thread.GetDomain().GetData(".appPath") == null)
                        {
                            _applicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        }
                        else
                        {
                            _applicationDirectory = Thread.GetDomain().GetData(".appPath") as string;
                        }

                        if (_applicationDirectory.EndsWith(@"\"))
                        {
                            _applicationDirectory = _applicationDirectory.Substring(0, _applicationDirectory.Length - 1);
                        }
                    }

                    return _applicationDirectory;
                }
            }
        }
        private static string _applicationDirectory;

        /// <summary>
        /// 
        /// </summary>
        public static string CacheDirectory
        {
            get
            {
                lock (_lock)
                {
                    if (_cacheDirectory.IsNullOrEmpty())
                    {
                        string setting = ConfigurationManager.AppSettings["Pequod.CachePath"].AssertNotNull("{userprofile}");

                        if (setting.IsSameAs("{userprofile}"))
                        {
                            _cacheDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                            _cacheDirectory = Path.Combine(_cacheDirectory, "Pequod");
                        }
                        else if (setting.IndexOf("{application}", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            _cacheDirectory = setting.Replace("{application}", ApplicationDirectory);
                        }
                        else
                        {
                            _cacheDirectory = Path.GetFullPath(setting);
                        }

                        if (_cacheDirectory.EndsWith(@"\"))
                        {
                            _cacheDirectory = _cacheDirectory.Substring(0, _cacheDirectory.Length - 1);
                        }

                        if (!Directory.Exists(_cacheDirectory))
                        {
                            Directory.CreateDirectory(_cacheDirectory);
                        }
                    }
                    return _cacheDirectory;
                }
            }
        }
        private static string _cacheDirectory;

        /// <summary>
        /// 
        /// </summary>
        public static string QuanDlApiKey
        {
            get
            {
                lock (_lock)
                {
                    if (_quanDlApiKey.IsNullOrEmpty())
                    {
                        _quanDlApiKey = ConfigurationManager.AppSettings["QuanDL.ApiKey"];
                    }

                    Ensure.That(_quanDlApiKey, "QuanDL.ApiKey")
                        .WithExtraMessageOf(() => "QuanDL.ApiKey AppSetting is missing")
                        .IsNotNullOrWhiteSpace();

                    return _quanDlApiKey;
                }
            }
        }
        private static string _quanDlApiKey;
    }
}
