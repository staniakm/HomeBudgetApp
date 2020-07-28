using System.Data;

namespace Engine
{
    public class Category
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public Category(int iD, string name)
        {
            ID = iD;
            Name = name;
        }

        public Category(DataRow item)
        {
            ID = (int)item["id"];
            Name = (string)item["nazwa"];
        }
    }
}
