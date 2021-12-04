using Engine.entity;
using Engine.reports;
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
            var report = GetReportByType(reportType);
            return sqlEngine.PrepareReport(report, reportSettings).DefaultView;

        }

        private IReport GetReportByType(ReportType.Type reportType)
        {
            switch (reportType)
            {
                case ReportType.Type.STANDARD:
                    return new StandardReports();
                case ReportType.Type.CATEGORY:
                    return new CategoryReport();
                case ReportType.Type.CATEGORY_AND_ACCOUNT:
                    return new CategoryAccountReport();
                case ReportType.Type.INVOICE_LIST:
                    return new InvoiceReport();
                default:
                    throw new Exception("Incorrect report value: " + reportType);
            }
        }

        public DataView GetInvoiceDetails(long invoiceId)
        {
            return sqlEngine.GetInvoiceDetails(invoiceId).DefaultView;
        }
    }
}
