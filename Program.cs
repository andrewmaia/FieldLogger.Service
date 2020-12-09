using System;
using Topshelf;
using Microsoft.Extensions.Configuration;



namespace FieldLogger.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            //var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();
            var hostConfig = new TopShelfHostConfiguration();
            var rc = HostFactory.Run(hostConfig.Configure);
            Environment.ExitCode = (int)rc;
        }
    }


}
