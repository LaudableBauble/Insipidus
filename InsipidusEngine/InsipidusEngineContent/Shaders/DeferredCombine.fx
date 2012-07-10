float Ambient;
float4 AmbientColor;
float LightAmbient;

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

Texture ShadingMap;
sampler ShadingMapSampler = sampler_state
{
	texture = <ShadingMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

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

float4 CombinedPixelShader(float4 color : COLOR0, float2 texCoords : TEXCOORD0) : COLOR0
{	
	float4 color2 = tex2D(ColorMapSampler, texCoords);
	float4 shading = tex2D(ShadingMapSampler, texCoords);
	float normal = tex2D(NormalMapSampler, texCoords).rgb;
		
	//if (normal > 0.0f)
	{
		//float4 finalColor = color2 * AmbientColor;
		//finalColor += shading;
		//return finalColor;

		// Darker
		float4 finalColor = color2 * AmbientColor * Ambient;
		finalColor += (shading * color2) * LightAmbient;
		
		return finalColor;
	}
	//else { return float4(0, 0, 0, 0); }
}

technique DeferredCombined
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 CombinedPixelShader();
    }
}