using Zenject;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

[TestFixture]
public class ItemsTests : ZenjectUnitTestFixture {

    [SetUp]
    public void BeforeEveryTest() {        

        Container.Bind<ItemsManager>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<DataManager>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<SimpleObjectPool>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<ItemsList>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<DiccionarioTexturas>().FromResource("Prefabs/DiccionarioTexturas").AsSingle();
        Container.Bind<AvatarFileManager>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<AvatarEditor>().FromNewComponentOnNewGameObject().AsSingle();

        Container.Inject(this);       
    }

    [Inject]
    ItemsManager _itemsManager;

    [Inject]
    DataManager _dataManager;

    [Inject]
    SimpleObjectPool _itemPool;

    [Inject]
    DiccionarioTexturas _diccionarioTexturas;

    [Inject]
    AvatarFileManager _avatarFileManager;

    [Inject]
    AvatarEditor _avatarEditor;

    [Inject]
    ItemsList _itemsList;


    /***** ITEMS MANAGER *****/    

    //Comprobar que el metodo PrimeraCarga copia el JSON correspondiente de Resources a PersistendDataPath
    [Test]
    public void TestCopiaPrimeraCarga () {
        string categoria = "camisetas";
        string jsonOriginal = Resources.Load<TextAsset>("JSONitems/" + categoria).text;

        //Metodo que copia el archivo
        _itemsManager.PrimeraCarga(categoria);

        //Comprobar que existe el archivo en PersistentDataPath
        string persistentPath = Path.Combine(Application.persistentDataPath, categoria + ".json");
        Assert.That(File.Exists(persistentPath), Is.True);

        //Comprobar que el texto de dicho archivo es el mismo que el original
        string jsonCopiado = File.ReadAllText(persistentPath, Encoding.UTF8);
        Assert.That(jsonCopiado, Is.EqualTo(jsonOriginal));
    }

    //Comprobar que se rellena una lista a partir del json correspondiente a la categoria
    [Test]
    public void TestCargarItems () {

        //Asegurar que el JSON original se carga desde Resources y se copia en el PersistenDataPath
        //Esto es necesario por si es la primera vez que se ejecuta el test, antes de ejecutar la app en el editor
        // y los JSON de Resources (solo lectura) no han sido copiados aun al PersistenDataPath (lectura y escritura)
        _itemsManager.PrimeraCarga("camisetas");

        List<Item> lista; //lista a rellenar

        //Metodo que carga el JSON desde PersistenDataPath para obtener la lista de items de la categoria
        lista = _itemsManager.LoadData("camisetas");

        //Comprobar que la lista no esta vacia (contiene algun elemento)
        Assert.That(lista.Count, Is.GreaterThan(0));

        //Obtener el primer elemento de la lista
        Item item = lista[0];
        //Comprobar que el campo "categoria" del item es "camiseta", es decir, que se ha cargado del json correcto
        Assert.That(item.categoria, Is.EqualTo("camiseta"));
    }

    //Comprobar que se ponen los datos correctos al abrir el panel de informacion de un item    
    [Test]
    public void TestDatosInfoItem () {
        //Crear item solo con los datos necesarios para probar el metodo
        Item item = new Item {
            icono = "camisetaAzul",
            precio = 100
        };

        //Generar panel a partir del prefab
        GameObject panelInfo = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/InfoItem") as GameObject);
        _itemsManager.infoItem = panelInfo;
        _itemsManager.ShowItemInfo(item, 0, "camiseta"); //Metodo que muestra el panel de info

        //Comprobar que la informacion en el panel es la correcta
        //Precio 
        string precio = panelInfo.transform.Find("PanelInfoItem").transform.Find("Precio").GetComponentInChildren<Text>().text;
        Assert.That(item.precio.ToString, Is.EqualTo(precio));
        //Sprite del icono
        string sprite = panelInfo.transform.Find("PanelInfoItem").transform.Find("Icono").GetComponent<Image>().sprite.name;
        Assert.That(item.icono, Is.EqualTo(sprite));

        UnityEngine.Object.DestroyImmediate(panelInfo);
    }

    //Comprobar que el boton de comprar es interactable si se tienen suficientes monedas
    [Test]
    public void TestBotonComprarActivo() {
        //Crear item solo con los datos necesarios para probar el metodo
        Item item = new Item {
            icono = "camisetaAzul",
            precio = 100
        };

        PlayerPrefs.SetInt("Monedas", 1000);

        //Generar panel a partir del prefab
        GameObject panelInfo = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/InfoItem") as GameObject);
        _itemsManager.infoItem = panelInfo;
        _itemsManager.ShowItemInfo(item, 0, "camiseta"); //Metodo que muestra el panel de info

        //Comprobar que el panel de comprar esta activo
        bool activo = panelInfo.transform.Find("PanelInfoItem").Find("Btn_Comprar").GetComponent<Button>().interactable;
        Assert.That(activo, Is.True);

        UnityEngine.Object.DestroyImmediate(panelInfo);
    }

