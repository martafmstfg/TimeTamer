/*Sistema que se ejecuta cuando se han destruido todas las naves hijas. 
 * Llama a los metodos del Bootstrap para finalizar la partida.
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

namespace Naves {

    public class FinJuegoSYS_NV : ComponentSystem {

        ComponentGroup m_NavesHijas;
        ComponentGroup m_Temporizador;

        protected override void OnCreateManager() {
            m_NavesHijas = GetComponentGroup(typeof(NaveHijaCMP_NV));
            m_Temporizador = GetComponentGroup(typeof(TemporizadorCMP_NV));
        }

        protected override void OnUpdate() {

            //Si se han destruido todas las naves hija
            if (m_NavesHijas.CalculateLength() == 0) {

                //Eliminar todas las entidades            
                ComponentGroup m_Jugador = GetComponentGroup(typeof(PlayerInputCMP_NV));
                EntityManager.DestroyEntity(m_Jugador);
                ComponentGroup m_Proyectil = GetComponentGroup(typeof(ProyectilJugadorCMP_NV));
                EntityManager.DestroyEntity(m_Proyectil);
                ComponentGroup m_Enemigos = GetComponentGroup(typeof(EnemigoActivoCMP_NV));
                EntityManager.DestroyEntity(m_Enemigos);
                ComponentGroup m_Premios = GetComponentGroup(typeof(PremioCMP_NV));
                EntityManager.DestroyEntity(m_Premios);                
                ComponentGroup m_TiempoSpawn = GetComponentGroup(typeof(TiempoSpawnCMP_NV));
                EntityManager.DestroyEntity(m_TiempoSpawn);
                ComponentGroup m_SonidoMoneda = GetComponentGroup(typeof(SonidoMonedaComponent));
                EntityManager.DestroyEntity(m_SonidoMoneda);
                ComponentGroup m_Explosion = GetComponentGroup(typeof(ExplosionCMP_NV));
                EntityManager.DestroyEntity(m_Explosion);
                EntityManager.DestroyEntity(m_Temporizador);
                EntityManager.DestroyEntity(m_NavesHijas);

                Bootstrap_NV.bootstrap_NV.StopMusic(); //parar musica de fondo 
                Bootstrap_NV.bootstrap_NV.PlaySound(2); //reproducir sonido fin partida
                Bootstrap_NV.bootstrap_NV.FinJuego(); //Metodo que muestra el pop-up de fin de partida y elimina todas las entidades   
                
            }
        }
    }
}
