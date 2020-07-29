using Engine.service;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace My_Finance_app
{
    /// <summary>
    /// Logika interakcji dla klasy BudgetPage.xaml
    /// </summary>
    public partial class BudgetPage : Page
    {
        private readonly BudgetService budgetService;
        private DateTime selectedDate = DateTime.Now;

        public BudgetPage(BudgetService budgetService)
        {
            InitializeComponent();
            this.budgetService = budgetService;
        }

        private void SetPreviousMonth(object sender, RoutedEventArgs e)
        {
            selectedDate = selectedDate.AddMonths(-1);
            lb_selectedMonth.Content = selectedDate.ToShortDateString().Substring(0, 7);
            ReloadBudgetData();
        }

        private void SetNextMonth(object sender, RoutedEventArgs e)
        {
            selectedDate = selectedDate.AddMonths(1);
            lb_selectedMonth.Content = selectedDate.ToShortDateString().Substring(0, 7);
            ReloadBudgetData();
        }

        private void RecalculateBudget(object sender, RoutedEventArgs e)
        {
            budgetService.RecalculateBudget(selectedDate);
        }

        private void EditBudget(object sender, RoutedEventArgs e)
        {
            if (dg_budzety.SelectedIndex > -1)
            {
                DataRowView dr = (DataRowView)dg_budzety.SelectedItem;
                BudgetEditWindow cw = new BudgetEditWindow(dr, budgetService, GetCurrentlySelectedDate());
                cw.ShowDialog();
            }
        }

        private void LoadBudget(object sender, RoutedEventArgs e)
        {
            ReloadBudgetData();

        }

        private void ReloadBudgetData()
        {
            var budgetData = budgetService.GetBudgetData(GetCurrentlySelectedDate());

            dg_budzety.DataContext = budgetService.GetBudgets(GetCurrentlySelectedDate());

            lb_przychodzy.Content = budgetData.Income;
            lb_wydatek.Content = budgetData.Expenses;
            lb_zaplanowane.Content = budgetData.Planned;
            lb_pozostalo.Content = budgetData.LeftToPlan < 0 ? 0.0 : budgetData.LeftToPlan;
            lb_oszczednosci.Content = budgetData.Savings;
        }

        private DateTime GetCurrentlySelectedDate()
        {
            return selectedDate;
        }
    }
}
