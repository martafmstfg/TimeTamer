/* Script para cambiar el nombre del usuario.
 * Agregado al boton de Aceptar del objeto PanelCambiarNombre,
 * desde donde se llama al metodo SaveName()
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CambiarNombre : MonoBehaviour {

	public void SaveName () {
        string name = GetComponent<InputField>().text;
        if(name.Length>0)
            PlayerPrefs.SetString("Name", name);
    }
}
