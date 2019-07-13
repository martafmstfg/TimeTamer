/* Script auxiliar que se asigna a los renglones de la lista de tareas para
 * indicar la tarea a la que corresponden y el id de la misma 
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskButton : MonoBehaviour {

    public Task task; //tarea a la que corresponde el boton
    public int id;
	
    //Al pulsar un renglon, se llama al metodo de TaskManager que muestra la ventana modal 
    // con la informacion de la tarea (titulo y descripcion) que este objeto le envia
    public void ShowTaskInfo () {
        TaskManager.instance.ShowTaskInfo(task.title, task.description, id);
    }
}
