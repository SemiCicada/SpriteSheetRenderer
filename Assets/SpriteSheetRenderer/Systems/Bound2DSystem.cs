using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

public class Bound2DSystem : JobComponentSystem {
  protected override JobHandle OnUpdate(JobHandle inputDeps) {
    var jobHandle = Entities
      .ForEach((ref Bound2D bound, in Position2D position, in Scale scale) => {
        bound.scale = scale.Value;
        bound.position = position.Value;
      }).Schedule(inputDeps);

    return jobHandle;
  }
}
