//@author: vvvv group
//@help: Aligns the orientation of a geometry to the camera.
//@tags: billboard, view space
//@credits:

Texture2D texture2d <string uiname="Texture";>;

SamplerState linearSampler : IMMUTABLE
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};
 
cbuffer cbPerDraw : register( b0 )
{
	float4x4 tVP : LAYERVIEWPROJECTION;	
};

cbuffer cbPerObj : register( b1 )
{
	float4x4 tW : WORLD;
	float4x4 tWV: WORLDVIEW;
	float4x4 tP: PROJECTION;
	float4 cAmb <bool color=true;String uiname="Color";> = { 1.0f,1.0f,1.0f,1.0f };
	float4x4 tA <string uiname="Transform in Viewspace";>;
	float2 Size = float2 (0.2, 0.2);
	bool fixedSize <string uiname = "Fixed Size"; > = false;
};

struct VS_IN
{
	float4 PosO : POSITION;
	float4 TexCd : TEXCOORD0;

};

struct vs2ps
{
    float4 PosWVP: SV_Position;
    float4 TexCd: TEXCOORD0;
};

vs2ps VS(VS_IN input)
{
    vs2ps output;
    
	float4 pos =  mul(float4(0, 0, 0, 1),tWV);
	
	
	//position
	if (fixedSize)
	{   
		// Apply Projection to the world's view position
		pos = mul (pos, tP);
		
		// Make a perspective division
		pos.xyz /= pos.w;
				
		float aX = tP[0][0];
		float aY = tP[1][1];
		float3 aspectRatio = float3 (aX, aY, 1);

		// Add the Object's position multiplied by the viewspace transform
		// to the WorldViewProjected position multiplied by the Aspect Ratio
		output.PosWVP = float4(pos.xyz + mul(input.PosO * float4(Size,1,1), tA).xyz * aspectRatio, 1);
		
	}
	else
	{

    pos.xyz += mul(input.PosO, tA).xyz;
	output.PosWVP  = mul(pos, tP);
		
	}
	
	output.TexCd = input.TexCd;
    return output;
	
}




float4 PS(vs2ps In): SV_Target
{
    float4 col = texture2d.Sample(linearSampler,In.TexCd.xy) * cAmb;
    return col;
}


technique10 Constant
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
	}
}




