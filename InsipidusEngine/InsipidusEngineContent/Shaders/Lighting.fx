float ScreenWidth;
float ScreenHeight;
float3 CameraPosition;
float4 AmbientColor;
float LightStrength;
float LightDecay;
float3 LightPosition;
float4 LightColor;
float LightRadius;
float SpecularStrength;

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
	//Create the output struct.
	PixelToFrame Output = (PixelToFrame)0;
	
	//Get the texture data.
	float4 color = tex2D(ColorMapSampler, PSIn.TexCoord);
	float3 normal = (2.0f * (tex2D(NormalMapSampler, PSIn.TexCoord))) - 1.0f;
	float3 depth = tex2D(DepthMapSampler, PSIn.TexCoord);
	
	//Transform the pixel from screen space into world space using the depth map.
	float3 pixelPosition = depth * 20 * 255;
	pixelPosition.x = ScreenWidth * PSIn.TexCoord.x;
	pixelPosition.y = ScreenHeight * PSIn.TexCoord.y;
	//pixelPosition.z = 0;

	float3 direction = pixelPosition - LightPosition;
	float3 dirNorm = normalize(direction);

	float attenuation = saturate(1.0f - length(direction) / LightRadius);
	float3 diffuse = saturate(dot(normal, -dirNorm));

	float3 reflection = normalize(reflect(-dirNorm, normal));
	float3 cameraDir = normalize(CameraPosition - pixelPosition);
	float specular = pow(saturate(dot(reflection, cameraDir)), SpecularStrength);

	//Output.Color = attenuation * float4(diffuse.rgb, specular);
	Output.Color = float4(saturate(AmbientColor + (color * LightColor * diffuse * 0.6) + (LightColor * SpecularStrength * specular * 0.5 * 0)), 1);
	//Output.Color = color * attenuation * LightColor * LightStrength + (specular * attenuation * SpecularStrength);

	return Output;
}

PixelToFrame PointLightShaderOld(VertexToPixel PSIn) : COLOR0
{	
	//Create the output struct.
	PixelToFrame Output = (PixelToFrame)0;
	
	//Get the texture data.
	float4 color = tex2D(ColorMapSampler, PSIn.TexCoord);
	float3 normal = (2.0f * (tex2D(NormalMapSampler, PSIn.TexCoord))) - 1.0f;
	float3 depth = tex2D(DepthMapSampler, PSIn.TexCoord);
	
	//Transform the pixel from screen space into world space using the depth map.
	float3 pixelPosition = depth * 20 * 255;
	/*pixelPosition.x = ScreenWidth * PSIn.TexCoord.x;
	pixelPosition.y = ScreenHeight * PSIn.TexCoord.y;
	pixelPosition.z = 0;
	depth = 0;*/

	float3 direction = LightPosition - pixelPosition;
	float distance = 1 / length(direction) * LightStrength;
	float3 dirNorm = normalize(direction);
	float3 halfVec = float3(0, 0, 1);

	float amount = max(dot(normal + depth, dirNorm), 0);
	float coneAttenuation = saturate(1.0f - length(direction) / LightDecay); 			
	float3 reflect = normalize(2 * amount * normal - dirNorm);
	float specular = min(pow(saturate(dot(reflect, halfVec)), 5), amount);
				
	Output.Color = color * coneAttenuation * LightColor * LightStrength + (specular * coneAttenuation * SpecularStrength);

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