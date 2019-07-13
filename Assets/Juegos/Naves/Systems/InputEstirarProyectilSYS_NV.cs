/*Sistema que calcula la distancia que se ha arrastrado el dedo por la pantalla
 * y la guarda en el ProyectilJugadorCMP_NV para despues utilizarla para estirar
 * el proyectil del jugador.
 */ 

using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;

namespace Naves {

    public class InputEstirarProyectilSYS_NV : JobComponentSystem {

        private bool isDragging;

        //Calcular la distancia y guardarla en el componente
        public struct ActualizarValorDistancia : IJobProcessComponentData<ProyectilJugadorCMP_NV> {

            [ReadOnly] public Vector3 touchPosition;
            [ReadOnly] public float origenY; //Posicion inicial con la que calcular la distancia

            public void Execute([WriteOnly] ref ProyectilJugadorCMP_NV proyectilJugadorCMP) {

                float distancia = Mathf.Abs(touchPosition.y - origenY);

                proyectilJugadorCMP.distY = distancia;                
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDepts) {

            //Detectar toques
            if (Input.touchCount > 0) {
                //Primer toque
                if (Input.touches[0].phase == TouchPhase.Began) {

                    isDragging = true; //Se esta deslizando el dedo
                    Bootstrap_NV.bootstrap_NV.PlaySound(3);
                }
                //Fin del toque (o cancelacion, por si acaso)
                else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled) {
                    isDragging = false;
                    Bootstrap_NV.bootstrap_NV.PlaySound(4);
                }
            }

            JobHandle job = new JobHandle();

            if (isDragging) {
                if (Input.touchCount > 0) {

                    //Punto donde se ha tocado en coordenadas del mundo
                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                    
                    job = new ActualizarValorDistancia() {
                        touchPosition = touchPosition,
                        origenY = 0f

                    }.Schedule(this, inputDepts);

                    job.Complete();

                }
            }

            return job;

        }
    }
}

