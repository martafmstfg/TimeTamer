/* Componente que guarda el tiempo de spawn tanto de obstaculos como de premios,
 * ya que la generacion de ambos elementos depende del mismo sistema */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct TiempoSpawnComponent : IComponentData {

    public float segundosObstaculos;
    public float segundosPremios;
}