    //Comprobar que el boton de comprar NO es interactable si NO se tienen suficientes monedas
    [Test]
    public void TestBotonComprarInactivo() {
        //Crear item solo con los datos necesarios para probar el metodo
        Item item = new Item {
            icono = "camisetaAzul",
            precio = 100
        };

        PlayerPrefs.SetInt("Monedas", 0);

        //Generar panel a partir del prefab
        GameObject panelInfo = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/InfoItem") as GameObject);
        _itemsManager.infoItem = panelInfo;
        _itemsManager.ShowItemInfo(item, 0, "camiseta"); //Metodo que muestra el panel de info

        //Comprobar que el panel de comprar esta activo
        bool activo = panelInfo.transform.Find("PanelInfoItem").Find("Btn_Comprar").GetComponent<Button>().interactable;
        Assert.That(activo, Is.False);

        UnityEngine.Object.DestroyImmediate(panelInfo);
    }

    //Comprobar que el item indicado por el id y el tipo se guarda como desbloqueado, se reduce el numero de monedas 
    // y aumenta el numero de items desbloqueados para el logro
    // !! Comentar la llamada a ItemsList.ReloadList() y la de LogrosManager.LogroCompletado
    [Test]
    public void TestComprarItem () {
        _dataManager.Awake();
        _itemsList.Awake();

        //Crear item solo con los datos necesarios para probar el metodo
        Item item = new Item {
            categoria = "camiseta",
            precio = 100,
            desbloqueado = false
        };

        //Diccionario y lista
        Dictionary<string, List<Item>> listas = new Dictionary<string, List<Item>>(); ;
        List<Item> camisetas = new List<Item>();
        camisetas.Add(item);
        listas.Add("camisetas", camisetas);
        _itemsManager.listas = listas;

        //Monedas
        int monedasPrev = 500;
        PlayerPrefs.SetInt("Monedas", monedasPrev);

        //Items desbloqueados (logros)
        float itemsDesbloqueadosPrev = PlayerPrefs.GetFloat("ObjetosConseguidos");

        //Generar panel a partir del prefab
        GameObject panelInfo = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/InfoItem") as GameObject);
        _itemsManager.infoItem = panelInfo;        

        //Comprar el item
        _itemsManager.DesbloquearItem("camisetas", 0);

        //Comprobar que el item esta desbloqueado
        Assert.That(item.desbloqueado, Is.True);
        //Comprobar que se ha reducido el numero de monedas
        int monedasPost = PlayerPrefs.GetInt("Monedas");
        Assert.That(monedasPost, Is.EqualTo(monedasPrev - item.precio));
        //Comprobar que hay un item desbloqueado mas
        float itemsDesbloqueadosPost = PlayerPrefs.GetFloat("ObjetosConseguidos");
        Assert.That(itemsDesbloqueadosPost, Is.EqualTo(itemsDesbloqueadosPrev + 1));

        UnityEngine.Object.DestroyImmediate(panelInfo);
    }

    /***** ITEMS LIST *****/

    //Comprobar que los items de una categoria se cargan correctamente en la UI desde la lista del json
    //Prueba a la vez el metodo Cargar+Categoria y el CargarItems general
    [Test]
    public void TestMostrarItemsUI () {
        //Cargar las listas desde los jsons
        _itemsManager.Awake();

        //Crear gameObject para el content ojos
        GameObject contentOjos = new GameObject("ContentOjos");       

        _itemPool.prefab = (GameObject)Resources.Load("Prefabs/Item", typeof(GameObject));
        _itemsList.itemPool = _itemPool;

        //Se va a probar a cargar los ojos, llama a su vez a CargarItems
        _itemsList.CargarOjos(true);

        //Comprobar que el content ahora tantos hijos como elementos hay en la lista ojos
        Assert.That(GameObject.Find("ContentOjos").transform.childCount, Is.EqualTo(_itemsManager.ojos.Count));

        //Comprobar que la informacion (el icono) que se muestra es el correspondiente al item, por ejemplo el primero
        string nombreEsperado = _itemsManager.ojos[0].icono;
        string nombreIcono = GameObject.Find("ContentOjos").transform.GetChild(0).Find("Icono").GetComponent<Image>().sprite.name;
        Assert.That(nombreIcono, Is.EqualTo(nombreEsperado));

        UnityEngine.Object.DestroyImmediate(contentOjos);
    }

    //Comprobar que la lista de items dada se vacia correctamente (en la    UI)
    [Test]
    public void TestEmptyList() {
        //Cargar las listas desde los jsons
        _itemsManager.Awake();

        //Crear gameObject para el content ojos
        GameObject contentOjos = new GameObject("ContentOjos");

        _itemPool.prefab = (GameObject)Resources.Load("Prefabs/Item", typeof(GameObject));
        _itemsList.itemPool = _itemPool;

        //Se va a probar a cargar los ojos
        _itemsList.CargarOjos(true);

        //Despues se borra la lista de ojos
        _itemsList.EmptyList("ojos");

        //Comprobar que el content ahora no tiene ningun renglon hijo
        Assert.That(GameObject.Find("ContentOjos").transform.childCount, Is.EqualTo(0));
    }
}