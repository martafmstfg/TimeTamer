using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
[Category("Unity ECS Tests")]
public class SushiTests : ECSTestsFixture {

    Bootstrap bootstrap;
    Entity playerEntity, obstaculo, premio, mano;

    [SetUp]
    public void BeforeEveryTest() {

        //bootstrap = new GameObject().AddComponent<Bootstrap>();  
    }

    /***** PLAYER *****/

    //Comprobar que el sushi se mueve al carril derecho desde el central
    [Test]
    public void TestMoverJugadorDerechaDesdeCentro () {

        CrearSushi();

        //Estando en el carril central no hay que cambiar la posicion respecto a la original
        m_Manager.SetComponentData<PlayerInputComponent>(playerEntity, new PlayerInputComponent {
            posicionFinal = new Unity.Mathematics.float3(0f, -2.8f, 0f),
            swipeLeft = 0,
            swipeRight = 1
        });

        //Ejecutar sistema
        World.Active.GetOrCreateManager<PlayerMovementSystem>().Update();

        //Comprobar que el sushi se ha movido a la derecha (por el MoveTowards, en un solo frame llega al carril derecho)
        var posicionActual = m_Manager.GetComponentData<Position>(playerEntity);
        Assert.That(posicionActual.Value.x, Is.GreaterThan(0));
        //Y que la posicion final en X a la que tiene que ir corresponde a la del carril derecho (1.53)
        var posicionFinal = m_Manager.GetComponentData<PlayerInputComponent>(playerEntity).posicionFinal;
        Assert.That(posicionFinal.x, Is.EqualTo(1.53f));
        
    }

    //Comprobar que el sushi se mueve al carril central desde el izquierdo
    [Test]
    public void TestMoverJugadorDerechaDesdeIzquierda() {

        CrearSushi();
        
        m_Manager.SetComponentData<PlayerInputComponent>(playerEntity, new PlayerInputComponent {
            posicionFinal = new Unity.Mathematics.float3(0f, -2.8f, 0f),
            swipeLeft = 0,
            swipeRight = 1
        });
        //Posicion = carril izquierdo
        m_Manager.SetComponentData(playerEntity, new Position {
            Value = new Unity.Mathematics.float3(-1.53f, -2.8f, 0f)
        });

        //Ejecutar sistema
        World.Active.GetOrCreateManager<PlayerMovementSystem>().Update();

        //Comprobar que el sushi se ha movido a la derecha (por el MoveTowards, en un solo frame llega al carril derecho)
        var posicionActual = m_Manager.GetComponentData<Position>(playerEntity);
        Assert.That(posicionActual.Value.x, Is.GreaterThan(-1.53f));
        //Y que la posicion final en X a la que tiene que ir corresponde a la del carril central (0)
        var posicionFinal = m_Manager.GetComponentData<PlayerInputComponent>(playerEntity).posicionFinal;
        Assert.That(posicionFinal.x, Is.EqualTo(0));
    }

    //Comprobar que el sushi se mueve al carril izquierdo desde el central
    [Test]
    public void TestMoverJugadorIzquierdaDesdeCentro() {

        CrearSushi();

        //Estando en el carril central no hay que cambiar la posicion respecto a la original
        m_Manager.SetComponentData<PlayerInputComponent>(playerEntity, new PlayerInputComponent {
            posicionFinal = new Unity.Mathematics.float3(0f, -2.8f, 0f),
            swipeLeft = 1,
            swipeRight = 0
        });

        //Ejecutar sistema
        World.Active.GetOrCreateManager<PlayerMovementSystem>().Update();

        //Comprobar que el sushi se ha movido a la izquierda (por el MoveTowards, en un solo frame llega al carril derecho)
        var posicionActual = m_Manager.GetComponentData<Position>(playerEntity);
        Assert.That(posicionActual.Value.x, Is.LessThan(0));
        //Y que la posicion final en X a la que tiene que ir corresponde a la del carril izquierdo (-1.53)
        var posicionFinal = m_Manager.GetComponentData<PlayerInputComponent>(playerEntity).posicionFinal;
        Assert.That(posicionFinal.x, Is.EqualTo(-1.53f));
    }

