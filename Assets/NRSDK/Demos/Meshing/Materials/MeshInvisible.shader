Shader "NRSDK/Invisible"
{
    SubShader
    {
        Tags { "Queue" = "Geometry" }
 
        ColorMask 0
        ZWrite On

        Pass {}
    }
}
