/*Sistema que calcula la posicion a la que se tiene que mover el proyectil 
 * en cada frame, en base a su posicion original y a la distancia que se ha estirado.
 * Si ya ha alcanzado la posicion final, vuelve a su posicíon inicial.
 */ 

using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;

namespace Naves {

    public class MovimientoProyectilSYS_NV : JobComponentSystem {

        private struct MoverProyectil : IJobProcessComponentData<ProyectilJugadorCMP_NV, Scale, Position> {

            [ReadOnly] public float dt;

            public void Execute(ref ProyectilJugadorCMP_NV proyectilJugador, [WriteOnly] ref Scale scale, ref Position position) {

                float posYFinal = proyectilJugador.posOriginal.y + proyectilJugador.distY;
                Vector3 posFinal = new Vector3(position.Value.x, posYFinal, position.Value.z);

                //Si ya ha alcanzado la posicion final, devolver a la inicial
                if (position.Value.y == posYFinal) {
                    position.Value = proyectilJugador.posOriginal;
                    //Reiniciar tambien la escala 
                    scale.Value.y = proyectilJugador.escalaYOriginal;

                    //Poner distancia a 0 porque ya se ha recorrido una vez
                    proyectilJugador.distY = 0;
                }
                //Si no, mover hacia la posicion final
                else {
                    //MoveTowards mueve el objeto una distancia determinada cada frame, hasta alcanzar la posicion final
                    position.Value = Vector3.MoveTowards(position.Value, posFinal, 13f * dt);
                }               
            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDepts) {

            JobHandle job = new MoverProyectil() {
                dt = Time.fixedDeltaTime
            }.Schedule(this, inputDepts);

            job.Complete();

            return job;
        }
    }

}


