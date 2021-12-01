using Engine.entity;
using System.Collections.Generic;

namespace Engine.reports
{
    class StandardReports : IReport
    {
    public List<string> GetColumnNames()
        {
            return new List<string> { "Miesiąc", "Rok", "Mariusz", "Asia", "Wspólne", "Wydatki", "Przychody", "Oszczędności" };
        }

        public string GetCommand()
        {
        return "select month , year , owner_1 , owner_2 , common , expence_sum , income , savings  from generateStandardReport()";
    }
    }

    class CategoryReport : IReport
    {
    public List<string> GetColumnNames()
        {
            return new List<string> { "Kategoria", "Wydane", "Procent calości" };
        }

        public string GetCommand()
        {
        return "select name, sum, percentage from  generateReportByCategory()";
    }
    }

    class CategoryAccountReport: IReport
    {
        public string GetCommand()
        {
            return "select owner_name, category_name, sum, percentage from generateReportByCategoryAndAccount()";
        }

        public List<string> GetColumnNames()
        {
            return new List<string> { "Właściciel", "Kategoria", "Suma",  "Procent calości" };
        }
    }

    class InvoiceReport : IReport
    {
        public List<string> GetColumnNames()
        {
            return new List<string> { "Id", "Date", "Numer paragonu", "Suma", "Właścicel", "Sklep" };
        }

        public string GetCommand()
        {
            return "select invoice_id, invoice_date, invoice_number, sum, owner_name, shop_name from showInvoiceList()";
        }
    }
}
