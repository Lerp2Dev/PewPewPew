// Shader created with Shader Forge Beta 0.30 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.30;sub:START;pass:START;ps:flbk:Diffuse,lico:1,lgpr:1,nrmq:1,limd:2,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:True;n:type:ShaderForge.SFN_Final,id:1,x:32147,y:32573|diff-12-OUT,spec-28-OUT,normal-116-RGB,transm-143-OUT,olwid-372-OUT,olcol-392-OUT;n:type:ShaderForge.SFN_Tex2d,id:4,x:32322,y:32203,ptlb:Base,ptin:_Base,tex:f9c6074c5a78e71448f9efd3013cadda,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:11,x:32601,y:32407,ptlb:Color,ptin:_Color,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Blend,id:12,x:32508,y:32407,blmd:1,clmp:False|SRC-4-RGB,DST-11-RGB;n:type:ShaderForge.SFN_ValueProperty,id:28,x:32382,y:32614,ptlb:Specular,ptin:_Specular,glob:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:34,x:32761,y:32840,ptlb:Specmap,ptin:_Specmap,tex:7ae1142e9b3a72048960dd9561e0e0d9,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:116,x:32802,y:32658,ptlb:Normal,ptin:_Normal,tex:3a3c0acc807e9264ca0b32602a928a9a,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Cubemap,id:142,x:32761,y:33026,ptlb:Reflection,ptin:_Reflection,cube:3ac9e87b174ff5244941d0c95f348348,pvfc:0|DIR-231-OUT;n:type:ShaderForge.SFN_Blend,id:143,x:32544,y:33041,blmd:16,clmp:True|SRC-34-RGB,DST-142-RGB;n:type:ShaderForge.SFN_ViewReflectionVector,id:231,x:32970,y:32991;n:type:ShaderForge.SFN_ValueProperty,id:372,x:32528,y:32893,ptlb:Outline,ptin:_Outline,glob:False,v1:0.025;n:type:ShaderForge.SFN_Color,id:391,x:32825,y:32513,ptlb:OutlineColor,ptin:_OutlineColor,glob:False,c1:0.2794118,c2:0.2794118,c3:0.2794118,c4:1;n:type:ShaderForge.SFN_Blend,id:392,x:32527,y:32582,blmd:1,clmp:True|SRC-12-OUT,DST-391-RGB;proporder:4-11-28-34-116-142-372-391;pass:END;sub:END;*/

Shader "BlackWhiteStudio/ToonyCuteScenario" {
    Properties {
        _Base ("Base", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Specular ("Specular", Float ) = 1
        _Specmap ("Specmap", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
        _Reflection ("Reflection", Cube) = "_Skybox" {}
        _Outline ("Outline", Float ) = 0.025
        _OutlineColor ("OutlineColor", Color) = (0.2794118,0.2794118,0.2794118,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
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
            uniform sampler2D _Base; uniform float4 _Base_ST;
            uniform float4 _Color;
            uniform float _Outline;
            uniform float4 _OutlineColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + v.normal*_Outline,1));
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_502 = i.uv0;
                float3 node_12 = (tex2D(_Base,TRANSFORM_TEX(node_502.rg, _Base)).rgb*_Color.rgb);
                return fixed4(saturate((node_12*_OutlineColor.rgb)),0);
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
            uniform sampler2D _Base; uniform float4 _Base_ST;
            uniform float4 _Color;
            uniform float _Specular;
            uniform sampler2D _Specmap; uniform float4 _Specmap_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform samplerCUBE _Reflection;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_503 = i.uv0;
                float3 normalLocal = tex2D(_Normal,TRANSFORM_TEX(node_503.rg, _Normal)).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 forwardLight = max(0.0, NdotL );
                float3 node_143 = saturate(round( 0.5*(tex2D(_Specmap,TRANSFORM_TEX(node_503.rg, _Specmap)).rgb + texCUBE(_Reflection,viewReflectDirection).rgb)));
                float3 backLight = max(0.0, -NdotL ) * node_143;
                float3 diffuse = (forwardLight+backLight) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float3 node_12 = (tex2D(_Base,TRANSFORM_TEX(node_503.rg, _Base)).rgb*_Color.rgb);
                finalColor += diffuseLight * node_12;
                finalColor += specular;
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
            uniform sampler2D _Base; uniform float4 _Base_ST;
            uniform float4 _Color;
            uniform float _Specular;
            uniform sampler2D _Specmap; uniform float4 _Specmap_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform samplerCUBE _Reflection;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_504 = i.uv0;
                float3 normalLocal = tex2D(_Normal,TRANSFORM_TEX(node_504.rg, _Normal)).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 forwardLight = max(0.0, NdotL );
                float3 node_143 = saturate(round( 0.5*(tex2D(_Specmap,TRANSFORM_TEX(node_504.rg, _Specmap)).rgb + texCUBE(_Reflection,viewReflectDirection).rgb)));
                float3 backLight = max(0.0, -NdotL ) * node_143;
                float3 diffuse = (forwardLight+backLight) * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 specular = attenColor * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float3 node_12 = (tex2D(_Base,TRANSFORM_TEX(node_504.rg, _Base)).rgb*_Color.rgb);
                finalColor += diffuseLight * node_12;
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
