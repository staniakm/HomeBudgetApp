using System;
using System.ComponentModel;

namespace Engine
{
    public class InvoiceDetails : INotifyPropertyChanged
    {
        private int productId;
        private string productName;
        private decimal price;
        private decimal quantity;
        private decimal totalPrice;
        private string description;

        public string ProductName { get => productName; set => productName = value; }
        public decimal Price { get => price; set => price = value; }
        public decimal Quantity { get => quantity; set => quantity = value; }
        public decimal TotalPrice { get => totalPrice; set => totalPrice = value; }
        public string Description { get => description; set => description = value; }



        public InvoiceDetails(int idAso, string produkt, decimal cena, decimal ilosc, string opis = "")
        {
            SetIDAso(idAso);
            ProductName =produkt;
            Price = cena;
            Quantity = ilosc;
            Description = opis;

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


        public decimal GetTotalPrice()
        {
            return TotalPrice;
        }

        public void SetCenaCalosc(decimal value)
        {
            TotalPrice = value;
        }



        public void setTotalPrice()
        {
            SetCenaCalosc(Math.Round(Price * Quantity, 2,MidpointRounding.AwayFromZero)); 
        }

 
        public void setPrice(decimal value)
        {
            if (Price != value)
            {
                Price = value;
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
