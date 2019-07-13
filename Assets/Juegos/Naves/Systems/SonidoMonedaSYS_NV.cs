/*Sistenma que se ejecuta cuando se crea una entidad que tenga el 
 * SonidoMonedaComponent. Llama al metodo del Bootstrap que reproduce sonido.
 */

using Unity.Entities;

namespace Naves {

    public class SonidoMonedaSYS_NV : ComponentSystem {

        ComponentGroup m_SonidosMoneda;
        EntityArray s;

        protected override void OnCreateManager() {
            m_SonidosMoneda = GetComponentGroup(typeof(SonidoMonedaComponent));
        }

        protected override void OnUpdate() {

            s = m_SonidosMoneda.GetEntityArray();

            for (int i = 0; i < s.Length; i++) {

                Bootstrap_NV.bootstrap_NV.PlaySound(0); //reproducir sonido moneda

                EntityManager.RemoveComponent<SonidoMonedaComponent>(s[i]);
            }

        }

    }
}

