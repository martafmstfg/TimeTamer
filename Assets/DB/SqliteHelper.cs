/* MIT License - Copyright (c) 2018 Rizwan Asif
 * https://github.com/rizasif/sqlite-unity-plugin-example/blob/master/LICENSE
 */
 
using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;

namespace Estadisticas_DB
{
    public class SqliteHelper
    {       
        private const string database_name = "tfg_db";

        public string db_connection_string;
        public IDbConnection db_connection;

        public SqliteHelper()
        {
            db_connection_string = "URI=file:" + Application.persistentDataPath + "/" + database_name;
            Debug.Log("db_connection_string" + db_connection_string);
            db_connection = new SqliteConnection(db_connection_string);
            db_connection.Open();
        }

        ~SqliteHelper()
        {
            db_connection.Close();
        }

        //vitual functions
        public virtual IDataReader BuscarEntradasPorId(int id)
        {
            Debug.Log("This function is not implemented");
            throw null;
        }        

        public virtual IDataReader BuscarEntradasPorFecha(string str)
        {
            Debug.Log("This function is not implemented");
            throw null;
        }

        public virtual void BorrarEntradaPorId(int id)
        {
            Debug.Log("This function is not implemented");
            throw null;
        }

		public virtual void BorrarEntradaPorFecha(string id)
        {
            Debug.Log("This function is not implemented");
            throw null;
        }

        public virtual IDataReader getAllData()
        {
            Debug.Log("This function is not implemented");
            throw null;
        }

        public virtual void EliminarTabla()
        {
            Debug.Log("This function is not implemented");
            throw null;
        }

        public virtual IDataReader getNumOfRows()
        {
            Debug.Log("This function is not implemented");
            throw null;
        }

        //helper functions
        public IDbCommand getDbCommand()
        {
            return db_connection.CreateCommand();
        }

        public IDataReader getAllData(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + table_name;
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        }

        public void deleteAllData(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText = "DROP TABLE IF EXISTS " + table_name;
            dbcmd.ExecuteNonQuery();
        }

        public IDataReader getNumOfRows(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText =
                "SELECT COALESCE(MAX(id)+1, 0) FROM " + table_name;
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        }

		public void close (){
            Debug.Log("close");
			db_connection.Close ();
		}
    }
}