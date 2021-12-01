﻿using Engine;
using Engine.service;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyFinanceApp.pages
{
    /// <summary>
    /// Logika interakcji dla klasy InvoicePage.xaml
    /// </summary>
    public partial class InvoicePage : Page
    {

        private Invoice invoice = null;
        private readonly InvoiceService invoiceService;
        private readonly ShopService shopService;
        private readonly BankAccountService bankAccountService;

        public InvoicePage(InvoiceService invoiceService, ShopService shopService, BankAccountService bankAccountService)
        {
            InitializeComponent();
            this.invoiceService = invoiceService;
            this.shopService = shopService;
            this.bankAccountService = bankAccountService;
            SetupAdditionalData();
        }

        /// <summary>
        /// Create or cancel new bill.
        /// </summary>
        /// 
        private void InvoiceOperation(object sender, RoutedEventArgs e)
        {
            if (InvoiceExists())
            {
                CancelCurrentInvoice();
            }
            else
            {
                CreateNewInvoice();
            }
        }

        private void Bt_AddNewItemToInvoice(object sender, RoutedEventArgs e)
        {
            if (cb_product.SelectedValue == null)
            {
                CreateNewProduct((int)cb_shop.SelectedValue);
            }
            //string productName = cb_product.Text;
            if (tb_price.Text.Trim() != "" && tb_quantity.Text.Trim() != "")
            {
                var product = (Product)cb_product.SelectedItem;
                Console.WriteLine(product.ToString());

                string description = tb_description.Text;
                decimal discount = 0.0M;
                decimal price = decimal.Parse(tb_price.Text.Replace(".", ","));
                decimal quantity = decimal.Parse(tb_quantity.Text.Replace(".", ","));

                if (tb_discount.Text != "")
                {
                    discount = CalculateDiscount(tb_discount.Text);
                }

                AddInvoiceItem(product, price, quantity, description, discount);
                l_totalPrice.Content = GetInvoiceTotalSum();
                ClearInvoiceItemForm();
            }

        }


        private void SetupAdditionalData()
        {
            dp_date.SelectedDate = DateTime.Now;
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Critical;
            //lb_selectedMonth.Content = selectedDate.ToShortDateString().Substring(0, 7);

            FillComboboxesWithData();

            //grid_konta.DataContext = bankAccountService.GetBankAccounts();

            UpdateControlsState(true);

            invoiceService.AddAutomaticInvoices();
        }
        /// <summary>
        /// Loading products connected with selected store into combo 
        /// Require during adding new bill and also when adding products into store.
        /// </summary>
        private void FillComboboxWithProductsForSelectedShop(int shopId)
        {
            cb_product.DataContext = shopService.GetProductsInStore(shopId);
        }

        private void FillComboboxesWithData()
        {
            cb_shop.DataContext = shopService.GetShopsCollection();
            cb_bankAccount.DataContext = bankAccountService.GetBankAccounts();
        }

        /// <summary>
        /// Save bill into database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bt_SaveInvoiceInDatabase(object sender, RoutedEventArgs e)
        {
            invoiceService.SaveInvoice(invoice);
            ClearInvoice();
            UpdateControlsState(true);
            dg_paragony.ItemsSource = null;

            bt_invoiceOperation.Content = "Dodaj paragon";
            l_totalPrice.Content = 0.00;
        }

        private void UpdateControlsState(bool state)
        {
            if (state)
            {
                cb_shop.SelectedIndex = -1;
                cb_bankAccount.SelectedIndex = -1;
                tb_nr_paragonu.Clear();
            }

            bt_ref.IsEnabled = !state;
            gr_produkty.IsEnabled = !state;
            gr_invoice.IsEnabled = state;
            bt_invoiceOperation.IsEnabled = true;
        }

        private void ClearInvoiceItemForm()
        {
            tb_price.Clear();
            tb_quantity.Clear();
            tb_discount.Clear();
            tb_description.Clear();
            cb_product.Focus();
            cb_product.SelectedIndex = -1;
        }
        private void CreateNewInvoice()
        {
            if (!string.IsNullOrEmpty(cb_shop.Text) && !string.IsNullOrEmpty(cb_bankAccount.Text))
            {
                
                //check if shop is on the list. If not then add new shop and reload combo.

                //TODO: method check for shop in DB and automaticly add if shop not exists. 
                //should be split to two separated methods
                CreateShopIfNotExists();


                //Load list of products for selected shop
                int shopId = (int)cb_shop.SelectedValue;

                var accountId = (int)cb_bankAccount.SelectedValue;
                var invoiceDate = (DateTime)dp_date.SelectedDate;
                var invoiceNumber = tb_nr_paragonu.Text;

                FillComboboxWithProductsForSelectedShop(shopId);

                //new instance of bill
                invoice = new Invoice(shopId, accountId, invoiceDate, invoiceNumber);
                //invoiceService.CreateNewInvoice(shopId, accountId, invoiceDate, invoiceNumber);

                //setting ItemSource of data grid to bill details
                dg_paragony.ItemsSource = GetInvoiceItems();
                l_totalPrice.Content = GetInvoiceTotalSum();
                UpdateControlsState(false);
                bt_invoiceOperation.Content = "Anuluj paragon";

            }
        }


        private decimal GetInvoiceTotalSum()
        {
            return invoice.TotalPrice();
        }

        public bool InvoiceExists()
        {
            return invoice != null;
        }

        //public void CreateNewInvoice(int shopId, int accountId, DateTime invoiceDate, string invoiceNumber)
        //{
        //    invoice = new Invoice(shopId, accountId, invoiceDate, invoiceNumber);
        //}

        private ObservableCollection<InvoiceDetails> GetInvoiceItems()
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

        /*
 * Invoice can be canceled at anytime.
 */
        private void CancelCurrentInvoice()
        {
            dg_paragony.ItemsSource = null;
            ClearInvoice();

            UpdateControlsState(true);
            bt_invoiceOperation.Content = "Dodaj paragon";

        }


        private static decimal CalculateDiscount(string discount)
        {
            decimal discountAmount = decimal.Parse(discount.Replace(".", ","));
            if (discountAmount != 0)
            {
                return Math.Abs(discountAmount);
            }
            return 0;
        }
        private void CreateShopIfNotExists()
        {
            if (shopService.CreateNewShopIfNotExists(cb_shop.Text.ToUpper()) > 0)
            {
                string SelectedShop = cb_shop.Text.ToUpper();
                cb_shop.DataContext = shopService.GetShopsCollection();
                cb_shop.Text = SelectedShop;
            }
        }

        private void CreateNewProduct(int shopId)
        {
            string productName = cb_product.Text.ToUpper();
            shopService.AddAsoToShop(productName, shopId);

            FillComboboxWithProductsForSelectedShop(shopId);
            cb_product.Text = productName;
        }



        /// <summary>
        /// Refresh bill details data grid..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDataGrid(object sender, RoutedEventArgs e)
        {
            dg_paragony.ItemsSource = null;
            dg_paragony.ItemsSource = GetInvoiceItems();

        }


        /// <summary>
        /// Validate that only numbers dot and coma is accepted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[^0-9]+");
            Regex regex = new Regex("^[-]?[0-9]*[.,]?[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void CellNumberValidation(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[^0-9]+");
            Regex regex = new Regex("^[-]?[0-9]*[.]?[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }
    }
}
