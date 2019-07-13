using System.Collections.Generic;

//Clase auxiliar que almacena una lista de tareas para serializarla en JSON
[System.Serializable]
public class TaskData {

    public List<Task> tasks;
}

//Clase que representa las tareas para serializarlas en JSON
[System.Serializable]
public class Task {

    public string title;
    public string description;
}
