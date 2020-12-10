using System;
using Topshelf;


namespace FieldLogger.Service
{
    class Program
    {

        static void Main(string[] args)
        {
            var hostConfig = new TopShelfHostConfiguration();
            var rc = HostFactory.Run(hostConfig.Configure);
            Environment.ExitCode = (int)rc;
        }
    }


}
