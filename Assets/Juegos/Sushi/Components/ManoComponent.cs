/* Componente que identifica a las entidades como manos*/

using Unity.Entities;
using Unity.Mathematics;

public struct ManoComponent : IComponentData {

    public int direccionX; //Direccion en la que se debe mover (hacia adelante o hacia atras)
    public float3 posicionInicial; //Posicion original (fuera de la pantalla)
    public float3 posicionFinal; //Posicion a la que se tiene que mover (carril extremo)
    public float3 posicionBorde; //Posicion en el borde de la cinta
}
