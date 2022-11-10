using Vista;
using Modelos;

namespace Controlador 
{
    public class FlujoJuego
    {
        public void jugar()
        {
            var option = Vistas.printMenu();
            switch (option)
            {
                case "1":
                    Juego game = setLocal();
                    jugarLocal(game);
                    break;
                case "2":
                    break;
            }
        }

        public Juego crearJuegoYAgregarJugadores(Jugador player1, Jugador player2)
        {
            Juego game = new Juego();
            game.agregarJugador(player1);
            game.agregarJugador(player2);
            return game;
        }

        public Juego setLocal()
        {
            var name = Vistas.nombrarJugador(1);
            Jugador player1 = new Jugador(name, "1");
            name = Vistas.nombrarJugador(2);
            Jugador player2 = new Jugador(name, "2");
            return this.crearJuegoYAgregarJugadores(player1, player2);
        }

        public void jugarLocal(Juego game)
        {
            while(!game.hayGanador())
            {
                procesoReparticion(game);
                procesoBajadaDeCarta(game);
                game.recontarPuntos();
            }
        }

        public void procesoReparticion(Juego game)
        {
            game.crearBaraja();
            game.barajar();
            game.repartirCartasAJugadores();
            game.repartirCartasCentroMesa();
            game.chequearEscobaJugadorRepartidor();
        }

        public void procesoBajadaDeCarta(Juego game)
        {
            while(game.baraja.Count != 0 || game.jugadores[0].mano.Count != 0 || game.jugadores[1].mano.Count != 0)
            {
                int carta_bajada = Vistas.seleccionCartaBajada(game.cartas_centro_mesa, game.jugadores[game.turno]);
                game.chequearCartaJugada(carta_bajada);
                game.chequearTodasCartasJugadas();
                game.chequearCantidadCartasEnMano();
                game.avanzarTurno();
            }
        }
    }
    
}