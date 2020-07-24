using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.service
{
    public class CategoryService
    {
        private SqlEngine sqlEngine;

        public CategoryService(SqlEngine sqlEngine)
        {
            this.sqlEngine = sqlEngine;
        }

        public ObservableCollection<Category> GetCategoryCollection()
        {
            return sqlEngine.GetCategoryCollection();
        }

        public DataTable GetItemsByCategory(string text)
        {
            return sqlEngine.GetItemsByCategory(text);
        }

        public void UpdateItemCategory(int productId, string newCategoryName, int newCategoryId, string newProductName)
        {
            sqlEngine.UpdateItemCategory(productId, newCategoryName, newCategoryId, newProductName);
        }
    }
}
