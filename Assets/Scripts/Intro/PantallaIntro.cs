/*Script que gestiona los cambios de escena y la visualizacion de los tutoriales.
 * Agregado al gameobject PantallaIntro.
 */ 

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PantallaIntro : MonoBehaviour {

    public int distanciaMover;
    public GameObject cambioNombre;

    private void Awake() {

        //Si NO es la primera vez que se ejecuta la app y si NO se ha abierto el tutorial desde ajustes, 
        // salta directamente a la pantalla inicial (el temporizador). Esto evita que aparezca el tutorial
        // cada vez que se abre la aplicacion.
        if (PlayerPrefs.GetInt("PrimeraEjecucion", 1) == 0  && PlayerPrefs.GetInt("VerTutorial", 0) == 0) {
            SceneManager.LoadScene("Tempo");
        }        
    }
    
    //Mueve hacia la izquierda los paneles de texto de los tutoriales para avanzar por ellos
    public void Mover (RectTransform rect) {
        rect.anchoredPosition = new Vector3 (rect.anchoredPosition.x - distanciaMover, rect.anchoredPosition.y);
    }   
    
    //Devuelve a su posicion original a los tutoriales que tienen 4 paneles de texto
    public void ReiniciarPosicion4 (RectTransform rect) {
        rect.anchoredPosition = new Vector3(1685, rect.anchoredPosition.y);
    }

    //Devuelve a su posicion original a los tutoriales que tienen 2 paneles de texto
    public void ReiniciarPosicion2(RectTransform rect) {
        rect.anchoredPosition = new Vector3(629, rect.anchoredPosition.y);
    }

    //Determina que hacer al salir de la pantalla de tutoriales: cargar la pantalla de inicio
    // (el temporizador) si no es la primera vez que se ejecuta la app, o mostrar la pantalla en 
    // la que el usuario introduce su nombre la primera vez que abre la app
    public void SiguienteAccion () {
        //Si NO es la primera vez que se ejecuta la app, cambiar directamente de escena
        if (PlayerPrefs.GetInt("PrimeraEjecucion", 1) == 0) {
            SceneManager.LoadScene("Tempo");
        }
        //Si es la primera vez, muestra el panel de cambio de nombre
        else {
            cambioNombre.SetActive(true);            
        }
    }

    //Obtiene el nombre del objeto input y lo guarda en una variable persistente
    public void GuardarNombre (InputField nombreInput) {
        string name = nombreInput.text;
        if (name.Length > 0) PlayerPrefs.SetString("Name", name);
        SceneManager.LoadScene("Avatar");
    }

    public void OnDestroy() {
        //Reinicia la variable que indica si se ha accedido a la pantalla de tutoriales desde los ajustes
        PlayerPrefs.SetInt("VerTutorial", 0);
    }
}
