Shader "Unlit/waveBlah"
{
	Properties
	{
		_Mouse("Mouse", Float) = 0.0
		_waveTimer("waveTimer", Float) = 0.0
		_seaHeight("seaHeight", Float) = -3.5
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct appdata 
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};
			struct v2f 
			{
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
			};
			v2f vert(appdata v) 
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float _WaveAmps[25];
			float _WaveXPositions[25];
			float _WaveWidths[25];
			float _WaveDecays[25];

			float _seaHeight;
			float _NumWaves;

			float4 frag(v2f i) : SV_Target
			{
				float2 uv = 2. * i.uv - 1.; // NDC coords
				//uv.x *= _ScreenParams.x / _ScreenParams.y; // AR correction
				
				float function = _seaHeight/5.;
				
				for (int i = 0; i < _NumWaves; i++)
				{
					function += _WaveAmps[i] / 5. * _WaveDecays[i] * exp(-50. * pow(_WaveWidths[i] * (uv.x - _WaveXPositions[i]/7.), 2.));
				}
				float val = (1. - smoothstep(function, function + 0.02, uv.y))
					      - (1. - smoothstep(function - 0.02, function, uv.y));

				float3 col = float3(val, val, val);
				
				float4 fragColor = float4(col, 1.0);
				return fragColor;
			}
			ENDCG
		}
	}
}
