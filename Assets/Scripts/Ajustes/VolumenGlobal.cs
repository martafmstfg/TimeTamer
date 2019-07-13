/* Script para cambiar el volumen de los efectos de sonido o de la musica.
 * Agregado al slider que controla cada uno.
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumenGlobal : MonoBehaviour {

    public AudioMixer mixer; //Mixer utilizado
    public string parametroVolumen; //Nombre del parametro volumen expuesto para cada grupo

    void Start() {
        //Al abrir la escena se cambia el valor del slider al del volumen guardado
        transform.GetComponent<Slider>().value = PlayerPrefs.GetFloat(parametroVolumen, 0.75f);
    }

    //Recibe el valor del slider y cambia el parametro volumen expuesto para el grupo correspondiente
    public void VolumenSonido(float value) {
        mixer.SetFloat(parametroVolumen, Mathf.Log10(value) * 20);
        float valueB;
        mixer.GetFloat(parametroVolumen, out valueB);
        PlayerPrefs.SetFloat(parametroVolumen, value);
    }    
}
