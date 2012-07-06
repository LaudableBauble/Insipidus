float _ScreenWidth;
float _ScreenHeight;
float4 _AmbientColor;

float _LightStrength;
float _LightDecay;
float3 _LightPosition;
float4 _LightColor;
float _LightRadius;
float _SpecularStrength;

Texture _NormalMap;
sampler NormalMapSampler = sampler_state
{
	texture = <_NormalMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture _ColorMap;
sampler ColorMapSampler = sampler_state
{
	texture = <_ColorMap>;
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
		
	float3 pixelPosition;
	pixelPosition.x = _ScreenWidth * PSIn.TexCoord.x;
	pixelPosition.y = _ScreenHeight * PSIn.TexCoord.y;
	pixelPosition.z = 0;

	float3 lightDirection = _LightPosition - pixelPosition;
	float3 lightDirNorm = normalize(lightDirection);
	float3 halfVec = float3(0, 0, 1);
		
	float amount = max(dot(normal, lightDirNorm), 0);
	float coneAttenuation = saturate(1.0f - length(lightDirection) / _LightDecay); 
				
	float3 reflect = normalize(2 * amount * normal - lightDirNorm);
	float specular = min(pow(saturate(dot(reflect, halfVec)), 5), amount);
				
	Output.Color = colorMap * coneAttenuation * _LightColor * _LightStrength + (specular * coneAttenuation * _SpecularStrength);

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