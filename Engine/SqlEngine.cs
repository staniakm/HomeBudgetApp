using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace Engine
{
    public class SqlEngine
    {
        private SqlConnection _con;
        private string Database;
        public string _spid { get; private set; }
        public string id;
        public SqlEngine(string database, string login, string pass)
        {
            _con = new SqlConnection();
            Database = database;
            Polacz_z_baza(login, pass);
        }

        public SqlEngine(string database)
        {
            _con = new SqlConnection();
            Database = database;
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

        public bool Polacz_z_baza(string user, string pass)
        {
            bool connected = false;
            string dbString = "";
            string strCon = "";
            //Console.Write("Łączenie: ");
            //Console.WriteLine(Con);
            if (Con)
            {
                Con = false;
                Console.WriteLine("rozłączam");
            }
            else
            {
                if (Environment.MachineName == "MARIUSZ_DOMOWY")
                {
                    dbString = @"MARIUSZ_DOMOWY\SQLEXPRESS";
                }
                else { dbString = "MARIUSZ_DOMOWY"; }
                    strCon = "Data Source=" + dbString + ";Initial Catalog="+Database+";Integrated Security=false;Connection Timeout=10;user id="+user+";password=" + pass; //'NT Authentication

                _con.ConnectionString = strCon;
                if (Con == false)
                {
                    try
                    {
                        _con.Open();
                        _spid = SQLgetScalar("select @@SPID");
                        connected = true;
                        Console.WriteLine("Connected to database");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        connected = false;
                    }
                }
            }
            return connected;
        }

        public int SQLexecuteNonQuerry(string querry)
        {
            int rowsAffected = 0;
            SqlCommand sql = new SqlCommand(querry, _con);
            try
            {
                Console.WriteLine(querry);
                rowsAffected = sql.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
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

        public int SQLexecuteNonQuerryProcedure(string querry, Dictionary<string, string> paramet)
        {

            int rowsAffected = 0;
            SqlCommand command = new SqlCommand(querry, _con)
            {
                CommandType = CommandType.StoredProcedure
            };


            Console.WriteLine(querry);
            foreach (var item in paramet)
            {
                SqlParameter par = new SqlParameter();
                Console.WriteLine(string.Format("klucz: {0}. Value: {1}", item.Key, item.Value));
                par.ParameterName = item.Key;
                par.Value = item.Value;
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

        public string SQLgetScalar(string querry)
        {
            string output = "";
            try
            {
                SqlCommand sql = new SqlCommand(querry, _con);
                output = sql.ExecuteScalar().ToString();

            }
            catch (Exception ex)
            {
                Console.WriteLine("SQL scalar Value MSG: " + ex.Message);
                throw;
            }
            return output;
        }

        public DataTable GetData(string sqlCommand)
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

        public DataTable GetTable(string name)
        {
            string sqlCommand = "";

            switch (name)
            {
                case "konta":
                    sqlCommand = "SELECT nazwa, id, kwota, opis, wlasciciel, oprocentowanie FROM konto order by id;";
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

        public DataTable GetData(string sqlCommand, Dictionary<string, string> param)
        {
            SqlCommand command = new SqlCommand(sqlCommand, _con);
            SqlParameter par = new SqlParameter();
            foreach (var item in param)
            {
                Console.WriteLine(item.Value);
                par.ParameterName = item.Key;
                par.Value = item.Value;
                //command.Parameters.AddWithValue(item.Key,item.Value);
            }
            command.Parameters.Add(par);

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

        public void PrzeliczParagon(int paragon)
        {
            SQLexecuteNonQuerry(string.Format("exec przeliczParagon {0}", paragon));
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

        public void Backup()
        {
            SQLexecuteNonQuerry("exec BackupDatabase");
        }

        /// <summary>
        /// Zwracamy kolekcję kont. Można ustawiać bezpośrednio do datacontextu.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<BankAccount> GetAccountColection()
        {
            ObservableCollection<BankAccount> konta = new ObservableCollection<BankAccount>();
            DataTable dt = GetTable("konta");
            foreach (DataRow item in dt.Rows)
            {
                konta.Add(new BankAccount((int)item["id"], (string)item["nazwa"], (decimal) item["kwota"], (string)item["opis"],(string)item["wlasciciel"],(decimal)item["oprocentowanie"]));
            }
            return konta;
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
        /// <summary>
        /// Zwracamy kolekcję kategorii. Można bezpośrednio bindować do datacontext
        /// </summary>
        public ObservableCollection<Category> GetCategoryCollection()
        {
            ObservableCollection<Category> kategorie = new ObservableCollection<Category>();

            DataTable dt = GetTable("kategorie");
            foreach (DataRow item in dt.Rows)
            {
              //  Console.WriteLine(item["id"] + " " + item["nazwa"]);
                kategorie.Add(new Category((int)item["id"], (string)item["nazwa"]));
            }
            return kategorie;
        }
    }

}
