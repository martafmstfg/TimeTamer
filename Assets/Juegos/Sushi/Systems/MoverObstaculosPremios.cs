/* Sistema que desplaza los obstaculos y los premios hacia abajo.
 * La velocidad va aumentando en funcion del tiempo, cada 20s. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class MoverObstaculosPremios : JobComponentSystem {

    ComponentGroup m_Temporizador;

    protected override void OnCreateManager() {
        //Obtener grupos de componentes que contengan los componentes indicados (el temporizador)
        m_Temporizador = GetComponentGroup(typeof(TemporizadorComponent));
    }

    //Mueve los obstaculos activos (en pantalla) hacia abajo, la velocidad aumenta con el tiempo
    [RequireComponentTag(typeof(ObstaculoActivoComponent))]
    private struct MoverObstaculo : IJobProcessComponentData<Position> {
        public float dt;
        public int v;
       
        public void Execute(ref Position position) {
                      
            float y = -3.5f - v*0.4f;
            position.Value.y += y * dt;
        }
    }

    //Mueve los premios activos (en pantalla) hacia abajo, la velocidad aumenta con el tiempo
    [RequireComponentTag(typeof(PremioActivoComponent))]
    private struct MoverPremios : IJobProcessComponentData<Position> {
        public float dt;
        public int v;

        public void Execute(ref Position position) {

            float y = -3.5f - v * 0.4f;
            position.Value.y += y * dt;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        //Obtener el componente del temporizador
        TemporizadorComponent temporizadorComponent = m_Temporizador.GetComponentDataArray<TemporizadorComponent>()[0];
        //Calcular el tiempo que ha pasado desde el inicio del juego
        float tiempoTotal = Time.time - temporizadorComponent.startTime;
        int v = (int)(tiempoTotal / 20); //La velocidad va a aumentar cada 20 segundos

        var jobObstaculo = new MoverObstaculo {
            dt = Time.deltaTime,
            v = v
        };
        JobHandle jobObs= jobObstaculo.Schedule(this, inputDeps);

        var jobPremio = new MoverPremios {
            dt = Time.deltaTime,
            v = v
        };

        return jobPremio.Schedule(this, jobObs);
    }
}
