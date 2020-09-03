using Unity.Entities;
using UnityEngine;

public class RenderInformation {
  // These buffers are populated in SpriteSheetRenderer.cs
  public ComputeBuffer matrixBuffer;
  public ComputeBuffer argsBuffer;
  public ComputeBuffer colorsBuffer;
  public ComputeBuffer uvBuffer;
  public ComputeBuffer indexBuffer;
  public ComputeBuffer layerBuffer;
  public Entity bufferEntity;
  public int spriteCount;
  public Material material;
  public uint[] args;
  public bool updateUvs;
  public RenderInformation(Material mat, Entity bufEntity) {
    material = mat;
    material.enableInstancing = true;
    spriteCount = SpriteSheetCache.GetLenght(material);
    bufferEntity = bufEntity;
    // Arguments for drawing mesh
    args = new uint[5] { 0, 0, 0, 0, 0 };
    argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
    // 0 == number of triangle indices, 1 == population, others are only relevant if drawing submeshes
    args[0] = (uint)6;
    args[2] = args[3] = (uint)0;
    updateUvs = true;
  }

  public void DestroyBuffers() {
    if(matrixBuffer != null)
      matrixBuffer.Release();
    matrixBuffer = null;

    if(argsBuffer != null)
      argsBuffer.Release();
    argsBuffer = null;

    if(colorsBuffer != null)
      colorsBuffer.Release();
    colorsBuffer = null;

    if(uvBuffer != null)
      uvBuffer.Release();
    uvBuffer = null;

    if(indexBuffer != null)
      indexBuffer.Release();
    indexBuffer = null;

    if(layerBuffer != null)
      layerBuffer.Release();
    layerBuffer = null;
  }
}
