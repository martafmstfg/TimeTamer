/*Sistema que calcula la rotacion que hay que aplicarle a la nave del jugador
 * en funcion de input tactil del usuario. La guarda en el PlayerInputCMP_NV.
 */

using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;

namespace Naves {

    public class PlayerInputSYS_NV : JobComponentSystem {

        private bool isDragging;
        
        //Calcula la rotacion segun el punto hacia el que hay que rotar
        public struct ActualizarValorRotacionPlayer : IJobProcessComponentData <PlayerInputCMP_NV, Position> {
            
            [ReadOnly] public Vector3 rotateTo;                        

            public void Execute([WriteOnly] ref PlayerInputCMP_NV playerInputCMP, [ReadOnly] ref Position position) {
                //El vector hacia delante es el que va desde la posicion actual hasta el punto hacia el que hay que rotar
                var forward = rotateTo - new Vector3 (position.Value.x, position.Value.y);
                var rot = Quaternion.LookRotation(forward, Vector3.forward); //Funcion que calcula la rotacion dados dos vectores

                playerInputCMP.rotZ = rot; //Almacenar rotacion calculada          
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

            //Solo se ejecuta mientras se esta deslizando el dedo por la pantalla
            if (isDragging) {
                 if (Input.touchCount > 0) {                                       

                    //Punto donde se ha tocado en coordenadas del mundo
                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                    //Punto hacia el que hay que rotar
                    Vector3 rotateTo = new Vector3(touchPosition.x, touchPosition.y, 0);                                        

                    job = new ActualizarValorRotacionPlayer() {
                        rotateTo = rotateTo

                    }.Schedule(this, inputDepts);                  

                }
             } 

            return job;

        }
    }
}


