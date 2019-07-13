/*Script que gestiona la interfaz grafica de la pantalla de Estadisticas, 
 * actualizandola con los datos requeridos.
 * Agregado al gameobject EstadisticasManager.
 */

using Estadisticas_DB;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EstadisticasManager : MonoBehaviour {

    public Text fecha, total;
    public GameObject flechaDerecha, flechaIzquierda;
    private float mEstudio, mEjercicio, mHogar, mOcio, mOtros, mTotal;

    //Propiedades calendario
    CultureInfo CI;
    CalendarWeekRule CWR;
    DayOfWeek DOW;
    Calendar calendario;

    //Dependencias
    IEstadisticasDB estadisticasDB;
    [Inject]
    public void Init(IEstadisticasDB db) {
        estadisticasDB = db;
    }

    public int semana;
    public int semanaActual; //semanaActual es una copia para no perder el dato al modificar semana
    public int primeraSemana; //Guarda la primera semana del año que hay en la bd para evitar retroceder mas alla
    public int ultimaSemana; //Guarda la ultima semana del año que hay en la bd para evitar avanzar mas alla
    public int anno;
    public int annoActual; //annoActual es una copia para no perder el dato al modificar anno   

    void Start () {

        //Calcular fecha y semana actuales
        CI = new CultureInfo("es-ES"); //español
        CultureInfo.CurrentCulture = CI;
        DateTime fechaHoy = DateTime.Today.Date;
        anno = fechaHoy.Year;
        annoActual = anno;
        primeraSemana = ObtenerPrimeraSemana(anno);
        ultimaSemana = ObtenerUltimaSemana(anno);
        //Calendario para calcular semana       
        calendario = CI.Calendar;
        //Propiedades calendario
        CWR = CI.DateTimeFormat.CalendarWeekRule; //Como se determina la primera semana en el calendario español
        DOW = CI.DateTimeFormat.FirstDayOfWeek; //Primer dia de la semana en el calendario español
        semanaActual = semana = calendario.GetWeekOfYear(fechaHoy, CWR, DOW);        

        MostrarEstadisticasSemana(); 
    }

    //Busca la primera semana almacenada del año dado
    private int ObtenerPrimeraSemana (int anno) {
        int semana = -1;
        System.Data.IDataReader reader = estadisticasDB.BuscarPrimeraSemanaAnno(anno);
        while (reader.Read()) {
            if (!reader.IsDBNull(0)) semana = reader.GetInt32(0);
        }
        return semana;
    }

    //Busca la ultima semana del año dado
    private int ObtenerUltimaSemana(int anno) {
        int semana = -1;
        System.Data.IDataReader reader = estadisticasDB.BuscarUltimaSemanaAnno(anno);
        while (reader.Read()) {
            if (!reader.IsDBNull(0)) semana = reader.GetInt32(0);
        }
        return semana;
    }

    //Obtiene el tiempo total dedicado a una actividad concreta a lo largo de la semana
    private float ObtenerTiempoTotal (string actividad, int semana) {
        float tiempo = 0;

        System.Data.IDataReader reader = estadisticasDB.TiempoTotalActividad(actividad, semana, anno); 
        while (reader.Read()) {
            if (!reader.IsDBNull(0)) tiempo = reader.GetFloat(0);
        }

        return tiempo;
    }

    //Buscar la fecha del primer dia almacenado de esta semana
    private string ObtenerPrimerDia (int semana) {
        string fecha = "";
        System.Data.IDataReader reader = estadisticasDB.BuscarPrimerDiaSemana(semana, anno);
        while (reader.Read()) {
            if (!reader.IsDBNull(0)) fecha = reader.GetString(0);
        }
        fecha = fecha.Substring(0, 6) + fecha.Substring(8, 2);
        return fecha;
    }

    //Buscar la fecha del ultimo dia almacenado de esta semana
    private string ObtenerUltimoDia(int semana) {
        string fecha = "";
        System.Data.IDataReader reader = estadisticasDB.BuscarUltimoDiaSemana(semana, anno);
        while (reader.Read()) {
            if (!reader.IsDBNull(0)) fecha = reader.GetString(0);
        }
        fecha = fecha.Substring(0, 6) + fecha.Substring(8, 2);
        return fecha;
    }

    //Metodo para cambiar a la siguiente semana almacenada
    public void AvanzarSemana () {
        System.Data.IDataReader reader;

        //Si se esta en la ultima semana de un año, se pasa a la primera semana almacenada del año siguiente almacenado
        if (semana == ultimaSemana) {
            //Buscar el siguiente año almacenado
            reader = estadisticasDB.BuscarSiguienteAnno(anno);
            while (reader.Read()) {
                if (!reader.IsDBNull(0)) anno = reader.GetInt32(0);
            }
            primeraSemana = ObtenerPrimeraSemana(anno);
            ultimaSemana = ObtenerUltimaSemana(anno);
            semana = primeraSemana;
        } 
        //Si esta en una semana cualquiera, busca la siguiente semana del mismo año almacenada en la bd
        else {
            reader = estadisticasDB.BuscarSiguienteSemanaAnno(semana, anno);
            while (reader.Read()) {
                if (!reader.IsDBNull(0)) semana = reader.GetInt32(0);
            }
        }

        MostrarEstadisticasSemana();
    }

    //Comprueba si hay una entrada anterior a la que se esta viendo
    public bool PuedeRetroceder () {
        bool retroceder = true;
        int a = anno;
        //Si esta en la primera semana del año, buscara la ultima semana del año anterior
        if(semana == primeraSemana) {
            a = anno - 1;         
        }

        //Si esta en otra semana cualquiera, buscara la anterior semana almacenada del mismo año
        int s = ObtenerUltimaSemana(a);

        //Si la semana obtenida es -1, significa que no puede encontrar una entrada anterior y no puede seguir retrocediendo
        if (s == -1) retroceder = false;

        return retroceder;
    }
    
    //Metodo para cambiar a la anterior semana almacenada
    public void RetrocederSemana () {
       
        System.Data.IDataReader reader;

        //Si esta en la primera semana del año, pasa a la ultima semana del año anterior
        if (semana == primeraSemana) {
            //Buscar el anterior año almacenado
            reader = estadisticasDB.BuscarAnteriorAnno(anno);
            while (reader.Read()) {
                if (!reader.IsDBNull(0)) anno = reader.GetInt32(0);
            }
            primeraSemana = ObtenerPrimeraSemana(anno);
            ultimaSemana = ObtenerUltimaSemana(anno);
            semana = ultimaSemana;
        } 
        //Si esta en una semana cualquiera, busca la anterior semana del mismo año almacenada en la bd
        else {
            reader = estadisticasDB.BuscarAnteriorSemanaAnno(semana, anno);
            while (reader.Read()) {
                if (!reader.IsDBNull(0)) semana = reader.GetInt32(0);
            }
        }

        MostrarEstadisticasSemana();
    }

    //Muestra las estadisticas de la semana que se esta viendo (calculada en el start, al avanzar o al retroceder)
    public void MostrarEstadisticasSemana () {

        //Recuperar minutos totales dedicados a cada actividad en la semana actual
        mEstudio = ObtenerTiempoTotal("estudio", semana);
        mEjercicio = ObtenerTiempoTotal("ejercicio", semana);
        mHogar = ObtenerTiempoTotal("hogar", semana);
        mOcio = ObtenerTiempoTotal("ocio", semana);
        mOtros = ObtenerTiempoTotal("otros", semana);
        mTotal = mEstudio + mEjercicio + mHogar + mOcio + mOtros;

        //Actualizar campos de texto
        GameObject.Find("MinutosEstudio").GetComponent<Text>().text = mEstudio + "min";
        GameObject.Find("MinutosEjercicio").GetComponent<Text>().text = mEjercicio + "min";
        GameObject.Find("MinutosHogar").GetComponent<Text>().text = mHogar + "min";
        GameObject.Find("MinutosOcio").GetComponent<Text>().text = mOcio + "min";
        GameObject.Find("MinutosOtros").GetComponent<Text>().text = mOtros + "min";
        total.text = "Total: " + mTotal + "min";

        //Actualizar barras progreso
        RellenarBarraProgreso("Estudio", mEstudio);
        RellenarBarraProgreso("Ejercicio", mEjercicio);
        RellenarBarraProgreso("Hogar", mHogar);
        RellenarBarraProgreso("Ocio", mOcio);
        RellenarBarraProgreso("Otros", mOtros);

        //Actualizar rangos fecha
        fecha.text = ObtenerPrimerDia(semana) + " - ";
        fecha.text += ObtenerUltimoDia(semana);

        //Ocultar / mostrar flechas segun se pueda o no avanzar y retroceder
        //No puede avanzar mas si se esta mostrando la semana equivalente a la actual
        flechaDerecha.SetActive(!(semana == semanaActual && anno == annoActual));
        flechaIzquierda.SetActive(PuedeRetroceder ());
    }

    //Metodo para rellena la barra de progreso de una actividad en proporcion a 
    // el tiempo que se le ha dedicado respecto al tiempo total
    private void RellenarBarraProgreso (string actividad, float minutosActividad) {
        Image fillBarraProgreso;

        fillBarraProgreso = GameObject.Find("Fill" + actividad).GetComponent<Image>();
        float fillAmount = (float)minutosActividad / mTotal;
        fillBarraProgreso.fillAmount = fillAmount;
    }   

    private void OnDestroy() {
        estadisticasDB.close();
    }
}
