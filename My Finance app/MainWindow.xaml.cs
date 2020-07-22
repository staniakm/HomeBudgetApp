using Engine;
using Engine.service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace My_Finance_app
{
    public partial class MainWindow : Window
    {
        private DateTime selectedDate = DateTime.Now;
        private readonly SqlEngine sqlEngine;
        private Dictionary<string, Grid> grids = new Dictionary<string, Grid>();
        private InvoiceService invoiceService = new InvoiceService();

        public MainWindow(SqlEngine sqlEngine)
        {
            this.sqlEngine = sqlEngine;
            InitializeComponent();
            SetupAdditionalData();
            this.Closed += new EventHandler(MainWindow_Closed);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Closing applicaton");
            Console.WriteLine("Closing applicaton");
        }

        private void SetupAdditionalData()
        {
            dp_date.SelectedDate = DateTime.Now;
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Critical;
            lb_selectedMonth.Content = selectedDate.ToShortDateString().Substring(0, 7);

            FillComboboxesWithData();

            grid_konta.DataContext = sqlEngine.GetBankAccountsCollection();

            LoadGrids();

            UpdateControlsState(true);

            sqlEngine.AddAutomaticBills();
        }

        private void LoadGrids()
        {
            grids.Add("Paragony", grid_paragony);
            grids.Add("Asortyment", grid_asortyment);
            grids.Add("Budżet", grid_budzet);
            grids.Add("Podział na kategorie", grid_zestawienie);
            grids.Add("Standardowe zestawienie", grid_zestawienie);
            grids.Add("Konta", grid_konta);
            grids.Add("Wykresy", grid_wykresy);
            ShowGrid("Paragony");
        }

        private void FillComboboxesWithData()
        {
            cb_shop.DataContext = sqlEngine.GetShopsCollection();
            cb_bankAccount.DataContext = sqlEngine.GetBankAccountsCollection();
        }

        private void PrepareReportDetails(ReportType.Type reportType)
        {
            dg_reports.ItemsSource = sqlEngine.PrepareReport(reportType).DefaultView;
        }

        public void LoadCategories()
        {
            string selectedCategory = cb_kategoria.Text.Length == 0 ? "" : cb_kategoria.Text;
            
                cb_kategoria.DataContext = sqlEngine.GetCategoryCollection();
                cb_kategoria.Text = selectedCategory;
        }

        /// <summary>
        /// Loading products connected with selected store into combo 
        /// Require during adding new bill and also when adding products into store.
        /// </summary>
        private void FillComboboxWithProductsForSelectedShop(int shopId)
        {
            cb_product.DataContext = sqlEngine.GetProductsInStore(shopId);
        }

        /// <summary>
        /// Create or cancel new bill.
        /// </summary>
        /// 
        private void InvoiceOperation (object sender, RoutedEventArgs e)
        {
            if (invoiceService.invoiceExists() )
            {
                CancelCurrentInvoice();
            }
            else
            {
                CreateNewInvoice();
            }
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
            invoiceService.cancelInvoice();

            UpdateControlsState(true);
            bt_invoiceOperation.Content = "Dodaj paragon";
            
        }

        private void CreateShopIfNotExists()
        {
            if (sqlEngine.CreateNewShopIfNotExists(cb_shop.Text.ToUpper()) > 0)
            {
                string SelectedShop = cb_shop.Text.ToUpper();
                cb_shop.DataContext = sqlEngine.GetShopsCollection();
                cb_shop.Text = SelectedShop;
            }
        }

        /// <summary>
        /// Set availability of controls
        /// </summary>
        /// <param name="state"> bool </param>

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

        private void Bt_AddNewItemToInvoice(object sender, RoutedEventArgs e)
        {
            if (cb_product.SelectedValue == null)
            {
                CreateNewProduct((int)cb_shop.SelectedValue);
            }
            string productName = cb_product.Text;
            if (tb_price.Text.Trim() != "" && tb_quantity.Text.Trim() != "")
            {
                int productId = (int)cb_product.SelectedValue;
                string description = tb_description.Text;
                decimal discount = 0.0M;
                decimal price = decimal.Parse(tb_price.Text.Replace(".", ","));
                decimal quantity = decimal.Parse(tb_quantity.Text.Replace(".", ","));

                if (tb_discount.Text != "")
                {
                    discount = CalculateDiscount(tb_discount.Text);
                }
                
                invoiceService.addInvoiceItem(productId, productName, price, quantity, description, discount);
                
                ClearInvoiceItemForm();
            }

        }

        private void CreateNewProduct(int shopId)
        {
            string productName = cb_product.Text.ToUpper();
            sqlEngine.AddAsoToShop(productName, shopId);
            FillComboboxWithProductsForSelectedShop(shopId);
            cb_product.Text = productName;
        }

        private decimal CalculateDiscount(string discount)
        {
            decimal discountAmount = decimal.Parse(discount.Replace(".", ","));
            if (discountAmount != 0)
            {
                return Math.Abs(discountAmount);
            }
            return 0;
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

        /// <summary>
        /// Save bill into database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bt_SaveInvoiceInDatabase(object sender, RoutedEventArgs e)
        {
            sqlEngine.SaveInvoiceInDatabase(invoiceService.GetInvoice());

            UpdateControlsState(true);
            dg_paragony.ItemsSource = null;
            invoiceService.Clear();
            
            bt_invoiceOperation.Content = "Dodaj paragon";
            //l_totalPrice.Content = 0.00;
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
        /// Switch between panels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItems(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            ShowGrid(mi.Header.ToString());
            switch (mi.Header.ToString())
            {
                case "Standardowe zestawienie":
                    LoadReportInitValues();
                    break;
                case "Asortyment":
                    LoadCategories();
                    break;
            }
        }

        private void LoadReportInitValues()
        {
            dp_report_start_date.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dp_report_end_date.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
            cb_report_category_collection.DataContext = sqlEngine.GetCategoryCollection();
            cb_report_shop_collection.DataContext = sqlEngine.GetShopsCollection();
        }

        private void ShowGrid(string v)
        {
            foreach (string s in grids.Keys)
            {
                grids[s].Visibility = Visibility.Hidden;
            }
            grids[v].Visibility = Visibility.Visible;
        }

        private void PrintConsole(string s)
        {
            Console.WriteLine(s);
        }

        /// <summary>
        /// Create session settings for specific report.
        /// </summary>
        private void GenerateReport(object sender, RoutedEventArgs e)
        {
            PrepareReportSettings();

            switch (cb_report_type.Text.ToUpper())
            {
                case "NORMALNE":
                    PrepareReportDetails(ReportType.Type.STANDARD);
                    break;
                case "KATEGORIE":
                    PrepareReportDetails(ReportType.Type.CATEGORY);
                    break;
                case "KATEGORIE I KONTO":
                    PrepareReportDetails(ReportType.Type.CATEGORY_AND_ACCOUNT);
                    break;
                case "LISTA PARAGONÓW":
                    PrepareReportDetails(ReportType.Type.INVOICE_LIST);
                    break;
                default:
                    break;
            }
        }


        private void PrepareReportSettings()
        {
            var settings = new Dictionary<int, Tuple<string, string>>();

            //date settings parametr = 1
            if (dp_report_start_date.SelectedDate.ToString() != "" || dp_report_end_date.SelectedDate.ToString() != "")
            {
                string startDate = dp_report_start_date.SelectedDate.ToString() != "" ? dp_report_start_date.SelectedDate.ToString() : "2000-01-01";
                string endDate = dp_report_end_date.SelectedDate.ToString() != "" ? dp_report_end_date.SelectedDate.ToString() : "2050-01-01";
                settings.Add(1, new Tuple<string, string>(startDate, endDate));
            }
            //category parametr = 2

            if ((int)cb_report_category_collection.SelectedIndex > 0)
            {
                settings.Add(2, new Tuple<string, string>(cb_report_category_collection.SelectedValue.ToString(), ""));
            }
            //shop parametr = 3
            if ((int)cb_report_shop_collection.SelectedIndex > 0)
            {
                settings.Add(3, new Tuple<string, string>(cb_report_shop_collection.SelectedValue.ToString(), ""));
            }
            sqlEngine.ReportSettings(settings);
        }

        public void GetItemsByCategory()
        {
            dg_asortyment.DataContext = sqlEngine.GetItemsByCategory(cb_kategoria.Text);
        }

        private void EditItemCategory(object sender, RoutedEventArgs e)
        {
            if (dg_asortyment.SelectedIndex > -1)
            {
                DataRowView dr = (DataRowView)dg_asortyment.SelectedItem;
                CategoryEditWindow cw = new CategoryEditWindow(dr, sqlEngine, this);
                cw.ShowDialog();
            }
        }

        private void EditBudget(object sender, RoutedEventArgs e)
        {
            if (dg_budzety.SelectedIndex > -1)
            {
                DataRowView dr = (DataRowView)dg_budzety.SelectedItem;
                BudgetEditWindow cw = new BudgetEditWindow(dr, sqlEngine, this);
                cw.ShowDialog();
            }
        }

        private void LoadCategory(object sender, RoutedEventArgs e)
        {
            GetItemsByCategory();
        }

        private void LoadBudget(object sender, RoutedEventArgs e)
        {
            double earned = sqlEngine.GetBudgetCalculations("earned", GetCurrentlySelectedDate());
            double spend = sqlEngine.GetBudgetCalculations("spend", GetCurrentlySelectedDate());
            double planned = sqlEngine.GetBudgetCalculations("planed", GetCurrentlySelectedDate());
            double leftToPlan = sqlEngine.GetBudgetCalculations("left", GetCurrentlySelectedDate());

            dg_budzety.DataContext = sqlEngine.GetBudgets(GetCurrentlySelectedDate());

            lb_przychodzy.Content = earned;
            lb_wydatek.Content = spend;
            lb_zaplanowane.Content = planned;
            lb_pozostalo.Content = leftToPlan < 0 ? 0.0 : leftToPlan;
            lb_oszczednosci.Content = earned - spend;

        }

        public DateTime GetCurrentlySelectedDate()
        {
            return selectedDate;
        }

        private void RecalculateBudget(object sender, RoutedEventArgs e)
        {
            sqlEngine.RecalculateBudget(selectedDate);
        }

        private void SaveAccount(object sender, RoutedEventArgs e)
        {
            string selectedAccount = konta_cb_konto.Text;
            Dictionary<string, string> tmpDic = new Dictionary<string, string>()
            {
                {"@nazwa", accountName.Text},
                {"@kwota", accountMoneyAmount.Text.Replace(",",".")},
                {"@opis", accountDescription.Text },
                {"@owner", accountOwner.Text },
                {"@oprocentowanie", konto_procent.Text},
                {"@id", (accountId.Text.Equals("")?null:accountId.Text)}
            };
            //save accout
            sqlEngine.ModifyBankAccount(tmpDic);
            ReloadAccoutDetails(selectedAccount);
        }

        public void ReloadAccoutDetails(string selectedAccount)
        {
            if (selectedAccount == "")
            {
                selectedAccount = konta_cb_konto.Text;
            }
            //reload account collection
            sqlEngine.GetBankAccountsCollection();
            grid_konta.DataContext = sqlEngine.GetBankAccountsCollection();
            //reset selected account
            konta_cb_konto.Text = selectedAccount;
        }

        private void SetPreviousMonth(object sender, RoutedEventArgs e)
        {
            selectedDate = selectedDate.AddMonths(-1);
            lb_selectedMonth.Content = selectedDate.ToShortDateString().Substring(0, 7);
            LoadBudget(null, null);
        }

        private void SetNextMonth(object sender, RoutedEventArgs e)
        {
            selectedDate = selectedDate.AddMonths(1);
            lb_selectedMonth.Content = selectedDate.ToShortDateString().Substring(0, 7);
            LoadBudget(null, null);
        }

        private void AddNewIncome(object sender, RoutedEventArgs e)
        {
            if (konta_cb_konto.Text != "")
            {
                SalaryAddingWindow sw = new SalaryAddingWindow((int)konta_cb_konto.SelectedValue, sqlEngine, this);
                sw.ShowDialog();
            }
        }

        private void AddNewUserAccount(object sender, RoutedEventArgs e)
        {
            accountId.Text = "";
            accountName.Text = "";
            accountMoneyAmount.Text = "";
            accountDescription.Text = "";
            accountOwner.Text = "";
            konto_procent.Text = "";
        }

    }
}//ostani
