/* Sistema que activa / crea un premio o un enemigo cada cierto tiempo (tiempo de spawn).
 * Para ello añade a las entidades los componentes necesarios para que aparezcan en pantalla,
 * se muevan y se consideren activos.
 */


using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;

namespace Naves {

    [UpdateAfter(typeof(ColisionProyectilSYS_NV))]
    [UpdateAfter(typeof(ColisionNaveHijaSYS_NV))]
    public class ActivarEnemigosPremiosSYS_NV : ComponentSystem {

        //ComponentGroup m_Enemigos; //Enemigos no activos
        ComponentGroup m_TiempoSpawn;
        ComponentGroup m_EnemigosActivos;
        ComponentGroup m_NavesHijas; //Para escoger el target de los enemigos
        ComponentGroup m_Premios;

        ComponentGroup m_Temporizador; //Para aumentar velocidad de spawn con el tiempo
        TemporizadorCMP_NV temporizadorComponent;

        float posicionEnemigoX, posicionEnemigoY, posicionPremioX, posicionPremioY;
        Vector2 posicionesEnemigoY, posicionesPremioY;

        EntityArchetype enemigoArchetype;        

        //Activa uno de los premios que estan inactivos (aleatorio)
        private struct ActivarPremio : IJob {

            public EntityArray premiosInactivos;
            public EntityCommandBuffer Commands;
            public int randomId;
            private Entity premioActivar;
            public float posicionX, posicionY;

            public void Execute() {

                //Activar una de las entidades aleatoria      
                premioActivar = premiosInactivos[randomId];

                //Annadirle componentes premioactivo, meshrenderer, escala y posicion (aleatoria)
                Commands.AddComponent<Position>(premioActivar, new Position {
                    Value = new float3(posicionX, posicionY, 0.0f)
                });
                Commands.AddComponent<Scale>(premioActivar, new Scale {
                    Value = new float3(0.7f, 0.7f, 1f)
                });
                Commands.AddSharedComponent<MeshInstanceRenderer>(premioActivar, new MeshInstanceRenderer {
                    mesh = Bootstrap_NV.bootstrap_NV.GetMesh(),
                    material = Bootstrap_NV.bootstrap_NV.GetMaterial(1)
                });
                Commands.AddComponent<PremioActivoCMP_NV>(premioActivar, new PremioActivoCMP_NV {
                    tiempoActivo = 7.0f
                });
            }
        }

        protected override void OnCreateManager() {
            //base.OnCreateManager();
            //Obtener grupos de componentes que contengan los componentes indicados
            //m_Enemigos = GetComponentGroup(typeof(EnemigoCMP_NV), ComponentType.Subtractive<EnemigoActivoCMP_NV>());
            
            m_TiempoSpawn = GetComponentGroup(typeof(TiempoSpawnCMP_NV));

            //Naves enemigas activas
            m_EnemigosActivos = GetComponentGroup(typeof(Position), typeof(EnemigoActivoCMP_NV));

            //Naves hijas existentes
            m_NavesHijas = GetComponentGroup(typeof(NaveHijaCMP_NV), typeof(Position));

            //Premios desactivados
            m_Premios = GetComponentGroup(typeof(PremioCMP_NV), ComponentType.Subtractive<PremioActivoCMP_NV>());
            
            m_Temporizador = GetComponentGroup(typeof(TemporizadorCMP_NV));

            enemigoArchetype = EntityManager.CreateArchetype(
                typeof(EnemigoActivoCMP_NV),
                typeof(Position),
                typeof(Rotation),
                typeof(Scale),
                typeof(MeshInstanceRenderer)
            );
        }

