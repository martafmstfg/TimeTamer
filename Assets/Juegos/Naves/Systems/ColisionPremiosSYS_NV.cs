/*Sistema que detecta las colisiones entre un premio y el proyectil del jugador.
 * Se suma una a las monedas obtenidas y se crea una entidad con el componente
 * SonidoMonedaComponent para que se ejecute el sistema SonidoMonedaSYS_NV.
 * Tambien desactiva el premio, quitandole sus componentes.
 */

using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Collections;

namespace Naves {

    public class ColisionPremiosSYS_NV : JobComponentSystem {

        private ComponentGroup m_PremiosActivos;
        private EntityArray premiosActivos;
        private ComponentGroup m_Proyectil;
        private EntityArray proyectil;

        private bool isDragging = true;

        Entity sonidoMoneda;

        //Comprueba si ha habido una colision, comparando la posicion del proyectil con las de los premios activos
        private struct DetectarColisionProyectil : IJob {

            public EntityArray premiosActivos;
            [ReadOnly] public ComponentDataArray<Position> posicionesPremios;
            [ReadOnly] public ComponentDataArray<LocalToWorld> posicionesProyectil;
            [ReadOnly] public EntityCommandBuffer Commands;
            public Entity sonidoMoneda;
            
            public void Execute() {
                float3 posicionProyectil;
                for (int i = 0; i < posicionesProyectil.Length; i++) {
                    for (int j = 0; j < premiosActivos.Length; j++) {
                        posicionProyectil = new float3(posicionesProyectil[i].Value.c3.x, posicionesProyectil[i].Value.c3.y, 0f);
                        float dist = math.distance(posicionProyectil, posicionesPremios[j].Value);                        

                        if (dist < 0.5f) {
                            Commands.AddComponent<SonidoMonedaComponent>(sonidoMoneda, new SonidoMonedaComponent());

                            //Desactivar premio quitandole los componentes a la entidad
                            Commands.RemoveComponent<MeshInstanceRenderer>(premiosActivos[j]);
                            Commands.RemoveComponent<Position>(premiosActivos[j]);
                            Commands.RemoveComponent<Scale>(premiosActivos[j]);
                            Commands.RemoveComponent<PremioActivoCMP_NV>(premiosActivos[j]);

                            //Sumar puntos
                            Bootstrap_NV.bootstrap_NV.SumarMonedas(1);
                        }
                    }
                }
            }
        }

        protected override void OnCreateManager() {
            base.OnCreateManager();
            //Obtener grupos de componentes que contengan los componentes indicados (enemigos activos y proyecil)
            m_PremiosActivos = GetComponentGroup(typeof(PremioActivoCMP_NV), typeof(Position));
            m_Proyectil = GetComponentGroup(typeof(ProyectilJugadorCMP_NV), typeof(LocalToWorld));

            sonidoMoneda = EntityManager.CreateEntity();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {

            JobHandle job = new JobHandle();

            //Detectar toques
            if (Input.touchCount > 0) {
                //Primer toque
                if (Input.touches[0].phase == TouchPhase.Began) {

                    isDragging = true; //Se esta deslizando el dedo
                }
                //Fin del toque (o cancelacion, por si acaso)
                else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled) {
                    isDragging = false;
                }
            }

            if (isDragging == false) {
                //Arrays de premios activos y de sus posiciones
                premiosActivos = m_PremiosActivos.GetEntityArray();
                ComponentDataArray<Position> posicionesPremios = m_PremiosActivos.GetComponentDataArray<Position>();
                //Arrays de jugadores (solo hay uno realmente) y de sus posiciones
                proyectil = m_Proyectil.GetEntityArray();
                ComponentDataArray<LocalToWorld> posicionesProyectil = m_Proyectil.GetComponentDataArray<LocalToWorld>();

                job = new DetectarColisionProyectil {
                    premiosActivos = premiosActivos,
                    posicionesPremios = posicionesPremios,
                    sonidoMoneda = sonidoMoneda,
                    posicionesProyectil = posicionesProyectil,
                    Commands = _collisionBarrier.CreateCommandBuffer()
                }.Schedule(inputDeps);

                job.Complete();
            }

            return job;
        }

        //Necesario para poder crear entidades desde el job
        private class CollisionBarrier : BarrierSystem {
        }
        [Inject] private CollisionBarrier _collisionBarrier;

    }

}

