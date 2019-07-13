/* Script que se ejecuta al cargar la escena de fin de la cuenta atras.
 * Guarda en la BD el tiempo de la ultima sesion, asignado a la categoria elegida por el usuario.
 * Tambien se encarga de reproducir el efecto de sonido de fin de la cuenta atras, manda comprobar 
 * si se ha cumplido el logro de horas de trabajo acumuladas y actualiza la UI.
 * Agregado al gameobject TempoFinScene.
 */ 

using UnityEngine;
using UnityEngine.SceneManagement;
using Estadisticas_DB;
using System;
using UnityEngine.UI;

public class TempoFinScene : MonoBehaviour {
    
    public AudioSource audioSource;
    public AudioClip finTempo;

    public Text monedas;

    [SerializeField]
    public string categoria; //categoria a la que se va a dedicar el tiempo;
    EstadisticasDB estadisticasDB;
    private string fechaActual;

    // Use this for initialization
    void Start () {

        monedas.text = Tempo_UltimaEscena.GetMonedas().ToString();
        categoria = "estudio";
        estadisticasDB = new EstadisticasDB();
        fechaActual = DateTime.Today.Date.ToShortDateString();
        
        //Reproducir sonido al terminar la cuenta atras
        audioSource.PlayOneShot(finTempo);

        //Antes de cargar esta escena, en el metodo EndCDT de CDAux, se han sumado los ultimos minutos
        // al numero total. Al iniciar esta escena, hay que comprobar si se ha cumplido el logro de tiempo
        //Se comprueba al cargar esta escena y no en la anterior para poder mostrar el pop up
        //Comprobar el logro de horas seguidas
        LogrosManager.ComprobarLogroCompletado(1);        
    }        

    //Carga de nuevo la escena del temporizador pero indicando que se debe repetir el mismo tiempo que antes
    public void RepetirTempo () {
        Tempo_UltimaEscena.SetEscenaAnterior("Tempo_Fin");
        SceneManager.LoadScene("Tempo");
    }

    public void SetCategoria (string cat) {
        categoria = cat;
    }

    //Suma el tiempo de esta ultima sesion a la categoria elegida por el usuario
    private void GuardarTiempoCategoria () {
        float minutos = Tempo_UltimaEscena.GetMinutos(); //tiempo que se ha estado trabajando esta ronda        

        //Recuperar de la bd los minutos almacenados para hoy de esta actividad
        float minutosGuardados = 0;
        System.Data.IDataReader reader = estadisticasDB.BuscarTiempoActividad(categoria, fechaActual);
        while (reader.Read()) {
            minutosGuardados = reader.GetFloat(0);
        }
        //Sumar minutos y guardarlos
        minutos += minutosGuardados;
        estadisticasDB.GuardarTiempoActividad(categoria, minutos, fechaActual);

        estadisticasDB.close();
    }

    //Guardar tipo de actividad y horas para estadistica antes de cerrar la escena
    private void OnDestroy() {
        GuardarTiempoCategoria();
    }
}
