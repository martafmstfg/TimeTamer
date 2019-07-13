/*Sistema que detecta las colisiones entre una nave aliada y una enemiga.
 * Se destruyen ambos objetos si chocan.
 */

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;


namespace Naves {

    [UpdateAfter(typeof(ColisionProyectilSYS_NV))]
    public class ColisionNaveHijaSYS_NV : JobComponentSystem {

        private ComponentGroup m_EnemigosActivos;
        private EntityArray enemigosActivos;
        private ComponentGroup m_NavesHijas;
        private EntityArray navesHijas;

        //Comprueba si ha habido una colision, comparando la posicion de las naves aliadas con las de los enemigos activos
        private struct ColisionNavesHijas : IJob {

            public EntityArray enemigosActivos;
            [ReadOnly] public EntityArray navesHijas;
            [ReadOnly] public ComponentDataArray<Position> posicionesEnemigos;
            [ReadOnly] public ComponentDataArray<Position> posicionesNavesHijas;
            [ReadOnly] public EntityCommandBuffer Commands;

            public void Execute() {
                for (int i = 0; i < navesHijas.Length; i++) {
                    for (int j = 0; j < enemigosActivos.Length; j++) {
                        float dist = math.distance(posicionesNavesHijas[i].Value, posicionesEnemigos[j].Value);

                        if (dist < 0.5f) {                                                       

                            //Eliminar enemigo
                            Commands.DestroyEntity(enemigosActivos[j]);

                            //Eliminar nave hija
                            Commands.DestroyEntity(navesHijas[i]);                                              
                        }
                    }
                }
            }
        }


        protected override void OnCreateManager() {
            base.OnCreateManager();
            //Obtener grupos de componentes que contengan los componentes indicados (enemigos activos y naves aliadas)
            m_EnemigosActivos = GetComponentGroup(typeof(EnemigoActivoCMP_NV), typeof(Position));
            m_NavesHijas = GetComponentGroup(typeof(NaveHijaCMP_NV), typeof(Position));            
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
         
            JobHandle job;

            //Arrays de enemigos activos y de sus posiciones
            enemigosActivos = m_EnemigosActivos.GetEntityArray();
            ComponentDataArray<Position> posicionesEnemigos = m_EnemigosActivos.GetComponentDataArray<Position>();
            //Arrays de naves aliadas y de sus posiciones
            navesHijas = m_NavesHijas.GetEntityArray();            
            ComponentDataArray<Position> posicionesNavesHijas = m_NavesHijas.GetComponentDataArray<Position>();

            job = new ColisionNavesHijas {
                enemigosActivos = enemigosActivos,
                posicionesEnemigos = posicionesEnemigos,
                navesHijas = navesHijas,
                posicionesNavesHijas = posicionesNavesHijas,
                Commands = _collisionBarrier.CreateCommandBuffer()
            }.Schedule(inputDeps);

            job.Complete();

            return job;
        }
               

        //Necesario para poder crear entidades desde el job
        private class CollisionBarrier : BarrierSystem {
        }
        [Inject] private CollisionBarrier _collisionBarrier;

    }
}

