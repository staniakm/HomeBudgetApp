using System.Collections.ObjectModel;
using System.Data;

namespace Engine.service
{
    public class CategoryService
    {
        private readonly SqlEngine sqlEngine;

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
            return sqlEngine.GetItemsByCategory(text.Trim());
        }

        public void UpdateItemCategory(int productId, string newCategoryName, int newCategoryId, string newProductName)
        {
            sqlEngine.UpdateItemCategory(productId, newCategoryName, newCategoryId, newProductName);
        }
    }
}
