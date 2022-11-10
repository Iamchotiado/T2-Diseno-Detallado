namespace ModeloCarta
{
    public class Carta
    {
        public string pinta { get; set; }
        public string valor { get; set; }
        public int puntos { get; set; }

        public Carta(string pinta, string valor, int puntos)
        {
            this.pinta = pinta;
            this.valor = valor;
            this.puntos = puntos;
        }

        public string nombre
        {
            get
            {
                return valor + " _ " + pinta;
            }
        }
    }
}