using System.Data;
using System.Windows;

namespace My_Finance_app
{
    /// <summary>
    /// Logika interakcji dla klasy InvoiceView.xaml
    /// </summary>
    public partial class InvoiceView : Window
    {
        
        public InvoiceView(DataView invoiceDetails)
        {
            InitializeComponent();
            dg_invoice_details.ItemsSource = invoiceDetails;
        }
    }
}
