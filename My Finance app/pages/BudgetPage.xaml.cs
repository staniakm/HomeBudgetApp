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

namespace My_Finance_app
{
    /// <summary>
    /// Logika interakcji dla klasy BudgetPage.xaml
    /// </summary>
    public partial class BudgetPage : Page
    {
        private BudgetService budgetService;
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
            UpdateBudgetData();
        }

        private void SetNextMonth(object sender, RoutedEventArgs e)
        {
            selectedDate = selectedDate.AddMonths(1);
            lb_selectedMonth.Content = selectedDate.ToShortDateString().Substring(0, 7);
            UpdateBudgetData();
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
                BudgetEditWindow cw = new BudgetEditWindow(dr, budgetService, this);
                cw.ShowDialog();
            }
        }

        private void LoadBudget(object sender, RoutedEventArgs e)
        {
            UpdateBudgetData();

        }

        private void UpdateBudgetData()
        {
            var budgetData = budgetService.GetBudgetData(GetCurrentlySelectedDate());

            dg_budzety.DataContext = budgetService.GetBudgets(GetCurrentlySelectedDate());

            lb_przychodzy.Content = budgetData.Income;
            lb_wydatek.Content = budgetData.Expenses;
            lb_zaplanowane.Content = budgetData.Planned;
            lb_pozostalo.Content = budgetData.LeftToPlan < 0 ? 0.0 : budgetData.LeftToPlan;
            lb_oszczednosci.Content = budgetData.Savings;
        }

        public DateTime GetCurrentlySelectedDate()
        {
            return selectedDate;
        }
    }
}
