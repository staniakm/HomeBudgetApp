using System.Collections.ObjectModel;

namespace Engine
{
    public class Shops
    {

        public ObservableCollection<Sklep> ListaSklepow { get; set; }

        public Shops()
        {
            ListaSklepow = new ObservableCollection<Sklep>();
        }
    }

    public class Sklep {
        public int ID { get; private set; }
        public string Nazwa { get; private set; }

        public Sklep(int id, string nazwa)
        {
            ID = id;
            Nazwa = nazwa;
        }

        //public int getID()
        //{
        //    return this.ID;
        //}

        //public string getName()
        //{
        //    return this.Nazwa;
        //}

    }
}

