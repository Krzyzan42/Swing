Shader "Custom/SpaceWarp2DWithExtras"
{
    Properties
    {
        [Header(Sprite and Warp)]
        _MainTex ("Sprite Texture (A for Shape)", 2D) = "white" {}
        _Color ("Tint (Sprite Color)", Color) = (1,1,1,1)
        _WarpStrength ("Warp Strength", Range(0, 0.1)) = 0.02
        _WarpFrequency ("Warp Frequency", Range(1, 100)) = 20
        _WarpSpeed ("Warp Speed", Range(0, 10)) = 1
        _DistortionTex ("Distortion Pattern (RG)", 2D) = "bump" {}
        _UseTextureDistortion ("Use Texture Distortion", Float) = 0 // 0 for procedural, 1 for texture

        [Header(Outline)]
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSobelScale ("Outline Sample Scale", Range(0.5, 5)) = 1.5 // Adjusts distance for edge detection, ~thickness
        _OutlineEdgeMultiplier ("Outline Edge Multiplier", Range(0.1, 10)) = 3.0 // Sensitivity of edge detection
        _OutlineThreshold ("Outline Visibility Threshold", Range(0.01, 0.99)) = 0.2 // How strong edge must be to show

        [Header(Stars)]
        _StarsEnabled ("Enable Stars", Float) = 1
        _StarColor ("Star Color", Color) = (1,1,1,1)
        _StarDensity ("Star Grid Density", Range(10, 200)) = 70
        _StarSize ("Star Point Size", Range(0.005, 0.2)) = 0.05 // Relative to grid cell
        _StarBrightness ("Star Max Brightness", Range(0, 5)) = 1.5
        _StarAnimationSpeed ("Star Animation Speed", Range(0, 10)) = 2.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        GrabPass
        {
            "_GrabTexture"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 grabUV : TEXCOORD1;
                float4 color : COLOR;
            };

            // Sprite and Warp
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize; // xy = 1/width, 1/height
            sampler2D _GrabTexture;
            sampler2D _DistortionTex;
            float4 _DistortionTex_ST;
            fixed4 _Color;
            float _WarpStrength;
            float _WarpFrequency;
            float _WarpSpeed;
            float _UseTextureDistortion;

            // Outline
            fixed4 _OutlineColor;
            float _OutlineSobelScale;
            float _OutlineEdgeMultiplier;
            float _OutlineThreshold;

            // Stars
            float _StarsEnabled;
            fixed4 _StarColor;
            float _StarDensity;
            float _StarSize;
            float _StarBrightness;
            float _StarAnimationSpeed;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.grabUV = ComputeGrabScreenPos(o.vertex);
                o.color = v.color * _Color;
                return o;
            }

            // Simple pseudo-random hash function
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453123);
            }

            // Star generation function
            fixed3 generate_stars(float2 uv_for_stars, float time)
            {
                // Scale UVs by density to define grid cells
                float2 grid_cell_coord = floor(uv_for_stars * _StarDensity);
                // Fractional part for position within cell
                float2 intra_cell_uv = frac(uv_for_stars * _StarDensity);

                fixed3 star_light = fixed3(0, 0, 0);

                // Check current cell and its 8 neighbors for stars
                // This makes stars appear more uniformly distributed and avoids sharp cell boundaries
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 cell_to_check = grid_cell_coord + float2(x, y);

                        // Generate a "random" point in each cell using hash of cell_to_check
                        float h1 = hash(cell_to_check + float2(3.71, 1.93)); // For x-offset
                        float h2 = hash(cell_to_check + float2(7.37, 5.19)); // For y-offset
                        float h3 = hash(cell_to_check + float2(1.13, 8.71)); // For twinkle phase/brightness variation

                        // Star's "true" position within its own cell (0 to 1 range)
                        float2 star_center_in_its_cell = float2(h1, h2);

                        // Position of this star relative to the current fragment's intra_cell_uv
                        // (current fragment is in cell `grid_cell_coord`, star is in `cell_to_check`)
                        float2 star_pos_relative_to_frag = star_center_in_its_cell - intra_cell_uv + float2(x, y);

                        float dist_to_star = length(star_pos_relative_to_frag);

                        // Star shape (smooth falloff for a point)
                        float current_star_intensity = smoothstep(_StarSize, _StarSize * 0.25, dist_to_star);
                        // Smaller second param = sharper

                        // Twinkling & Brightness Variation
                        current_star_intensity *= (0.5 + 0.5 * h3); // Base brightness variation per star
                        current_star_intensity *= (0.6 + 0.4 * sin(time * _StarAnimationSpeed + h1 * 6.2831));
                        // Twinkle animation

                        star_light += _StarColor.rgb * current_star_intensity * _StarBrightness;
                    }
                }
                return saturate(star_light);
            }


            fixed4 frag(v2f i) : SV_Target
            {
                // 1. Base sprite alpha (shape of the effect)
                // Alpha from texture, modulated by vertex color and global tint
                float spriteVisibleAlpha = tex2D(_MainTex, i.texcoord).a * i.color.a;

                // 2. Warped background calculation
                float2 screenUV = i.grabUV.xy / i.grabUV.w;
                float2 distortionOffset = float2(0, 0);
                if (_UseTextureDistortion > 0.5)
                {
                    float2 distTexUV = TRANSFORM_TEX(i.texcoord, _DistortionTex);
                    float4 distortionSample = tex2D(_DistortionTex, distTexUV + _Time.y * _WarpSpeed * 0.1);
                    distortionOffset = (distortionSample.rg * 2.0 - 1.0) * _WarpStrength;
                }
                else
                {
                    float timeFactor = _Time.y * _WarpSpeed;
                    float offsetX = sin(screenUV.y * _WarpFrequency + timeFactor) * _WarpStrength;
                    float offsetY = cos(screenUV.x * _WarpFrequency + timeFactor + 1.57) * _WarpStrength;
                    distortionOffset = float2(offsetX, offsetY);
                }
                float2 distortedScreenUV = screenUV + distortionOffset;
                float4 grabTexUV = float4(distortedScreenUV * i.grabUV.w, i.grabUV.z, i.grabUV.w);
                fixed4 warpedBgColor = tex2Dproj(_GrabTexture, grabTexUV);

                // 3. Stars (additive)
                fixed3 starsRgb = fixed3(0, 0, 0);
                if (_StarsEnabled > 0.5)
                {
                    starsRgb = generate_stars(i.texcoord, _Time.y);
                    // Stars are only visible within the main sprite shape (before outline)
                    starsRgb *= step(0.1, spriteVisibleAlpha); // Hard cut-off based on sprite alpha
                }

                // 4. Combine warp and stars for the "content"
                // The sprite's own texture color (from _MainTex RGB) can be blended here if desired
                // For now, just use the warp + stars, alpha defined by spriteVisibleAlpha
                fixed3 contentRgb = warpedBgColor.rgb * i.color.rgb + starsRgb;
                // Modulate warp by sprite tint, add stars
                fixed4 content = fixed4(contentRgb, warpedBgColor.a * spriteVisibleAlpha);

                // 5. Outline calculation (Sobel filter on sprite's alpha)
                float2 texel = _MainTex_TexelSize.xy * _OutlineSobelScale; // Use texel size for consistent scaling

                // Sample direct neighbors (more samples = smoother/thicker potential edge)
                float alpha_t = tex2D(_MainTex, i.texcoord + float2(0, texel.y)).a * i.color.a;
                float alpha_b = tex2D(_MainTex, i.texcoord - float2(0, texel.y)).a * i.color.a;
                float alpha_l = tex2D(_MainTex, i.texcoord - float2(texel.x, 0)).a * i.color.a;
                float alpha_r = tex2D(_MainTex, i.texcoord + float2(texel.x, 0)).a * i.color.a;
                // Diagonal neighbors
                float alpha_tl = tex2D(_MainTex, i.texcoord + float2(-texel.x, texel.y)).a * i.color.a;
                float alpha_tr = tex2D(_MainTex, i.texcoord + float2(texel.x, texel.y)).a * i.color.a;
                float alpha_bl = tex2D(_MainTex, i.texcoord + float2(-texel.x, -texel.y)).a * i.color.a;
                float alpha_br = tex2D(_MainTex, i.texcoord + float2(texel.x, -texel.y)).a * i.color.a;

                // Sobel operator for edge detection on alpha
                float Gx = -alpha_tl - 2 * alpha_l - alpha_bl + alpha_tr + 2 * alpha_r + alpha_br;
                float Gy = -alpha_tl - 2 * alpha_t - alpha_tr + alpha_bl + 2 * alpha_b + alpha_br;
                float edgeStrength = saturate(sqrt(Gx * Gx + Gy * Gy) * _OutlineEdgeMultiplier);

                // Outline appears where edgeStrength is high AND sprite's own alpha is low (i.e., outside the main shape)
                float outlineAmount = edgeStrength * (1.0 - smoothstep(0.0, 0.25, spriteVisibleAlpha));
                outlineAmount = smoothstep(_OutlineThreshold, _OutlineThreshold + 0.15, outlineAmount);
                // Sharpen and threshold

                // 6. Composite: Lerp between content and outline color.
                // Alpha also lerps, or uses max to ensure outline extends visibility.
                fixed3 finalRgb = lerp(content.rgb, _OutlineColor.rgb, outlineAmount);
                float finalAlpha = lerp(content.a, _OutlineColor.a * outlineAmount, outlineAmount); // Blend alpha
                // Alternative for alpha: max(content.a, outlineAmount * _OutlineColor.a);
                // Using lerp makes the outline potentially replace semi-transparent content alpha.
                // Using max ensures the outline area is at least as opaque as the outline color's alpha.
                // Let's use max for a more distinct outline:
                finalAlpha = max(content.a, outlineAmount * _OutlineColor.a);


                return fixed4(finalRgb, finalAlpha);
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}