# 🎨 Custom Shaders in Unity: HLSL / ShaderLab — Basics for Shader Artists & Programmers

This material introduces you to the fundamentals of writing custom shaders in Unity using ShaderLab (wrapper) and HLSL (shader programming language). 
We'll cover syntax, shader structure, and vertex/pixel (fragment) shaders.

> Basic familiarity with Unity's interface and minimal programming skills (C# not required but helpful) is assumed.

---

## 🧱 1. What is a Shader and Why Use It?
A shader is a program that runs on the GPU and determines how each pixel or vertex of a 3D model looks. With shaders you can create:
- material effects (metal, wood, plastic);
- distortions, glow, transparency;
- post-processing (blur, color grading);
- animated textures (water, lava, grass waving).

In Unity, shaders are written in HLSL inside a ShaderLab wrapper, which connects the shader to Material settings in the Inspector.

---

## 🏗️ 2. Shader Structure in ShaderLab
A basic shader looks like this:
```glsl
Shader "Custom/MyFirstShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            // variable declarations and functions
            
            ENDHLSL
        }
    }
}
```

### Breakdown:

| Part | Purpose |
| --- | --- |
| `Shader "Name"` | Shader name in the material selection menu |
| `Properties` | Parameters visible in the Inspector (colors, textures, floats, sliders) |
| `SubShader` | Main shader body. Multiple can exist for different GPU capabilities |
| `Pass` | One rendering pass. Multiple passes possible |
| `HLSLPROGRAM` | Block containing HLSL code |
| `#pragma vertex ...` | Specifies the vertex shader function |
| `#pragma fragment ...` | Specifies the fragment (pixel) shader function |

> [!Important]
> ⚠️ In modern versions of Unity, HLSLPROGRAM is recommended instead of CGPROGRAM.

---

## 🧬 3. Vertex Shader
Vertex shader runs once per vertex of the model. It is responsible for:
- transforming coordinates from local space to screen space;
- passing data to the fragment shader (UVs, normals, color);
- simple vertex animations (e.g., grass swaying).

### Minimal vertex shader example:
```hlsl
v2f vert (appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex); // local -> clip space
    o.uv = v.uv;                               // pass UVs
    return o;
}
```

Where `appdata` is a built-in structure containing vertex data (position, normal, UV), and `v2f` is a custom structure for passing data to the fragment shader.

---

## 🎨 4. Pixel (Fragment) Shader
Fragment shader runs once per pixel (fragment) after rasterization. It determines the final color of each pixel on screen.

### Simple fragment shader example:
```hlsl
fixed4 frag (v2f i) : SV_Target
{
    fixed4 col = tex2D(_MainTex, i.uv); // sample texture color
    col *= _Color;                      // multiply by main color
    return col;
}
```
- `SV_Target` — semantic linking to the output color (render target).
- `tex2D` — samples a texture at given UV coordinates.

---

## 🔁 5. Complete Simple Textured Shader Example
```hlsl
Shader "Custom/TexturedColor"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _Color;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color;
                return col;
            }
            ENDHLSL
        }
    }
}
```

---

## 📦 6. Commonly Used Functions & Include Files

| File / Function | Purpose |
| --- | --- |
| `UnityCG.cginc` | Standard helper functions (coordinate transforms, lighting) |
| `UnityObjectToClipPos(v)` | Transforms a local vertex to screen position (MVP) |
| `tex2D(tex, uv)` | Samples a texture color |
| `_Time` | Global time variable (for animations) |

---

## 🧪 7. Where to Use Custom Shaders
- VFX — fire, water, teleportation, energy shields.
- Stylized graphics — toon shader, watercolor, pixelation.
- GPU animations — swaying plants, waves, particles.
- Post-processing — motion blur, old film effects.

---

## ⚠️ Important Notes for Beginners
- Shaders are compiled per platform — test on your target platform (PC / Mobile / Console).
- Avoid heavy operations in the fragment shader (hurts performance).
- Use Shader Graph for visual shader creation if you are an artist.
- Debug with return `fixed4(1,0,0,1);` — paints everything red, making it obvious which shader is active.

---

### ⭐ If this project was useful, put a star on GitHub!
