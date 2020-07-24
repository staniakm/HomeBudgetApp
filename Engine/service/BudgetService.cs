using Engine.entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.service
{
    public class BudgetService
    {
        private SqlEngine sqlEngine;

        public BudgetService(SqlEngine sqlEngine)
        {
            this.sqlEngine = sqlEngine;
        }

        public object GetBudgets(DateTime dateTime)
        {
            return sqlEngine.GetBudgets(dateTime);
        }

        public BudgetData GetBudgetData(DateTime dateTime)
        {
            var budgetData = new BudgetData(
            
                 sqlEngine.GetBudgetCalculations("earned", dateTime),
                sqlEngine.GetBudgetCalculations("spend", dateTime),
                sqlEngine.GetBudgetCalculations("planed", dateTime),
                sqlEngine.GetBudgetCalculations("left", dateTime)
            );
            return budgetData;
        }

        public DataTable GetSallaryDescriptions()
        {
            return sqlEngine.GetSallaryDescriptions();
        }

        public void RecalculateBudget(DateTime selectedDate)
        {
            sqlEngine.RecalculateBudget(selectedDate);
        }

        public void UpdatePlannedBudget(int databaseBudgetRowID, string newValue, DateTime dateTime)
        {
            sqlEngine.UpdatePlannedBudget(databaseBudgetRowID, newValue, dateTime);
        }

        public void AddNewSallary(int accountID, string description, decimal moneyAmount, DateTime date)
        {
            sqlEngine.AddNewSallary(accountID, description, moneyAmount, date);
        }
    }
}
