/* Script que detecta el input tactil del jugador.
 * Agreagado al GameObject InputManager.
 */


using UnityEngine;
using Zenject;

public class InputManager_NO : MonoBehaviour {

    private bool isDragging = false; //Indica si el jugador sigue deslizando el dedo
    public Vector2 startTouch, swipeDelta; //Punto inicial donde se coloca el dedo y distancia de swipe
    Vector3 touchPosition = Vector3.zero;

    private int direccion;

    //Tiene como dependencia el GameManager
    GameManager_NO _gm;
    [Inject]
    public void Init(GameManager_NO gm) {
        _gm = gm;
    }    
	
	void Update () {

        //Resetear direccion
        direccion = -1;

        //Detectar toques
        if (Input.touchCount > 0) {
            //Primer toque
            if (Input.touches[0].phase == TouchPhase.Began) {
                startTouch = Input.touches[0].position; //Posicion donde se toca por primera vez
                isDragging = true; //Se esta deslizando el dedo
            }
            //Fin del toque (o cancelacion, por si acaso)
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled) {
                Reset(); //reiniciar variables auxiliares     
            }
        }

        //Calcular distancia que se ha arrastrado
        swipeDelta = Vector2.zero;
        if (isDragging) {
            if (Input.touchCount > 0) {
                //Distancia entre posicion actual del dedo y posicion donde se ha pulsado por primera vez
                swipeDelta = Input.touches[0].position - startTouch;
            }
        }

        //Calcular direccion de arrastre, en base a la distancia, si se ha pasado el umbral        
        if (swipeDelta.magnitude > 30) {
            direccion = CalcularDireccion();
            //Llamar metodo move del game manager, pasandole el id de la direccion calculada
            _gm.Mover(direccion);
            Reset();
        }

        //Input con flechas del teclado para pruebas en el editor
        if (Input.GetKeyDown(KeyCode.DownArrow)) _gm.Mover(1);
        else if (Input.GetKeyDown(KeyCode.UpArrow)) _gm.Mover(0);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) _gm.Mover(3);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) _gm.Mover(2);
        else if (Input.GetKeyDown(KeyCode.Space)) _gm.GenerarEstrella();
    }

    //Metodo y job para reiniciar las variables entre frames
    private void Reset() {
        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
    }

    public int CalcularDireccion () {
        float x, y;

        x = swipeDelta.x;
        y = swipeDelta.y;

        //Derecha o izquierda
        if (Mathf.Abs(x) > Mathf.Abs(y)) {

            //Derecha = 3, izquierda = 2
            if (x > 0) {
                return 3;
            }
            else {
                return 2;
            }
        }
        //Arriba o abajo
        else {

            //Arriba = 0, abajo = 1
            if (y > 0) {
                return 0;
            }
            else {
                return 1;
            }
        }
    }
}
