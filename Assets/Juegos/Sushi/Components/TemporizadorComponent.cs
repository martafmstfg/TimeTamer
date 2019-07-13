/* Componente que guarda el tiempo total de juego*/

using UnityEngine;
using Unity.Entities;

public struct TemporizadorComponent : IComponentData {
    public float startTime;
    public float totalTime;
}