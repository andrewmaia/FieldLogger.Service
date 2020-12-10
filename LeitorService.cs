using System;
using System.Timers;
using System.IO;
using RestSharp;
using System.Data;
using MySql.Data.MySqlClient;
using System.Xml.Serialization;
using FieldLogger.Service.Xml;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FieldLogger.Service
{
    public class LeitorService
    {
        private readonly string pathExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private  IConfigurationRoot _configuration;
        private readonly Timer _timer;
        public LeitorService()
        {
            try
            {
                CarregarConfig();
                int tempoExecucaoServicoEmMinutos = int.Parse(_configuration["TempoExecucaoServicoEmMinutos"].ToString());
                int tempoExecucaoEmMiliSegundos= tempoExecucaoServicoEmMinutos * 60000;
                _timer = new Timer(tempoExecucaoEmMiliSegundos) {AutoReset = true};
                _timer.Elapsed += Timer_Elapsed;
                GravarLog($"Serviço iniciado às {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
            }
            catch(Exception ex)
            {
                GravarLogErro(ex.Message);
            }
        }

        public void Start()
        {
            if(_timer!=null)
                _timer.Start();
        }

        public void Stop()
        {
            if(_timer!=null)            
                _timer.Stop();
        }

        private void Timer_Elapsed(object sender, EventArgs eventArgs)
        {
                _timer.Stop();
                try{
  
                    Executar();
                }
                catch(Exception ex)
                {
                    GravarLogErro(ex.Message);

                }
                finally
                {
                    _timer.Start();
                }
        }

        private void Executar()
        {
            string xml = ObterDados();
            var serializer = new XmlSerializer(typeof(Channels));

            using (TextReader reader = new StringReader(xml))
            {
                Channels channels = (Channels)serializer.Deserialize(reader);
                DateTime hora= DateTime.Now;
                foreach(Channel channel in channels.AnalogChannels)
                    InserirCanal(channel, hora);
            }
        }

        private string ObterDados()
        {
            RestClient restClient = new RestClient(_configuration["FieldLoggerConnection"]);
            RestRequest restRequest = new RestRequest("channels.xml",Method.GET);
            IRestResponse response = restClient.Execute(restRequest);
            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
            return encoding.GetString(response.RawBytes);
        }


        private void InserirCanal(Channel channel,DateTime hora)
        {
            var connection = new MySqlConnection(_configuration["ConnectionStrings:MySqlConnection"]);
            var command = connection.CreateCommand();
            command.CommandText = $"Insert Into Channel (Tag,Value,Unit,Logged,Date) Values (@Tag,@Value,@Unit,@Logged,@Date)";
            command.Parameters.AddWithValue("@Tag",channel.Tag);
            command.Parameters.AddWithValue("@Value",channel.Value);
            command.Parameters.AddWithValue("@Unit",channel.Unit);
            command.Parameters.AddWithValue("@Logged",channel.Logged);                        
            command.Parameters.AddWithValue("@Date",hora); 

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                if(connection.State == ConnectionState.Open)
                    connection.Close();            
            }
        }

        private void CarregarConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(pathExe)
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();            
        }

        private void GravarLogErro(string mensagem)
        {
            GravarLog("Erro:"+mensagem );
        }
        private void GravarLog(string mensagem)
        {
            string pathLog= Path.Combine(pathExe,"log.txt");
            using (StreamWriter log = File.AppendText(pathLog))
            {
                log.WriteLine(mensagem);
            }
        }
    }
}
