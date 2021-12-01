using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Engine.entity;
using Npgsql;

namespace Engine
{
    public class SqlEngine
    {
        private string conString;
        private static SqlEngine _instance;

        // This is the static method that controls the access to the singleton
        // instance. On the first run, it creates a singleton object and places
        // it into the static field. On subsequent runs, it returns the client
        // existing object stored in the static field.
        public static SqlEngine GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SqlEngine();
            }
            return _instance;
        }

        private SqlEngine(){}
        public bool TryLogin(string database, string user, string pass)
        {
           // string dbString = @"localhost\SQLEXPRESS";
            // conString = string.Format("Data Source={0}; Initial Catalog={1}; Integrated Security=false;" +
               //     "Connection Timeout=10; user id={2}; password={3}", dbString, database, user, pass);
            conString = string.Format("Host=localhost;Username={0};Password={1};Database=postgres",user, pass);

            bool connected = false;
            using (NpgsqlConnection _con = new NpgsqlConnection(conString))
            {
                try
                {
                    _con.Open();
                    connected = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    connected = false;
                }
            }

            return connected;
        }




        public void AddAutomaticInvoices() => SQLexecuteNonQuerry("call autoInvoice()");

        public void UpdateItemCategory(int productId, string newCategoryName, int newCategoryId, string newProductName)
        {
            SQLexecuteNonQuerry(string.Format("call updateAsoCategory( {0}, '{1}', '{2}', {3})", productId, newCategoryName, newProductName, newCategoryId ));
        }


            internal DataTable GetAutoInvoiceItemList()
        {
            string query = @"select a.name product,
	                            s.name shop,
	                            k.account_name account,
	                            p.price price,
	                            p.QUANTITY 
                            from automatic_invoice_products p 
	                            join assortment a on a.id = p.aso 
	                            join shop s on s.ID = p.shop
	                            join account k on k.ID = p.account; ";            
            return GetData(query);
        }

        public DataTable PrepareReport(IReport report, Dictionary<int, Tuple<string, string>> reportSettings)
        {
            return GetDataWithSettings(report, reportSettings);
        }

        public DataTable GetItemsByCategory(string categoryName)
        {

            if (categoryName != "")
            {
                Dictionary<string, string> param = new Dictionary<string, string>
                {
                    { "category", categoryName }
                };
                var sqlquery = @"select a.id,
	                                a.name as Produkt,
	                                a.category as Id_kategorii,
	                                k.name as Kategoria
                                from assortment a
                                join category k on a.category = k.id 
                                where del = false 
	                                and k.name = @category order by a.name;";
                return GetData(sqlquery, param);
            }
            else
            {
                var sqlquery = @"select a.id,
	                                a.name,
	                                a.category,
	                                k.name category_name 
                                from assortment a
	                                join category k on a.category = k.id and del = false order by a.name;";
                return GetData(sqlquery);
            }
        }


        public void SaveInvoiceInDatabase(Invoice invoice)
        {
            using (NpgsqlConnection _con = new NpgsqlConnection(conString))
            {
                _con.Open();
                try
                {
                    //todo do in transaction
                    var invoiceId = SaveNewInvoiceInDatabase(invoice, _con);
                    SaveInvoiceItemsInDatabase(invoice, invoiceId, _con);

                    RecalculateInvoiceAndUpdateInvoiceCategories(invoiceId, _con);

                    UpdateBankAccount(invoiceId, _con);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString(), "Error during saving process", System.Windows.MessageBoxButton.OK);
                }
            }
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
                kategorie.Add(new Category(item));
            }
            return kategorie;
        }

        public DataTable GetInvoiceDetails(long invoiceId)
        {
            string command = string.Format(@"select a.name ,
	                                    amount,
                                        unit_price,
	                                    discount,
                                        price, 
	                                    k.name as category 
                                    from invoice_details ps join assortment a on a.id = ps.category 
	                                    join category k on k.id = a.category 
                                    where invoice = {0}", invoiceId);
            return GetData(command);
        }


        private DataTable GetDataWithSettings(IReport report, Dictionary<int, Tuple<string, string>> reportSettings)
        {
            DataTable table = new DataTable
            {
                Locale = System.Globalization.CultureInfo.InvariantCulture
            };

            using (NpgsqlConnection _con = new NpgsqlConnection(conString))
            {
                _con.Open();
                ReportSettings(reportSettings, _con);
                NpgsqlCommand command = new NpgsqlCommand(report.GetCommand(), _con);
                NpgsqlDataAdapter  adapter = new NpgsqlDataAdapter 
                {
                    SelectCommand = command
                };

                adapter.Fill(table);
            }
            var index = 0;
            foreach (var item in report.GetColumnNames())
            {
                table.Columns[index].ColumnName = item;
                index++;
            }

            return table;
        }

        private void ReportSettings(Dictionary<int, Tuple<string, string>> dic, NpgsqlConnection _con)
        {
            string _spid = SQLgetScalar("select pg_backend_pid()", _con);
            string querry = string.Format("delete from report_settings where session_id = {0};\n", _spid);

            foreach (KeyValuePair<int, Tuple<string, string>> entry in dic)
            {
                querry += string.Format("insert into report_settings select '{0}', '{1}', {2} ,{3};\n", entry.Value.Item1, entry.Value.Item2, entry.Key, _spid);
            }

            SQLexecuteNonQuerry(querry, _con);

        }

        internal DataTable GetData(string NpgsqlCommand)
        {
            DataTable table = new DataTable
            {
                Locale = System.Globalization.CultureInfo.InvariantCulture
            };
            using (NpgsqlConnection _con = new NpgsqlConnection(conString))
            {
                _con.Open();
                NpgsqlCommand command = new NpgsqlCommand(NpgsqlCommand, _con);
                NpgsqlDataAdapter  adapter = new NpgsqlDataAdapter 
                {
                    SelectCommand = command
                };

                adapter.Fill(table);
            }
            return table;
        }

        private DataTable GetTableByName(string name)
        {
            string NpgsqlCommand = "";

            switch (name)
            {
                case "kategorie":
                    NpgsqlCommand = "select '' name, -1 id union SELECT name,id FROM category order by name;";
                    break;
                case "account_owner":
                    NpgsqlCommand = "SELECT owner_id as id, name ,description FROM account_owner order by name;";
                    break;
            }
            return GetData(NpgsqlCommand);
        }

                internal DataTable GetData<T>(string NpgsqlCommand, Dictionary<string, T> param)
        {
            DataTable table = new DataTable
            {
                Locale = System.Globalization.CultureInfo.InvariantCulture
            };
            using (NpgsqlConnection _con = new NpgsqlConnection(conString))
            {
                _con.Open();
                NpgsqlCommand command = new NpgsqlCommand(NpgsqlCommand, _con);
                foreach (var item in param)
                {
                    command.Parameters.AddWithValue(item.Key, item.Value);
                }
                NpgsqlDataAdapter  adapter = new NpgsqlDataAdapter 
                {
                    SelectCommand = command
                };
                table.Reset();
                adapter.Fill(table);
            }
            return table;
        }
        private void RecalculateInvoiceAndUpdateInvoiceCategories(long invoiceId, NpgsqlConnection _con)
        {
            SQLexecuteNonQuerry($@"call recalculateInvoice ({invoiceId})", _con);
        }

        private void UpdateBankAccount(long invoiceId, NpgsqlConnection _con)
        {
            NpgsqlCommand com = new NpgsqlCommand("update account a set money = money-i.sum from invoice i where a.ID = i.account and i.id = @invoice_id", _con);
            com.Parameters.AddWithValue("invoice_id", invoiceId);
            com.ExecuteNonQuery();
        }

        private void SaveInvoiceItemsInDatabase(Invoice invoice, long invoiceId, NpgsqlConnection _con)
        {
            var sb = new StringBuilder();
            foreach (var item in invoice.GetInvoiceItems())
            {
                sb.AppendLine(
                    $@"insert into invoice_details(invoice, unit_price, amount, price, discount, assortment, description)
                            values ({invoiceId}, {item.getPriceAsString()}, {item.getQuantityAsString()}, {item.getTotalPriceAsString()}, {item.getDiscountAsString()}, {item.GetIDAso()}, '{item.Description}');");
            }
            Console.WriteLine(sb.ToString());

            var com = new NpgsqlCommand(sb.ToString(), _con);
            com.ExecuteNonQuery();
        }

        private void PrepareInvoiceDetailsInsertQueryParameters(int invoiceId, NpgsqlCommand com, InvoiceDetails item)
        {
            com.Parameters[0].Value = invoiceId;
            com.Parameters[1].Value = item.Price;
            com.Parameters[2].Value = item.Quantity;
            com.Parameters[3].Value = item.TotalPrice;
            com.Parameters[4].Value = item.Discount;
            com.Parameters[5].Value = item.GetIDAso();
            com.Parameters[6].Value = item.Description;
        }

        private long SaveNewInvoiceInDatabase(Invoice invoice, NpgsqlConnection _con)
        {
            NpgsqlCommand com = new NpgsqlCommand("insert into invoice(invoice_number, date, shop, account, sum, description)" +
                "  values (@invoice_number, @date,@shop,@account, 0.0,'' ) RETURNING id;", _con);

            com.Parameters.AddWithValue("invoice_number", NpgsqlTypes.NpgsqlDbType.Varchar, 50, invoice.InvoiceNumber.ToUpper());

            NpgsqlParameter par = new NpgsqlParameter("date", NpgsqlTypes.NpgsqlDbType.Date)
            {
                Value = invoice.InvoiceDate
            };
            com.Parameters.Add(par);

            par = new NpgsqlParameter("shop", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = invoice.ShopId
            };
            com.Parameters.Add(par);

            par = new NpgsqlParameter("account", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = invoice.AccountId
            };
            com.Parameters.Add(par);

            //com.Prepare();
           var id = com.ExecuteScalar();
            return (long)id;
        }

        internal int SQLexecuteNonQuerry(string querry)
        {
            int rowsAffected = 0;
            using (NpgsqlConnection _con = new NpgsqlConnection(conString))
            {
                _con.Open();
                NpgsqlCommand sql = new NpgsqlCommand(querry, _con);
                try
                {
                    rowsAffected = sql.ExecuteNonQuery();
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(querry);
                    System.Windows.MessageBox.Show(e.Message, "Non query execution error");
                    throw;
                }
            }
            return rowsAffected;
        }
        private int SQLexecuteNonQuerry(string querry, NpgsqlConnection _con)
        {
            int rowsAffected = 0;
            NpgsqlCommand sql = new NpgsqlCommand(querry, _con);
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

        internal int SQLexecuteNonQuerryProcedure(string querry, Dictionary<string, string> param)
        {

            int rowsAffected = 0;
            using (NpgsqlConnection _con = new NpgsqlConnection(conString))
            {
                _con.Open();
                NpgsqlCommand command = new NpgsqlCommand(querry, _con);


                //Console.WriteLine(querry);
                foreach (var item in param)
                {
                    command.Parameters.AddWithValue(item.Key, item.Value);
                }

                try
                {
                    rowsAffected = command.ExecuteNonQuery();
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
            return rowsAffected;
        }

        internal int SQLexecuteNonQuerry(string querry, Dictionary<string, object> param)
        {

            int rowsAffected = 0;
            using (NpgsqlConnection _con = new NpgsqlConnection(conString))
            {
                _con.Open();
                NpgsqlCommand command = new NpgsqlCommand(querry, _con);
               

                //Console.WriteLine(querry);
                foreach (var item in param)
                {
                    command.Parameters.AddWithValue(item.Key, item.Value);
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

        internal string SQLgetScalarWithParameters(string querry, Dictionary<string, string> param)
        {
            string output = "";
            using (NpgsqlConnection _con = new NpgsqlConnection(conString))
            {
                _con.Open();
                NpgsqlCommand command = new NpgsqlCommand(querry, _con);
                SqlParameter par;
                foreach (var item in param)
                {
                    command.Parameters.AddWithValue(item.Key, item.Value);
                }

                try
                {
                    output = command.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(querry);
                    System.Windows.MessageBox.Show(ex.Message.ToString(), "SQL scalar with parameters error");
                    throw;
                }
            }
            return output;
        }


        internal string SQLgetScalarWithParameters(string querry, Dictionary<string, object> param)
        {
            string output = "";
            using (NpgsqlConnection _con = new NpgsqlConnection(conString))
            {
                _con.Open();
                NpgsqlCommand command = new NpgsqlCommand(querry, _con);
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

        private string SQLgetScalar(string querry, NpgsqlConnection conn)
        {
            try
            {
                NpgsqlCommand sql = new NpgsqlCommand(querry, conn);
                return sql.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString(), "SQL scalar error");
                //Console.WriteLine("SQL scalar Value MSG: " + ex.Message);
                throw;
            }
        }
    }

}
