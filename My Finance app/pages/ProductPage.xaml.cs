using Engine.service;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace My_Finance_app.pages
{
    /// <summary>
    /// Logika interakcji dla klasy ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        private CategoryService categoryService;

        public ProductPage(CategoryService categoryService)
        {
            InitializeComponent();
            this.categoryService = categoryService;
            LoadCategories();
        }

        private void EditItemCategory(object sender, RoutedEventArgs e)
        {
            if (dg_asortyment.SelectedIndex > -1)
            {
                DataRowView dr = (DataRowView)dg_asortyment.SelectedItem;
                CategoryEditWindow cw = new CategoryEditWindow(dr, categoryService, this);
                cw.ShowDialog();
            }
        }


        private void LoadCategory(object sender, RoutedEventArgs e)
        {
            GetItemsByCategory();
        }

        public void GetItemsByCategory()
        {
            dg_asortyment.DataContext = categoryService.GetItemsByCategory(cb_kategoria.Text);
        }

        public void LoadCategories()
        {
            string selectedCategory = cb_kategoria.Text.Length == 0 ? "" : cb_kategoria.Text;

            cb_kategoria.DataContext = categoryService.GetCategoryCollection();
            cb_kategoria.Text = selectedCategory;
        }
    }
}
