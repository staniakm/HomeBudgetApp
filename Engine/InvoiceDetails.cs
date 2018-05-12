using System;
using System.ComponentModel;

namespace Engine
{
    public class InvoiceDetails : INotifyPropertyChanged
    {
        private int productId;
        private string productName;
        public decimal price;
        private decimal quantity;
        private decimal totalPrice;
        private string description;


        public InvoiceDetails(int idAso, string produkt, decimal cena, decimal ilosc, string opis = "")
        {
            SetIDAso(idAso);
            SetNazwaAso(produkt);
            price = cena;
            SetIlosc(ilosc);
            SetOpis(opis);

            setTotalPrice();
        }

        public int GetIDAso()
        {
            return productId;
        }

        public void SetIDAso(int value)
        {
            productId = value;
        }

        public string GetNazwaAso()
        {
            return productName;
        }

        public void SetNazwaAso(string value)
        {
            productName = value;
        }

        public decimal GetIlosc()
        {
            return quantity;
        }

        public void SetIlosc(decimal value)
        {
            quantity = value;
        }


        public decimal GetCenaCalosc()
        {
            return totalPrice;
        }

        public void SetCenaCalosc(decimal value)
        {
            totalPrice = value;
        }


        public string GetOpis()
        {
            return description;
        }

        public void SetOpis(string value)
        {
            description = value;
        }


        public void setTotalPrice()
        {
            SetCenaCalosc(Math.Round(price * quantity, 2,MidpointRounding.AwayFromZero)); 
        }

        public decimal getPrice()
        {
            return price;
        }

        public void setPrice(decimal value)
        {
            if (price != value)
            {
                price = value;
                OnPropertyChanged("test");
                setTotalPrice();
            }
        }

      
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
