/*Script que representa a una celda de la cuadricula.
 * Agregado a las 16 celdas que forman la cuadricula.
 */


using UnityEngine;
using UnityEngine.UI;

public class Celda : MonoBehaviour {

    public Sprite[] sprites; //Array de los sprites para cada nivel
    public Image imagen; //Muestra el sprite de la estrella
    private Animator animator;

    //Identificadores y posicion celda (se rellenan en el inspector)
    public int id;
    public int fila, columna;
    //Ids celdas adyacentes
    public int idDerecha, idIzquierda, idArriba, idAbajo;

    public int TipoEstrella {
        get {
            return tipoEstrella;
        }

        set {
            tipoEstrella = value;

            //Si el nivel es 0, la casilla no tiene ninguna estrella
            if(value == 0) {
                VaciarCasilla();
            } else {
                //El sprite correspondiente a cada nivel tiene indice nivel-1 en el array
                CambiarSprite(value - 1);
            }
        }
    }

    private int tipoEstrella;

    public void Awake() {
        imagen = GetComponent<Image>();
        animator = GetComponent<Animator>();
    }

    //Cambia el sprite por aquel cuyo id se reciba como argumento
    public void CambiarSprite (int id) {
        if (imagen.enabled == false) imagen.enabled = true;
        imagen.sprite = sprites[id];
    }

    //Desactiva el componente imagen
    public void VaciarCasilla () {
        imagen.enabled = false;
    }

    public void TriggerAnimacion (string trigger) {
        animator.SetTrigger(trigger);
    }
}
