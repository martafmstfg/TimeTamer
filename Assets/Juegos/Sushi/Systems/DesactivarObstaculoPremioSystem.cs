/* Sistema para desactivar los obstaculos y los premios cuando llegan al borde de la pantalla.
 * Desactivar uno de estos elementos implica quitarle a su entidad los componentes
 * necesarios para que aparezca en pantalla y se mueva: meshinstancerenderer y position */

using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Collections;

public class DesactivarObstaculoPremioSystem : JobComponentSystem {

    ComponentGroup m_ObstaculosActivos;
    EntityArray obstaculosActivos;

    ComponentGroup m_PremiosActivos;
    EntityArray premiosActivos;

    //Comprueba si alguno de los obstaculos activos han pasado 
    // del borde inferior de la pantalla para desactivarlos 
    [RequireComponentTag(typeof(ObstaculoActivoComponent))]
    [RequireSubtractiveComponent(typeof(PalillosComponent))]
    private struct DesactivarObstaculo : IJobProcessComponentDataWithEntity<Position> {
                
        [ReadOnly] public EntityCommandBuffer Commands;
               
        public void Execute(Entity entity, int index, [ReadOnly] ref Position position) {                

            //Comprobar si ha cruzado el limite
            if (position.Value.y < -4.3f) {
                //Eliminar componentes de obstaculo activo
                Commands.RemoveComponent<MeshInstanceRenderer>(entity);
                Commands.RemoveComponent<Position>(entity);
                Commands.RemoveComponent<ObstaculoActivoComponent>(entity);

            }
        }
    }

    //Comprueba si alguno de los premios activos han pasado
    // del borde inferior de la pantalla para desactivarlos 
    [RequireComponentTag(typeof(PremioActivoComponent))]
    private struct DesactivarPremio : IJobProcessComponentDataWithEntity<Position> {
                
        [ReadOnly] public EntityCommandBuffer Commands;
             
        public void Execute(Entity entity, int index, [ReadOnly] ref Position position) {
            
            //Comprobar si ha cruzado el limite       
            if (position.Value.y < -4.3f) {
                //Eliminar componentes de premio activo
                Commands.RemoveComponent<PremioActivoComponent>(entity);
                Commands.RemoveComponent<Position>(entity);
                Commands.RemoveComponent<MeshInstanceRenderer>(entity);
            }
        }
    }

    //Comprueba si alguno de los palillos ha pasado del borde inferior para desactivarlo
    [RequireComponentTag(typeof(PalillosComponent))]
    private struct DesactivarPalillos : IJobProcessComponentDataWithEntity<Position> {

        [ReadOnly] public EntityCommandBuffer Commands;

        public void Execute(Entity entity, int index, [ReadOnly] ref Position position) {

            //Comprobar si ha cruzado el limite       
            if (position.Value.y < -4.3f) {
                //Eliminar componentes del palillo activo
                Commands.RemoveComponent<MeshInstanceRenderer>(entity);
                Commands.RemoveComponent<Position>(entity);
                Commands.RemoveComponent<Rotation>(entity);
                Commands.RemoveComponent<ObstaculoActivoComponent>(entity);
                Commands.RemoveComponent<PalillosComponent>(entity);
            }
        }
    }


    protected override void OnCreateManager() {
        base.OnCreateManager();
        //Obtener grupos de componentes que contengan los componentes indicados (obstaculos y premios activos)
        m_ObstaculosActivos = GetComponentGroup(typeof(ObstaculoComponent), typeof(ObstaculoActivoComponent), typeof(Position));
        m_PremiosActivos = GetComponentGroup(typeof(PremioComponent), typeof(PremioActivoComponent), typeof(Position));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {

        //Arrays de obstaculos activos y de sus posiciones
        obstaculosActivos = m_ObstaculosActivos.GetEntityArray();
        ComponentDataArray<Position> posicionesObstaculos = m_ObstaculosActivos.GetComponentDataArray<Position>();

        //Arrays de premios activos y de sus posiciones
        premiosActivos = m_PremiosActivos.GetEntityArray();
        ComponentDataArray<Position> posicionesPremios = m_PremiosActivos.GetComponentDataArray<Position>();

        var jobObs = new DesactivarObstaculo {           
            Commands = _collisionBarrier.CreateCommandBuffer()
        };

        JobHandle jO = jobObs.Schedule(this, inputDeps);

        var jobPrem = new DesactivarPremio {   
            Commands = _collisionBarrier.CreateCommandBuffer()
        };

        JobHandle jP = jobPrem.Schedule(this, jO); //"depende" del job anterior (espera a que termine)

        var jobPalillos = new DesactivarPalillos {
            Commands = _collisionBarrier.CreateCommandBuffer()
        };

        JobHandle jPl = jobPalillos.Schedule(this, jP); //"depende" del job anterior (espera a que termine)

        return jPl;
    }

    //Necesario para eliminar componentes de una entidad desde un job
    private class CollisionBarrier : BarrierSystem {
    }
    [Inject] private CollisionBarrier _collisionBarrier;
}
