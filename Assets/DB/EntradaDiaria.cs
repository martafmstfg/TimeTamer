/* Objeto que representa una entrada en la base de datos*/

namespace Estadisticas_DB {

	public class EntradaDiaria {

        public int id;
        public string fecha; //fecha correspondiente al dia en el que se crea la entrada
        public int semana; //semana del año a la que corresponde el dia en el que se crea la entrada
        public int anno; 
        public float estudio, ejercicio, hogar, ocio, otros; //tiempo en segundos dedicado a cada actividad en un dia
        
        //Cuando se crea una entrada al abrir la aplicacion, solo necesita la fecha y semana actual
        //El resto de columnas se rellenaran posteriormente
        //El id de la entrada se autoincrementa en la base de datos
        public EntradaDiaria(string f, int s, int a)
        {            
            fecha = f;
            semana = s;
            anno = a;
        }

        public EntradaDiaria(int i, string f, int s, int a) {
            id = i;
            fecha = f;
            semana = s;
            anno = a;
        }

    }
}
