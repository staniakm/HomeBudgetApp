using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.repository
{
   public class ShopRepository
    {
        private readonly SqlEngine sqlEngine;

        public ShopRepository()
        {
            this.sqlEngine = SqlEngine.GetInstance();
        }

        /// <summary>
        /// Zwracamy kolekcję sklepów. Można bezpośrednio bindować do datacontext
        /// </summary>
        public ObservableCollection<Shop> GetShopsCollection()
        {
            ObservableCollection<Shop> sklepy = new ObservableCollection<Shop>();

            DataTable dt = sqlEngine.GetData("select '' sklep, -1 id union SELECT sklep, id FROM sklepy order by sklep;");
            foreach (DataRow item in dt.Rows)
            {
                sklepy.Add(new Shop(item));
            }
            return sklepy;
        }

        public ObservableCollection<Product> GetProductsInStore(int shop)
        {
            ObservableCollection<Product> ShopAso = new ObservableCollection<Product>();

            DataTable dt = GetAsoList(shop);
            foreach (DataRow item in dt.Rows)
            {
                ShopAso.Add(new Product((int)item["id"], (string)item["NAZWA"]));
            }
            return ShopAso;
        }


        /// <summary>
        /// Jeśli procesura zwróci wartość > 0 tzn że sklep został dopisany. 
        /// </summary>
        /// <param name="shopName"></param>
        /// <returns></returns>
        public int CreateNewShopIfNotExists(string shopName)
        {
            return sqlEngine.SQLexecuteNonQuerry(string.Format("if not exists(select 1 from sklepy where sklep = '{0}') insert into sklepy(sklep) select '{0}'", shopName));
        }

        public void AddAsoToShop(string produkt, int shopId)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "@produkt", produkt },
                { "@idsklep", shopId.ToString() }
            };

            sqlEngine.SQLexecuteNonQuerryProcedure("dbo.addAsoToStore", dic);
        }

        private DataTable GetAsoList(int shop)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>
            {
                { "@id", shop }
            };

            string querry = "select '' as NAZWA, 0 id union  select a.NAZWA, a.id from ASORTYMENT a join ASORTYMENT_SKLEP sk on sk.id_aso = a.id " +
                "join sklepy s on sk.id_sklep = s.id and s.ID = @id where a.del = 0 and sk.del = 0 order by nazwa";
            return sqlEngine.GetData(querry, dict);
        }
    }
}
