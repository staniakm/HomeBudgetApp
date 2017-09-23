using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class ParagonSzczegoly : INotifyPropertyChanged
    {
        public int IDAso { get; set; }
        public string NazwaAso { get; set; }
        public decimal _cena;

       

        public decimal Ilosc { get; set; }
        //public decimal CenaCalosc { get { return Math.Round(Cena * Ilosc, 2); }; set; }
        public decimal CenaCalosc { get;  set; }

        public string Opis { get; set; }
        public ParagonSzczegoly(int idAso, string produkt, decimal cena, decimal ilosc, string opis = "")
        {
            IDAso = idAso;
            NazwaAso = produkt;
            Cena = cena;
            Ilosc = ilosc;
            Opis = opis;
            CenaCaloscSet();
        }

        public void CenaCaloscSet()
        {
            CenaCalosc =Math.Round(Cena * Ilosc, 2); 
        }

        public decimal Cena
        {
            get
            {
                return _cena;
            }
            set
            {
                if (_cena != value)
                {
                  
                    _cena = value;
                    OnPropertyChanged("test");

                    CenaCaloscSet();   
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
