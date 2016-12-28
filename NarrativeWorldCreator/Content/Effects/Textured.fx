#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
texture Texture;
sampler2D textureSampler = sampler_state {
	Texture = (Texture);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
	float4 Position : SV_POSITION;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput TextureVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, WorldViewProjection);
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 TexturePS(VertexShaderOutput input) : COLOR
{
	return tex2D(textureSampler, input.TextureCoordinate);
}

float4 TextureSelectedPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(textureSampler, input.TextureCoordinate);
	color.r = 1.0f;
	color.g = 1.0f;
	color.b = 1.0f;

	return color;
}

technique TexturedSelected
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL TextureVS();
		PixelShader = compile PS_SHADERMODEL TextureSelectedPS();
	}
};

technique Textured
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL TextureVS();
		PixelShader = compile PS_SHADERMODEL TexturePS();
	}
};