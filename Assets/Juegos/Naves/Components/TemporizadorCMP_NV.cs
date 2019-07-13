/* Componente que guarda el tiempo total de juego*/

using Unity.Entities;

public struct TemporizadorCMP_NV : IComponentData {

    public float startTime;
    public float totalTime;
}
