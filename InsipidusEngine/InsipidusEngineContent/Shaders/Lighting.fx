float ScreenWidth;
float ScreenHeight;
float4 AmbientColor;

float LightStrength;
float LightDecay;
float3 LightPosition;
float4 LightColor;
float LightRadius;
float SpecularStrength;

Texture NormalMap;
sampler NormalMapSampler = sampler_state
{
	texture = <NormalMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture ColorMap;
sampler ColorMapSampler = sampler_state
{
	texture = <ColorMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture DepthMap;
sampler DepthMapSampler = sampler_state
{
	texture = <DepthMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

struct VertexToPixel
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;
};

struct PixelToFrame
{
	float4 Color : COLOR0;
};

VertexToPixel MyVertexShader(float4 inPos: POSITION0, float2 texCoord: TEXCOORD0, float4 color: COLOR0)
{
	VertexToPixel Output = (VertexToPixel)0;
	
	Output.Position = inPos;
	Output.TexCoord = texCoord;
	Output.Color = color;
	
	return Output;
}

PixelToFrame PointLightShader(VertexToPixel PSIn) : COLOR0
{	
	PixelToFrame Output = (PixelToFrame)0;
	
	float4 colorMap = tex2D(ColorMapSampler, PSIn.TexCoord);
	float3 normal = (2.0f * (tex2D(NormalMapSampler, PSIn.TexCoord))) - 1.0f;
	float3 depth = tex2D(DepthMapSampler, PSIn.TexCoord);
		
	float3 pixelPosition;
	pixelPosition.x = ScreenWidth * PSIn.TexCoord.x;
	pixelPosition.y = ScreenHeight * PSIn.TexCoord.y;
	pixelPosition.z = depth;

	/*float3 lightDirection = LightPosition - pixelPosition;
	float3 lightDirNorm = normalize(lightDirection);
	float3 halfVec = float3(0, 0, 1);

	float amount = max(dot(normal, lightDirNorm), 0);
	float coneAttenuation = saturate(1.0f - length(lightDirection) / LightDecay);		
	float3 reflect = normalize(2 * amount * normal - lightDirNorm);
	float specular = min(pow(saturate(dot(reflect, halfVec)), 5), amount);*/

	float3 direction = LightPosition - pixelPosition;
	float distance = 1 / length(direction) * LightStrength;
	float3 dirNorm = normalize(distance);
	float3 halfVec = float3(0, 0, 1);		

	float amount = max(dot(normal + depth, dirNorm), 0);
	float coneAttenuation = saturate(1.0f - length(direction) / LightDecay); 			
	float3 reflect = normalize(2 * amount * normal - dirNorm);
	float specular = min(pow(saturate(dot(reflect, halfVec)), 5), amount);
				
	Output.Color = colorMap * coneAttenuation * LightColor * LightStrength + (specular * coneAttenuation * SpecularStrength);

	return Output;
}

technique DeferredLighting
{
    pass Pass1
    {
		VertexShader = compile vs_2_0 MyVertexShader();
        PixelShader = compile ps_2_0 PointLightShader();
    }
}