#ifndef VAT_CUSTOM_NODES
#define VAT_CUSTOM_NODES

inline float CalcVatAnimationTime(in float time, in float3 animData)
{
    // (time % length) * fps
    return (time % animData.x) * animData.y;
}

inline float4 CalcVatTexCoord(in float vertexId, in float animationTime, in float4 texelSize, in float offset)
{
    float x = vertexId + 0.5;
    float y = animationTime + 0.5;

    return float4(x, y + offset, 0, 0) * texelSize;
}

void ApplyVatUnlit_float(in float vertexId, in float time, in float3 animData, in UnityTexture2D vertexVat, out float3 vertex)
{
    float4 uv = CalcVatTexCoord(vertexId, CalcVatAnimationTime(time, animData), vertexVat.texelSize, animData.z);
    vertex = tex2Dlod(vertexVat, uv);
}

void ApplyVat_float(in float vertexId, in float time, in float3 animData, in UnityTexture2D vertexVat, in UnityTexture2D normalVat,
    out float3 vertex, out float3 normal)
{
    float4 uv = CalcVatTexCoord(vertexId, CalcVatAnimationTime(time, animData), vertexVat.texelSize, animData.z);
    vertex = tex2Dlod(vertexVat, uv);
    normal = tex2Dlod(normalVat, uv);
}
#endif