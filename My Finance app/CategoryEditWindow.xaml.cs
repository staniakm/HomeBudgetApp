using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using Engine;
namespace My_Finance_app
{
    /// <summary>
    /// Interaction logic for CategoryEditWindow.xaml
    /// </summary>
    public partial class CategoryEditWindow : Window
    {
        SqlEngine SQL;
        MainWindow mw;
        public CategoryEditWindow(DataRowView id, SqlEngine _sql, MainWindow mw)
        {
            InitializeComponent();
            lb_id_aso.Content = id[0];
            lb_nazwa_aso.Text= id["Nazwa"].ToString();
            lb_id_kat.Content = id["id_kat"];
            lb_nazwa_kat.Content = id["Nazwa kategorii"];
            SQL = _sql;
            this.mw = mw;
            loadCategories();
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;

        }

        private void loadCategories()
        {
            cb_nowa_kat.DataContext = mw.GetCategoryCollection();
        }
        private void bt_zatwierdz_Click(object sender, RoutedEventArgs e)
        {
            //SQL.SQLexecuteNonQuerry(string.Format("update " ));
          //  Console.WriteLine("Do zmiany!!!");
            int idASO = (int)lb_id_aso.Content;
            string nowaKategoria = cb_nowa_kat.Text;
            int idKAT = (cb_nowa_kat.SelectedValue == null)?-1:int.Parse(cb_nowa_kat.SelectedValue.ToString());
            string nowaNazwa = lb_nazwa_aso.Text;

            try
            {
                SQL.SQLexecuteNonQuerry(String.Format("exec dbo.updateAsoCategory {0}, '{1}', {2}, '{3}'", idASO, nowaKategoria, idKAT, nowaNazwa));
            }
            catch (Exception)
            {

                throw;
            }

            //        Console.WriteLine(string.Format("ASO: {0}, ID_KAT {1}, Nazwa: {2}", lb_id_aso.Content, cb_nowa_kat.Text, cb_nowa_kat.SelectedValue.ToString()));
            mw.LoadCategories();
            mw.LoadCategoryData();
            this.Close();
        }
    }
}
