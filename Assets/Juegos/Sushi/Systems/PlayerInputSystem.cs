/* Sistema que detecta y guarda el input del jugador: desliza el dedo hacia 
 * la derecha o hacia la izquierda para cambiar de carril.
 * Este sistema NO aplica el movimiento sobre el jugador.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

public class PlayerInputSystem : JobComponentSystem {

    private bool isDragging = false; //Indica si el jugador sigue deslizando el dedo
    private Vector2 startTouch, swipeDelta; //Punto inicial donde se coloca el dedo y distancia de swipe
    Vector3 touchPosition = Vector3.zero; 

    //Metodo y job para reiniciar las variables entre frames
    private void Reset () {
        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
    }

    private struct ResetBoolsDireccion : IJobProcessComponentData<PlayerInputComponent> {
        
        public void Execute(ref PlayerInputComponent playerInputComponent) {
            playerInputComponent.swipeLeft = playerInputComponent.swipeRight = 0;
        }
    }

    //Guarda en el component la direccion en la que ha deslizado el dedo el jugador
    // (es decir, la direccion en la que PlayerMovementSystem debe mover el sprite)
    // en base a si la distancia de swipe calculada es`positiva (dcha) o negativa (izqda)
    private struct SetBoolsDireccion : IJobProcessComponentData<PlayerInputComponent> {

        public Vector2 swipeDelta;        

        public void Execute(ref PlayerInputComponent playerInputComponent) {
            if (swipeDelta.x < 0) {
                playerInputComponent.swipeLeft = 1;
                playerInputComponent.swipeRight = 0;
            } else if (swipeDelta.x > 0) {
                playerInputComponent.swipeRight = 1;
                playerInputComponent.swipeLeft = 0;
            }            
        }
    }            
    
    protected override JobHandle OnUpdate(JobHandle inputDepts) {                

        //Resetear bools direccion del PlayerInputComponent
        JobHandle resetInicial = new ResetBoolsDireccion() { }.Schedule(this, inputDepts);
        resetInicial.Complete ();

        //Detectar toques
        if(Input.touchCount > 0) {
            //Primer toque
            if(Input.touches[0].phase == TouchPhase.Began) {
                startTouch = Input.touches[0].position; //Posicion donde se toca por primera vez
                isDragging = true; //Se esta deslizando el dedo
            }
            //Fin del toque (o cancelacion, por si acaso)
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled) {
                Reset(); //reiniciar variables auxiliares      
                Bootstrap.bootstrap.PlaySound(3); //reproducir sonido deslizarse
            }
        }

        //Calcular distancia que se ha arrastrado
        swipeDelta = Vector2.zero;
        if(isDragging) {
            if (Input.touchCount > 0) {
                //Distancia entre posicion actual del dedo y posicion donde se ha pulsado por primera vez
                swipeDelta = Input.touches[0].position - startTouch; 
            }
        }

        //Calcular direccion de arrastre, en base a la distancia, si se ha pasado el umbral
        //swipedelta originalmente > 30
        JobHandle setBoolsDireccion = new JobHandle();
        if (swipeDelta.magnitude > 20) {
            setBoolsDireccion = new SetBoolsDireccion() { swipeDelta = swipeDelta }.Schedule(this, inputDepts);
            setBoolsDireccion.Complete();

            Reset();
        }

        //JobHandle empty = new Empty() { }.Schedule(this, inputDepts);
        //empty.Complete();
        return setBoolsDireccion;
    }
}
