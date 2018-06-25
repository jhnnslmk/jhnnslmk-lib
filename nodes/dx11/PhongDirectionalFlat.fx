float4x4 tW : WORLD;
float4x4 tVP : VIEWPROJECTION;
float4x4 tWVP : WORLDVIEWPROJECTION;
float4x4 tWV : WORLDVIEW;

struct vsin
{
	float4 pos : POSITION;
	float3 norm : NORMAL;
};

struct vs2gs
{
    float4 pos : POSITION;
};

struct psIn
{
    float4 posScreen : SV_Position;
    float3 LightDirV: TEXCOORD0;
    float3 NormV: TEXCOORD1;
    float3 ViewDirV: TEXCOORD2;
};

vs2gs VS(vsin input)
{
	//Passtrough in that case, since we will process in gs
	
	//We don't need normals we will calculate them on the fly
	vs2gs output;
	output.pos =input.pos;
    return output;
}


float eps : EPSILON = 0.000001f;

[maxvertexcount(3)]
void GS(triangle vs2gs input[3], inout TriangleStream<psIn> gsout)
{
	psIn o;
	
	//Get triangle face direction
	float3 f1 = input[1].pos.xyz - input[0].pos.xyz;
    float3 f2 = input[2].pos.xyz - input[0].pos.xyz;
    
	//Compute flat normal
	float3 norm = normalize(cross(f1, f2));

	//Convert into view space
	float3 normv = mul(float4(norm,0),tWV).xyz;
	normv = normalize(normv);
	
	o.norm = float4(normalize(normv),1);

	//Transform triangles
	o.pos = mul(input[0].pos,tWVP);
	gsout.Append(o);
	
	o.pos = mul(input[1].pos,tWVP);
	gsout.Append(o);
	
	o.pos = mul(input[2].pos,tWVP);
	gsout.Append(o);
}

float4 PS(psIn input): SV_Target
{
	/*Set normals range form -1 to 1 to 0 -> 1
	for friendly display */
    return float4(input.norm.xyz*0.5+0.5,1);
}

technique10 RenderFlat
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetGeometryShader( CompileShader( gs_4_0, GS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
	}
}





