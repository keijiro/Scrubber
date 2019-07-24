Shader "Hidden/Scrubber/Pen"
{
    SubShader
    {
        Pass
        {
            Cull Off
            ZTest Always
            ZWrite Off

            CGPROGRAM

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "UnityCG.cginc"

            float3 _Point0;
            float3 _Point1;
            float4 _Color;

            float4 Vertex(uint vid : SV_VertexID) : SV_Position
            {
                return float4(lerp(_Point0, _Point1, vid), 1);
            }

            float4 Fragment(float4 position : SV_Position) : SV_Target
            {
                return _Color;
            }

            ENDCG
        }
    }
}
