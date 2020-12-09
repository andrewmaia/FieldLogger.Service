using System;
using System.Timers;
using System.IO;
using RestSharp;
using System.Data;
using MySql.Data.MySqlClient;
using System.Xml.Serialization;
using FieldLogger.Service.Xml;
using System.Text;

namespace FieldLogger.Service
{
    public class LeitorService
    {
        private readonly Timer _timer;
        public LeitorService()
        {
            _timer = new Timer(10000) {AutoReset = true};
            _timer.Elapsed += (sender, eventArgs) => {
                _timer.Stop();
                try{
                    Executar();
                }
                finally
                {
                    _timer.Start();
                }
            };
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void Executar()
        {
            string xml = ObterDados2();
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
            RestClient restClient = new RestClient("http://romiotto.dyndns.info:5000/channels.xml");
            RestRequest restRequest = new RestRequest();
            restRequest.Method = Method.GET;
            IRestResponse response = restClient.Get(restRequest);
            return response.Content;
        }

        private string ObterDados2()
        {
            RestClient restClient = new RestClient("http://romiotto.dyndns.info:5000/channels.xml");
            RestRequest restRequest = new RestRequest(Method.GET);
            IRestResponse response = restClient.Execute(restRequest);
            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
            return encoding.GetString(response.RawBytes);
        }


        private void InserirCanal(Channel channel,DateTime hora)
        {
            var connString = "Server=sql10.freesqldatabase.com;Database=sql10380992;Uid=sql10380992;Pwd=jRz9fkDAbR"; 
            var connection = new MySqlConnection(connString);
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
    }
}
