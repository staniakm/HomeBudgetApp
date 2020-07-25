﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.service
{

    public class InvoiceService
    {
        private Invoice invoice;
        private SqlEngine sqlEngine;

        public InvoiceService()
        {
        }

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
            invoice = new Invoice();
            invoice.SetDate(invoiceDate);
            invoice.SetInvoiceNumber(invoiceNumber);
            invoice.SetShopId(shopId);
            invoice.SetAccount(accountId);
        }

        public ObservableCollection<InvoiceDetails> GetInvoiceItems()
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

        public void Clear()
        {
            cancelInvoice();
        }

        public void AddAutomaticInvoices()
        {
            sqlEngine.AddAutomaticInvoices();
        }

        public void SaveInvoice()
        {
            sqlEngine.SaveInvoiceInDatabase(invoice);
        }
    }
}
