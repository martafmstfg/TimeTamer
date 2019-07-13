/* Script que gestiona la creacion de las entidades de los logros, la ejecucion
 * del sistema ComprobarLogros y la aparicion del pop-up cuando se completa un logro.
 */ 


using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;

public static class LogrosManager {

    //ECS
    public static EntityManager entityManager;
    //Entidades para los logros
    public static Entity logroSimpleEntity, logroDiasEntity, logroHorasEntity, logroObjetosEntity, logroJuegosEntity, 
        logroTareasEntity, logroKaitenzushiEntity, logroSlingerEntity, logroNovaEntity;
    public static ComprobarLogroSystem cls; //Sistema que comprueba si se ha completado un logro
    public static EntityArchetype logroArchetype;

    public static string[] titulosLogros = new string[] { "Dia a día", "Productividad", "Shopaholic", "True gamer", "Taskminator", "Kaitenzushi", "Space Slinger", "Nova+", "Test"};
    public static string[] nombresRequisitos = new string[] { "DiasSeguidos", "HorasUso", "ObjetosConseguidos", "JuegosConseguidos", "TareasCompletadas", "PuntosKaitenzushi", "PuntosSlinger", "PuntosNova", "Test"};

    public static List<Entity> entidadesLogros;

    //Metodo que crea las entidades de los logros, se ejecuta al iniciar la primera escena
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeAfterSceneLoad() {

        entidadesLogros = new List<Entity>(); 

        //CREAR ENTIDADES LOGROS
        entityManager = World.Active.GetOrCreateManager<EntityManager>();
        logroArchetype = entityManager.CreateArchetype(ComponentType.Create<LogroComponent>());
                
        //Logro Dias Seguidos
        logroDiasEntity = CrearEntidadLogro(0, 3, 3, 1.0f);        
        entidadesLogros.Add(logroDiasEntity);

        //Logro Horas Uso
        logroHorasEntity = CrearEntidadLogro(1, 3, 3, 1.5f);       
        entidadesLogros.Add(logroHorasEntity);

        //Logro Objetos Conseguidos
        logroObjetosEntity = CrearEntidadLogro(2, 3, 3, 1.0f);        
        entidadesLogros.Add(logroObjetosEntity);

        //Logro Juegos Conseguidos
        logroJuegosEntity = CrearEntidadLogro(3, 1, 5, 1.0f);        
        entidadesLogros.Add(logroJuegosEntity);

        //Logro Tareas Completadas
        logroTareasEntity = CrearEntidadLogro(4, 5, 5, 1.5f);
        entidadesLogros.Add(logroTareasEntity);

        //Logro Kaitenzushi
        logroKaitenzushiEntity = CrearEntidadLogro(5, 300, 5, 1.0f);
        entidadesLogros.Add(logroKaitenzushiEntity);

        //Logro Space Slinger
        logroSlingerEntity = CrearEntidadLogro(6, 300, 5, 1.0f);
        entidadesLogros.Add(logroSlingerEntity);

        //Logro Nova
        logroNovaEntity = CrearEntidadLogro(7, 1000, 5, 1.0f);
        entidadesLogros.Add(logroNovaEntity);

        cls = World.Active.GetOrCreateManager<ComprobarLogroSystem>();    
    }

    //Metodo para crear entidades con los datos que se reciben como argumentos
    public static Entity CrearEntidadLogro (int id, float requisitoBase, int recompensaBase, float factor) {
        Entity entidad = entityManager.CreateEntity(logroArchetype);
       
        entityManager.SetComponentData(entidad, new LogroComponent {
            id = id,
            requisitoBase = requisitoBase,
            recompensaBase = recompensaBase,
            nivel = PlayerPrefs.GetInt("Nivel" + id, 1),
            factor = factor
        });

        return entidad;
    }

    //Metodo que agrega el componente ComprobarLogro a la entidad que representa al logro que
    // se quiera comprobar, y que ordena al ComprobarLogroSystem que se ejecute
    public static void ComprobarLogroCompletado(int id) {
        Debug.Log("entidades logro length: " + entidadesLogros.Count);
        Entity logroComprobar = entidadesLogros[id]; //Obtener la entidad del logro a comprobar
        //Agregarle el componente para que el sistema la procese
        entityManager.AddComponent(logroComprobar, ComponentType.Create<ComprobarLogroComponent>());        

        cls.Update(); //Ejecutar el ComprobarLogroSystem
         
        //Una vez se ha comprobado este logro, se le quita el componente
        entityManager.RemoveComponent<ComprobarLogroComponent>(logroComprobar);
    }
    
    public static List<Entity> GetListaEntidades () {
        return entidadesLogros;
    }

    public static string GetNombreRequisito (int id) {
        return nombresRequisitos[id];
    }

    //Metodo que muestra un pop-up cuando se ha comppletado un logros
    public static void PopUpLogroCompletado (int id, int nivel, int recompensa) {
        Transform canvas = GameObject.Find("Canvas").transform;

        AudioSource audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();

        //Instancia el prefab base del pop-up
        var PopUpJuego = MonoBehaviour.Instantiate(Resources.Load("Prefabs/LogroCompletadoPopUp") as GameObject, canvas);

        var PPPanel = PopUpJuego.transform.Find("PanelLogroCompletado");

        //Pone la informacion correspondiente al logro en el pop-up
        PPPanel.transform.Find("NombreLogro").GetComponent<Text>().text = titulosLogros[id] + " " + nivel;
        PPPanel.transform.Find("MonedasLogro").GetComponent<Text>().text = "+" + recompensa;
        string path = "Logros/Logro" + nombresRequisitos[id];
        var sprite = Resources.Load<Sprite>(path);
        PPPanel.transform.Find("Medalla").GetComponent<Image>().sprite = sprite;

        //Sonido logro completado
        audioSource.PlayOneShot(Resources.Load("Sonidos/Logro") as AudioClip);
    }
}
