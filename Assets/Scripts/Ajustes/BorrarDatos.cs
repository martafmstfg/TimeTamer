/* Script para eliminar todos los datos del juego.
 * Agregado en el objeto Datos de la UI
 */ 

using Estadisticas_DB;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BorrarDatos : MonoBehaviour {

    public void BorrarTodosDatos () {

        //Eliminar todas las playerprefs (nombre, monedas, logros...)
        PlayerPrefs.DeleteAll();

        //Eliminar base de datos
        EstadisticasDB estadisticasDB = new EstadisticasDB();
        estadisticasDB.EliminarTabla();

        //Eliminar tareas
        string filePath = Path.Combine(Application.persistentDataPath, "tasks.json");
        try {
            File.Delete(filePath);
        } catch (Exception ex) {
            Debug.LogException(ex);
        }

        //Los JSON de los items se sobreescriben con los originales si PlayerPrefs primeraEjecucion = 1 --> No hace falta borrarlos
        //El binario del avatar tambien se sobreescribe al borrar los datos

        //Cargar de nuevo la introduccion / tutorial
        SceneManager.LoadScene("Intro");
    }
}
