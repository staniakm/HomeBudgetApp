using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Engine
{
    public class Invoice
    {
        public ObservableCollection<InvoiceDetails> invoiceItems;

        public string InvoiceNumber { get; private set; }
        public int AccountId { get; private set; }
        public DateTime InvoiceDate { get; private set; }
        public int ShopId { get; private set; }

        public Invoice(int shopId, int accountId, DateTime invoiceDate, string invoiceNumber)
        {
            invoiceItems = new ObservableCollection<InvoiceDetails>();
            ShopId = shopId;
            AccountId = accountId;
            InvoiceDate = invoiceDate;
            InvoiceNumber = invoiceNumber;
        }

        public ObservableCollection<InvoiceDetails> GetInvoiceItems()
        {
            return invoiceItems;
        }

        public void AddInvoiceItem(InvoiceDetails invoiceItem)
        {
            invoiceItems.Add(invoiceItem);
        }

        public decimal TotalPrice()
        {
            return GetInvoiceItems().Sum(item => item.TotalPrice);
        }
    }
}
