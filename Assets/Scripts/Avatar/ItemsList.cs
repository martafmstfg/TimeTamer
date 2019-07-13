/* Script que gestiona la interfaz de la pantalla de personalizacion del avatar.
 * Carga los items de cada categoria, con su icono y sus datos correspondientes,
 * actualiza los elementos de la interfaz cuando se produce algun cambio, etc.
 * Agregado al gameobject ItemsManager
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ItemsList : MonoBehaviour {

    public GameObject prefabItem; //Prefab del item para instanciar uno por cada item de la lista
    public SimpleObjectPool itemPool; //Los items se crean mediante un metodo de pooling generico

    //Dependencias
    private AvatarEditor avatarEditor;
    private ItemsManager itemsManager;
    [Inject]
    public void Init(AvatarEditor AE, ItemsManager im) {
        avatarEditor = AE;
        itemsManager = im;
    }

    public static ItemsList instance {
        private set;
        get;
    } //Singleton

    public void Awake() {        

        //Comprobar si ya hay algun otro objeto con el script ItemList
        ItemsList[] instancias = FindObjectsOfType <ItemsList> ();
        if (instancias.Length > 1) {
            Destroy(gameObject);
        }
        else instance = this;
    }
        
    void Start() {
        //La categoria que se muestra por defecto al iniciar la escena es la de los ojos
        CargarOjos(true);
    }

    /* Metodos auxiliares que son llamados cuando se pulsa el icono de una categoria,
     * para cargar en el panel los items correspondientes */
    public void CargarOjos(bool activo) {

        //Solo rellena el panel de la categoria si es el que esta activo / visible
        if (activo) {
            Transform contentOjos = GameObject.Find("ContentOjos").transform;
            //Comprueba que los items no hayan sido agregados ya al panel
            if (contentOjos.childCount == 0) {
                List<Item> ojos = itemsManager.ojos;
                CargarItems(ojos, contentOjos, "ojos");
            }
        }
    }

    public void CargarPeinados(bool activo) {

        //Solo rellena el panel de la categoria si es el que esta activo / visible
        if (activo) {
            Transform contentPeinados = GameObject.Find("ContentPeinados").transform;
            //Comprueba que los items no hayan sido agregados ya al panel
            if (contentPeinados.childCount == 0) {
                List<Item> peinados = itemsManager.peinados;
                CargarItems(peinados, contentPeinados, "peinados");
            }
        }
    }

    public void CargarCamisetas (bool activo) {

        //Solo rellena el panel de la categoria si es el que esta activo / visible
        if (activo) {
            Transform contentCamisetas = GameObject.Find("ContentCamisetas").transform;
            //Comprueba que los items no hayan sido agregados ya al panel
            if (contentCamisetas.childCount == 0) {
                List<Item> camisetas = itemsManager.camisetas;
                CargarItems(camisetas, contentCamisetas, "camisetas");
            }            
        }    
    }

    public void CargarPantalones(bool activo) {

        //Solo rellena el panel de la categoria si es el que esta activo / visible
        if (activo) {
            Transform contentPantalones = GameObject.Find("ContentPantalones").transform;
            //Comprueba que los items no hayan sido agregados ya al panel
            if (contentPantalones.childCount == 0) {
                List<Item> pantalones = itemsManager.pantalones;
                CargarItems(pantalones, contentPantalones, "pantalones");
            }
        }
    }

    public void CargarZapatos(bool activo) {

        //Solo rellena el panel de la categoria si es el que esta activo / visible
        if (activo) {
            Transform contentZapatos = GameObject.Find("ContentZapatos").transform;
            //Comprueba que los items no hayan sido agregados ya al panel
            if (contentZapatos.childCount == 0) {
                List<Item> zapatos = itemsManager.zapatos;
                CargarItems(zapatos, contentZapatos, "zapatos");
            }
        }
    }


    //Metodo que carga los items de la lista recibida en el panel recibido
    //Tambien pone la informacion correspondiente en el item
    private void CargarItems (List<Item> listaItems, Transform panel, string tipo) {

        Sprite icono;
        int precio;

        //Crear un objeto en la UI por cada item y asignarle la informacion correspondiente
        for (int i = 0; i < listaItems.Count; i++) {
            Item aux = listaItems[i];
            GameObject newItem = itemPool.GetObject(); //Obtener item a partir de la pool de items
            newItem.transform.SetParent(panel, false); //Hacerlo hijo del panel de su categoria            

            //Poner sprite 
            icono = Resources.Load<Sprite>("IconosItems/" + aux.icono);
            newItem.transform.Find("Icono").GetComponent<Image>().sprite = icono;

            //Si aun esta bloqueado, mostrar su precio
            if(!aux.desbloqueado) {
                precio = aux.precio;
                newItem.transform.Find("Precio").gameObject.SetActive(true);
                newItem.transform.Find("Precio").GetComponentInChildren<Text>().text = precio.ToString();                

                //Si esta bloqueado, al pulsarlo se mostrara el panel de info
                int id = i; //evitar closure problem de c#
                newItem.GetComponent<Button>().onClick.AddListener(() => itemsManager.ShowItemInfo(aux, id, tipo));
            }
            //Si esta desbloqueado, ocultar el precio
            else {
                newItem.transform.Find("Precio").gameObject.SetActive(false);
                                
                string categoria = aux.categoria;
                string textura = aux.textura;
                string mesh = aux.mesh;

                //Al pulsarlo, se modificara el avatar
                //Añadir listener que llama al metodo correspondiente al tipo de prenda
                switch (tipo) {
                    case "camisetas":
                        newItem.GetComponent<Button>().onClick.AddListener(() => avatarEditor.MostrarArriba(categoria, textura));
                        break;
                    case "pantalones":
                        newItem.GetComponent<Button>().onClick.AddListener(() => avatarEditor.MostrarAbajo(categoria, textura));
                        break;
                    case "zapatos":
                        newItem.GetComponent<Button>().onClick.AddListener(() => avatarEditor.MostrarZapatos(categoria, textura));
                        break;
                    case "peinados":
                        newItem.GetComponent<Button>().onClick.AddListener(() => avatarEditor.MostrarPelo(mesh));
                        break;
                    case "ojos":
                        //mesh = ojo base, textura = pupila
                        newItem.GetComponent<Button>().onClick.AddListener(() => avatarEditor.MostrarOjos(mesh, textura));
                        break;
                }
            }
        }
    }    

    //Refrescar la lista de items en pantalla
    public void ReloadList(string tipo) {
        EmptyList(tipo); //Primero la vacia

        //Luego recarga el panel correspondiente
        switch(tipo) {
            case "ojos":
                CargarOjos(true);
                break;
            case "peinados":
                CargarPeinados(true);
                break;
            case "camisetas":
                CargarCamisetas(true);
                break;
            case "pantalones":
                CargarPantalones(true);
                break;
            case "zapatos":
                CargarZapatos(true);
                break; 
        }
    }

    //Eliminar todos los items mediante el sistema de pooling
    public void EmptyList(string tipo) {

        tipo = char.ToUpper(tipo[0]) + tipo.Substring(1);

        Transform content = GameObject.Find("Content"+tipo).transform;

        while (content.childCount > 0) {
            GameObject toRemove = content.GetChild(0).gameObject;
            toRemove.GetComponent<Button>().onClick.RemoveAllListeners();
            itemPool.ReturnObject(toRemove);
        }
    }

}
