/*Sistema que se ejeucta cuando existe una entidad con el componente ExplosionCMP_NV.
 * Llama a un metodo del bootstrap para instanciar un prefab con un sistema de particulas,
 * en la posicion que haya guardada en el componente.
 */ 

using UnityEngine;
using Unity.Entities;

namespace Naves {
   
    [UpdateAfter(typeof(ColisionProyectilSYS_NV))]
    public class ExplosionSYS_NV : ComponentSystem {

        ComponentGroup m_Explosiones;
        EntityArray exp;
        ComponentDataArray<ExplosionCMP_NV> explosiones;

        protected override void OnCreateManager() {
            m_Explosiones = GetComponentGroup(typeof(ExplosionCMP_NV));
        }      

        protected override void OnUpdate() {

            exp = m_Explosiones.GetEntityArray();
            explosiones = m_Explosiones.GetComponentDataArray<ExplosionCMP_NV>();

            Vector3 posicion;
            for(int i=0; i<exp.Length; i++) {

                posicion = explosiones[i].posicion;

                Bootstrap_NV.bootstrap_NV.Explosion(posicion);
                Bootstrap_NV.bootstrap_NV.PlaySound(1);

                EntityManager.RemoveComponent<ExplosionCMP_NV>(exp[i]);
            }
        }
    }
}
