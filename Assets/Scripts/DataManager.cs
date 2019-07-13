/* Script para mostrar datos en la interfaz.
 * Agregado en el gameobject DataManager, comun a varias escenas
 */ 

using Estadisticas_DB;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour {

    //Objetos de texto en los que se va a mostrar la informacion
    public Text nombre;
    public Text monedas;
    public Text tareas;
    public Text tiempoTotal;    

    public static DataManager instance {
        private set;
        get;
    } //Singleton

    public void Awake() {
        
        //Comprobar si ya hay algun otro objeto con el script DataManager
        DataManager[] instancias = FindObjectsOfType<DataManager>();
        if (instancias.Length > 1) {
            Destroy(gameObject);
        }
        else instance = this;
    }

    void Start () {
        //Mostrar la informacion en los campos que existan en la escena
        if (nombre != null) nombre.text =PlayerPrefs.GetString("Name", "Nombre");
        if (monedas != null) ActualizarMonedas();
        if (tareas != null) ActualizarTareas();
        if (tiempoTotal != null) ActualizarTiempoTotal();
    }

    //Mostrar el numero de monedas (0 por defecto)
    public void ActualizarMonedas () {
        monedas.text = (PlayerPrefs.GetInt("Monedas", 0)).ToString();
    } 

    //Mostrar el numero de tareas completadas en el dia
    private void ActualizarTareas () {
        //Recuperar de la bd tareas alamcenadas hoy
        EstadisticasDB estadisticasDB = new EstadisticasDB(); //Abrir conexion a la bd
        string fechaActual = DateTime.Today.Date.ToShortDateString();
        int tareasGuardadas = 0;
        System.Data.IDataReader reader = estadisticasDB.BuscarTareasPorFecha(fechaActual); //Realizar consulta a la bd
        while (reader.Read()) {
            tareasGuardadas = reader.GetInt32(0);
        }
        tareas.text = tareasGuardadas.ToString();

        estadisticasDB.close(); //Cerrar la conexion a la bd
    }

    //Mostrar el tiempo que se ha utilizado el temporizador en el dia
    private void ActualizarTiempoTotal() {
        //Recuperar de la bd el tiempo de trabajo almacenado hoy
        EstadisticasDB estadisticasDB = new EstadisticasDB(); //Abrir conexion a la bd
        string fechaActual = DateTime.Today.Date.ToShortDateString();
        float tiempoHoy = 0.0f;
        System.Data.IDataReader reader = estadisticasDB.TiempoTotalActividadHoy(fechaActual); //Realizar consulta a la bd
        while (reader.Read()) {
            if (!reader.IsDBNull(0)) tiempoHoy = reader.GetFloat(0);
        }

        int horas = (int) (tiempoHoy / 60.0f);
        int minutos = (int) (tiempoHoy % 60.0f);
        tiempoTotal.text = horas.ToString("D2") + ":" + minutos.ToString("D2");

        estadisticasDB.close(); //Cerrar la conexion a la bd
    }
}
