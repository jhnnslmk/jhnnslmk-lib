//@author: vvvv group
//@help: draws a mesh with a constant color
//@tags: template, basic
//@credits:

// --------------------------------------------------------------------------------------------------
// PARAMETERS:
// --------------------------------------------------------------------------------------------------

//transforms
float4x4 tW: WORLD;        //the models world matrix
float4x4 tV: VIEW;         //view matrix as set via Renderer (EX9)
float4x4 tP: PROJECTION;   //projection matrix as set via Renderer (EX9)
float4x4 tWVP: WORLDVIEWPROJECTION;

//material properties
float4 c0 <bool color=true; String uiname="Color Inner";>  = {1, 1, 1, 1};
float4 c1 <bool color=true; String uiname="Color Outer";>  = {1, 1, 1, 0};
float4 Tint <bool color=true;> = {1, 1, 1, 1};
float Alpha = 1;

//the data structure: vertexshader to pixelshader
//used as output data with the VS function
//and as input data with the PS function
struct vs2ps
{
	float4 Pos : SV_Position;
	float4 TexCd : TEXCOORD0;
};

// --------------------------------------------------------------------------------------------------
// VERTEXSHADERS
// --------------------------------------------------------------------------------------------------

vs2ps VS(
float4 Pos : POSITION,
float4 TexCd : TEXCOORD0)
{
	//inititalize all fields of output struct with 0
	vs2ps Out;
	
	//transform position
	Out.Pos = mul(Pos, tWVP);
	
	//transform texturecoordinates
	Out.TexCd = TexCd;
	
	return Out;
}

// --------------------------------------------------------------------------------------------------
// PIXELSHADERS:
// --------------------------------------------------------------------------------------------------

float r <string uiname="Radius";> = 0.5f;
float w <string uiname="Width";> = 0.05f;
float f0 <string uiname="Fade Inner";> = 0.025f;
float f1 <string uiname="Fade Outer";> = 0.025f;
float p <string uiname="Phase";> = 1.0f;

static const float PI = 3.14159265f;

float4 PS(vs2ps In): SV_Target
{
	float4 col = 0.0f.xxxx;
	
	float2 uv = (In.TexCd.xy - 0.5) * 2;
	
	float phi = (atan2(uv.y/2,uv.x/2))/(2*PI)+0.5f;
	
	float d = distance(float2(0,0), uv);
	float r0 = r - w/2;
	float r1 = r + w/2;
	
	float4 b = 0.0f.xxxx;
	
	bool inside = ((d < r0) && (d > r0 - f0));
	bool outside = ((d > r1) && (d < r1 + f1));
	
	//	solid part in the center
	col = c0 * (abs(r - d) <= w/2);
	
	//	inner fading
	col = saturate(col + lerp(c1, c0, smoothstep(r0 - f0, r0, d))*inside);
	
	//	outer fading
	col = saturate(col + lerp(c0, c1, smoothstep(r1, r1 + f1, d))*outside);
	
	// Apply Phase
	col.a *= phi <= p;
	
	col.a *= Alpha;
	
	col *= Tint;
	
	return col;
}

// --------------------------------------------------------------------------------------------------
// TECHNIQUES:
// --------------------------------------------------------------------------------------------------

technique11 HaloRing
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
	}
}
