/// <summary>The input sample for the shader effect.</summary>
sampler2D input : register(s0);

/// <summary>The saturation of the image (amount of color).</summary>
/// <type>float</type>
/// <minValue>0</minValue>
/// <maxValue>1.0</maxValue>
/// <defaultValue>0.0</defaultValue>
float saturation : register(c0);

/// <summary>
/// Changes the amount of color (saturation) in a texture.
/// </summary>
/// <copyright>Copyright © 2010-2012 - Teraque, Inc.  All Rights Reserved.</copyright>
float4 main(float2 uv : TEXCOORD) : COLOR
{
	// Sample the color, turn it to gray and then factor back the color according to the amount of color saturation requested.  A value of 0 for
	// saturation will produce a completely gray image.  A saturation of 1.0 produces the original texture.
    float4 color = tex2D(input, uv);
	float gray = dot(color.rgb, float3(0.3, 0.59, 0.11));
	return float4((color.rgb - gray) * saturation + gray, color.a);
}