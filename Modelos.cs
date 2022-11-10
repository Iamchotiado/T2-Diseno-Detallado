using Vista;
namespace Modelos
{
    public static class GeneradorNumerosAleatorios
    {
        private const int RandomSeed = 0;
        private static Random rng = new Random(RandomSeed);

        public static double Generar () => rng.Next();
    }
    // clases juego la escoba
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

    public class Juego
    {
        public List<Jugador> jugadores { get; set; }
        public List<Carta> cartas_centro_mesa { get; set; }
        public List<Carta> baraja { get; set; }
        public int turno { get; set; }
        public int jugador_repartidor { get; set; }
        public int numero_ultimo_jugador { get; set; }

        public Juego()
        {
            this.jugadores = new List<Jugador>();
            this.cartas_centro_mesa = new List<Carta>();
            this.baraja = new List<Carta>();
            this.turno = 1;
            this.jugador_repartidor = 0;
            this.numero_ultimo_jugador = 0;
        }

        public void avanzarTurno()
        {
            if (this.baraja.Count != 0 || this.jugadores[0].mano.Count != 0 || this.jugadores[1].mano.Count != 0)
            {
                if (this.turno == 1)
                {
                    this.turno = 0;
                }
                else
                {
                    this.turno = 1;
                }
            }
        }

        public void cambiarJugadorRepartidor()
        {
            if (this.jugador_repartidor == 1)
            {
                this.jugador_repartidor = 0;
                this.turno = 1;
            }
            else
            {
                this.jugador_repartidor = 1;
                this.turno = 0;
            }
        }
        public void agregarJugador(Jugador jugador)
        {
            this.jugadores.Add(jugador);
        }

        public void crearBaraja()
        {
            string[] pintas = { "oro", "copa", "espada", "basto" };
            string[] valores = { "1", "2", "3", "4", "5", "6", "7", "sota", "caballo", "rey" };
            int[] puntos = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            for (int i = 0; i < pintas.Length; i++)
            {
                for (int j = 0; j < valores.Length; j++)
                {
                    Carta carta = new Carta(pintas[i], valores[j], puntos[j]);
                    this.baraja.Add(carta);
                }
            }
        }
        public void barajar()
        {
            Random rnd = new Random();
            this.baraja = this.baraja.OrderBy(x => GeneradorNumerosAleatorios.Generar()).ToList();
        }

        public void repartirCartasAJugadores()
        {
            int cantidad_cartas = 3;
            for (int i = 0; i < cantidad_cartas; i++)
            {
                foreach (Jugador jugador in this.jugadores)
                {
                    jugador.mano.Add(this.baraja[0]);
                    this.baraja.RemoveAt(0);
                }
            }
        }

        public void repartirCartasCentroMesa()
        {
            int cantidad_cartas = 4;
            for (int j = 0; j < cantidad_cartas; j++)
            {
                this.cartas_centro_mesa.Add(this.baraja[0]);
                this.baraja.RemoveAt(0);
            }
        }

        public List<Carta> convertirNombreCartasEnObjetos(List<string> nombres_cartas, List<Carta> cartas_a_chequear)
        {
            List<Carta> cartas = new List<Carta>();
            foreach (string nombre in nombres_cartas)
            {
                string[] nombre_carta = nombre.Split('_');
                string valor = nombre_carta[0].Trim();
                string pinta = nombre_carta[1].Trim();
                Carta carta = cartas_a_chequear.Find(x => x.valor == valor && x.pinta == pinta);
                cartas.Add(carta);
            }
            return cartas;
        }

        // https://stackoverflow.com/questions/10738926/efficient-algorithm-to-find-a-combination-which-summation-is-equal-to-a-known-n
        public static IEnumerable<string> GetCombinations(List<Carta> set, int sum, string values) 
        {
            for (int i = 0; i < set.Count; i++) 
            {
                int left = sum - set[i].puntos;
                string vals = set[i].nombre + "," + values;
                if (left == 0) 
                {
                    yield return vals;
                } 
                else 
                {
                    List<Carta> possible = set.Take(i).Where(n => n.puntos <= sum).ToList();
                    if (possible.Count > 0) 
                    {
                        foreach (string s in GetCombinations(possible, left, vals)) {
                            yield return s;
                        }
                    }
                }
            }
        }