    //Comprobar que el sushi se mueve al carril central desde el derecho
    [Test]
    public void TestMoverJugadorIzquierdaDesdeDerecha() {

        CrearSushi();

        m_Manager.SetComponentData<PlayerInputComponent>(playerEntity, new PlayerInputComponent {
            posicionFinal = new Unity.Mathematics.float3(0f, -2.8f, 0f),
            swipeLeft = 1,
            swipeRight = 0
        });
        //Posicion = carril derecho
        m_Manager.SetComponentData(playerEntity, new Position {
            Value = new Unity.Mathematics.float3(1.53f, -2.8f, 0f)
        });

        //Ejecutar sistema
        World.Active.GetOrCreateManager<PlayerMovementSystem>().Update();

        //Comprobar que el sushi se ha movido a la izquierda (por el MoveTowards, en un solo frame llega al carril derecho)
        var posicionActual = m_Manager.GetComponentData<Position>(playerEntity);
        Assert.That(posicionActual.Value.x, Is.LessThan(1.53f));
        //Y que la posicion final en X a la que tiene que ir corresponde a la del carril central (0)
        var posicionFinal = m_Manager.GetComponentData<PlayerInputComponent>(playerEntity).posicionFinal;
        Assert.That(posicionFinal.x, Is.EqualTo(0));
    }

    //Comprobar que NO se mueve mas a la derecha si ya esta en el carril derecho
    [Test]
    public void TestNoMoverDerecha() {

        CrearSushi();

        m_Manager.SetComponentData<PlayerInputComponent>(playerEntity, new PlayerInputComponent {
            posicionFinal = new Unity.Mathematics.float3(1.53f, -2.8f, 0f),
            swipeLeft = 0,
            swipeRight = 1
        });
        //Posicion = carril derecho
        m_Manager.SetComponentData(playerEntity, new Position {
            Value = new Unity.Mathematics.float3(1.53f, -2.8f, 0f)
        });

        //Ejecutar sistema
        World.Active.GetOrCreateManager<PlayerMovementSystem>().Update();

        //Comprobar que el sushi sigue en el carril derecho
        var posicionActual = m_Manager.GetComponentData<Position>(playerEntity);
        Assert.That(posicionActual.Value.x, Is.EqualTo(1.53f));        
    }

    //Comprobar que NO se mueve mas a la izquierda si ya esta en el carril izquierdo
    [Test]
    public void TestNoMoverIzquierda() {

        CrearSushi();

        m_Manager.SetComponentData<PlayerInputComponent>(playerEntity, new PlayerInputComponent {
            posicionFinal = new Unity.Mathematics.float3(-1.53f, -2.8f, 0f),
            swipeLeft = 1,
            swipeRight = 0
        });
        //Posicion = carril izquierdo
        m_Manager.SetComponentData(playerEntity, new Position {
            Value = new Unity.Mathematics.float3(-1.53f, -2.8f, 0f)
        });

        //Ejecutar sistema
        World.Active.GetOrCreateManager<PlayerMovementSystem>().Update();

        //Comprobar que el sushi sigue en el carril izquierdo
        var posicionActual = m_Manager.GetComponentData<Position>(playerEntity);
        Assert.That(posicionActual.Value.x, Is.EqualTo(-1.53f));
    }

    /***** OBSTACULOS *****/

    //Comprobar que el obstaculo activo se mueve hacia abajo
    [Test]
    public void TestMovimientoObstaculoActivo () {
        CrearTemporizador();    
        //Crear un obstaculo en el carril central 
        CrearObstaculo(0.0f, 6.0f, true);

        //Ejecutar sistema
        World.Active.GetOrCreateManager<MoverObstaculosPremios>().Update();

        //Comprobar que en un frame se ha movido hacia abajo (por lo que y < 6.0f)
        var posicionActual = m_Manager.GetComponentData<Position>(obstaculo);
        Assert.That(posicionActual.Value.y, Is.LessThan(6.0f));
    }

