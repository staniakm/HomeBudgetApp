namespace Engine
{
    public class Category
    {
        public int ID { get; set; }
        public string Nazwa { get; set; }
        public Category(int iD, string name)
        {
            ID = iD;
            Nazwa = name;
        }
    }
}
