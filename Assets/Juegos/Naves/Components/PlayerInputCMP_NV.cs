/* En este componente se guarda la rotacion que se debe aplicar a la nave 
 * del jugador segun su input en la pantalla tactil.
 */

using Unity.Entities;
using UnityEngine;

namespace Naves {

    public struct PlayerInputCMP_NV : IComponentData {
        public Quaternion rotZ; //Grados en el eje Z que se va a rotar la nave segun el input    
        //public float vida;
    }
}

