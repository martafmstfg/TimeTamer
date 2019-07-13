/*Sistema que detecta las colisiones entre un enemigo y el proyectil del jugador.
 * Se suman 5 puntos y se crea una entidad con el componente ExplosionCMP_NV
 * para que se ejecute el sistema ExplosionSYS_NV.
 * Tambien elimina la entidad del enemigo.
 */

using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;

namespace Naves {

    public class ColisionProyectilSYS_NV : JobComponentSystem {


        private ComponentGroup m_EnemigosActivos;
        private EntityArray enemigosActivos;
        private ComponentGroup m_Proyectil;
        private EntityArray proyectil;
        private Entity explosion;
        
        private bool isDragging = true;

        //Comprueba si ha habido una colision, comparando la posicion del proyectil con las de los enemigos activos
        private struct DetectarColisionProyectil : IJob {

            public EntityArray enemigosActivos;
            [ReadOnly] public EntityArray proyectil;
            [ReadOnly] public ComponentDataArray<Position> posicionesEnemigos;
            [ReadOnly] public ComponentDataArray<LocalToWorld> posicionesProyectil;
            [ReadOnly] public EntityCommandBuffer Commands;
            public Entity explosion;            

            public void Execute() {
                float3 posicionProyectil;
                for (int i = 0; i < proyectil.Length; i++) {
                    for (int j = 0; j < enemigosActivos.Length; j++) {
                        posicionProyectil = new float3(posicionesProyectil[i].Value.c3.x, posicionesProyectil[i].Value.c3.y, 0f);
                        float dist = math.distance(posicionProyectil, posicionesEnemigos[j].Value);                        

                        if (dist < 0.5f) {                           

                            //Eliminar enemigo 
                            Commands.DestroyEntity(enemigosActivos[j]);

                            Commands.AddComponent<ExplosionCMP_NV>(explosion, new ExplosionCMP_NV {
                                posicion = posicionesEnemigos[j].Value
                            });

                            //Sumar puntos
                            Bootstrap_NV.bootstrap_NV.SumarPuntos(5);
                        }
                    }
                }
            }

        }


        protected override void OnCreateManager() {
            base.OnCreateManager();
            //Obtener grupos de componentes que contengan los componentes indicados (enemigos activos y proyectil)
            m_EnemigosActivos = GetComponentGroup(typeof(EnemigoActivoCMP_NV), typeof(Position));
            m_Proyectil = GetComponentGroup(typeof(ProyectilJugadorCMP_NV), typeof(LocalToWorld));

            explosion = EntityManager.CreateEntity();
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

            if(isDragging == false) {
                //Arrays de enemigos activos y de sus posiciones
                enemigosActivos = m_EnemigosActivos.GetEntityArray();
                ComponentDataArray<Position> posicionesEnemigos = m_EnemigosActivos.GetComponentDataArray<Position>();
                //Arrays de jugadores (solo hay uno realmente) y de sus posiciones
                proyectil = m_Proyectil.GetEntityArray();
                ComponentDataArray<LocalToWorld> posicionesProyectil = m_Proyectil.GetComponentDataArray<LocalToWorld>();

                job = new DetectarColisionProyectil {
                    enemigosActivos = enemigosActivos,
                    posicionesEnemigos = posicionesEnemigos,
                    proyectil = proyectil,
                    posicionesProyectil = posicionesProyectil,
                    Commands = _collisionBarrier.CreateCommandBuffer(),
                    explosion = explosion
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


