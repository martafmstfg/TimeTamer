/*Este componente identifica a la entidad del proyectil y almacena
 * su posicion y escala originales, y la distancia que se tiene que estirar
 * segun lo qe se haya deslizado el dedo.
 */

using Unity.Entities;
using Unity.Mathematics;

namespace Naves {

    public struct ProyectilJugadorCMP_NV : IComponentData {
               
        public float distY; //distancia que se ha arrastrado el dedo para estirar
        public float escalaYOriginal;
        public float3 posOriginal; 
    }
}
