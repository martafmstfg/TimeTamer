/*Script para cargar una animacion aleatoria cuando se toca al personaje.
 * Agregado al gameobject Personaje,
 */ 
 
using UnityEngine;

public class TocarAvatar : MonoBehaviour {

    private Animator animator;

    private void Start() {
        animator = GameObject.Find("PersonajeModelo").GetComponent<Animator>();
    }

    private void OnMouseDown() {
        AccionTocar();
    }

    private void AccionTocar () {
        //Generar proximo estado aleatoriamente
        int nextIdleState = (int)Random.Range(1.0f, 4.0f);
        //Activar el trigger del estado generado para que se reproduzca la animación
        animator.SetTrigger("Touch" + nextIdleState);
    }
	
}
