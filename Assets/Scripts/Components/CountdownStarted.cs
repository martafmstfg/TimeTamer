/* Componente que almacena el tiempo restante de la cuenta atras.
 */ 

using Unity.Entities;
using UnityEngine;

public struct CountdownStarted : IComponentData {
    public int minutesLeft;
    public int secondsLeft;
}
