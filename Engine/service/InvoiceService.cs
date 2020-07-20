using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.service
{



    public class InvoiceService
    {
        private Invoice invoice;

        public InvoiceService()
        {
        }

        public bool invoiceExists()
        {
            return invoice != null;
        }

        public void CreateNewInvoice(int shopId, int accountId, DateTime invoiceDate, string invoiceNumber)
        {
            invoice = new Invoice();
            invoice.SetDate(invoiceDate);
            invoice.SetInvoiceNumber(invoiceNumber);
            invoice.SetShopId(shopId);
            invoice.SetAccount(accountId);
        }

        public IEnumerable GetInvoiceItems()
        {
            return invoice.GetInvoiceItems();
        }

        public void addInvoiceItem(int productId, string productName, decimal price, decimal quantity, string description, decimal discount)
        {
            InvoiceDetails invoiceDetails = new InvoiceDetails(productId, productName, price, quantity, description, discount);
            invoice.AddInvoiceItem(invoiceDetails);
        }

        public void cancelInvoice()
        {
            invoice = null;
        }

        public Invoice GetInvoice()
        {
            return invoice;
        }

        public void Clear()
        {
            cancelInvoice();
        }
    }
}
