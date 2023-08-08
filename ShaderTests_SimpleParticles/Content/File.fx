#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif



matrix matrixx;
texture displacementMap; // наша карта

sampler TextureSampler : register(s0); // тут та текстура, которая отрисовалась на экран
SamplerState DisplacementSampler
{ 
    Texture = <displacementMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};


struct VertexShaderOutput
{
    float4 Position : SV_Position0;
    float2 texCoord : TEXCOORD0;
    float4 color : COLOR0;
};
struct VertexIn
{
    float4 Position : SV_Position0;
    float4 color : COLOR0;
    float3 texCoord : TEXCOORD0;
};
float4 mainPS(VertexShaderOutput input) : COLOR0
{

    /* PIXEL DISTORTION BY DISPLACEMENT MAP */
     float3 displacement = tex2D(DisplacementSampler, input.texCoord); // получаем R,G,B из карты
 /////*    input.texCoord.x *= (1 - input.texCoord.y / 10);
 ////    input.texCoord.x += - 0.05 - input.texCoord.y*0.05;
 ////    input.texCoord.x *= 1.3;*/
     //input.texCoord.x *= 2;
     //input.texCoord.y += 0.2;

     // Offset the main texture coordinates.
     //input.texCoord.x += displacement.r * 0.1; // меняем позицию пикселя
     return float4(1,1,1,1);
     float lr = displacement.r;

     if (displacement.r == 0)
     {
         return float4(0, 0, 0, 0);
     }
     float bst = 0;
     //for (float i = 0; i < 0.02; i+= 0.01)
     //{ 
     //    if (tex2D(DisplacementSampler, input.texCoord + float2(0, i)).r > lr)
     //    {
     //        lr = tex2D(DisplacementSampler, input.texCoord + float2(0, i)).r + 0.2;
     //        bst = i;
     //    }
     //}
     //input.texCoord.y *= 2;
     //input.texCoord.y -= 0.5;
     float4 output = tex2D(DisplacementSampler, input.texCoord + float2(0, bst)); // получаем цвет для нашей текстуры TextureSampler
     //input.texCoord.y += pow(displacement.r, 1.1)  * 0.6; // меняем позицию пикселя
    // input.texCoord.y *= 3;

    if (input.texCoord.x > 1 ||
        input.texCoord.x < 0 ||
        input.texCoord.y > 1 ||
        input.texCoord.y < 0)
    {
        return float4(0, 0, 0, 0);
    }
     return input.color * output;
}
VertexShaderOutput VS(in VertexShaderOutput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = input.Position*0.001;
    if (input.Position.x < 1)
    {
        output.Position.x = 0;
    }
    else {
        output.Position.x = 1;
    }
    if (input.Position.y < 1)
    {
        output.Position.y = 0;
    }
    else {
        output.Position.y = 1;
    }
    output.color = input.color;
    output.texCoord = input.texCoord; // added
    return output;
}
technique DistortionPosteffect
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VS(); // компилируем шейдер
        PixelShader = compile PS_SHADERMODEL mainPS(); // компилируем шейдер
    }
}
