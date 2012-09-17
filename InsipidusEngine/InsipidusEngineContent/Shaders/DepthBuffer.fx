//The minimum position of the drawn entity.
float3 MinPosition;
//Whether to draw to a depth map.
bool DrawToDepthMap;


//Is filled automatically by sprite batch.
Texture TextureMap;
sampler TextureMapSampler = sampler_state
{
	texture = <TextureMap>;
};
Texture DepthMap;
sampler DepthMapSampler = sampler_state
{
	texture = <DepthMap>;
};

float4 DepthShader(float4 color : COLOR0, float2 texCoords : TEXCOORD0, out float depth : DEPTH0) : COLOR0
{		
	//Get the texture data.
	float4 colorMap = tex2D(TextureMapSampler, texCoords);
	float4 depthMap = tex2D(DepthMapSampler, texCoords);

	//Fiddle with the depth buffer.
	if (colorMap.a == 0) { depth = 0; }
	else { depth = (MinPosition.z + depthMap.b * 255) / 255; }

	//If the target map is a depth map, add the entity's bottom depth to the return color.
	if (DrawToDepthMap) { colorMap.rgb += ((MinPosition.z / 1) / 255); }

	//Return the color data for the texture.
	return colorMap;
}

technique DepthBuffer
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 DepthShader();
    }
}