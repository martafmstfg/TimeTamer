/*Script que recoge una serie de metodos para hacer consultas preestablecidas a la bd.
 * 
 * Basado en el script de Rizwan Asif, MIT License - Copyright (c) 2018 Rizwan Asif
 * https://github.com/rizasif/sqlite-unity-plugin-example/blob/master/LICENSE
 */

using System;
using System.Data;

namespace Estadisticas_DB {

	public class EstadisticasDB : SqliteHelper, IEstadisticasDB {		
        
        private const String TABLE_NAME = "Estadisticas";
        private const String KEY_ID = "id";
        private const String KEY_FECHA = "fecha";
        private const String KEY_ANNO = "anno";
        private const String KEY_SEMANA = "semana";
        private const String KEY_ESTUDIO = "estudio";
		private const String KEY_EJERCICIO= "ejercicio";
        private const String KEY_HOGAR = "hogar";
        private const String KEY_OCIO = "ocio";
        private const String KEY_OTROS = "otros";
        private const String KEY_TAREAS = "tareas";
        private String[] COLUMNS = new String[] {KEY_ID, KEY_FECHA, KEY_ANNO, KEY_SEMANA, KEY_ESTUDIO,
            KEY_EJERCICIO, KEY_HOGAR, KEY_OCIO, KEY_OTROS, KEY_TAREAS};

        public EstadisticasDB() : base()
        {            
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                KEY_ID + " INTEGER PRIMARY KEY, " +
                KEY_FECHA + " TEXT UNIQUE, " +
                KEY_SEMANA + " INTEGER, " +
                KEY_ANNO + " INTEGER, " +
                KEY_ESTUDIO + " REAL, " +
                KEY_EJERCICIO + " REAL, " +
                KEY_HOGAR + " REAL, " +
                KEY_OCIO + " REAL, " +
                KEY_OTROS + " REAL, " +
                KEY_TAREAS + " INTEGER )";
            dbcmd.ExecuteNonQuery();
        }         

        //Añadir nueva entrada vacia (solo con fecha y semana actuales) al abrir la app
        //SQLite calcula el id automaticamente (rowid)
        public void NuevaEntradaDiaria (string fecha, int semana, int anno) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_FECHA + ", "                
                + KEY_SEMANA + ", "
                + KEY_ANNO + ", "
                + KEY_ESTUDIO + ", "
                + KEY_EJERCICIO + ", "
                + KEY_HOGAR + ", "
                + KEY_OCIO + ", "
                + KEY_OTROS + ", "
                + KEY_TAREAS + ") "

                + "VALUES ( '"
                + fecha + "', '"                
                + semana + "', '"
                + anno + "', '"
                + 0.0f + "', '" + 0.0f + "', '" + 0.0f + "', '"
                + 0.0f + "', '" + 0.0f+ "', '" + 0 +"' )";
            dbcmd.ExecuteNonQuery();
        }

        //Añadir nueva entrada a partir de objeto EntradaDiaria
        //SQLite calcula el id automaticamente (rowid)
        public void NuevaEntradaDiaria(EntradaDiaria entrada) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_FECHA + ", "
                + KEY_SEMANA + ", "
                + KEY_ANNO + " ) "

