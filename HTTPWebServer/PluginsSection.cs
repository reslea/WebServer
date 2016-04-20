using System.Configuration;

namespace HTTPWebServer
{
    public class PluginsSection : ConfigurationSection
    {
        [ConfigurationProperty("Plugins", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(PluginsCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public PluginsCollection Plugins
        {
            get
            {
                return base["Plugins"] as PluginsCollection;
            }
        }
    }
}
