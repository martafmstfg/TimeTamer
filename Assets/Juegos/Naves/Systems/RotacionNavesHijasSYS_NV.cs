/*Sistema que en cada frame rota ligeramente las naves aliadas en el eje z, en sentido
 * horario o antihorario, hasta que alcanzan los 45 grados y cambian de direccion.
 */ 

using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Naves {

    public class RotacionNavesHijasSYS_NV : JobComponentSystem {
        
        private struct MoverNaveHija : IJobProcessComponentData<NaveHijaCMP_NV, Rotation> {      
            
            public float direccion;

            public void Execute([ReadOnly]ref NaveHijaCMP_NV naveHijaCMP_NV, ref Rotation rotation) {

                float direccion = naveHijaCMP_NV.direccion; //Sentido en el que se va a rotar

                Quaternion rotMax = Quaternion.Euler(0.0f, 0.0f, 45.0f*direccion); //Grados maximos

                //Si no se han alcanzado los grados maximos, rota ligeramente la nave en la direccion correspondiente
                if (rotation.Value != rotMax) {
                    rotation.Value = math.mul(math.normalizesafe(rotation.Value), Quaternion.Euler(0.0f, 0.0f, 0.5f*direccion));
                } else {
                    //Si ha alcanzado los grados maximos, invierte el sentido de la rotacion
                    naveHijaCMP_NV.direccion = direccion * (-1);
                }                
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDepts) {

            JobHandle job = new MoverNaveHija() {

            }.Schedule(this, inputDepts);

            job.Complete();

            return job;
        }


    }

}

