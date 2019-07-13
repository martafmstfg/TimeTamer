/* Sistema para eliminar las naves enemigas cuando llegan al borde de la pantalla.*/

using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

namespace Naves {

    public class EliminarEnemigosLimitesSYS_NV : JobComponentSystem {

        //Comprueba si alguno de los enemigos activos ha pasado de los bordes de la pantalla para eliminarlo 
        [RequireComponentTag(typeof(EnemigoActivoCMP_NV))]
        private struct EliminarEnemigo : IJobProcessComponentDataWithEntity<Position> {

            [ReadOnly] public EntityCommandBuffer Commands;
            [ReadOnly] public float bordeSup, bordeInf, bordeDch, bordeIzq;

            public void Execute(Entity entity, int index, [ReadOnly] ref Position position) {

                //Comprobar si ha cruzado el limite
                if (position.Value.x > bordeDch || position.Value.x < bordeIzq ||
                    position.Value.y < bordeInf || position.Value.y > bordeSup) {
                    
                    //Eliminar enemigo
                    Commands.DestroyEntity(entity);
                }
            }
        }

        protected override void OnCreateManager() {
            base.OnCreateManager();            
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            

            JobHandle job = new EliminarEnemigo {                          
                Commands = _collisionBarrier.CreateCommandBuffer(),
                bordeSup = 8.2f,
                bordeInf = -7f,
                bordeDch = 4f,
                bordeIzq = -4f

            }.Schedule(this, inputDeps); 

            return job;
        }

        //Necesario para eliminar componentes de una entidad desde un job
        private class CollisionBarrier : BarrierSystem {
        }
        [Inject] private CollisionBarrier _collisionBarrier;
    }

}


