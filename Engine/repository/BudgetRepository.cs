using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.repository
{
    public class BudgetRepository
    {
        private readonly SqlEngine sqlEngine;

        public BudgetRepository()
        {
            this.sqlEngine = SqlEngine.GetInstance();
        }

        public DataTable GetSallaryDescriptions()
        {
            return sqlEngine.GetData("select name from salary_type;");
        }

        public void RecalculateBudget(DateTime date)
        {
           sqlEngine.SQLexecuteNonQuerry($@"call recalculatebudget('{date.ToShortDateString()}')");
        }

        public void UpdatePlannedBudget(int databaseBudgetRowID, string newBudgetValue, DateTime date)
        {
            sqlEngine.SQLexecuteNonQuerry(string.Format("update budget set planned = {0} where id = {1}", newBudgetValue, databaseBudgetRowID));
            RecalculateBudget(date);
        }

        public DataTable GetBudgets(DateTime date)
        {
            int month = date.Month;
            int year = date.Year;
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (month != 0)
            {

                param.Add("month", month.ToString());
                param.Add("year", year.ToString());
                var sqlquery = @"select b.id, b.month, k.name, b.planned, b.used, b.percentage 
                                from budget b join category k on k.id = b.category
                                where month = @month::INTEGER and year = @year::INTEGER order by k.name; ";
                return sqlEngine.GetData(sqlquery, param);
            }
            else
            {
                var sqlquery = @"select b.id, b.month,k.name, b.planned, b.used, b.percentage
                    from budget b join category k on k.id = b.category 
                    where month = EXTRACT(MONTH from CURRENT_DATE)
						and year = extract(YEAR from current_date)
					order by k.name;";
                return sqlEngine.GetData(sqlquery);
            }
        }

        public void AddNewSallary(int accountId, string description, decimal moneyAmount, DateTime date)
        {
            Dictionary<string, object> param = new Dictionary<string, object>
            {
                {"amount", moneyAmount },
                {"description",description },
                {"accountId",accountId },
                {"date", date }
            };

            sqlEngine.SQLexecuteNonQuerry("insert into income(value, description, account, date) values (@amount, @description ,@accountId, @date);", param);


            //todo add trigger
          param =  new Dictionary<string, object>
            {
                {"amount", moneyAmount },
                {"accountId",accountId },
            };
            sqlEngine.SQLexecuteNonQuerry("update account set money = money + @amount where id = @accountId", param);

        }
        public double GetBudgetCalculations(string operation, DateTime date)
        {
            switch (operation)
            {
                case "earned":
                    return GetSelectedMonthSallary(date);
                case "left":
                    return GetSelectedMonthLeftToPlan(date);
                case "planed":
                    return GetSelectedMonthPlaned(date);
                case "spend":
                    return GetSelectedMonthSpend(date);
            }
            return 0;
        }


        public double GetSelectedMonthLeftToPlan(DateTime data)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "month", data.Month.ToString()},
                {"year",data.Year.ToString() }
            };
            var query = @"select 
                        (select COALESCE(sum(value), 0) from income 
 	                        where EXTRACT(MONTH from date) = @month::INTEGER
 		                        and extract (YEAR from date) = @year::INTEGER)
                            - (select COALESCE(sum(planned),0) from budget where month = @month::INTEGER
	                                and year = @year::INTEGER)";
            return Convert.ToDouble(sqlEngine.SQLgetScalarWithParameters(query, param));
        }

        private double GetSelectedMonthSpend(DateTime data)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "month", data.Month.ToString() },
                {"year", data.Year.ToString() }
            };
            var querry = @"select COALESCE(sum(sum),0) from invoice
                            where extract(YEAR from date) = @year::INTEGER
                            and extract( MONTH from date) = @month::INTEGER and del = false";
            return Convert.ToDouble(sqlEngine.SQLgetScalarWithParameters(querry, param));
        }
        private double GetSelectedMonthPlaned(DateTime data)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "month", data.Month.ToString() },
                {"year", data.Year.ToString() }
            };
            var querry = @"select COALESCE(sum(planned),0) from budget 
                            where month = @month::INTEGER
                            and year = @year::INTEGER";
            return System.Convert.ToDouble(sqlEngine.SQLgetScalarWithParameters(querry, param));
        }
        private double GetSelectedMonthSallary(DateTime providedDate)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "month", providedDate.Month.ToString() },
                {"year", providedDate.Year.ToString() }
            };
            var querry = @"select COALESCE(sum(value),0) from income
                        where extract(MONTH from date) = @month::INTEGER
                        and extract(year from date) = @year::INTEGER";
            return Convert.ToDouble(sqlEngine.SQLgetScalarWithParameters(querry, param));
        }
    }
}
