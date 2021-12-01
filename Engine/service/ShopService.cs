using Engine.repository;
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
        private ShopRepository shopRepository;

        public ShopService(ShopRepository shopRepository)
        {
            this.shopRepository = shopRepository;
            Console.WriteLine("Loading shop service");
        }

        public ObservableCollection<Shop> GetShopsCollection()
        {
            return shopRepository.GetShopsCollection();
        }

        public ObservableCollection<Product> GetProductsInStore(int shopId)
        {
            return shopRepository.GetProductsInStore(shopId);
        }

        public int CreateNewShopIfNotExists(string shopName)
        {
            return shopRepository.CreateNewShopIfNotExists(shopName);
        }

        public void AddAsoToShop(string productName, int shopId)
        {
            shopRepository.AddAsoToShop(productName, shopId);
        }
    }
}
