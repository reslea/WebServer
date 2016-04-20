using System.Configuration;

namespace HTTPWebServer
{
    public class PluginsCollection : ConfigurationElementCollection
    {
        public Plugin this[int index]
        {
            get { return (Plugin)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(Plugin plugin)
        {
            BaseAdd(plugin);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new Plugin();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Plugin)element).Extention;
        }

        public void Remove(Plugin plugin)
        {
            BaseRemove(plugin.Extention);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }
    }
}
