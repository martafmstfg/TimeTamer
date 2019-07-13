using Zenject;
using NUnit.Framework;
using System;
using NSubstitute;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class TasksTests : ZenjectUnitTestFixture {

    [SetUp]
    public void BeforeEveryTest() {
        //Container.Bind<TaskManager>().AsSingle();
        //GameObject taskmanagerprefab = (GameObject)Resources.Load("prefabs/TaskManager", typeof(GameObject));

        //Container.Bind<TaskManager>().FromInstance(new TaskManager());
        //Container.Bind<TaskList>().FromInstance(new TaskList());

        //Container.InstantiatePrefab(taskmanagerprefab);
        //Container.InstantiatePrefabForComponent<TaskManager>(taskmanagerprefab);

        Container.Bind<TaskManager>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<TaskList>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<SimpleObjectPool>().FromNewComponentOnNewGameObject().AsSingle();


        Container.Inject(this);   

        //_taskmanager = new GameObject().AddComponent<TaskManager>();
    }

    [Inject]
    TaskManager _taskmanager;

    [Inject]
    TaskList _tasklist;

    [Inject]
    SimpleObjectPool _renglonPool;

    /***** TASK MANAGER *****/

    //Comprueba que se cargan tareas desde un archivo json existente
    [Test]
    public void TestLoadTasksFromExistingJSON() {
        _taskmanager.LoadTasksData();

        //Comprobar que existe una lista de tareas, aunque este vacia
        // (si no existiera el archivo, saltaria una excepcion)
        Assert.That(_taskmanager.tasks.Count, Is.GreaterThanOrEqualTo(0));
    }

    //Comprueba que si no existe el archivo json, salta una excepcion
    [Test]
    public void TestLoadingTasksFails() {
        _taskmanager.taskDataFileName = ""; //cambiar nombre del fichero para que falle

        Assert.That(() => _taskmanager.LoadTasksData(), Throws.TypeOf<FileNotFoundException>());
    }

    //Comprueba que la tarea seleccionada se elimina correctamente
    [Test]
    public void TestDeleteTask () {
        _taskmanager.Awake(); //cargar tareas
        _tasklist.tasks = _taskmanager.tasks; //asignar lista de tareas
        _renglonPool.prefab = (GameObject)Resources.Load("prefabs/Renglon", typeof(GameObject));
        _tasklist.renglonPool = _renglonPool;
        GameObject panelInfo = new GameObject(); //Mock GameObject para el task manager
        _taskmanager.panelInfo = panelInfo;

        //Crear nueva tarea de prueba y annadirla
        Task task = new Task {
            title = "titulo",
            description = "descripcion"
        };
        _tasklist.CreateItem(task, 0);

        //Hijos del content (game object que contiene el script TaskList, padre de los renglones) antes de eliminar la tarea
        int renglonesPrev = _tasklist.gameObject.transform.childCount;
        _taskmanager.idOpenTask = 0; //id de la tarea que se va a eliminar
        _taskmanager.DeleteTask(); //Elimina la tarea con el indice guardado en idOpenTask
        //Hijos del content (game object que contiene el script TaskList, padre de los renglones) antes de eliminar la tarea
        int renglonesPost = _tasklist.gameObject.transform.childCount;
        //Comprobar que el content tiene un hijo menos
        Assert.That(renglonesPost, Is.EqualTo(renglonesPrev + -1));

        //Comprobar que la tarea ya no existe en la lista
        bool tareaEnLista = _taskmanager.tasks.Contains(task);
        Assert.That(tareaEnLista, Is.False);
    }

    //Comprueba que se ha creado una nueva tarea correctamente
    [Test]
    public void TestNewTask() {
        _taskmanager.Awake(); //cargar tareas
        _tasklist.tasks = _taskmanager.tasks; //asignar lista de tareas
        _renglonPool.prefab = (GameObject)Resources.Load("prefabs/Renglon", typeof(GameObject));
        _tasklist.renglonPool = _renglonPool;

        //Mock inputs
        InputField inputTituloNueva = new GameObject().AddComponent<InputField>();
        inputTituloNueva.text = "Titulo";
        InputField inputDescripcionNueva = new GameObject().AddComponent<InputField>();
        inputDescripcionNueva.text = "Descripcion";
        _taskmanager.inputTituloNueva = inputTituloNueva;
        _taskmanager.inputDescripcionNueva = inputDescripcionNueva;
        //Mock panel editar tarea
        GameObject panelNueva = new GameObject();
        _taskmanager.panelNuevaTarea = panelNueva;


        //Longitud de la lista de tareas del Task Manager antes de crear la tarea
        int longitudPrev = _taskmanager.tasks.Count;
        //Crea tarea con los datos del input
        _taskmanager.NewTask();
        //Longitud de la lista de tareas del Task Manager despues de crear la tarea
        int longitudPost = _taskmanager.tasks.Count;
        //Comprobar que la lista tiene un elemento mas
        Assert.That(longitudPost, Is.EqualTo(longitudPrev + 1));

        //Obtener la ultima tarea de la lista y comprobar que la informacion coincide
        Task ultima = _taskmanager.tasks[longitudPost - 1];
        Assert.That(ultima.title, Is.EqualTo("Titulo"));
        Assert.That(ultima.description, Is.EqualTo("Descripcion"));
    }

    //Comprobar que la tarea NO se crea si el input del titulo esta vacio
    [Test]
    public void TestCantCreateNewTaskIfTitleEmpty() {
        _taskmanager.Awake(); //cargar tareas       

        //Mock inputs
        InputField inputTituloNueva = new GameObject().AddComponent<InputField>();
        inputTituloNueva.text = "";
        inputTituloNueva.placeholder = new GameObject().AddComponent<Text>();
        InputField inputDescripcionNueva = new GameObject().AddComponent<InputField>();
        inputDescripcionNueva.text = "Descripcion";
        _taskmanager.inputTituloNueva = inputTituloNueva;
        _taskmanager.inputDescripcionNueva = inputDescripcionNueva;       


        //Longitud de la lista de tareas del Task Manager antes de crear la tarea
        int longitudPrev = _taskmanager.tasks.Count;
        //Crea tarea con los datos del input
        _taskmanager.NewTask();
        //Longitud de la lista de tareas del Task Manager despues de intentar crear la tarea
        int longitudPost = _taskmanager.tasks.Count;
        //Comprobar que la lista tiene el mismo numero de elementos
        Assert.That(longitudPost, Is.EqualTo(longitudPrev));
    }

    //El metodo para editar una tarea es practicamente igual al metodo para crearla
        

    /***** TASK LIST *****/

    //Comprueba que se annade un renglon mas a la lista de tareas, con los datos correspondientes
    [Test]
    public void TestCreateItem() {
        //Crear nueva tarea
        Task task = new Task {
            title = "titulo",
            description = "descripcion"
        };

        //Inicializar renglonPool de TaskList
        _renglonPool.prefab = (GameObject)Resources.Load("prefabs/Renglon", typeof(GameObject));
        _tasklist.renglonPool = _renglonPool;

        //Hijos del content (game object que contiene el script TaskList, padre de los renglones) antes de annadir la tarea
        int renglonesPrev = _tasklist.gameObject.transform.childCount;
        //Annadir renglon para la tarea
        _tasklist.CreateItem(task, 0); 
        //Hijos del content despues de annadir la tarea
        int renglonesPost = _tasklist.gameObject.transform.childCount;
        //Comprobar que el content tiene un hijo mas
        Assert.That(renglonesPost, Is.EqualTo(renglonesPrev+1));

        //Obtener el componente TaskButton del nuevo renglon creado
        GameObject nuevoRenglon = _tasklist.gameObject.transform.GetChild(renglonesPost - 1).gameObject;
        TaskButton nuevoTaskButton = nuevoRenglon.GetComponent<TaskButton>();
        //Comprobar que la informacion del TaskButton es la correcta
        Assert.That(nuevoTaskButton.id, Is.EqualTo(0));
        Assert.That(nuevoTaskButton.task, Is.EqualTo(task));
    }


    //Comprueba que, tras ejecutar el metodo AddAllItems, el numero de hijos del content del scrollview
    // (es decir, el numero de renglones) es igual al numero de tareas que hay en la lista tasks
    [Test]
    public void TestTasksAddedToList() {
        _taskmanager.Awake(); //cargar tareas
        _tasklist.tasks = _taskmanager.tasks; //asignar lista de tareas
        _renglonPool.prefab = (GameObject)Resources.Load("prefabs/Renglon", typeof(GameObject));
        _tasklist.renglonPool = _renglonPool;

        _tasklist.AddAllItems(); //Metodo que crea un renglon por tarea de la lista

        //Numero hijos content (renglones) = numero tareas cargadas desde el json
        Assert.That(_tasklist.gameObject.transform.childCount, Is.EqualTo(_tasklist.tasks.Count));
    }

    //Comprobar que se muestran los datos correspondientes a la tarea que se ha pulsado
    [Test]
    public void TestTaskInfo() {
       
        //Crear tarea de prueba e insertar los datos
        Container.Bind<Task>().FromInstance(new Task());
        var _task = Container.Resolve<Task>();
        _task.title = "Titulo";
        _task.description = "Descripcion";
        //Crear boton / renglon de prueba
        Container.Bind<TaskButton>().FromInstance(new TaskButton());
        var _taskbutton = Container.Resolve<TaskButton>();
        _taskbutton.task = _task; //asignarle la tarea
        _taskbutton.id = 0;

        //Crear panel info con los dos text components
        Image darklayer = new GameObject().AddComponent<Image>();
        _taskmanager.panelInfo = darklayer.gameObject;
        Image panel = new GameObject("PanelVerTarea").AddComponent<Image>();
        panel.transform.SetParent(darklayer.transform);
        Text title = new GameObject("Title").AddComponent<Text>();
        title.transform.SetParent(panel.transform);
        Text desc = new GameObject("Description").AddComponent<Text>();
        desc.transform.SetParent(panel.transform);
       
        _taskmanager.ShowTaskInfo(_taskbutton.task.title, _taskbutton.task.description, 0); //Metodo que muestra la info en TaskManager

        Assert.That(title.text, Is.EqualTo(_taskbutton.task.title)); //comprobar que los titulos coinciden
        Assert.That(desc.text, Is.EqualTo(_taskbutton.task.description)); //comprobar que las descripciones coinciden
    }

    //Comprobar que la lista de tareas se vacia correctamente
    [Test]
    public void TestEmptyList () {
        _taskmanager.Awake(); //cargar tareas
        _tasklist.tasks = _taskmanager.tasks; //asignar lista de tareas

        _tasklist.EmptyList(); //Metodo que vacia la lista

        //Comprobar que el content ahora no tiene ningun renglon hijo
        Assert.That(_tasklist.gameObject.transform.childCount, Is.EqualTo(0));
    }

}