/*Script que actualiza la interfaz de la escena de logros para mostrar el progreso
 * en cada uno, sus requisitos y la recompensa que proporcionan.
 */

using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;

public class LogrosUI : MonoBehaviour {

    private EntityManager entityManager;
    private List<Entity> entidadesLogros;

    // Use this for initialization
    void Start () {
        
        entityManager = World.Active.GetOrCreateManager<EntityManager>();

        entidadesLogros = LogrosManager.GetListaEntidades();

        LogroComponent componenteAux;

        //Actualiza la UI por cada entidad de Logro existente, con los datos almacenados en su componentes
        foreach (Entity logro in entidadesLogros) {
            componenteAux = entityManager.GetComponentData<LogroComponent>(logro);
            RellenarBarraProgreso(componenteAux);
            ActualizarRequisito(componenteAux);
            ActualizarRecompensa(componenteAux);
        }
    }    

    //Rellena la barra de progreso del logro según el porcentaje de requisitos cumplidos
    private void RellenarBarraProgreso (LogroComponent componenteAux) {        
        Image fillBarraProgreso;

        int idLogro = componenteAux.id;        

        fillBarraProgreso = GameObject.Find("Fill" + idLogro).GetComponent<Image>();

        string nombreRequisito = LogrosManager.GetNombreRequisito(idLogro);
        float valorRequisito;

        //Si aun no hay ningun valor almacenado para el requisito del logro, se obtiene el valor
        // por defecto, que en el caso de los dias seguidos es 1 (porque cuenta el primer dia que abre la app)
        if(nombreRequisito.Equals("DiasSeguidos")) {
            valorRequisito = PlayerPrefs.GetFloat(nombreRequisito, 1);
        } else {
            valorRequisito = PlayerPrefs.GetFloat(nombreRequisito, 0);
        }

        //Calcular la proporcion con la que hay que rellenar la barra
        float requisito = componenteAux.requisitoBase * componenteAux.nivel * componenteAux.factor;
        float fillAmount = (float)valorRequisito / requisito;
        fillBarraProgreso.fillAmount = fillAmount;

        //Debug.Log("Id: "+idLogro+", "+nombreRequisito+": "+valorRequisito+"/"+requisito);
    }

    //Actualiza en la UI los requisitos para un logro y cuantos se han cumplido
    private void ActualizarRequisito (LogroComponent componenteAux) {
        int idLogro = componenteAux.id;

        string nombreRequisito = LogrosManager.GetNombreRequisito(componenteAux.id);
        float valorRequisito;

        if (nombreRequisito.Equals("DiasSeguidos")) {
            valorRequisito = PlayerPrefs.GetFloat(nombreRequisito, 1);
        }
        else {
            valorRequisito = PlayerPrefs.GetFloat(nombreRequisito, 0);
        }
        //El valor de los requisitos depende del valor base, el nivel del logro y un factor de multiplicacion
        float requisito = componenteAux.requisitoBase * componenteAux.nivel * componenteAux.factor;        

        GameObject.Find("Requisito" + idLogro).GetComponent<Text>().text = (Mathf.Round(valorRequisito * 10f) / 10f) + "/" + (int)requisito;
    }

    //Actualiza en la UI la recompensa de un logro
    private void ActualizarRecompensa (LogroComponent componenteAux) {
        int idLogro = componenteAux.id;
;
        //El valor de la recompensa depende del valor base y del nivel del logro
        int recompensa = componenteAux.recompensaBase * componenteAux.nivel;

        GameObject.Find("Recompensa" + idLogro).GetComponent<Text>().text = recompensa.ToString();
    }
}
