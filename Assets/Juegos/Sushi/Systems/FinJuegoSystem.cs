/* Sistema que se activa cuando se crea la entidad auxiliar que posee el FinJuegoComponent
 * (cuando se detecta una colision entre el jugador y un obstaculo).
 * Se encarga de parar los sistemas y de llamar al metodo de Bootstrap que muestra
 * el pop-up del final de la partida*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class FinJuegoSystem : ComponentSystem {

    ComponentGroup m_FinJuego;
    ComponentGroup m_Temporizador;

    protected override void OnCreateManager() {
        m_FinJuego = GetComponentGroup(typeof(FinJuegoComponent));
        m_Temporizador = GetComponentGroup(typeof(TemporizadorComponent));
    }

    protected override void OnUpdate() {       

        //Si hay alguna entidad que posea el FinJuegoComponent
        if (m_FinJuego.CalculateLength() > 0) {
            
            Bootstrap.bootstrap.StopMusic(); //parar musica de fondo
            Bootstrap.bootstrap.PlaySound(1); //reproducir sonido choque

            //Eliminar todas las entidades            
            ComponentGroup m_Jugador = GetComponentGroup(typeof(PlayerInputComponent));
            EntityManager.DestroyEntity(m_Jugador);
            ComponentGroup m_Obstaculos = GetComponentGroup(typeof(ObstaculoComponent));
            EntityManager.DestroyEntity(m_Obstaculos);
            ComponentGroup m_Premios = GetComponentGroup(typeof(PremioComponent));
            EntityManager.DestroyEntity(m_Premios);
            ComponentGroup m_Manos = GetComponentGroup(typeof(ManoComponent));
            EntityManager.DestroyEntity(m_Manos);
            ComponentGroup m_TiempoSpawnManos = GetComponentGroup(typeof(TiempoSpawnManosComponent));
            EntityManager.DestroyEntity(m_TiempoSpawnManos);
            ComponentGroup m_TiempoSpawnObstaculos = GetComponentGroup(typeof(TiempoSpawnComponent));
            EntityManager.DestroyEntity(m_TiempoSpawnObstaculos);
            ComponentGroup m_SonidoMoneda = GetComponentGroup(typeof(SonidoMonedaComponent));
            EntityManager.DestroyEntity(m_SonidoMoneda);

            //Calcula y guarda el tiempo total que ha durado la partida
            TemporizadorComponent temporizadorComponent = m_Temporizador.GetComponentDataArray<TemporizadorComponent>()[0];
            float t = Time.time - temporizadorComponent.startTime;
            Bootstrap.bootstrap.GuardarTiempoTotal(t);            

            EntityManager.DestroyEntity(m_FinJuego);
            EntityManager.DestroyEntity(m_Temporizador);
            
            Bootstrap.bootstrap.FinJuego(); //Metodo que muestra el pop-up de fin de partida     
            Bootstrap.bootstrap.PlaySound(2); //reproducir sonido fin partida

        }        
    }
}
