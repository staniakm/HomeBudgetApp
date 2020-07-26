using System.Collections.ObjectModel;

namespace Engine
{

    public class Product
    {

        public int ID { get; set; }
        public string Nazwa { get; set; }

        public Product(int id, string nazwa)
        {
            ID = id;
            Nazwa = nazwa;
        }

    }
}
