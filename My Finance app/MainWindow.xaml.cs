using Engine;
using Engine.service;
using My_Finance_app.pages;
using System;
using System.Windows;
using System.Windows.Controls;

namespace My_Finance_app
{
    public partial class MainWindow : Window
    {
        private readonly ShopService shopService;
        private readonly BankAccountService bankAccountService;
        private readonly InvoiceService invoiceService;
        private readonly ReportService reportService;
        private readonly CategoryService categoryService;
        private readonly BudgetService budgetService;

        public MainWindow(SqlEngine sqlEngine)
        {
            shopService = new ShopService(sqlEngine);
            bankAccountService = new BankAccountService(sqlEngine);
            invoiceService = new InvoiceService(sqlEngine);
            reportService = new ReportService(sqlEngine);
            categoryService = new CategoryService(sqlEngine);
            budgetService = new BudgetService(sqlEngine);

            InitializeComponent();
            Main.Content = new InvoicePage(invoiceService, shopService, bankAccountService);
            this.Closed += new EventHandler(MainWindow_Closed);

        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Closing applicaton");
        }

        private void SwichPage(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            
            switch (mi.Name.ToString())
            {
                case "mi_budget":
                    Main.Content = new BudgetPage(budgetService);
                    break;
                case "mi_account":
                    Main.Content = new AccountPage(bankAccountService, budgetService);
                    break;
                case "mi_product":
                    Main.Content = new ProductPage(categoryService);
                    break;
                case "mi_standardReport":
                    Main.Content = new ReportPage(reportService, categoryService, shopService);
                    break;
                case "mi_invoice":
                    Main.Content = new InvoicePage(invoiceService, shopService, bankAccountService);
                    break;
            }
        }
    }
}
