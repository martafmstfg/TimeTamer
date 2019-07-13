/* Script para cargar la escena indicada o la pantalla de tutoriales,
 * Agregado al gameobject CambiarEscena.
 */ 

using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    
    //Carga inmediatamente la escena que se pasa como argumento
    public void CargarEscena(string escena)
    {
        SceneManager.LoadScene(escena);
    }    

    //Carga la escena de los tutoriales
    public void CargarTutorial () {
        //Variable persistente para indicar que la escena se ha abierto desde el botón "Tutoriales" de los ajustes
        // para que al abrirla no salte directamente a la escena del temporizador
        if (PlayerPrefs.GetInt("VerTutorial", 0) == 0) PlayerPrefs.SetInt("VerTutorial", 1);
        SceneManager.LoadScene("Intro");
    }

}
