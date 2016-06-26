Shader "Custom/Lines" {
SubShader 
	{
		Pass {


				ZWrite Off 
				Cull Off 
				Fog { Mode Off } 

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct v2f {
					float4 pos : SV_POSITION;
					float3 color : COLOR0;
				};

				struct appdata_self{
					float4 vertex : POSITION;
					float4 color : COLOR;
				};

				v2f vert (appdata_self v)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.color = v.color;
					return o;
				}

				half4 frag (v2f i) : COLOR
				{
					return half4 (i.color, 1);
				}
				ENDCG

			}
	}
	Fallback "VertexLit"
} 
