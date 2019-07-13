using Zenject;
using NUnit.Framework;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[TestFixture]
public class NovaTests : ZenjectUnitTestFixture {

    [SetUp]
    public void BeforeEveryTest() {

        Container.Bind<Celda>().FromNewComponentOnNewGameObject().AsSingle();

        //Objetos que necesita el game manager
        Container.Bind<RectTransform>().WithId("canvasTransform").FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<Text>().WithId("puntosTxt").FromNewComponentOnNewGameObject().AsCached();
        Container.Bind<Text>().WithId("tiempoTxt").FromNewComponentOnNewGameObject().AsCached();
        Container.Bind<AudioSource>().FromNewComponentOnNewGameObject().AsCached();
        
        Container.Bind<GameManager_NO>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<InputManager_NO>().FromNewComponentOnNewGameObject().AsSingle();
               
        Container.Inject(this);        
    }

    [Inject]
    Celda _celda;
    [Inject]
    GameManager_NO _gameManager;
    [Inject]
    InputManager_NO _inputManager;

    Sprite sprite;

    /***** CELDA *****/

    //Comprueba que vaciar una casilla equivale a ocultar su imagen
    [Test]
    public void TestVaciarCelda () {
        GenerarCelda(_celda);

        _celda.VaciarCasilla();

        Assert.That(_celda.GetComponent<Image>().enabled, Is.False);
    }

    //Comprueba que se cambia el sprite al indicado
    [Test]
    public void TestCambiarSprite() {
        GenerarCelda(_celda);

        _celda.CambiarSprite(0);

        Assert.That(_celda.GetComponent<Image>().sprite, Is.EqualTo(sprite));
    }

    
    /***** INPUT MANAGER *****/

    //Comprobar que la direccion se calcula correctamente a partir del parametro swipeDelta
    [Test]
    public void TestCalcularDireccion (){
        Vector2 swipeDelta = Vector2.zero;
        _inputManager.swipeDelta = swipeDelta;

        //Mover a la derecha (abs x > abs y, x > 0)
        _inputManager.swipeDelta.x = 1;
        _inputManager.swipeDelta.y = 0;
        int direccion = _inputManager.CalcularDireccion();
        Assert.That(direccion, Is.EqualTo(3));

        //Mover a la izquierda (abs x > abs y, x < 0)
        _inputManager.swipeDelta.x = -1;
        _inputManager.swipeDelta.y = 0;
        direccion = _inputManager.CalcularDireccion();
        Assert.That(direccion, Is.EqualTo(2));

        //Mover arriba (abs x < abs y, x > 0)
        _inputManager.swipeDelta.x = 0;
        _inputManager.swipeDelta.y = 1;
        direccion = _inputManager.CalcularDireccion();
        Assert.That(direccion, Is.EqualTo(0));

        //Mover abajo (abs x < abs y, x < 0)
        _inputManager.swipeDelta.x = 0;
        _inputManager.swipeDelta.y = -1;
        direccion = _inputManager.CalcularDireccion();
        Assert.That(direccion, Is.EqualTo(1));
    }


    /***** GAME MANAGER *****/

    //Comprobar que se crea una estrella si quedan casillas vacias
    [Test]
    public void TestCrearEstrella () {
        //Crear una celda vacia y meterla en la lista
        GenerarCelda(_celda);
        _celda.TipoEstrella = 0;
        List<Celda> celdasVacias = new List<Celda>();
        celdasVacias.Add(_celda);
        _gameManager.celdasVacias = celdasVacias;

        _gameManager.GenerarEstrella();

        //Comprobar que la lista de celdas vacias no tiene ya ningun elemento
        Assert.That(_gameManager.celdasVacias.Count, Is.EqualTo(0));
        //Y que la celda ahora tiene un tipo de estrella distinto de 0 (no vacia)
        Assert.That(_celda.TipoEstrella, Is.GreaterThan(0));
    }

    //Comprobar que una estrella pasa de una casilla a otra vacia al moverse, es decir:
    // 1) La casilla original pasa a estar vacia (tipo 0), la casilla destino gana una estrella (tipo != 0)
    // 2) La casilla original entra en la lista de celdas vacias, la destino sale de ella
    [Test]
    public void TestIntercambiarCasillas () {
        //Crear dos celdas: una vacia y otra de tipo 1
        List<Celda> celdasVacias = new List<Celda>();
        List<Celda> listaCeldas = new List<Celda>();
        GenerarCelda(_celda);
        _celda.TipoEstrella = 0;
        _celda.id = 0;
        celdasVacias.Add(_celda);
        listaCeldas.Add(_celda);
        _gameManager.celdasVacias = celdasVacias;

        Celda celdaActual = new GameObject().AddComponent<Celda>();
        GenerarCelda(celdaActual);
        celdaActual.id = 1;
        celdaActual.TipoEstrella = 1;
        listaCeldas.Add(celdaActual);
        _gameManager.listaCeldas = listaCeldas;

        _gameManager.Intercambiar(1, 0);

        //Comprobar intercambio de tipos
        Assert.That(_celda.TipoEstrella, Is.EqualTo(1));
        Assert.That(celdaActual.TipoEstrella, Is.EqualTo(0));

        //Comprobar listas
        Assert.That(_gameManager.celdasVacias.Contains(_celda), Is.False);
        Assert.That(_gameManager.celdasVacias.Contains(celdaActual), Is.True);
    }

    //Comprobar que dos estrellas del mismo tipo se fusionan correctamente, es decir:
    // 1) La casilla original pasa a estar vacia (tipo 0), la casilla destino aumenta su tipo en 1
    // 2) La casilla original entra en la lista de celdas vacias
    // !! Comentar llamada al metodo PlaySound y SumarPuntos
    [Test]
    public void TestFusionarEstrellas () {
        //Crear dos celdas del mismo tipo
        List<Celda> celdasVacias = new List<Celda>();
        List<Celda> listaCeldas = new List<Celda>();
        GenerarCelda(_celda);
        _celda.TipoEstrella = 1;
        int tipoEstrellaOriginal = 1;
        _celda.id = 0;
        listaCeldas.Add(_celda);
        _gameManager.celdasVacias = celdasVacias;

        Celda celdaActual = new GameObject().AddComponent<Celda>();
        GenerarCelda(celdaActual);
        celdaActual.id = 1;
        celdaActual.TipoEstrella = 1;
        listaCeldas.Add(celdaActual);
        _gameManager.listaCeldas = listaCeldas;

        _gameManager.Fusionar(1, 0);

        //Comprobar intercambio de tipos
        Assert.That(_celda.TipoEstrella, Is.EqualTo(tipoEstrellaOriginal + 1));
        Assert.That(celdaActual.TipoEstrella, Is.EqualTo(0));

        //Comprobar listas
        Assert.That(_gameManager.celdasVacias.Contains(celdaActual), Is.True);
    }

    /***** Metodos auxiliares *****/
    private void GenerarCelda (Celda celda) {
        celda.gameObject.AddComponent<Image>();
        celda.gameObject.AddComponent<Animator>();
        celda.Awake();

        sprite = Resources.Load("IconosItems/botas") as Sprite;
        celda.sprites = new Sprite[2];
        celda.sprites[0] = sprite;
        celda.sprites[1] = sprite;
    }
}
