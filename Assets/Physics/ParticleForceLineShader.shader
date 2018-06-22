    Shader "Instanced/ParticleForceLineShader"
    {
        Properties
        {
            _Color ("Color", Color) = (1, 1, 1, 1)
        }
     
        SubShader
        {
            Tags { "RenderType"="Opaque" }
            LOD 100
     
            Pass
            {
                CGPROGRAM
                #include "UnityCG.cginc"
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing
                #pragma instancing_options procedural:setup
     			void setup() {}


                struct appdata
                {
                    float4 vertex : POSITION;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                };
     
                float4 _Color;
     
               
                StructuredBuffer<float3> positions;
                StructuredBuffer<float3> vectors;

                v2f vert(appdata v)
                {
                    v2f o;
     
                    UNITY_SETUP_INSTANCE_ID(v);
                    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    int instance_id = UNITY_GET_INSTANCE_ID(v);
                    float3 position = positions[instance_id];
                    float3 vect = vectors[instance_id];
                    //vect = float3(0,0,0);
                    float3 endPoint = float3(vect.x * v.vertex.x, vect.y * v.vertex.y, vect.z * v.vertex.z);

                   	o.vertex = UnityObjectToClipPos(position + endPoint);
                    #else
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #endif
                    return o;
                }
             
                fixed4 frag(v2f i) : SV_Target
                {
                    return _Color;
                }
                ENDCG
            }
        }
    }
