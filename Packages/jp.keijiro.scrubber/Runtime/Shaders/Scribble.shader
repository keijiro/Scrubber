Shader "Hidden/UITK/Scribble"
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
            float _Width;
            float _Aspect;

            float4 Vertex(uint vid : SV_VertexID) : SV_Position
            {
                float3 va = normalize(_Point1 - _Point0);
                float3 vb = cross(va, float3(0, 0, 1));

                va *= _Width * float3(1, _Aspect, 0);
                vb *= _Width * float3(1, _Aspect, 0);

                float3 v0 = _Point0 - va;
                float3 v1 = _Point0 - vb;
                float3 v2 = _Point0 + vb;
                float3 v3 = _Point1 - vb;
                float3 v4 = _Point1 + vb;
                float3 v5 = _Point1 + va;

                float3 vertices[] = {
                    v0, v1, v2,
                    v1, v2, v3,
                    v2, v3, v4,
                    v3, v4, v5
                };

                return float4(vertices[vid], 1);
            }

            float4 Fragment(float4 position : SV_Position) : SV_Target
            {
                return _Color;
            }

            ENDCG
        }
    }
}
