﻿using Engine;
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
        private DateTime month = DateTime.Now;
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
            dp_data.SelectedDate = DateTime.Now;
            PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            selectedMonth.Content = month.ToShortDateString().Substring(0, 7);

            LoadDataFromDB();

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

        private void LoadDataFromDB()
        {
            LoadDataToCombo();
            LoadCategories();
        }
        //create bank accounts and shop collections
        private void LoadDataToCombo()
        {
            _sql.CreateAccountColection();
            cb_sklep.DataContext = _sql.GetShopsCollection();
            cb_konto.DataContext = _sql.GetBankAccounts();
            
            grid_konta.DataContext = _sql.GetBankAccounts();
        }

        /// <summary>
        /// Create report
        /// </summary>
        private void LoadReportStatistic(int reportID)
        {
                    dg_zestawienie.ItemsSource = _sql.ZestawienieWydatkow(reportID).DefaultView;
        }

        public void LoadCategories()
        {
            string selectedCategory = cb_kategoria.Text.Length == 0 ? "wszystkie" : cb_kategoria.Text;
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
        private void LoadProducts(int shopId)
        {
            cb_product.DataContext = _sql.GetProductsInStore(shopId);
        }

        /// <summary>
        /// Create new bill.
        /// </summary>
        private void NewBill(object sender, RoutedEventArgs e)
        {
            if (cb_sklep.Text != "" && cb_konto.Text != "")
            {
                //check if shop is on the list. If not then we add new shop and reload combo.
                
                //TODO: method check for shop in DB and automaticly add if shop not exists. 
                //should be split to two separated methods
                if (_sql.ShopExists(cb_sklep.Text.ToString().ToUpper()) > 0)
                {

                    string SelectedShop = cb_sklep.Text.ToString().ToUpper();
                    cb_sklep.DataContext = _sql.GetShopsCollection();
                    cb_sklep.Text = SelectedShop;
                }
                //Load list of products for selected shop
                int shopId = (int)cb_sklep.SelectedValue;
                LoadProducts(shopId);

                //new instance of bill
                _paragon = new Invoice();
                //setting ItemSource of data grid to bill details
                dg_paragony.ItemsSource = _paragon.details;
                UpdateControlsState(false);

                //set basic bill details 
                _paragon.SetDate((DateTime)dp_data.SelectedDate);//bill date
                _paragon.SetInvoiceNumber(tb_nr_paragonu.Text);//bill number
                _paragon.SetShopId(shopId);//id of shop
                _paragon.SetAccount((int)cb_konto.SelectedValue);//id of bank accout 
                _paragon.SetShop(cb_sklep.Text);//shop name - this is not needed

                //disable basic controls so basic bill data can't be changed after bill creation.
                dp_data.IsEnabled = false;
                tb_nr_paragonu.IsEnabled = false;
                cb_sklep.IsEnabled = false;
                cb_konto.IsEnabled = false;
            }
        }


        private void CancelBill(object sender, RoutedEventArgs e)
        {
            //after creation bill can be canceled.
            dg_paragony.ItemsSource = null;
            _paragon = null;

            UpdateControlsState(true);

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
                cb_sklep.SelectedIndex = -1;
                cb_konto.SelectedIndex = -1;
                tb_nr_paragonu.Clear();

                dp_data.IsEnabled = true;
                tb_nr_paragonu.IsEnabled = true;
                cb_sklep.IsEnabled = true;
                cb_konto.IsEnabled = true;
                cb_sklep.IsEnabled = true;
            }
        }

        private void bt_AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (cb_product.SelectedValue == null)
            {
                string chosen = cb_product.Text.ToString().ToUpper();
                _sql.AddAsoToStore(chosen, cb_sklep.Text.ToString());
                int shopId = (int)cb_sklep.SelectedValue;
                LoadProducts(shopId);
                cb_product.Text = chosen;
            }
            string produkt = cb_product.Text;
            if (tb_cena.Text != "" && tb_ilosc.Text != "")
            {
                string opis = tb_opis.Text;
                decimal rabat = 0.0M;
                decimal cena = decimal.Parse(tb_cena.Text.Replace(".", ","));
                decimal ilosc = decimal.Parse(tb_ilosc.Text.Replace(".", ","));
                

                if (tb_rabat.Text != "")
                {
                    rabat = decimal.Parse(tb_rabat.Text.Replace(".", ","));
                    if (rabat != 0)
                    {
                        if (rabat < 0)
                        { rabat = rabat * (-1); }

                        opis = opis + " Cena: " + cena + " Rabat: " + rabat;
                        cena = cena - Math.Round((rabat / ilosc), 2, MidpointRounding.AwayFromZero);
                    }
                }

                _paragon.details.Add(new InvoiceDetails((int)cb_product.SelectedValue, produkt, cena, ilosc, opis));
                tb_cena.Clear();
                tb_ilosc.Clear();
                tb_rabat.Clear();
                cb_product.Focus();
                cb_product.SelectedIndex = -1;

                Console.WriteLine("number of elements "+ _paragon.Getdetails().Count);
                Console.WriteLine(  _paragon.Getdetails()[0].ProductName);
            }

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
            _sql.SaveBilInDatabase(_paragon);

            UpdateControlsState(true);
            dg_paragony.ItemsSource = null;
        }

        /// <summary>
        /// Refresh bill details data grid..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDataGrid(object sender, RoutedEventArgs e)
        {
            dg_paragony.ItemsSource = null;
            dg_paragony.ItemsSource = _paragon.details;

        }
        /// <summary>
        /// Switch between panels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            ShowGrid(mi.Header.ToString());
            switch (mi.Header.ToString())
            {
                case "Podział na kategorie":
                case "Standardowe zestawienie":
                    //id_zest = 1;

                    dp_dataOd_zest.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    dp_dataDo_zest.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
                    cb_kategoria_zestawienie.DataContext = _sql.GetCategoryCollection();
                    cb_sklep_zestawienie.DataContext = _sql.GetShopsCollection();
                    if (mi.Header.ToString() == "Podział na kategorie")
                    {
                    //    id_zest = 2;
                    }

                    break;
            }
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
        private void bt_generuj_Click(object sender, RoutedEventArgs e)
        {
            SetReportRules();
            
            switch (cb_report_type.Text.ToUpper())
            {
                case "NORMALNE":
                    LoadReportStatistic(1);
                    break;
                case "KATEGORIE":
                    LoadReportStatistic(2);
                    break;
                case "KATEGORIE I KONTO":
                    LoadReportStatistic(3);
                    break;
            }
        }


        private void SetReportRules()
        {
            var settings = new Dictionary<int, Tuple<string, string>>();

            //date settings parametr = 1
            if (dp_dataOd_zest.SelectedDate.ToString() != "" || dp_dataDo_zest.SelectedDate.ToString() != "")
            {
                string dataod = dp_dataOd_zest.SelectedDate.ToString() != "" ? dp_dataOd_zest.SelectedDate.ToString() : "2000-01-01";
                string datado = dp_dataDo_zest.SelectedDate.ToString() != "" ? dp_dataDo_zest.SelectedDate.ToString() : "2050-01-01";
                settings.Add(1, new Tuple<string, string>(dataod, datado));
            }
            //category parametr = 2
            if ((int)cb_kategoria_zestawienie.SelectedIndex > 0)
            {
                settings.Add(2, new Tuple<string, string>(cb_kategoria_zestawienie.SelectedValue.ToString(), ""));
            }
            //shop parametr = 3
            if ((int)cb_sklep_zestawienie.SelectedIndex > 0)
            {
                settings.Add(3, new Tuple<string, string>(cb_sklep_zestawienie.SelectedValue.ToString(), ""));
            }
            _sql.ReportSettings(settings);
        }

        public void LoadCategoryData()
        {
            dg_asortyment.DataContext = _sql.GetCategoryItems(cb_kategoria.Text);
        }

        private void bt_edit_category_Click(object sender, RoutedEventArgs e)
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
                Console.WriteLine(dr[0]);
                BudgetEditWindow cw = new BudgetEditWindow(dr, _sql, this);
                cw.ShowDialog();
            }
        }

        private void LoadCategory(object sender, RoutedEventArgs e)
        {
            LoadCategoryData();
        }

        private void LoadBudget(object sender, RoutedEventArgs e)
        {
            double earn = _sql.GetBudgetCalculations("earn", month);
            double spend = _sql.GetBudgetCalculations("spend", month);

            dg_budzety.DataContext = _sql.GetBudgets(month);
            lb_przychodzy.Content = earn;
            lb_pozostalo.Content = _sql.GetBudgetCalculations("left", month);
            lb_zaplanowane.Content = _sql.GetBudgetCalculations("planed", month);
            lb_wydatek.Content = spend;
            lb_oszczednosci.Content = earn - spend;

        }

        public DateTime getCurrentSelectedDate()
        {
            return month;
        }

        private void RecalculateBudget(object sender, RoutedEventArgs e)
        {
           _sql.RecalculateBudget(month);
        }

        private void br_zapisz_konto_Click(object sender, RoutedEventArgs e)
        {
            string chosen = konta_cb_konto.Text.ToString();
            Dictionary<string, string> tmpDic = new Dictionary<string, string>()
            {
                {"@nazwa", konto_nazwa.Text},
                {"@kwota", konto_kwota.Text.Replace(",",".")},
                {"@opis", konto_opis.Text },
                {"@owner", konto_owner.Text },
                {"@oprocentowanie", konto_procent.Text},
                {"@id", (konto_ID.Text.Equals("")?null:konto_ID.Text)}
            };
            //save accout
            _sql.ModifyBankAccount(tmpDic);
            //reload account collection
            _sql.CreateAccountColection();
            grid_konta.DataContext = _sql.GetBankAccounts();
            //reset selected account
            konta_cb_konto.Text = chosen;

        }

        private void previusMonth(object sender, RoutedEventArgs e)
        {
            month = month.AddMonths(-1);
            selectedMonth.Content = month.ToShortDateString().Substring(0,7);
            LoadBudget(null,null);
        }

        private void nextMonth(object sender, RoutedEventArgs e)
        {
            month = month.AddMonths(1);
            selectedMonth.Content = month.ToShortDateString().Substring(0, 7);
            LoadBudget(null, null);
        }


        private void bt_nowe_konto_Click(object sender, RoutedEventArgs e)
        {
            konto_ID.Text = "";
            konto_nazwa.Text = "";
            konto_kwota.Text = "";
            konto_opis.Text = "";
            konto_owner.Text = "";
            konto_procent.Text = "";
        }

        private void br_add_salary(object sender, RoutedEventArgs e)
        {
            if (konta_cb_konto.Text != "") { 
            SalaryAddingWindow sw = new SalaryAddingWindow((int)konta_cb_konto.SelectedValue, _sql);
            sw.Show();
        }
        }

    }
}//ostani
