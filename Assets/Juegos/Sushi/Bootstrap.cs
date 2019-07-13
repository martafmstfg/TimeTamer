/*Esta clase se encarga de inicializar el minijuego del sushi, creando los arquetipos y entidades
 * del jugador, de los distintos obstaculos y de las monedas.
 * Tambien contiene una serie de metodos para gestionar y mostrar por pantalla los datos del juego 
 * (tiempo, puntos y monedas), reproducir sonidos y mostrar el fin de la partida.
 */

using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour {
        
    //Meshes y materiales 
    /*Jugador*/
    public Mesh playerMesh;    
    public Material playerMaterial;
    /*Obstaculos*/
    public Material[] materiales;
    public Mesh[] meshes; 
    /*Manos*/
    public Mesh manoMesh;
    public Material manoMaterial;

    //Auxiliares
    public static Bootstrap bootstrap {
        private set;
        get;
    } 
    private static Mesh meshStatic;
    private static Material materialStatic;

    //Datos del juego y UI
    private int n_monedas;
    private int n_monedasAntiguo;
    private static Text monedasTxt;
    private float tiempoTotal;
    private static Text tiempoTxt;
    private static Transform canvas;
  
    //ECS
    private static EntityManager entityManager;
    private NativeArray<Entity> entidadesObstaculos;

    //AUDIO
    private static AudioSource audioSourceFX;
    public AudioClip[] sonidos;

    public void Awake() {

        //Comprobar si ya hay algun otro objeto con el script Bootstrap
        Bootstrap[] instancias = FindObjectsOfType<Bootstrap>();
        if (instancias.Length > 1) {
            Destroy(gameObject);
        }
        else bootstrap = this;

        //Elementos UI
        monedasTxt = GameObject.Find("Monedas").GetComponent<Text>();
        tiempoTxt = GameObject.Find("Tiempo").GetComponent<Text>();
        canvas = GameObject.Find("Canvas").transform;

        audioSourceFX = GameObject.Find("Audio Source").GetComponent<AudioSource>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public void Start () {

        entityManager = World.Active.GetOrCreateManager<EntityManager> ();
        
        /***** ENTIDAD JUGADOR *****/       
        var playerArchetype = entityManager.CreateArchetype (
            typeof(Position),
            typeof(MeshInstanceRenderer),
            typeof(Scale),
            typeof(PlayerInputComponent)
        );

        var player = entityManager.CreateEntity (playerArchetype);

        entityManager.SetSharedComponentData(player, new MeshInstanceRenderer {
            mesh = playerMesh,
            material = playerMaterial
        });
        entityManager.SetComponentData(player, new Position {
            Value = new Unity.Mathematics.float3(0f, -2.8f, 0f)
        });
        entityManager.SetComponentData(player, new Scale {
            Value = new Unity.Mathematics.float3(1f, 1f, 1f)
        });        
        entityManager.SetComponentData(player, new PlayerInputComponent {
            posicionFinal = new Unity.Mathematics.float3(0f, -2.8f, 0f)
        });


        /***** ENTIDADES OBSTACULOS Y PREMIOS *****/
        /*Pooling de objetos: En lugar de crear y destruir los objetos (entidades) continuamente,
         * se crea inicialmente una cantidad determinada de objetos "desactivados"
         * (en este caso, sin componentes de renderizado, posicion, etc), para ir activandolos
         * cuando sea necesario y desactivandolos (sin destruirlos) cuando no tengan que aparecer en la escena */

        var obstaculoArchetype = entityManager.CreateArchetype (            
            typeof(ObstaculoComponent)
        );
        CrearEntidadesBase(30, obstaculoArchetype);

        var premioArchetype = entityManager.CreateArchetype(
            typeof(PremioComponent)
        );
        CrearEntidadesBase(20, premioArchetype);

        //Inicializar spawner de obstaculos y premios
        ActivarObstaculosPremiosSystem.IniciarSpawner(entityManager);


        /***** ENTIDADES MANOS *****/
        var manoArchetype = entityManager.CreateArchetype(
            typeof(ManoComponent),
            typeof(ManoActivaComponent),
            typeof(Position),
            typeof(Scale),
            typeof(Rotation),
            typeof(MeshInstanceRenderer) 
        );

        //Derecha
        var manoDch = entityManager.CreateEntity(manoArchetype);

        entityManager.SetComponentData(manoDch, new ManoComponent {
            direccionX = 1,
            posicionInicial = new Unity.Mathematics.float3(4.8f, -2.8f, 0f),
            posicionFinal = new Unity.Mathematics.float3(2.5f, -2.8f, 0f),
            posicionBorde = new Unity.Mathematics.float3(3.8f, -2.8f, 0f)
        });
        entityManager.SetComponentData(manoDch, new ManoActivaComponent {
            activa = 0            
        });
        entityManager.SetComponentData(manoDch, new Position {
            Value = new Unity.Mathematics.float3(4.8f, -2.8f, 0f)
        });
        entityManager.SetComponentData(manoDch, new Rotation {
            Value = Quaternion.Euler(0, 0, 90)
        });
        entityManager.SetComponentData(manoDch, new Scale {
            Value = new Unity.Mathematics.float3(0.14f)
        });
        entityManager.SetSharedComponentData(manoDch, new MeshInstanceRenderer {
            mesh = manoMesh,
            material = manoMaterial
        });

        //Izquierda
        var manoIzq = entityManager.CreateEntity(manoArchetype);

        entityManager.SetComponentData(manoIzq, new ManoComponent {
            direccionX = 1,
            posicionInicial = new Unity.Mathematics.float3(-4.8f, -2.8f, 0f),
            posicionFinal = new Unity.Mathematics.float3(-2.5f, -2.8f, 0f),
            posicionBorde = new Unity.Mathematics.float3(-3.8f, -2.8f, 0f)
        });
        entityManager.SetComponentData(manoIzq, new ManoActivaComponent {
            activa = 0
        });
        entityManager.SetComponentData(manoIzq, new Position {
            Value = new Unity.Mathematics.float3(-4.8f, -2.8f, 0f)
        });
        entityManager.SetComponentData(manoIzq, new Rotation {
            Value = Quaternion.Euler(0, 0, -90)
        });
        entityManager.SetComponentData(manoIzq, new Scale {
            Value = new Unity.Mathematics.float3(0.14f)
        });
        entityManager.SetSharedComponentData(manoIzq, new MeshInstanceRenderer {
            mesh = manoMesh,
            material = manoMaterial
        });

        //Inicializar sistema manos
        MoverManoSystem.IniciarManos(entityManager);

        /***** ENTIDAD TEMPORIZADOR *****/
        //Se encarga de actualizar el tiempo total que esta durando la partida y de actualizar la UI        
        var temporizadorArchetype = entityManager.CreateArchetype(
            typeof(TemporizadorComponent)
        );
        var temporizador = entityManager.CreateEntity(temporizadorArchetype);
        entityManager.SetComponentData(temporizador, new TemporizadorComponent {
            startTime = Time.time
        });          
    }

    //Actualiza el numero de monedas conseguidas si este ha aumentado
    // (no se puede hacer desde el job que detecta las colisiones con las monedas porque
    // los metodos GameObject.Find() y ToString solo se pueden utilizar desde el main thread)
    private void Update() {
        if(n_monedas != n_monedasAntiguo) {
            monedasTxt.text = n_monedas.ToString();
            n_monedasAntiguo = n_monedas;
        }
    }

    //Metodo para crear tantas entidades como se indiquen, a partir del arquetipo recibido
    private void CrearEntidadesBase (int cantidad, EntityArchetype archetype) {
        //Rellenar el array con entidades del arquetipo que se recibe
        entidadesObstaculos = new NativeArray<Entity>(cantidad, Allocator.Temp);
        entityManager.CreateEntity(archetype, entidadesObstaculos); //Crea tantas entidades como capacidad tenga el array        

        entidadesObstaculos.Dispose();        
    }

    public Mesh GetMesh (int i) {
        meshStatic = bootstrap.meshes[i];
        return meshStatic;
    }

    public Material GetMaterial(int i) {
        materialStatic = bootstrap.materiales[i];
        return materialStatic;
    }

    //Al terminar el juego, el FinJuegoSystem llama a este metodo para mostrar por pantalla
    // un pop-up con toda la informacion de la partida
    public void FinJuego () {     
       
        var PopUpJuego = Instantiate(Resources.Load("Prefabs/PopUpJuego") as GameObject, canvas);

        var PPPanel = PopUpJuego.transform.Find("PanelPopUp");
        PPPanel.transform.Find("TituloPopUp").GetComponent<Text>().text = "¡Fin del juego!";

        //Info del popup
        PPPanel.transform.Find("MonedasPopUp").GetComponent<Text>().text = bootstrap.n_monedas.ToString();
        PPPanel.transform.Find("TiempoPopUp").GetComponent<Text>().text = ObtenerTiempoTotal();
        int puntosTotales = (int)bootstrap.tiempoTotal * 5 + bootstrap.n_monedas * 10;
        PPPanel.transform.Find("PuntosPopUp").GetComponent<Text>().text = "Puntos: " + puntosTotales;

        //Comprobar si se ha superado el record
        int recordActual = PlayerPrefs.GetInt("RecordSushi", 0);
        if (puntosTotales > recordActual) {
            PlayerPrefs.SetInt("RecordSushi", puntosTotales);
            PPPanel.transform.Find("RecordPopUp").GetComponent<Text>().text = "¡Nuevo récord!";

            //Al superar el record, comprobar si tambien se ha alcanzado el logro
            //Guardar nuevo numero de puntos
            PlayerPrefs.SetFloat("PuntosKaitenzushi", puntosTotales);
            //Comprobar si se ha completado el logro kaitenzushi (id = 5)
            LogrosManager.ComprobarLogroCompletado(5);

        } else {
            PPPanel.transform.Find("RecordPopUp").GetComponent<Text>().text = "Record: " + recordActual;
        }

        //Sumar las monedas obtenidas
        int monedasActuales = PlayerPrefs.GetInt("Monedas");
        PlayerPrefs.SetInt("Monedas", monedasActuales + bootstrap.n_monedas); 
    }

    //Metodos auxiliares para sumar y obtener el numero de monedas
    public int GetNumMonedas() {
        return bootstrap.n_monedas;
    }

    public void SumarMonedas(int cantidad) {
        bootstrap.n_monedas += cantidad;
    }

    //Al terminar el juego, el FinJuegoSystem llama a este metodo para guardar el tiempo total
    public void GuardarTiempoTotal(float tiempo) {
        bootstrap.tiempoTotal = tiempo;
    }

    //Devuelve el tiempo total de la partida en formato MM:SS
    private string ObtenerTiempoTotal () {    

        int min = ((int)bootstrap.tiempoTotal / 60);
        string minutos;
        if (min < 10) {
            minutos = "0" + min.ToString();
        }
        else {
            minutos = min.ToString();
        }

        int sec = ((int)bootstrap.tiempoTotal % 60);
        string segundos;
        if (sec < 10) {
            segundos = "0" + sec.ToString();
        }
        else {
            segundos = sec.ToString();
        }

        return minutos + ":" + segundos;
    }
    
    //Se llama a este metodo para dar por terminado el juego cuando el jugador pulsa el boton de cancelar/salir
    public void SalirJuego () {
        var finJuegoArchetype = entityManager.CreateArchetype(typeof(FinJuegoComponent));
        entityManager.CreateEntity(finJuegoArchetype);
    }

    //AUDIO
    //Parar musica de fondo
    public void StopMusic() {
        GameObject.Find("Musica").GetComponent<AudioSource>().Stop();
    }

    public void PlaySound(int i) {
        audioSourceFX.PlayOneShot(bootstrap.sonidos[i], 1.5f);
    }

    public void OnDestroy() {
        Destroy(GameObject.Find("PopUpJuego(Clone)"));
    }      


}

