using Engine.service;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace My_Finance_app.pages
{
    /// <summary>
    /// Logika interakcji dla klasy AccountPage.xaml
    /// </summary>
    public partial class AccountPage : Page
    {
        private BankAccountService bankAccountService;
        private BudgetService budgetService;

        public AccountPage(BankAccountService bankAccountService, BudgetService budgetService)
        {
            InitializeComponent();
            this.bankAccountService = bankAccountService;
            this.budgetService = budgetService;

            grid_konta.DataContext = bankAccountService.GetBankAccounts();
        }

        public void ReloadAccoutDetails(string selectedAccount)
        {
            if (selectedAccount == "")
            {
                selectedAccount = konta_cb_konto.Text;
            }
            //reload account collection
            grid_konta.DataContext = bankAccountService.GetBankAccounts();
            //reset selected account
            konta_cb_konto.Text = selectedAccount;
        }

        private void AddNewIncome(object sender, RoutedEventArgs e)
        {
            if (konta_cb_konto.Text.Trim() != "")
            {
                SalaryAddingWindow sw = new SalaryAddingWindow((int)konta_cb_konto.SelectedValue, budgetService, this);
                sw.ShowDialog();
            }
        }

        private void AddNewUserAccount(object sender, RoutedEventArgs e)
        {
            accountId.Text = "";
            accountName.Text = "";
            accountMoneyAmount.Text = "";
            accountDescription.Text = "";
            accountOwner.Text = "";
            konto_procent.Text = "";
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
            bankAccountService.ModifyBankAccount(tmpDic);

            ReloadAccoutDetails(selectedAccount);
        }

        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[^0-9]+");
            Regex regex = new Regex("^[-]?[0-9]*[.,]?[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void CellNumberValidation(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[^0-9]+");
            Regex regex = new Regex("^[-]?[0-9]*[.]?[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }
    }
}
