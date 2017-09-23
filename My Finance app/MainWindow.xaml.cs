using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Engine;
using System.Text.RegularExpressions;
using System.Data;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace My_Finance_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int id_zest = 1;
        SqlEngine _sql;
        Paragon _paragon;
        public MainWindow()
        {
            InitializeComponent();
            dp_data.SelectedDate = DateTime.Now;
            PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
        }
        private void ConnectDatabase(object sender, RoutedEventArgs e)
        {
            if (_sql == null)
            {
                if (cb_database.Text != "")
                {
                    _sql = new SqlEngine(cb_database.Text);
                    l_spid.Content = _sql._spid;
                    cb_database.IsEnabled = !_sql.Con;
                    button.Content = "Rozłącz";
                    LoadDataToCombo();
                   
                    UpdateControlsState(true);
                    LoadCategories();
                    try
                    {
                        _sql.SQLexecuteNonQuerry("EXEC dodaj_rachunki");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }

                    try
                    {
                        Task t = Task.Run(() =>
                        {
                            _sql.Backup();
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                }
            }
            else
            {

                Console.WriteLine("");
                _sql.Con = false;
                cb_database.IsEnabled = !_sql.Con;
                _sql = null;
                _paragon = null;
                button.Content = "Połącz";
                UpdateControlsState(false);
            }
        }

        //ustawamy context na sklepy i konto dla 1 zakładki
        private void LoadDataToCombo()
        {

            cb_sklep.DataContext = GetShopsCollection();
            cb_konto.DataContext = GetAccountColection();
        }

        /// <summary>
        /// Zestawienie - na później
        /// </summary>
        private void ZaladujZestawienie()
        {

            if (_sql.Con)
            {
                try
                {
                    dg_zestawienie.ItemsSource = _sql.ZestawienieWydatkow(id_zest).DefaultView;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        /// <summary>
        /// Zwracamy kolekcję kont. Można ustawiać bezpośrednio do datacontextu.
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<BankAccount> GetAccountColection()
        {
            ObservableCollection<BankAccount> konta = new ObservableCollection<BankAccount>();
            DataTable dt = _sql.GetTable("konta");
            foreach (DataRow item in dt.Rows)
            {
                konta.Add(new BankAccount((int)item["id"], (string)item["nazwa"]));
            }
            return konta;
        }

        /// <summary>
        /// Zwracamy kolekcję sklepów. Można bezpośrednio bindować do datacontext
        /// </summary>
        private ObservableCollection<Sklep> GetShopsCollection()
        {
            ObservableCollection<Sklep> sklepy = new ObservableCollection<Sklep>();

            DataTable dt = _sql.GetTable("sklepy");
            foreach (DataRow item in dt.Rows)
            {
                sklepy.Add(new Sklep((int)item["id"], (string)item["sklep"]));
            }
            return sklepy;
        }

        /// <summary>
        /// Zwracamy kolekcję kategorii. Można bezpośrednio bindować do datacontext
        /// </summary>
        public ObservableCollection<Category> GetCategoryCollection()
        {
            ObservableCollection<Category> kategorie = new ObservableCollection<Category>();

            DataTable dt = _sql.GetTable("kategorie");
            foreach (DataRow item in dt.Rows)
            {
                Console.WriteLine(item["id"]+" "+ item["nazwa"]);
                kategorie.Add(new Category((int)item["id"], (string)item["nazwa"]));
            }
            return kategorie;
        }

        public ObservableCollection<Asortyment> GetProductInStoreCollection(string shop)
        {
            ObservableCollection<Asortyment> ShopAso = new ObservableCollection<Asortyment>();

            DataTable dt = _sql.GetAsoList(shop);
            foreach (DataRow item in dt.Rows)
            {
                ShopAso.Add(new Asortyment((int)item["id"], (string)item["NAZWA"]));
            }
            return ShopAso;
        }

        public void LoadCategories()
        {
            string selectedCategory = cb_kategoria.Text.Length == 0 ? "wszystkie" : cb_kategoria.Text;
            try
            {
                cb_kategoria.DataContext = GetCategoryCollection();// _sql.GetData("SELECT row_number() over (order by nazwa) [licznik], nazwa, id FROM kategoria union select 0, 'wszystkie',0 order by licznik;").DefaultView;
                cb_kategoria.Text = selectedCategory;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

 
        /// <summary>
        /// Ładowanie produktów do combo.
        /// Konieczne po dodaniu paragonu a także przy dodawaniu asortymentu do sklepu.
        /// </summary>
        private void LoadProducts()
        {
            cb_product.DataContext = GetProductInStoreCollection(cb_sklep.Text);
        }

        /// <summary>
        /// Rozpoczynamy tworzenie nowego paragonu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NowyParagon(object sender, RoutedEventArgs e)
        {
            if (cb_sklep.Text != "" && cb_konto.Text != "")
            {
                //sprawdzamy czy podany sklep jest jak nie to dodajemy. Jeśli został dodany to przeładowyjemy combo sklepy
                if (_sql.ShopExists(cb_sklep.Text.ToString().ToUpper()) > 0)
                {
                    string SelectedShop = cb_sklep.Text.ToString().ToUpper();
                    cb_sklep.DataContext = GetShopsCollection();
                    cb_sklep.Text = SelectedShop;
                }
                //ładujemy aso sklepu
                LoadProducts();
                _paragon = new Paragon();
                dg_paragony.ItemsSource = _paragon.Szczegoly;
                UpdateControlsState(false);
            }
        }

        /// <summary>
        /// Ustawiamy dostępnośc kontrolek. true - dostępne sklep, konto, dodanie paragonu
        /// </summary>
        /// <param name="state"> bool </param>
        private void UpdateControlsState(bool state)
        {
            Console.WriteLine(state);
            gr_paragon.IsEnabled = state;
            if (_sql != null)
            {
                gr_produkty.IsEnabled = !state;
                mi_Zestawienia.IsEnabled = true;
                mi_Kategorie.IsEnabled = true;
                gr_paragon.IsEnabled = state;
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
            }
        }

        private void bt_dodajProdukt_Click(object sender, RoutedEventArgs e)
        {
            if (cb_product.SelectedValue == null)
            {
                string produkt_wybrany = cb_product.Text.ToString().ToUpper();
                _sql.AddAsoToStore(produkt_wybrany, cb_sklep.Text.ToString());

                LoadProducts();
                cb_product.Text = produkt_wybrany;
            }
            string produkt = cb_product.Text;
            if (tb_cena.Text != "" && tb_ilosc.Text != "")
            {
                _paragon.Szczegoly.Add(new ParagonSzczegoly((int)cb_product.SelectedValue, produkt, decimal.Parse(tb_cena.Text.Replace(".", ",")), decimal.Parse(tb_ilosc.Text.Replace(".", ",")), tb_opis.Text));
                tb_cena.Clear();
                tb_ilosc.Clear();
                cb_product.Focus();
                cb_product.SelectedIndex = -1;
            }

        }

        /// <summary>
        /// sprawdzamy poprawność pól cena i ilość.
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
        /// Zapisujemy paragon i szczegoly do bazy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZapiszParagonaWBazie(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Do POPRAWY");
            /*
             * Have to change teh way we insert data into database. Try to insert all at once
             Zmienić sposób insertu danych do bazy.
             Najlepiej jakby była możliwość insertu wszystkiego na raz.
             */

            _paragon.IdParagon = int.Parse(_sql.SQLgetScalar("exec dbo.getNextIdForParagon"));
            _paragon.Data = (DateTime)dp_data.SelectedDate;
            _paragon.NrParagonu = tb_nr_paragonu.Text;
            _paragon.IdSklep = (int)cb_sklep.SelectedValue;
            _paragon.Konto = (int)cb_konto.SelectedValue;
            _paragon.Sklep = cb_sklep.Text;
            try
            {
                
                string strCommand = String.Format("insert into paragony(nr_paragonu, data, sklep, konto, suma, opis) values ('{0}', '{1}','{2}',{3}, 0,'' )",
                                                      _paragon.NrParagonu.ToUpper(), _paragon.Data, _paragon.Sklep.ToUpper(), _paragon.Konto);
                _sql.SQLexecuteNonQuerry(strCommand);
                //dodawnie pozycji paragonu
                foreach (ParagonSzczegoly p in _paragon.Szczegoly)
                {
                    _sql.SQLexecuteNonQuerry(string.Format("insert into paragony_szczegoly(id_paragonu, cena_za_jednostke, ilosc, cena, ID_ASO, opis) values ({0}, {1},{2},{3},{4},'{5}')"
                        , _paragon.IdParagon, p.Cena.ToString().Replace(",", "."), p.Ilosc.ToString().Replace(",", "."),
                        p.CenaCalosc.ToString().Replace(",", "."), p.IDAso, p.Opis));
                }
            }
            catch (Exception)
            {

                throw;
            }

            dg_paragony.ItemsSource = null;
            _sql.PrzeliczParagon(_paragon.IdParagon);

            UpdateControlsState(true);
        }

        /// <summary>
        /// odświeżanie grida ze szczegolami paragonow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDataGrid(object sender, RoutedEventArgs e)
        {
            dg_paragony.ItemsSource = null;
            dg_paragony.ItemsSource = _paragon.Szczegoly;

        }
        /// <summary>
        /// Przełączanie między panelami
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            //  Console.WriteLine(mi.Name);

            switch (mi.Header.ToString())
            {
                case "Podział na kategorie":
                case "Standardowe zestawienie":
                    id_zest = 1;

                    grid_paragony.Visibility = Visibility.Hidden;
                    grid_zestawienie.Visibility = Visibility.Visible;
                    grid_asortyment.Visibility = Visibility.Hidden;

                    dp_dataOd_zest.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    dp_dataDo_zest.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);


                    cb_kategoria_zestawienie.DataContext = GetCategoryCollection();

                    cb_sklep_zestawienie.DataContext = GetShopsCollection();


                    if (mi.Header.ToString() == "Podział na kategorie")
                    {
                        id_zest = 2;
                    }

                    break;


                case "Paragony":
                    grid_paragony.Visibility = Visibility.Visible;
                    grid_zestawienie.Visibility = Visibility.Hidden;
                    grid_asortyment.Visibility = Visibility.Hidden;
                    break;
                case "Asortyment":
                    grid_paragony.Visibility = Visibility.Hidden;
                    grid_zestawienie.Visibility = Visibility.Hidden;
                    grid_asortyment.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void PrintConsole(string s)
        {
            Console.WriteLine(s);
        }

        private void bt_generuj_Click(object sender, RoutedEventArgs e)
        {

            //kasujemy wszystkie ustawienia dla sesji
            _sql.SQLexecuteNonQuerry(string.Format("delete from rapOrg where sesja = {0}", _sql._spid));

            if (dp_dataOd_zest.SelectedDate.ToString() != "" || dp_dataDo_zest.SelectedDate.ToString() != "")
            {
                string dataod = dp_dataOd_zest.SelectedDate.ToString() != "" ? dp_dataOd_zest.SelectedDate.ToString() : "2000-01-01";
                string datado = dp_dataDo_zest.SelectedDate.ToString() != "" ? dp_dataDo_zest.SelectedDate.ToString() : "2050-01-01";
                //ustawiamy ograniczenia dla dat parametr = 1
                _sql.SQLexecuteNonQuerry(string.Format("insert into rapOrg select '{0}', '{1}', 1 ,{2}", dataod, datado, _sql._spid));
            }
            //kategoria parametr = 2
            if ((int)cb_kategoria_zestawienie.SelectedIndex > 0)
            {
                _sql.SQLexecuteNonQuerry(string.Format("insert into rapOrg select '{0}', '', 2 ,{1}", cb_kategoria_zestawienie.SelectedValue.ToString(), _sql._spid));
            }
            //sklep parametr = 3
            if ((int)cb_sklep_zestawienie.SelectedIndex > 0)
            {
                Console.WriteLine(cb_sklep_zestawienie.SelectedValue);
                _sql.SQLexecuteNonQuerry(string.Format("insert into rapOrg select '{0}', '', 3 ,{1}", cb_sklep_zestawienie.SelectedValue.ToString(), _sql._spid));
            }
            ZaladujZestawienie();
        }


        public void LoadCategoryData()
        {

            dg_asortyment.DataContext = _sql.GetCategoryItems(cb_kategoria.Text);
        }

        private void bt_edit_category_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("zmiana kategorii");
            Console.WriteLine(dg_asortyment.SelectedIndex);
            if (dg_asortyment.SelectedIndex > -1)
            {
                DataRowView dr = (DataRowView)dg_asortyment.SelectedItem;
                CategoryEditWindow cw = new CategoryEditWindow(dr, _sql, this);
                cw.ShowDialog();
            }
        }

        private void LoadCategory(object sender, RoutedEventArgs e)
        {
            LoadCategoryData();
        }


    }
}//ostani
