using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Unity.Entities.Tests;
using UnityEngine.UI;
using Unity.Entities;

[TestFixture]
[Category("Unity ECS Tests")]
public class CountdownTests : ECSTestsFixture {

    private CDAux cdaux;

    [SetUp]
    public void BeforeEveryTest() {
        cdaux = new GameObject().AddComponent<CDAux>();

        //Inicializar entity y manager
        var countdownEntity = m_Manager.CreateEntity(typeof(CountdownStarted));
        cdaux.entityManager = m_Manager;
        cdaux.countdownEntity = countdownEntity;
    }

    //Comprobar que se crea e inicia correctamente la cuenta atras    
    // !! Hay que comentar la llamada a CountdownRunning
    [Test]
    public void StartCountdown() {

        //Inicializar objetos android
        cdaux.InstantiateJavaObject("com.tfg.marta.androidplugin2.PluginClass");
        cdaux.SendActivityReference("com.tfg.marta.androidplugin2.PluginClass");

        //Inicializar el slider y ponerle un valor (1 minuto)
        CrearSlider(1.0f);     

        //Ejecutar metodo startCDT
        cdaux.StartCDT();        

        //Comprobar que countdownstarted es true
        Assert.That(cdaux.countdownStarted, Is.True);

        //Comprobar que se ha creado la entity
        bool entityExists = World.Active.GetOrCreateManager<EntityManager>().Exists(cdaux.countdownEntity);
        Assert.That(entityExists, Is.True);
    }

    //Comprobar que el metodo StopCDT detiene la cuenta atras en el plugin y elimina la entidad
    [Test]
    public void StopCountdown () {

        //Inicializar objetos android
        cdaux.InstantiateJavaObject("com.tfg.marta.androidplugin2.PluginClass");
        cdaux.SendActivityReference("com.tfg.marta.androidplugin2.PluginClass");

        //Crear objetos de la escena necesarios
        AudioSource audioSource = new GameObject().AddComponent<AudioSource>();
        cdaux.audioSource = audioSource;
        CrearSlider(1.0f);
        CrearTexto();
        CrearBoton("Btn_Tempo");
        CrearBoton("Btn_ToDo");
        CrearBoton("Btn_Perfil");
        GameObject btnStart = new GameObject();
        GameObject btnCancel = new GameObject();
        cdaux.Btn_Start = btnStart;
        cdaux.Btn_Cancel = btnCancel;
        CrearAnimator("PersonajeModelo");

        //Ejecutar el metodo a testear
        cdaux.StopCDT();

        //Comprobar que la entity ya no existe
        bool entityExists = World.Active.GetOrCreateManager<EntityManager>().Exists(cdaux.countdownEntity);
        Assert.That(entityExists, Is.False);        
    }

    //Comprobar que el metodo EndCDT elimina la entidad y pone countdownStarted a false
    //Hay que quitar loadscene para que funcione
    [Test]
    public void EndCountdown() {

        CrearSlider(1.0f);
       
        cdaux.EndCDT("end");

        Assert.That(cdaux.countdownStarted, Is.False);
        Assert.That(m_Manager.Exists(cdaux.countdownEntity), Is.False);
    }

    //Comprobar que TimeLeft pone el tiempo que queda en el componente de la entidad
    [Test]
    public void TestTimeLeft () {
        //Sistema auxiliar
        cdaux.cs = World.Active.GetOrCreateManager<CountdownSystem>();

        //Objeto texto
        CrearSlider(1.0f);
        CrearTexto();

        cdaux.TimeLeft("30"); //Quedan 30 segundos

        //Comprobar que el componente tiene los datos correctos
        var component = m_Manager.GetComponentData<CountdownStarted>(cdaux.countdownEntity);
        int minutesLeft = component.minutesLeft;
        int secondsLeft = component.secondsLeft;
        Assert.That(minutesLeft, Is.EqualTo(0));
        Assert.That(secondsLeft, Is.EqualTo(30));

        //Comprobar que el texto tiene el valor correcto
        string texto = GameObject.Find("Tiempo").GetComponent<Text>().text;
        Assert.That(texto, Is.EqualTo("0:30"));
    }

    //Metodos auxiliares para crear los objetos de la escena
    private void CrearSlider (float valor) {
        Slider slider = new GameObject().AddComponent<Slider>();
        slider.gameObject.name = "Slider";
        slider.gameObject.AddComponent<SetTimeValue>();
        slider.gameObject.GetComponent<SetTimeValue>().Awake();
        slider.gameObject.GetComponent<SetTimeValue>().SetValue(valor);
        cdaux.slider = slider;
    }

    private void CrearTexto() {
        GameObject tiempo = new GameObject();
        tiempo.AddComponent<Text>();
        tiempo.gameObject.name = "Tiempo";

        cdaux.slider.GetComponent<SetTimeValue>().textoTiempo = tiempo.GetComponent<Text>();
    }

    private void CrearBoton (string nombre) {
        Button boton = new GameObject().AddComponent<Button>();
        boton.gameObject.name = nombre;
    }

    private void CrearAnimator (string nombre) {
        Animator animator = new GameObject().AddComponent<Animator>();
        animator.gameObject.name = nombre;
    }
}
