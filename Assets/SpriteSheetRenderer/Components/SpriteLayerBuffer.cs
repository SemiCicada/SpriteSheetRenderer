using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
[InternalBufferCapacity(sizeof(int))]
public struct SpriteLayerBuffer : IBufferElementData {
  public static implicit operator int(SpriteLayerBuffer e) { return e.index; }
  public static implicit operator SpriteLayerBuffer(int e) { return new SpriteLayerBuffer { index = e }; }
  public int index;
}
