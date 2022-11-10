using ModeloCarta;

namespace ModeloJugador
{
    public class Jugador
    {
        public string nombre { get; set; }
        public int puntos { get; set; }
        public List<Carta> mano { get; set; }
        public List<Carta> pila_acumulada { get; set; }
        public int numero_escobas { get; set; }
        public string numero_jugador { get; set; }

        public Jugador(string nombre, string numero_jugador)
        {
            this.nombre = nombre;
            this.puntos = 0;
            this.mano = new List<Carta>();
            this.pila_acumulada = new List<Carta>();
            this.numero_escobas = 0;
            this.numero_jugador = numero_jugador;
        }
    }
}