/* Script para abrir el link a la encuesta.
 * Agregado en el boton de la encuesta que hay en la pantalla del temporizador
 */
 
using UnityEngine;

public class AbrirEncuesta : MonoBehaviour {

    //https://forms.gle/ZT65Zq6RzHeGyKa6A
    public string linkEncuesta;

	public void AbrirEnlaceEncuesta () {
        Application.OpenURL(linkEncuesta);
    }
}
