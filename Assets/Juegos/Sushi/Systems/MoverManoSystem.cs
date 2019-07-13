/* Sistema que desplaza las manos hacia delante y hacia atras cada cierto tiempo (spawn)*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class MoverManoSystem : JobComponentSystem {

    private ComponentGroup m_Manos;
    private ComponentGroup m_TiempoSpawn;

    //Mueve la mano activa en la direccion correspondiente
    private struct MoverMano : IJobProcessComponentData<Position, ManoComponent, ManoActivaComponent> {

        public float dt;

        public void Execute(ref Position position, ref ManoComponent manoComponent, ref ManoActivaComponent manoActivaComponent) {               
            
            //Solo mueve la mano que esta activa
            if(manoActivaComponent.activa == 1) {

                float velocidad = 5f;

                //direccionX indica si la mano se tiene que mover hacia delante (1, "hacia el jugador") 
                // o hacia atras (-1, "para ocultarse")
                if (manoComponent.direccionX > 0) {

                    float3 posicionFinal = new float3(0, 0, 0); //Carril de un extremo o borde del carril, segun el numero de avisos
                    
                    //Primero mueve la mano al borde un par de veces
                    if (manoActivaComponent.movimientos < 2) {
                        posicionFinal = manoComponent.posicionBorde;
                        velocidad = 3f;
                    }
                    //Despues, saca la mano entera
                    else if (manoActivaComponent.movimientos >= 2) {
                        posicionFinal = manoComponent.posicionFinal;
                    }

                    position.Value = Vector3.MoveTowards(position.Value, posicionFinal, velocidad * dt);

                    //Si ha alcanzado la posicion final ("hacia el jugador"), cambia de direccion y se incrementa el numero de movimientos
                    if (position.Value.x == posicionFinal.x) {
                        manoComponent.direccionX = -1;
                        manoActivaComponent.movimientos++;
                    }
                }
                else if (manoComponent.direccionX < 0) {
                    position.Value = Vector3.MoveTowards(position.Value, manoComponent.posicionInicial, 3f * dt);

                    //Si ha vuelto a la posicion inicial, cambiar la direccion
                    if (position.Value.x == manoComponent.posicionInicial.x) {
                        manoComponent.direccionX = 1;

                        //Si el numero de avisos ya es 4, lo resetea y desactiva la mano
                        if(manoActivaComponent.movimientos == 3) {
                            manoActivaComponent.movimientos = 0;
                            manoActivaComponent.activa = 0;
                        }                        
                    }
                }

            }        
        }
    }

    protected override void OnCreateManager() {
        //Obtener grupos de componentes que contengan los componentes indicados (manos y tiempospawnmanos)
        m_Manos = GetComponentGroup(typeof(ManoComponent));
        m_TiempoSpawn = GetComponentGroup(typeof(TiempoSpawnManosComponent));
    }

    protected override JobHandle OnUpdate(JobHandle inputDepts) {
        //Componente que guarda el tiempo de espera para activar una de las manos
        TiempoSpawnManosComponent tiempoSpawn = m_TiempoSpawn.GetComponentDataArray<TiempoSpawnManosComponent>()[0];       

        //Comprobar si se ha agotado el tiempo de espera del spawner de manos
        float tiempoManos = tiempoSpawn.segundosManos;
        tiempoManos = Mathf.Max(0.0f, tiempoSpawn.segundosManos - Time.deltaTime);
        bool spawnManos = tiempoManos <= 0.0f;              

        if (spawnManos) {
            //Resetear el tiempo de spawn a un valor aleatorio entre 15s y 30s
            tiempoManos = UnityEngine.Random.Range(15f, 30f);
        }

        //Array de manos
        EntityArray manos = m_Manos.GetEntityArray();
        
        //Si se ha agotado el tiempo de espera, escoger aleatoriamente una de las dos manos y activarla
        if (spawnManos) {                        
            int manoId = UnityEngine.Random.Range(0, manos.Length);
            EntityManager.SetComponentData(manos[manoId], new ManoActivaComponent { activa = 1 });
        }

        //Actualizar tiempo de spawn restante en el componente
        Entity ent = m_TiempoSpawn.GetEntityArray()[0];
        EntityManager.SetComponentData<TiempoSpawnManosComponent>(ent, new TiempoSpawnManosComponent {
            segundosManos = tiempoManos
        });

        //Crear trabajo para mover la mano activada
        return new MoverMano() { dt = Time.fixedDeltaTime }.Schedule(this, inputDepts);
    }

    //Metodo auxiliar utilizado por Bootstrap para inicializar la entidad de spawn de manos
    public static void IniciarManos(EntityManager entityManager) {
        var arc = entityManager.CreateArchetype(typeof(TiempoSpawnManosComponent));
        var ent = entityManager.CreateEntity(arc);

        entityManager.SetComponentData(ent, new TiempoSpawnManosComponent {
            segundosManos = 10.0f
        });
    }
}
