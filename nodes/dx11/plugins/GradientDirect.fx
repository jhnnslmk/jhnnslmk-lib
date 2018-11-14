//@author: 
//@help: 
//@tags: source
//@credits:

struct vsInput
{
    float4 posObject : POSITION;
	float4 uv: TEXCOORD0;
};

struct psInput
{
    float4 posScreen : SV_Position;
    float4 uv: TEXCOORD0;
};

cbuffer cbPerDraw : register(b0)
{
	float4x4 tVP : LAYERVIEWPROJECTION;

	float4 Color1 <bool color=true;> =float4(0,0,0,1);
	float4 Color2 <bool color=true;> =1;
	float Gamma=1;
	bool ClampColor <bool visible=false;> =0;
};

cbuffer cbPerObj : register( b1 )
{
	float4x4 tW : WORLD;
	float Alpha <float uimin=0.0; float uimax=1.0;> = 1; 
};

cbuffer cbTextureData : register(b2)
{
	float4x4 tTex <string uiname="Texture Transform"; bool uvspace=true; >;
};

psInput VS(vsInput input)
{
	psInput output;
	output.posScreen = mul(input.posObject,mul(tW,tVP));
	output.uv = mul(float4((input.uv.xy*2-1)*float2(1,-1),0,1),tTex)*0.5*float4(1,-1, 0, 1)+0.5;
	return output;
}

float4 pGRAD(psInput input, uniform int interp,uniform bool radial):SV_TARGET{
	float4 c=0;
//	float2 x0=mul(float4((input.uv.xy*2-1)*float2(1,-1),0,1),tTex).xy*0.5*float2(1,-1)+0.5;
	float2 x0=input.uv.xy;
	float fade=x0.x;
	if(radial)fade=length(x0.xy-.5)*2;
	if(interp==2)fade=exp(-(1-fade));
	fade=sign(fade)*pow(abs(fade),Gamma);
	if(ClampColor)fade=saturate(fade);
	if(interp==1)fade=smoothstep(0,1,fade);
	c=lerp(Color1,Color2,fade);
	
	c.a *= Alpha;
    return c;
}

technique10 Linear				{pass P1{SetVertexShader( CompileShader( vs_4_0, VS() ) );SetPixelShader(CompileShader(ps_4_0,pGRAD(0,0)));}}
technique10 Radial				{pass P1{SetVertexShader( CompileShader( vs_4_0, VS() ) );SetPixelShader(CompileShader(ps_4_0,pGRAD(0,1)));}}
technique10 Smoothstep			{pass P1{SetVertexShader( CompileShader( vs_4_0, VS() ) );SetPixelShader(CompileShader(ps_4_0,pGRAD(1,0)));}}
technique10 Smoothstep_Radial	{pass P1{SetVertexShader( CompileShader( vs_4_0, VS() ) );SetPixelShader(CompileShader(ps_4_0,pGRAD(1,1)));}}
technique10 Exponential			{pass P1{SetVertexShader( CompileShader( vs_4_0, VS() ) );SetPixelShader(CompileShader(ps_4_0,pGRAD(2,0)));}}
technique10 Exponential_Radial	{pass P1{SetVertexShader( CompileShader( vs_4_0, VS() ) );SetPixelShader(CompileShader(ps_4_0,pGRAD(2,1)));}}
