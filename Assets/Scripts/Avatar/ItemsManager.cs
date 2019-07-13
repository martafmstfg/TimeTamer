/* Script que gestiona los archivos JSON que contienen los items del avatar.
 * Tambien se encarga de mostrar la informacion de los items y de gestionar las compras.
 */


using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text;

public class ItemsManager : MonoBehaviour {

    //Diccionario para guardar las listas de cada categoria
    public Dictionary<string, List<Item>> listas;
    //Listas de cada categoria
    public List<Item> ojos;
    public List<Item> peinados;
    public List<Item> camisetas;
    public List<Item> pantalones;
    public List<Item> zapatos;
    //JSON de cada categoria
    private string ojosDataFileName = "ojos.json";
    private string peinadosDataFileName = "peinados.json";
    private string camisetasDataFileName = "camisetas.json";
    private string pantalonesDataFileName = "pantalones.json";
    private string zapatosDataFileName = "zapatos.json";

    private int itemAbiertoId;

    public AudioSource audioSource;
    public AudioClip comprarClip;   

    //UI
    public GameObject infoItem; //Panel con info del item    

    public void Awake() {

        //PlayerPrefs.SetInt("Monedas", 1000);

        //La primera vez que se ejecuta se cargan los jsons desde la carpeta Resources
        if (PlayerPrefs.GetInt("PrimeraEjecucion", 1) == 1) {
            Debug.Log("Primera vez que se ejecuta");
            PlayerPrefs.SetInt("PrimeraEjecucion", 0);

            PrimeraCarga("ojos");
            PrimeraCarga("peinados");
            PrimeraCarga("camisetas");
            PrimeraCarga("pantalones");
            PrimeraCarga("zapatos");
        }

        //Inicializar listas a partir del JSON y añadirlas al diccionario
        listas = new Dictionary<string, List<Item>>();
        ojos = LoadData("ojos"); 
        listas.Add("ojos", ojos);
        peinados = LoadData("peinados");
        listas.Add("peinados", peinados);
        camisetas = LoadData("camisetas");
        listas.Add("camisetas", camisetas);
        pantalones = LoadData("pantalones");
        listas.Add("pantalones", pantalones);
        zapatos = LoadData("zapatos");
        listas.Add("zapatos", zapatos);
    }

    //Si se ha abierto la aplicacion por primera vez, hay que copiar el json de Resources (solo lectura)
    // al Application.persistentDataPath. Esta copia es la que se leera y editara realmente.
    public void PrimeraCarga (string categoria) {
        var jsonTextFile = Resources.Load<TextAsset>("JSONitems/"+categoria);   
        string filePath = Path.Combine(Application.persistentDataPath, categoria+".json");
        string dataAsJson = jsonTextFile.text;        
        File.WriteAllText(filePath, dataAsJson, Encoding.UTF8);
    }

    //Obtener lista de items almacenados a partir del fichero JSON
    public List<Item> LoadData (string categoria) {
               
        string filePath = Path.Combine(Application.persistentDataPath, categoria+".json");
        string dataAsJson;
        List<Item> listaAux = new List<Item>(); //lista a partir del fichero guardado en persistentDataPath

        if (File.Exists(filePath)) {
            dataAsJson = File.ReadAllText(filePath, Encoding.UTF8);
            //Crear objeto ItemData a partir de la informacion del json
            ItemData loadedData = JsonUtility.FromJson<ItemData>(dataAsJson);    
            //Recuperar lista de items
            listaAux = loadedData.items;

            //Recuperar lista actualizada. Esto esta pensado para futuras actualizaciones
            // en las que se añadan nuevos items y haya que actualizar los JSONS de Resources              
            // sin perder los items desbloqueados, cuyos datos se guardan en el persistentDataPath
            var jsonTextFile = Resources.Load<TextAsset>("JSONitems/" + categoria);
            string dataJsonRes = jsonTextFile.text;
            ItemData itemDataRes = JsonUtility.FromJson<ItemData>(dataJsonRes);
            List<Item> listaRes = itemDataRes.items;

            //Comprobar si la lista de Resources tiene mas items (ha sido actualizada) que la del persistent
            if(listaRes.Count > listaAux.Count) {
                //Añadir items nuevos a la lista auxiliar
                for (int i = listaAux.Count; i < listaRes.Count; i++) {
                    listaAux.Add(listaRes[i]);
                }
            }  
        }
        /*else {
            throw new FileNotFoundException("El fichero no existe");
        } */       

        return listaAux;
    }

