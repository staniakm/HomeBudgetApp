using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Category
    {
        public Category(int iD, string name)
        {
            ID = iD;
            Nazwa = name;
        }

        public int ID { get; set; }
        public string Nazwa { get; set; }

    }
}