        // check the combinations of cards that sum 15 in the deck
        public List<List<Carta>> checkCombinations(List<Carta> cartas)
        {
            List<List<Carta>> combinations = new List<List<Carta>>();
            foreach (string s in GetCombinations(cartas, 15, "")) {
                string combination_string = s.Remove(s.Length - 1);
                List<String> combination_string_list = combination_string.Split(',').ToList();
                List<Carta> combination_object_list = this.convertirNombreCartasEnObjetos(combination_string_list, cartas);
                combinations.Add(combination_object_list);
            }
            return combinations;
        }

        public void chequearCartaJugada(int indice_carta)
        {
            Carta carta_bajada = this.jugadores[this.turno].mano[indice_carta];
            this.cartas_centro_mesa.Add(carta_bajada);
            this.jugadores[this.turno].mano.RemoveAt(indice_carta);
            List<List<Carta>> combinations = this.eliminarCombinacionesSinCartaBajada(this.checkCombinations(this.cartas_centro_mesa), carta_bajada);
            
            if (combinations.Count == 0)
            {
                Vistas.printNoExisteCombinacion();
            }
            else if (combinations.Count == 1)
            {
                Vistas.printCartasRobadasMesa(this.jugadores[this.turno], combinations[0]);
                pasarCartasRobadasAPilaJugador(combinations[0], this.turno);
                chequearEscoba();
                this.numero_ultimo_jugador = this.turno;
            }
            else if (combinations.Count > 1)
            {
                int combinacion_seleccionada = Vistas.seleccionCombinacion(combinations);
                Vistas.printCartasRobadasMesa(this.jugadores[this.turno], combinations[combinacion_seleccionada]);
                pasarCartasRobadasAPilaJugador(combinations[combinacion_seleccionada], this.turno);
                chequearEscoba();
                this.numero_ultimo_jugador = this.turno;
            }

        }

        public List<List<Carta>> eliminarCombinacionesSinCartaBajada(List<List<Carta>> combinations, Carta carta_bajada)
        {
            List<List<Carta>> combinations_con_carta_bajada = new List<List<Carta>>();
            foreach (List<Carta> combination in combinations)
            {
                if (combination.Contains(carta_bajada))
                {
                    combinations_con_carta_bajada.Add(combination);
                }
            }
            return combinations_con_carta_bajada;
        }

        public void pasarCartasRobadasAPilaJugador(List<Carta> combinacion_seleccionada, int turno)
        {
            foreach (Carta carta in combinacion_seleccionada)
            {
                this.jugadores[turno].pila_acumulada.Add(carta);
                this.cartas_centro_mesa.Remove(carta);
            }
        }

        public void chequearEscoba() 
        {
            if (this.cartas_centro_mesa.Count == 0)
            {
                this.jugadores[this.turno].numero_escobas ++;
                Vistas.printAvisoEscoba(this.jugadores[this.turno]);
            }
        }

        public void chequearCantidadCartasEnMano()
        {
            bool se_roba = true;
            foreach (Jugador jugador in this.jugadores)
            {
                if (jugador.mano.Count > 0)
                {
                    se_roba = false;
                }
            }
            if (se_roba && this.baraja.Count > 0)
            {
                this.repartirCartasAJugadores();
            }
        }

        public void chequearTodasCartasJugadas()
        {
            if (this.baraja.Count == 0 && this.jugadores[0].mano.Count == 0 && this.jugadores[1].mano.Count == 0)
            {
                this.repatirCartasMesaUltimoJugador(this.numero_ultimo_jugador);
                this.cambiarJugadorRepartidor();
            }
        }

        public void repatirCartasMesaUltimoJugador(int indice_jugador)
        {
            this.jugadores[indice_jugador].pila_acumulada.AddRange(this.cartas_centro_mesa);
            this.cartas_centro_mesa.Clear();
        }

        public void chequearEscobaJugadorRepartidor()
        {
            List<List<Carta>> combinations = this.contarCombinacionesEscobaRepartidor(this.checkCombinations(this.cartas_centro_mesa));
            Vistas.printAvisoReparticion(this.jugadores[this.jugador_repartidor], 1);
            if (combinations.Count == 1)
            {
                this.jugadores[this.jugador_repartidor].numero_escobas ++;
                this.pasarCartasRobadasAPilaJugador(combinations[0], this.jugador_repartidor);
                Vistas.printEscobaEnReparticion(this.jugadores[this.jugador_repartidor], 1);
            }
            else if (combinations.Count == 2)
            {
                this.jugadores[this.jugador_repartidor].numero_escobas += 2;
                this.pasarCartasRobadasAPilaJugador(combinations[0], this.jugador_repartidor);
                this.pasarCartasRobadasAPilaJugador(combinations[1], this.jugador_repartidor);
                Vistas.printEscobaEnReparticion(this.jugadores[this.jugador_repartidor], 2);
            }
        }

