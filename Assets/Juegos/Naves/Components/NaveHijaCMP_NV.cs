/* Este componente identifica a una entidad como nave hija y almacena
 * su posicion y la direccion en la que rota.
 */ 

using Unity.Entities;
using Unity.Mathematics;

namespace Naves {
    public struct NaveHijaCMP_NV : IComponentData {
        public float3 posicion; //posicion original en la que se crea
        public float direccion; //direccion en la que gira (sentido horario o anti-horario)
    }
}

