using System;
using System.Configuration;
using System.IO;
using System.Threading;
using Augment;

namespace Ahab.Core
{
    public static class Configuration
    {
        static string ApplicationDirectory
        {
            get
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
        private static string _applicationDirectory;

        public static string CacheDirectory
        {
            get
            {
                if (_cacheDirectory.IsNullOrEmpty())
                {
                    string setting = ConfigurationManager.AppSettings["Ahab.CachePath"].AssertNotNull("{userprofile}");

                    if (setting.IsSameAs("{userprofile}"))
                    {
                        _cacheDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                        _cacheDirectory = Path.Combine(_cacheDirectory, "Ahab");
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
        private static string _cacheDirectory;
    }
}
