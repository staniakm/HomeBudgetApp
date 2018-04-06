using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Engine
{
    public class SqlEngine
    {
        private SqlConnection _con;
        private string database;
        public string _spid { get; private set; }
        public string id;
        public static ObservableCollection<BankAccount> bankAccounts { get; set; }

        public SqlEngine(string database, string login, string pass)
        {
            _con = new SqlConnection();
            this.database = database;
            if (ConnectSQLDatabase(login, pass))
            {
                SQLexecuteNonQuerry("EXEC dodaj_rachunki");
                try
                {
                    Task t = Task.Run(() =>
                    {
                        Backup();
                    });
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message, "Error");
                }
            }
                ;

        }

        public SqlEngine(string database)
        {
            _con = new SqlConnection();
            this.database = database;
        }


        public bool Con
        {
            //return true;
            get
            {
                if (_con.State == ConnectionState.Open)
                {
                    return true;
                }
                else { return false; }
            }
            set
            {
                if (value == false)
                {
                    _con.Close();
                    _con.Dispose();
                }
            }
        }

        public bool ConnectSQLDatabase(string user, string pass)
        {
            bool connected = false;
            string dbString = "";
            string strCon = "";
            if (Con)
            {
                Con = false;
            }
            else
            {
                if (Environment.MachineName == "MARIUSZ_DOMOWY")
                {
                    dbString = @"MARIUSZ_DOMOWY\SQLEXPRESS";
                }
                else { dbString = "MARIUSZ_DOMOWY"; }
                    strCon = "Data Source=" + dbString + ";Initial Catalog="+database+";Integrated Security=false;Connection Timeout=10;user id="+user+";password=" + pass; //'NT Authentication

                _con.ConnectionString = strCon;
                if (Con == false)
                {
                    try
                    {
                        _con.Open();
                        _spid = SQLgetScalar("select @@SPID");
                        connected = true;
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex.Message);
                        connected = false;
                    }
                }
            }
            return connected;
        }

        public void UpdateCategoryOfAso(int idASO, string newCategory, int category_id, string newName)
        {
            SQLexecuteNonQuerry(String.Format("exec dbo.updateAsoCategory {0}, '{1}', {2}, '{3}'", idASO, newCategory, category_id, newName));
        }

        public void UpdateBudget(int databaseBudgetRowID, string newValue, DateTime date)
        {
            try
            {
                SQLexecuteNonQuerry(String.Format("update budzet set planed = {0} where id = {1}", newValue, databaseBudgetRowID));
                RecalculateBudget(date);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private int SQLexecuteNonQuerry(string querry)
        {
            //Console.WriteLine( "query: "+querry);
            int rowsAffected = 0;
            SqlCommand sql = new SqlCommand(querry, _con);
            try
            {
                //Console.WriteLine(querry);
                rowsAffected = sql.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                //Console.WriteLine(e.Message);
                throw;
            }
            return rowsAffected;
        }

        public void AddAsoToStore(string produkt, string sklep)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "@produkt", produkt },
                { "@sklep", sklep }
            };

            SQLexecuteNonQuerryProcedure("dbo.addAsoToStore", dic);
        }

        private int SQLexecuteNonQuerryProcedure(string querry, Dictionary<string, string> param)
        {

            int rowsAffected = 0;
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
            return 0;
        }

        private string SQLgetScalarWithParameters(string querry, Dictionary<string, string> param)
        {
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

            string output = "";
            try
            {
                output = command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("SQL scalar Value MSG: " + ex.Message);
                throw;
            }
            return output;
        }

        private string SQLgetScalar(string querry)
        {
            string output = "";
            try
            {
                SqlCommand sql = new SqlCommand(querry, _con);
                
                output = sql.ExecuteScalar().ToString();

            }
            catch (Exception ex)
            {
                //Console.WriteLine("SQL scalar Value MSG: " + ex.Message);
                throw;
            }
            return output;
        }

        private DataTable GetData(string sqlCommand)
        {
            SqlCommand command = new SqlCommand(sqlCommand, _con);
            SqlDataAdapter adapter = new SqlDataAdapter
            {
                SelectCommand = command
            };
            DataTable table = new DataTable
            {
                Locale = System.Globalization.CultureInfo.InvariantCulture
            };
            adapter.Fill(table);
            return table;
        }

        private DataTable GetTable(string name)
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

        public DataTable GetAsoList(string shop)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                { "@sklep", shop }
            };

            string querry = "select '' as NAZWA, 0 id union  select a.NAZWA, a.id from ASORTYMENT a join sklepy s on s.sklep = @sklep " +
           "join ASORTYMENT_SKLEP sk on sk.id_aso = a.id where a.del = 0 and  sk.del = 0 and sk.id_sklep = s.id";
            return GetData(querry, dict);
        }

        private DataTable GetData(string sqlCommand, Dictionary<string, string> param)
        {
            SqlCommand command = new SqlCommand(sqlCommand, _con);
            SqlParameter par;// = new SqlParameter();
            foreach (var item in param)
            {
                par = new SqlParameter();
                //Console.WriteLine(item.Value);
                par.ParameterName = item.Key;
                par.Value = item.Value;
                //command.Parameters.AddWithValue(item.Key,item.Value);
                command.Parameters.Add(par);
            }
            

            SqlDataAdapter adapter = new SqlDataAdapter
            {
                SelectCommand = command
            };
            DataTable table = new DataTable
            {
                Locale = System.Globalization.CultureInfo.InvariantCulture
            };
            adapter.Fill(table);
            return table;
        }

        public DataTable ZestawienieWydatkow(int id)
        {
            DataTable dt = null;

            switch (id)
            {
                case 1:
                    dt = GetData("exec generuj_zestawienie_2");
                    break;
                case 2:
                    dt = GetData("exec generuj_zestawienie_podzial_na_kategorie");
                    break;

            }
            return dt;
        }

        public DataTable GetCategoryItems(string categoryName)
        {
            string sqlquery = "";
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (categoryName != "wszystkie")
            {
                param.Add("@kategoria", categoryName);
                sqlquery = "Select a.[id],a.[NAZWA],a.[id_kat], k.nazwa [Nazwa kategorii] From [dbo].[ASORTYMENT] a	Join kategoria k on a.id_kat = k.id where del = 0 and k.nazwa = @kategoria order by a.nazwa;";
                return GetData(sqlquery, param);
            }
            else
            {
                sqlquery = "Select a.[id],a.[NAZWA],a.[id_kat], k.nazwa  [Nazwa kategorii]From [dbo].[ASORTYMENT] a	Join kategoria k on a.id_kat = k.id and del = 0 order by a.nazwa;";
                return GetData(sqlquery);
            }
        }

        private void RecalculateBill(int paragon)
        {
            SQLexecuteNonQuerry(string.Format("exec przeliczParagon {0}", paragon));
        }

        public void RecalculateBudget(DateTime date)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "@month", date.ToShortDateString() }
            };

            SQLexecuteNonQuerryProcedure("dbo.RecalculateBudget", dic);
        }

        public void SaveBilInDatabase(Paragon paragon)
        {
                                    
            paragon.IdParagon = int.Parse(SQLgetScalar("exec dbo.getNextIdForParagon"));
            
            try
            {
                SqlCommand com = new SqlCommand("insert into paragony(nr_paragonu, data, sklep, konto, suma, opis) values (@nrParagonu, @data,@sklep,@konto, 0,'' );", _con);
                SqlParameter par = new SqlParameter("@nrParagonu", SqlDbType.VarChar,50);
                par.Value= paragon.NrParagonu.ToUpper();
                com.Parameters.Add(par);
                par = new SqlParameter("@data", SqlDbType.Date);
                par.Value = paragon.Data;
                com.Parameters.Add(par);
                par = new SqlParameter("@sklep", SqlDbType.VarChar,150);
                par.Value= paragon.Sklep;
                com.Parameters.Add(par);

                par = new SqlParameter("@konto", SqlDbType.Int);
                par.Value= paragon.Konto;
                com.Parameters.Add(par);
                com.Prepare();
                com.ExecuteNonQuery();
                

                com = new SqlCommand("insert into paragony_szczegoly(id_paragonu, cena_za_jednostke, ilosc, cena, ID_ASO, opis) values (@idParagon, @cenaJednostkowa, @ilosc, @cena, @idAso, @opis)", _con);
                par = new SqlParameter("@idParagon", SqlDbType.Int);
                com.Parameters.Add(par);
                par = new SqlParameter("@cenaJednostkowa", SqlDbType.Decimal);
                par.Precision = 8;
                par.Scale = 2;
                
                com.Parameters.Add(par);
                par = new SqlParameter("@ilosc", SqlDbType.Decimal);
                par.Precision = 6;
                par.Scale = 3;
                com.Parameters.Add(par);
                par = new SqlParameter("@cena", SqlDbType.Money);

                com.Parameters.Add(par);
                par = new SqlParameter("@idAso", SqlDbType.Int);
                com.Parameters.Add(par);
                par = new SqlParameter("@opis", SqlDbType.VarChar,150);
                com.Parameters.Add(par);
                com.Prepare();

                for (int x = 0; x < paragon.Szczegoly.Count; x++)
                {
                    ParagonSzczegoly p = paragon.Szczegoly[x];
                    com.Parameters[0].Value = paragon.IdParagon;
                    com.Parameters[1].Value = p.Cena;
                    com.Parameters[2].Value = p.Ilosc;
                    com.Parameters[3].Value = p.CenaCalosc;
                    com.Parameters[4].Value = p.IDAso;
                    com.Parameters[5].Value = p.Opis;
                    com.ExecuteNonQuery();
                }
                RecalculateBill(paragon.IdParagon);
                com = new SqlCommand("update k set k.kwota = k.kwota-p.suma from paragony p join konto k on k.ID = p.konto where p.id = @idPagaron", _con);
                com.Parameters.AddWithValue("@idPagaron", paragon.IdParagon);
                com.ExecuteNonQuery();
                GetAccountColection();
            }
            catch (Exception)
            {

                throw;
            }
            



        }

        /// <summary>
        /// Jeśli procesura zwróci wartość > 0 tzn że sklep został dopisany. 
        /// </summary>
        /// <param name="sklep"></param>
        /// <returns></returns>
        public int ShopExists(string sklep)
        {
            int rowsNumb = 0;
            rowsNumb = SQLexecuteNonQuerry(string.Format("if not exists(select 1 from sklepy where sklep = '{0}') insert into sklepy(sklep) select '{0}'", sklep));
            return rowsNumb;
        }

        public void ReportSettings(Dictionary<int, Tuple<string, string>> dic)
        {
            string querry = string.Format("delete from rapOrg where sesja = {0};\n", _spid);

            foreach (KeyValuePair<int, Tuple<string,string>> entry in dic)
            {
                querry += string.Format("insert into rapOrg select '{0}', '{1}', {2} ,{3};\n", entry.Value.Item1, entry.Value.Item2, entry.Key, _spid);
            }

            //Console.WriteLine(querry);
            SQLexecuteNonQuerry(querry);
               
        }

        private void Backup()
        {
            SQLexecuteNonQuerry("exec BackupDatabase");
        }

        /// <summary>
        /// Zwracamy kolekcję kont. Można ustawiać bezpośrednio do datacontextu.
        /// </summary>
        /// <returns></returns>
        public void GetAccountColection()
        {
            ObservableCollection<BankAccount> konta = new ObservableCollection<BankAccount>();
            DataTable dt = GetTable("konta");
            foreach (DataRow item in dt.Rows)
            {
                konta.Add(new BankAccount((int)item["id"], (string)item["nazwa"], (decimal) item["kwota"], (string)item["opis"],(string)item["wlasciciel"],(decimal)item["oprocentowanie"]));
            }
            bankAccounts = konta;
            //return konta;
        }
        /// <summary>
        /// Zwracamy kolekcję sklepów. Można bezpośrednio bindować do datacontext
        /// </summary>
        public ObservableCollection<Sklep> GetShopsCollection()
        {
            ObservableCollection<Sklep> sklepy = new ObservableCollection<Sklep>();

            DataTable dt = GetTable("sklepy");
            foreach (DataRow item in dt.Rows)
            {
                sklepy.Add(new Sklep((int)item["id"], (string)item["sklep"]));
            }
            return sklepy;
        }

        public double GetBudgetCalculations(string v, DateTime date)
        {
            switch (v)
            {
                case "earn":
                    return GetCurrentMonthSalary(date);
                case "left":
                    return GetCurrentMonthLeft(date);
                case "planed":
                    return GetCurrentMonthPlaned(date);
                case "spend":
                    return GetCurrentMonthSpend(date);
            }
            return 0;
        }

        private double GetCurrentMonthSpend(DateTime data)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("@currentDate", data.ToShortDateString());
            return System.Convert.ToDouble(SQLgetScalarWithParameters("select isnull(sum(suma),0) from paragony where YEAR(data) = year(@currentDate) and MONTH(data) = month(@currentDate) and del = 0", param));
        }

        private double GetCurrentMonthPlaned(DateTime data)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("@currentDate", data.ToShortDateString());
            return System.Convert.ToDouble(SQLgetScalarWithParameters ("select isnull(sum(planed),0) from budzet where miesiac = MONTH(@currentDate) and rok = year(@currentDate)",param));
        }

        public double GetCurrentMonthLeft( DateTime data)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("@currentDate", data.ToShortDateString());
            return System.Convert.ToDouble(SQLgetScalarWithParameters("select (select  isnull(sum(kwota), 0) from przychody where MONTH(data) = MONTH(@currentDate) and year(data) = year(@currentDate))" +
            "- (select isnull(sum(planed),0) from budzet where miesiac = MONTH(@currentDate) and rok = year(@currentDate)) ",param));
        }

        private double GetCurrentMonthSalary(DateTime data)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("@currentDate", data.ToShortDateString());
            return System.Convert.ToDouble(SQLgetScalarWithParameters("select  isnull(sum(kwota),0) from przychody where MONTH(data) = MONTH(@currentDate) and year(data) = year(@currentDate)",param));
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

            DataTable dt = GetTable("kategorie");
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
                
                param.Add("@miesiac", ""+month);
                param.Add("@year", ""+year); 
                sqlquery = "select b.id, b.miesiac, k.nazwa, b.planed, b.used, b.percentUsed from budzet b join kategoria k on k.id = b.category " +
                    "where miesiac = @miesiac and rok = @year; ";
                //Console.WriteLine(sqlquery);
                return GetData(sqlquery, param);
            }
            else
            {
                sqlquery = "select b.id, b.miesiac, k.nazwa, b.planed, b.used, b.percentUsed from budzet b join kategoria k on k.id = b.category " +
                    "where miesiac = month(getdate()); ";
                return GetData(sqlquery);
            }
        }
    }

}
