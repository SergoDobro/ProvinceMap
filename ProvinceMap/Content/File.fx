#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float rnd;
 float intensity;
  float colorswap;
 float weaponcharge;

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
 


//float rand(vec2 co)
//{
//	float a = 12.9898f;
//	float b = 78.233f;
//	float c = 43758.5453f;
//	float dt = dot(co.xy, float2(a, b));
//	float sn = mod(dt, 3.14f);
//	return fract(sin(sn) * c);
//}
//float4 Electrify(float4 c)
//{
//	float f = rand(vec2(int(c.g / 3 * rnd), int(c.r / 3 * rnd))) * rand(vec2(int(c.g / 3 * rnd), int(c.r / 3 * rnd)));
//	float c1 = tex2D(SpriteTextureSampler, input.TextureCoordinates).r + tex2D(SpriteTextureSampler, input.TextureCoordinates).g + tex2D(SpriteTextureSampler, input.TextureCoordinates).b;
//	c1 *= 3;
//
//	float col = (1.0f - min(c1, 0.5f)) * f;
//	float4 color2 = float4(-col * 0.3f, col * 0.9f, col, 0);
//	c += color2 * 1;
//
//	return c;
//}


float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color; 
	return (color);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};