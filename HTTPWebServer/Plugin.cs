using System.Configuration;

namespace HTTPWebServer
{
    public class Plugin : ConfigurationSection
    {
        public Plugin() { }

        public Plugin(string name, string classType)
        {
            Extention = name;
            ClassType = classType;
        }

        [ConfigurationProperty("extention", DefaultValue = ".date", IsRequired = true, IsKey = true)]
        public string Extention
        {
            get { return (string)this["extention"]; }
            set { this["extention"] = value; }
        }

        [ConfigurationProperty("classType", DefaultValue = "DateHandler", IsRequired = true, IsKey = true)]
        public string ClassType
        {
            get { return (string)this["classType"]; }
            set { this["classType"] = value; }
        }
    }
}
