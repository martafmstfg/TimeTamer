/* Sistema ECS que actualiza el valor de la cuenta atras en la interfaz*/

using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;


[DisableAutoCreation]
public class CountdownSystem : ComponentSystem {

    ComponentGroup m_Countdown;
    Text textTime;    

    protected override void OnStartRunning() {
        base.OnStartRunning();

        m_Countdown = GetComponentGroup(typeof(CountdownStarted));        
    }

    public void GetTextTiempo () {
        textTime = GameObject.Find("Tiempo").GetComponent<Text>();
    }

    protected override void OnUpdate() {
        //Datos del componente CountdownStarted, que contiene el tiempo restante
        CountdownStarted countdownStarted = m_Countdown.GetComponentDataArray<CountdownStarted>()[0];
        textTime.text = countdownStarted.minutesLeft.ToString("D2") + ":" + countdownStarted.secondsLeft.ToString("D2");
    }

}


