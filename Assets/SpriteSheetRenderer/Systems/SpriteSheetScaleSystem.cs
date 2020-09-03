using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;

public class SpriteSheetScaleSystem : JobComponentSystem {
  protected override JobHandle OnUpdate(JobHandle inputDeps) {
    var jobHandle = Entities
      .WithChangeFilter<Scale>()
      .ForEach((ref SpriteMatrix renderData, in Scale scale) => {
        renderData.matrix.w = scale.Value;
      }).Schedule(inputDeps);
    return jobHandle;
  }
}
