#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0 
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1 
#endif


#define GroupSize 256
struct TileData
{
	float3 colorA;
    float3 colorB;
};
RWStructuredBuffer<TileData> TileDatas;
float totalArr;
float DeltaTime;
[numthreads(GroupSize, 1, 1)]
void CS(uint3 localID : SV_GroupThreadID, uint3 groupID : SV_GroupID,
    uint  localIndex : SV_GroupIndex, uint3 globalID : SV_DispatchThreadID)
{
    TileData d = TileDatas[globalID.x]; 
    TileDatas[globalID.x] = d;
}

//==============================================================================
// Vertex shader
//==============================================================================

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexIn
{
	float3 Position : POSITION0;
	float4 Color : COLOR0;
};
 
struct VertexOut
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

StructuredBuffer<TileData> DatasReadOnly;
VertexOut VS(in VertexIn input)
{
    VertexOut output = (VertexOut)0;
    output.Position = float4(input.Position, 0);
    output.Color = input.Color;
	//VertexOut output = (VertexOut)0;
	// 
 //   //output.Position = float4(DeltaTime,1,1, 1);
	////output.Color = input.Color;
	// 

	////output.Color = float4(TileDatas[0].tileId, 0, 0, 1);;


 //   for (int i = 0; i < totalArr; i++)
 //   {
 //       if (input.Color.x == DatasReadOnly[i].colorA.x)
 //       {
 //           output.Color = float4(DatasReadOnly[i].colorB, 1);
 //       }
 //   }

	return output;
}

//==============================================================================
// Pixel shader 
//==============================================================================
float4 PS(VertexOut input) : COLOR
{
	//float sdt = 10 + 3000 * pow((sin(DeltaTime / 1) + 1) / 2, 3);
	//float4 clr = tex2D(SpriteTextureSampler, float2(round((input.TextureCoordinates.x - 0.5) * sdt) / sdt + 0.5,round((input.TextureCoordinates.y - 0.5) * sdt) / sdt + 0.5));
    float4 clr = tex2D(SpriteTextureSampler, float2(round(input.TextureCoordinates.x * 1000)/ 1000,round(input.TextureCoordinates.y* 1000)/ 1000));
	//clr.r = round(clr.r * 255) / 255;
	//clr.g = round(clr.g * 255) / 255;
	//clr.b = round(clr.b * 255) / 255;
	float d = 0.000;
    for (int i = 0; i < totalArr; i++)
    {
		if (clr.x == DatasReadOnly[i].colorA.x - d && clr.x <= DatasReadOnly[i].colorA.x + d &&
			clr.y == DatasReadOnly[i].colorA.y - d && clr.y <= DatasReadOnly[i].colorA.y + d &&
			clr.z == DatasReadOnly[i].colorA.z - d && clr.z <= DatasReadOnly[i].colorA.z + d
			)
        {
			//clr = float4(mix(DatasReadOnly[i].colorA, DatasReadOnly[i].colorB, float3(0.5,0.5,0.5)),1);
            clr = float4(DatasReadOnly[i].colorB.x, sin(DeltaTime / 10) * DatasReadOnly[i].colorB.y, sin(DeltaTime - DatasReadOnly[i].colorB.x) * DatasReadOnly[i].colorB.z, 1);
        }
    }
	return clr;
}

//==============================================================================
// Techniques
//==============================================================================
technique Tech0
{
	pass P0
	{
		//VertexShader = compile vs_5_0 VS();
		PixelShader = compile ps_5_0 PS();
        //ComputeShader = compile cs_5_0 CS();
	}
};