using System.Collections.Generic;

//Clase auxiliar que almacena una lista de camisetas para serializarla en un fichero
[System.Serializable]
public class ItemData {

    public List<Item> items;
}

//Clase auxiliar que representa los items ara serializarlos en un fichero
[System.Serializable]
public class Item {

    public string nombre;
    public string categoria;
    public int precio;
    public string icono;
    public string mesh;
    public string textura;
    public bool desbloqueado;
}
