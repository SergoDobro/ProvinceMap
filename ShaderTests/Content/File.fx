﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
float2 center;

//StructuredBuffer<TileData> TileDatasReadOnly;
//==============================================================================
// Vertex shader
//==============================================================================
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


//==============================================================================
// Pixel shader 
//==============================================================================

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
}
//==============================================================================
// Techniques
//==============================================================================
technique BasicColorDrawing
{
	pass P0
	{ 
		PixelShader = compile ps_4_0_level_9_1 MainPS();
	}
};