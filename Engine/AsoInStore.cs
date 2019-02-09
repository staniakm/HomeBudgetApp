using System.Collections.ObjectModel;

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
