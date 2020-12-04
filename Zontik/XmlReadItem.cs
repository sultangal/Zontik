using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;

namespace Zontik
{

    class XmlReadItem
    {
        List<Item> itemList;
        public XmlReadItem(string path)
        {
            int city_counter = 0;
            while (true) { 
                try
                {
                    XmlReader reader = XmlReader.Create(path);
                    itemList = new List<Item>();
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "record"))
                        {
                            if (reader.HasAttributes)
                            {
                                Item i = new Item();
                                i.City = reader.GetAttribute("city");
                                i.Lat = Convert.ToDouble((reader.GetAttribute("lat")).Replace(".", ","));
                                i.Lon = Convert.ToDouble((reader.GetAttribute("lon")).Replace(".", ","));
                                itemList.Add(i);
                                city_counter++;
                            }
                        }

                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    ConsoleMessage.Write("Ошибка при чтении xml файла. Попробую снова через минуту...", e);
                    Thread.Sleep(60000);
                    continue;
                }
                bool isEmpty = itemList.Any();
                if (isEmpty){ break;}
            }
        }

        public List<Item> ReturnItemList()
        {
            
            return itemList;
        }

    }


}
