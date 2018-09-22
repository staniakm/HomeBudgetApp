namespace Engine
{
    public class Category
    {
        public Category(int iD, string name)
        {
            ID = iD;
            Nazwa = name;
        }

        public int ID { get; set; }
        public string Nazwa { get; set; }

    }
}
