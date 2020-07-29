using Engine.service;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace My_Finance_app
{
    /// <summary>
    /// Interaction logic for BudgetEditWindow.xaml
    /// </summary>
    public partial class BudgetEditWindow : Window
    {
        //private readonly SqlEngine SQL;
        private readonly BudgetService budgetService;
        private readonly int databaseBudgetRowID;
        private readonly DateTime selectedDate;

        public BudgetEditWindow(DataRowView id, BudgetService budgetService, DateTime dateTime)
        {
            InitializeComponent();
            this.selectedDate = dateTime;
            this.Top = SystemParameters.PrimaryScreenHeight / 2;
            this.Left = SystemParameters.PrimaryScreenWidth / 2;

            databaseBudgetRowID = (int)id[0];
            lb_kat.Content = id[2];
            lb_planed.Text = id[3].ToString();
            lb_used.Content = id[4];
            lb_percent_used.Content = id[5];
            this.budgetService = budgetService;

            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;

        }

        private void bt_zatwierdz_Click(object sender, RoutedEventArgs e)
        {
            string newValue = lb_planed.Text.Replace(",", ".");
            budgetService.UpdatePlannedBudget(databaseBudgetRowID, newValue, selectedDate);
            this.Close();
        }
        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[-]?[0-9]*[.,]?[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }
    }
}
