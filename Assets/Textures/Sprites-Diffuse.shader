Shader "Sprites/CustomDiffuse"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_EmmissiveTex("Emmissive Texture", 2D) = "black" {}
		_EmmissivePower("Emmissive Power", Range(0,1)) = 0
		_MaxEmissivePower("Max Emissive Power", float) = 0
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf SimpleLambert vertex:vert nofog keepalpha
		#pragma multi_compile _ PIXELSNAP_ON
		#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

		sampler2D _MainTex;
		sampler2D _EmmissiveTex;
		float _EmmissivePower;
		float _MaxEmissivePower;
		fixed4 _Color;
		sampler2D _AlphaTex;
		float _AlphaSplitEnabled;
		
		struct SurfaceOutputCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;

			fixed2 UV;
		};

		half4 LightingSimpleLambert(SurfaceOutputCustom s, half3 lightDir, half atten) {
			float power = _EmmissivePower*_MaxEmissivePower;
			fixed4 e = tex2D(_EmmissiveTex, s.UV) * power;
			half4 c;
			c.rgb = s.Albedo * saturate(_LightColor0.rgb + e.rgb*e.a) * atten;
			c.a = max(s.Alpha, e.a);
			return c;
		}

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			#if defined(PIXELSNAP_ON)
			v.vertex = UnityPixelSnap (v.vertex);
			#endif
			
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
		}

		fixed4 SampleSpriteTexture (float2 uv)
		{
			fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
			color.a = tex2D (_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

			return color;
		}

		void surf (Input IN, inout SurfaceOutputCustom o)
		{
			fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb * c.a;	
			o.Alpha = c.a;
			o.UV = IN.uv_MainTex;
		}
		ENDCG
	}

Fallback "Transparent/VertexLit"
}
