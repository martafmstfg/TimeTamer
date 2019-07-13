/* Componente que almacena el input del jugador (si ha deslizado hacia
 * la derecha o hacia la izquierda) y la posicion final hacia la que
 * hay que mover el sprite */


using UnityEngine;
using Unity.Entities;

public struct PlayerInputComponent : IComponentData {

    public int swipeRight;
    public int swipeLeft;

    public Vector3 posicionFinal;
}
