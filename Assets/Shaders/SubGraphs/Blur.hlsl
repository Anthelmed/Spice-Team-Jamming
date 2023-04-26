
void Blur_float(float2 ScreenPosition, UnityTexture2D _SceneColor, UnitySamplerState sampler_SceneColor, float2 resolution, float Size, out float4 Out)
{
    float Pi = 6.28318530718; // Pi*2
    
    // GAUSSIAN BLUR SETTINGS {{{
    float Directions = 16.0; // BLUR DIRECTIONS (Default 16.0 - More is better but slower)
    float Quality = 3.0; // BLUR QUALITY (Default 4.0 - More is better but slower)

    // GAUSSIAN BLUR SETTINGS }}}
   
    float2 Radius = Size/resolution.xy;
    
    // Normalized pixel coordinates (from 0 to 1)
    float2 uv = ScreenPosition;
    // Pixel colour
    float4 Color =  _SceneColor.Sample(sampler_SceneColor, ScreenPosition);
    
    // Blur calculations
    for( float d=0.0; d<Pi; d+=Pi/Directions)
    {
		for(float i=1.0/Quality; i<=1.0; i+=1.0/Quality)
        {
			Color +=  _SceneColor.Sample(sampler_SceneColor, ScreenPosition + float2(cos(d),sin(d))*Radius*i);		
        }
    }
    
    // Output to screen
    Color /= Quality * Directions - 15.0;
    Out =  Color;

//blurredOutput = _SceneColor.Sample(sampler_SceneColor, screenPosition).r;
}