    //Comprobar que un obstaculo NO activo NO se mueve
    [Test]
    public void TestObstaculoInactivoNoMovimiento() {
        CrearTemporizador();
        //Crear un obstaculo en el carril central 
        CrearObstaculo(0.0f, 6.0f, false);

        //Ejecutar sistema
        World.Active.GetOrCreateManager<MoverObstaculosPremios>().Update();

        //Comprobar que no se ha movido porque no tiene el componente ObstaculoActivo
        var posicionActual = m_Manager.GetComponentData<Position>(obstaculo);
        Assert.That(posicionActual.Value.y, Is.EqualTo(6.0f));
    }

    //Comprobar que, cuando el jugador choca con un obstaculo, el jugador es destruido
    /*[Test]
    public void TestColisionObstaculo() {
        CrearTemporizador();
        CrearSushi();
        CrearObstaculo(0.0f, -2.8f, true); //crear obstaculo en la misma posicion que el jugador

        //Ejecutar sistema
        World.Active.GetOrCreateManager<ColisionObstaculo>().Update();
        m_Manager.CompleteAllJobs();

        //La entidad del jugador ya no existe
        Assert.That(m_Manager.Exists(playerEntity), Is.False);
    }*/

    /***** PREMIOS *****/

    //Comprobar que el premio activo se mueve hacia abajo
    [Test]
    public void TestMovimientoPremioActivo() {
        CrearTemporizador();
        //Crear un obstaculo en el carril central 
        CrearPremio(0.0f, 6.0f, true);

        //Ejecutar sistema
        World.Active.GetOrCreateManager<MoverObstaculosPremios>().Update();

        //Comprobar que en un frame se ha movido hacia abajo (por lo que y < 6.0f)
        var posicionActual = m_Manager.GetComponentData<Position>(premio);
        Assert.That(posicionActual.Value.y, Is.LessThan(6.0f));
    }

    //Comprobar que un premio NO activo NO se mueve
    [Test]
    public void TestPremioInactivoNoMovimiento() {
        CrearTemporizador();
        //Crear un obstaculo en el carril central 
        CrearPremio(0.0f, 6.0f, false);

        //Ejecutar sistema
        World.Active.GetOrCreateManager<MoverObstaculosPremios>().Update();

        //Comprobar que no se ha movido porque no tiene el componente ObstaculoActivo
        var posicionActual = m_Manager.GetComponentData<Position>(premio);
        Assert.That(posicionActual.Value.y, Is.EqualTo(6.0f));
    }

    //Comprobar que, cuando el jugador coge un premio, a este se le quitan los componentes y se suman monedas
    //Comentar llamada al bootstrap
    /*[Test]
    public void TestColisionPremio () {
        CrearTemporizador();
        CrearSushi();
        CrearPremio(0.0f, -2.8f, true); //crear premio en la misma posicion que el jugador

        //Ejecutar sistema
        World.Active.GetOrCreateManager<ColisionPremio>().Update();
        m_Manager.CompleteAllJobs();        

        //La entidad del premio ya no tiene el componente PremioActivo ni la posicion
        bool componentePremio = m_Manager.HasComponent(premio, typeof(PremioActivoComponent));
        Assert.That(componentePremio, Is.False);
        bool componentePosicion = m_Manager.HasComponent(premio, typeof(Position));
        Assert.That(componentePosicion, Is.False);
    }*/

    /***** OTROS *****/

    //Comprobar que el bootstrap crea todas las entidades
    [Test]
    public void TestCrearEntidadesBootstrap () {
        //Crear objeto bootstrap y los GO que necesita
        GameObjectsBootstrap();
        bootstrap = new GameObject().AddComponent<Bootstrap>();

        //Numero de entidsdes existentes antes de ejecutar el start
        NativeArray<Entity> allEntities = m_Manager.GetAllEntities();
        int numEntidadesPrev = allEntities.Length;

        //Se ejecuta el start
        bootstrap.Start();

        //Comprobar que en total hay 56 entidades MAS creadas
        //1 jugador + 30 obstaculos + 20 premios + 2 manos + 1 temporizador + 2 spawners
        allEntities = m_Manager.GetAllEntities();
        int numEntidadesPost = allEntities.Length;        
        allEntities.Dispose();
        Assert.That(numEntidadesPost, Is.EqualTo(numEntidadesPrev + 56));       
    }

