/*Esta clase se encarga de inicializar el minijuego de las naves, creando los arquetipos y entidades
 * del jugador, de las naves hijas y de las naves enemigas.
 * Tambien contiene una serie de metodos para gestionar y mostrar por pantalla los datos del juego 
 * (puntos y monedas), reproducir sonidos y mostrar el fin de la partida.
 */

using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using UnityEngine.UI;

namespace Naves {

    public class Bootstrap_NV : MonoBehaviour {

        //Meshes y materiales 
        /*Jugador*/
        public Mesh quad;
        public Material playerMaterial;
        //Proyectil jugador
        public Material proyectilJugadorMaterial;
        /*Naves hijas*/
        public Material naveHijaMaterial;
        /*Nave enemiga (0) y monedas (1) */
        public Material[] materiales;
        
        //Auxiliares
        public static Bootstrap_NV bootstrap_NV {
            private set;
            get;
        }
        private static Mesh meshStatic;
        private static Material materialStatic;

        public GameObject deathParticlesPrefab;

        //ECS
        private static EntityManager entityManager;
        private NativeArray<Entity> arrayEntidades;

        //Datos del juego y UI
        private int n_puntos;
        private int n_puntosAntiguos;
        private static Text puntosTxt;
        private int n_monedas;
        private int n_monedasAntiguo;
        private static Text monedasTxt;
        private static Transform canvas;

        //AUDIO
        private static AudioSource audioSourceFX;
        public AudioClip[] sonidos;

        public GameObject fondo;

