using System;
using System.Collections.Generic;
using System.Data;

namespace Engine.service
{
    public class ReportService
    {
        private readonly SqlEngine sqlEngine;

        public ReportService(SqlEngine sqlEngine)
        {
            this.sqlEngine = sqlEngine;
        }

        public DataView PrepareReport(ReportType.Type reportType, Dictionary<int, Tuple<string, string>> reportSettings)
        {

            string sqlCommand;
            switch (reportType)
            {
                case ReportType.Type.STANDARD:
                    sqlCommand = "exec generateStandardReport";
                    break;

                case ReportType.Type.CATEGORY:
                    sqlCommand = "exec generateReportByCategory";
                    break;
                case ReportType.Type.CATEGORY_AND_ACCOUNT:
                    sqlCommand = "exec generateReportByCategoryAndAccount";
                    break;
                case ReportType.Type.INVOICE_LIST:
                    sqlCommand = "exec showInvoiceList";
                    break;
                default:
                    throw new Exception("Incorrect value");
            }
            return sqlEngine.PrepareReport(sqlCommand, reportSettings).DefaultView;

        }

        public DataView GetInvoiceDetails(int invoiceId)
        {
            return sqlEngine.GetInvoiceDetails(invoiceId).DefaultView;
        }
    }
}
