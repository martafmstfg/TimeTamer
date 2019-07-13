/*Sistema que utiliza el metodo del Bootstrap para reproducir un sonido cuando
 * detecta que se ha creado una entidad con el componente SonidoMonedaComponent.
 * (No se puede reproducir el sonido directamente desde el sistema porque necesita el archivo)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;


public class SonidoMoneda : ComponentSystem {

    ComponentGroup m_SonidosMoneda;
    EntityArray s;    

    protected override void OnCreateManager() {
        m_SonidosMoneda = GetComponentGroup(typeof(SonidoMonedaComponent));
    }

    protected override void OnUpdate() {

        s = m_SonidosMoneda.GetEntityArray();        
        
        for (int i = 0; i < s.Length; i++) {

            //Bootstrap.PlaySound(0); //reproducir sonido moneda

            Bootstrap.bootstrap.PlaySound(0);

            EntityManager.RemoveComponent<SonidoMonedaComponent>(s[i]);
        }

    }

}

