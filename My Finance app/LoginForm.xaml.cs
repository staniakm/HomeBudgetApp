using Engine;
using System.Windows;
namespace My_Finance_app
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void confirmLogin_Click(object sender, RoutedEventArgs e)
        {
            SqlEngine sql = new SqlEngine("Normalna");
            if (sql.polacz_z_baza(login.Text,pass.Password.ToString()))
            {
                MainWindow mw = new MainWindow(sql);
                mw.Show();
                this.Close();
            } else
            {
                if (sql.Con)
                {
                    sql.Con = false;
                }
                pass.Clear();
                login.Clear();

                MessageBox.Show("Niepoprawne dane logowania");

            }
        }
    }
}