        protected override void OnUpdate() {

            //Componente que contiene los tiempos de spawn de cada elemento
            TiempoSpawnCMP_NV tiempoSpawnComponent = m_TiempoSpawn.GetComponentDataArray<TiempoSpawnCMP_NV>()[0];            

            /****** SPAWN ENEMIGOS *******/

            //Comprobar si se ha agotado el tiempo de espera del spawner de enemigos
            float tiempoNaves = tiempoSpawnComponent.segundosNaves;
            tiempoNaves = Mathf.Max(0.0f, tiempoSpawnComponent.segundosNaves - Time.deltaTime);
            bool spawnNave = tiempoNaves <= 0.0f;

            //Resetear el tiempo de espera si se ha agotado
            if (spawnNave) {                
                //Calcular cuanto hay que reducir tiempo de spawn en funcion del tiempo de juego
                temporizadorComponent = m_Temporizador.GetComponentDataArray<TemporizadorCMP_NV>()[0];
                float tiempoTotal = Time.time - temporizadorComponent.startTime; //tiempo que ha pasado desde el inicio del juego
                int t = (int)(tiempoTotal / 20); //cada 20s, el tiempo de spawn se reduce
                tiempoNaves = Mathf.Max(0.5f, 3.0f - 0.4f * t); //reducir 0.4s cada 20s, hasta un minimo de 0.5s
            }

            //Array de naves que estan inactivas
            //EntityArray navesInactivas = m_Enemigos.GetEntityArray();            

            //Spawnear nave si el tiempo de espera ha llegado a 0 y si quedan naves por activar
            if (spawnNave) {

                //Generar posiciones aleatorias hasta encontrar una que no se solape con otrar naves activas
                bool valida;
                do {             
                    posicionEnemigoX = UnityEngine.Random.Range(-3f, 3f);
                    posicionesEnemigoY = new Vector2(UnityEngine.Random.Range(5.5f, 8f), UnityEngine.Random.Range(-3.3f, -6.8f));
                    int a = (int)UnityEngine.Random.Range(0, 2);
                    posicionEnemigoY = posicionesEnemigoY[a];                    

                    valida = CompararPosiciones(posicionEnemigoX, posicionEnemigoY);

                } while (valida == false); ///Sale del bucle al encontrar una posicion valida     


                //Elegir nave hija target aleatoriamente y obtener su posicion             
                int idTarget = (int) UnityEngine.Random.Range(0, m_NavesHijas.CalculateLength());
                float3 posicionTarget = m_NavesHijas.GetComponentDataArray<Position>()[idTarget].Value;                
                                
                CrearEnemigo(posicionEnemigoX, posicionEnemigoY, posicionTarget);
            }

            /***** SPAWN PREMIOS *****/
            //Comprobar si se ha agotado el tiempo de espera del spawner de premios
            float tiempoPremios = tiempoSpawnComponent.segundosPremios;
            tiempoPremios = Mathf.Max(0.0f, tiempoSpawnComponent.segundosPremios - Time.deltaTime);
            bool spawnPremios = tiempoPremios <= 0.0f;

            if (spawnPremios) {
                //Resetear
                tiempoPremios = UnityEngine.Random.Range(15f, 30f);
            }

            //Array de premios que estan inactivos
            EntityArray premiosInactivos = m_Premios.GetEntityArray();

            //Spawnear premio si el tiempo de espera ha llegado a 0 y si quedan premios por activar
            if (spawnPremios && premiosInactivos.Length > 0) {

                //Escoger posicion aleatoriamente
                posicionPremioX = UnityEngine.Random.Range(-2.4f, 2.4f);
                posicionesPremioY = new Vector2(UnityEngine.Random.Range(2.52f, 4.8f), UnityEngine.Random.Range(-1.8f, -3.28f));
                int a = (int)UnityEngine.Random.Range(0, 2);
                posicionPremioY = posicionesPremioY[a];

                JobHandle jobPremio = new ActivarPremio() {
                    premiosInactivos = premiosInactivos,
                    Commands = _collisionBarrier.CreateCommandBuffer(),
                    randomId = UnityEngine.Random.Range(0, premiosInactivos.Length),
                    posicionX = posicionPremioX,
                    posicionY = posicionPremioY

                }.Schedule();
                jobPremio.Complete(); //Esperar a que termine el trabajo
            }

            //Actualizar el tiempo que queda en el componente 
            Entity ent = m_TiempoSpawn.GetEntityArray()[0];
            EntityManager.SetComponentData<TiempoSpawnCMP_NV>(ent, new TiempoSpawnCMP_NV {
                segundosNaves = tiempoNaves,
                segundosPremios = tiempoPremios
            });

        }

        //Metodo auxiliar utilizado por Bootstrap para inicializar la entidad de spawn
        public static void IniciarSpawner(EntityManager entityManager) {
            var arc = entityManager.CreateArchetype(typeof(TiempoSpawnCMP_NV));
            var ent = entityManager.CreateEntity(arc);

            entityManager.SetComponentData(ent, new TiempoSpawnCMP_NV {
                segundosNaves = 0.5f,
                segundosPremios = 5.0f
            });
        }

        //Metodo auxiliar para evitar que las naves enemigas se solapen
        private bool CompararPosiciones (float posX, float posY) {
            
            var posiciones = m_EnemigosActivos.GetComponentDataArray<Position>();

            for(int i=0; i < posiciones.Length; i++) {

                if((posY > posiciones[i].Value.y + 1 && posY > posiciones[i].Value.y - 1) == false) {
                    if ((posY < posiciones[i].Value.y + 1 && posY < posiciones[i].Value.y - 1) == false) {
                        if ((posX > posiciones[i].Value.x + 1 && posX > posiciones[i].Value.x - 1) == false) {
                            if ((posX < posiciones[i].Value.x + 1 && posX < posiciones[i].Value.x - 1) == false) {
                                return false;
                            }
                        }
                    }
                }                 
            }

            return true;
        }

        //Metodo auxiliar para crea una entidad de nave enemiga con su arquetipo y los valores dados
        private void CrearEnemigo (float posicionX, float posicionY, float3 posicionTarget) {
            var enemigo = EntityManager.CreateEntity(enemigoArchetype);
            float3 posicion = new float3(posicionX, posicionY, 0.0f);

            //Annadirle componentes enemigoActivo, meshrenderer y posicion 
            EntityManager.SetComponentData(enemigo, new Position {
                Value = posicion
            });
            //Annadirle component Rotacion con la Y negativa si va a aparecer en la zona inferior
            float rotZ = posicionY > 0 ? 180 : 0;
            EntityManager.SetComponentData(enemigo, new Rotation {
                Value = Quaternion.Euler(0, 0, rotZ)
            });
            EntityManager.SetComponentData(enemigo, new Scale {
                Value = new float3(1.1f, 1.1f, 1f)
            });
            EntityManager.SetSharedComponentData(enemigo, new MeshInstanceRenderer {
                mesh = Bootstrap_NV.bootstrap_NV.GetMesh(),
                material = Bootstrap_NV.bootstrap_NV.GetMaterial(0)
            });

            //Calcular direccion en la que se tiene que mover (hacia la nave objetivo)
            float3 direccion = posicionTarget - posicion;
            //Normalizar
            Vector3 dirNormalizada = new Vector3(direccion.x, direccion.y, direccion.z).normalized;
            EntityManager.SetComponentData(enemigo, new EnemigoActivoCMP_NV {
                direccion = new float3(dirNormalizada.x, dirNormalizada.y, dirNormalizada.z)
            });
        }

        private class CollisionBarrier : BarrierSystem {
        }
        [Inject] private CollisionBarrier _collisionBarrier;

    }

}


