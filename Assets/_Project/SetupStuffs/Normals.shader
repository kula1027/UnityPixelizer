Shader "Custom/RenderNormal"{

	SubShader{
		Cull Off
		ZWrite Off 
		ZTest Always

		Pass{
			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _CameraDepthNormalsTexture;

			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v){
				v2f o;

				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
		
			fixed4 frag(v2f i) : SV_TARGET{
				float4 depthNormal = tex2D(_CameraDepthNormalsTexture, i.uv);

				float3 normal;
				float depth;
				DecodeDepthNormal(depthNormal, depth, normal);				

				normal = (normal + float3(1, 1, 1)) * 0.5f;

				float a = 1;
				if (depth > 0.99) {
					a = 0;
				}

				return float4(normal, a);
			}
			ENDCG
		}
	}
}

