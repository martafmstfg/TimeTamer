/* Script para gestionar el archivo binario que guarda la configuracion del avatar.
 * Se utiliza para cargar la configuracion al abrir una escena en la que esta el avatar
 * y para guardar la configuracion nueva si se cambia el aspecto.
 * Agregado en el gameobject PersonajeModelo
 */ 

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class AvatarFileManager : MonoBehaviour {

    //Almacenan el estado (activo o inactivo) de cada elemento
    public bool[] arribaActivos;
    public bool[] abajoActivos;
    public bool[] zapatosActivos;
    public bool[] cuerpoActivos;
    public bool peloActivo;

    //Arrays de elementos hijos --> Se rellenan en el inspector
    public GameObject[] arriba;
    public GameObject[] abajo;
    public GameObject[] zapatos;
    public GameObject[] cuerpo; //solo las partes que se vayan a mostrar y ocultar
    public GameObject pelo;
    public GameObject ojo, pupila;

    public string meshPelo; //Poner en el inspector la mesh por defecto
    
    //Texturas 
    public string[] texturas = new string[5]; //Poner en el inspector las texturas por defecto
    private DiccionarioTexturas diccionarioTexturas;
    [Inject]
    public void Init(DiccionarioTexturas dt) {
        diccionarioTexturas = dt;
    }

    //Colores por defecto para piel, ojos y pelo
    public string[] colores = new string[3];

    public void Awake() {        

        //La primera vez que se ejecuta aparece el avatar por defecto
        if (PlayerPrefs.GetInt("AvatarPrimera", 0) == 0) {
            CargarColores(); //Asegurarse de que se ponen los colores por defecto
            //Guardar los activos y texturas actuales por si acaso
            GuardarEstadoArriba();
            GuardarEstadoAbajo();
            GuardarEstadoCuerpo();
            GuardarEstadoZapatos();
            GuardarFichero();

            PlayerPrefs.SetInt("AvatarPrimera", 1);
        }  
        //Si no es la primera vez que se ejecuta, la configuracion se carga desde el fichero
        else {
            CargarFichero();
            CargarActivos();
            CargarTexturas();
            CargarPeinado();
            CargarColores();
        }
    }    

    //Muestra y oculta las partes del avatar segun el estado de cada una, cargado desde el fichero
    public void CargarActivos () {
        for (int i = 0; i < arribaActivos.Length; i++) {
            arriba[i].SetActive(arribaActivos[i]);
        }

        for (int i = 0; i < abajoActivos.Length; i++) {
            abajo[i].SetActive(abajoActivos[i]);
        }

        for (int i = 0; i < zapatosActivos.Length; i++) {
            zapatos[i].SetActive(zapatosActivos[i]);
        }

        for (int i = 0; i < cuerpoActivos.Length; i++) {
            cuerpo[i].SetActive(cuerpoActivos[i]);
        }

        pelo.SetActive(peloActivo);
    }
   
    //Cargar la textura que se esta usando en cada elemento activo
    public void CargarTexturas () {                

        Texture tex;

        //Parte de arriba (indice 0)
        tex = diccionarioTexturas.GetTextura(texturas[0]); //Buscar la textura en el diccionario por el nombre
        for (int i = 0; i < arriba.Length; i++) {
            if (arriba[i].activeSelf) {
                arriba[i].GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
            }
        }

        //Parte de abajo (indice 1)
        tex = diccionarioTexturas.GetTextura(texturas[1]); //Buscar la textura en el diccionario por el nombre
        for (int i = 0; i < abajo.Length; i++) {
            if (abajo[i].activeSelf) {
                abajo[i].GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
            }
        }

        //Zapatos (indice 2)
        tex = diccionarioTexturas.GetTextura(texturas[2]); //Buscar la textura en el diccionario por el nombre
        for (int i = 0; i < zapatos.Length; i++) {
            if (zapatos[i].activeSelf) {
                zapatos[i].GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
            }
        }

        //Ojos (indices 3 y 4)
        tex = diccionarioTexturas.GetTextura(texturas[3]);
        ojo.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
        tex = diccionarioTexturas.GetTextura(texturas[4]);
        pupila.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
    }

    //Cargar el modelo del peinado obtenido desde el archivo binario
    private void CargarPeinado () {

        if(peloActivo) {
            //Cargar mesh
            GameObject peinado = Resources.Load("Peinados/" + meshPelo) as GameObject;
            Mesh nuevaMesh = peinado.GetComponentInChildren<MeshFilter>().sharedMesh;

            //Asignarla al objeto pelo
            pelo.GetComponent<MeshFilter>().sharedMesh = nuevaMesh;
        }        
    }

    //GUARDAR ESTADO DE LOS ELEMENTOS
    //Se llaman antes de salir de la escena de personalización del avatar
    private void GuardarEstadoArriba () {
        arribaActivos = new bool[arriba.Length];
        for(int i=0; i < arriba.Length; i++) {
            arribaActivos[i] = arriba[i].activeSelf;
        }

        peloActivo = pelo.activeSelf;
    }

    private void GuardarEstadoAbajo() {
        abajoActivos = new bool[abajo.Length];
        for (int i = 0; i < abajo.Length; i++) {
            abajoActivos[i] = abajo[i].activeSelf;
        }
    }

    private void GuardarEstadoZapatos() {
        zapatosActivos = new bool[zapatos.Length];
        for (int i = 0; i < zapatos.Length; i++) {
            zapatosActivos[i] = zapatos[i].activeSelf;
        }
    }

    private void GuardarEstadoCuerpo() {
        cuerpoActivos = new bool[cuerpo.Length];
        for (int i = 0; i < cuerpo.Length; i++) {
            cuerpoActivos[i] = cuerpo[i].activeSelf;
        }
    }

    //Guardar textura en uso para cierta parte
    //El avatar editor lo llama cada vez que se selecciona una nueva prenda
    public void GuardarTextura (int parte, string textura) {        
        texturas[parte] = textura;
    }

    //Guardar el nombre de la mesh del peinado que esta usando
    public void GuardarPeinado (string mesh) {
        meshPelo = mesh;
    }
    
    public string GetColor (int i) {
        return colores[i];
    }

    //Guardar color en uso para cierta parte
    public void GuardarColor (int i, string color) {
        colores[i] = "#"+color;
    }

    //Poner los colores guardados en el array, que o bien tiene los colores por defecto,
    // o bien se ha rellenado con los valores guardados en el fichero binario
    public void CargarColores () {        

        Color color;
        //Cabeza (indice 0 en el array de colores)
        ColorUtility.TryParseHtmlString(colores[0], out color);
        GameObject.Find("Cabeza").GetComponent<Renderer>().sharedMaterial.SetColor("_Color", color);

        //Ojos (indice 1)
        ColorUtility.TryParseHtmlString(colores[1], out color);
        pupila.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", color);

        //Pelo (indice 2)
        ColorUtility.TryParseHtmlString(colores[2], out color);
        pelo.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", color);
    }

    //GESTION DEL FICHERO
    public void CargarFichero () {

        if(File.Exists(Application.persistentDataPath + "/avatarData.dat")) {
            BinaryFormatter bf = new BinaryFormatter ();
            FileStream file = File.Open(Application.persistentDataPath + "/avatarData.dat", FileMode.Open);
            AvatarData data = (AvatarData)bf.Deserialize(file);

            arribaActivos = data.arribaActivos;
            abajoActivos = data.abajoActivos;
            zapatosActivos = data.zapatosActivos;
            cuerpoActivos = data.cuerpoActivo;
            peloActivo = data.peloActivo;
            texturas = data.texturas;
            meshPelo = data.meshPelo;
            colores = data.colores;

            file.Close();
        } 
    }

    private void GuardarFichero () {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/avatarData.dat");

        AvatarData data = new AvatarData {
            arribaActivos = this.arribaActivos,
            abajoActivos = this.abajoActivos,
            zapatosActivos = this.zapatosActivos,
            cuerpoActivo = this.cuerpoActivos,
            peloActivo = this.peloActivo,
            texturas = this.texturas,
            meshPelo = this.meshPelo,
            colores = this.colores
        };

        bf.Serialize(file, data);
        file.Close();
    }

    public void OnDestroy() {
        if (SceneManager.GetActiveScene().name.Equals("Avatar")) {
            GuardarEstadoArriba();
            GuardarEstadoAbajo();
            GuardarEstadoZapatos();
            GuardarEstadoCuerpo();
            GuardarFichero();
        }
    }    
} 

//Clase auxiliar serializable para guardar y cargar los datos del fichero binario
[Serializable]
public class AvatarData {
    public bool[] arribaActivos;
    public bool[] abajoActivos;
    public bool[] zapatosActivos;
    public bool[] cuerpoActivo;
    public bool peloActivo;

    public string[] texturas;

    public string meshPelo;

    public string[] colores;
}


