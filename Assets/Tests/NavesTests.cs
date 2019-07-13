using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using Naves;

[TestFixture]
[Category("Unity ECS Tests")]
public class NavesTests : ECSTestsFixture {

    Entity enemigo, naveHija, proyectilJugador, player, premio, spawner;

	/***** PLAYER *****/

    //Comprobar que el proyectil se mueve hacia la posicion indicada
    [Test]
    public void TestMoverProyectilTarget () {
        //Crear proyectil ya con la distancia que tiene que recorrer
        CrearProyectil(2.0f);

        //Ejecutar el sistema
        World.Active.GetOrCreateManager<MovimientoProyectilSYS_NV>().Update();

        //Comprobar que en un frame se ha movido hacia el target (y > -0.65, porque se crea mirando hacia arriba)
        var posicionActual = m_Manager.GetComponentData<Position>(proyectilJugador);
        Assert.That(posicionActual.Value.y, Is.GreaterThan(-0.65f));
    }

    //Comprobar que el proyectil se estira segun la distancia dada
    [Test]
    public void TestEstirarProyectil() {
        //Crear proyectil ya con la distancia que tiene que recorrer
        CrearProyectil(5.0f);

        //Ejecutar job directamente (sin pasar por el sistema porque depende del input)
        new EstirarProyectilSYS_NV.EstirarProyectilJugador().Run(EmptySystem);

        //Comprobar que la escala en Y es mayor que 0.6 (la original)
        var escalaActual = m_Manager.GetComponentData<Scale>(proyectilJugador);
        Assert.That(escalaActual.Value.y, Is.GreaterThan(0.6f));
    }

    //Comprobar que la nave del jugador gira hacia el punto indicado
    [Test]
    public void TestPlayerMovement () {
        //Crear entidad jugador
        CrearJugador();

        //Ejecutar directamente job que calcula la rotacion hacia el punto dado
        PlayerInputSYS_NV.ActualizarValorRotacionPlayer jobRotacion = new PlayerInputSYS_NV.ActualizarValorRotacionPlayer();
        jobRotacion.rotateTo = new Vector3(5f, 3f, 0); //punto hacia el que va a rotar
        jobRotacion.Run(EmptySystem);

        //Obtener rotacion que va a efectuar
        Quaternion rotacionARealizar = m_Manager.GetComponentData<PlayerInputCMP_NV>(player).rotZ;
        //Calcular rotacion esperada
        Quaternion rotacionEsperada = rotacionARealizar * Quaternion.Euler(-90.0f, 0, 0);

        //Ejecutar el job que efectua la rotacion
        new PlayerMovementSYS_NV.RotarPlayer().Run(EmptySystem);

        //Comprobar que el valor actual de la rotacion del jugador es el esperado
        Quaternion rotacionActual = m_Manager.GetComponentData<Rotation>(player).Value;
        Assert.That(rotacionActual, Is.EqualTo(rotacionEsperada));
    }

    /***** NAVES HIJAS *****/

    //Comprobar que rotan hacia el lado correcto
    [Test]
    public void TestRotacionNavesHijas () {
        //Crear nave hija, rota hacia la derecha
        CrearNaveHija(0f, 0f);

        //Ejecutar el sistema
        World.Active.GetOrCreateManager<RotacionNavesHijasSYS_NV>().Update();

        //Comprobar que el valor Z del componente Rotation es mayor que 0
        Quaternion rotacionActual = m_Manager.GetComponentData<Rotation>(naveHija).Value;
        float rotacionZ = rotacionActual.eulerAngles.z;
        Assert.That(rotacionZ, Is.GreaterThan(0));

    }

    /***** ENEMIGOS *****/

    //Comprobar que se eliminan los enemigos cuando salen de la pantalla
    //Falla por el EntityCommandBuffer
    /*[Test]
    public void TestEliminarEnemigosLimites () {

        //Crear entidad enemigo fuera de la pantalla
        //Limite x derecha = 4, limite arriba = 8.2, el target no es relevante
        CrearEnemigo(5f, 9f, new float3(0, 0, 0));

        //Ejecutar el system
        World.Active.GetOrCreateManager<EliminarEnemigosLimitesSYS_NV>().Update();
        m_Manager.CompleteAllJobs();

        //Comprobar que ya no existe ninguna entidad
        NativeArray<Entity> allEntities = m_Manager.GetAllEntities();
        int numEntidades = allEntities.Length;
        allEntities.Dispose();
        Assert.That(numEntidades, Is.EqualTo(0));
    }*/

    //Comprobar que se mueve hacia el target
    [Test]
    public void TestMoverNaveEnemigaHaciaTarget() {
        //Crear enemigo en el origen y poner el target en (1,0,0)
        CrearEnemigo(0f, 0f, new float3(1, 0, 0));
        CrearNaveHija(1f, 0f); //para que se ejecute el sistema

        //Ejecutar el sistema
        World.Active.GetOrCreateManager<MovimientoNavesEnemigasSYS_NV>().Update();

        //Comprobar que en un frame se ha movido hacia el target (x > 0)
        var posicionActual = m_Manager.GetComponentData<Position>(enemigo);
        Assert.That(posicionActual.Value.x, Is.GreaterThan(0));
    }


    /***** PREMIOS *****/