        public List<List<Carta>> contarCombinacionesEscobaRepartidor(List<List<Carta>> combinaciones_posibles)
        {
            if (combinaciones_posibles.Count == 1 && combinaciones_posibles[0].Count == 4)
            {
                return combinaciones_posibles;
            }
            else if (combinaciones_posibles.Count == 2 && combinaciones_posibles[0].Count == 2 && combinaciones_posibles[1].Count == 2)
            {
                return chequearDosEscobas(combinaciones_posibles);
            }
            return new List<List<Carta>>();
        }

        public List<List<Carta>> chequearDosEscobas(List<List<Carta>> combinaciones_posibles)
        {
            List<Carta> cartas_dentro_combinaciones = new List<Carta>();
            foreach (List<Carta> combinacion in combinaciones_posibles)
            {
                foreach (Carta carta in combinacion)
                {
                    if (!cartas_dentro_combinaciones.Contains(carta))
                    {
                        cartas_dentro_combinaciones.Add(carta);
                    }
                    else
                    {
                        return new List<List<Carta>>();
                    }
                }
            }
            return combinaciones_posibles;
        }

        public void recontarPuntos()
        {
            foreach (Jugador jugador in this.jugadores)
            {
                jugador.puntos += jugador.numero_escobas;
                jugador.puntos += chequearSieteOro(int.Parse(jugador.numero_jugador) - 1);
                jugador.puntos += chequearMayoriaDeSietes(int.Parse(jugador.numero_jugador) - 1);
                jugador.puntos += chequearMayoriaBaraja(int.Parse(jugador.numero_jugador) - 1);
                jugador.puntos += chequearMayoriaPintaOro(int.Parse(jugador.numero_jugador) - 1);
            }
            this.mandarPrintCartasGanadasyPuntajes();
            this.llamarReseteoDeParametros();
        }

        public void mandarPrintCartasGanadasyPuntajes()
        {
            Vistas.printCartasGanadas(this);
            Vistas.printPuntajes(this);
        }

        public void llamarReseteoDeParametros()
        {
            this.resetearParametrosRonda(0);
            this.resetearParametrosRonda(1);
        }

        public int chequearSieteOro(int numero_jugador)
        {
            foreach (Carta carta in this.jugadores[numero_jugador].pila_acumulada)
            {
                if (carta.valor == "7" && carta.pinta == "oro")
                {
                    return 1;
                }
            }
            return 0;
        }

        public int chequearMayoriaDeSietes(int numero_jugador)
        {
            int numero_sietes = 0;
            foreach (Carta carta in this.jugadores[numero_jugador].pila_acumulada)
            {
                if (carta.valor == "7")
                {
                    numero_sietes ++;
                }
            }
            if (numero_sietes >= 2)
            {
                return 1;
            }
            return 0;           
        }

        public int chequearMayoriaBaraja(int numero_jugador)
        {
            int numero_cartas = this.jugadores[numero_jugador].pila_acumulada.Count;
            if (numero_cartas >= 20)
            {
                return 1;
            }
            return 0;           
        }

        public int chequearMayoriaPintaOro(int numero_jugador)
        {
            int numero_cartas = 0;
            foreach (Carta carta in this.jugadores[numero_jugador].pila_acumulada)
            {
                if (carta.pinta == "oro")
                {
                    numero_cartas ++;
                }
            }
            if (numero_cartas >= 5)
            {
                return 1;
            }
            return 0;           
        }

        public void resetearParametrosRonda(int numero_jugador)
        {
            this.jugadores[numero_jugador].pila_acumulada.Clear();
            this.jugadores[numero_jugador].numero_escobas = 0;
        }

        public bool hayGanador()
        {
            foreach (Jugador jugador in this.jugadores)
            {
                if (jugador.puntos >= 15)
                {
                    this.showGanador();
                    return true;
                }
            }
            return false;
        }

        public void showGanador()
        {
            if (this.jugadores[0].puntos > this.jugadores[1].puntos)
            {
                Vistas.printGanador(this.jugadores[0], this.jugadores[1]);
            }
            else if (this.jugadores[0].puntos < this.jugadores[1].puntos)
            {
                Vistas.printGanador(this.jugadores[1], this.jugadores[0]);
            }
            else
            {
                Vistas.printEmpate(this.jugadores[0], this.jugadores[1]);
            }   
        }

    }
}

