namespace Engine
{
    public class BankAccount
    {
        public int ID { get; private set; }
        public string Name { get; set; }
        public decimal Value { set; get; } = 1;
        public decimal InterestRate { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }

        public BankAccount(int iD, string name, decimal value, string description, string owner, decimal interestRate)
        {
            ID = iD;
            Name = name;
            Value = value;
            Description = description;
            Owner = owner;
            InterestRate = interestRate;
        }
    }
}