        private void Awake() {
            //Comprobar si ya hay algun otro objeto con el script Bootstrap
            Bootstrap_NV[] instancias = FindObjectsOfType<Bootstrap_NV>();
            if (instancias.Length > 1) {
                Destroy(gameObject);
            }
            else bootstrap_NV = this;

            //Elementos UI
            puntosTxt = GameObject.Find("Puntos").GetComponent<Text>();
            monedasTxt = GameObject.Find("Monedas").GetComponent<Text>();
            canvas = GameObject.Find("Canvas").transform;

            audioSourceFX = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public void Start() {
            
            entityManager = World.Active.GetOrCreateManager<EntityManager>();

            /***** ENTIDAD JUGADOR *****/
            var playerArchetype = entityManager.CreateArchetype(
                typeof(PlayerInputCMP_NV),
                typeof(Position),
                typeof(Rotation),
                typeof(Scale),
                typeof(MeshInstanceRenderer)
            );

            var player = entityManager.CreateEntity(playerArchetype);

            entityManager.SetComponentData(player, new PlayerInputCMP_NV {
                //vida = 4.0f
            });
            entityManager.SetSharedComponentData(player, new MeshInstanceRenderer {
                mesh = quad,
                material = playerMaterial
            });
            entityManager.SetComponentData(player, new Position {
                Value = new Unity.Mathematics.float3(0f, 0.5f, 0f)
            });
            entityManager.SetComponentData(player, new Scale {
                Value = new Unity.Mathematics.float3(2.2f, 2.2f, 1f)
            });

            /***** ENTIDAD PROYECTIL JUGADOR *****/
            var proyectilJugadorchetype = entityManager.CreateArchetype(
                typeof(ProyectilJugadorCMP_NV),
                typeof(Position),
                typeof(Rotation),
                typeof(Scale),               
                typeof(MeshInstanceRenderer)
            );

            var proyectilJugador = entityManager.CreateEntity(proyectilJugadorchetype);

            entityManager.SetComponentData(proyectilJugador, new ProyectilJugadorCMP_NV {
                escalaYOriginal = 0.6f,
                posOriginal = new Unity.Mathematics.float3(0f, -0.65f, 0f)
            });
            entityManager.SetSharedComponentData(proyectilJugador, new MeshInstanceRenderer {
                mesh = quad,
                material = proyectilJugadorMaterial
            });
            entityManager.SetComponentData(proyectilJugador, new Position {
                Value = new Unity.Mathematics.float3(0f, -0.65f, 0f)
            });
            entityManager.SetComponentData(proyectilJugador, new Scale {
                Value = new Unity.Mathematics.float3(0.3f, 0.6f, 1f)
            });

            //Emparentar proyectil a nave jugador, se utiliza una entidad auxiliar
            var attach = entityManager.CreateEntity(typeof(Attach));
            entityManager.SetComponentData(attach, new Attach {
                Parent = player,
                Child = proyectilJugador
            });

            /***** ENTIDADES NAVES HIJAS *****/
            var naveHijaArchetype = entityManager.CreateArchetype(
                typeof(NaveHijaCMP_NV),
                typeof(Position),
                typeof(Rotation),
                typeof(Scale),
                typeof(MeshInstanceRenderer)
            );

            //1: Arriba izquierda
            var naveHija1 = entityManager.CreateEntity(naveHijaArchetype);
            entityManager.SetComponentData(naveHija1, new NaveHijaCMP_NV {
                posicion = new Unity.Mathematics.float3(-1.8f, 1.48f, 0f),
                direccion = 1.0f
            });
            entityManager.SetSharedComponentData(naveHija1, new MeshInstanceRenderer {
                mesh = quad,
                material = naveHijaMaterial
            });
            entityManager.SetComponentData(naveHija1, new Scale {
                Value = new Unity.Mathematics.float3(1.1f, 1.1f, 1f)
            });
            entityManager.SetComponentData(naveHija1, new Position {
                Value = new Unity.Mathematics.float3(-1.8f, 1.48f, 0f)
            });

            //2: Arriba derecha
            var naveHija2 = entityManager.CreateEntity(naveHijaArchetype);
            entityManager.SetComponentData(naveHija2, new NaveHijaCMP_NV {
                posicion = new Unity.Mathematics.float3(1.8f, 1.48f, 0f),
                direccion = -1.0f
            });
            entityManager.SetSharedComponentData(naveHija2, new MeshInstanceRenderer {
                mesh = quad,
                material = naveHijaMaterial
            });
            entityManager.SetComponentData(naveHija2, new Scale {
                Value = new Unity.Mathematics.float3(1.1f, 1.1f, 1f)
            });
            entityManager.SetComponentData(naveHija2, new Position {
                Value = new Unity.Mathematics.float3(1.8f, 1.48f, 0f)
            });

            //3: Abajo izquierda
            var naveHija3 = entityManager.CreateEntity(naveHijaArchetype);
            entityManager.SetComponentData(naveHija3, new NaveHijaCMP_NV {
                posicion = new Unity.Mathematics.float3(-1.8f, -0.56f, 0f),
                direccion = -1.0f
            });
            entityManager.SetSharedComponentData(naveHija3, new MeshInstanceRenderer {
                mesh = quad,
                material = naveHijaMaterial
            });
            entityManager.SetComponentData(naveHija3, new Scale {
                Value = new Unity.Mathematics.float3(1.1f, 1.1f, 1f)
            });
            entityManager.SetComponentData(naveHija3, new Position {
                Value = new Unity.Mathematics.float3(-1.8f, -0.56f, 0f)
            });

            //4: Abajo derecha
            var naveHija4 = entityManager.CreateEntity(naveHijaArchetype);
            entityManager.SetComponentData(naveHija4, new NaveHijaCMP_NV {
                posicion = new Unity.Mathematics.float3(1.8f, -0.56f, 0f),
                direccion = 1.0f
            });
            entityManager.SetSharedComponentData(naveHija4, new MeshInstanceRenderer {
                mesh = quad,
                material = naveHijaMaterial
            });
            entityManager.SetComponentData(naveHija4, new Scale {
                Value = new Unity.Mathematics.float3(1.1f, 1.1f, 1f)
            });
            entityManager.SetComponentData(naveHija4, new Position {
                Value = new Unity.Mathematics.float3(1.8f, -0.56f, 0f)
            });


            /***** ENTIDAD NAVE ENEMIGA *****/
            //var enemigoArchetype = entityManager.CreateArchetype(typeof(EnemigoCMP_NV));
            //CrearEntidadesBase(50, enemigoArchetype);           


            /***** ENTIDAD MONEDA *****/
            /*Pooling de objetos: En lugar de crear y destruir los objetos (entidades) continuamente,
             * se crea inicialmente una cantidad determinada de objetos "desactivados"
             * (en este caso, sin componentes de renderizado, posicion, etc), para ir activandolos
             * cuando sea necesario y desactivandolos (sin destruirlos) cuando no tengan que aparecer en la escena */
            var premioArchetype = entityManager.CreateArchetype(
                typeof(PremioCMP_NV)
            );
            CrearEntidadesBase(30, premioArchetype);

            //Inicializar spawner de enemigos (y premios)
            ActivarEnemigosPremiosSYS_NV.IniciarSpawner(entityManager);                       

            /***** ENTIDAD TEMPORIZADOR *****/
            //Se encarga de actualizar el tiempo total que esta durando la partida y de actualizar la UI        
            var temporizadorArchetype = entityManager.CreateArchetype(
                typeof(TemporizadorCMP_NV)
            );
            var temporizador = entityManager.CreateEntity(temporizadorArchetype);
            entityManager.SetComponentData(temporizador, new TemporizadorCMP_NV {
                startTime = Time.time
            });         

        }

        //Actualiza el numero de monedas y puntos conseguidos si este ha aumentado
        // (no se puede hacer desde el job que detecta las colisiones porque
        // los metodos GameObject.Find() y ToString solo se pueden utilizar desde el main thread)
        private void Update() {
            if (n_puntos != n_puntosAntiguos) {
                puntosTxt.text = n_puntos.ToString();
                n_puntosAntiguos = n_puntos;
            }

            if (n_monedas != n_monedasAntiguo) {
                monedasTxt.text = n_monedas.ToString();
                n_monedasAntiguo = n_monedas;
            }
        }

        //Metodo para crear tantas entidades como se indiquen, a partir del arquetipo recibido
        private void CrearEntidadesBase(int cantidad, EntityArchetype archetype) {
            //Rellenar el array con entidades del arquetipo que se recibe
            arrayEntidades = new NativeArray<Entity>(cantidad, Allocator.Temp);
            entityManager.CreateEntity(archetype, arrayEntidades); //Crea tantas entidades como capacidad tenga el array        

            arrayEntidades.Dispose();
        }

        public Mesh GetMesh() {
            meshStatic = bootstrap_NV.quad;
            return meshStatic;
        }

        public Material GetMaterial(int i) {
            materialStatic = bootstrap_NV.materiales[i];
            return materialStatic;
        }

        public void Explosion (Vector3 posicion) {
            Instantiate(bootstrap_NV.deathParticlesPrefab, posicion, Quaternion.identity);
        }

        //Al terminar el juego, el FinJuegoSystem llama a este metodo para mostrar por pantalla
        // un pop-up con toda la informacion de la partida
        public void FinJuego() {
            /*NativeArray<Entity> allEntities = entityManager.GetAllEntities();
            entityManager.DestroyEntity(allEntities);
            allEntities.Dispose();*/

            //Detener scroll fondo
            bootstrap_NV.fondo.GetComponent<ScrollFondo>().enabled = false;           

            var PopUpJuego = Instantiate(Resources.Load("Prefabs/PopUpJuego") as GameObject, canvas);

            var PPPanel = PopUpJuego.transform.Find("PanelPopUp");
            PPPanel.transform.Find("TituloPopUp").GetComponent<Text>().text = "¡Fin del juego!";

            //Info del popup
            PPPanel.transform.Find("MonedasPopUp").GetComponent<Text>().text = bootstrap_NV.n_monedas.ToString();
            PPPanel.transform.Find("TiempoPopUp").GetComponent<Text>().text = bootstrap_NV.n_puntos.ToString();
            int puntosTotales = (int)bootstrap_NV.n_puntos * 2 + bootstrap_NV.n_monedas * 5;
            PPPanel.transform.Find("PuntosPopUp").GetComponent<Text>().text = "Total: " + puntosTotales;

            //Record
            int recordActual = PlayerPrefs.GetInt("RecordNaves", 0);
            if (puntosTotales > recordActual) {
                PlayerPrefs.SetInt("RecordNaves", puntosTotales);
                PPPanel.transform.Find("RecordPopUp").GetComponent<Text>().text = "¡Nuevo récord!";

                //Al superar el record, comprobar si tambien se ha alcanzado el logro
                //Guardar nuevo numero de puntos
                PlayerPrefs.SetFloat("PuntosSlinger", puntosTotales);
                //Comprobar si se ha completado el logro slinger (id = 6)
                LogrosManager.ComprobarLogroCompletado(6);
            }
            else {
                PPPanel.transform.Find("RecordPopUp").GetComponent<Text>().text = "Record: " + recordActual;
            }

            //Sumar las monedas obtenidas
            int monedasActuales = PlayerPrefs.GetInt("Monedas");
            PlayerPrefs.SetInt("Monedas", monedasActuales + bootstrap_NV.n_monedas); 
        }

        //Metodos auxiliares para sumar y obtener el numero de monedas
        public int GetNumMonedas() {
            return bootstrap_NV.n_monedas;
        }

        public void SumarMonedas(int cantidad) {
            bootstrap_NV.n_monedas += cantidad;
        }

        //Metodo para sumar los puntos cuando se elimina un enemigo
        public void SumarPuntos(int cantidad) {
            bootstrap_NV.n_puntos += cantidad;
        }

        //AUDIO
        //Parar musica de fondo
        public void StopMusic() {
            GameObject.Find("Musica").GetComponent<AudioSource>().Stop();
        }

        public void PlaySound(int i) {
            audioSourceFX.PlayOneShot(bootstrap_NV.sonidos[i], 1.5f);
        }
        //Se llama a este metodo para dar por terminado el juego cuando el jugador pulsa el boton de cancelar/salir
        public void SalirJuego() {
            ComponentGroup m_NavesHijas = entityManager.CreateComponentGroup(typeof(NaveHijaCMP_NV));
            entityManager.DestroyEntity(m_NavesHijas);
        }

        public void OnDestroy() {
            Destroy(GameObject.Find("PopUpJuego(Clone)"));
        }        
    }    
}

