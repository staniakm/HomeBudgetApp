using Engine;
using Engine.service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace My_Finance_app.pages
{
    /// <summary>
    /// Logika interakcji dla klasy InvoicePage.xaml
    /// </summary>
    public partial class InvoicePage : Page
    {
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

                invoiceService.AddInvoiceItem(product, price, quantity, description, discount);

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
            invoiceService.SaveInvoice();

            UpdateControlsState(true);
            dg_paragony.ItemsSource = null;
            invoiceService.Clear();

            bt_invoiceOperation.Content = "Dodaj paragon";
            //l_totalPrice.Content = 0.00;
        }
        private void UpdateControlsState(bool state)
        {
            if (state)
            {
                cb_shop.SelectedIndex = -1;
                cb_bankAccount.SelectedIndex = -1;
                tb_nr_paragonu.Clear();
            }

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
                FillComboboxWithProductsForSelectedShop(shopId);

                //new instance of bill
                invoiceService.CreateNewInvoice(shopId, (int)cb_bankAccount.SelectedValue, (DateTime)dp_date.SelectedDate, tb_nr_paragonu.Text);

                //setting ItemSource of data grid to bill details
                dg_paragony.ItemsSource = invoiceService.GetInvoiceItems();
                UpdateControlsState(false);
                bt_invoiceOperation.Content = "Anuluj paragon";

            }
        }

        /*
 * Invoice can be canceled at anytime.
 */
        private void CancelCurrentInvoice()
        {
            dg_paragony.ItemsSource = null;
            invoiceService.CancelInvoice();

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
        /// Create or cancel new bill.
        /// </summary>
        /// 
        private void InvoiceOperation(object sender, RoutedEventArgs e)
        {
            if (invoiceService.InvoiceExists())
            {
                CancelCurrentInvoice();
            }
            else
            {
                CreateNewInvoice();
            }
        }

        /// <summary>
        /// Refresh bill details data grid..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDataGrid(object sender, RoutedEventArgs e)
        {
            dg_paragony.ItemsSource = null;
            dg_paragony.ItemsSource = invoiceService.GetInvoiceItems();

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
