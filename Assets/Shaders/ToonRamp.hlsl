#ifndef CUSTOM_TOON_INCLUDED
#define CUSTOM_TOON_INCLUDED

void ToonRamp_float(in float3 Normal, in float ToonRampSmoothness, in float4 ClipSpacePos, in float3 WorldPos, in float3 ToonRampTinting, in float ToonRampOffset, in float ToonRampOffsetPoint, in float Ambient, out float3 ToonRampOutput, out float3 Dir)
{
    #ifdef SHADERGRAPH_PREVIEW
       ToonRampOutput = float3(0.5, 0.5, 0);
       Dir = float3(0.5, 0.5, 0);
    #else

       // ВИПРАВЛЕНО: float4 замість half4, щоб прибрати попередження (truncation warning)
       #if SHADOWS_SCREEN
          float4 shadowCoord = ComputeScreenPos(ClipSpacePos);
       #else
          float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
       #endif 
       
       #if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
          Light light = GetMainLight(shadowCoord);
       #else
          Light light = GetMainLight();
       #endif

       half d = dot(Normal, light.direction) * 0.5 + 0.5;
       half toonRamp = smoothstep(ToonRampOffset, ToonRampOffset + ToonRampSmoothness, d);

       float3 extraLights = float3(0, 0, 0);

       InputData inputData = (InputData)0;
       inputData.positionWS = WorldPos;
       inputData.normalWS = Normal;
       inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(WorldPos);
       
       float4 screenPos = float4(ClipSpacePos.x, (_ScaledScreenParams.y - ClipSpacePos.y), 0, 0);
       inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(screenPos);

       uint lightsCount = GetAdditionalLightsCount();
       LIGHT_LOOP_BEGIN(lightsCount)
            
          Light aLight = GetAdditionalLight(lightIndex, WorldPos, half4(1, 1, 1, 1));
          half da = dot(Normal, aLight.direction) * 0.5 + 0.5;
          float3 attenuatedLightColor = aLight.color * (aLight.distanceAttenuation * aLight.shadowAttenuation);
          half toonRampExtra = smoothstep(ToonRampOffsetPoint, ToonRampOffsetPoint + ToonRampSmoothness, da);
          extraLights += (toonRampExtra * attenuatedLightColor);
                
       LIGHT_LOOP_END
       
       toonRamp = toonRamp * light.shadowAttenuation;
       ToonRampOutput = light.color * (toonRamp + ToonRampTinting) + Ambient;
       ToonRampOutput += extraLights;
       Dir = normalize(light.direction);
    
    #endif
}

#endif