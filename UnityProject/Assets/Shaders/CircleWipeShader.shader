Shader "Unlit/CircleWipeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Circle Wipe Radius", float) = 0
        _Aspact("Aspact Ratio of Camera", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Radius;
            float _Aspact;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 pos = float3(
                    (i.uv.x - 0.5) * _Aspact,
                    i.uv.y - 0.5, 
                    0 );
                _Radius *= 1.025;
                if (length(pos) < _Radius)
                {
                    // sample the texture
                    return tex2D(_MainTex, i.uv);
                }
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
