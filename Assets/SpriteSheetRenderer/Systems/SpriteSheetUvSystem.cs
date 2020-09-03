using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class SpriteSheetUvJobSystem : JobComponentSystem {
  protected override JobHandle OnUpdate(JobHandle inputDeps) {
    var buffers = DynamicBufferManager.GetIndexBuffers();
    NativeArray<JobHandle> jobs = new NativeArray<JobHandle>(buffers.Length, Allocator.TempJob);
    for(int i = 0; i < buffers.Length; i++) {
      DynamicBuffer<SpriteIndexBuffer> indexBuffer = buffers[i];
      jobs[i] = Entities
        .WithNativeDisableParallelForRestriction(indexBuffer)
        .WithChangeFilter<SpriteIndex>()
        .ForEach((in SpriteIndex data, in BufferHook hook) => {
          if(i == hook.bufferEnityID)
            indexBuffer[hook.bufferID] = data.Value;
        }).Schedule(inputDeps);
    }
    JobHandle.CompleteAll(jobs);
    jobs.Dispose();
    return inputDeps;
  }
}
