//@author: woei
//@help: fx replacement for segment dx9
//@tags: segment, circle

// PARAMETERS ------------------------------------------------------------------
//transforms
float4x4 tW: WORLD;        //the models world matrix
float4x4 tV: VIEW;         //view matrix as set via Renderer (EX9)
float4x4 tP: PROJECTION;   //projection matrix as set via Renderer (EX9)
float4x4 tWVP: WORLDVIEWPROJECTION;

#include "SegmentVS.fxh"

//material properties
float4 cAmb <bool color = true; String uiname="Color";>  = {1, 1, 1, 1};
float Alpha = 1;

//texture
Texture2D inputTexture <string uiname="Texture";>;
SamplerState linearSampler <string uiname="Sampler State";>
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4x4 tTex: TEXTUREMATRIX <string uiname="Texture Transform";>;

struct vs2ps
{
    float4 Pos : SV_Position;
    float4 TexCd : TEXCOORD0;
//	float4 norm : NORMAL;
};

// VERTEXSHADERS ---------------------------------------------------------------
vs2ps VS(
    float4 Pos : POSITION,
    float4 TexCd : TEXCOORD0)
{
    vs2ps output;

    output.Pos = mul(GridToSegment(Pos, TexCd), tWVP);
//	output.norm = float4(0, -1, 0, 0);
    output.TexCd = mul(TexCd, tTex);

    return output;
}
// PIXELSHADERS ----------------------------------------------------------------
float4 PS(vs2ps In): SV_Target
{
    float4 col = inputTexture.Sample(linearSampler, In.TexCd) * cAmb;
    col.a *= Alpha;
    return col;
}
// TECHNIQUES ------------------------------------------------------------------
technique11 Segment
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
	}
}