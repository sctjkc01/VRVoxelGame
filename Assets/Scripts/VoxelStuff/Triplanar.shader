Shader "Custom/Triplanar" {
    Properties{
        _DiffuseTex("Spritesheet", 2D) = "white" {}
        _SpriteSize("Sprites to a Side",float) = 8
        _Fudge("Fudge Factor",float) = 0.001
    }
    SubShader{
        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
#pragma enable_d3d11_debug_symbols
#pragma surface surf Lambert
        struct Input {
            float2 uv_DiffuseTex; // Hijacking UV to define which sprite we're using; x & y goes from 0 to 7
            float3 worldPos; // In Unity Units
            float3 worldNormal;
        };
        sampler2D _DiffuseTex;
        float _SpriteSize;
        float _Fudge;
        void surf(Input IN, inout SurfaceOutput o) {
            float2 spriteStart = IN.uv_DiffuseTex / _SpriteSize; // get where sprite starts
            //spriteStart += float2(_Fudge, _Fudge);
            float2 worldUV = fmod(
                abs(
                    (IN.worldNormal.x * IN.worldPos.zy)
                    + (IN.worldNormal.y * IN.worldPos.xz)
                    + (IN.worldNormal.z * IN.worldPos.xy)
                ) * _Fudge, 1)
                / _SpriteSize; // get the "uv" of the world position
            //worldUV *= 1 - (_Fudge * 2);
            float2 uv = spriteStart + worldUV; // combine

            o.Albedo = tex2D(_DiffuseTex, uv).rgb;
        }
        ENDCG
    }
    Fallback "Diffuse"
}

/*
Shader "Custom/Triplanar" {
    Properties{
        _DiffuseMap("Diffuse Map ", 2D) = "white" {}
        _TextureScale("Texture Scale",float) = 1
        _TriplanarBlendSharpness("Blend Sharpness",float) = 1
    }
    SubShader {
        Tags{ "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf Lambert

        sampler2D _DiffuseMap;
        float _TextureScale;
        float _TriplanarBlendSharpness;

        struct Input {
            float3 worldPos;
            float3 worldNormal;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            // Find our UVs for each axis based on world position of the fragment.
            half2 yUV = IN.worldPos.xz / _TextureScale;
            half2 xUV = IN.worldPos.zy / _TextureScale;
            half2 zUV = IN.worldPos.xy / _TextureScale;
            // Now do texture samples from our diffuse map with each of the 3 UV set's we've just made.
            half3 yDiff = tex2D(_DiffuseMap, yUV);
            half3 xDiff = tex2D(_DiffuseMap, xUV);
            half3 zDiff = tex2D(_DiffuseMap, zUV);
            // Get the absolute value of the world normal.
            // Put the blend weights to the power of BlendSharpness, the higher the value, 
            // the sharper the transition between the planar maps will be.
            half3 blendWeights = pow(abs(IN.worldNormal), _TriplanarBlendSharpness);
            // Divide our blend mask by the sum of it's components, this will make x+y+z=1
            blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);
            // Finally, blend together all three samples based on the blend mask.
            o.Albedo = xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

/*
Shader "Custom/Triplanar" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
    }
    SubShader{
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
*/
*/