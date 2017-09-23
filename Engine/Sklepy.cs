using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Sklepy
    {
        public ObservableCollection<Sklep> listaSklepow { get; set; }

        public Sklepy()
        {
            listaSklepow = new ObservableCollection<Sklep>();
        }
    }

    public class Sklep {
        public int ID { get; set; }
        public string Nazwa { get; set; }

        public Sklep(int id, string nazwa)
        {
            ID = id;
            Nazwa = nazwa;
        }

    }
}

