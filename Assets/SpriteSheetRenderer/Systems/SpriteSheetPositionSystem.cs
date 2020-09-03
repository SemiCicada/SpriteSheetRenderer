using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class SpriteSheetPositionSystem : JobComponentSystem {
  protected override JobHandle OnUpdate(JobHandle inputDeps) {
    var jobHandle = Entities
      .WithChangeFilter<Position2D>()
      .ForEach((ref SpriteMatrix renderData, in Position2D translation) =>{
        renderData.matrix.x = translation.Value.x;
        renderData.matrix.y = translation.Value.y;
      }).Schedule(inputDeps);
      
    return jobHandle;
  }
}
