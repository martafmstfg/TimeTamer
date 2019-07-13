/* Installer de Zenject para declarar las dependencias de las clases relacionadas con las estadisticas*/

using Zenject;
using Estadisticas_DB;

public class EstadisticasInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        //Cada clase que necesite un objeto que implemente la interfaz IEstadisticasDB 
        // recibira siempre la misma instancia de EstadisticasDB
        Container.Bind<IEstadisticasDB>().To<EstadisticasDB>().AsSingle();
    }
}