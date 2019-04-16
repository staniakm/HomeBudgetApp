namespace Engine
{
    public class BankAccount
    {
        public BankAccount(int iD, string name, decimal value, string description, string owner, decimal interestRate)
        {
            this.ID = iD;
            this.Name = name;
            this.Value = value;
            this.description = description;
            this.Owner = owner;
            this.InterestRate = interestRate;
        }

        public int ID { get; private set; }
        public string Name { get;  set; }
        private decimal _Value=1;
        private string description;
        private string owner;
        private decimal interestRate;



        public decimal Value
        {
            set
            {
                _Value = value;
            }

            get
            {
                return _Value;
            }
        }

        public decimal InterestRate { get { return interestRate; } set { interestRate = value; } }
        public string Owner { get { return owner; } set { owner = value; } }
        public string Opis { get { return description; } set { description = value; } }
        public void modifyAccount(string name, string opis, string owner, decimal oprocentowanie, decimal value)
        {
            this.Name = name;
            this.description = opis;
            this.owner = owner;
            this.interestRate = oprocentowanie;
            this.Value = value;
        }
    }
}
