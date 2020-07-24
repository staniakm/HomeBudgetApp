using Engine;
using Engine.service;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace My_Finance_app
{
    /// <summary>
    /// Interaction logic for SalaryAddingWindow.xaml
    /// </summary>
    public partial class SalaryAddingWindow : Window
    {
        private readonly BudgetService budgetService;
        private readonly int accountID;
        private readonly MainWindow mw;
        public SalaryAddingWindow(int accountID, BudgetService budgetService, MainWindow mainWindow)
        {
            this.budgetService = budgetService;
            this.accountID = accountID;
            this.mw = mainWindow;
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            salary_description.DataContext = budgetService.GetSallaryDescriptions();
            income_date.SelectedDate = DateTime.Now;
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
        }

        private void SaveNewIncome(object sender, RoutedEventArgs e)
        {
            string description = salary_description.Text;
            if (sallary_amount.Text != "")
            {
                decimal moneyAmount = decimal.Parse(sallary_amount.Text.Replace(".", ","));
                DateTime date = (DateTime)income_date.SelectedDate;
                budgetService.AddNewSallary(this.accountID, description, moneyAmount, date);
                mw.ReloadAccoutDetails("");
                this.Close();
            }
        }

        private void CancelIncomeAddingProcess(object sender, RoutedEventArgs e)
        {
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
