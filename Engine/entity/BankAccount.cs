namespace Engine
{
    public class BankAccount
    {
        public int ID { get; private set; }
        public string Name { get; set; }
        public decimal Value { set; get; } = 1;
        public int Owner { get; set; }
        public string Description { get; set; }

        public string OwnerName { get; }

        public BankAccount(int iD, string name, decimal value, string description, int owner, string ownerName)
        {
            ID = iD;
            Name = name;
            Value = value;
            Description = description;
            Owner = owner;
            OwnerName = ownerName;
        }
    }
}
