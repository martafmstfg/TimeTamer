/* Gestiona la lista de tareas que se muestran en pantalla:
 *  -Añadir tantos renglones como tareas haya en su lista.
 *  -Borrar todos los renglones.
 *  -Refrescar la lista.
 *  
 *  Este script es un componente del game object Content (panel > scrollview > viewport > content).
 */
 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TaskList : MonoBehaviour {
       
    public List<Task> tasks; //Lista propia de las tareas existentes
    public GameObject prefabRenglon; //Prefab del renglon para instanciar uno por cada tarea de la lista    

    public SimpleObjectPool renglonPool; //Los renglones se crean mediante un metodo de pooling generico
        

    void Start () {        
        AddAllItems(); //Mostrar la lista de tareas al iniciar la escena
	}
	
    //Añadir tantos renglones como tareas haya en la lista al contenido del scrollview
	public void AddAllItems () {
        tasks = TaskManager.instance.tasks; //Referencia a la lista del TaskManager 

        //Crear un renglon por cada tarea y asignarle la informacion correspondiente
        for (int i=0; i < tasks.Count; i++) {
            Task task = tasks[i]; //obtener tarea de la lista
            CreateItem(task, i);
        }
    }

    //Eliminar todos los renglones mediante el sistema de pooling
    public void EmptyList () {
        while (transform.childCount > 0) {
            GameObject toRemove = transform.GetChild(0).gameObject;
            renglonPool.ReturnObject(toRemove);
        }
    }

    //Refrescar la lista de tareas en pantalla
    public void ReloadList () {
        EmptyList();
        AddAllItems();
    }

    //Agregar un solo renglon, el de la ultima tarea que haya en la lista
    public void AddLastItem () {
        tasks = TaskManager.instance.tasks; //Referencia a la lista del TaskManager
        Task task = tasks.Last(); //obtener ultima tarea de la lista
        CreateItem(task);
    }

    //Crea un nuevo renglon a partir de una tarea y un id dados
    public void CreateItem (Task task, int id) {
        GameObject newRenglon = renglonPool.GetObject(); //Obtener renglon a partir de la pool de renglones
        newRenglon.transform.SetParent(transform, false); //Hacerlo hijo del content
        newRenglon.transform.GetChild(0).GetComponent<Text>().text = task.title; //Mostrar el titulo
        newRenglon.GetComponent<TaskButton>().task = task; //Asignarle la tarea correspondiente a su componente TaskButton
        newRenglon.GetComponent<TaskButton>().id = id; //Asignarle el id de la tarea correspondiente
    }

    //Crea un nuevo renglon a partir de una tarea dada. Como no recibe id, asume que se trata de la ultima tarea annadida.
    public void CreateItem (Task task) {
        GameObject newRenglon = renglonPool.GetObject(); //Obtener renglon a partir de la pool de renglones
        newRenglon.transform.SetParent(transform, false); //Hacerlo hijo del content
        newRenglon.transform.GetChild(0).GetComponent<Text>().text = task.title; //Mostrar el titulo
        newRenglon.GetComponent<TaskButton>().task = task; //Asignarle la tarea correspondiente a su componente TaskButton
        newRenglon.GetComponent<TaskButton>().id = tasks.Count - 1; //Asignarle el id de la tarea correspondiente
        // el id nunca va a ser -1 porque este metodo se llama tras annadir una tarea a la lista
    }
}
