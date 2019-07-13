/* Componente que guarda la posicion en la que se debe generar una explosion*/

using Unity.Entities;
using Unity.Mathematics;

namespace Naves {
    public struct ExplosionCMP_NV : IComponentData {

        public float3 posicion;
    }
}


