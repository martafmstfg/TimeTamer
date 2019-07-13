/*Este componente guarda el tiempo maximo que va a permanecer activo un premio*/

using Unity.Entities;

namespace Naves {

    public struct PremioActivoCMP_NV : IComponentData {
        public float tiempoActivo; //tiempo que va a permanecer activo el premio
    }
}
