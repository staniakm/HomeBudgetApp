using Engine;
using System.Windows;
namespace MyFinanceApp
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

        private void ConfirmLogin_Click(object sender, RoutedEventArgs e)
        {
            //create instance to SqlEngine for selected database
            SqlEngine sql = SqlEngine.GetInstance();
            ////check if login data are correct
            ///
            bool isLogged = sql.TryLogin(cb_db_name.Text, login.Text, pass.Password.ToString());

            if (isLogged)
            {
                //if login data are correctthen we can create main window.
                //we can pass sql instance to main window so we don't have to create another instance in main window
                //and we can use passed instance
                MainWindow mw = new MainWindow(sql);
                mw.Show();
                this.Close();
            }
            else
            {
                pass.Clear();
                login.Clear();

                MessageBox.Show("Niepoprawne dane logowania");

            }
        }
    }
}
