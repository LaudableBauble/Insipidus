float _Ambient;
float4 _AmbientColor;
float _LightAmbient;

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

Texture _ShadingMap;
sampler ShadingMapSampler = sampler_state
{
	texture = <_ShadingMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

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

float4 CombinedPixelShader(float4 color : COLOR0, float2 texCoords : TEXCOORD0) : COLOR0
{	
	float4 color2 = tex2D(ColorMapSampler, texCoords);
	float4 shading = tex2D(ShadingMapSampler, texCoords);
	float normal = tex2D(NormalMapSampler, texCoords).rgb;
		
	if (normal > 0.0f)
	{
		//float4 finalColor = color2 * _AmbientColor;
		//finalColor += shading;
		//return finalColor;

		// Darker
		float4 finalColor = color2 * _AmbientColor * _Ambient;
		finalColor += (shading * color2) * _LightAmbient;
		
		return finalColor;
	}
	else { return float4(0, 0, 0, 0); }
}

technique DeferredCombined
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 CombinedPixelShader();
    }
}