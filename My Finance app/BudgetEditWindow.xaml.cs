using Engine;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace My_Finance_app
{
    /// <summary>
    /// Interaction logic for BudgetEditWindow.xaml
    /// </summary>
    public partial class BudgetEditWindow : Window
    {
        SqlEngine SQL;
        MainWindow mw;
        int databaseBudgetRowID;
        public BudgetEditWindow(DataRowView id, SqlEngine _sql, MainWindow mw)
        {
            InitializeComponent();
            this.Top = SystemParameters.PrimaryScreenHeight / 2;
            this.Left = SystemParameters.PrimaryScreenWidth / 2;

            databaseBudgetRowID = (int)id[0];
            lb_kat.Content = id[2];
            lb_planed.Text = id[3].ToString();
            lb_used.Content = id[4];
            lb_percent_used.Content = id[5];
            SQL = _sql;
            this.mw = mw;
            //LoadCategories();
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;

        }

        //private void LoadCategories()
        //{
        //    cb_nowa_kat.DataContext = SQL.GetCategoryCollection();
        //}
        private void bt_zatwierdz_Click(object sender, RoutedEventArgs e)
        {
            //SQL.SQLexecuteNonQuerry(string.Format("update " ));
            //  Console.WriteLine("Do zmiany!!!");
            //int idASO = (int)lb_id_aso.Content;
            //string nowaKategoria = cb_nowa_kat.Text;
            //int idKAT = (cb_nowa_kat.SelectedValue == null) ? -1 : int.Parse(cb_nowa_kat.SelectedValue.ToString());
            //string nowaNazwa = lb_nazwa_aso.Text;

            string newValue = lb_planed.Text.Replace(",",".");

            try
            {
                SQL.UpdateBudget(databaseBudgetRowID, newValue);
            }
            catch (Exception)
            {
                throw;
            }
            //mw.LoadCategories();
            //mw.LoadCategoryData();

            this.Close();
        }
        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[^0-9]+");
            Regex regex = new Regex("^[-]?[0-9]*[.,]?[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }
    }
}
