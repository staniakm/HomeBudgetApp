using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Paragon
    {
        public int IdParagon { get; set; }
        public DateTime Data { get; set; }
        public string NrParagonu { get; set; }
        public string Sklep { get; set; }
        public int Konto { get; set; }
        public int IdSklep { get; set; }
                        
        public ObservableCollection<ParagonSzczegoly> Szczegoly { get; set; }

        public Paragon()
        {
            
            Szczegoly = new ObservableCollection<ParagonSzczegoly>();
        }

    }
}
