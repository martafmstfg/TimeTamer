/*Interfaz para la clase de la base de datos.
 * Necesaria para inyectarla como dependencia y para sustituirla por mocks en los tests.
 */ 

using UnityEngine;
using System.Collections;
using System.Data;

namespace Estadisticas_DB {

    public interface IEstadisticasDB {
        
        void NuevaEntradaDiaria(string fecha, int semana, int anno);
        void NuevaEntradaDiaria(EntradaDiaria entrada);
        IDataReader BuscarEntradasPorSemana(int semana, int anno);
        void GuardarTiempoActividad(string actividad, float minutos, string fecha);
        IDataReader BuscarTiempoActividad(string actividad, string fecha);
        IDataReader TiempoTotalActividad(string actividad, int semana, int anno);
        IDataReader TiempoTotalActividadHoy(string fecha);
        IDataReader BuscarUltimoDiaSemana(int semana, int anno);
        IDataReader BuscarPrimerDiaSemana(int semana, int anno);
        IDataReader BuscarPrimeraSemanaAnno(int anno);
        IDataReader BuscarUltimaSemanaAnno(int anno);
        IDataReader BuscarSiguienteSemanaAnno(int semana, int anno);
        IDataReader BuscarAnteriorSemanaAnno(int semana, int anno);
        IDataReader BuscarSiguienteAnno(int anno);
        IDataReader BuscarAnteriorAnno(int anno);
        IDataReader BuscarTareasPorFecha(string fecha);
        void GuardarTareasPorFecha(string fecha, int tareas);
        void close();
    }
}
