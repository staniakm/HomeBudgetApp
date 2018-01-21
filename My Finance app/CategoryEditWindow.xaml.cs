using Engine;
using System.Data;
using System.Windows;
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
            this.Top = SystemParameters.PrimaryScreenHeight/2;
            this.Left = SystemParameters.PrimaryScreenWidth/2;
  
            lb_id_aso.Content = id[0];
            lb_nazwa_aso.Text= id["Nazwa"].ToString();
            lb_id_kat.Content = id["id_kat"];
            lb_nazwa_kat.Content = id["Nazwa kategorii"];
            SQL = _sql;
            this.mw = mw;
            LoadCategories();
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;

        }

        private void LoadCategories()
        {
            cb_nowa_kat.DataContext = SQL.GetCategoryCollection();
        }
        private void bt_zatwierdz_Click(object sender, RoutedEventArgs e)
        {
            int idASO = (int)lb_id_aso.Content;
            string nowaKategoria = cb_nowa_kat.Text;
            int idKAT = (cb_nowa_kat.SelectedValue == null)?-1:int.Parse(cb_nowa_kat.SelectedValue.ToString());
            string nowaNazwa = lb_nazwa_aso.Text;

            SQL.UpdateCategoryOfAso(idASO,nowaKategoria,idKAT,nowaNazwa);

            mw.LoadCategories();
            mw.LoadCategoryData();
            this.Close();
        }
    }
}
