/* Sistema que detecta las colisiones con los obstaculos y con las manos.
 * Cuando el jugador choca contra alguno de esos elementos, el juego termina.
 * Para que el juego termine, se crea una entidad auxiliar, con el FinJuegoComponent,
 * que activa el FinJuegoSystem. */

using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;

public class ColisionObstaculo: JobComponentSystem {

    //Grupos de componentes y entidades que intervienen
    ComponentGroup m_ObstaculosActivos;
    EntityArray obstaculosActivos;
    ComponentGroup m_PalillosActivos;
    EntityArray palillosActivos;
    ComponentGroup m_Manos;
    EntityArray manos;
    ComponentGroup m_Player;
    EntityArray players;

    EntityArchetype finJuegoArchetype;
        
    //Comprueba si ha habido una colision, comparando la posicion del jugador con las de los obstaculos activos
    private struct DetectarColision : IJob {

        public EntityArray obstaculosActivos;
        public EntityArray players;
        public EntityArray manos;
        [ReadOnly] public ComponentDataArray<Position> posicionesObstaculos;
        [ReadOnly] public ComponentDataArray<Position> posicionesPlayers;
        [ReadOnly] public ComponentDataArray<Position> posicionesManos;
        [ReadOnly] public EntityCommandBuffer Commands;
        public EntityArchetype finJuegoArchetype;      

        public void Execute() {
            for (int i = 0; i < players.Length; i++) {
                //Comparar la posicion del jugador con la de todos los obstaculos que hay en pantalla
                for (int j = 0; j < obstaculosActivos.Length; j++) {
                    float dist = math.distance(posicionesPlayers[i].Value, posicionesObstaculos[j].Value);

                    if (dist < 1.2f) {    
                        Commands.DestroyEntity(players[0]);
                        Commands.CreateEntity(finJuegoArchetype); //Crear entidad de fin de juego si han chocado                        
                    }
                }

                //Comparar la posicion del jugador con las de las manos
                for (int k = 0; k < manos.Length; k++) {
                    float dist = math.distance(posicionesPlayers[i].Value, posicionesManos[k].Value);

                    if (dist < 2.2f) {
                        Commands.DestroyEntity(players[0]);
                        Commands.CreateEntity(finJuegoArchetype); //Crear entidad de fin de juego si han chocado                     
                    }
                }
            }
        }
    }

    private struct DetectarColisionPalillos : IJob {

        public EntityArray palillosActivos;
        public EntityArray players;
        [ReadOnly] public ComponentDataArray<Position> posicionesPalillos;
        [ReadOnly] public ComponentDataArray<Position> posicionesPlayers;
        [ReadOnly] public EntityCommandBuffer Commands;
        public EntityArchetype finJuegoArchetype;
        
        public void Execute() {
            for (int i = 0; i < players.Length; i++) {
                //Comparar la posicion del jugador con la de todos los palillos que hay en pantalla
                for (int j = 0; j < palillosActivos.Length; j++) {  

                    float dist = math.distance(posicionesPlayers[i].Value.y, posicionesPalillos[j].Value.y);

                    if (dist < 1f) {
                        if (posicionesPlayers[i].Value.x == 0f || posicionesPalillos[j].Value.x == posicionesPlayers[i].Value.x) {
                                                        
                            Commands.DestroyEntity(players[0]);
                            Commands.CreateEntity(finJuegoArchetype); //Crear entidad de fin de juego si han chocado                                               
                        }
                    }                    
                }                
            }
        }
    }


    protected override void OnCreateManager() {
        base.OnCreateManager();
        //Obtener grupos de componentes que contengan los componentes indicados (obstaculos, palillos y manos activas, jugador)
        m_ObstaculosActivos = GetComponentGroup(typeof(ObstaculoComponent), typeof(ObstaculoActivoComponent), typeof(Position));
        m_PalillosActivos = GetComponentGroup(typeof(PalillosComponent), typeof(Position));
        m_Manos = GetComponentGroup(typeof(ManoComponent), typeof(ManoActivaComponent), typeof(Position));
        m_Player = GetComponentGroup(typeof(PlayerInputComponent), typeof(Position));

        finJuegoArchetype = EntityManager.CreateArchetype(typeof(FinJuegoComponent));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {

        //Arrays de obstaculos activos (en pantalla) y de sus posiciones
        obstaculosActivos = m_ObstaculosActivos.GetEntityArray();
        ComponentDataArray<Position> posicionesObstaculos = m_ObstaculosActivos.GetComponentDataArray<Position>();
        //Arrays de jugadores (solo hay uno realmente) y de sus posiciones
        players = m_Player.GetEntityArray();
        ComponentDataArray<Position> posicionesPlayers = m_Player.GetComponentDataArray<Position>();
        //Arrays de manos activas y de sus posiciones
        manos = m_Manos.GetEntityArray();
        ComponentDataArray<Position> posicionesManos = m_Manos.GetComponentDataArray<Position>();


        JobHandle job = new DetectarColision {
            obstaculosActivos = obstaculosActivos,
            posicionesObstaculos = posicionesObstaculos,
            players = players,
            posicionesPlayers = posicionesPlayers,
            manos = manos,
            posicionesManos = posicionesManos,
            finJuegoArchetype = finJuegoArchetype,
            Commands = _collisionBarrier.CreateCommandBuffer()
        }.Schedule(inputDeps);        

        //Arrays de palillos activos (en pantalla) y de sus posiciones
        palillosActivos = m_PalillosActivos.GetEntityArray();
        ComponentDataArray<Position> posicionesPalillos = m_PalillosActivos.GetComponentDataArray<Position>();

        var job2 = new DetectarColisionPalillos {
            palillosActivos = palillosActivos,
            posicionesPalillos = posicionesPalillos,
            players = players,
            posicionesPlayers = posicionesPlayers,            
            finJuegoArchetype = finJuegoArchetype,
            //finJuego = finJuego,
            Commands = _collisionBarrier.CreateCommandBuffer()
        };

        return job2.Schedule(job); //depende del primer job por los componentes de posicion

    }

    //Necesario para poder crear entidades desde el job
    public class CollisionBarrier : BarrierSystem {
    }
    [Inject] public CollisionBarrier _collisionBarrier;
}
