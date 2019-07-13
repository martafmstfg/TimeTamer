/* Sistema que detecta las colisiones con los premios.
 * Cuando el jugador choca contra una moneda, se llama al metodo de Bootstrap
 * que suma la cantidad indicada al numero total de monedas.
 * Tambien desactiva el premio para que desaparezca de la pantalla*/

using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Collections;

public class ColisionPremio : JobComponentSystem {

    ComponentGroup m_PremiosActivos;
    EntityArray premiosActivos;
    ComponentGroup m_Player;
    EntityArray players;

    Entity sonidoMoneda;

    //Comprueba si ha habido una colision, comparando la posicion del jugador con las de los premios activos
    private struct DetectarColision : IJob {

        public EntityArray premiosActivos;
        public EntityArray players;
        [ReadOnly] public ComponentDataArray<Position> posicionesPremios;
        [ReadOnly] public ComponentDataArray<Position> posicionesPlayers;
        [ReadOnly] public EntityCommandBuffer Commands;
        public Entity sonidoMoneda;

        public void Execute() {            

            for (int i = 0; i < players.Length; i++) {
                for (int j = 0; j < premiosActivos.Length; j++) {
                    float dist = math.distance(posicionesPlayers[i].Value, posicionesPremios[j].Value);
                    
                    if (dist < 1.0f) {                        
                        Commands.AddComponent<SonidoMonedaComponent>(sonidoMoneda, new SonidoMonedaComponent());

                        Debug.Log("premio");
                                                
                        //Desactivar premio quitandole los componentes a la entidad del premio                        
                        Commands.RemoveComponent<MeshInstanceRenderer>(premiosActivos[j]);
                        Commands.RemoveComponent<Position>(premiosActivos[j]);
                        Commands.RemoveComponent<PremioActivoComponent>(premiosActivos[j]);

                        //Sumar puntos
                        Bootstrap.bootstrap.SumarMonedas(1);
                    }
                }
            }
        }
    }


    protected override void OnCreateManager() {
        base.OnCreateManager();
        //Obtener grupos de componentes que contengan los componentes indicados (premios activos y jugador)
        m_PremiosActivos = GetComponentGroup(typeof(PremioComponent), typeof(PremioActivoComponent), typeof(Position));
        m_Player = GetComponentGroup(typeof(PlayerInputComponent), typeof(Position));

        sonidoMoneda = EntityManager.CreateEntity();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {

        //Arrays de premios activos y de sus posiciones
        premiosActivos = m_PremiosActivos.GetEntityArray();
        ComponentDataArray<Position> posicionesPremios = m_PremiosActivos.GetComponentDataArray<Position>();
        //Arrays de jugadores (solo hay uno realmente) y de sus posiciones
        players = m_Player.GetEntityArray();
        ComponentDataArray<Position> posicionesPlayers = m_Player.GetComponentDataArray<Position>();

        var job = new DetectarColision {
            premiosActivos = premiosActivos,
            posicionesPremios = posicionesPremios,
            players = players,
            posicionesPlayers = posicionesPlayers,
            sonidoMoneda = sonidoMoneda,
            Commands = _collisionBarrier.CreateCommandBuffer()
        };

        return job.Schedule(inputDeps);
    }

    //Necesario para poder crear entidades desde el job
    private class CollisionBarrier : BarrierSystem {
    }
    [Inject] private CollisionBarrier _collisionBarrier;
}
