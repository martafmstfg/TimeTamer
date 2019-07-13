using Zenject;
using NUnit.Framework;
using Moq;
using Estadisticas_DB;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class EstadisticasTests : ZenjectUnitTestFixture
{
    private Mock<IEstadisticasDB> _estadisticasDB;
    private EstadisticasManager _estadisticasManager;

    [SetUp]
    public void BeforeEveryTest () {
        _estadisticasDB = new Mock<IEstadisticasDB>();

        Container.BindInstance(_estadisticasDB.Object);
        Container.Bind<EstadisticasManager>().FromNewComponentOnNewGameObject().AsSingle();

        _estadisticasManager = Container.Resolve<EstadisticasManager>();

        CrearObjetos();
    }

    //Comprobar que el tiempo que se muestra en cada categoria, el total y el fill de las barras son los correctos
    [Test]
    public void TestEstadisticasSemana()
    {
        //Los años y semanas actuales no son relevantes
        _estadisticasManager.anno = _estadisticasManager.annoActual = 2019;
        _estadisticasManager.semana = _estadisticasManager.semanaActual = 
            _estadisticasManager.primeraSemana = _estadisticasManager.ultimaSemana = 1;    

        MockTiempoTotalActividad(1, 2019); //Mocks para ObtenerTiempoTotal
        MockPrimerDia(1, 2019); //La semana y el año no son importantes
        MockUltimoDia(1, 2019); //La semana y el año no son importantes
        MockUltimaSemana(2019, -1); //-1 para que no pueda seguir retrocediendo

        _estadisticasManager.MostrarEstadisticasSemana();

        //Comprobar que los tiempos puestos en los GO de cada actividad son los correctos
        // (los mismos que se han puesto en MockTiempoTotalActividad)
        Assert.That(GameObject.Find("MinutosEstudio").GetComponent<Text>().text, Is.EqualTo("1min"));
        Assert.That(GameObject.Find("MinutosEjercicio").GetComponent<Text>().text, Is.EqualTo("1min"));
        Assert.That(GameObject.Find("MinutosHogar").GetComponent<Text>().text, Is.EqualTo("1min"));
        Assert.That(GameObject.Find("MinutosOcio").GetComponent<Text>().text, Is.EqualTo("1min"));
        Assert.That(GameObject.Find("MinutosOtros").GetComponent<Text>().text, Is.EqualTo("1min"));
        Assert.That(GameObject.Find("total").GetComponent<Text>().text, Is.EqualTo("Total: 5min"));

        //Comprobar que el fill de las barras es el correcto
        Assert.That(GameObject.Find("FillEstudio").GetComponent<Image>().fillAmount, Is.EqualTo(1.0f/5.0f));
        Assert.That(GameObject.Find("FillEjercicio").GetComponent<Image>().fillAmount, Is.EqualTo(1.0f / 5.0f));
        Assert.That(GameObject.Find("FillHogar").GetComponent<Image>().fillAmount, Is.EqualTo(1.0f / 5.0f));
        Assert.That(GameObject.Find("FillOcio").GetComponent<Image>().fillAmount, Is.EqualTo(1.0f / 5.0f));
        Assert.That(GameObject.Find("FillOtros").GetComponent<Image>().fillAmount, Is.EqualTo(1.0f / 5.0f));

        //Comprobar que en el texto Fecha pone las fechas devueltas por los mocks metodos de la db
        Assert.That(GameObject.Find("fecha").GetComponent<Text>().text, Is.EqualTo("17/01/18 - 23/01/18"));
    }

    //Comprobar que se cambia de año al avanzar de semana si ya se esta en la ultima semana 
    [Test]
    public void TestCambiarAnnoUltimaSemana () {

        _estadisticasManager.anno = 2018; //Año cuyas estadisticas se estan mostrando en el momento
        _estadisticasManager.annoActual = 2019; //año real
        //La semana actual y la ultima guardada son la misma para que se avance al año siguiente
        _estadisticasManager.semana = _estadisticasManager.semanaActual =
            _estadisticasManager.primeraSemana = _estadisticasManager.ultimaSemana = 1;

        //Mocks necesarios porque el metodo AvanzarSemana llama a MostrarEstadisticasSemana
        MockTiempoTotalActividad(1, 2018); //Mocks para ObtenerTiempoTotal
        MockPrimerDia(1, 2018); //La semana y el año no son importantes
        MockUltimoDia(1, 2018); //La semana y el año no son importantes
        MockUltimaSemana(2018, 3); //La ultima semana almacenada del año siguiente es la semana 3

        //Mock para obtener el siguiente año almacenados
        MockSiguienteAnno(2018);

        //Mock para el metodo ObtenerPrimeraSemana
        MockPrimeraSemana(2019, 2); //La primera semana almacenada del año siguiente es la semana 2

        _estadisticasManager.AvanzarSemana();

        //Comprobar que se ha pasado al siguiente año (anno + 1)
        Assert.That(_estadisticasManager.anno, Is.EqualTo(2019));
        //Comprobar que se han actualizado la primera y la ultima semana almacenadas para el nuevo año
        Assert.That(_estadisticasManager.primeraSemana, Is.EqualTo(2));
        Assert.That(_estadisticasManager.ultimaSemana, Is.EqualTo(3));
    }

    //Comprobar que al avanzar de semana no se cambia de año si no se esta en la ultima semana almacenada
    [Test]
    public void TestAvanzarSemanaMismoAnno() {

        _estadisticasManager.anno = 2018; //Año cuyas estadisticas se estan mostrando en el momento
        _estadisticasManager.annoActual = 2019; //año real
        //La semana actual y la ultima guardada NO son la misma, se puede seguir avanzando
        _estadisticasManager.semana = _estadisticasManager.semanaActual = 1;
        _estadisticasManager.primeraSemana = 1;
        _estadisticasManager.ultimaSemana = 3;

        //Mocks necesarios porque el metodo AvanzarSemana llama a MostrarEstadisticasSemana
        MockTiempoTotalActividad(1, 2018); //Mocks para ObtenerTiempoTotal
        MockPrimerDia(1, 2018); //La semana y el año no son importantes
        MockUltimoDia(1, 2018); //La semana y el año no son importantes
        MockUltimaSemana(2019, 3); //La ultima semana almacenada del año siguiente es la semana 3       

        //Mock para obtener la siguiente semana almacenada
        MockSiguienteSemana(1);

        _estadisticasManager.AvanzarSemana();

        //Comprobar que NO se ha pasado al siguiente año 
        Assert.That(_estadisticasManager.anno, Is.EqualTo(2018));
        //Comprobar que se ha pasado a la siguiente semana almacenada (en este caso, directamente la siguiente a la actual)
        Assert.That(_estadisticasManager.semana, Is.EqualTo(2));
        //Comprobar que no se han cambiado los valores de primera y ultima semana del año almacenadas
        Assert.That(_estadisticasManager.primeraSemana, Is.EqualTo(1));
        Assert.That(_estadisticasManager.ultimaSemana, Is.EqualTo(3));
    }

    //Comprobar que no se puede retroceder en la fecha si no hay una entrada anterior
    [Test]
    public void TestNoPuedeRetroceder () {
        bool retroceder;
        
        //No hace falta inicializar los datos, todo depende del valor que devuelva el mock de la bd
        MockUltimaSemana(2018, -1); //Devolver -1 para que no pueda seguir retrocediendo

        retroceder = _estadisticasManager.PuedeRetroceder();

        Assert.That(retroceder, Is.False);
    }

    //Comprobar que si se puede retroceder en la fecha si no hay una entrada anterior
    [Test]
    public void TestPuedeRetroceder() {
        bool retroceder;

        //No hace falta inicializar los datos, todo depende del valor que devuelva el mock de la bd
        MockUltimaSemana(2018, 1); //Devolver 1 para que si pueda seguir retrocediendo

        retroceder = _estadisticasManager.PuedeRetroceder();

        Assert.That(retroceder, Is.True);
    }

    //Comprobar que se cambia de año al retroceder de semana si ya se esta en la primera semana 
    [Test]
    public void TestCambiarAnnoPrimeraSemana() {

        _estadisticasManager.anno = 2019; //Año cuyas estadisticas se estan mostrando en el momento
        _estadisticasManager.annoActual = 2019; //año real
        //La semana actual y la primera guardada son la misma para que se avance al año siguiente
        _estadisticasManager.semana = _estadisticasManager.semanaActual =
            _estadisticasManager.primeraSemana = _estadisticasManager.ultimaSemana = 1;

        //Mocks necesarios porque el metodo AvanzarSemana llama a MostrarEstadisticasSemana
        MockTiempoTotalActividad(1, 2019); //Mocks para ObtenerTiempoTotal
        MockPrimerDia(1, 2019); //La semana y el año no son importantes
        MockUltimoDia(1, 2019); //La semana y el año no son importantes
        MockUltimaSemana(2018, 3); //La ultima semana almacenada del año anterior es la semana 3

        //Mock para obtener el siguiente año almacenados
        MockAnteriorAnno(2019);

        //Mock para el metodo ObtenerPrimeraSemana
        MockPrimeraSemana(2019, 2); //La primera semana almacenada del año anterior es la semana 2

        _estadisticasManager.RetrocederSemana();

        //Comprobar que se ha pasado al siguiente año (anno + 1)
        Assert.That(_estadisticasManager.anno, Is.EqualTo(2018));
        //Comprobar que se han actualizado la primera y la ultima semana almacenadas para el nuevo año
        Assert.That(_estadisticasManager.primeraSemana, Is.EqualTo(2));
        Assert.That(_estadisticasManager.ultimaSemana, Is.EqualTo(3));
    }

    //Comprobar que al retroceder de semana no se cambia de año si no se esta en la primera semana almacenada
    [Test]
    public void TestRetrocederSemanaMismoAnno() {

        _estadisticasManager.anno = 2019; //Año cuyas estadisticas se estan mostrando en el momento
        _estadisticasManager.annoActual = 2019; //año real
        //La semana actual y la primera guardada NO son la misma, se puede seguir avanzando
        _estadisticasManager.semana = _estadisticasManager.semanaActual = 5;
        _estadisticasManager.primeraSemana = 1;
        _estadisticasManager.ultimaSemana = 6;

        //Mocks necesarios porque el metodo AvanzarSemana llama a MostrarEstadisticasSemana
        MockTiempoTotalActividad(1, 2019); //Mocks para ObtenerTiempoTotal
        MockPrimerDia(1, 2019); //La semana y el año no son importantes
        MockUltimoDia(1, 2019); //La semana y el año no son importantes
        MockUltimaSemana(2018, 3); //La ultima semana almacenada del año 2019 es la semana 3       

        //Mock para obtener la siguiente semana almacenada
        MockAnteriorSemana(5);

        _estadisticasManager.RetrocederSemana();

        //Comprobar que NO se ha pasado al siguiente año 
        Assert.That(_estadisticasManager.anno, Is.EqualTo(2019));
        //Comprobar que se ha pasado a la anterior semana almacenada (en este caso, directamente la anterior a la actual)
        Assert.That(_estadisticasManager.semana, Is.EqualTo(4));
        //Comprobar que no se han cambiado los valores de primera y ultima semana del año almacenadas
        Assert.That(_estadisticasManager.primeraSemana, Is.EqualTo(1));
        Assert.That(_estadisticasManager.ultimaSemana, Is.EqualTo(6));
    }

    /***** METODOS AUXILIARES *****/

    //Mock del metodo TiempoTotalActividad que accede a la base de datos para cada actividad
    //Lo importante es el tipo de actividad que se pasa, aqui la semana y el año dan igual
    //Devuelve un reader con un float, tambien se sustituye
    private void MockTiempoTotalActividad(int semana, int anno) {

        var reader = new Mock<System.Data.IDataReader>();
        reader.Setup(r => r.IsDBNull(0)).Returns(false);
        reader.Setup(r => r.GetFloat(0)).Returns(1.0f);
        reader.SetupSequence(r => r.Read()).Returns(true).Returns(false)
            .Returns(true).Returns(false)
            .Returns(true).Returns(false)
            .Returns(true).Returns(false)
            .Returns(true).Returns(false);        

        _estadisticasDB.Setup(m => m.TiempoTotalActividad("estudio", It.IsAny<int>(), It.IsAny<int>())).Returns(reader.Object);
        _estadisticasDB.Setup(m => m.TiempoTotalActividad("ejercicio", It.IsAny<int>(), It.IsAny<int>())).Returns(reader.Object);
        _estadisticasDB.Setup(m => m.TiempoTotalActividad("hogar", It.IsAny<int>(), It.IsAny<int>())).Returns(reader.Object);
        _estadisticasDB.Setup(m => m.TiempoTotalActividad("ocio", It.IsAny<int>(), It.IsAny<int>())).Returns(reader.Object);
        _estadisticasDB.Setup(m => m.TiempoTotalActividad("otros", It.IsAny<int>(), It.IsAny<int>())).Returns(reader.Object);
    }

    //Mock del metodo ObtenerPrimerDia que accede a la bd
    //Devuelve un reader con un string, tambien se sustituye (el mock siempre devolvera la misma fecha)
    private void MockPrimerDia (int semana, int anno) {
        var reader = new Mock<System.Data.IDataReader>();
        reader.Setup(r => r.IsDBNull(0)).Returns(false);
        reader.Setup(r => r.GetString(0)).Returns("17/01/2018");
        reader.SetupSequence(r => r.Read()).Returns(true).Returns(false);

        _estadisticasDB.Setup(m => m.BuscarPrimerDiaSemana(It.IsAny<int>(), It.IsAny<int>())).Returns(reader.Object);
    }

    //Mock del metodo ObtenerPrimerDia que accede a la bd
    //Devuelve un reader con un string, tambien se sustituye (el mock siempre devolvera la misma fecha)
    private void MockUltimoDia(int semana, int anno) {
        var reader = new Mock<System.Data.IDataReader>();
        reader.Setup(r => r.IsDBNull(0)).Returns(false);
        reader.Setup(r => r.GetString(0)).Returns("23/01/2018");//La fecha que devuelve no es importante en este caso
        reader.SetupSequence(r => r.Read()).Returns(true).Returns(false);

        _estadisticasDB.Setup(m => m.BuscarUltimoDiaSemana(It.IsAny<int>(), It.IsAny<int>())).Returns(reader.Object);
    }

    //Mock del metodo BuscarUltimaSemanaAnno que accede a la bd
    //Devuelve un reader con un int que siempre sera el mismo (1)
    private void MockUltimaSemana (int anno, int semanaReturn) {
        var reader = new Mock<System.Data.IDataReader>();
        reader.Setup(r => r.IsDBNull(0)).Returns(false);
        reader.Setup(r => r.GetInt32(0)).Returns(semanaReturn);
        reader.SetupSequence(r => r.Read()).Returns(true).Returns(false);

        _estadisticasDB.Setup(m => m.BuscarUltimaSemanaAnno(It.IsAny<int>())).Returns(reader.Object);
    }

    //Mock del metodo BuscarPrimeraSemanaAnno que accede a la bd
    //Devuelve un reader con un int que siempre sera el mismo (1)
    private void MockPrimeraSemana(int anno, int semanaReturn) {
        var reader = new Mock<System.Data.IDataReader>();
        reader.Setup(r => r.IsDBNull(0)).Returns(false);
        reader.Setup(r => r.GetInt32(0)).Returns(semanaReturn);
        reader.SetupSequence(r => r.Read()).Returns(true).Returns(false);

        _estadisticasDB.Setup(m => m.BuscarPrimeraSemanaAnno(It.IsAny<int>())).Returns(reader.Object);
    }

    //Mock del metodo BuscarSiguienteAnno que accede a la bd
    //Se supone que el siguiente año almacenado es directamente el siguiente al que se esta viendo
    private void MockSiguienteAnno (int anno) {
        var reader = new Mock<System.Data.IDataReader>();
        reader.Setup(r => r.IsDBNull(0)).Returns(false);
        reader.Setup(r => r.GetInt32(0)).Returns(anno+1);
        reader.SetupSequence(r => r.Read()).Returns(true).Returns(false);

        _estadisticasDB.Setup(m => m.BuscarSiguienteAnno(It.IsAny<int>())).Returns(reader.Object);
    }

    //Mock del metodo BuscarAnteriorAnno que accede a la bd
    //Se supone que el anterior año almacenado es directamente el anterior al que se esta viendo
    private void MockAnteriorAnno(int anno) {
        var reader = new Mock<System.Data.IDataReader>();
        reader.Setup(r => r.IsDBNull(0)).Returns(false);
        reader.Setup(r => r.GetInt32(0)).Returns(anno - 1);
        reader.SetupSequence(r => r.Read()).Returns(true).Returns(false);

        _estadisticasDB.Setup(m => m.BuscarAnteriorAnno(It.IsAny<int>())).Returns(reader.Object);
    }

    //Mock del metodo BuscarSiguienteSemanaAnno que accede a la bd
    //Se supone que la siguiente semana almacenada es directamente la siguiente a la que se esta viendo
    private void MockSiguienteSemana(int semana) {
        var reader = new Mock<System.Data.IDataReader>();
        reader.Setup(r => r.IsDBNull(0)).Returns(false);
        reader.Setup(r => r.GetInt32(0)).Returns(semana + 1);
        reader.SetupSequence(r => r.Read()).Returns(true).Returns(false);

        _estadisticasDB.Setup(m => m.BuscarSiguienteSemanaAnno(It.IsAny<int>(), It.IsAny<int>())).Returns(reader.Object);
    }

    //Mock del metodo BuscarAnteriorSemanaAnno que accede a la bd
    //Se supone que la anterior semana almacenada es directamente la anterior a la que se esta viendo
    private void MockAnteriorSemana(int semana) {
        var reader = new Mock<System.Data.IDataReader>();
        reader.Setup(r => r.IsDBNull(0)).Returns(false);
        reader.Setup(r => r.GetInt32(0)).Returns(semana - 1);
        reader.SetupSequence(r => r.Read()).Returns(true).Returns(false);

        _estadisticasDB.Setup(m => m.BuscarAnteriorSemanaAnno(It.IsAny<int>(), It.IsAny<int>())).Returns(reader.Object);
    }

    private void CrearObjetos () {
        //Campos de texto
        new GameObject("MinutosEstudio").AddComponent<Text>();
        new GameObject("MinutosEjercicio").AddComponent<Text>();
        new GameObject("MinutosHogar").AddComponent<Text>();
        new GameObject("MinutosOcio").AddComponent<Text>();
        new GameObject("MinutosOtros").AddComponent<Text>();
        _estadisticasManager.fecha = new GameObject("fecha").AddComponent<Text>();
        _estadisticasManager.total = new GameObject("total").AddComponent<Text>();

        //Barras de progreso
        new GameObject("FillEstudio").AddComponent<Image>();
        new GameObject("FillEjercicio").AddComponent<Image>();
        new GameObject("FillHogar").AddComponent<Image>();
        new GameObject("FillOcio").AddComponent<Image>();
        new GameObject("FillOtros").AddComponent<Image>();

        //Flecha
        _estadisticasManager.flechaIzquierda = new GameObject();
        _estadisticasManager.flechaDerecha = new GameObject();
    }
}