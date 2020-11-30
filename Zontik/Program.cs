using System;
using Topshelf;

namespace Zontik
{
    class Program
    {

        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<ZontikMainService>(s =>
                {
                    s.ConstructUsing(zontikMainService => new ZontikMainService());
                    s.WhenStarted(zontikMainService => zontikMainService.OnStart());
                    s.WhenStopped(zontikMainService => zontikMainService.OnStop());                    
                });

                x.RunAsLocalSystem();
                
                x.SetServiceName("Zontik");
                x.SetDisplayName("Zontik");
                x.SetDescription("Служба для парсинга данных сервера погоды и дальнейшей передачи в VizEngine");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
