/* Scipt que realiza las comprobaciones diarias al abrir la app por primera vez.
 * No esta agregado a ningun gameobject, se inicaliza automaticamente con la primera escena.
 */

using System;
using UnityEngine;
using System.Globalization;
using Estadisticas_DB;

public class ComprobacionesDiarias : MonoBehaviour {
        
    //Metodo que se ejecuta automaticamente al abrir la primera escena
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeAfterSceneLoad() {        

        //Comprobar si ha pasado un solo dia desde que se abrio la aplicacion
        ComprobarDiasSeguidos(DateTime.Now);

        //Crear una nueva entrada en la bd correspondiente al dia actual (si no se ha creado ya)
        NuevaEntradaDB();
    }
    
    //Metodo para comprobar ha pasado un solo dia desde la ultima vez que se abrio la app,
    // para el logro de dias seguidos
    public static void ComprobarDiasSeguidos(DateTime hoy) {

        //Informacion para obtener la fecha en el formato de España
        CultureInfo CI = new CultureInfo("es-ES"); //Español
        CultureInfo.CurrentCulture = CI;

        //Recuperar ultimo dia guardado (por defecto, si no hay valor guardado, es la fecha actual)
        DateTime ultimoDia = Convert.ToDateTime(PlayerPrefs.GetString("UltimoDia", hoy.ToString()));
        TimeSpan diferencia = hoy.Subtract(ultimoDia); //Calcular la diferencia entre el ultimo dia guardado y el actual

        //Debug.Log("Hoy: " + hoy + ", ultimo: " + ultimoDia + ", diferencia: " + diferencia.Days);

        //Si hay 1 dia entero de diferencia, o si hay menos de 24h de diferencia pero los dias son distintos 
        // es que se ha abierto la app en dias seguidos        
        if (diferencia.Days == 1 || (diferencia.Days < 1 && hoy.Day != ultimoDia.Day)) {
            PlayerPrefs.SetFloat("DiasSeguidos", PlayerPrefs.GetFloat("DiasSeguidos", 1) + 1);

            //Comprobar si se ha cumplido el logro de dias seguidos (id = 0)
            LogrosManager.ComprobarLogroCompletado(0);
        }
        //Si han pasado mas de 24h / 1 dia, se pierde la racha de dias seguidos
        else if (diferencia.Days > 1) {
            //Reiniciar numero de dias seguidos
            PlayerPrefs.SetFloat("DiasSeguidos", 1);
        }

        //Guardar la fecha actual
        PlayerPrefs.SetString("UltimoDia", hoy.ToString());
    }

    //Crea una nueva entrada en la base de datos para el dia actual, si no existe ya
    private static void NuevaEntradaDB () {

        EstadisticasDB estadisticasDB = new EstadisticasDB(); //Abrir conexion a la bd

        //Informacion para obtener la fecha en el formato de España
        CultureInfo CI = new CultureInfo("es-ES"); //Español
        CultureInfo.CurrentCulture = CI;

        //Comprobar si ya existe una entrada para el dia actual
        DateTime fechaHoy = DateTime.Today.Date;
        string fecha = fechaHoy.ToShortDateString();
        System.Data.IDataReader reader = estadisticasDB.BuscarEntradasPorFecha(fecha); //Consulta a la bd        
        //Si no se ha encontrado una entrada para el dia actual
        if(!reader.Read()) {
            
            //Calendario para calcular semana            
            Calendar calendario = CI.Calendar;
            //Propiedades calendario
            CalendarWeekRule CWR = CI.DateTimeFormat.CalendarWeekRule; //Como se determina la primera semana en el calendario español
            DayOfWeek DOW = CI.DateTimeFormat.FirstDayOfWeek; //Primer dia de la semana en el calendario español
            int anno = fechaHoy.Year;
            //Crear la entrada
            estadisticasDB.NuevaEntradaDiaria(fecha, calendario.GetWeekOfYear(fechaHoy, CWR, DOW), anno);
        }

        estadisticasDB.close(); //Cerrar la conexion a la bd
    }
}
