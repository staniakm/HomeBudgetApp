using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AsoInStore
    {
        //możemy zrobić observable collecition
        public ObservableCollection<Asortyment> ListaAsortymentu;
        public AsoInStore()
        {
            ListaAsortymentu = new ObservableCollection<Asortyment>();
        }
    }

    public class Asortyment
    {

        public int ID { get; set; }
        public string Nazwa { get; set; }

        public Asortyment(int id, string nazwa)
        {
            ID = id;
            Nazwa = nazwa;
        }

    }
}
