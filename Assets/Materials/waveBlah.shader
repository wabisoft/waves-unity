Shader "Unlit/waveBlah"
{
	Properties
	{
		_seaHeight("seaHeight", Float) = -3.5
		_blah("blah", Float) = 10
	}
	SubShader
	{
		Pass
		{
			ZWrite Off

			BlendOp Add
			Blend SrcAlpha OneMinusSrcAlpha

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

			float when_gt(float x, float y) {
				return max(sign(x - y), 0.0); // 1 iff x > y; 0.
			}
			//
			float _WaveAmps[25];
			float _WaveXPositions[25];
			float _WaveWidths[25];
			float _WaveDecays[25];
			//
			float _seaHeight;
			float _NumWaves;
			//
			float _quadPosY;
			float _quadPosX;
			float _quadSizeX;
			float _quadSizeY;

			float _blah;

			float4 frag(v2f i) : SV_Target
			{
				float2 uv = 2. * i.uv - 1.; // NDC coords
				//uv.x *= _ScreenParams.x / _ScreenParams.y; // AR correction
				
				float function = (_seaHeight - _quadPosY)/ _quadSizeY;
				
				for (int i = 0; i < _NumWaves; i++)
				{
					function += _WaveAmps[i] / _quadSizeY * _WaveDecays[i] * exp(-1. * pow(_WaveWidths[i] * _quadSizeX * (uv.x - (_WaveXPositions[i] - _quadPosX)/ _quadSizeX), 2.));
				}
				float val = (1. - smoothstep(function, function + 0.015, uv.y))
					      - (1. - smoothstep(function - 0.015, function, uv.y));

				float3 col = float3(val, val, val);
				
				float4 fragColor = float4(col, when_gt(val, 0.1));
				return fragColor;
			}
			ENDCG
		}
	}
}
