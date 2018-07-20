float2 Center = {0, 0};
float2 MinMax <string uiname="MinMax Radius";> = {0, 1};
float2 MinMaxFactor <string uiname="MinMax Multiplier";> = {1, 0};

float RadialFade(float3 pW)
{
	float normalizedDistance = smoothstep(MinMax.x, MinMax.y, distance(Center, pW.xz));
	float result = lerp(MinMaxFactor.x, MinMaxFactor.y, normalizedDistance);

	return result;
}