/* Componente que guarda el tiempo de spawn tanto de enemigos como de premios,
 * ya que la generacion de ambos elementos depende del mismo sistema */
 
using Unity.Entities;

namespace Naves {
    public struct TiempoSpawnCMP_NV : IComponentData {
        public float segundosNaves;
        public float segundosPremios;
    }
}


