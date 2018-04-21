using Engine;
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
using System.Windows.Shapes;

namespace My_Finance_app
{
    /// <summary>
    /// Interaction logic for SalaryAddingWindow.xaml
    /// </summary>
    public partial class SalaryAddingWindow : Window
    {
        SqlEngine _sql;
        public SalaryAddingWindow(SqlEngine sql)
        {
            this._sql = sql;
            InitializeComponent();
        }

        private void ok_button_Click(object sender, RoutedEventArgs e)
        {


            _sql.addNewSallary(accountId, description, moneyAmount);
            this.Close();
        }

        private void cancel_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
