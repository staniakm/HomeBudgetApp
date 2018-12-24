using Engine;
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
using System.Windows.Shapes;

namespace My_Finance_app
{
    /// <summary>
    /// Interaction logic for SalaryAddingWindow.xaml
    /// </summary>
    public partial class SalaryAddingWindow : Window
    {
        private SqlEngine _sql;
        private int accountID;
        private MainWindow mw;
        public SalaryAddingWindow(int accountID, SqlEngine sql, MainWindow mainWindow)
        {
            this._sql = sql;
            this.accountID = accountID;
            this.mw = mainWindow;
            InitializeComponent();
            salary_description.DataContext = _sql.GetSallaryDescriptions();
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
                _sql.AddNewSallary(this.accountID, description, moneyAmount, date);
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
