 Shader "Instanced/SpriteSheet" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    
    SubShader {
        Tags{
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass {
            CGPROGRAM
            // Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
            #pragma exclude_renderers gles

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma target 4.5
            #pragma instancing_options procedural:setup

            #include "UnityCG.cginc"

            /* Vertex shader inputs
            struct appdata_full {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                float4 texcoord3 : TEXCOORD3;
                fixed4 color : COLOR;
            }
            */

            // Vertex shader outputs (Vertex to fragment)
            struct v2f{
                float4 pos : SV_POSITION;
                float2 uv: TEXCOORD0;
				fixed4 color : COLOR0;
            };

            float4x4 rotationZMatrix(float rZ){
                float angleZ = radians(rZ);
                float c = cos(angleZ);
                float s = sin(angleZ);
                float4x4 ZMatrix  = 
                    float4x4( 
                       c,  -s, 0,  0,
                       s,  c,  0,  0,
                       0,  0,  1,  0,
                       0,  0,  0,  1);
                return ZMatrix;
            }

            sampler2D _MainTex;

            StructuredBuffer<float4> matrixBuffer;
            StructuredBuffer<int> indexBuffer;
            StructuredBuffer<int> layerBuffer;
            StructuredBuffer<float4> uvBuffer;
			StructuredBuffer<float4> colorsBuffer;

            // Vertex shader
            v2f vert (appdata_full input, uint instanceID : SV_InstanceID){
                //transform.xy = posizion x and y
                //transform.z = rotation angle
                //transform.w = scale
                float4 transform = matrixBuffer[instanceID];
                float4 uv = uvBuffer[indexBuffer[instanceID]];
                int layer = layerBuffer[instanceID];
                //rotate the vertex (around its center)
                input.vertex = mul(input.vertex - float4(0.5,0.5,0,0), rotationZMatrix(transform.z));
                //scale it
                float3 worldPosition = float3(transform.x, transform.y, layer) + (input.vertex.xyz * transform.w);

                v2f output;
                // Transform position to clip space
                output.pos = UnityObjectToClipPos(float4(worldPosition, 0.0f));
                output.uv = (input.texcoord * uv.xy) + uv.zw;
                //output.uv =  input.texcoord * uv.xy + uv.zw;
				output.color = colorsBuffer[instanceID];
                return output;
            }

            // Fragment shader
            fixed4 frag (v2f input) : SV_Target{
                // Sample texture and apply color
                fixed4 col = tex2D(_MainTex, input.uv) * input.color;
				clip(col.a - 1.0 / 255.0);
                col.rgb *= col.a;
				return col;
            }

            ENDCG
        }
    }
}