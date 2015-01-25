Shader "Unlit/Color" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Lighting Off
       ZWrite Off
       Cull Back
       Blend SrcAlpha OneMinusSrcAlpha
	   Color [_Color]
	   Pass {
		
	   }
    }
}