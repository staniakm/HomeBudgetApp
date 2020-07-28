using Engine;
using Engine.service;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace My_Finance_app.pages
{
    /// <summary>
    /// Logika interakcji dla klasy ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        private ReportService reportService;
        private readonly CategoryService categoryService;
        private readonly ShopService shopService;

        public ReportPage()
        {
            
        }

        public ReportPage(ReportService reportService, CategoryService categoryService, ShopService shopService)
        {
            InitializeComponent();
            this.reportService = reportService;
            this.categoryService = categoryService;
            this.shopService = shopService;
            LoadReportInitValues();
        }

        private void LoadReportInitValues()
        {
            dp_report_start_date.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dp_report_end_date.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
            cb_report_category_collection.DataContext = categoryService.GetCategoryCollection();
            cb_report_shop_collection.DataContext = shopService.GetShopsCollection();
        }

        /// <summary>
        /// Create session settings for specific report.
        /// </summary>
        private void GenerateReport(object sender, RoutedEventArgs e)
        {


            switch (cb_report_type.Text.ToUpper())
            {
                case "NORMALNE":
                    PrepareReportDetails(ReportType.Type.STANDARD);
                    break;
                case "KATEGORIE":
                    PrepareReportDetails(ReportType.Type.CATEGORY);
                    break;
                case "KATEGORIE I KONTO":
                    PrepareReportDetails(ReportType.Type.CATEGORY_AND_ACCOUNT);
                    break;
                case "LISTA PARAGONÓW":
                    PrepareReportDetails(ReportType.Type.INVOICE_LIST);
                    break;
                default:
                    break;
            }
        }

        private void PrepareReportDetails(ReportType.Type reportType)
        {
            Dictionary<int, Tuple<string, string>> reportSettings = PrepareReportSettings();
            dg_reports.ItemsSource = reportService.PrepareReport(reportType, reportSettings);
        }

        private Dictionary<int, Tuple<string, string>> PrepareReportSettings()
        {
            var settings = new Dictionary<int, Tuple<string, string>>();

            //date settings parametr = 1
            if (dp_report_start_date.SelectedDate.ToString() != "" || dp_report_end_date.SelectedDate.ToString() != "")
            {
                string startDate = dp_report_start_date.SelectedDate.ToString() != "" ? dp_report_start_date.SelectedDate.ToString() : "2000-01-01";
                string endDate = dp_report_end_date.SelectedDate.ToString() != "" ? dp_report_end_date.SelectedDate.ToString() : "2050-01-01";
                settings.Add(1, new Tuple<string, string>(startDate, endDate));
            }
            //category parametr = 2

            if ((int)cb_report_category_collection.SelectedIndex > 0)
            {
                settings.Add(2, new Tuple<string, string>(cb_report_category_collection.SelectedValue.ToString(), ""));
            }
            //shop parametr = 3
            if ((int)cb_report_shop_collection.SelectedIndex > 0)
            {
                settings.Add(3, new Tuple<string, string>(cb_report_shop_collection.SelectedValue.ToString(), ""));
            }
            return settings;

        }
    }
}
