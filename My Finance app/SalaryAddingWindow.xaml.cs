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
        public SalaryAddingWindow(int accountID, SqlEngine sql)
        {
            this._sql = sql;
            this.accountID = accountID;
            InitializeComponent();
            salary_description.DataContext = _sql.getSallaryDescriptions();
        }

        private void ok_button_Click(object sender, RoutedEventArgs e)
        {
            string description = salary_description.Text;
            if (sallary_amount.Text != "")
            {
                decimal moneyAmount = decimal.Parse(sallary_amount.Text.Replace(".", ","));

                _sql.addNewSallary(this.accountID, description, moneyAmount);
                this.Close();
            }
        }

        private void cancel_Clicked(object sender, RoutedEventArgs e)
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
