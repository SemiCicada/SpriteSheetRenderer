using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

public class SpriteSheetLayerSystem : JobComponentSystem {
    protected override JobHandle OnUpdate(JobHandle inputDeps) {

        var buffers = DynamicBufferManager.GetLayerBuffers();
        NativeArray<JobHandle> jobs = new NativeArray<JobHandle>(buffers.Length, Allocator.TempJob);
        for(int i = 0; i < buffers.Length; i++) {
            DynamicBuffer<SpriteLayerBuffer> layerBuffer = buffers[i];
            jobs[i] = Entities
                .WithNativeDisableParallelForRestriction(layerBuffer)
                .WithChangeFilter<SpriteSheetSortingLayer>()
                .ForEach((in SpriteSheetSortingLayer layer, in BufferHook hook) => {
                    if(i == hook.bufferEnityID)
                        layerBuffer[hook.bufferID] = layer.Value;
            }).Schedule(inputDeps);
        }

        JobHandle.CompleteAll(jobs);
        jobs.Dispose();
        return inputDeps;
    }
}
