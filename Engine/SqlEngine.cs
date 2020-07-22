using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace Engine
{
    public class SqlEngine
    {
        private readonly string conString;
        private readonly string database;
        public string id;

        public SqlEngine(string database, string user, string pass)
        {
            string dbString = @"localhost\SQLEXPRESS";
            conString = string.Format("Data Source={0}; Initial Catalog={1}; Integrated Security=false;" +
                    "Connection Timeout=10; user id={2}; password={3}", dbString, database, user, pass);
            this.database = database;
        }
        public bool TryToLoginIntoDatabase()
        {
            bool connected = false;
            using (SqlConnection _con = new SqlConnection(conString))
            {
                try
                {
                    _con.Open();
                    connected = true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Connection error");
                    connected = false;
                }
            }

            return connected;
        }

        public DataTable GetSallaryDescriptions()
        {
            return GetData("select text from SALARY_TYPE;");
        }

        public void AddNewSallary(int accountId, string description, decimal moneyAmount, DateTime date)
        {
            using (SqlConnection _con = new SqlConnection(conString))
            {
                _con.Open();
                using (SqlCommand com = new SqlCommand("insert into przychody(kwota, opis, konto, data) values (@kwota, @opis ,@konto, @data);", _con))
                {
                    SqlParameter par = new SqlParameter("@kwota", SqlDbType.Money)
                    {
                        Value = moneyAmount
                    };
                    com.Parameters.Add(par);
                    par = new SqlParameter("@opis", SqlDbType.VarChar, 100)
                    {
                        Value = description
                    };
                    com.Parameters.Add(par);
                    par = new SqlParameter("@konto", SqlDbType.Int)
                    {
                        Value = accountId
                    };
                    com.Parameters.Add(par);
                    par = new SqlParameter("@data", SqlDbType.VarChar, 10)
                    {
                        Value = date
                    };
                    com.Parameters.Add(par);
                    com.Prepare();
                    com.ExecuteNonQuery();
                }
                string query = "update konto set kwota = kwota + @kwota where ID = @konto";
                using (SqlCommand com = new SqlCommand(query, _con))
                {
                    SqlParameter par = new SqlParameter("@kwota", SqlDbType.Money)
                    {
                        Value = moneyAmount
                    };
                    com.Parameters.Add(par);
                    par = new SqlParameter("@konto", SqlDbType.Int)
                    {
                        Value = accountId
                    };
                    com.Parameters.Add(par);
                    com.Prepare();
                    com.ExecuteNonQuery();
                }
            }

        }

        public void AddAutomaticBills() => SQLexecuteNonQuerry("EXEC dodaj_rachunki");

        public void UpdateItemCategory(int productId, string newCategoryName, int newCategoryId, string newProductName)
        {
            SQLexecuteNonQuerry(string.Format("exec dbo.updateAsoCategory {0}, '{1}', {2}, '{3}'", productId, newCategoryName, newCategoryId, newProductName));
        }

        public void UpdatePlannedBudget(int databaseBudgetRowID, string newBudgetValue, DateTime date)
        {
            SQLexecuteNonQuerry(string.Format("update budzet set planed = {0} where id = {1}", newBudgetValue, databaseBudgetRowID));
            RecalculateBudget(date);
        }

        public void AddAsoToShop(string produkt, int shopId)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "@produkt", produkt },
                { "@idsklep", shopId.ToString() }
            };

            SQLexecuteNonQuerryProcedure("dbo.addAsoToStore", dic);
        }

        public ObservableCollection<Asortyment> GetProductsInStore(int shop)
        {
            ObservableCollection<Asortyment> ShopAso = new ObservableCollection<Asortyment>();

            DataTable dt = GetAsoList(shop);
            foreach (DataRow item in dt.Rows)
            {
                ShopAso.Add(new Asortyment((int)item["id"], (string)item["NAZWA"]));
            }
            return ShopAso;
        }

        public DataTable PrepareReport(ReportType.Type reportId, Dictionary<int, Tuple<string, string>> reportSettings)
        {
            DataTable dt = null;

            switch (reportId)
            {
                case ReportType.Type.STANDARD:
                    dt = GetDataWithSettings("exec generuj_zestawienie_2", reportSettings);
                    break;
                case ReportType.Type.CATEGORY:
                    dt = GetDataWithSettings("exec generuj_zestawienie_podzial_na_kategorie", reportSettings);
                    break;
                case ReportType.Type.CATEGORY_AND_ACCOUNT:
                    dt = GetDataWithSettings("exec generuj_zestawienie_podzial_na_kategorie_konto", reportSettings);
                    break;
                case ReportType.Type.INVOICE_LIST:
                    dt = GetDataWithSettings("exec show_invoice_list", reportSettings);
                    break;
            }
            return dt;
        }

        public DataTable GetItemsByCategory(string categoryName)
        {
            string sqlquery = "";
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (categoryName != "")
            {
                param.Add("@category", categoryName);
                sqlquery = "Select a.[id],a.[NAZWA],a.[id_kat], k.nazwa [Nazwa kategorii] From [dbo].[ASORTYMENT] a	Join kategoria k on a.id_kat = k.id where del = 0 and k.nazwa = @category order by a.nazwa;";
                return GetData(sqlquery, param);
            }
            else
            {
                sqlquery = "Select a.[id],a.[NAZWA],a.[id_kat], k.nazwa  [Nazwa kategorii]From [dbo].[ASORTYMENT] a	Join kategoria k on a.id_kat = k.id and del = 0 order by a.nazwa;";
                return GetData(sqlquery);
            }
        }

        public void RecalculateBudget(DateTime date)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "@month", date.ToShortDateString() }
            };

            SQLexecuteNonQuerryProcedure("dbo.RecalculateBudget", dic);
        }

        public void SaveInvoiceInDatabase(Invoice invoice)
        {
            using (SqlConnection _con = new SqlConnection(conString))
            {
                _con.Open();

                invoice.SetInvoiceId(int.Parse(SQLgetScalar("exec dbo.getNextIdForParagon", _con)));

                try
                {
                    SaveNewInvoiceInDatabase(invoice, _con);

                    SaveInvoiceItemsInDatabase(invoice, _con);

                    RecalculateInvoiceAndUpdateInvoiceCategories(invoice.GetInvoiceId(), _con);

                    UpdateBankAccount(invoice.GetInvoiceId(), _con);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString(), "Error during saving process", System.Windows.MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Jeśli procesura zwróci wartość > 0 tzn że sklep został dopisany. 
        /// </summary>
        /// <param name="shopName"></param>
        /// <returns></returns>
        public int CreateNewShopIfNotExists(string shopName)
        {
            return SQLexecuteNonQuerry(string.Format("if not exists(select 1 from sklepy where sklep = '{0}') insert into sklepy(sklep) select '{0}'", shopName));
        }



        /// <summary>
        /// Aktualizauje kolekcję kont. Można ustawiać bezpośrednio do datacontextu.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<BankAccount> GetBankAccountsCollection()
        {
            ObservableCollection<BankAccount> konta = new ObservableCollection<BankAccount>();
            DataTable dt = GetTableByName("konta");
            foreach (DataRow item in dt.Rows)
            {
                konta.Add(new BankAccount((int)item["id"], (string)item["nazwa"], (decimal)item["kwota"], (string)item["opis"], (string)item["wlasciciel"], (decimal)item["oprocentowanie"]));
            }
            return konta;
        }

        /// <summary>
        /// Zwracamy kolekcję sklepów. Można bezpośrednio bindować do datacontext
        /// </summary>
        public ObservableCollection<Shop> GetShopsCollection()
        {
            ObservableCollection<Shop> sklepy = new ObservableCollection<Shop>();

            DataTable dt = GetTableByName("sklepy");
            foreach (DataRow item in dt.Rows)
            {
                sklepy.Add(new Shop((int)item["id"], (string)item["sklep"]));
            }
            return sklepy;
        }

        public double GetBudgetCalculations(string operation, DateTime date)
        {
            switch (operation)
            {
                case "earned":
                    return GetSelectedMonthSallary(date);
                case "left":
                    return GetSelectedMonthLeftToPlan(date);
                case "planed":
                    return GetSelectedMonthPlaned(date);
                case "spend":
                    return GetSelectedMonthSpend(date);
            }
            return 0;
        }

        public double GetSelectedMonthLeftToPlan(DateTime data)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "@currentDate", data.ToShortDateString() }
            };
            return Convert.ToDouble(SQLgetScalarWithParameters("select (select  isnull(sum(kwota), 0) from przychody where MONTH(data) = MONTH(@currentDate) and year(data) = year(@currentDate))" +
            "- (select isnull(sum(planed),0) from budzet where miesiac = MONTH(@currentDate) and rok = year(@currentDate)) ", param));
        }

        public void ModifyBankAccount(Dictionary<string, string> tmpDic)
        {
            SQLexecuteNonQuerryProcedure("dbo.bankAccountModification", tmpDic);
        }

        /// <summary>
        /// Zwracamy kolekcję kategorii. Można bezpośrednio bindować do datacontext
        /// </summary>
        public ObservableCollection<Category> GetCategoryCollection()
        {
            ObservableCollection<Category> kategorie = new ObservableCollection<Category>();

            DataTable dt = GetTableByName("kategorie");
            foreach (DataRow item in dt.Rows)
            {
                kategorie.Add(new Category((int)item["id"], (string)item["nazwa"]));
            }
            return kategorie;
        }

        public object GetBudgets(DateTime date)
        {
            int month = date.Month;
            int year = date.Year;
            string sqlquery = "";
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (month != 0)
            {

                param.Add("@miesiac", month.ToString());
                param.Add("@year", year.ToString());
                sqlquery = "select b.id, b.miesiac, k.nazwa, b.planed, b.used, b.percentUsed from budzet b join kategoria k on k.id = b.category " +
                    "where miesiac = @miesiac and rok = @year order by k.nazwa; ";
                return GetData(sqlquery, param);
            }
            else
            {
                sqlquery = "select b.id, b.miesiac, k.nazwa, b.planed, b.used, b.percentUsed from budzet b join kategoria k on k.id = b.category " +
                    "where miesiac = month(getdate()) order by k.nazwa; ";
                return GetData(sqlquery);
            }
        }


        private DataTable GetDataWithSettings(string sqlCommand, Dictionary<int, Tuple<string, string>> reportSettings)
        {
            DataTable table = new DataTable
            {
                Locale = System.Globalization.CultureInfo.InvariantCulture
            };
            using (SqlConnection _con = new SqlConnection(conString))
            {
                _con.Open();
                ReportSettings(reportSettings, _con);
                SqlCommand command = new SqlCommand(sqlCommand, _con);
                SqlDataAdapter adapter = new SqlDataAdapter
                {
                    SelectCommand = command
                };

                adapter.Fill(table);
            }
            return table;
        }

        private void ReportSettings(Dictionary<int, Tuple<string, string>> dic, SqlConnection _con)
        {
            string _spid = SQLgetScalar("select @@SPID", _con);
            string querry = string.Format("delete from rapOrg where sesja = {0};\n", _spid);

            foreach (KeyValuePair<int, Tuple<string, string>> entry in dic)
            {
                querry += string.Format("insert into rapOrg select '{0}', '{1}', {2} ,{3};\n", entry.Value.Item1, entry.Value.Item2, entry.Key, _spid);
            }

            SQLexecuteNonQuerry(querry, _con);

        }

        private DataTable GetData(string sqlCommand)
        {
            DataTable table = new DataTable
            {
                Locale = System.Globalization.CultureInfo.InvariantCulture
            };
            using (SqlConnection _con = new SqlConnection(conString))
            {
                _con.Open();
                SqlCommand command = new SqlCommand(sqlCommand, _con);
                SqlDataAdapter adapter = new SqlDataAdapter
                {
                    SelectCommand = command
                };

                adapter.Fill(table);
            }
            return table;
        }

        private DataTable GetTableByName(string name)
        {
            string sqlCommand = "";

            switch (name)
            {
                case "konta":
                    sqlCommand = "SELECT nazwa, id, kwota, opis, wlasciciel, oprocentowanie FROM konto where del = 0 order by id;";
                    break;
                case "sklepy":
                    sqlCommand = "select '' sklep, -1 id union SELECT sklep,id FROM sklepy order by sklep;";
                    break;
                case "kategorie":
                    sqlCommand = "select '' nazwa, -1 id union SELECT nazwa,id FROM kategoria order by nazwa;";
                    break;
            }
            return GetData(sqlCommand);
        }

        private DataTable GetAsoList(int shop)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>
            {
                { "@id", shop }
            };

            string querry = "select '' as NAZWA, 0 id union  select a.NAZWA, a.id from ASORTYMENT a join ASORTYMENT_SKLEP sk on sk.id_aso = a.id " +
                "join sklepy s on sk.id_sklep = s.id and s.ID = @id where a.del = 0 and sk.del = 0 order by nazwa";
            return GetData(querry, dict);
        }

        private DataTable GetData<T>(string sqlCommand, Dictionary<string, T> param)
        {
            DataTable table = new DataTable
            {
                Locale = System.Globalization.CultureInfo.InvariantCulture
            };
            using (SqlConnection _con = new SqlConnection(conString))
            {
                _con.Open();
                SqlCommand command = new SqlCommand(sqlCommand, _con);
                SqlParameter par;// = new SqlParameter();
                foreach (var item in param)
                {
                    par = new SqlParameter
                    {
                        ParameterName = item.Key,
                        Value = item.Value
                    };
                    command.Parameters.Add(par);
                }
                SqlDataAdapter adapter = new SqlDataAdapter
                {
                    SelectCommand = command
                };

                adapter.Fill(table);
            }
            return table;
        }
        private void RecalculateInvoiceAndUpdateInvoiceCategories(int invoiceId, SqlConnection _con)
        {
            SQLexecuteNonQuerry(string.Format("exec przeliczParagon {0}", invoiceId), _con);
        }

        private void UpdateBankAccount(int invoiceId, SqlConnection _con)
        {
            SqlCommand com = new SqlCommand("update k set k.kwota = k.kwota-p.suma from paragony p join konto k on k.ID = p.konto where p.id = @idPagaron", _con);
            com.Parameters.AddWithValue("@idPagaron", invoiceId);
            com.ExecuteNonQuery();
        }

        private void SaveInvoiceItemsInDatabase(Invoice invoice, SqlConnection _con)
        {
            SqlCommand com = new SqlCommand("insert into paragony_szczegoly(id_paragonu, cena_za_jednostke, ilosc, cena, rabat, ID_ASO, opis) values (@InvoiceId, @cenaJednostkowa, @ilosc, @cena, @rabat, @idAso, @opis)", _con);
            SqlParameter par = new SqlParameter("@InvoiceId", SqlDbType.Int);
            com.Parameters.Add(par);
            par = new SqlParameter("@cenaJednostkowa", SqlDbType.Decimal)
            {
                Precision = 8,
                Scale = 2
            };

            com.Parameters.Add(par);
            par = new SqlParameter("@ilosc", SqlDbType.Decimal)
            {
                Precision = 6,
                Scale = 3
            };
            com.Parameters.Add(par);

            par = new SqlParameter("@cena", SqlDbType.Money);
            com.Parameters.Add(par);

            par = new SqlParameter("@rabat", SqlDbType.Money);
            com.Parameters.Add(par);

            par = new SqlParameter("@idAso", SqlDbType.Int);
            com.Parameters.Add(par);

            par = new SqlParameter("@opis", SqlDbType.VarChar, 150);
            com.Parameters.Add(par);
            com.Prepare();

            for (int x = 0; x < invoice.GetNumberOfItems(); x++)
            {
                InvoiceDetails item = invoice.GetItem(x);
                PrepareInvoiceDetailsInsertQueryParameters(invoice, com, item);
                com.ExecuteNonQuery();
            }
        }

        private void PrepareInvoiceDetailsInsertQueryParameters(Invoice invoice, SqlCommand com, InvoiceDetails item)
        {
            com.Parameters[0].Value = invoice.GetInvoiceId();
            com.Parameters[1].Value = item.Price;
            com.Parameters[2].Value = item.Quantity;
            com.Parameters[3].Value = item.TotalPrice;
            com.Parameters[4].Value = item.Discount;
            com.Parameters[5].Value = item.GetIDAso();
            com.Parameters[6].Value = item.Description;
        }

        private void SaveNewInvoiceInDatabase(Invoice invoice, SqlConnection _con)
        {
            SqlCommand com = new SqlCommand("insert into paragony(nr_paragonu, data, ID_sklep, konto, suma, opis) values (@nrParagonu, @data,@idsklep,@konto, 0,'' );", _con);

            SqlParameter par = new SqlParameter("@nrParagonu", SqlDbType.VarChar, 50)
            {
                Value = invoice.GetInvoiceNumber().ToUpper()
            };
            com.Parameters.Add(par);

            par = new SqlParameter("@data", SqlDbType.Date)
            {
                Value = invoice.GetDate()
            };
            com.Parameters.Add(par);

            par = new SqlParameter("@idsklep", SqlDbType.VarChar, 150)
            {
                Value = invoice.GetShopId()
            };
            com.Parameters.Add(par);

            par = new SqlParameter("@konto", SqlDbType.Int)
            {
                Value = invoice.GetAccount()
            };
            com.Parameters.Add(par);

            com.Prepare();
            com.ExecuteNonQuery();
        }

        private void Backup()
        {
            SQLexecuteNonQuerry("exec BackupDatabase");
        }

        private double GetSelectedMonthSpend(DateTime data)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "@currentDate", data.ToShortDateString() }
            };
            return Convert.ToDouble(SQLgetScalarWithParameters("select isnull(sum(suma),0) from paragony where YEAR(data) = year(@currentDate) and MONTH(data) = month(@currentDate) and del = 0", param));
        }

        private double GetSelectedMonthPlaned(DateTime data)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "@currentDate", data.ToShortDateString() }
            };
            return System.Convert.ToDouble(SQLgetScalarWithParameters("select isnull(sum(planed),0) from budzet where miesiac = MONTH(@currentDate) and rok = year(@currentDate)", param));
        }

        private double GetSelectedMonthSallary(DateTime providedDate)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "@providedDate", providedDate.ToShortDateString() }
            };
            return Convert.ToDouble(SQLgetScalarWithParameters("select  isnull(sum(kwota),0) from przychody where MONTH(data) = MONTH(@providedDate) and year(data) = year(@providedDate)", param));
        }

        private int SQLexecuteNonQuerry(string querry)
        {
            int rowsAffected = 0;
            using (SqlConnection _con = new SqlConnection(conString))
            {
                _con.Open();
                SqlCommand sql = new SqlCommand(querry, _con);
                try
                {
                    rowsAffected = sql.ExecuteNonQuery();
                }
                catch (System.Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message, "Non query execution error");
                    throw;
                }
            }
            return rowsAffected;
        }
        private int SQLexecuteNonQuerry(string querry, SqlConnection _con)
        {
            int rowsAffected = 0;
            SqlCommand sql = new SqlCommand(querry, _con);
            try
            {
                rowsAffected = sql.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "Non query execution error");
                throw;
            }
            return rowsAffected;
        }

        private int SQLexecuteNonQuerryProcedure(string querry, Dictionary<string, string> param)
        {

            int rowsAffected = 0;
            using (SqlConnection _con = new SqlConnection(conString))
            {
                _con.Open();
                SqlCommand command = new SqlCommand(querry, _con)
                {
                    CommandType = CommandType.StoredProcedure
                };


                //Console.WriteLine(querry);
                foreach (var item in param)
                {
                    SqlParameter par = new SqlParameter
                    {
                        ParameterName = item.Key,
                        Value = item.Value
                    };
                    command.Parameters.Add(par);
                }

                try
                {
                    rowsAffected = command.ExecuteNonQuery();
                }
                catch (System.Exception)
                {

                    throw;
                }
            }
            return 0;
        }

        private string SQLgetScalarWithParameters(string querry, Dictionary<string, string> param)
        {
            string output = "";
            using (SqlConnection _con = new SqlConnection(conString))
            {
                _con.Open();
                SqlCommand command = new SqlCommand(querry, _con);
                SqlParameter par;
                foreach (var item in param)
                {
                    par = new SqlParameter
                    {
                        ParameterName = item.Key,
                        Value = item.Value
                    };
                    command.Parameters.Add(par);
                }

                try
                {
                    output = command.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString(), "SQL scalar with parameters error");
                    throw;
                }
            }
            return output;
        }

        private string SQLgetScalar(string querry, SqlConnection conn)
        {
            string output = "";
            try
            {
                SqlCommand sql = new SqlCommand(querry, conn);

                output = sql.ExecuteScalar().ToString();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString(), "SQL scalar error");
                //Console.WriteLine("SQL scalar Value MSG: " + ex.Message);
                throw;
            }
            return output;
        }
    }

}
