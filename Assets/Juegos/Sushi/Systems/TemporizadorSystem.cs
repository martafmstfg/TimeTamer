/* Sistema que calcula el tiempo total que ha pasado desde el inicio de 
 * la partida y lo muestra en la interfaz con formato MM:SS. */

using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;

public class TemporizadorSystem : ComponentSystem {

    ComponentGroup m_Temporizador;

    protected override void OnCreateManager() {
        m_Temporizador = GetComponentGroup(typeof(TemporizadorComponent));
    }

    protected override void OnUpdate() {

        //Componente que almacena el tiempo inicial y el total
        TemporizadorComponent temporizadorComponent = m_Temporizador.GetComponentDataArray<TemporizadorComponent>()[0];
        float t = Time.time - temporizadorComponent.startTime; //Calcular diferencia de tiempo        

        //Calcular minutos y segundos, y generar el string MM:SS
        int min = ((int)t / 60);
        string minutos;
        if (min < 10) {
            minutos = "0" + min.ToString();
        }
        else {
            minutos = min.ToString();
        }

        int sec = ((int)t % 60);
        string segundos;
        if (sec < 10) {
            segundos = "0" + sec.ToString();
        }
        else {
            segundos = sec.ToString();
        }

        //Mostrar el string MM:SS en la UI
        GameObject.Find("Tiempo").GetComponent<Text>().text = minutos + ":" + segundos;
    }
}
