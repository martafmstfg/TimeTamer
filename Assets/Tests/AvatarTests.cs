using Zenject;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class AvatarTests : ZenjectUnitTestFixture
{
    [SetUp]
    public void BeforeEveryTest() {

        //Dependencias
        Container.Bind<DiccionarioTexturas>().FromResource("Prefabs/DiccionarioTexturas").AsSingle();
        Container.Bind<AvatarFileManager>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<AvatarEditor>().FromNewComponentOnNewGameObject().AsSingle();
        
        Container.Inject(this);

        //Crear gameobject del personaje
        personajePrefab = Resources.Load<GameObject>("Prefabs/Personaje");
        personaje = MonoBehaviour.Instantiate(personajePrefab).transform.GetChild(0).gameObject;        
    }

    [Inject]
    DiccionarioTexturas _diccionarioTexturas;

    [Inject]
    AvatarFileManager _avatarFileManager;

    [Inject]
    AvatarEditor _avatarEditor;
        
    GameObject personajePrefab;
    GameObject personaje;


    /***** AVATAR EDITOR *****/

    //Comprobar que se muestra la CAMISETA con la textura dada
    // y que se muestran y ocultan las partes del cuerpo necesarias
    [Test]
    public void TestMostrarCamiseta () {
        InicializarAvatarEditor();

        Texture tex = _diccionarioTexturas.GetTextura("camisetaAzul");

        _avatarEditor.MostrarArriba("camiseta", "camisetaAzul");

        //Comprobar que se han ocultado o mostrado las partes del cuerpo necesarias
        Assert.That(_avatarEditor.camisa.activeSelf, Is.False);
        Assert.That(_avatarEditor.jersey.activeSelf, Is.False);
        Assert.That(_avatarEditor.camiseta.activeSelf, Is.True);
        Assert.That(_avatarEditor.brazos.activeSelf, Is.True);
        Assert.That(_avatarEditor.cuerpo.activeSelf, Is.False);

        //Comprobar que la textura de la camiseta es la correcta
        Assert.That(_avatarEditor.camiseta.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex"), Is.EqualTo(tex));
    }

    //Comprobar que se muestra la CAMISA con la textura dada
    // y que se muestran y ocultan las partes del cuerpo necesarias
    [Test]
    public void TestMostrarCamisa() {
        InicializarAvatarEditor();
        Texture tex = _diccionarioTexturas.GetTextura("camisaCorbata");

        _avatarEditor.MostrarArriba("camisa", "camisaCorbata");

        //Comprobar que se han ocultado o mostrado las partes del cuerpo necesarias
        Assert.That(_avatarEditor.camisa.activeSelf, Is.True);
        Assert.That(_avatarEditor.jersey.activeSelf, Is.False);
        Assert.That(_avatarEditor.camiseta.activeSelf, Is.False);
        Assert.That(_avatarEditor.brazos.activeSelf, Is.False);
        Assert.That(_avatarEditor.cuerpo.activeSelf, Is.False);

        //Comprobar que la textura de la camiseta es la correcta
        Assert.That(_avatarEditor.camisa.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex"), Is.EqualTo(tex));
    }

    //Comprobar que se muestra el JERSEY con la textura dada
    // y que se muestran y ocultan las partes del cuerpo necesarias
    [Test]
    public void TestMostrarJersey() {
        InicializarAvatarEditor();
        Texture tex = _diccionarioTexturas.GetTextura("jerseyRayas");

        _avatarEditor.MostrarArriba("jersey", "jerseyRayas");

        //Comprobar que se han ocultado o mostrado las partes del cuerpo necesarias
        Assert.That(_avatarEditor.camisa.activeSelf, Is.False);
        Assert.That(_avatarEditor.jersey.activeSelf, Is.True);
        Assert.That(_avatarEditor.camiseta.activeSelf, Is.False);
        Assert.That(_avatarEditor.brazos.activeSelf, Is.False);
        Assert.That(_avatarEditor.cuerpo.activeSelf, Is.False);

        //Comprobar que la textura de la camiseta es la correcta
        Assert.That(_avatarEditor.jersey.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex"), Is.EqualTo(tex));
    }

    //Comprobar que se muestra el PANTALON LARGO con la textura dada
    // y que se muestran y ocultan las partes del cuerpo necesarias
    [Test]
    public void TestMostrarPantalonLargo() {
        InicializarAvatarEditor();
        Texture tex = _diccionarioTexturas.GetTextura("largoNegro");

        _avatarEditor.MostrarAbajo("largo", "largoNegro");

        //Comprobar que se han ocultado o mostrado las partes del cuerpo necesarias
        Assert.That(_avatarEditor.largo.activeSelf, Is.True);
        Assert.That(_avatarEditor.corto.activeSelf, Is.False);
        Assert.That(_avatarEditor.falda1.activeSelf, Is.False);
        Assert.That(_avatarEditor.falda2.activeSelf, Is.False);
        Assert.That(_avatarEditor.piernas.activeSelf, Is.False);
        Assert.That(_avatarEditor.cintura.activeSelf, Is.False);

        //Comprobar que la textura de la camiseta es la correcta
        Assert.That(_avatarEditor.largo.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex"), Is.EqualTo(tex));
    }

    //Comprobar que se muestra el PANTALON CORTO con la textura dada
    // y que se muestran y ocultan las partes del cuerpo necesarias
    [Test]
    public void TestMostrarPantalonCorto() {
        InicializarAvatarEditor();
        Texture tex = _diccionarioTexturas.GetTextura("shortsVaqueros");

        _avatarEditor.MostrarAbajo("corto", "shortsVaqueros");

        //Comprobar que se han ocultado o mostrado las partes del cuerpo necesarias
        Assert.That(_avatarEditor.largo.activeSelf, Is.False);
        Assert.That(_avatarEditor.corto.activeSelf, Is.True);
        Assert.That(_avatarEditor.falda1.activeSelf, Is.False);
        Assert.That(_avatarEditor.falda2.activeSelf, Is.False);
        Assert.That(_avatarEditor.piernas.activeSelf, Is.True);
        Assert.That(_avatarEditor.cintura.activeSelf, Is.False);

        //Comprobar que la textura de la camiseta es la correcta
        Assert.That(_avatarEditor.corto.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex"), Is.EqualTo(tex));
    }

    //Comprobar que se muestra la FALDA 1 con la textura dada
    // y que se muestran y ocultan las partes del cuerpo necesarias
    [Test]
    public void TestMostrarFalda1() {
        InicializarAvatarEditor();
        Texture tex = _diccionarioTexturas.GetTextura("faldaCuadros");

        _avatarEditor.MostrarAbajo("falda1", "faldaCuadros");

        //Comprobar que se han ocultado o mostrado las partes del cuerpo necesarias
        Assert.That(_avatarEditor.largo.activeSelf, Is.False);
        Assert.That(_avatarEditor.corto.activeSelf, Is.False);
        Assert.That(_avatarEditor.falda1.activeSelf, Is.True);
        Assert.That(_avatarEditor.falda2.activeSelf, Is.False);
        Assert.That(_avatarEditor.piernas.activeSelf, Is.True);
        Assert.That(_avatarEditor.cintura.activeSelf, Is.False);

        //Comprobar que la textura de la camiseta es la correcta
        Assert.That(_avatarEditor.falda1.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex"), Is.EqualTo(tex));
    }


    //Comprobar que se muestran los ZAPATOS ALTOS con la textura dada
    // y que se muestran y ocultan las partes del cuerpo necesarias
    [Test]
    public void TestMostrarZapatosAltos() {
        InicializarAvatarEditor();
        Texture tex = _diccionarioTexturas.GetTextura("botas");

        _avatarEditor.MostrarZapatos("altos", "botas");

        //Comprobar que se han ocultado o mostrado las partes del cuerpo necesarias
        Assert.That(_avatarEditor.altos.activeSelf, Is.True);
        Assert.That(_avatarEditor.bajos.activeSelf, Is.False);
        Assert.That(_avatarEditor.pies.activeSelf, Is.False);

        //Comprobar que la textura de la camiseta es la correcta
        Assert.That(_avatarEditor.altos.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex"), Is.EqualTo(tex));
    }

    //Comprobar que se muestran los ZAPATOS BAJOS con la textura dada
    // y que se muestran y ocultan las partes del cuerpo necesarias
    [Test]
    public void TestMostrarZapatosBajos() {
        InicializarAvatarEditor();
        Texture tex = _diccionarioTexturas.GetTextura("zapatillas");

        _avatarEditor.MostrarZapatos("bajos", "zapatillas");

        //Comprobar que se han ocultado o mostrado las partes del cuerpo necesarias
        Assert.That(_avatarEditor.altos.activeSelf, Is.False);
        Assert.That(_avatarEditor.bajos.activeSelf, Is.True);
        Assert.That(_avatarEditor.pies.activeSelf, Is.True);

        //Comprobar que la textura de la camiseta es la correcta
        Assert.That(_avatarEditor.bajos.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex"), Is.EqualTo(tex));
    }

    //Comprobar que se oculta el pelo cuando se selecciona la opcion Calvo
    [Test]
    public void TestCalvo () {
        InicializarAvatarEditor();
        _avatarEditor.MostrarPelo("calvo");

        Assert.That(_avatarEditor.pelo.activeSelf, Is.False);
    }

    //Comprobar que se muestra el pelo y que tiene la mesh correcta
    [Test]
    public void TestMostrarPeinado() {
        InicializarAvatarEditor();
        GameObject peinado = Resources.Load("Peinados/peinadoCorto2") as GameObject;
        Mesh nuevaMesh = peinado.GetComponentInChildren<MeshFilter>().sharedMesh;

        _avatarEditor.MostrarPelo("peinadoCorto2");

        Assert.That(_avatarEditor.pelo.activeSelf, Is.True);
        Assert.That(_avatarEditor.pelo.GetComponent<MeshFilter>().sharedMesh, Is.EqualTo(nuevaMesh));
    }

    //Comprobar que los ojos tienen la base y pupila correctos
    [Test]
    public void TestCambiarOjos () {
        InicializarAvatarEditor();
        Texture texBase = _diccionarioTexturas.GetTextura("ojo1_base");
        Texture pupila = _diccionarioTexturas.GetTextura("pupila1");

        _avatarEditor.MostrarOjos("ojo1_base", "pupila1");

        //Comprobar que las texturas son las correctas
        Assert.That(_avatarEditor.ojos.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex"), Is.EqualTo(texBase));
        Assert.That(_avatarEditor.pupila.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex"), Is.EqualTo(pupila));
    }

    //Comprobar que el color de la piel se cambia correctamente
    [Test]
    public void TestCambiarColorPiel () {
        InicializarAvatarEditor();
        Color color;
        ColorUtility.TryParseHtmlString("#FFD6AE", out color);
        _avatarEditor.CambiarColor(0, color);

        //Comprobar que el color de la piel (mismo material que el de la cabeza) es el mismo
        Assert.That(_avatarEditor.cabeza.GetComponent<Renderer>().sharedMaterial.GetColor("_Color"), Is.EqualTo(color));
    }

    //Comprobar que el color de los ojos se cambia correctamente
    [Test]
    public void TestCambiarColorOjos() {
        InicializarAvatarEditor();
        Color color;
        ColorUtility.TryParseHtmlString("#74D2E4", out color);
        _avatarEditor.CambiarColor(1, color);

        //Comprobar que el color de la piel (mismo material que el de la cabeza) es el mismo
        Assert.That(_avatarEditor.pupila.GetComponent<Renderer>().sharedMaterial.GetColor("_Color"), Is.EqualTo(color));
    }

    //Comprobar que el color de la piel se cambia correctamente
    [Test]
    public void TestCambiarColorPelo() {
        InicializarAvatarEditor();
        Color color;
        ColorUtility.TryParseHtmlString("#573916", out color);
        _avatarEditor.CambiarColor(2, color);

        //Comprobar que el color de la piel (mismo material que el de la cabeza) es el mismo
        Assert.That(_avatarEditor.pelo.GetComponent<Renderer>().sharedMaterial.GetColor("_Color"), Is.EqualTo(color));
    }


    /***** AVATAR FILE MANAGER *****/

    //Comprobar que se cargan los arrays desde el fichero binario
    [Test]
    public void TestCargarFichero () {
        InicializarAvatarFileManager();
        _avatarFileManager.CargarFichero();

        Assert.That(_avatarFileManager.arribaActivos.Length, !Is.EqualTo(0));
        Assert.That(_avatarFileManager.abajoActivos.Length, !Is.EqualTo(0));
        Assert.That(_avatarFileManager.zapatosActivos.Length, !Is.EqualTo(0));
        Assert.That(_avatarFileManager.cuerpoActivos.Length, !Is.EqualTo(0));
        Assert.That(_avatarFileManager.texturas.Length, !Is.EqualTo(0));
        Assert.That(_avatarFileManager.arribaActivos.Length, !Is.EqualTo(0));
    }

    //Comprobar que cada parte del avatar esta o no activa segun se habia guardado
    [Test]
    public void TestCargarActivos () {
        InicializarAvatarFileManager();
        _avatarFileManager.CargarActivos();

        //Comprobar que el activeSelf coincide con el del array de activos
        Assert.That(personaje.transform.GetChild(4).gameObject.activeSelf, Is.EqualTo(_avatarFileManager.arribaActivos[0]));
        Assert.That(personaje.transform.GetChild(5).gameObject.activeSelf, Is.EqualTo(_avatarFileManager.arribaActivos[1]));
        Assert.That(personaje.transform.GetChild(10).gameObject.activeSelf,Is.EqualTo(_avatarFileManager.arribaActivos[2]));
        Assert.That(personaje.transform.GetChild(8).gameObject.activeSelf, Is.EqualTo(_avatarFileManager.abajoActivos[0]));
        Assert.That(personaje.transform.GetChild(9).gameObject.activeSelf, Is.EqualTo(_avatarFileManager.abajoActivos[1]));
        Assert.That(personaje.transform.GetChild(14).gameObject.activeSelf,Is.EqualTo(_avatarFileManager.abajoActivos[2]));
        Assert.That(personaje.transform.GetChild(15).gameObject.activeSelf,Is.EqualTo(_avatarFileManager.abajoActivos[3]));
        Assert.That(personaje.transform.GetChild(18).gameObject.activeSelf,Is.EqualTo(_avatarFileManager.zapatosActivos[0]));
        Assert.That(personaje.transform.GetChild(19).gameObject.activeSelf,Is.EqualTo(_avatarFileManager.zapatosActivos[1]));
        Assert.That(personaje.transform.GetChild(7).gameObject.activeSelf, Is.EqualTo(_avatarFileManager.cuerpoActivos[0]));
        Assert.That(personaje.transform.GetChild(1).gameObject.activeSelf, Is.EqualTo(_avatarFileManager.cuerpoActivos[1]));
        Assert.That(personaje.transform.GetChild(16).gameObject.activeSelf,Is.EqualTo(_avatarFileManager.cuerpoActivos[2]));
        Assert.That(personaje.transform.GetChild(6).gameObject.activeSelf, Is.EqualTo(_avatarFileManager.cuerpoActivos[3]));
        Assert.That(personaje.transform.GetChild(17).gameObject.activeSelf, Is.EqualTo(_avatarFileManager.cuerpoActivos[4])); 
    }

    //Comprobar que las texturas asignadas son las correctas (solo si el objeto esta activo)
    [Test]
    public void TestTexturasAsignadas () {
        InicializarAvatarFileManager();
        _avatarFileManager.CargarTexturas();

        if (personaje.transform.GetChild(4).gameObject.activeSelf == true) 
            Assert.That(personaje.transform.GetChild(4).gameObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex").name, Is.EqualTo(_avatarFileManager.texturas[0]));
        if (personaje.transform.GetChild(5).gameObject.activeSelf == true)
            Assert.That(personaje.transform.GetChild(5).gameObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex").name, Is.EqualTo(_avatarFileManager.texturas[0]));
        if (personaje.transform.GetChild(10).gameObject.activeSelf == true)
            Assert.That(personaje.transform.GetChild(10).gameObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex").name, Is.EqualTo(_avatarFileManager.texturas[0]));
        if (personaje.transform.GetChild(8).gameObject.activeSelf == true)
            Assert.That(personaje.transform.GetChild(8).gameObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex").name, Is.EqualTo(_avatarFileManager.texturas[1]));
        if (personaje.transform.GetChild(9).gameObject.activeSelf == true)                                                       
            Assert.That(personaje.transform.GetChild(9).gameObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex").name, Is.EqualTo(_avatarFileManager.texturas[1]));
        if (personaje.transform.GetChild(14).gameObject.activeSelf == true)
            Assert.That(personaje.transform.GetChild(14).gameObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex").name, Is.EqualTo(_avatarFileManager.texturas[1]));
        if (personaje.transform.GetChild(15).gameObject.activeSelf == true)
            Assert.That(personaje.transform.GetChild(15).gameObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex").name, Is.EqualTo(_avatarFileManager.texturas[1]));
        if (personaje.transform.GetChild(18).gameObject.activeSelf == true)
            Assert.That(personaje.transform.GetChild(18).gameObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex").name, Is.EqualTo(_avatarFileManager.texturas[2]));
        if (personaje.transform.GetChild(19).gameObject.activeSelf == true)
            Assert.That(personaje.transform.GetChild(19).gameObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex").name, Is.EqualTo(_avatarFileManager.texturas[2]));
    }

    /***** METODOS AUXILIARES *****/
    private void InicializarAvatarEditor () {
        //Asignar partes del personaje en el AvatarEditor
        _avatarEditor.brazos = personaje.transform.GetChild(1).gameObject;
        _avatarEditor.cabeza = personaje.transform.GetChild(3).gameObject;
        _avatarEditor.camisa = personaje.transform.GetChild(4).gameObject;
        _avatarEditor.camiseta = personaje.transform.GetChild(5).gameObject;
        _avatarEditor.cintura = personaje.transform.GetChild(6).gameObject;
        _avatarEditor.cuerpo = personaje.transform.GetChild(7).gameObject;
        _avatarEditor.falda1 = personaje.transform.GetChild(8).gameObject;
        _avatarEditor.falda2 = personaje.transform.GetChild(9).gameObject;
        _avatarEditor.jersey = personaje.transform.GetChild(10).gameObject;
        _avatarEditor.ojos = personaje.transform.GetChild(12).gameObject;
        _avatarEditor.pupila = personaje.transform.GetChild(13).gameObject;
        _avatarEditor.corto = personaje.transform.GetChild(14).gameObject;
        _avatarEditor.largo = personaje.transform.GetChild(15).gameObject;
        _avatarEditor.piernas = personaje.transform.GetChild(16).gameObject;
        _avatarEditor.pies = personaje.transform.GetChild(17).gameObject;
        _avatarEditor.altos = personaje.transform.GetChild(18).gameObject;
        _avatarEditor.bajos = personaje.transform.GetChild(19).gameObject;
        _avatarEditor.pelo = GameObject.Find("Pelo");
    }

    private void InicializarAvatarFileManager () {
        _avatarFileManager.arriba = new GameObject[3];
        _avatarFileManager.abajo = new GameObject[4];
        _avatarFileManager.zapatos = new GameObject[2];
        _avatarFileManager.cuerpo = new GameObject[5];

        //Asignar partes del cuerpo en el AvatarFileManager
        _avatarFileManager.arriba[0] = personaje.transform.GetChild(4).gameObject;
        _avatarFileManager.arriba[1] = personaje.transform.GetChild(5).gameObject;
        _avatarFileManager.arriba[2] = personaje.transform.GetChild(10).gameObject;
        _avatarFileManager.abajo[0] = personaje.transform.GetChild(8).gameObject;
        _avatarFileManager.abajo[1] = personaje.transform.GetChild(9).gameObject;
        _avatarFileManager.abajo[2] = personaje.transform.GetChild(14).gameObject;
        _avatarFileManager.abajo[3] = personaje.transform.GetChild(15).gameObject;
        _avatarFileManager.zapatos[0] = personaje.transform.GetChild(18).gameObject;
        _avatarFileManager.zapatos[1] = personaje.transform.GetChild(19).gameObject;
        _avatarFileManager.cuerpo[0] = personaje.transform.GetChild(7).gameObject;
        _avatarFileManager.cuerpo[1] = personaje.transform.GetChild(1).gameObject;
        _avatarFileManager.cuerpo[2] = personaje.transform.GetChild(16).gameObject;
        _avatarFileManager.cuerpo[3] = personaje.transform.GetChild(6).gameObject;
        _avatarFileManager.cuerpo[4] = personaje.transform.GetChild(17).gameObject;
        _avatarFileManager.pelo = GameObject.Find("Pelo");
        _avatarFileManager.ojo = personaje.transform.GetChild(12).gameObject;
        _avatarFileManager.pupila = personaje.transform.GetChild(13).gameObject;
        _avatarFileManager.meshPelo = "peinadoCorto2";
        //Asignar texturas y colores por defecto (por si no existiera aun el fichero)
        _avatarFileManager.texturas[0] = "camisetaAzul";
        _avatarFileManager.texturas[1] = "shortsRosas";
        _avatarFileManager.texturas[2] = "zapatillas";
        _avatarFileManager.texturas[3] = "ojo1_base";
        _avatarFileManager.texturas[4] = "pupila1";
        _avatarFileManager.colores[0] = "#FFD6AE";
        _avatarFileManager.colores[1] = "#74D2E4";
        _avatarFileManager.colores[2] = "#573916";

        //Esto es necesario por si se ejecuta el test antes de ejecutar la app por primera vez en el editor
        // y el binario en el PersistentDataPath aun no ha sido creado (se crea al iniciar la aplicacion por primera vez)
        _avatarFileManager.Awake();
    }
}