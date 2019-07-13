/*Componente que almacena el tiempo de spawn de las manos*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct TiempoSpawnManosComponent : IComponentData {
    
    public float segundosManos;
}

