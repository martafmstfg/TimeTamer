/*Sistema que aplica la rotacion almacenada en el PlayerInputCMP_NV a la nave.*/

using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;

namespace Naves {

    
    public class PlayerMovementSYS_NV : JobComponentSystem {

        private bool isDragging;

        //Aplica la rotacion para mirar hacia el punto determinado
        public struct RotarPlayer : IJobProcessComponentData<PlayerInputCMP_NV, Rotation> {            
                                   
            public void Execute([ReadOnly] ref PlayerInputCMP_NV playerInputCMP, ref Rotation rotation) {                              

                rotation.Value = playerInputCMP.rotZ;

                rotation.Value = rotation.Value * Quaternion.Euler(-90.0f, 0, 0); //Rota en X para que se vea correctamente
            }
        }        

        protected override JobHandle OnUpdate(JobHandle inputDepts) {

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

            JobHandle job = new JobHandle();

            //Solo llama al trabajo si se esta arrastrando (si no, giraría constantemente)
            if (isDragging) {
                if (Input.touchCount > 0) {                                      

                    job = new RotarPlayer() {}.Schedule(this, inputDepts);

                }
            }

            return job;

        }
    }
    
}
