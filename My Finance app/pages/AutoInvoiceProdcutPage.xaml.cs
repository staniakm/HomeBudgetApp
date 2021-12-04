using Engine.service;
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

namespace My_Finance_app.pages
{
    /// <summary>
    /// Logika interakcji dla klasy AutoInvoiceProdcutPage.xaml
    /// </summary>
    public partial class AutoInvoiceProdcutPage : Page
    {
        private readonly InvoiceService invoiceService;
        private readonly BankAccountService bankAccountService;

        public AutoInvoiceProdcutPage()
        {
            
        }

        public AutoInvoiceProdcutPage(InvoiceService invoiceService, BankAccountService bankAccountService)
        {
            InitializeComponent();
            this.invoiceService = invoiceService;
            this.bankAccountService = bankAccountService;
            SetDataSource();
        }

        private void SetDataSource()
        {
            dg_autoInvoiceItems.ItemsSource = invoiceService.GetAutoInvoiceItemList().DefaultView;
        }

        private void AddAutoInvoiceItem(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Add new item dialog");
        }

        private void UpdateAutoinvoiceData(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Updating autoinvoice for current month");
        }

        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Delete item");
        }

        private void EditItem(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Edit item");
        }
    }
}
