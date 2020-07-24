using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.service
{
    public class ReportService
    {
        private SqlEngine sqlEngine;

        public ReportService(SqlEngine sqlEngine)
        {
            this.sqlEngine = sqlEngine;
        }

        public DataView PrepareReport(ReportType.Type reportType, Dictionary<int, Tuple<string, string>> reportSettings)
        {

            var sqlCommand = "";
            switch(reportType)
            {
                case ReportType.Type.STANDARD:
                    sqlCommand = "exec generuj_zestawienie_2";
                    break;

                case ReportType.Type.CATEGORY:
                    sqlCommand = "exec generuj_zestawienie_podzial_na_kategorie";
                    break;
                case ReportType.Type.CATEGORY_AND_ACCOUNT:
                    sqlCommand = "exec generuj_zestawienie_podzial_na_kategorie_konto";
                    break;
                case ReportType.Type.INVOICE_LIST:
                    sqlCommand = "exec show_invoice_list";
                    break;
                default:
                    throw new Exception("Incorrect value");
            }
            return sqlEngine.PrepareReport(sqlCommand, reportSettings).DefaultView;

        }
    }
}
