using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class BankAccount
    {
        public BankAccount(int iD, string name, decimal value, string opis, string owner, decimal oprocentowanie)
        {
            ID = iD;
            Name = name;
            Value = value;
            Opis = opis;
            Owner = owner;
            Oprocentowanie = oprocentowanie;
        }

        public int ID { get; private set; }
        public string Name { get;  set; }
        private decimal _Value=1;// { get; set; }
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
    }
}
