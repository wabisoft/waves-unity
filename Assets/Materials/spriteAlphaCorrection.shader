Shader "Unlit/spriteAlphaCorrection"
{
	// exposed in inspector
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BlahBlah("AR float", Float) = 1.
	}

	SubShader
	{
		Pass
		{
			ZWrite Off
			// https://docs.unity3d.com/Manual/SL-Blend.html
			// I think this means:
			// Blend SrcFactor DstFactor:
			// The generated color is multiplied by the SrcFactor (the shader output)
			// The color already on screen is multiplied by DstFactor and the two are added together.
			BlendOp Add
			Blend SrcAlpha OneMinusSrcAlpha
			// SrcFactor = 1
			// DstFactor = 1
			// Blend => 1 + pixel.a * 1
			// this should be what I want
			CGPROGRAM
			#pragma vertex vertexShader
			#pragma fragment fragmentShader
			#include "UnityCG.cginc"

			// this is like vertex attribs in OpenGL
			// literally the same as appdata_base, but nice to see
			struct appdata
			 {
				 float4 vertex   : POSITION;  // The vertex position in model space.
				 float4 texcoord : TEXCOORD0; // The first UV coordinate.
			 };
			struct v2f
			{
				float4 position : SV_POSITION; // gl_Position gibi; the position of the vertex after being transformed into projection space. 
				float4 uv : TEXCOORD0;
			};
			v2f vertexShader(appdata vertexAttrib)
			{
				v2f vertexShaderOutput;
				vertexShaderOutput.position = UnityObjectToClipPos(vertexAttrib.vertex);
				vertexShaderOutput.uv = vertexAttrib.texcoord;
				return vertexShaderOutput;
			}

			sampler2D _MainTex;
			float _BlahBlah;
			fixed when_eq(fixed x, fixed y) 
			{
				return 1.0 - abs(sign(x - y));
			}

			fixed4 fragmentShader(v2f vertexShaderOutput) : SV_Target
			{
				// Texel sampling
				fixed2 uv = vertexShaderOutput.uv;
				
				uv.x *= _BlahBlah;
				return tex2D(_MainTex, uv);
			}
			ENDCG
		}
	}
}