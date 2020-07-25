using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.service
{
    public class ShopService
    {
        private SqlEngine sqlEngine;

        public ShopService(SqlEngine sql)
        {
            sqlEngine = sql;
            Console.WriteLine("Loading shop service");
        }

        public ObservableCollection<Shop> GetShopsCollection()
        {
            return sqlEngine.GetShopsCollection();
        }

        public ObservableCollection<Asortyment> GetProductsInStore(int shopId)
        {
            return sqlEngine.GetProductsInStore(shopId);
        }

        public int CreateNewShopIfNotExists(string shopName)
        {
            return sqlEngine.CreateNewShopIfNotExists(shopName);
        }

        public void AddAsoToShop(string productName, int shopId)
        {
            sqlEngine.AddAsoToShop(productName, shopId);
        }
    }
}
