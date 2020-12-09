using System.Xml.Serialization;

namespace FieldLogger.Service.Xml
{
    [XmlRoot]
    public class Channel
    {
        [XmlElement]          
        public string Tag; 
        [XmlElement]          
        public double Value;
        [XmlElement]          
        public string Unit;  
        [XmlElement]          
        public string Logged;                         
                  
    }
}
