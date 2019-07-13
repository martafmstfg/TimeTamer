using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Unity.Entities.Tests;

[TestFixture]
[Category("Unity ECS Tests")]
public class AndroidPluginTests : ECSTestsFixture{

    private CDAux cdaux;
    private AndroidJavaObject javaobjAux;

    [SetUp]
    public void BeforeEveryTest () {
        cdaux = new GameObject().AddComponent<CDAux>();
        javaobjAux = new AndroidJavaObject("com.tfg.marta.androidplugin2.PluginClass");
    }

    //Comprobar que se crea correctamente el androidjavaobject
	[Test]
    public void CreateJavaAndroidObject () {
       
        //Metodo start de CDAux llama a InstantiateJavaObject
        cdaux.InstantiateJavaObject("com.tfg.marta.androidplugin2.PluginClass");
        
        Assert.That(cdaux.javaObject.GetRawClass(), Is.EqualTo(javaobjAux.GetRawClass()));       
    }

    //Comprobar que la actividad se pasa correctamente
    //FALLA: No puedo obtener la activity actual en el modo test
    [Test]
    public void SendActivity () {
        var unityClassAux = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var unityActivityAux = unityClassAux.GetStatic<AndroidJavaObject>("currentActivity");

        //Llama a metodos de CDAux para que se instancien los objetos y se envie la actividad
        cdaux.InstantiateJavaObject("com.tfg.marta.androidplugin2.PluginClass");
        cdaux.SendActivityReference("com.tfg.marta.androidplugin2.PluginClass");

        //Comprobar que los dos objetos se han creado correctamente
        Assert.That(cdaux.unityClass.GetRawClass(), Is.EqualTo(unityClassAux.GetRawClass()));
        Assert.That(cdaux.unityActivity.GetRawClass(), Is.EqualTo(unityActivityAux.GetRawClass()));
        //Comprobar que la actividad se ha enviado
        //Assert.That(unityActivityAux.GetHashCode(), Is.EqualTo(javaobjAux.GetStatic<AndroidJavaObject>("myActivity").GetHashCode()));
    }
}