    //Sobreescribir el fichero JSON con la nueva informacion (item desbloqueado)
    private void SaveItemsData (string tipo) {
        string filePath = Path.Combine(Application.persistentDataPath, tipo+".json");
        List<Item> listaAux = listas[tipo];

        //Crear objeto ItemData y guardar en el la lista actual de la categoria indicada
        ItemData itemDataAux = new ItemData {
            items = listaAux
        };
        //Convertir objeto ItemData (y, con el, la lista de items de esa categoria) a JSON
        string dataAsJson = JsonUtility.ToJson(itemDataAux);
        //Sobreescribir el archivo json con el nuevo texto (si no existe el fichero, lo crea)
        File.WriteAllText(filePath, dataAsJson, Encoding.UTF8);
    }

    //Abrir la ventana modal que muestra la informacion del item recibido como argumento
    public void ShowItemInfo(Item item, int id, string tipo) {
        itemAbiertoId = id; //Guardar el id del item que se esta mostrando

        //Poner sprite y precio 
        Sprite icono = Resources.Load<Sprite>("IconosItems/" + item.icono);
        infoItem.transform.Find("PanelInfoItem").transform.Find("Icono").GetComponent<Image>().sprite = icono;
        infoItem.transform.Find("PanelInfoItem").transform.Find("Precio").GetComponentInChildren<Text>().text = item.precio.ToString();

        //Si no se tienen monedas suficientes, se desactiva el boton de comprar y se pone el precio en rojo
        if(item.precio > PlayerPrefs.GetInt("Monedas")) {
            infoItem.transform.Find("PanelInfoItem").Find("Btn_Comprar").GetComponent<Button>().interactable = false;
            infoItem.transform.Find("PanelInfoItem").transform.Find("Precio").GetComponentInChildren<Text>().color = Color.red;
        } else {
            infoItem.transform.Find("PanelInfoItem").Find("Btn_Comprar").GetComponent<Button>().onClick.RemoveAllListeners();
            infoItem.transform.Find("PanelInfoItem").Find("Btn_Comprar").GetComponent<Button>().onClick.AddListener(
                () => audioSource.PlayOneShot(comprarClip));
            infoItem.transform.Find("PanelInfoItem").Find("Btn_Comprar").GetComponent<Button>().onClick.AddListener(
                () => DesbloquearItem(tipo, id));            
        }
        
        infoItem.SetActive(true); //Activa la ventana modal con la informacion
    }

    //Metodo para comprar un item si se tienen las monedas suficientes
    public void DesbloquearItem (string tipo, int id) {
        
        int precio = listas[tipo][id].precio;
        int monedas = PlayerPrefs.GetInt("Monedas");

        //Comprobar que se tienen monedas suficientes
        if (precio <= monedas) {            
            listas[tipo][id].desbloqueado = true;
            PlayerPrefs.SetInt("Monedas", monedas - precio); //Restar monedas gastadas

            //Actualizar monedas en pantalla
            DataManager.instance.ActualizarMonedas();

            //Guardar los cambios en el JSON
            SaveItemsData(tipo);
            //Ocultar el pop-up de info item
            infoItem.SetActive(false);
            //Refrescar la lista de items en pantalla
            ItemsList.instance.ReloadList(tipo);

            //Incrementar el numero de objetos desbloqueados
            float nObjetos = PlayerPrefs.GetFloat("ObjetosConseguidos");
            PlayerPrefs.SetFloat("ObjetosConseguidos", nObjetos + 1);
            //Comprobar si se ha completado el logro de objetos (id=2)
            LogrosManager.ComprobarLogroCompletado(2);
        } 
    }
}
