using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Zontik
{

    class XmlReadItem
    {
        List<Item> item;
        public XmlReadItem(string path)
        {
            try
            {
                XmlReader reader = XmlReader.Create(path);
                item = new List<Item>();
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "record"))
                    {
                        if (reader.HasAttributes)
                        {
                            Item i = new Item();
                            i.Index = Convert.ToInt32(reader.GetAttribute("index"));
                            i.City = reader.GetAttribute("city");
                            i.Lat = Convert.ToDouble(reader.GetAttribute("lat"));
                            i.Lon = Convert.ToDouble(reader.GetAttribute("lon"));
                            i.Popul = Convert.ToInt32(reader.GetAttribute("popul"));
                            item.Add(i);

                        }
                    }

                }
                reader.Close();
            }
            catch (Exception e)
            {
                ConsoleMessage.Write("Ошибка при чтении xml файла", e);
            }
        }

        public List<Item> ReturnItemList()
        {
            return item;
        }

    }


}