                + "VALUES ( '"
                + entrada.fecha + "', '"
                + entrada.semana + "', '"
                + entrada.anno + "' )";
            dbcmd.ExecuteNonQuery();
        }

        //Recuperar entradas por ID
        public override IDataReader BuscarEntradasPorId(int id)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = " + id;
            return dbcmd.ExecuteReader();
        }

        //Recuperar entradas por fecha
        public override IDataReader BuscarEntradasPorFecha(string fecha) {            

            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_FECHA + " = '" + fecha + "'";
            return dbcmd.ExecuteReader();
        }

        //Recuperar entradas por semana y año
        public IDataReader BuscarEntradasPorSemana(int semana, int anno) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_SEMANA + " = " + semana + " AND " + KEY_ANNO + " = " + anno;
            return dbcmd.ExecuteReader();
        }

        public override void BorrarEntradaPorFecha(string fecha)
        {            
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "DELETE FROM " + TABLE_NAME + " WHERE " + KEY_FECHA + " = '" + fecha + "'";
            dbcmd.ExecuteNonQuery();
        }


		public override void BorrarEntradaPorId(int id)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "DELETE FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = " + id;
            dbcmd.ExecuteNonQuery();
        }

        //Guarda los minutos totales dedicados a una actividad concreta en la fecha dada
        public void GuardarTiempoActividad (string actividad, float minutos, string fecha) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "UPDATE " + TABLE_NAME
                + " SET " 
                + actividad + " = " + minutos
                + " WHERE " + KEY_FECHA + "= '" + fecha + "'";                
            dbcmd.ExecuteNonQuery();
        }

        //Devuelve los minutos totales dedicados a una actividad concreta en la fecha dada
        public IDataReader BuscarTiempoActividad(string actividad, string fecha) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + actividad + " FROM " + TABLE_NAME + " WHERE " + KEY_FECHA + " = '" + fecha + "'";
            return dbcmd.ExecuteReader();
        }

        //Suma el tiempo total dedicado a la actividad indicada a lo largo de toda la semana
        public IDataReader TiempoTotalActividad (string actividad, int semana, int anno) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT SUM (" + actividad + ") FROM " + TABLE_NAME + " WHERE " + KEY_SEMANA + " = " + semana + " AND " + KEY_ANNO + " = " + anno;
            return dbcmd.ExecuteReader();
        }

        //Devuelve tiempo de uso de la aplicacion en el dia actual (suma los tiempos de todas las actividades)
        public IDataReader TiempoTotalActividadHoy(string fecha) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT SUM (" + KEY_ESTUDIO + " + " + KEY_EJERCICIO + " + " + KEY_HOGAR + " + " +KEY_OCIO + " + " + KEY_OTROS + ") FROM " 
                + TABLE_NAME + " WHERE " + KEY_FECHA + " = '" + fecha+"'";
            return dbcmd.ExecuteReader();
        }

        //Devuelve el ultimo dia guardado de la semana del año dados
        public IDataReader BuscarUltimoDiaSemana (int semana, int anno) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_FECHA + " FROM " + TABLE_NAME + " WHERE " + KEY_SEMANA + " = " + semana + " AND " + KEY_ANNO + " = " 
                + anno + " ORDER BY " + KEY_FECHA + " DESC LIMIT 1";
            return dbcmd.ExecuteReader();
        }

        //Devuelve el primer dia guardado de la semana del año dados
        public IDataReader BuscarPrimerDiaSemana(int semana, int anno) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_FECHA + " FROM " + TABLE_NAME + " WHERE " + KEY_SEMANA + " = " + semana + " AND " + KEY_ANNO + " = " 
                + anno + " ORDER BY " + KEY_FECHA + " ASC LIMIT 1";
            return dbcmd.ExecuteReader();
        }

        //Devuelve la primera semana almacenada del año dado
        public IDataReader BuscarPrimeraSemanaAnno(int anno) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_SEMANA + " FROM " + TABLE_NAME + " WHERE " + KEY_ANNO + " = " + anno + " ORDER BY " + KEY_SEMANA + " ASC LIMIT 1";
            return dbcmd.ExecuteReader();
        }

        //Devuelve la ultima semana almacenada del año dado
        public IDataReader BuscarUltimaSemanaAnno(int anno) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_SEMANA + " FROM " + TABLE_NAME + " WHERE " + KEY_ANNO + " = " + anno + " ORDER BY " + KEY_SEMANA + " DESC LIMIT 1";
            return dbcmd.ExecuteReader();
        }

        //Devuelve la siguiente semana almacenada del año dado
        public IDataReader BuscarSiguienteSemanaAnno(int semana, int anno) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_SEMANA + " FROM " + TABLE_NAME + " WHERE " + KEY_SEMANA + " > " + semana + " AND " 
                    + KEY_ANNO + " = " + anno + " ORDER BY " + KEY_SEMANA + " ASC LIMIT 1";
            return dbcmd.ExecuteReader();
        }

        //Devuelve la siguiente semana almacenada del año dado
        public IDataReader BuscarAnteriorSemanaAnno(int semana, int anno) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_SEMANA + " FROM " + TABLE_NAME + " WHERE " + KEY_SEMANA + " < " + semana + " AND "
                    + KEY_ANNO + " = " + anno + " ORDER BY " + KEY_SEMANA + " DESC LIMIT 1";
            return dbcmd.ExecuteReader();
        }

        //Devuelve el siguiente año almacenado
        public IDataReader BuscarSiguienteAnno(int anno) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_ANNO + " FROM " + TABLE_NAME + " WHERE " + KEY_ANNO + " > " + anno + " ORDER BY " + KEY_ID + " ASC LIMIT 1";
            return dbcmd.ExecuteReader();
        }

        //Devuelve el anterior año almacenado
        public IDataReader BuscarAnteriorAnno(int anno) {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_ANNO + " FROM " + TABLE_NAME + " WHERE " + KEY_ANNO + " < " + anno + " ORDER BY " + KEY_ID + " DESC LIMIT 1";
            return dbcmd.ExecuteReader();
        }

        //Devuelve el numero de tareas dada una fecha
        public IDataReader BuscarTareasPorFecha(string fecha) {

            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_TAREAS +" FROM " + TABLE_NAME + " WHERE " + KEY_FECHA + " = '" + fecha + "'";
            return dbcmd.ExecuteReader();
        }

        //Guardar tareas completadas
        public void GuardarTareasPorFecha(string fecha, int tareas) {

            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "UPDATE " + TABLE_NAME
                + " SET "
                + KEY_TAREAS + " = " + tareas
                + " WHERE " + KEY_FECHA + "= '" + fecha + "'";
            dbcmd.ExecuteNonQuery();
        }        

        public override void EliminarTabla()
        {
            base.deleteAllData(TABLE_NAME);
        }

        public override IDataReader getAllData()
        {
            return base.getAllData(TABLE_NAME);
        }

	}
}
