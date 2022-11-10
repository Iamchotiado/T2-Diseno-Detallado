using ModeloJugador;
using ModeloCarta;
using ModeloJuego;

namespace Vista
{
    public static class Vistas
    {
        public static bool chequearValidezInput(string seleccionado, int max){
            if (int.TryParse(seleccionado, out int result))
            {
                if (result > 0 && result <= max)
                {
                    return true;
                }
            }
            return false;
        }
        public static string printMenu()
        {
            Console.WriteLine("BIENVENIDO AL JUEGO LA ESCOBA\n");
            Console.WriteLine("1. Jugar en modo local");
            Console.WriteLine("2. Jugar en modo servidor");
            Console.WriteLine("Opcion:");
            var option = Console.ReadLine();
            if (chequearValidezInput(option, 2))
            {
                return option;
            }
            else
            {
                Console.WriteLine("Opcion invalida");
                return printMenu();
            }
        }

        public static string nombrarJugador(int numero)
        {
            Console.WriteLine("\nIngrese el nombre del Jugador " + numero + ":");
            var name = Console.ReadLine();
            return name;
        }

        public static int seleccionCartaBajada(List<Carta> mesa, Jugador jugador)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("\nJuega jugador " + jugador.numero_jugador + "( " + jugador.nombre + " )" + "\n");
            Console.WriteLine("Mesa actual:");
            int indice_carta = 1;
            foreach (var carta in mesa)
            {
                Console.WriteLine("(" + indice_carta + ") " + carta.nombre);
                indice_carta++;
            }

            indice_carta = 1;
            Console.WriteLine("\nMano Jugador:");
            foreach (var carta in jugador.mano)
            {
                Console.WriteLine("(" + indice_carta + ") " + carta.nombre);
                indice_carta++;
            }
            Console.WriteLine("\n Que carta quieres bajar?:");
            Console.WriteLine("\nIngresa un numero entre 1 y " + (jugador.mano.Count).ToString());
            var option = Console.ReadLine();
            if (chequearValidezInput(option, jugador.mano.Count))
            {
                int carta_bajada = (int.Parse(option) - 1);
                return carta_bajada;
            }
            else
            {
                Console.WriteLine("Opcion invalida");
                return seleccionCartaBajada(mesa, jugador);
            }
        }

        public static int seleccionCombinacion(List<List<Carta>> combinaciones)
        {
            Console.WriteLine("\nHay " + combinaciones.Count + " jugadas en la mesa:");
            int indice_combinacion = 1;
            foreach (var combinacion in combinaciones)
            {
                string jugada = "(" + indice_combinacion + ") ";
                foreach (var carta in combinacion)
                {
                    jugada += ", " + carta.nombre;
                }
                Console.WriteLine(jugada);
                indice_combinacion++;
            }
            Console.WriteLine("\nIngresa un numero entre 1 y " + (combinaciones.Count).ToString());

            var option = Console.ReadLine();
            if (chequearValidezInput(option, combinaciones.Count))
            {
                int combinacion_seleccionada = (int.Parse(option) - 1);
                return combinacion_seleccionada;
            }
            else
            {
                Console.WriteLine("Opcion invalida");
                return seleccionCombinacion(combinaciones);
            }
        }

        public static void printCartasRobadasMesa(Jugador jugador, List<Carta> cartas_llevadas)
        {
            string linea_print = "\nJugador " + jugador.numero_jugador + "( " + jugador.nombre + " )" + " se lleva las siguientes cartas: ";
            foreach (var carta in cartas_llevadas)
            {
                linea_print += carta.nombre + ", ";
            }
            Console.WriteLine(linea_print);
        }

        public static void printAvisoEscoba(Jugador jugador)
        {
            Console.WriteLine("\n-------------------------------------------------------------");
            Console.WriteLine("JUGADOR " + jugador.numero_jugador + "( " + jugador.nombre + " )" + " HIZO ESCOBA!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.WriteLine("-------------------------------------------------------------");
        }

        public static void printNoExisteCombinacion()
        {
            Console.WriteLine("\n-------------------------------------------------------------");
            Console.WriteLine("No existe ninguna combinacion que sume 15 con la carta bajada");
            Console.WriteLine("-------------------------------------------------------------");
        }

        public static void printAvisoReparticion(Jugador jugador_repartidor)
        {
            Console.WriteLine("\n-------------------------------------------------------------");
            Console.WriteLine("Jugador " + jugador_repartidor.numero_jugador + "( " + jugador_repartidor.nombre + " )" + " reparte!");
            Console.WriteLine("-------------------------------------------------------------\n");
        }

        public static void printEscobaEnReparticion(Jugador jugador_repartidor, int numero_escobas)
        {
            Console.WriteLine("\n");
            Console.WriteLine("Jugador " + jugador_repartidor.numero_jugador + "( " + jugador_repartidor.nombre + " )" + " hizo " + numero_escobas + " escobas mientras repartia!\n");
        }

        public static void printCartasGanadas(Juego game)
        {
            Console.WriteLine("\n-------------------------------------------------------------");
            Console.WriteLine("Se jugaron todas las cartas de la baraja!!");
            Console.WriteLine("Cartas ganadas por cada jugador:\n");
            foreach (var jugador in game.jugadores)
            {
                Console.WriteLine("Jugador " + jugador.numero_jugador + "( " + jugador.nombre + " )" + " tiene " + jugador.pila_acumulada.Count + " cartas:");
                printCartasInline(jugador.pila_acumulada);
            }
            Console.WriteLine("-------------------------------------------------------------\n");
        }

        public static void printPuntajes(Juego game)
        {
            Console.WriteLine("\n-------------------------------------------------------------");
            Console.WriteLine("Total puntos ganados por cada jugador:\n");
            foreach (var jugador in game.jugadores)
            {
                Console.WriteLine("Jugador " + jugador.numero_jugador + "( " + jugador.nombre + " )" + " : " + jugador.puntos + " puntos");
            }
            Console.WriteLine("-------------------------------------------------------------\n");
        }

        public static void printCartasInline(List<Carta> cartas)
        {
            string linea_print = "";
            foreach (var carta in cartas)
            {
                linea_print += carta.nombre + ", ";
            }
            Console.WriteLine(linea_print);
        }

        public static void printGanador(Jugador ganador, Jugador perdedor)
        {
            Console.WriteLine("\n-------------------------------------------------------------");
            Console.WriteLine("Juego terminado! Se llego a los 15 puntos");
            Console.WriteLine("Jugador " + ganador.numero_jugador + "( " + ganador.nombre + " )" + " gana con: " + ganador.puntos + "puntos!!");
            Console.WriteLine("Jugador " + perdedor.numero_jugador + "( " + perdedor.nombre + " )" + " pierde con: " + perdedor.puntos + "puntos!!");
            Console.WriteLine("-------------------------------------------------------------\n");
        }

        public static void printEmpate(Jugador jugador1, Jugador jugador2)
        {
            Console.WriteLine("\n-------------------------------------------------------------");
            Console.WriteLine("Juego terminado! Se llego a los 15 puntos");
            Console.WriteLine("Jugador " + jugador1.numero_jugador + "( " + jugador1.nombre + " )" + " y Jugador " + jugador2.numero_jugador + "( " + jugador2.nombre + " )" + " empatan con: " + jugador1.puntos + "puntos!!");
            Console.WriteLine("-------------------------------------------------------------\n");
        }

    }
}