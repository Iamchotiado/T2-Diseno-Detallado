using Vista;
using ModeloCarta;
using ModeloJugador;


namespace ModeloJuego
{
    public static class GeneradorNumerosAleatorios
    {
        private const int RandomSeed = 0;
        private static Random rng = new Random(RandomSeed);

        public static double Generar () => rng.Next();
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
        public static IEnumerable<string> obtenerCombinacionesString(List<Carta> cartas_mesa, int suma_objetivo, string nombre_cartas) 
        {
            for (int i = 0; i < cartas_mesa.Count; i++) 
            {
                int faltante_suma_objetivo = suma_objetivo - cartas_mesa[i].puntos;
                string nombre_cartas_actual = cartas_mesa[i].nombre + "," + nombre_cartas;
                if (faltante_suma_objetivo == 0) 
                {
                    yield return nombre_cartas_actual;
                } 
                else 
                {
                    List<Carta> cartas_posibles = cartas_mesa.Take(i).Where(n => n.puntos <= suma_objetivo).ToList();
                    if (cartas_posibles.Count > 0) 
                    {
                        foreach (string string_combinacion in obtenerCombinacionesString(cartas_posibles, faltante_suma_objetivo, nombre_cartas_actual)) {
                            yield return string_combinacion;
                        }
                    }
                }
            }
        }

        public List<List<Carta>> obtenerObjetosCombinaciones(List<Carta> cartas)
        {
            List<List<Carta>> lista_todas_combinaciones = new List<List<Carta>>();
            foreach (string string_combinacion in obtenerCombinacionesString(cartas, 15, "")) {
                string string_combinacion_formateado = string_combinacion.Remove(string_combinacion.Length - 1);
                List<String> lista_string_combinacion = string_combinacion_formateado.Split(',').ToList();
                List<Carta> lista_cartas_combinacion = this.convertirNombreCartasEnObjetos(lista_string_combinacion, cartas);
                lista_todas_combinaciones.Add(lista_cartas_combinacion);
            }
            return lista_todas_combinaciones;
        }

        public Carta bajarCarta(int indice_carta)
        {
            Carta carta_bajada = this.jugadores[this.turno].mano[indice_carta];
            this.cartas_centro_mesa.Add(carta_bajada);
            this.jugadores[this.turno].mano.RemoveAt(indice_carta);
            return carta_bajada;
        }

        public void robarCartasDeLaMesa(List<Carta> combinacion_seleccionada)
        {
            Vistas.printCartasRobadasMesa(this.jugadores[this.turno], combinacion_seleccionada);
            pasarCartasRobadasAPilaJugador(combinacion_seleccionada, this.turno);
            chequearEscoba();
            this.numero_ultimo_jugador = this.turno;
        }

        public void chequearCartaJugada(int indice_carta)
        {
            Carta carta_bajada = this.bajarCarta(indice_carta);
            List<List<Carta>> combinations = this.eliminarCombinacionesSinCartaBajada(this.obtenerObjetosCombinaciones(this.cartas_centro_mesa), carta_bajada);
            if (combinations.Count == 0)
            {
                Vistas.printNoExisteCombinacion();
            }
            else if (combinations.Count == 1)
            {
                robarCartasDeLaMesa(combinations[0]);
            }
            else if (combinations.Count > 1)
            {
                int combinacion_seleccionada = Vistas.seleccionCombinacion(combinations);
                robarCartasDeLaMesa(combinations[combinacion_seleccionada]);
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

        public void robarCartasMesaReparticion(int numero_escobas, List<List<Carta>> combinaciones_escoba)
        {
            this.jugadores[this.jugador_repartidor].numero_escobas += numero_escobas;
            if (numero_escobas == 1)
            {
                this.pasarCartasRobadasAPilaJugador(combinaciones_escoba[0], this.jugador_repartidor);
            }
            else if (numero_escobas == 2)
            {
                this.pasarCartasRobadasAPilaJugador(combinaciones_escoba[0], this.jugador_repartidor);
                this.pasarCartasRobadasAPilaJugador(combinaciones_escoba[1], this.jugador_repartidor);
            }
            Vistas.printEscobaEnReparticion(this.jugadores[this.jugador_repartidor], numero_escobas);
        }

        public void chequearEscobaJugadorRepartidor()
        {
            List<List<Carta>> combinations = this.contarCombinacionesEscobaRepartidor(this.obtenerObjetosCombinaciones(this.cartas_centro_mesa));
            Vistas.printAvisoReparticion(this.jugadores[this.jugador_repartidor]);
            if (combinations.Count == 1)
            {
                this.robarCartasMesaReparticion(1, combinations);
            }
            else if (combinations.Count == 2)
            {
                this.robarCartasMesaReparticion(2, combinations);
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
            this.pasarDeRonda();
        }

        public void pasarDeRonda()
        {
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