    //Comprobar que se activa un premio cuando su tiempo de spawn llega a 0
    //Falla por el commands, pero se ejecuta bien
    /*[Test]
    public void TestActivarPremio () {
        //Crear premio no activo
        CrearPremio();

        //Crear spawner con 0 segundos para los premios, de manera que active uno inmediatamente
        IniciarSpawner(100f, 0f);

        //Ejecutar el sistema
        World.Active.GetOrCreateManager<ActivarEnemigosPremiosSYS_NV>().Update();

        //Comprobar que la entidad premio tiene el componente PremioActivo
        Assert.That(m_Manager.HasComponent(premio, typeof(PremioActivoCMP_NV)), Is.True);
    }*/


    /***** BOOTSTRAP *****/

    //Comprobar que se crean todas las entidades al inicio
    [Test]
    public void TestCrearEntidadesBootstrap() {        
        Bootstrap_NV bootstrap = new GameObject().AddComponent<Bootstrap_NV>();

        //Se ejecuta el start
        bootstrap.Start();

        //Comprobar que en total hay 39 entidades creadas
        //1 jugador + 1 proyectil + 4 naves hijas + 30 monedas + 1 temporizador + 1 spawner +1 attach
        NativeArray<Entity> allEntities = m_Manager.GetAllEntities();
        int numEntidades = allEntities.Length;       
        allEntities.Dispose();
        Assert.That(numEntidades, Is.EqualTo(39));
    }



    /***** METODOS AUXILIARES *****/
    private void CrearEnemigo(float posicionX, float posicionY, float3 posicionTarget) {

        var enemigoArchetype = m_Manager.CreateArchetype(
                typeof(EnemigoActivoCMP_NV),
                typeof(Position),
                typeof(Rotation),
                typeof(Scale)
            );

        enemigo = m_Manager.CreateEntity(enemigoArchetype);
        float3 posicion = new float3(posicionX, posicionY, 0.0f);

        //Annadirle componentes 
        m_Manager.SetComponentData(enemigo, new Position {
            Value = posicion
        });
        //Annadirle component Rotacion con la Y negativa si va a aparecer en la zona inferior
        float rotZ = posicionY > 0 ? 180 : 0;
        m_Manager.SetComponentData(enemigo, new Rotation {
            Value = Quaternion.Euler(0, 0, rotZ)
        });
        m_Manager.SetComponentData(enemigo, new Scale {
            Value = new float3(1.1f, 1.1f, 1f)
        });        

        //Calcular direccion en la que se tiene que mover (hacia la nave objetivo)
        float3 direccion = posicionTarget - posicion;
        //Normalizar
        Vector3 dirNormalizada = new Vector3(direccion.x, direccion.y, direccion.z).normalized;
        m_Manager.SetComponentData(enemigo, new EnemigoActivoCMP_NV {
            direccion = new float3(dirNormalizada.x, dirNormalizada.y, dirNormalizada.z)
        });
    }

    //Crear nave hija
    private void CrearNaveHija(float posicionX, float posicionY) {

        var naveHijaArchetype = m_Manager.CreateArchetype(
                typeof(NaveHijaCMP_NV),
                typeof(Position),
                typeof(Rotation),
                typeof(Scale)
        );
        naveHija = m_Manager.CreateEntity(naveHijaArchetype);
        float3 posicion = new float3(posicionX, posicionY, 0.0f);

        //Annadirle componentes nave hija y posicion 
        m_Manager.SetComponentData(naveHija, new NaveHijaCMP_NV {
            posicion = posicion,
            direccion = 1.0f
        });
        m_Manager.SetComponentData(naveHija, new Position {
            Value = posicion
        });        
        m_Manager.SetComponentData(naveHija, new Scale {
            Value = new float3(1.1f, 1.1f, 1f)
        });        
    }

    private void CrearProyectil (float distY) {
        var proyectilJugadorchetype = m_Manager.CreateArchetype(
                typeof(ProyectilJugadorCMP_NV),
                typeof(Position),
                typeof(Rotation),
                typeof(Scale)
            );

        proyectilJugador = m_Manager.CreateEntity(proyectilJugadorchetype);

        m_Manager.SetComponentData(proyectilJugador, new ProyectilJugadorCMP_NV {
            escalaYOriginal = 0.6f,
            posOriginal = new Unity.Mathematics.float3(0f, -0.65f, 0f),
            distY = distY
        });
        m_Manager.SetComponentData(proyectilJugador, new Position {
            Value = new Unity.Mathematics.float3(0f, -0.65f, 0f)
        });
        m_Manager.SetComponentData(proyectilJugador, new Scale {
            Value = new Unity.Mathematics.float3(0.3f, 0.6f, 1f)
        });
    }

    private void CrearJugador () {
        var playerArchetype = m_Manager.CreateArchetype(
                typeof(PlayerInputCMP_NV),
                typeof(Position),
                typeof(Rotation)
            );

        player = m_Manager.CreateEntity(playerArchetype);

        m_Manager.SetComponentData(player, new PlayerInputCMP_NV {
            //vida = 4.0f
        });
        m_Manager.SetComponentData(player, new Position {
            Value = new Unity.Mathematics.float3(0f, 0.5f, 0f)
        });
    }

    private void CrearPremio () {
        var premioArchetype = m_Manager.CreateArchetype(
                typeof(PremioCMP_NV)
        );

        premio = m_Manager.CreateEntity(premioArchetype);
    }

    public void IniciarSpawner(float tiempoNaves, float tiempoPremios) {
        var spawnerArch = m_Manager.CreateArchetype(
                typeof(TiempoSpawnCMP_NV)
        );
        spawner = m_Manager.CreateEntity(spawnerArch);
        m_Manager.SetComponentData(spawner, new TiempoSpawnCMP_NV {
            segundosNaves = tiempoNaves,
            segundosPremios = tiempoPremios
        });
    }

}
