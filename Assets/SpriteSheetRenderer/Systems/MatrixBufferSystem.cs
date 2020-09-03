using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class MatrixBufferSystem : JobComponentSystem {
  protected override JobHandle OnUpdate(JobHandle inputDeps) {
    var buffers = DynamicBufferManager.GetMatrixBuffers();
    NativeArray<JobHandle> jobs = new NativeArray<JobHandle>(buffers.Length, Allocator.TempJob);
    for(int i = 0; i < buffers.Length; i++) {
      DynamicBuffer<MatrixBuffer> matrixBuffer = buffers[i];
      jobs[i] = Entities
        .WithNativeDisableParallelForRestriction(matrixBuffer)
        .WithChangeFilter<SpriteMatrix>()
        .ForEach((in SpriteMatrix data, in BufferHook hook) => {
          if(i == hook.bufferEnityID)
            matrixBuffer[hook.bufferID] = data.matrix;
        }).Schedule(inputDeps);
    }
    JobHandle.CompleteAll(jobs);
    jobs.Dispose();
    return inputDeps;
  }
}