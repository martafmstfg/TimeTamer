/* Gestiona las operaciones relacionadas con las tareas: 
 *  -Cargar el archivo JSON con las tareas guardadas.
 *  -Sobreescribir el archivo JSON con las nuevas tareas o las modificadas.
 *  -Mostrar la informacion de una tarea (titulo y descripcion).
 *  -Eliminar una tarea.
 *  -Añadir una tarea.
 *  -Editar la informacion de una tarea.
 *  -Marcar tarea como completada.
 *  
 *  Este script es un componente del game object TaskManager.
 */

using Estadisticas_DB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TaskManager : MonoBehaviour {

    public List<Task> tasks; //Lista de tareas
    public string taskDataFileName = "tasks.json"; //Fichero json con las tareas

    public static TaskManager instance { set; get; } //Singleton

    public GameObject panelInfo; //GUI mostrar informacion de una tarea
    public GameObject descripcionGO;
    //GOs para crear una nueva tarea
    public GameObject panelNuevaTarea;
    public InputField inputTituloNueva;
    public InputField inputDescripcionNueva;
    //GOs editar una tarea existente
    public GameObject panelEditarTarea;
    public InputField inputTituloEditar;
    public InputField inputDescripcionEditar;

    public int idOpenTask { set; get; } //id en la lista tasks de la ultima tarea cuya informacion se ha mostrado
                                                //Este id se guarda por si el usuario realiza a continuacion alguna accion sobre esta tarea (eliminarla, completarla o editarla)

    private TaskList taskList;

    [Inject]
    public void Init(TaskList tl) {
        taskList = tl;
    }

    public void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
        
        LoadTasksData(); //Al abrirse, carga los datos del fichero JSON de tareas
    }

    //Obtener tareas almacenadas a partir del fichero JSON
    public void LoadTasksData () {       
        string filePath = Path.Combine(Application.persistentDataPath, taskDataFileName);
        string dataAsJson;        

        if (File.Exists(filePath)) {
            dataAsJson = File.ReadAllText(filePath);
            //Crear objeto TaskData a partir de la informacion del json
            TaskData loadedData = JsonUtility.FromJson<TaskData>(dataAsJson);
            //Recuperar lista de tasks
            tasks = loadedData.tasks;
        }
        else {
            throw new FileNotFoundException("El fichero no existe");
        }       
    }

    //Sobreescribir el fichero JSON con la nueva informacion (tareas nuevas o editadas)
    private void SaveTaskData () {
        string filePath = Path.Combine(Application.persistentDataPath, taskDataFileName);

        //Crear objeto TaskData y guardar en el la lista de tareas actuales
        TaskData taskDataAux = new TaskData {
            tasks = tasks
        };
        //Convertir objeto TaskData (y, con el, la lista de tareas) a JSON
        string dataAsJson = JsonUtility.ToJson(taskDataAux);
        //Sobreescribir el archivo json con el nuevo texto (si no existe el fichero, lo crea)
        File.WriteAllText(filePath, dataAsJson);        
    }

    //Abrir la ventana modal que muestra la informacion (titulo y descripcion) recibida como argumento
    public void ShowTaskInfo (string title, string description, int id) {
        panelInfo.transform.Find("PanelVerTarea").transform.Find("Title").GetComponent<Text>().text = title;
        descripcionGO.GetComponent<Text>().text = description;
        idOpenTask = id; //Guardar el id de la tarea que se esta mostrando
        panelInfo.SetActive(true); //Activa la ventana modal con la informacion
    }

    //Eliminar la tarea que se acaba de abrir
    public void DeleteTask () {
        //Eliminar tarea de la lista de tareas
        tasks.RemoveAt(idOpenTask);   
        //Ocultar la ventana modal que muestra la informacion de la tarea
        panelInfo.SetActive(false);
        //Refrescar la lista de tareas en pantalla
        taskList.ReloadList();

        //Guardar los cambios en el JSON
        SaveTaskData();
    }

    //Crear una nueva tarea
    public void NewTask () {
        //Obtener el texto de los inputs
        string titulo = inputTituloNueva.text;
        string descripcion = inputDescripcionNueva.text;

        //Comprobar que los inputs no esten vacios
        if (titulo.Length != 0 /*&& descripcion.Length!= 0*/) {
            //Crear nueva tarea y asignarle el titulo y la descripcion introducidas por el usuario
            Task nuevaTarea = new Task();
            nuevaTarea.title = titulo;
            nuevaTarea.description = descripcion;

            //Agregar la tarea a la lista del TaskManager
            tasks.Add(nuevaTarea);
            //Guardar los cambios en el JSON
            SaveTaskData();

            //Limpiar texto de los inputs
            inputTituloNueva.text = "";
            inputDescripcionNueva.text = "";
            //Ocultar la ventana modal de nueva tarea
            panelNuevaTarea.SetActive(false);
            //Refrescar la lista de tareas en pantalla            
            taskList.AddLastItem();
        }
        //Si el input del nombre esta vacio, cambiar el color a rojo
        else if (titulo.Length == 0) {
            inputTituloNueva.placeholder.color = Color.red;
        }
    }

    //Editar la informacion (titulo y descripcion) de una tarea existente
    public void EditTask() {
        //Obtener el texto de los inputs
        string titulo = inputTituloEditar.text;
        string descripcion = inputDescripcionEditar.text;

        //Comprobar que los inputs no esten vacios
        if (titulo.Length != 0 /*&& descripcion.Length != 0*/) {
            //Obtener la ultima tarea abierta y modificar sus datos por los introducidos
            tasks[idOpenTask].title = titulo;
            tasks[idOpenTask].description = descripcion;

            //Guardar los cambios en el JSON
            SaveTaskData();

            //Limpiar texto en los campos
            inputTituloEditar.text = string.Empty;
            inputDescripcionEditar.text = string.Empty;

            //Ocultar la ventana modal de nueva tarea
            panelEditarTarea.SetActive(false);
            //Refrescar la lista de tareas en pantalla 
            taskList.ReloadList();
        }
        //Si el input del nombre esta vacio, cambiar el color a rojo
        else if (titulo.Length == 0) {
            inputTituloEditar.placeholder.color = Color.red;
        }
    }

    //Marcar la tarea como completada
    public void CompleteTask () {

        //Eliminarla de la lista
        DeleteTask();
        //Sumar 5 monedas por tarea completada
        int monedasActuales = PlayerPrefs.GetInt("Monedas");
        PlayerPrefs.SetInt("Monedas", monedasActuales + 1);

        //Aumentar el numero de tareas completadas
        float tareasCompletadas = PlayerPrefs.GetFloat("TareasCompletadas");
        PlayerPrefs.SetFloat("TareasCompletadas", tareasCompletadas + 1);

        //Recuperar de la bd tareas almacenadas hoy
        EstadisticasDB estadisticasDB = new EstadisticasDB();
        string fechaActual = DateTime.Today.Date.ToShortDateString();
        int tareasGuardadas = 0;
        System.Data.IDataReader reader = estadisticasDB.BuscarTareasPorFecha(fechaActual);
        while (reader.Read()) {
            tareasGuardadas = reader.GetInt32(0);
        }
        //Sumar minutos y guardarlos
        tareasGuardadas++;
        estadisticasDB.GuardarTareasPorFecha(fechaActual, tareasGuardadas);

        estadisticasDB.close();

        //Comprobar logro (id = 4)
        LogrosManager.ComprobarLogroCompletado(4);
    }

    //Al abrir la ventana modal de Editar Tarea, pone el titulo y la descripcion de la tarea 
    // como placeholders de los inputs (para que el usuario sepa que esta modificando)
    public void SetEditTitleDescription() {
        inputTituloEditar.transform.Find("Placeholder").GetComponent<Text>().text = tasks[idOpenTask].title;
        inputDescripcionEditar.transform.Find("Placeholder").GetComponent<Text>().text = tasks[idOpenTask].description;
    }
}