    //Comprobar que al final del juego se eliminan todas las entidades
    //Comentar llamadas a metodos del bootstrap
    [Test]
    public void TestEliminarEntidadesFinJuego() {
        //Crear objeto bootstrap y los GO que necesita
        GameObjectsBootstrap();
        bootstrap = new GameObject().AddComponent<Bootstrap>();
        bootstrap.Start();

        //Crear entidad de fin del juego
        var finJuegoArch = m_Manager.CreateArchetype(typeof(FinJuegoComponent));
        var finJuegoEntity = m_Manager.CreateEntity(finJuegoArch);


        //Ejecutar update del sistema
        World.Active.GetOrCreateManager<FinJuegoSystem>().Update();

        //Comprobar que se han elimnado las entidades
        NativeArray<Entity> allEntities = m_Manager.GetAllEntities();
        int numEntidades = allEntities.Length;
        allEntities.Dispose();
        Assert.That(numEntidades, Is.EqualTo(0));
    }


    /***** METODOS AUXILIARES *****/

    private void CrearSushi () {
        var playerArchetype = m_Manager.CreateArchetype(
            typeof(Position),
            typeof(Scale),
            typeof(PlayerInputComponent)
        );

        playerEntity = m_Manager.CreateEntity(playerArchetype);
        
        m_Manager.SetComponentData(playerEntity, new Position {
            Value = new Unity.Mathematics.float3(0f, -2.8f, 0f)
        });
        m_Manager.SetComponentData(playerEntity, new Scale {
            Value = new Unity.Mathematics.float3(1f, 1f, 1f)
        });
        m_Manager.SetComponentData(playerEntity, new PlayerInputComponent {
            posicionFinal = new Unity.Mathematics.float3(0f, -2.8f, 0f)
        });
    }

    private void CrearObstaculo (float posicionX, float posicionY, bool activo) {

        var obstaculoArchetype = m_Manager.CreateArchetype(
            typeof(ObstaculoComponent),
            typeof(Position)
        );

        obstaculo = m_Manager.CreateEntity(obstaculoArchetype);

        m_Manager.SetComponentData(obstaculo, new Position {
            Value = new float3(posicionX, posicionY, 0.0f)
        });

        if(activo) {
            m_Manager.AddComponent(obstaculo, typeof(ObstaculoActivoComponent));
        }
    }

    private void CrearPremio(float posicionX, float posicionY, bool activo) {

        var premioArchetype = m_Manager.CreateArchetype(
            typeof(PremioComponent),
            typeof(Position)
        );

        premio = m_Manager.CreateEntity(premioArchetype);

        m_Manager.SetComponentData(premio, new Position {
            Value = new float3(posicionX, posicionY, 0.0f)
        });

        if (activo) {
            m_Manager.AddComponent(premio, typeof(PremioActivoComponent));
        }
    }

    private void CrearTemporizador () {
        var temporizadorArchetype = m_Manager.CreateArchetype(
            typeof(TemporizadorComponent)
        );
        var temporizador = m_Manager.CreateEntity(temporizadorArchetype);
        m_Manager.SetComponentData(temporizador, new TemporizadorComponent {
            startTime = Time.time
        });
    }

    private void GameObjectsBootstrap () {

        new GameObject("Monedas").AddComponent<Text>();
        new GameObject("Tiempo").AddComponent<Text>();
        new GameObject("Canvas");
        new GameObject("Audio Source").AddComponent<AudioSource>();
        new GameObject("Musica").AddComponent<AudioSource>();

    }

    //Necesario para poder crear entidades desde el job
    private class CollisionBarrier : BarrierSystem {
    }
    [Inject] private CollisionBarrier _collisionBarrier;
}
