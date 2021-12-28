using System.Collections.ObjectModel;

namespace Engine
{

    public class Product
    {

        public int ID { get;  set; }
        public string Name { get;  set; }

        public int CategoryId { get; set; }

        public Product(int id, string name, int categoyId)
        {
            ID = id;
            Name = name;
            CategoryId = categoyId;
        }

    }
}
