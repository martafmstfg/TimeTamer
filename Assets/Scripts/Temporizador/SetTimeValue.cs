/* Script para obtener y modificar el valor del slider que controla el tiempo.
 * Gestiona tambien el numero de moneds obtenidas en funcion del tiempo.
 * Agregado al gameobject Slider.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTimeValue : MonoBehaviour {

    public Text textoTiempo;
    public Text textCoins;
    private Slider slider;
    private int monedas;

    public void Awake() {
        slider = GetComponent<Slider>();
        monedas = 5;
    }    
	
    //Muestra el valor de tiempo seleccionado con el slider
    public void SetTextoTiempo (float value)
    {
        textoTiempo.text = value.ToString()+":00";        
    }

    //Cambiar el numero de monedas obtenidas segun el tiempo
    public void SetCoins (float value) {
        monedas = (int)(value / 5);
        textCoins.text = "+"+monedas;
    }

    //Devolver el numero de monedas obtenidas segun el tiempo
    public int GetMonedas () {
        return monedas;
    }

    //Devuelve el valor del slider en un momento determinado
    public float GetValue () {
        return slider.value;
    }

    //Pone un valor en el slider
    public void SetValue(float value) {
        slider.value = value;
    }
}
