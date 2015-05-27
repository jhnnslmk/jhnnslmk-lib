////////////////////////////////////////////////////////////////////////
// litSphere.fx < Ported to dx11 by readme, now called MatCap.fx
// Matballz.fx
//
//	Shader by Charles Hollemeersch http://charles.hollemeersch.net/
//	Public domain.
//
//	Parameters:
//
//		Bumpmap Strength: Scales the bumps to be flatter/steeper
//                        (i.e. more/less towards 0,0,1).
//		Normal Map      : Provides a bumpmap if no map is provided the
//                        geometry normal is used.
//		Lit Sphere Map  : The prelighted sphere to apply as a light. Check
//                        http://www.cs.utah.edu/npr/papers/LitSphere_HTML/
//                        to get some inspiration.
//
//  Edited: 12/13/2009 - Leonardo Covarrubias - For use in Maya
//
//
////////////////////////////////////////////////////////////////////////

cbuffer cbPerDraw : register( b0 )
{
	float4x4 tVP : VIEWPROJECTION;
};


cbuffer cbPerObj : register( b1 )
{
	float4x4 tW    : WORLD;
	float4x4 tWV   : WORLDVIEW;
	float4x4 tWVP  : WORLDVIEWPROJECTION;
	
	float normalPower
	<
	string uiname = "Bumpmap Strength";
	float uimin = 0.0;
	float uimax = 2.0;
	float uistep = 0.1;
	> = 1.0;
	
	bool flipGreen
	<
	string uiname = "Invert Normal Map Green Channel";
	> = false;
	
};

Texture2D normalMap <string uiname="Normal Map";>;
Texture2D litSphereMap <string uiname="Lit Sphere Map";>;

////////////////////////
// Samplers
////////////////////////

SamplerState linearSampler <string uiname="Sampler State";>
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Clamp;
	AddressV = Clamp;
};

////////////////////////
// Exchange structs
////////////////////////

struct vsin
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Tangent  : TANGENT;
	float3 Binormal : BINORMAL;
	float3 Normal   : NORMAL;
};

struct vs2ps
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 TexCoord1 : TEXCOORD1;
	float3 TexCoord2 : TEXCOORD2;
	float3 TexCoord3 : TEXCOORD3;
};

////////////////////////
// Vertex shader
////////////////////////

vs2ps BumpReflectVS(vsin IN)
{
	vs2ps OUT;
	
	// Pos to NDC
	OUT.Position = mul(float4(IN.Position.xyz,1), tWVP);
	
	// Texcoords (for normal map)
	OUT.TexCoord.xy = IN.TexCoord;
	
	// Tangent space vectors
	float3 vtan = IN.Tangent;
	float3 vbinorm = -IN.Binormal;
	float3 vnorm = IN.Normal;
	
	OUT.TexCoord1.xyz = mul(vtan,tWV).xyz;
	OUT.TexCoord2.xyz = mul(vbinorm,tWV).xyz;
	OUT.TexCoord3.xyz = mul(vnorm,tWV).xyz;
	
	return OUT;
}

////////////////////////
// Pixel shader
////////////////////////

float4 BumpReflectPS(vs2ps IN) : SV_Target {
	
	float3 texNormal = normalMap.Sample(linearSampler, IN.TexCoord.xy).xyz;
	if(flipGreen)texNormal.g = 1-texNormal.g;
	texNormal.rgb = texNormal.rgb*2.0-1.0;
	// Fixes normals if no normal map texture is supplied
	if ( dot(texNormal,texNormal) > 2.0 ) {
		texNormal = float3(0,0,1);
	}
	
	// The normalizes can probably go away... but meh...
	float3 T = normalize(IN.TexCoord1.xyz) * normalPower;
	float3 B = normalize(IN.TexCoord2.xyz) * normalPower;
	float3 N = normalize(IN.TexCoord3.xyz);
	
	// Put in world space and renormalize (after scaling)
	float3 worldNorm = normalize(N + texNormal.y * B + texNormal.x * T);
	
	// Swap it around a bit...
	worldNorm.y = - worldNorm.y;
	
	// Look up in the litsphere!
	float3 light = litSphereMap.Sample(linearSampler, worldNorm.xy * 0.5 + 0.5).xyz;
	return float4( light, 1.0 );
}

////////////////////////
// Pixel shader
////////////////////////

technique10 litSphere
{
	pass one
	{
		SetVertexShader( CompileShader( vs_4_0, BumpReflectVS() ) );
		SetPixelShader( CompileShader( ps_4_0, BumpReflectPS() ) );
	}
}

