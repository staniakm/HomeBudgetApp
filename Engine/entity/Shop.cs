using System.Collections.ObjectModel;
using System.Data;

namespace Engine
{
     public class Shop {

        public int ID { get; private set; }
        public string Name { get; private set; }

        public Shop(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public Shop(DataRow item)
        {
            ID = (int)item["id"];
            Name = (string)item["name"];
        }
    }
}

