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
        public decimal Price { get => price; set => SetPrice(value) ; }
        public decimal Quantity { get => quantity; set => SetQuantity(value); }
        public decimal TotalPrice { get => totalPrice; set => totalPrice = value; }
        public string Description { get => description; set => description = value; }



        public InvoiceDetails(int idAso, string produkt, decimal cena, decimal ilosc, string opis = "")
        {
            SetIDAso(idAso);
            ProductName =produkt;
            Quantity = ilosc;
            Price = cena;
            Description = opis;
            SetTotalPrice();
        }

        public int GetIDAso()
        {
            return productId;
        }

        public void SetIDAso(int value)
        {
            productId = value;
        }

        public void SetTotalPrice()
        {
            totalPrice = (Math.Round(Price * Quantity, 2,MidpointRounding.AwayFromZero)); 
        }

        private void SetQuantity(decimal value)
        {
            if(Quantity != value)
            {
                quantity = value;
                OnPropertyChanged("quantity");
            }
        }

        private void SetPrice(decimal value)
        {
            if (Price != value)
            {
                price = value;
                
                OnPropertyChanged("price");
            }
        }

      
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            SetTotalPrice();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("totalPrice"));
        }
    }
}
