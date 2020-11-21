// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Surface"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_GrassCol("Grass Color", Color) = (1,1,1,1)
		_GrassTex("Grass Tex", float) = 0
		_DirtCol ("Dirt Color", Color) = (1,1,1,1)
		_DirtZone("Dirt Zone", Range(-1, 1)) = 0
		_DirtTex("Dirt Tex", float) = 0
		_StoneCol ("Stone Color", Color) = (1,1,1,1)
		_StoneZone("Stone Zone", Range(-1, 1)) = 0
		_StoneTex("Stone Tex", float) = 0
		_ShineCol("Shine Color", Color) = (1,1,1,1)
		_Shine("Shine", float) = 10
    }
    SubShader
    {

        Pass
        {
			Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
			#pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
			#include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _GrassCol;
			fixed4 _DirtCol;
			fixed4 _StoneCol;
			fixed4 _ShineCol;
			fixed _DirtZone;
			fixed _StoneZone;
			fixed _AmbientLight;
			half _Shine;
			float _GrassTex;
			float _DirtTex;
			float _StoneTex;

			float4 _LightColor0;
			
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				
				half3 normal : TEXCOORD0;
				half3 worldNormal : TEXCOORD1;
				LIGHTING_COORDS(2, 3)
				fixed4 col : COLOR;
			};

			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				fixed4 col;
				float3 normalDirection = normalize(mul(float4(v.normal, 0), unity_WorldToObject).xyz);
				float3 lightDirection;
				float atten = 1;

				lightDirection = normalize(_WorldSpaceLightPos0.xyz);

				half3 normal = mul(unity_ObjectToWorld, v.vertex);
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				if (worldNormal.y < _StoneZone) {
					col = _StoneCol * max(.65, min(.8, (worldNormal.y * 3)));
					v.vertex.y += _StoneTex;
				}
				else if (worldNormal.y < _DirtZone) {
					col = _DirtCol * worldNormal.y;
					v.vertex.y += _DirtTex;
				}
				else {
					col = _GrassCol * worldNormal.y;
					v.vertex.y += _GrassTex;
				}

				float3 diffuseReflection = atten * _LightColor0.xyz * max(0, dot(normalDirection, lightDirection));
				float3 lightFinal = diffuseReflection + UNITY_LIGHTMODEL_AMBIENT.xyz;
				col.xyz *= lightFinal;

				o.col = fixed4(col.xyz, 1);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				TRANSFER_VERTEX_TO_FRAGMENT(o)
				return o;
			}
            


			fixed4 frag(vertexOutput i) : SV_Target
			{
				float4 col = half4(1,1,1,1);
				float atten = LIGHT_ATTENUATION(i);

				col = i.col * atten;
				
                return col;
            }
            ENDCG
        }


		
		Pass
		{
			Tags {"LightMode" = "ForwardAdd"}
			Blend One One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _GrassCol;
			fixed4 _DirtCol;
			fixed4 _StoneCol;
			fixed4 _ShineCol;
			fixed _DirtZone;
			fixed _StoneZone;
			fixed _AmbientLight;
			half _Shine;
			float _GrassTex;
			float _DirtTex;
			float _StoneTex;


			fixed4 _LightColor0;
			

			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				half3 posWorld : TEXCOORD0;
				half3 worldNormal : TEXCOORD1;

			};

			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				float3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);

				if (worldNormal.y < _StoneZone) {
					v.vertex.y += _StoneTex;
				}
				else if (worldNormal.y < _DirtZone) {
					v.vertex.y += _DirtTex;
				}
				else {
					v.vertex.y += _GrassTex;
				}

				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = normalDirection;
				return o;
			}



			fixed4 frag(vertexOutput i) : COLOR
			{
				float3 lightDirection;
				float atten;
				float3 normalDirection = i.worldNormal;
				float3 viewDirection = normalize(_WorldSpaceLightPos0.xyz - i.posWorld.xyz);

				if (_WorldSpaceLightPos0.w == 0) {
					//DirectionalLight
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
					atten = 1;
				}
				else {
					//PointLight
					float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					float distance = length(fragmentToLightSource);
					atten = 1 / distance;
					lightDirection = normalize(fragmentToLightSource);
				}

				float3 diffuseReflection = atten * _LightColor0.xyz * saturate(max(0, dot(normalDirection, lightDirection)));
				float3 specularReflection = atten * _LightColor0.xyz * saturate(max(0, dot(normalDirection, lightDirection))) * pow(max(0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shine) * _ShineCol;
				float3 lightFinal = diffuseReflection + specularReflection + UNITY_LIGHTMODEL_AMBIENT.rgb;
				float4 col;
				col = half4(max(0, lightFinal.xyz * fixed3(1,1,1)), 1);


				return col;
			}

			ENDCG
		}



		
    }
	FallBack"Diffuse"
}
