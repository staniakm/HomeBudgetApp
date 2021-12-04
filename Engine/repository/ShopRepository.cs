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

            DataTable dt = sqlEngine.GetData("select '' name, -1 id union SELECT name, id FROM shop order by name;");
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
                ShopAso.Add(new Product((int)item["id"], (string)item["name"]));
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
            return sqlEngine.SQLexecuteNonQuerry(string.Format("insert into shop(name) select '{0}'where not exists(select 1 from shop where Upper(name) = Upper('{0}'))", shopName));
        }

        public void AddAsoToShop(string produkt, int shopId)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "produkt", produkt.ToString() },
                { "shopid", shopId.ToString() }
            };

            sqlEngine.SQLexecuteNonQuerry($@"call addasotoshop ('{produkt}', {shopId})");
        }

        private DataTable GetAsoList(int shop)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>
            {
                { "id", shop }
            };

            string querry = "select '' as name, 0 id union  select a.name, a.id from assortment a join shop_assortment sk on sk.aso = a.id " +
                "join shop s on sk.shop = s.id and s.ID = @id where a.del = false and sk.del = false order by name";
            return sqlEngine.GetData(querry, dict);
        }
    }
}
