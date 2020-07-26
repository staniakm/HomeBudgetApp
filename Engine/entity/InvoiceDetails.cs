using System;
using System.ComponentModel;

namespace Engine
{
    public class InvoiceDetails : INotifyPropertyChanged
    {
        private readonly Product product;
        private decimal price;
        private decimal quantity;
        private decimal discount;

        public string ProductName { get => product.Nazwa ; private set => product.Nazwa = value; }
        public decimal Price { get => price; set => SetPrice(value) ; }
        public decimal Quantity { get => quantity; set => SetQuantity(value); }
        public decimal Discount { get => discount; set => SetDiscount(value); }
        public decimal TotalPrice { get; set; }
        public string Description { get; set; }



        public InvoiceDetails(Product product, decimal cena, decimal ilosc, string opis = "", decimal discount=0.0M)
        {
            this.product = product;
            Quantity = ilosc;
            Price = cena;
            Discount = discount;
            Description = opis;
            SetTotalPrice();
        }

        public void SetTotalPrice()
        {
            TotalPrice = (Math.Round(Price * Quantity, 2,MidpointRounding.AwayFromZero)) - Discount; 
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

        private void SetDiscount(decimal value)
        {
            if (Discount != value)
            {
                discount = value;

                OnPropertyChanged("discount");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            SetTotalPrice();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("totalPrice"));
        }

        internal int GetIDAso()
        {
            return product.ID;
        }
    }
}
