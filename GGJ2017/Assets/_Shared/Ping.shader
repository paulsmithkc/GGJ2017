Shader "Custom/Ping" {

    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _DistPeriod("Distance Period", Float) = 0.01
        _TimePeriod("Time Period", Float) = 1.0
        _WaveRadius("Wave Radius", Float) = 1.0
        _Radius("Radius", Float) = 2.0
        _Center("Center", Vector) = (0,0,0,0)
    }

    SubShader {
        Tags {
            "Queue" = "Transparent"  // draw after all opaque geometry has been drawn
        }

        Pass {
            ZWrite Off  // don't write to depth buffer in to avoid occluding other objects
            Blend SrcAlpha OneMinusSrcAlpha   // use alpha blending
            Cull off

            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag 

            uniform float4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform float _DistPeriod;
            uniform float _TimePeriod;
            uniform float _WaveRadius;
            uniform float _Radius;
            uniform float4 _Center;
            
            struct vertexInput {
                float4 vertex : POSITION;
            };
            struct vertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            vertexOutput vert(vertexInput input) {
                vertexOutput output;

                // transformation of input.vertex from object coordinates to screen coordinates
                output.pos = mul(UNITY_MATRIX_MVP, input.vertex);

                // transformation of input.vertex from object coordinates to world coordinates
                output.worldPos = mul(unity_ObjectToWorld, input.vertex);

                return output;
            }

            float4 frag(vertexOutput input) : COLOR {
                float time = _Time.w;
                float dist = distance(input.worldPos.xy, _Center.xy);
                float x = dist / _DistPeriod - time / _TimePeriod;
                if (x > -_WaveRadius && x < _WaveRadius)
                {
                    float4 color = _Color;
                    color.a = max(sin(x), 0);
                    color.a *= (1.0 - (dist / _Radius));
                    return color;
                }
                else
                {
                    return float4(0.0, 0.0, 0.0, 0.0);
                }
            }

            ENDCG
        }
    }
}