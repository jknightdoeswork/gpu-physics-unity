/******************************************************************************/
/*
  Project - Unity CJ Lib
            https://github.com/TheAllenChou/unity-cj-lib
  
  Author  - Ming-Lun "Allen" Chou
  Web     - http://AllenChou.net
  Twitter - @TheAllenChou
*/
/******************************************************************************/

#ifndef CJ_LIB_VECTOR
#define CJ_LIB_VECTOR

#include "Math.cginc"

inline float3 find_ortho(float3 v)
{
  if (v.x >= kSqrt3Inv)
    return float3(v.y, -v.x, 0.0);
  else
    return float3(0.0, v.z, -v.y);
}

// both a & b must be unit vectors
inline float3 slerp(float3 a, float3 b, float t)
{
  float d = dot(a, b);
  if (d > 0.99999)
  {
    return lerp(a, b, t);
  }

  float r = acos(saturate(d));
  return (sin((1.0 - t) * r) * a + sin(t * r) * b) / sin(r);
}

inline float3 nlerp(float3 a, float b, float t)
{
  return normalize(lerp(a, b, t));
}

#endif
