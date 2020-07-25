using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.entity
{
    public class BudgetData
    {
        private double earned;
        private double spend;
        private double planned;
        private double leftToPlan;
        
        public BudgetData(double earned, double spend, double planned, double leftToPlan)
        {
            this.Income = earned;
            this.Expenses = spend;
            this.Planned = planned;
            this.LeftToPlan = leftToPlan;
        }

        public double Income { get => earned; set => earned = value; }
        public double Expenses { get => spend; set => spend = value; }
        public double Planned { get => planned; set => planned = value; }
        public double LeftToPlan { get => leftToPlan; set => leftToPlan = value; }

        public double Savings { get => earned - spend; }
    }
}
