namespace Engine.entity
{
    public class BudgetData
    {

        public double Income { get; set; }
        public double Expenses { get; set; }
        public double Planned { get; set; }
        public double LeftToPlan { get; set; }
        public double Savings { get => Income - Expenses; }

        public BudgetData(double earned, double spend, double planned, double leftToPlan)
        {
            Income = earned;
            Expenses = spend;
            Planned = planned;
            LeftToPlan = leftToPlan;
        }

    }
}
