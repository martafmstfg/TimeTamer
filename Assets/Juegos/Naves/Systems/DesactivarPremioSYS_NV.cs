/*Sistema que reduce en cada frame el tiempo restante para desactivar un premio
 * que esta activo en pantalla. Cuando llega a 0, elimina todos los componentes 
 * que se le habian annadido al activarlo.
 */ 

using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;

namespace Naves {

    public class DesactivarPremioSYS_NV : JobComponentSystem {
                     
        //Se ejecuta en paralelo sobre todos los premios activos para ir reduciendo el 
        // tiempo que les queda de estar activos en pantalla
        private struct DesactivarPremio : IJobProcessComponentDataWithEntity<PremioActivoCMP_NV> {

            [ReadOnly] public float dt;
            [ReadOnly] public EntityCommandBuffer Commands;

            public void Execute(Entity entity, int index, ref PremioActivoCMP_NV premioActivo) {

                //Reducir el tiempo segun los segundos que han pasado desde el frame anterior
                float tiempoPremio = premioActivo.tiempoActivo;
                tiempoPremio = Mathf.Max(0.0f, tiempoPremio - dt);
                bool desactivarPremio = tiempoPremio <= 0.0f;                
                
                //Si el tiempo ha llegado a 0, desactivar el premio quitandole los componentes
                if (desactivarPremio) {

                    Commands.RemoveComponent<PremioActivoCMP_NV>(entity);
                    Commands.RemoveComponent<MeshInstanceRenderer>(entity);
                    Commands.RemoveComponent<Position>(entity);
                    Commands.RemoveComponent<Scale>(entity);
                }
                //Si no, actualizar el tiempo que queda
                else {
                    premioActivo.tiempoActivo = tiempoPremio;
                }
            }
        }               

        protected override JobHandle OnUpdate(JobHandle inputDepts) {                      

            JobHandle jobPremio = new DesactivarPremio() {
                dt = Time.deltaTime,
                Commands = _collisionBarrier.CreateCommandBuffer()
            }.Schedule(this, inputDepts);

            jobPremio.Complete();

            return jobPremio;
        }

        //Necesario para poder crear entidades desde el job
        private class CollisionBarrier : BarrierSystem {
        }
        [Inject] private CollisionBarrier _collisionBarrier;
    }
}


