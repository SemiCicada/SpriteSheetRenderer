using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class ColorBufferSystem : JobComponentSystem {
  protected override JobHandle OnUpdate(JobHandle inputDeps) {
    var buffers = DynamicBufferManager.GetColorBuffers();
    NativeArray<JobHandle> jobs = new NativeArray<JobHandle>(buffers.Length, Allocator.TempJob);
    for(int i = 0; i < buffers.Length; i++) {
      DynamicBuffer<SpriteColorBuffer> indexBuffer = buffers[i];
      jobs[i] = Entities
        .WithNativeDisableParallelForRestriction(indexBuffer)
        .WithChangeFilter<SpriteSheetColor>()
        .ForEach((in SpriteSheetColor data, in BufferHook hook) => {
          if(i == hook.bufferEnityID)
            indexBuffer[hook.bufferID] = data.color;
        }).Schedule(inputDeps);
    }
    JobHandle.CompleteAll(jobs);
    jobs.Dispose();
    return inputDeps;


  }
}
