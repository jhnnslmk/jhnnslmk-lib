interface iChannel{
   float Channel(float4 c); 
};
class cRedChannel : iChannel{
   float Channel(float4 c) { 
		return c.r;
	}
};
class cGreenChannel : iChannel{
   float Channel(float4 c) { 
	   return c.g;
   }
};
class cBlueChannel : iChannel{
   float Channel(float4 c) { 
	   return c.b;
   }
};
class cAlphaChannel : iChannel{
   float Channel(float4 c) { 
	   return c.a;
   }
};

cRedChannel Red;
cGreenChannel Green;
cBlueChannel Blue;
cAlphaChannel Alpha;
iChannel ControlChannel <string uiname="Control Channel";string linkclass="Red,Green,Blue,Alpha";> = Red;