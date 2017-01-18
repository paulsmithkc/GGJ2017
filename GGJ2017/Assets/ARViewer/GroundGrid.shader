// Based on 
// https://www.youtube.com/watch?v=gNwduUQrlJs
// http://answers.unity3d.com/questions/442581/how-to-draw-a-grid-over-parts-of-the-terrain.html
Shader "Custom/Ground Grid" {

    Properties{
        _GridThickness("Grid Thickness", Float) = 0.02
        _GridSpacing("Grid Spacing", Float) = 10.0
        _GridRadius("Grid Radius", Float) = 20.0
        _GridColor("Grid Color", Color) = (0.5, 1.0, 0.5, 1.0)
        //_OutsideColor("Color Outside Grid", Color) = (0.0, 0.0, 0.0, 0.0)
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

            uniform float _GridThickness;
            uniform float _GridSpacing;
            uniform float _GridRadius;
            uniform float4 _GridColor;
            //uniform float4 _OutsideColor;

            struct vertexInput {
                float4 vertex : POSITION;
            };
            struct vertexOutput {
                float4 pos : SV_POSITION;
                float4 worldPos : TEXCOORD0;
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
                if (frac(input.worldPos.x / _GridSpacing) < _GridThickness ||
                    frac(input.worldPos.z / _GridSpacing) < _GridThickness) {

                    float dist = distance(input.worldPos.xyz, float3(0.0, 0.0, 0.0));
                    float4 color = _GridColor;
                    color.a = 1.0 - (dist / _GridRadius);
                    return color;
                } else {
                    return float4(0.0, 0.0, 0.0, 0.0);
                }
            }

            ENDCG
        }
    }
}