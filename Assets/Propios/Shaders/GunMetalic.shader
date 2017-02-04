// Shader created with Shader Forge Beta 0.30 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.30;sub:START;pass:START;ps:flbk:Diffuse,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:True;n:type:ShaderForge.SFN_Final,id:1,x:32498,y:32668|diff-2-RGB,spec-934-OUT,emission-673-OUT,transm-4-RGB,olwid-708-OUT,olcol-317-RGB;n:type:ShaderForge.SFN_Tex2d,id:2,x:32928,y:32509,tex:0ec8cd15b4fcf7e43af1a14b0cb1bd73,ntxv:0,isnm:False|TEX-1008-TEX;n:type:ShaderForge.SFN_Cubemap,id:4,x:33026,y:32882,ptlb:Reflection,ptin:_Reflection,cube:3ac9e87b174ff5244941d0c95f348348,pvfc:0|DIR-966-OUT;n:type:ShaderForge.SFN_Color,id:317,x:32796,y:33191,ptlb:outlineColor,ptin:_outlineColor,glob:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:673,x:32891,y:32777,ptlb:Lightning,ptin:_Lightning,glob:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:708,x:32826,y:33058,ptlb:Outline,ptin:_Outline,glob:False,v1:0.04;n:type:ShaderForge.SFN_ValueProperty,id:934,x:32829,y:32689,ptlb:Specular,ptin:_Specular,glob:False,v1:0.5;n:type:ShaderForge.SFN_ViewReflectionVector,id:966,x:33247,y:32882;n:type:ShaderForge.SFN_Tex2dAsset,id:1008,x:33161,y:32472,ptlb:Base,ptin:_Base,glob:False,tex:0ec8cd15b4fcf7e43af1a14b0cb1bd73;proporder:1008-4-317-673-708-934;pass:END;sub:END;*/

Shader "Black&WhiteStudio/GunMetalic" {
    Properties {
        _Base ("Base", 2D) = "white" {}
        _Reflection ("Reflection", Cube) = "_Skybox" {}
        _outlineColor ("outlineColor", Color) = (0,0,0,1)
        _Lightning ("Lightning", Float ) = 0
        _Outline ("Outline", Float ) = 0.04
        _Specular ("Specular", Float ) = 0.5
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 2.0
            uniform float4 _outlineColor;
            uniform float _Outline;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + v.normal*_Outline,1));
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                return fixed4(_outlineColor.rgb,0);
            }
            ENDCG
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 2.0
            uniform float4 _LightColor0;
            uniform samplerCUBE _Reflection;
            uniform float _Lightning;
            uniform float _Specular;
            uniform sampler2D _Base; uniform float4 _Base_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 forwardLight = max(0.0, NdotL );
                float3 backLight = max(0.0, -NdotL ) * texCUBE(_Reflection,viewReflectDirection).rgb;
                float3 diffuse = (forwardLight+backLight) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
////// Emissive:
                float3 emissive = float3(_Lightning,_Lightning,_Lightning);
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_1017 = i.uv0;
                finalColor += diffuseLight * tex2D(_Base,TRANSFORM_TEX(node_1017.rg, _Base)).rgb;
                finalColor += specular;
                finalColor += emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 2.0
            uniform float4 _LightColor0;
            uniform samplerCUBE _Reflection;
            uniform float _Lightning;
            uniform float _Specular;
            uniform sampler2D _Base; uniform float4 _Base_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 forwardLight = max(0.0, NdotL );
                float3 backLight = max(0.0, -NdotL ) * texCUBE(_Reflection,viewReflectDirection).rgb;
                float3 diffuse = (forwardLight+backLight) * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_1018 = i.uv0;
                finalColor += diffuseLight * tex2D(_Base,TRANSFORM_TEX(node_1018.rg, _Base)).rgb;
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
