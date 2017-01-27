using System;
using System.Reflection;
using Ahab.Core.Services;
using log4net;
using log4net.Config;

namespace Ahab.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            SimpleMovingAverageSimulation m = new SimpleMovingAverageSimulation(new DateTime(2007, 6, 1));

            m.RunSimulation();
        }
    }
}
