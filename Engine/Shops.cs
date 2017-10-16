using System.Collections.ObjectModel;

namespace Engine
{
    public class Shops
    {
        public ObservableCollection<Sklep> listaSklepow { get; set; }

        public Shops()
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

