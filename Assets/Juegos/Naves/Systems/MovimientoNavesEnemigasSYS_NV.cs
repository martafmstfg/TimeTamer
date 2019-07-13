/*Sistema que mueve las naves enemigas en la direccion que indique su componente.
 * Ademas, las rota hacia la nave aliada que tengan como objetivo.
 */ 

using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;

namespace Naves {

   [UpdateAfter(typeof(ColisionNaveHijaSYS_NV))]
   public class MovimientoNavesEnemigasSYS_NV : JobComponentSystem {

        private ComponentGroup m_NavesHijas;        

        //Mueve la nave en la direccion almacenada y la va rotando para que mire hacia ella
        private struct MoverNaveEnemiga : IJobProcessComponentData<EnemigoActivoCMP_NV, Position, Rotation> {

            [ReadOnly] public float dt;

            public void Execute([ReadOnly]ref EnemigoActivoCMP_NV enemigoActivo, ref Position position, ref Rotation rotation) {                               

                //Mover en la direccion leida
                position.Value += enemigoActivo.direccion * dt * 1.2f;

                //Rotar hacia la nave objetivo
                var forward = new Vector3(enemigoActivo.direccion.x, enemigoActivo.direccion.y, enemigoActivo.direccion.z);
                var rot = Quaternion.LookRotation(-forward, Vector3.forward);
                rotation.Value = rot;
                rotation.Value = rotation.Value * Quaternion.Euler(-90.0f, 0, 0);
            }
        } 

        protected override void OnCreateManager() {
            base.OnCreateManager();
            //Obtener grupos de componentes que contengan los componentes indicados (enemigos activos y proyecil)
            m_NavesHijas = GetComponentGroup(typeof(NaveHijaCMP_NV), typeof(Position));            
        }

        protected override JobHandle OnUpdate(JobHandle inputDepts) {
            
            JobHandle job = new JobHandle();

            if(m_NavesHijas.CalculateLength() > 0) {
                job = new MoverNaveEnemiga() {
                    dt = Time.fixedDeltaTime,

                }.Schedule(this, inputDepts);

                job.Complete();
            }            

            return job;       
        }        

    }

}


