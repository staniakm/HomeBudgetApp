﻿using System;
using System.Collections.ObjectModel;

namespace Engine
{
    public class Invoice
    {
        private int invoiceId;

        public int GetInvoiceId()
        {
            return invoiceId;
        }

        public void SetInvoiceId(int value)
        {
            invoiceId = value;
        }

        public DateTime date;

        public DateTime GetDate()
        {
            return date;
        }

        public void SetDate(DateTime value)
        {
            date = value;
        }

        public string invoiceNumber;

        public string GetInvoiceNumber()
        {
            return invoiceNumber;
        }

        public void SetInvoiceNumber(string value)
        {
            invoiceNumber = value;
        }

        public string shop;

        public string GetShop()
        {
            return shop;
        }

        public void SetShop(string value)
        {
            shop = value;
        }

        public int account;

        public int GetAccount()
        {
            return account;
        }

        public void SetAccount(int value)
        {
            account = value;
        }

        public int shopId;

        public int GetShopId()
        {
            return shopId;
        }

        public void SetShopId(int value)
        {
            shopId = value;
        }

        private ObservableCollection<ParagonSzczegoly> details;

        public ObservableCollection<ParagonSzczegoly> Getdetails()
        {
            return details;
        }

        //public void Setdetails(ObservableCollection<ParagonSzczegoly> value)
        //{
        //    details1 = value;
        //}

        public Invoice()
        {
           details = new ObservableCollection<ParagonSzczegoly>();
        }


    }
}
