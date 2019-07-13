/* Componente que indica si la entidad de la mano esta activa (=1) y se debe mover */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct ManoActivaComponent : IComponentData {
    public int activa;
    public int movimientos; //veces que se ha movido la mano hacia delante en un "turno"
}
