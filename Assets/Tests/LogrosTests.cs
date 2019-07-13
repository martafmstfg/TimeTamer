using System;
using UnityEngine;
using NUnit.Framework;
using Unity.Entities.Tests;
using UnityEngine.UI;
using Unity.Entities;

[TestFixture]
[Category("Unity ECS Tests")]
public class LogrosTests : ECSTestsFixture {

    [SetUp]
    public void BeforeEveryTest() {
        //Necesita el entity Manager y el arquetipo
        LogrosManager.entityManager = m_Manager;
        LogrosManager.logroArchetype = m_Manager.CreateArchetype(ComponentType.Create<LogroComponent>());
    }

    [TearDown]
    public void Cleanup() {
    }

    //LOGROS MANAGER

    //Comprobar que el metodo CrearEntidadLogro crea una nueva entidad con los parametros que se le pasan
    [Test]
    public void CrearEntidadLogroTest() {

        //Numero original de entidades
        int n_entidades_prev = m_Manager.GetAllEntities().Length;

        //Llamar al metodo
        Entity creada = LogrosManager.CrearEntidadLogro(0, 5, 8, 1);
        //Numero de entidades actualizado
        int n_entidades = m_Manager.GetAllEntities().Length;

        //Comprobar que hay una entidad mas
        Assert.That(n_entidades, Is.EqualTo(n_entidades_prev + 1));
        //Comprobar que existe la entidad que se acaba de crear 
        Assert.That(m_Manager.Exists(creada), Is.True);

        //Comprobar que esa entidad tiene los datos correctos
        LogroComponent componenteLogro = m_Manager.GetComponentData<LogroComponent>(creada);
        Assert.That(componenteLogro.id, Is.EqualTo(0));
        Assert.That(componenteLogro.requisitoBase, Is.EqualTo(5));
        Assert.That(componenteLogro.recompensaBase, Is.EqualTo(8));
    }

    //Comprobar que se crean las 8 entidades de los logros al inicio de la escena y que se annaden a la lista
    [Test]
    public void CrearTodasEntidadesLogros() {
        LogrosManager.InitializeAfterSceneLoad();

        //Comprobar que hay 5 entidades
        int n_entidades = m_Manager.GetAllEntities().Length;
        Assert.That(n_entidades, Is.EqualTo(8));
        Assert.That(LogrosManager.entidadesLogros.Count, Is.EqualTo(8));
    }

    /***************************************************************************************/

    //COMPROBARLOGROSYSTEM

    //Metodo auxiliar para crear un logro
    private Entity CrearLogroAux (int id, int requisitoBase, int recompensaBase, int nivel, bool comprobar) {

        Entity logro;

        if (comprobar) {
            //Crear una nueva entidad de logro
             logro = m_Manager.CreateEntity(typeof(LogroComponent), typeof(ComprobarLogroComponent));
            //Dar valores a los componentes
            m_Manager.SetComponentData(logro, new LogroComponent { id = id, requisitoBase = requisitoBase,
                recompensaBase = recompensaBase, nivel = nivel});
        } else {
            //Crear una nueva entidad de logro
            logro = m_Manager.CreateEntity(typeof(LogroComponent));
            //Dar valores a los componentes
            m_Manager.SetComponentData(logro, new LogroComponent {
                id = id,
                requisitoBase = requisitoBase,
                recompensaBase = recompensaBase,
                nivel = nivel
            });
        }

        return logro;
    }

    //Comprobar que no se obtiene recompensa ni se sube de nivel cuando el logro no ha sido completado
    [Test]
    public void LogroNoCompletado () {
        //Crear una nueva entidad de logro
        Entity logro = CrearLogroAux(8, 3, 5, 1, true);

        //Resetear playerprefab del test
        PlayerPrefs.SetInt("Nivel8", 1);
        PlayerPrefs.SetFloat("Test", 0.0f);

        //Numero de monedas antes de comprobar el logro
        int monedas_prev = PlayerPrefs.GetInt("Monedas");

        //Ejecutar sistema
        World.CreateManager<ComprobarLogroSystem>().Update();

        //Comprobar que el numero de monedas es el mismo porque no se ha obtenido recompensa
        Assert.That(PlayerPrefs.GetInt("Monedas"), Is.EqualTo(monedas_prev));
        //Comprobar que tampoco se ha subido de nivel
        LogroComponent componenteLogro = m_Manager.GetComponentData<LogroComponent>(logro);
        Assert.That(componenteLogro.nivel, Is.EqualTo(1));
    }

    //Comprobar que se obtiene recompensa y se sube de nivel cuando el logro es completado
    [Test]
    public void LogroCompletado() {
        //Crear una nueva entidad de logro
        Entity logro = CrearLogroAux(8, 3, 5, 1, true);

        //Guardar pprefs para que el requisito se cumpla
        PlayerPrefs.SetInt("Nivel8", 1);
        PlayerPrefs.SetFloat("Test", 3.0f);

        //Numero de monedas antes de comprobar el logro
        int monedas_prev = PlayerPrefs.GetInt("Monedas");

        //Ejecutar sistema
        World.CreateManager<ComprobarLogroSystem>().Update();

        //Comprobar que el numero de monedas es el mismo porque no se ha obtenido recompensa
        Assert.That(PlayerPrefs.GetInt("Monedas"), Is.EqualTo(monedas_prev+5));
        //Comprobar que se ha subido de nivel        
        Assert.That(m_Manager.GetComponentData<LogroComponent>(logro).nivel, Is.EqualTo(2));
    }

