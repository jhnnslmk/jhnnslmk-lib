float Factor = 1.0;
float Phase = 0;
float innerRadius <string uiname="Inner Radius";>;
float outerRadius <string uiname="Outer Radius";> = 1.0;
float4 GridToSegment(float4 Pos, float2 TexCd)
{
    const float PI = 3.14159265359;
    float range = (((TexCd.x)*-Factor)+0.25-Phase)*PI*2;
    float radius = outerRadius-(TexCd.y*(outerRadius-innerRadius));
    sincos(range,Pos.x,Pos.y);
    Pos.xy*=radius;
//    float2 addquarter;
    Pos.xy/=2.;
    return Pos;
}