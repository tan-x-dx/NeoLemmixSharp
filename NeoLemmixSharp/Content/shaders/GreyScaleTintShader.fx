#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

extern float4 TintColor;

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input): COLOR0
{
	float4 pixelColor = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
	
	pixelColor.rgb = pixelColor.r * 0.2126 + pixelColor.g * 0.7152 + pixelColor.b * 0.0722;
	float4 tintedPixelColor = pixelColor * TintColor;

	return tintedPixelColor;
}

technique Techninque1
{
	pass Pass1
	{
		AlphaBlendEnable = TRUE;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
		PixelShader = compile ps_3_0 MainPS();
	}
};
