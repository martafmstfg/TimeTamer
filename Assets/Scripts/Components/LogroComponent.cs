/* Componente que almacena los datos de una entidad Logro*/

using UnityEngine;
using Unity.Entities;

public struct LogroComponent : IComponentData {

    public int id;
    public float requisitoBase;
    public int recompensaBase;
    public int nivel;
    public float factor;
}
