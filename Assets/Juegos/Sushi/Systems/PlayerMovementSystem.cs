/* Sistema que aplica a la entidad del jugador el movimiento 
 * en la direccion X calculada por el PlayerInputSystem. */
 
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;

public class PlayerMovementSystem : JobComponentSystem {

    //Utiliza el metodo MoveTowards para mover a la entidad hacia el carril correspondiente,
    // cuya posicion se almacena en el PlayerInputComponent.    
    private struct MoveToPosition : IJobProcessComponentData<Position, PlayerInputComponent> {
                
        public float dt;

        public void Execute(ref Position position, [ReadOnly] ref PlayerInputComponent playerInputComponent) {

            //Si se ha deslizado a la izquierda...
            if (playerInputComponent.swipeLeft == 1) {
                //...y esta en el carril del medio (x=0), se mueve al carril izquierdo (x=-1.53)
                if(position.Value.x == 0f) {
                    playerInputComponent.posicionFinal = new Vector3(-1.53f, position.Value.y, position.Value.z);
                }
                //Si esta en el carril derecho (x>0), se mueve al del centro (x=0)
                else if (position.Value.x > 0) {
                    playerInputComponent.posicionFinal = new Vector3(0f, position.Value.y, position.Value.z);
                } 
            }
            //Si se ha deslizado hacia la derecha...
            else if (playerInputComponent.swipeRight == 1) {
                //... y esta en el carril del medio (x=0), se mueve al carril derecho
                if (position.Value.x == 0f) {
                    playerInputComponent.posicionFinal = new Vector3(1.53f, position.Value.y, position.Value.z);
                }
                //Si esta en el carril izquierdo (x<0), se mueve al del centro (x=0)
                else if (position.Value.x < 0) {
                    playerInputComponent.posicionFinal = new Vector3(0f, position.Value.y, position.Value.z);
                }                
            }

            //MoveTowards mueve el objeto una distancia determinada cada frame, hasta alcanzar la posicion final
            position.Value = Vector3.MoveTowards(position.Value, playerInputComponent.posicionFinal, 7.5f * dt);            
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDepts) {
               
        //Planificar el job y devolver su handle
        return new MoveToPosition() {
            dt = Time.fixedDeltaTime
        }.Schedule(this, inputDepts);

    }
}
