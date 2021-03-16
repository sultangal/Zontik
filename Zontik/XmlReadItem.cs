using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Globalization;

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
                    using XmlReader reader = XmlReader.Create(path);
                    itemList = new List<Item>();
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "record"))
                        {
                            if (reader.HasAttributes)
                            {
                                NumberFormatInfo numberFormat = new NumberFormatInfo();
                                numberFormat.NumberDecimalSeparator = ".";

                                Item i = new Item();
                                i.City = reader.GetAttribute("city");
                                string latStr = reader.GetAttribute("lat").Replace(",", ".");
                                i.Lat = Math.Round(Convert.ToDecimal(latStr, numberFormat),14);
                                string lonStr = reader.GetAttribute("lon").Replace(",", ".");
                                i.Lon = Math.Round(Convert.ToDecimal(lonStr, numberFormat),14);
                                itemList.Add(i);
                                city_counter++;
                            }
                        }

                    }
                    reader.Close();                    
                    if (itemList.Any()) { break; }
                }
                catch (Exception e)
                {
                    LogMessage.Write("Ошибка при чтении xml файла. Попробую снова через минуту...", e);
                    Thread.Sleep(60000);
                    continue;
                }
                
            }
        }

        public List<Item> ReturnItemList()
        {
            
            return itemList;
        }

    }


}
