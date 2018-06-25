float2 CartesianToPolar(float2 uv, bool2 mirror)
{
	float pi = 3.1415;
	float r = length(uv.xy);
	float phi = atan2(uv.y,uv.x)/(2*pi);
	return float2(phi, r) * lerp(1, -1, mirror);
}