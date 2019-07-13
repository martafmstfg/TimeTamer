/* Script que gestiona los elementos del panel de informacion de un juego
 * y que gestiona su compra (si esta aun bloqueado).
 * Agregado en los gameobjects PanelVerJuego de cada juego.
 */

using UnityEngine;
using UnityEngine.UI;

public class PanelJuego : MonoBehaviour {

    public string nombreJuego;
    public int precioJuego;

    public AudioSource audioSource;
    public AudioClip comprarClip;
       

    private void OnEnable() {
        //Si el juego esta aun bloqueado, desactiva el boton de jugar y activa el precio
        int desbloqueado = PlayerPrefs.GetInt(nombreJuego + "Desbloqueado", 0);
        if (desbloqueado == 0) {
            transform.Find("Btn_Jugar").gameObject.SetActive(false);
            transform.Find("Precio").gameObject.SetActive(true);

            //Si no se tienen monedas suficientes, impedir que se interactue con el boton desbloquear
            if (precioJuego > PlayerPrefs.GetInt("Monedas")) {
                transform.Find("Btn_Desbloquear").gameObject.GetComponent<Button>().interactable = false;
            }
            else {
                //Si se tienen monedas suficientes para comprarlo, se añade el listener con el metodo para desbloquearlo
                transform.Find("Btn_Desbloquear").gameObject.GetComponent<Button>().onClick.AddListener(() => DesbloquearJuego());
            }
        }
    }

    //Metodo para comprar un juego
    private void DesbloquearJuego () {

        audioSource.PlayOneShot(comprarClip);

        //Restar monedas
        int monedasActuales = PlayerPrefs.GetInt("Monedas");
        PlayerPrefs.SetInt("Monedas", monedasActuales - precioJuego);

        //Guardar juego como desbloqueado
        PlayerPrefs.SetInt(nombreJuego + "Desbloqueado", 1);

        //Desactivar boton desbloquear y precio
        transform.Find("Btn_Desbloquear").gameObject.SetActive(false);
        transform.Find("Precio").gameObject.SetActive(false);
        //Activar boton jugar, desactivado anteriormente
        transform.Find("Btn_Jugar").gameObject.SetActive(true);

        //LOGRO
        //Incrementar el numero de juegos desbloqueados
        float nJuegos = PlayerPrefs.GetFloat("JuegosConseguidos");
        PlayerPrefs.SetFloat("JuegosConseguidos", nJuegos + 1);
        //Comprobar si se ha completado el logro de objetos (id=2)
        LogrosManager.ComprobarLogroCompletado(3);
    }

    void OnDisable() {
        //Eliminar los listeners para evitar problemas de listeners duplicados
        transform.Find("Btn_Desbloquear").gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
    }
}
