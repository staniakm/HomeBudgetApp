using System;
using System.Collections.ObjectModel;

namespace Engine
{
    public class Invoice
    {
        private int invoiceId;
        public DateTime date;
        public string InvoiceNumber { get; private set; }
        public string shop;
        public int account;
        public ObservableCollection<InvoiceDetails> invoiceItems;
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

        public int GetInvoiceId()
        {
            return invoiceId;
        }

        public void SetInvoiceId(int value)
        {
            invoiceId = value;
        }


        public ObservableCollection<InvoiceDetails> GetInvoiceItems()
        {
            return invoiceItems;
        }

        public void AddInvoiceItem(InvoiceDetails invoiceItem)
        {
            invoiceItems.Add(invoiceItem);
        }
    }
}
