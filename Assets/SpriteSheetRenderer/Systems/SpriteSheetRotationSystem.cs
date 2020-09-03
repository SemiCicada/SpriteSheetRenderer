using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

public class SpriteSheetRotationSystem : JobComponentSystem {
  protected override JobHandle OnUpdate(JobHandle inputDeps) {
    var jobHandle = Entities
      .ForEach((ref SpriteMatrix renderData, in Rotation2D rotation) => {
        renderData.matrix.z = rotation.angle;
      }).Schedule(inputDeps);
    return jobHandle;
  }
}
