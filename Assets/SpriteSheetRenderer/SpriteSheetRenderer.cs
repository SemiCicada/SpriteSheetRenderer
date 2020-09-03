using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
public class SpriteSheetRenderer : ComponentSystem {
  private Mesh mesh;
  protected override void OnCreate() {
    mesh = MeshExtension.Quad();
  }

  protected override void OnDestroy() {
    SpriteSheetManager.CleanBuffers();
  }
  protected override void OnUpdate() {

    for(int i = 0; i < SpriteSheetManager.renderInformation.Count; i++) {
      if(UpdateBuffers(i) > 0) {
        // The bounds in DrawMeshInstancedIndirect are in terms of world position
        // Therefore the camera will cull the meshes when the bounds are not on screen
        // In other words, the bounds will delineate the drawing space of the sprites
        // The easiest solution is to simply increase the area of the bounds (as the method is fast already)
        Graphics.DrawMeshInstancedIndirect(
          mesh, 
          0, 
          SpriteSheetManager.renderInformation[i].material, 
          new Bounds(Vector2.zero, Vector2.one * 500), 
          SpriteSheetManager.renderInformation[i].argsBuffer, 
          0);
      }
      
      //this is w.i.p to clean the old buffers
      DynamicBuffer<SpriteIndexBuffer> indexBuffer = EntityManager.GetBuffer<SpriteIndexBuffer>(SpriteSheetManager.renderInformation[i].bufferEntity);
      int size = indexBuffer.Length - 1;
      int toRemove = 0;
      for(int j = size; j >= 0; j--) {
        if(indexBuffer[j].index == -1) {
          toRemove++;
        }
        else {
          break;
        }
      }
      if(toRemove > 0) {
        EntityManager.GetBuffer<SpriteIndexBuffer>(SpriteSheetManager.renderInformation[i].bufferEntity).RemoveRange(size + 1 - toRemove, toRemove);
        EntityManager.GetBuffer<SpriteLayerBuffer>(SpriteSheetManager.renderInformation[i].bufferEntity).RemoveRange(size + 1 - toRemove, toRemove);
        EntityManager.GetBuffer<MatrixBuffer>(SpriteSheetManager.renderInformation[i].bufferEntity).RemoveRange(size + 1 - toRemove, toRemove);
        EntityManager.GetBuffer<SpriteColorBuffer>(SpriteSheetManager.renderInformation[i].bufferEntity).RemoveRange(size + 1 - toRemove, toRemove);
      }
    }
  }
      //we should only update the index of the changed datas for index buffer,matrixbuffer and color buffer inside a burst job to avoid overhead
  int UpdateBuffers(int renderIndex) {
    SpriteSheetManager.ReleaseBuffer(renderIndex);

    RenderInformation renderInformation = SpriteSheetManager.renderInformation[renderIndex];
    int instanceCount = EntityManager.GetBuffer<SpriteIndexBuffer>(renderInformation.bufferEntity).Length;
    if(instanceCount > 0) {
      int stride = instanceCount >= 16 ? 16 : 16 * SpriteSheetCache.GetLenght(renderInformation.material);
      if(renderInformation.updateUvs) {
        SpriteSheetManager.ReleaseUvBuffer(renderIndex);
        renderInformation.uvBuffer = new ComputeBuffer(instanceCount, stride);
        renderInformation.uvBuffer.SetData(EntityManager.GetBuffer<UvBuffer>(renderInformation.bufferEntity).Reinterpret<float4>().AsNativeArray());
        renderInformation.material.SetBuffer("uvBuffer", renderInformation.uvBuffer);
        renderInformation.updateUvs = false;
      }

      renderInformation.indexBuffer = new ComputeBuffer(instanceCount, sizeof(int));
      renderInformation.indexBuffer.SetData(EntityManager.GetBuffer<SpriteIndexBuffer>(renderInformation.bufferEntity).Reinterpret<int>().AsNativeArray());
      renderInformation.material.SetBuffer("indexBuffer", renderInformation.indexBuffer);

      renderInformation.layerBuffer = new ComputeBuffer(instanceCount, sizeof(int));
      renderInformation.layerBuffer.SetData(EntityManager.GetBuffer<SpriteLayerBuffer>(renderInformation.bufferEntity).Reinterpret<int>().AsNativeArray());
      renderInformation.material.SetBuffer("layerBuffer", renderInformation.layerBuffer);

      renderInformation.matrixBuffer = new ComputeBuffer(instanceCount, 16);
      renderInformation.matrixBuffer.SetData(EntityManager.GetBuffer<MatrixBuffer>(renderInformation.bufferEntity).Reinterpret<float4>().AsNativeArray());
      renderInformation.material.SetBuffer("matrixBuffer", renderInformation.matrixBuffer);

      renderInformation.args[1] = (uint)instanceCount;
      renderInformation.argsBuffer.SetData(renderInformation.args);

      renderInformation.colorsBuffer = new ComputeBuffer(instanceCount, 16);
      renderInformation.colorsBuffer.SetData(EntityManager.GetBuffer<SpriteColorBuffer>(renderInformation.bufferEntity).Reinterpret<float4>().AsNativeArray());
      renderInformation.material.SetBuffer("colorsBuffer", renderInformation.colorsBuffer);
    }
    return instanceCount;
  }

}