  using Topshelf;
  using Topshelf.HostConfigurators;


namespace FieldLogger.Service
{
    public class TopShelfHostConfiguration
    {
        public void Configure(HostConfigurator config)
        {
            config.Service<LeitorService>((s) =>
            {
                s.ConstructUsing(name=> new LeitorService());  
                s.WhenStarted(app => app.Start());
                s.WhenStopped(app => app.Stop());
            });

            config.RunAsLocalSystem();
            config.SetDescription("Serviço responsável por enviar para o banco de dados as leituras do Field Logger");                 
            config.SetDisplayName("FieldLogger.Service");                                  
            config.SetServiceName("FieldLogger.Service");                                
        }
    } 
}

  
