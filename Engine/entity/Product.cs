using System.Collections.ObjectModel;

namespace Engine
{

    public class Product
    {

        public int ID { get;  set; }
        public string Name { get;  set; }

        public Product(int id, string name)
        {
            ID = id;
            Name = name;
        }

    }
}
