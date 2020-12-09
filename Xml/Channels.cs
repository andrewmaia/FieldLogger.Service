using System.Xml.Serialization;
using System.Collections;

namespace FieldLogger.Service.Xml
{
    [XmlRoot]
    public class Channels
    {
        [XmlElement]
        public string DeviceTag;
        [XmlElement]        
        public string SerialNumber; 
        [XmlElement]        
        public int EnabledAnalogChannels;
        [XmlElement]        
        public int EnabledDigitalChannels;
        [XmlElement]        
        public int EnabledRemoteChannels;
        [XmlElement]        
        public int EnabledVirtualChannels;    
        [XmlArray]    
        public Channel[] AnalogChannels;                       
    }
}
