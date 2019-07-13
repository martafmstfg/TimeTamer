/* Este componente indica que un enemigo esta activo y almacena la direccion
 * y la posicion destino hacia la que se tiene que mover.
 */ 

using Unity.Entities;
using Unity.Mathematics;

namespace Naves {

    public struct EnemigoActivoCMP_NV : IComponentData {
        
        public float3 direccion; //direccion en la que se debe mover
        public float3 posicionDestino; //posicion objetivo para calcular la direccion

    }
}

