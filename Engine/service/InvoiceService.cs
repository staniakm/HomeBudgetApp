using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;

namespace Engine.service
{

    public class InvoiceService
    {
        private Invoice invoice;
        private readonly SqlEngine sqlEngine;


        public InvoiceService(SqlEngine sqlEngine)
        {
            this.sqlEngine = sqlEngine;
        }

        public bool InvoiceExists()
        {
            return invoice != null;
        }

        public void CreateNewInvoice(int shopId, int accountId, DateTime invoiceDate, string invoiceNumber)
        {
            invoice = new Invoice(shopId, accountId, invoiceDate, invoiceNumber);
        }

        public ObservableCollection<InvoiceDetails> GetInvoiceItems()
        {
            return invoice.GetInvoiceItems();
        }

        public void AddInvoiceItem(Product product, decimal price, decimal quantity, string description, decimal discount)
        {
            InvoiceDetails invoiceDetails = new InvoiceDetails(product, price, quantity, description, discount);
            invoice.AddInvoiceItem(invoiceDetails);
        }


        public void ClearInvoice()
        {
            invoice = null;
        }

        public void AddAutomaticInvoices()
        {
            sqlEngine.AddAutomaticInvoices();
        }

        public DataTable GetAutoInvoiceItemList()
        {
            return sqlEngine.GetAutoInvoiceItemList();
        }

        public void SaveInvoice()
        {
            sqlEngine.SaveInvoiceInDatabase(invoice);
            ClearInvoice();
        }

        public object GetInvoiceTotalSum()
        {
            return invoice.TotalPrice();
        }
    }
}
