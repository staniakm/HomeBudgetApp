using System.Collections.ObjectModel;

namespace Engine
{
     public class Shop {
        public int ID { get; private set; }
        public string Nazwa { get; private set; }

        public Shop(int id, string nazwa)
        {
            ID = id;
            Nazwa = nazwa;
        }

    }
}

