float3 _Position;

//Is filled automatically by sprite batch.
Texture _TextureMap;
sampler TextureMapSampler = sampler_state
{
	texture = <_TextureMap>;
};

Texture _DepthMap;
sampler DepthMapSampler = sampler_state
{
	texture = <_DepthMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

float4 DepthShader(float4 color : COLOR0, float2 texCoords : TEXCOORD0, out float depth : DEPTH0) : COLOR0
{		
	//Get the texture data.
	float4 colorMap = tex2D(TextureMapSampler, texCoords);
	float4 depthMap = tex2D(DepthMapSampler, texCoords);

	//Fiddle with the depth buffer.
    depth = (_Position.z + tex2D(DepthMapSampler, texCoords).b) / 10;
	//depth = _Position.z / 100;

	//Return the color data for the texture.
	//colorMap.rgb = depth;
	return colorMap;
}

technique DepthBuffer
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 DepthShader();
    }
}