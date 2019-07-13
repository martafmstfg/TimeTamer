/*Sistema que modifica la escala en el eje Y del proyectil para estirarlo, 
 * en funcion de la distancia que se haya deslizado el dedo.
 */ 

using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;

namespace Naves {

    public class EstirarProyectilSYS_NV : JobComponentSystem {

        private bool isDragging;

        //Estirar proyectil segun la distancia guardada en el componente
        public struct EstirarProyectilJugador : IJobProcessComponentData<ProyectilJugadorCMP_NV, Scale, Position> {

            public void Execute([ReadOnly] ref ProyectilJugadorCMP_NV proyectilJugador, [WriteOnly] ref Scale scale, ref Position position) {
                                
                float distancia = proyectilJugador.distY;
                float original = proyectilJugador.escalaYOriginal;
                   
                //Para deformar el proyectil tomando como punto de referencia el centro de la pantalla, 
                // en lugar del centro del plano (ya que en Unity no se puede modificar el centro de la mesh), 
                // hay que realizar unos calculos adicionales
                Vector3 A = position.Value; //centro original del objeto
                Vector3 B = new Vector3(0, 0, 0); //punto de referencia desde el que se quiere estirar

                Vector3 C = A - B; // Distancia desde el centro original hasta el punto de referencia

                float RS = -(original + distancia*0.3f) / A.y; // Factor de escalado relativo segun la distancia anterior

                //Calcular la posicion final del objeto despues de escalar
                Vector3 FP = B + C * RS;

                //Escalar y desplazar
                scale.Value.y = original + distancia * 0.3f;
                position.Value = FP + new Vector3(0, distancia * 0.1f, 0);

            }
        }       

        protected override JobHandle OnUpdate(JobHandle inputDepts) {

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

            //Solo llama al trabajo si se esta arrastrando (si no, se estiraría constantemente)
            if (isDragging) {
                if (Input.touchCount > 0) {
                    
                    job = new EstirarProyectilJugador () { }.Schedule(this, inputDepts);

                    job.Complete();
                }
            }

            return job;

        }
    }
}
