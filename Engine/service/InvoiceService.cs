using System.Data;

namespace Engine.service
{

    public class InvoiceService
    {
        private readonly SqlEngine sqlEngine;

        public InvoiceService(SqlEngine sqlEngine)
        {
            this.sqlEngine = sqlEngine;
        }


        public void AddAutomaticInvoices()
        {
             sqlEngine.AddAutomaticInvoices();
        }

        public DataTable GetAutoInvoiceItemList()
        {
            return sqlEngine.GetAutoInvoiceItemList();
        }

        public void SaveInvoice(Invoice invoice)
        {
            if (invoice != null)
            {
                sqlEngine.SaveInvoiceInDatabase(invoice);
            }
        }
    }
}
