using Engine.entity;
using Engine.repository;
using System;
using System.Data;

namespace Engine.service
{
    public class BudgetService
    {
        private readonly BudgetRepository repository;

        public BudgetService(BudgetRepository repository)
        {
            this.repository = repository;
        }

        public object GetBudgets(DateTime dateTime)
        {
            return repository.GetBudgets(dateTime);
        }

        public BudgetData GetBudgetData(DateTime dateTime)
        {
            var budgetData = new BudgetData(

                 repository.GetBudgetCalculations("earned", dateTime),
                repository.GetBudgetCalculations("spend", dateTime),
                repository.GetBudgetCalculations("planed", dateTime),
                repository.GetBudgetCalculations("left", dateTime)
            );
            return budgetData;
        }

        public DataTable GetSallaryDescriptions()
        {
            return repository.GetSallaryDescriptions();
        }

        public void RecalculateBudget(DateTime selectedDate)
        {
            repository.RecalculateBudget(selectedDate);
        }

        public void UpdatePlannedBudget(int databaseBudgetRowID, string newValue, DateTime dateTime)
        {
            repository.UpdatePlannedBudget(databaseBudgetRowID, newValue, dateTime);
        }

        public void AddNewSallary(int accountID, string description, decimal moneyAmount, DateTime date)
        {
            repository.AddNewSallary(accountID, description, moneyAmount, date);
        }
    }
}
