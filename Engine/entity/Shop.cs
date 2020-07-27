using System.Collections.ObjectModel;

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

    }
}

