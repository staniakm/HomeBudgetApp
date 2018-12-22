using Engine;
using Engine.service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace My_Finance_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime selectedDate = DateTime.Now;
        private SqlEngine _sql;
        private Invoice _paragon;
        private Dictionary<string, Grid> grids = new Dictionary<string, Grid>();
        private ShopService shopService; 

        public MainWindow(SqlEngine _sql)
        {
            this._sql = _sql;
            shopService = new ShopService(_sql); ;
            InitializeComponent();
            SetupAdditionalData();
        }

        private void SetupAdditionalData()
        {
            dp_date.SelectedDate = DateTime.Now;
            PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            lb_selectedMonth.Content = selectedDate.ToShortDateString().Substring(0, 7);

            FillComboboxesWithData();

            grid_konta.DataContext = _sql.GetBankAccounts();

            //puting grids to dictionary. This will allow to switch between grids.
            LoadGrids();
            
         //   ObservableCollection<string> list = new ObservableCollection<string>();
            UpdateControlsState(true);
        }

        private void LoadGrids()
        {
            grids.Add("Paragony", grid_paragony);
            grids.Add("Asortyment", grid_asortyment);
            grids.Add("Budżet", grid_budzet);
            grids.Add("Podział na kategorie", grid_zestawienie);
            grids.Add("Standardowe zestawienie", grid_zestawienie);
            grids.Add("Konta", grid_konta);
            ShowGrid("Paragony");
        }

        private void FillComboboxesWithData()
        {
            _sql.reloadBankAccountsCollection();
            cb_shop.DataContext = _sql.GetShopsCollection();
            cb_bankAccount.DataContext = _sql.GetBankAccounts();
        }

        private void PrepareReportDetails(Reports.ReportType reportType)
        {
            dg_reports.ItemsSource = _sql.PrepareReport(reportType).DefaultView;
        }

        public void LoadCategories()
        {
            string selectedCategory = cb_kategoria.Text.Length == 0 ? "" : cb_kategoria.Text;
            try
            {
                cb_kategoria.DataContext = _sql.GetCategoryCollection();
                cb_kategoria.Text = selectedCategory;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }


        /// <summary>
        /// Loading products connected with selected store into combo 
        /// Require during adding new bill and also when adding products into store.
        /// </summary>
        private void FillComboboxWithProductsForSelectedShop(int shopId)
        {
            cb_product.DataContext = _sql.GetProductsInStore(shopId);
        }

        /// <summary>
        /// Create new bill.
        /// </summary>
        private void CreateNewInvoice(object sender, RoutedEventArgs e)
        {
            if (cb_shop.Text != "" && cb_bankAccount.Text != "")
            {
                //check if shop is on the list. If not then we add new shop and reload combo.

                //TODO: method check for shop in DB and automaticly add if shop not exists. 
                //should be split to two separated methods
                CreateShopIfNotExists();
                //Load list of products for selected shop
                int shopId = (int)cb_shop.SelectedValue;
                FillComboboxWithProductsForSelectedShop(shopId);

                //new instance of bill
                _paragon = new Invoice();
                SetInvoiceBasicValues(shopId);
                //setting ItemSource of data grid to bill details
                dg_paragony.ItemsSource = _paragon.GetInvoiceItems();
                UpdateControlsState(false);

                //set basic invoice details 
                

                //disable basic controls so basic bill data can't be changed after bill creation.
                DisableBasicInvoiceValuesFields();
            }
        }

        private void CancelCurrentInvoice(object sender, RoutedEventArgs e)
        {
            //after creation bill can be canceled.
            dg_paragony.ItemsSource = null;
            _paragon = null;

            UpdateControlsState(true);
        }

        private void DisableBasicInvoiceValuesFields()
        {
            dp_date.IsEnabled = false;
            tb_nr_paragonu.IsEnabled = false;
            cb_shop.IsEnabled = false;
            cb_bankAccount.IsEnabled = false;
        }

        private void SetInvoiceBasicValues(int shopId)
        {
            _paragon.SetDate((DateTime)dp_date.SelectedDate);//Invoice date
            _paragon.SetInvoiceNumber(tb_nr_paragonu.Text);//Invoice number
            _paragon.SetShopId(shopId);//id of shop
            _paragon.SetAccount((int)cb_bankAccount.SelectedValue);//id of bank accout 
        }

        private void CreateShopIfNotExists()
        {
            if (_sql.CreateNewShopIfNotExists(cb_shop.Text.ToUpper()) > 0)
            {
                string SelectedShop = cb_shop.Text.ToUpper();
                cb_shop.DataContext = _sql.GetShopsCollection();
                cb_shop.Text = SelectedShop;
            }
        }

        /// <summary>
        /// Ustawiamy dostępnośc kontrolek. true - dostępne sklep, konto, dodanie paragonu
        /// </summary>
        /// <param name="state"> bool </param>
        
            // TODO: refactor required
        private void UpdateControlsState(bool state)
        {
            gr_paragon.IsEnabled = state;
            if (_sql != null)
            {
                gr_produkty.IsEnabled = !state;
                mi_Zestawienia.IsEnabled = true;
                mi_Kategorie.IsEnabled = true;
                gr_paragon.IsEnabled = state;
                mi_Konta.IsEnabled = true;
            }
            else
            {
                gr_produkty.IsEnabled = false;
                mi_Zestawienia.IsEnabled = false;
                mi_Kategorie.IsEnabled = false;
            }
            if (state)
            {
                cb_shop.SelectedIndex = -1;
                cb_bankAccount.SelectedIndex = -1;
                tb_nr_paragonu.Clear();

                dp_date.IsEnabled = true;
                tb_nr_paragonu.IsEnabled = true;
                cb_shop.IsEnabled = true;
                cb_bankAccount.IsEnabled = true;
                cb_shop.IsEnabled = true;
            }
        }

        private void bt_AddNewItemToInvoice(object sender, RoutedEventArgs e)
        {
            if (cb_product.SelectedValue == null)
            {
                CreateNewProduct();
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
                    discount = CalculateDiscount(tb_discount.Text, quantity);
                    price = price - discount;
                    description = description + string.Format("\n Cena: {0} Rabat {1}", price, discount);
                }

                _paragon.AddInvoiceItem(new InvoiceDetails(productId, productName, price, quantity, description));

                l_totalPrice.Content = _paragon.GetTotalInvoiceValue();

                ClearInvoiceItemForm();
            }

        }

        private void CreateNewProduct()
        {
            string productName = cb_product.Text.ToUpper();
            _sql.AddAsoToStore(productName, _paragon.GetShopId());
            FillComboboxWithProductsForSelectedShop(_paragon.GetShopId());
            cb_product.Text = productName;
        }

        private decimal CalculateDiscount(string discount, decimal quantity)
        {
            decimal discountAmount = decimal.Parse(discount.Replace(".", ","));
            if (discountAmount != 0)
            {
                if (discountAmount < 0)
                    { discountAmount = discountAmount * (-1); }
                return Math.Round((discountAmount / quantity), 2, MidpointRounding.AwayFromZero);
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
        /// Validate that only numbers dot and coma is entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[^0-9]+");
            Regex regex = new Regex("^[-]?[0-9]*[.,]?[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        /// <summary>
        /// Save bill into database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_SaveBillInDatabase(object sender, RoutedEventArgs e)
        {
            _sql.SaveInvoiceInDatabase(_paragon);

            UpdateControlsState(true);
            dg_paragony.ItemsSource = null;
            _paragon = null;
        }

        /// <summary>
        /// Refresh bill details data grid..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDataGrid(object sender, RoutedEventArgs e)
        {
            dg_paragony.ItemsSource = null;
            dg_paragony.ItemsSource = _paragon.GetInvoiceItems();

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
            cb_report_category_collection.DataContext = _sql.GetCategoryCollection();
            cb_report_shop_collection.DataContext = _sql.GetShopsCollection();
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
                    PrepareReportDetails(Reports.ReportType.STANDARD);
                    break;
                case "KATEGORIE":
                    PrepareReportDetails(Reports.ReportType.CATEGORY);
                    break;
                case "KATEGORIE I KONTO":
                    PrepareReportDetails(Reports.ReportType.CATEGORY_AND_ACCOUNT);
                    break;
                case "LISTA PARAGONÓW":
                    PrepareReportDetails(Reports.ReportType.INVOICE_LIST);
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
            _sql.ReportSettings(settings);
        }

        public void GetItemsByCategory()
        {
            dg_asortyment.DataContext = _sql.getItemsByCategory(cb_kategoria.Text);
        }

        private void EditItemCategory(object sender, RoutedEventArgs e)
        {
            if (dg_asortyment.SelectedIndex > -1)
            {
                DataRowView dr = (DataRowView)dg_asortyment.SelectedItem;
                CategoryEditWindow cw = new CategoryEditWindow(dr, _sql, this);
                cw.ShowDialog();
            }
        }

        private void EditBudget(object sender, RoutedEventArgs e)
        {
            if (dg_budzety.SelectedIndex > -1)
            {
                DataRowView dr = (DataRowView)dg_budzety.SelectedItem;
                BudgetEditWindow cw = new BudgetEditWindow(dr, _sql, this);
                cw.ShowDialog();
            }
        }

        private void LoadCategory(object sender, RoutedEventArgs e)
        {
            GetItemsByCategory();
        }

        private void LoadBudget(object sender, RoutedEventArgs e)
        {
            double earned = _sql.GetBudgetCalculations("earned", GetCurrentlySelectedDate());
            double spend = _sql.GetBudgetCalculations("spend", GetCurrentlySelectedDate());
            double planned = _sql.GetBudgetCalculations("planed", GetCurrentlySelectedDate());
            double leftToPlan = _sql.GetBudgetCalculations("left", GetCurrentlySelectedDate());
            
            dg_budzety.DataContext = _sql.GetBudgets(GetCurrentlySelectedDate());

            lb_przychodzy.Content = earned;
            lb_wydatek.Content = spend;
            lb_zaplanowane.Content = planned;
            lb_pozostalo.Content = leftToPlan;
            lb_oszczednosci.Content = earned - spend;

        }

        public DateTime GetCurrentlySelectedDate()
        {
            return selectedDate;
        }

        private void RecalculateBudget(object sender, RoutedEventArgs e)
        {
           _sql.RecalculateBudget(selectedDate);
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
            _sql.ModifyBankAccount(tmpDic);
            //reload account collection
            _sql.reloadBankAccountsCollection();
            grid_konta.DataContext = _sql.GetBankAccounts();
            //reset selected account
            konta_cb_konto.Text = selectedAccount;
        }

        private void SetPreviousMonth(object sender, RoutedEventArgs e)
        {
            selectedDate = selectedDate.AddMonths(-1);
            lb_selectedMonth.Content = selectedDate.ToShortDateString().Substring(0,7);
            LoadBudget(null,null);
        }

        private void SetNextMonth(object sender, RoutedEventArgs e)
        {
            selectedDate = selectedDate.AddMonths(1);
            lb_selectedMonth.Content = selectedDate.ToShortDateString().Substring(0, 7);
            LoadBudget(null, null);
        }

        private void AddNewIncome(object sender, RoutedEventArgs e)
        {
            if (konta_cb_konto.Text != "") { 
            SalaryAddingWindow sw = new SalaryAddingWindow((int)konta_cb_konto.SelectedValue, _sql);
            sw.Show();
        }
        }

    }
}//ostani
