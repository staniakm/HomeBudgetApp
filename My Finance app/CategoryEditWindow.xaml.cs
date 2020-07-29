using Engine;
using Engine.service;
using MyFinanceApp.pages;
using System.Data;
using System.Windows;
namespace MyFinanceApp
{
    /// <summary>
    /// Interaction logic for CategoryEditWindow.xaml
    /// </summary>
    public partial class CategoryEditWindow : Window
    {
        private readonly CategoryService categoryService;
        private ProductPage mw;
        private readonly int productId;

        public CategoryEditWindow(DataRowView id, CategoryService categoryService, ProductPage mw)
        {
            InitializeComponent();
            this.Top = SystemParameters.PrimaryScreenHeight/2;
            this.Left = SystemParameters.PrimaryScreenWidth/2;

            productId = (int)id[0];
            lb_id_aso.Content = id[0];
            lb_product_name.Text= id["Nazwa"].ToString();
            lb_id_kat.Content = id["id_kat"];
            lb_nazwa_kat.Content = id["Nazwa kategorii"];
            this.categoryService = categoryService;
            this.mw = mw;
            LoadCategories();
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;

        }

        private void LoadCategories()
        {
            cb_newCategory.DataContext = categoryService.GetCategoryCollection();
        }
        private void bt_zatwierdz_Click(object sender, RoutedEventArgs e)
        {
            string newProductName = lb_product_name.Text;
            string newCategoryName = cb_newCategory.Text;
            int newCategoryId = (cb_newCategory.SelectedValue == null)?-1:int.Parse(cb_newCategory.SelectedValue.ToString());

            categoryService.UpdateItemCategory(productId,newCategoryName,newCategoryId,newProductName);

            mw.LoadCategories();
            mw.GetItemsByCategory();
            this.Close();
        }
    }
}
