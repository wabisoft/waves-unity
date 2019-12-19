// https://colececil.io/blog/2017/scaling-pixel-art-without-destroying-it/
// https://csantosbh.wordpress.com/2014/01/25/manual-texture-filtering-for-pixelated-games-in-webgl/


 /*{TextureName}_TexelSize - a float4 property contains texture size information :

	 x contains 1.0 / width
	 y contains 1.0 / height
	 z contains width
	 w contains height*/

Shader "Unlit/spriteAlphaAndSamplingCorrection"
{
	// exposed in inspector
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Alpha("Alpha", Range(0., .5)) = 0.07
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
			float4 _MainTex_TexelSize;
			float _Alpha;

			fixed when_eq(fixed x, fixed y) 
			{
				return 1.0 - abs(sign(x - y));
			}

			fixed4 fragmentShader(v2f vertexShaderOutput) : SV_Target
			{
				fixed2 uv = vertexShaderOutput.uv;
				float w = _MainTex_TexelSize.z;
				float h = _MainTex_TexelSize.w;

				fixed2 vUv = fixed2(uv.x * w, uv.y * h);

				fixed2 alpha = fixed2(0.07, 0.07);

				fixed2 x = frac(vUv);

				fixed2 x_ = clamp(0.5 / alpha * x, 0.0, 0.5) +

					clamp(0.5 / alpha * (x - 1.0) + 0.5,
						0.0, 0.5);


				fixed2 texCoord = (floor(vUv) + x_) / fixed2(w, h);
				fixed4 FragColor = tex2D(_MainTex, texCoord);

				return FragColor;
			}
			ENDCG
		}
	}
}