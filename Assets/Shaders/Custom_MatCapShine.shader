Shader "Custom/MatCapShine"
{
  Properties
  {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _MatCap ("MatCap (RGB)", 2D) = "white" {}
    _MatCapStrength ("MatCapStrength", Range(0, 3)) = 1
    _ShineRamp ("ShineRamp", 2D) = "white" {}
    _ShineStrength ("Shine Strength", Range(0, 5)) = 2
    _ScrollSpeed ("Shine Scroll Speed", float) = 10
  }
  SubShader
  {
    Tags
    { 
      "RenderType" = "Opaque"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "LIGHTMODE" = "ALWAYS"
        "RenderType" = "Opaque"
      }
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _Time;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixV;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MainTex_ST;
      uniform float4 _ShineRamp_ST;
      uniform float _ScrollSpeed;
      uniform sampler2D _MainTex;
      uniform sampler2D _ShineRamp;
      uniform sampler2D _MatCap;
      uniform float _MatCapStrength;
      uniform float _ShineStrength;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float2 xlv_TEXCOORD2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float2 xlv_TEXCOORD2 :TEXCOORD2;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          tmpvar_1.w = 1;
          tmpvar_1.xyz = in_v.vertex.xyz;
          float4 v_2;
          v_2.x = conv_mxt4x4_0(unity_WorldToObject).x;
          v_2.y = conv_mxt4x4_1(unity_WorldToObject).x;
          v_2.z = conv_mxt4x4_2(unity_WorldToObject).x;
          v_2.w = conv_mxt4x4_3(unity_WorldToObject).x;
          float4 v_3;
          v_3.x = conv_mxt4x4_0(unity_WorldToObject).y;
          v_3.y = conv_mxt4x4_1(unity_WorldToObject).y;
          v_3.z = conv_mxt4x4_2(unity_WorldToObject).y;
          v_3.w = conv_mxt4x4_3(unity_WorldToObject).y;
          float4 v_4;
          v_4.x = conv_mxt4x4_0(unity_WorldToObject).z;
          v_4.y = conv_mxt4x4_1(unity_WorldToObject).z;
          v_4.z = conv_mxt4x4_2(unity_WorldToObject).z;
          v_4.w = conv_mxt4x4_3(unity_WorldToObject).z;
          float3x3 tmpvar_5;
          tmpvar_5[0] = conv_mxt4x4_0(unity_MatrixV).xyz;
          tmpvar_5[1] = conv_mxt4x4_1(unity_MatrixV).xyz;
          tmpvar_5[2] = conv_mxt4x4_2(unity_MatrixV).xyz;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_1));
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.xlv_TEXCOORD1 = (TRANSFORM_TEX(in_v.texcoord1.xy, _ShineRamp) + frac((float2(_ScrollSpeed, _ScrollSpeed) * _Time.xy)));
          out_v.xlv_TEXCOORD2 = ((mul(tmpvar_5, normalize((((v_2.xyz * in_v.normal.x) + (v_3.xyz * in_v.normal.y)) + (v_4.xyz * in_v.normal.z)))).xy * 0.5) + 0.5);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 shine_1;
          float4 tmpvar_2;
          tmpvar_2 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          float4 tmpvar_3;
          tmpvar_3 = tex2D(_ShineRamp, in_f.xlv_TEXCOORD1);
          float4 tmpvar_4;
          tmpvar_4 = (tmpvar_3 * _ShineStrength);
          shine_1 = tmpvar_4;
          float4 tmpvar_5;
          tmpvar_5 = tex2D(_MatCap, in_f.xlv_TEXCOORD2);
          float4 tmpvar_6;
          tmpvar_6 = (((tmpvar_2 * tmpvar_5) * (_MatCapStrength * 2)) + shine_1);
          out_f.color = tmpvar_6;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "VertexLit"
}
