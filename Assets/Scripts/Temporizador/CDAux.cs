/*Clase que gestiona la creacion y el funcionamiento del plugin de la cuenta atras
 * y de la entity y system que actualizan la interfaz.
 */

using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CDAux : MonoBehaviour {

    public AndroidJavaObject javaObject; //JavaObject para invocar los métodos del plugin

    //Objetos para pasarle al plugin la main activity
    public AndroidJavaClass unityClass;
    public AndroidJavaObject unityActivity;

    private bool isScreenOn;
    public bool countdownStarted;

    //UI
    public GameObject Btn_Cancel;
    public GameObject Btn_Start;
    public Slider slider;
    public GameObject Btn_Encuesta;
    public GameObject reportCustom;

    //ECS
    public EntityManager entityManager;
    public Entity countdownEntity;
    public CountdownSystem cs;

    private float minutos;

    private Animator animator;

    public AudioSource audioSource;

    void Start() {

        //Objeto para el plugin y enviar actividad
        InstantiateJavaObject("com.tfg.marta.androidplugin2.PluginClass");
        SendActivityReference("com.tfg.marta.androidplugin2.PluginClass");

        cs = World.Active.GetOrCreateManager<CountdownSystem>();

        //Si se ha cargado esta escena desde la escena "Tempo_Fin", es que se quiere
        //repetir la sesion anterior directamente (reinciar la cuenta atras con el mismo tiempo)
        if (Tempo_UltimaEscena.GetEscenaAnterior().Equals("Tempo_Fin")) {
            minutos = Tempo_UltimaEscena.GetMinutos();
            slider.GetComponent<SetTimeValue>().SetValue(minutos);
            Debug.Log("Repetir minutos: " + minutos);
            StartCDT();
        }

        Tempo_UltimaEscena.SetEscenaAnterior("Tempo");
    }

    public void InstantiateJavaObject(string pacakgeName) {
        javaObject = new AndroidJavaObject(pacakgeName);
    }

    //Pasarle al plugin esta actividad
    public void SendActivityReference(string packageName) {
        unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        javaObject.CallStatic("receiveActivityInstance", unityActivity);

        //De paso, inicia los canales de notificacion
        javaObject.Call("CreateNotificationChannel");
    }

    //Crear y comenzar la cuenta atras con el numero de minutos seleccionados con el slider
    public void StartCDT() {
        minutos = GameObject.Find("Slider").GetComponent<SetTimeValue>().GetValue();
        int millis = (int)minutos * 60 * 1000;
        javaObject.Call("CreateCDT", millis);

        //Crear entidad con el componente CountdownStarted
        entityManager = World.Active.GetOrCreateManager<EntityManager>();
        var countdownArchetype = entityManager.CreateArchetype(
            typeof(CountdownStarted)
        );
        countdownEntity = entityManager.CreateEntity(countdownArchetype);

        cs.GetTextTiempo();

        javaObject.Call("StartCDT"); //Empezar la cuenta atras
        countdownStarted = true;        

        CountdownRunning();
    }

    //Cancelar la cuenta atras
    public void StopCDT() {
        javaObject.Call("StopCDT");

        //ELMINAR ENTITY
        entityManager.DestroyEntity(countdownEntity);

        Debug.Log("STOP CDT");

        audioSource.Play();

        //Reactivar los botones
        Btn_Start.SetActive(true);
        Btn_Cancel.SetActive(false);
        slider.interactable = true;
        Btn_Encuesta.SetActive(true);
        reportCustom.SetActive(true);

        //Menu inferior
        GameObject.Find("Btn_Tempo").GetComponent<Button>().interactable = true;
        GameObject.Find("Btn_ToDo").GetComponent<Button>().interactable = true;
        GameObject.Find("Btn_Perfil").GetComponent<Button>().interactable = true;

        //Restaurar tiempo en el reloj mediante el metodo del slider
        float value = slider.GetComponent<SetTimeValue>().GetValue();
        slider.GetComponent<SetTimeValue>().SetTextoTiempo(value);

        //Pasar a estado triste
        animator = GameObject.Find("PersonajeModelo").GetComponent<Animator>();
        int nextTristeState = (int)Random.Range(1.0f, 4.0f);
        animator.SetTrigger("Triste"+nextTristeState);

    }

    public void EndCDT(string txt) {
        Debug.Log("Fin countdown");
        countdownStarted = false;

        //Eliminar entidad
        entityManager.DestroyEntity(countdownEntity);

        //Sumar las monedas        
        int monedasObtenidas = slider.GetComponent<SetTimeValue>().GetMonedas();
        int monedasActuales = PlayerPrefs.GetInt("Monedas", 0);
        PlayerPrefs.SetInt("Monedas", monedasActuales + monedasObtenidas);

        //Sumar los minutos de trabajo
        float horasActuales = PlayerPrefs.GetFloat("HorasUso", 0);
        PlayerPrefs.SetFloat("HorasUso", horasActuales + (minutos / 60));
        //El logro de horas de uso se comprueba en la siguiente escena

        Tempo_UltimaEscena.SetMinutos(minutos);
        Tempo_UltimaEscena.SetMonedas(monedasObtenidas);

        //Cargar escena de fin del temporizador
        SceneManager.LoadScene("Tempo_Fin");
    }

    //Recibe el tiempo que queda en segundos
    public void TimeLeft(string txt) {
        int.TryParse(txt, out int secondsRcv);

        entityManager.SetComponentData<CountdownStarted>(countdownEntity, new CountdownStarted {
            minutesLeft = secondsRcv / 60,
            secondsLeft = secondsRcv % 60
        });

        cs.Update(); //Indicarle al system de la cuenta atras que se actualice
    }


    void OnApplicationPause(bool pausedStatus) {
        
        //Ejecutar solo si la cuenta atras esta funcionando
        if(countdownStarted) {
            isScreenOn = javaObject.Call<bool>("IsScreenOn");
            //Si la pantalla esta encendida, significa que el usuario ha salido de la aplicacion para abrir otra
            //Por tanto, hay que parar la cuenta atras
            if (pausedStatus && isScreenOn) {
                StopCDT();
            }
            else if (pausedStatus && !isScreenOn) {
                //Debug.Log("Pantalla bloqueada");
            }
        }
        
    }

    //Desactivar boton de Start y activar el de Cancel. Impedir que se mueva el slider.
    public virtual void CountdownRunning() {
        Btn_Start.SetActive(false);
        Btn_Cancel.SetActive(true);
        slider.interactable = false;

        //Ocultar los botones de encuesta y bugs
        Btn_Encuesta.SetActive(false);
        reportCustom.SetActive(false);

        //Desactivar tambien los botones del menu inferior
        GameObject.Find("Btn_Tempo").GetComponent<Button>().interactable = false;
        GameObject.Find("Btn_ToDo").GetComponent<Button>().interactable = false;
        GameObject.Find("Btn_Perfil").GetComponent<Button>().interactable = false;

        //Pasar al estado "feliz"
        animator = GameObject.Find("PersonajeModelo").GetComponent<Animator>();
        int nextFelizState = (int) Random.Range(1.0f, 3.0f);
        animator.SetTrigger("Feliz" + nextFelizState);
    }

}
