Shader "Unlit/Color" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }
    SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Lighting Off
       ZWrite Off
       Cull Back
       Blend SrcAlpha OneMinusSrcAlpha
	   Color [_Color]
	   Pass {
           SetTexture[_MainTex] {
               constantColor [_Color]
               combine constant lerp(texture) previous
           }
           SetTexture[_MainTex] {
               combine previous * texture
           }
	   }
    }
}