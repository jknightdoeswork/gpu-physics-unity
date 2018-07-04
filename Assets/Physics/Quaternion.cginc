/******************************************************************************/
/*
  Project - Unity CJ Lib
            https://github.com/TheAllenChou/unity-cj-lib
  
  Author  - Ming-Lun "Allen" Chou
  Web     - http://AllenChou.net
  Twitter - @TheAllenChou
*/
/******************************************************************************/

#ifndef CJ_LIB_QUATERNION
#define CJ_LIB_QUATERNION

#include "Math.cginc"

#define kUnitQuat (float4(0.0, 0.0, 0.0, 1.0))

inline float4 quat_conj(float4 q)
{
  return float4(-q.xyz, q.w);
}

// q must be unit quaternion
inline float4 quat_pow(float4 q, float p)
{
  float r = length(q.xyz);
  if (r < kEpsilon)
    return kUnitQuat;

  float t = p * atan2(q.w, r);

  return float4(sin(t) * q.xyz / r, cos(t));
}

inline float4 quat_axis_angle(float3 v, float a)
{
  float h = 0.5 * a;
  return float4(sin(h) * normalize(v), cos(h));
}

inline float4 quat_concat(float4 q1, float4 q2)
{
  return float4(q1.w * q2.xyz + q2.w * q1.xyz + cross(q1.xyz, q2.xyz), q1.w * q2.w - dot(q1.xyz, q2.xyz));
}

inline float3 quat_mul(float4 q, float3 v)
{
  return dot(q.xyz, v) * q.xyz + q.w * q.w * v + 2.0 * q.w * cross(q.xyz, v) - cross(cross(q.xyz, v), q.xyz);
}

// both a & b must be unit quaternions
inline float4 slerp(float4 a, float4 b, float t)
{
  float d = dot(a, b);
  if (d > 0.99999)
  {
    return lerp(a, b, t);
  }

  float r = acos(saturate(d));
  return (sin((1.0 - t) * r) * a + sin(t * r) * b) / sin(r);
}

inline float4 nlerp(float4 a, float b, float t)
{
  return normalize(lerp(a, b, t));
}

float4x4 quaternion_to_matrix(float4 quat)
{
    float4x4 m = float4x4(float4(0, 0, 0, 0), float4(0, 0, 0, 0), float4(0, 0, 0, 0), float4(0, 0, 0, 0));

    float x = quat.x, y = quat.y, z = quat.z, w = quat.w;
    float x2 = x + x, y2 = y + y, z2 = z + z;
    float xx = x * x2, xy = x * y2, xz = x * z2;
    float yy = y * y2, yz = y * z2, zz = z * z2;
    float wx = w * x2, wy = w * y2, wz = w * z2;

    m[0][0] = 1.0 - (yy + zz);
    m[0][1] = xy - wz;
    m[0][2] = xz + wy;

    m[1][0] = xy + wz;
    m[1][1] = 1.0 - (xx + zz);
    m[1][2] = yz - wx;

    m[2][0] = xz - wy;
    m[2][1] = yz + wx;
    m[2][2] = 1.0 - (xx + yy);

    m[3][3] = 1.0;

    return m;
}

float3x3 quaternion_to_matrix3x3(float4 quat)
{
    float3x3 m = float3x3(float3(0, 0, 0), float3(0, 0, 0), float3(0, 0, 0));

    float x = quat.x, y = quat.y, z = quat.z, w = quat.w;
    float x2 = x + x, y2 = y + y, z2 = z + z;
    float xx = x * x2, xy = x * y2, xz = x * z2;
    float yy = y * y2, yz = y * z2, zz = z * z2;
    float wx = w * x2, wy = w * y2, wz = w * z2;

    m[0][0] = 1.0 - (yy + zz);
    m[0][1] = xy - wz;
    m[0][2] = xz + wy;

    m[1][0] = xy + wz;
    m[1][1] = 1.0 - (xx + zz);
    m[1][2] = yz - wx;

    m[2][0] = xz - wy;
    m[2][1] = yz + wx;
    m[2][2] = 1.0 - (xx + yy);

    return m;
}
#endif
