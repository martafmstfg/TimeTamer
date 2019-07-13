/* Sistema que activa los obstaculos y los premios cada cierto tiempo (tiempo de spawn).
 * Activar uno de estos elementos implica agregar a su entidad el los componentes necesarios
 * para que aparezca en pantalla y se mueva: meshinstancerenderer y position*/

using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class ActivarObstaculosPremiosSystem : ComponentSystem {

    ComponentGroup m_Obstaculos;
    ComponentGroup m_Premios;
    private float3 posiciones; //Los 3 carriles en los que pueden aparecer

    ComponentGroup m_TiempoSpawn;

    ComponentGroup m_Temporizador;

    //Activa uno de los obstaculos que estan inactivos (aleatorio)
    private struct ActivarObstaculo : IJob {

        public EntityArray obstaculosInactivos;
        public EntityCommandBuffer Commands;
        [ReadOnly]  public int randomId;
        private Entity obstaculoActivar;
        public float posicionX;
        [ReadOnly] public int idObstaculo; //Indice del tipo de obstaculo para obtener su mesh y material

        private int z;

        public void Execute() {
            //Activar una de las entidades aleatoria
            obstaculoActivar = obstaculosInactivos[randomId];

            //Annadirle componentes obstaculoactivo, meshrenderer y posicion (uno de los carriles aleatorio)
            Commands.AddComponent<Position>(obstaculoActivar, new Position {
                Value = new float3(posicionX, 6.0f, 0.0f)
            });            

            //Si se han activado los palillos en el carril del extremo derecho, se voltean
            if(idObstaculo == 2) {    
                Commands.AddComponent<PalillosComponent>(obstaculoActivar, new PalillosComponent());

                if (posicionX == 1.53f) z = -180;
                else z = 0;

                Commands.AddComponent<Rotation>(obstaculoActivar, new Rotation {
                    Value = Quaternion.Euler(0, 0, -z)
                });
            }

            Commands.AddSharedComponent<MeshInstanceRenderer>(obstaculoActivar, new MeshInstanceRenderer {
                mesh = Bootstrap.bootstrap.GetMesh(idObstaculo),
                material = Bootstrap.bootstrap.GetMaterial(idObstaculo)
            });
            Commands.AddComponent<ObstaculoActivoComponent>(obstaculoActivar, new ObstaculoActivoComponent());
        }
    }

    //Activa uno de los obstaculos que estan inactivos (aleatorio)
    private struct ActivarPremio : IJob {

        public EntityArray premiosInactivos;
        public EntityCommandBuffer Commands;
        public int randomId;
        private Entity premioActivar;
        public float posicionX;

        public void Execute() {

            //Activar una de las entidades aleatoria      
            premioActivar = premiosInactivos[randomId];

            //Annadirle componentes premioactivo, meshrenderer y posicion (uno de los carriles aleatorio)
            Commands.AddComponent<Position>(premioActivar, new Position {
                Value = new float3(posicionX, 6.0f, 0.0f) 
            });            
            Commands.AddSharedComponent<MeshInstanceRenderer>(premioActivar, new MeshInstanceRenderer {
                mesh = Bootstrap.bootstrap.GetMesh(3),
                material = Bootstrap.bootstrap.GetMaterial(3)
            });
            Commands.AddComponent<PremioActivoComponent>(premioActivar, new PremioActivoComponent());
        }
    }

    protected override void OnCreateManager() {
        //Obtener grupos que contengan los componentes indicados
        m_Obstaculos = GetComponentGroup(typeof(ObstaculoComponent), ComponentType.Subtractive<ObstaculoActivoComponent>());
        m_Premios = GetComponentGroup(typeof(PremioComponent), ComponentType.Subtractive<PremioActivoComponent>());
        
        m_TiempoSpawn = GetComponentGroup(typeof(TiempoSpawnComponent));

        m_Temporizador = GetComponentGroup(typeof(TemporizadorComponent));

        posiciones = new float3(-1.53f, 1.53f, 0); //posicion x de los carriles
    }

    protected override void OnUpdate () {
                
        //Componente que contiene los tiempos de spawn de cada elemento
        TiempoSpawnComponent componente = m_TiempoSpawn.GetComponentDataArray<TiempoSpawnComponent>()[0];

        /****** SPAWN OBSTACULOS *******/

        //Comprobar si se ha agotado el tiempo de espera del spawner de obstaculos
        float tiempoObstaculos = componente.segundosObstaculos;
        tiempoObstaculos = Mathf.Max(0.0f, componente.segundosObstaculos - Time.deltaTime);
        bool spawnObstaculo = tiempoObstaculos <= 0.0f;

        //Resetear el tiempo de espera si se ha agotado
        if (spawnObstaculo) {            
            //Calcular cuanto hay que reducir tiempo de spawn en funcion del tiempo de juego
            // (reducir el tiempo de spawn al aumentar la velocidad de movimiento)
            TemporizadorComponent temporizadorComponent = m_Temporizador.GetComponentDataArray<TemporizadorComponent>()[0];
            float tiempoTotal = Time.time - temporizadorComponent.startTime; //tiempo que ha pasado desde el inicio del juego
            int t = (int)(tiempoTotal / 20); //cada 20s, la velocidad aumenta y el tiempo de spawn disminuye

            tiempoObstaculos = Mathf.Max(0.5f, 1.2f - 0.15f*t); //reducir 0.15s cada 20s, hasta un minimo de 0.5s
        }
                
        //Array de obstaculos que estan inactivos
        EntityArray obstaculosInactivos = m_Obstaculos.GetEntityArray();

        //Spawnear obstaculo si el tiempo de espera ha llegado a 0 y si quedan obstaculos por activar
        // (se supone que siempre van a quedar obstaculos por activar, puesto que da tiempo a que alguno se desactive)
        if (spawnObstaculo && obstaculosInactivos.Length > 0) {

            //Escoger tipo de obstaculo aleatoriamente
            int idObstaculo = UnityEngine.Random.Range(0, 3);
            //Escoger posicion aleatoriamente
            float posicionX ;
            if (idObstaculo == 2) {
                //Los palillos solo pueden aparecer en los extremos
                posicionX = posiciones[UnityEngine.Random.Range(0, 2)];
            } else {
                posicionX = posiciones[UnityEngine.Random.Range(0, 3)];
            }            

            JobHandle jobActivar = new ActivarObstaculo() {
                obstaculosInactivos = obstaculosInactivos,
                Commands = _collisionBarrier.CreateCommandBuffer(),
                randomId = UnityEngine.Random.Range(0, obstaculosInactivos.Length),
                posicionX = posicionX,
                idObstaculo = idObstaculo

            }.Schedule();
            jobActivar.Complete();  
        }
        

        /************ SPAWN PREMIOS *******/

        //Comprobar si se ha agotado el tiempo de espera del spawner de premios
        float tiempoPremios = componente.segundosPremios;
        tiempoPremios = Mathf.Max(0.0f, componente.segundosPremios - Time.deltaTime);
        bool spawnPremios = tiempoPremios <= 0.0f;

        if (spawnPremios) {
            //Resetear
            tiempoPremios = 7.0f; //7 segundos entre premios (no disminuye con la velocidad)
        }

        //Array de premios que estan inactivos
        EntityArray premiosInactivos = m_Premios.GetEntityArray();

        //Spawnear premio si el tiempo de espera ha llegado a 0 y si quedan premios por activar
        if (spawnPremios && premiosInactivos.Length > 0) {
            
            //Escoger posicion aleatoriamente
            float posicionX = posiciones[UnityEngine.Random.Range(0, 3)];

            JobHandle jobActivar = new ActivarPremio() {
                premiosInactivos = premiosInactivos,
                Commands = _collisionBarrier.CreateCommandBuffer(),
                randomId = UnityEngine.Random.Range(0, premiosInactivos.Length),
                posicionX = posicionX

            }.Schedule();
            jobActivar.Complete(); 
        }

        //Actualizar el tiempo que queda en el componente 
        Entity ent = m_TiempoSpawn.GetEntityArray()[0];
        EntityManager.SetComponentData<TiempoSpawnComponent>(ent, new TiempoSpawnComponent {
            segundosObstaculos = tiempoObstaculos,
            segundosPremios = tiempoPremios
        });        
    }

    //Metodo auxiliar utilizado por Bootstrap para inicializar la entidad de spawn
    public static void IniciarSpawner (EntityManager entityManager) {
        var arc = entityManager.CreateArchetype(typeof(TiempoSpawnComponent));
        var ent = entityManager.CreateEntity(arc);
        
        entityManager.SetComponentData(ent, new TiempoSpawnComponent {
            segundosObstaculos = 0.0f,
            segundosPremios = 7.0f,
        });
    }
       

    private class CollisionBarrier : BarrierSystem {
    }
    [Inject] private CollisionBarrier _collisionBarrier;
}