    //Comprobar que, al iniciar la app, se compara la fecha actual con la ultima guardada y, si solo hay
    // un dia de diferencia, se incrementa el numero de dias seguidos en pprefs
    // !! Para que funcione hay que comentar la llamada a ComprobarLogroCompletado del metodo ComprobarDiasSeguidos
    [Test]
    public void DiasSeguidos () {
        //Guardar ultima fecha
        PlayerPrefs.SetString("UltimoDia", "26/01/2019 12:15:05");
        //Fecha de "hoy"
        DateTime hoy = Convert.ToDateTime("27/01/2019 17:30:15");
        //Numero de dias seguidos antes de hacer la comprobacion
        float diasSeguidos_prev = PlayerPrefs.GetFloat("DiasSeguidos");

        //Lamar al metodo de LogrosManager pasandole la fecha de "hoy"
        LogrosManager.InitializeAfterSceneLoad();
        ComprobacionesDiarias.ComprobarDiasSeguidos(hoy);

        //Nuevo numero de dias seguidos
        float diasSeguidos_nuevo = PlayerPrefs.GetFloat("DiasSeguidos");

        //Comprobar que el numero de dias seguidos se ha incrementado en 1
        Assert.That(diasSeguidos_nuevo, Is.EqualTo(diasSeguidos_prev + 1));
    }

    //Comprobar que se recibe la recompensa al cumplir el logro de dias seguidos
    //Esto implica comprobar que el metodo ComprobarLogroCompletado de LogrosManager funciona correctamente
    // (es decir, que annade el componente ComprobarLogroComponent para que el ComprobarLogroSystem procese la entidad)
    [Test]
    public void LogroDiasSeguidosCompletado () {
        //Llamar al metodo inicial del LogrosManager para que cree las entidades de los logros y las annada al array
        // !!! descomentar la llamada a ComprobarDiasSeguidos 
        LogrosManager.InitializeAfterSceneLoad();

        //Guardar ultima fecha
        PlayerPrefs.SetString("UltimoDia", "26/01/2019 12:15:05");
        //Fecha de "hoy"
        DateTime hoy = Convert.ToDateTime("27/01/2019 17:30:15");

        //Poner numero de dias seguidos actuales a 2 para que se cumpla el primer nivel del logro
        PlayerPrefs.SetInt("Nivel0", 1);
        PlayerPrefs.SetFloat("DiasSeguidos", 2.0f);

        //Cantidad de monedas antes de comprobar el logro
        int monedas_prev = PlayerPrefs.GetInt("Monedas");

        //Lamar al metodo de LogrosManager pasandole la fecha de "hoy"
        ComprobacionesDiarias.ComprobarDiasSeguidos(hoy);

        //Cantidad de monedas despues de comprobar el logro
        int monedas_nuevo = PlayerPrefs.GetInt("Monedas");

        //Comprobar que el numero de monedas se ha incrementado al recibir la recompensa
        Assert.That(monedas_nuevo, Is.GreaterThan(monedas_prev));
    }

    //Comprobar que, al iniciar la app, se compara la fecha actual con la ultima guardada y, si hay
    // mas de dia de diferencia, se resetea el numero de dias seguidos en pprefs
    [Test]
    public void DiasNoSeguidos () {
        //Guardar ultima fecha
        PlayerPrefs.SetString("UltimoDia", "26/01/2019 12:15:05");
        //Fecha de "hoy"
        DateTime hoy = Convert.ToDateTime("29/01/2019 17:30:15");
        
        //Lamar al metodo de LogrosManager pasandole la fecha de "hoy"
        ComprobacionesDiarias.ComprobarDiasSeguidos(hoy);

        //Nuevo numero de dias seguidos
        int diasSeguidos_nuevo = PlayerPrefs.GetInt("DiasSeguidos");

        //Comprobar que el numero de dias seguidos se ha reiniciado
        Assert.That(diasSeguidos_nuevo, Is.EqualTo(0));
    }

    //Comprobar que, si la diferencia es menor que de un dia (horas), no se altera el numero de dias seguidos
    [Test]
    public void DiferenciaDeHoras() {
        //Guardar ultima fecha
        PlayerPrefs.SetString("UltimoDia", "26/01/2019 12:15:05");
        //Fecha de "hoy"
        DateTime hoy = Convert.ToDateTime("26/01/2019 17:30:15");

        //Numero de dias seguidos original
        int diasSeguidos_prev = PlayerPrefs.GetInt("DiasSeguidos");

        //Lamar al metodo de LogrosManager pasandole la fecha de "hoy"
        ComprobacionesDiarias.ComprobarDiasSeguidos(hoy);

        //Nuevo numero de dias seguidos
        int diasSeguidos_nuevo = PlayerPrefs.GetInt("DiasSeguidos");

        //Comprobar que el numero de dias seguidos se ha reiniciado
        Assert.That(diasSeguidos_nuevo, Is.EqualTo(diasSeguidos_prev));
    }
}
