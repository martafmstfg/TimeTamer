/* Sistema ECS que comprueba si se ha cumplido los requisitos del logro 
 * a cuya entidad se le ha añadido el componente ComprobarLogroComponent.
 */ 

using Unity.Entities;
using UnityEngine;
using Unity.Jobs;

//[DisableAutoCreation]
public class ComprobarLogroSystem : ComponentSystem {
       
    ComponentGroup m_Logros;

    protected override void OnCreateManager() {        
        m_Logros = GetComponentGroup(typeof(LogroComponent), typeof(ComprobarLogroComponent));
    }

    protected override void OnUpdate () {
        //Entidades que poseen los componentes LogroComponent y ComprobarLogroComponent
        EntityArray logros = m_Logros.GetEntityArray();
        //Datos de los LogroComponent obtenidos
        var logroComponents = m_Logros.GetComponentDataArray<LogroComponent>();
        bool completado;
        LogroComponent componenteAux;

        for (int i=0; i<logros.Length; i++) {
            componenteAux = logroComponents[i];            
            completado = ComprobarLogroCompletado(componenteAux);

            //Subir el nivel en la entidad para la ui
            if (completado) {
                EntityManager.SetComponentData(logros[i], new LogroComponent {
                    id = componenteAux.id,
                    requisitoBase = componenteAux.requisitoBase,
                    recompensaBase = componenteAux.recompensaBase,
                    nivel = componenteAux.nivel + 1,
                    factor = componenteAux.factor
                });

                //Mostrar el pop-up de logro completado
                LogrosManager.PopUpLogroCompletado(componenteAux.id, componenteAux.nivel, componenteAux.recompensaBase * componenteAux.nivel);
            }

            EntityManager.RemoveComponent(logros[i], typeof(ComprobarLogroComponent));
            
        }
    }    

    private bool ComprobarLogroCompletado(LogroComponent logroComponent) {
        string nombreRequisito = LogrosManager.nombresRequisitos[logroComponent.id];
        float valorActual = PlayerPrefs.GetFloat(nombreRequisito);
        float requisitoBase = logroComponent.requisitoBase;
        int nivelActual = logroComponent.nivel;
        float factor = logroComponent.factor;
                
        //El logro se ha completado si se ha igualado o superado el valor del requisito
        if (valorActual >= (int)(requisitoBase * nivelActual * factor)) {
            Debug.Log("Logro completado");
            //Subir en nivel y guardarlo en pps
            int nivel = logroComponent.nivel;               
            PlayerPrefs.SetInt("Nivel"+ logroComponent.id, nivel+1);
            ObtenerRecompensa(logroComponent);

            return true;
        }

        return false;
    }

    //Suma las monedas obtenidas segun el nivel de logro completado
    public void ObtenerRecompensa(LogroComponent logroComponent) {
        int nivelActual = logroComponent.nivel;
        int monedasInicial = PlayerPrefs.GetInt("Monedas");
        PlayerPrefs.SetInt("Monedas", monedasInicial + logroComponent.recompensaBase * nivelActual);
    }
}
