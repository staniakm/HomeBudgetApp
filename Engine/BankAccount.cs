namespace Engine
{
    public class BankAccount
    {
        public BankAccount(int iD, string name, decimal value, string opis, string owner, decimal oprocentowanie)
        {
            this.ID = iD;
            this.Name = name;
            this.Value = value;
            this.opis = opis;
            this.Owner = owner;
            this.Oprocentowanie = oprocentowanie;
        }

        public int ID { get; private set; }
        public string Name { get;  set; }
        private decimal _Value=1;
        private string opis;
        private string owner;
        private decimal oprocentowanie;



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

        public decimal Oprocentowanie { get { return oprocentowanie; } set { oprocentowanie = value; } }
        public string Owner { get { return owner; } set { owner = value; } }
        public string Opis { get { return opis; } set { opis = value; } }
        public void modifyAccount(string name, string opis, string owner, decimal oprocentowanie, decimal value)
        {
            this.Name = name;
            this.opis = opis;
            this.owner = owner;
            this.oprocentowanie = oprocentowanie;
            this.Value = value;
        }
    }
}
