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
    }
}
