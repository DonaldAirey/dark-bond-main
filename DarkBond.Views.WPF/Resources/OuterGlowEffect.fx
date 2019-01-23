/// <summary>The input sample for the shader effect.</summary>
sampler2D input : register(s0);

/// <summary>The partial derivatives of the shader coordinates to pixel coordinates.</summary>
float4 ddxUvDdyUv : register(c0);

/// <summary>the color of the halo glow.</summary>
/// <type>float4</type>
/// <minValue>0</minValue>
/// <maxValue>0xFFFFFF</maxValue>
/// <defaultValue>#FFFFFF</defaultValue>
float4 GlowColor : register(c1);

/// <summary>The thickness of the halo glow.</summary>
/// <type>float</type>
/// <minValue>0</minValue>
/// <maxValue>10</maxValue>
/// <defaultValue>1.0</defaultValue>
float GlowSize : register(c2);

/// <summary>The degree of opacity of the halo glow.</summary>
/// <type>float</type>
/// <minValue>0</minValue>
/// <maxValue>1.0</maxValue>
/// <defaultValue>1.0</defaultValue>
float Opacity : register(c3);

/// <summary>These points are used to create a halo around the texture.</summary>
static const float2 disk[18] = 
{
	float2(0.00000, 1.00000),
	float2(0.34202, 0.93969),
	float2(0.64279, 0.76604),
	float2(0.86603, 0.50000),
	float2(0.98481, 0.17365),
	float2(0.98481, -0.17365),
	float2(0.86603, -0.50000),
	float2(0.64279, -0.76604),
	float2(0.34202, -0.93969),
	float2(0.00000, -1.00000),
	float2(-0.34202, -0.93969),
	float2(-0.64279, -0.76604),
	float2(-0.86603, -0.50000),
	float2(-0.98481, -0.17365),
	float2(-0.98481, 0.17365),
	float2(-0.86603, 0.50000),
	float2(-0.64279, 0.76604),
	float2(-0.34202, 0.93969)
};

/// <summary>A fudge factor so that the value provided for Opacity can be specified from 0.0 to 1.0</summary>
static const float opacityFactor = 7.0;

/// This will produce a glow around the opaque elements of the texture.
float4 main(float2 uv : TEXCOORD) : COLOR
{
	// The GlowSize parameter to the effect are given in pixel units.  This calculation is required to convert the shader units (normalized to 1.0, 1.0 for the
	// entire texture given to this effect) to pixel units.
	float2 diskSize = float2(length(ddxUvDdyUv.xy), length(ddxUvDdyUv.zw)) * GlowSize;

	// The main idea of producing the outer glow effect is to create several copies of the opaque textures and distribute them evenly around using a pre-calculated
	// Poisson Disk Distribution.  Then the original texture is overlayed on top of the blurred copies in an alpha blending operation that simulates a second pass.
	float glowOpacity = 0.0;
    for(int angle = 0; angle < 18; angle++)
        glowOpacity += tex2D(input, uv.xy + (disk[angle] * diskSize)).a;
	glowOpacity = min(glowOpacity / 18.0 * Opacity * opacityFactor, Opacity);

	// This retrieves the original pixel at the given location.
    float4 sample = tex2D(input, uv);

	// The final product is a combination of the original pixel -- which will override the background or glow effect -- and the glow effect.
	return float4(sample.rgb * sample.a + (GlowColor.rgb * glowOpacity) * (1.0 - sample.a), min(sample.a + glowOpacity, 1.0));
}
