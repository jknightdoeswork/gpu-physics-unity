/******************************************************************************/
/*
  Project - Unity CJ Lib
            https://github.com/TheAllenChou/unity-cj-lib
  
  Author  - Ming-Lun "Allen" Chou
  Web     - http://AllenChou.net
  Twitter - @TheAllenChou
*/
/******************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CjLib
{
  // DO NOT modify nor keep references to the meshes returned by this factory
  // because they are from shared pools of cached meshes
  public class PrimitiveMeshFactory
  {

    // line
    // ------------------------------------------------------------------------

    public static Mesh Line(Vector3 v0, Vector3 v1)
    {
      Mesh mesh = new Mesh();

      Vector3[] aVert = { v0, v1 };
      int[] aIndex = { 0, 1 };

      mesh.vertices = aVert;
      mesh.SetIndices(aIndex, MeshTopology.Lines, 0);

      return mesh;
    }

    public static Mesh Lines(Vector3[] aVert)
    {
      if (aVert.Length <= 1)
        return null;

      Mesh mesh = new Mesh();

      int[] aIndex = new int[aVert.Length];
      for (int i = 0; i < aVert.Length; ++i)
      {
        aIndex[i] = i;
      }

      mesh.vertices = aVert;
      mesh.SetIndices(aIndex, MeshTopology.Lines, 0);

      return mesh;
    }

    public static Mesh LineStrip(Vector3[] aVert)
    {
      if (aVert.Length <= 1)
        return null;

      Mesh mesh = new Mesh();

      int[] aIndex = new int[aVert.Length];
      for (int i = 0; i < aVert.Length; ++i)
      {
        aIndex[i] = i;
      }

      mesh.vertices = aVert;
      mesh.SetIndices(aIndex, MeshTopology.LineStrip, 0);

      return mesh;
    }

    // ------------------------------------------------------------------------
    // end: line


    // box
    // ------------------------------------------------------------------------

    private static Mesh s_boxWireframeMesh;
    private static Mesh s_boxSolidColorMesh;
    private static Mesh s_boxFlatShadedMesh;

    public static Mesh BoxWireframe()
    {
      if (s_boxWireframeMesh == null)
      {
        s_boxWireframeMesh = new Mesh();

        Vector3[] aVert =
        {
          new Vector3(-0.5f, -0.5f, -0.5f), 
          new Vector3(-0.5f,  0.5f, -0.5f), 
          new Vector3( 0.5f,  0.5f, -0.5f), 
          new Vector3( 0.5f, -0.5f, -0.5f), 
          new Vector3(-0.5f, -0.5f,  0.5f), 
          new Vector3(-0.5f,  0.5f,  0.5f), 
          new Vector3( 0.5f,  0.5f,  0.5f), 
          new Vector3( 0.5f, -0.5f,  0.5f), 
        };

        int[] aIndex =
        {
          0, 1, 
          1, 2, 
          2, 3, 
          3, 0, 
          2, 6, 
          6, 7, 
          7, 3, 
          7, 4, 
          4, 5, 
          5, 6, 
          5, 1, 
          1, 0, 
          0, 4, 
        };

        s_boxWireframeMesh.vertices = aVert;
        s_boxWireframeMesh.normals = aVert; // for GizmosUtil
        s_boxWireframeMesh.SetIndices(aIndex, MeshTopology.Lines, 0);
      }

      return s_boxWireframeMesh;
    }

    public static Mesh BoxSolidColor()
    {
      if (s_boxSolidColorMesh == null)
      {
        s_boxSolidColorMesh = new Mesh();

        Vector3[] aVert =
        {
          new Vector3(-0.5f, -0.5f, -0.5f), 
          new Vector3(-0.5f,  0.5f, -0.5f), 
          new Vector3( 0.5f,  0.5f, -0.5f), 
          new Vector3( 0.5f, -0.5f, -0.5f), 
          new Vector3(-0.5f, -0.5f,  0.5f), 
          new Vector3(-0.5f,  0.5f,  0.5f), 
          new Vector3( 0.5f,  0.5f,  0.5f), 
          new Vector3( 0.5f, -0.5f,  0.5f), 
        };

        int[] aIndex =
        {
          0, 1, 2, 0, 2, 3, 
          3, 2, 6, 3, 6, 7, 
          7, 6, 5, 7, 5, 4, 
          4, 5, 1, 4, 1, 0, 
          1, 5, 6, 1, 6, 2, 
          0, 3, 7, 0, 7, 4, 
        };

        s_boxSolidColorMesh.vertices = aVert;
        s_boxSolidColorMesh.SetIndices(aIndex, MeshTopology.Triangles, 0);
      }

      return s_boxSolidColorMesh;
    }

    public static Mesh BoxFlatShaded()
    {
      if (s_boxFlatShadedMesh == null)
      {
        s_boxFlatShadedMesh = new Mesh();

        Vector3[] aRawVert =
        {
          new Vector3(-0.5f, -0.5f, -0.5f), 
          new Vector3(-0.5f,  0.5f, -0.5f), 
          new Vector3( 0.5f,  0.5f, -0.5f), 
          new Vector3( 0.5f, -0.5f, -0.5f), 
          new Vector3(-0.5f, -0.5f,  0.5f), 
          new Vector3(-0.5f,  0.5f,  0.5f), 
          new Vector3( 0.5f,  0.5f,  0.5f), 
          new Vector3( 0.5f, -0.5f,  0.5f), 
        };

        Vector3[] aVert =
        {
          aRawVert[0], aRawVert[1], aRawVert[2], aRawVert[0], aRawVert[2], aRawVert[3],
          aRawVert[3], aRawVert[2], aRawVert[6], aRawVert[3], aRawVert[6], aRawVert[7],
          aRawVert[7], aRawVert[6], aRawVert[5], aRawVert[7], aRawVert[5], aRawVert[4],
          aRawVert[4], aRawVert[5], aRawVert[1], aRawVert[4], aRawVert[1], aRawVert[0],
          aRawVert[1], aRawVert[5], aRawVert[6], aRawVert[1], aRawVert[6], aRawVert[2],
          aRawVert[0], aRawVert[3], aRawVert[7], aRawVert[0], aRawVert[7], aRawVert[4],
        };

        Vector3[] aRawNormal =
        {
          new Vector3( 0.0f,  0.0f, -1.0f), 
          new Vector3( 1.0f,  0.0f,  0.0f), 
          new Vector3( 0.0f,  0.0f,  1.0f), 
          new Vector3(-1.0f,  0.0f,  0.0f), 
          new Vector3( 0.0f,  1.0f,  0.0f), 
          new Vector3( 0.0f, -1.0f,  0.0f), 
        };

        Vector3[] aNormal =
        {
          aRawNormal[0], aRawNormal[0], aRawNormal[0], aRawNormal[0], aRawNormal[0], aRawNormal[0],
          aRawNormal[1], aRawNormal[1], aRawNormal[1], aRawNormal[1], aRawNormal[1], aRawNormal[1],
          aRawNormal[2], aRawNormal[2], aRawNormal[2], aRawNormal[2], aRawNormal[2], aRawNormal[2],
          aRawNormal[3], aRawNormal[3], aRawNormal[3], aRawNormal[3], aRawNormal[3], aRawNormal[3],
          aRawNormal[4], aRawNormal[4], aRawNormal[4], aRawNormal[4], aRawNormal[4], aRawNormal[4],
          aRawNormal[5], aRawNormal[5], aRawNormal[5], aRawNormal[5], aRawNormal[5], aRawNormal[5],
        };
        
        int[] aIndex = new int[aVert.Length];
        for (int i = 0; i < aIndex.Length; ++i)
        {
          aIndex[i] = i;
        }

        s_boxFlatShadedMesh.vertices = aVert;
        s_boxFlatShadedMesh.normals = aNormal;
        s_boxFlatShadedMesh.SetIndices(aIndex, MeshTopology.Triangles, 0);
      }

      return s_boxFlatShadedMesh;
    }

    // ------------------------------------------------------------------------
    // end: box


    // rect
    // ------------------------------------------------------------------------

    private static Mesh s_rectWireframeMesh;
    private static Mesh s_rectSolidColorMesh;
    private static Mesh s_rectFlatShadedMesh;

    // rectangle on the XZ plane centered at origin in object space, dimensions = (X dimension, Z dimension)
    public static Mesh RectWireframe()
    {
      if (s_rectWireframeMesh == null)
      {
        s_rectWireframeMesh = new Mesh();

        Vector3[] aVert =
        {
          new Vector3(-0.5f, 0.0f, -0.5f), 
          new Vector3(-0.5f, 0.0f,  0.5f), 
          new Vector3( 0.5f, 0.0f,  0.5f),
          new Vector3( 0.5f, 0.0f, -0.5f), 
        };

        int[] aIndex =
        {
          0, 1, 
          1, 2, 
          2, 3, 
          3, 0, 
        };

        s_rectWireframeMesh.vertices = aVert;
        s_rectWireframeMesh.normals = aVert; // for GizmosUtil
        s_rectWireframeMesh.SetIndices(aIndex, MeshTopology.Lines, 0);
      }

      return s_rectWireframeMesh;
    }

    // rectangle on the XZ plane centered at origin in object space, dimensions = (X dimension, Z dimension)
    public static Mesh RectSolidColor()
    {
      if (s_rectSolidColorMesh == null)
      {
        s_rectSolidColorMesh = new Mesh();

        Vector3[] aVert =
        {
          new Vector3(-0.5f, 0.0f, -0.5f), 
          new Vector3(-0.5f, 0.0f,  0.5f), 
          new Vector3( 0.5f, 0.0f,  0.5f), 
          new Vector3( 0.5f, 0.0f, -0.5f), 
        };

        int[] aIndex =
        {
          0, 1, 2, 0, 2, 3, 
          0, 2, 1, 0, 3, 2, 
        };

        s_rectSolidColorMesh.vertices = aVert;
        s_rectSolidColorMesh.SetIndices(aIndex, MeshTopology.Triangles, 0);
      }

      return s_rectSolidColorMesh;
    }

    // rectangle on the XZ plane centered at origin in object space, dimensions = (X dimension, Z dimension)
    public static Mesh RectFlatShaded()
    {
      if (s_rectFlatShadedMesh == null)
      {
        s_rectFlatShadedMesh = new Mesh();

        Vector3[] aVert =
        {
          new Vector3(-0.5f, 0.0f, -0.5f), 
          new Vector3(-0.5f, 0.0f,  0.5f), 
          new Vector3( 0.5f, 0.0f,  0.5f), 
          new Vector3( 0.5f, 0.0f, -0.5f), 
          new Vector3(-0.5f, 0.0f, -0.5f), 
          new Vector3(-0.5f, 0.0f,  0.5f), 
          new Vector3( 0.5f, 0.0f,  0.5f), 
          new Vector3( 0.5f, 0.0f, -0.5f), 
        };

        Vector3[] aNormal =
        {
          new Vector3(0.0f,  1.0f, 0.0f), 
          new Vector3(0.0f,  1.0f, 0.0f), 
          new Vector3(0.0f,  1.0f, 0.0f), 
          new Vector3(0.0f,  1.0f, 0.0f), 
          new Vector3(0.0f, -1.0f, 0.0f), 
          new Vector3(0.0f, -1.0f, 0.0f), 
          new Vector3(0.0f, -1.0f, 0.0f), 
          new Vector3(0.0f, -1.0f, 0.0f), 
        };

        int[] aIndex =
        {
          0, 1, 2, 0, 2, 3, 
          4, 6, 5, 4, 7, 6, 
        };

        s_rectFlatShadedMesh.vertices = aVert;
        s_rectFlatShadedMesh.normals = aNormal;
        s_rectFlatShadedMesh.SetIndices(aIndex, MeshTopology.Triangles, 0);
      }

      return s_rectFlatShadedMesh;
    }

    // ------------------------------------------------------------------------
    // end: rect


    // circle
    // ------------------------------------------------------------------------

    private static Dictionary<int, Mesh> s_circleWireframeMeshPool;
    private static Dictionary<int, Mesh> s_circleSolidColorMeshPool;
    private static Dictionary<int, Mesh> s_circleFlatShadedMeshPool;

    // draw a circle on the XZ plane centered at origin in object space
    public static Mesh CircleWireframe(int numSegments)
    {
      if (numSegments <= 1)
        return null;

      if (s_circleWireframeMeshPool == null)
        s_circleWireframeMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_circleWireframeMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[numSegments];
        int[] aIndex = new int[numSegments + 1];

        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          aVert[i] = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          aIndex[i] = i;
          angle += angleIncrement;
        }
        aIndex[numSegments] = 0;

        mesh.vertices = aVert;
        mesh.normals = aVert; // for GizmosUtil
        mesh.SetIndices(aIndex, MeshTopology.LineStrip, 0);

        if (s_circleWireframeMeshPool.ContainsKey(numSegments))
          s_circleWireframeMeshPool.Remove(numSegments);

        s_circleWireframeMeshPool.Add(numSegments, mesh);
      }

      return mesh;
    }

    // draw a circle on the XZ plane centered at origin in object space
    public static Mesh CircleSolidColor(int numSegments)
    {
      if (numSegments <= 1)
        return null;

      if (s_circleSolidColorMeshPool == null)
        s_circleSolidColorMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_circleSolidColorMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[numSegments + 1];
        int[] aIndex = new int[numSegments * 6];

        int iIndex = 0;
        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          aVert[i] = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          angle += angleIncrement;

          aIndex[iIndex++] = numSegments;
          aIndex[iIndex++] = (i + 1) % numSegments;
          aIndex[iIndex++] = i;
          aIndex[iIndex++] = numSegments;
          aIndex[iIndex++] = i;
          aIndex[iIndex++] = (i + 1) % numSegments;

        }
        aVert[numSegments] = Vector3.zero;

        mesh.vertices = aVert;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_circleSolidColorMeshPool.ContainsKey(numSegments))
          s_circleSolidColorMeshPool.Remove(numSegments);

        s_circleSolidColorMeshPool.Add(numSegments, mesh);
      }

      return mesh;
    }

    // draw a circle on the XZ plane centered at origin in object space
    public static Mesh CircleFlatShaded(int numSegments)
    {
      if (numSegments <= 1)
        return null;

      if (s_circleFlatShadedMeshPool == null)
        s_circleFlatShadedMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_circleFlatShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[(numSegments + 1) * 2];
        Vector3[] aNormal = new Vector3[aVert.Length];
        int[] aIndex = new int[numSegments * 6];

        int iIndex = 0;
        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          aVert[i] = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          angle += angleIncrement;

          aNormal[i] = new Vector3(0.0f, 1.0f, 0.0f);

          aIndex[iIndex++] = numSegments;
          aIndex[iIndex++] = (i + 1) % numSegments;
          aIndex[iIndex++] = i;
        }
        aVert[numSegments] = Vector3.zero;
        aNormal[numSegments] = new Vector3(0.0f, 1.0f, 0.0f);
        angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          aVert[i + numSegments + 1] = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          angle -= angleIncrement;

          aNormal[i + numSegments + 1] = new Vector3(0.0f, -1.0f, 0.0f);

          aIndex[iIndex++] = numSegments * 2 + 1;
          aIndex[iIndex++] = ((i + 1) % numSegments) + numSegments + 1;
          aIndex[iIndex++] = i + (numSegments + 1);
        }
        aVert[numSegments * 2 + 1] = Vector3.zero;
        aNormal[numSegments * 2 + 1] = new Vector3(0.0f, -1.0f, 0.0f);

        mesh.vertices = aVert;
        mesh.normals = aNormal;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_circleFlatShadedMeshPool.ContainsKey(numSegments))
          s_circleFlatShadedMeshPool.Remove(numSegments);

        s_circleFlatShadedMeshPool.Add(numSegments, mesh);
      }

      return mesh;
    }

    // ------------------------------------------------------------------------
    // end: circle


    // cylinder
    // ------------------------------------------------------------------------

    private static Dictionary<int, Mesh> s_cylinderWireframeMeshPool;
    private static Dictionary<int, Mesh> s_cylinderSolidColorMeshPool;
    private static Dictionary<int, Mesh> s_cylinderFlatShadedMeshPool;
    private static Dictionary<int, Mesh> s_cylinderSmoothShadedMeshPool;

    public static Mesh CylinderWireframe(int numSegments)
    {
      if (numSegments <= 1)
        return null;

      if (s_cylinderWireframeMeshPool == null)
        s_cylinderWireframeMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_cylinderWireframeMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[numSegments * 2];
        int[] aIndex = new int[numSegments * 6];

        Vector3 bottom = new Vector3(0.0f, -0.5f, 0.0f);
        Vector3 top = new Vector3(0.0f, 0.5f, 0.0f);

        int iIndex = 0;
        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          Vector3 offset = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          aVert[i] = bottom + offset;
          aVert[numSegments + i] = top + offset;

          aIndex[iIndex++] = i;
          aIndex[iIndex++] = ((i + 1) % numSegments);

          aIndex[iIndex++] = i;
          aIndex[iIndex++] = numSegments + i;

          aIndex[iIndex++] = numSegments + i;
          aIndex[iIndex++] = numSegments + ((i + 1) % numSegments);

          angle += angleIncrement;
        }

        mesh.vertices = aVert;
        mesh.normals = aVert; // for GizmosUtil
        mesh.SetIndices(aIndex, MeshTopology.Lines, 0);

        if (s_cylinderWireframeMeshPool.ContainsKey(numSegments))
          s_cylinderWireframeMeshPool.Remove(numSegments);

        s_cylinderWireframeMeshPool.Add(numSegments, mesh);
      }

      return mesh;
    }

    public static Mesh CylinderSolidColor(int numSegments)
    {
      if (numSegments <= 1)
        return null;

      if (s_cylinderSolidColorMeshPool == null)
        s_cylinderSolidColorMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_cylinderSolidColorMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[numSegments * 2 + 2];
        int[] aIndex = new int[numSegments * 12];

        Vector3 bottom = new Vector3(0.0f, -0.5f, 0.0f);
        Vector3 top = new Vector3(0.0f, 0.5f, 0.0f);

        int iBottomCapStart = 0;
        int iTopCapStart = numSegments;
        int iBottom = numSegments * 2;
        int iTop = numSegments * 2 + 1;

        aVert[iBottom] = bottom;
        aVert[iTop] = top;

        int iIndex = 0;
        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          Vector3 offset = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          aVert[iBottomCapStart + i] = bottom + offset;
          aVert[iTopCapStart + i] = top + offset;

          aIndex[iIndex++] = iBottom;
          aIndex[iIndex++] = iBottomCapStart + i;
          aIndex[iIndex++] = iBottomCapStart + ((i + 1) % numSegments);

          aIndex[iIndex++] = iBottomCapStart + i;
          aIndex[iIndex++] = iTopCapStart + ((i + 1) % numSegments);
          aIndex[iIndex++] = iBottomCapStart + ((i + 1) % numSegments);

          aIndex[iIndex++] = iBottomCapStart + i;
          aIndex[iIndex++] = iTopCapStart + i;
          aIndex[iIndex++] = iTopCapStart + ((i + 1) % numSegments);

          aIndex[iIndex++] = iTop;
          aIndex[iIndex++] = iTopCapStart + ((i + 1) % numSegments);
          aIndex[iIndex++] = iTopCapStart + i;

          angle += angleIncrement;
        }

        mesh.vertices = aVert;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_cylinderSolidColorMeshPool.ContainsKey(numSegments))
          s_cylinderSolidColorMeshPool.Remove(numSegments);

        s_cylinderSolidColorMeshPool.Add(numSegments, mesh);
      }

      return mesh;
    }

    public static Mesh CylinderFlatShaded(int numSegments)
    {
      if (numSegments <= 1)
        return null;

      if (s_cylinderFlatShadedMeshPool == null)
        s_cylinderFlatShadedMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_cylinderFlatShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[numSegments * 6 + 2];
        Vector3[] aNormal = new Vector3[aVert.Length];
        int[] aIndex = new int[numSegments * 12];

        Vector3 bottom = new Vector3(0.0f, -0.5f, 0.0f);
        Vector3 top = new Vector3(0.0f, 0.5f, 0.0f);

        int iBottomCapStart = 0;
        int iTopCapStart = numSegments;
        int iSideStart = numSegments * 2;
        int iBottom = numSegments * 6;
        int iTop = numSegments * 6 + 1;

        aVert[iBottom] = bottom;
        aVert[iTop] = top;

        aNormal[iBottom] = new Vector3(0.0f, -1.0f, 0.0f);
        aNormal[iTop] = new Vector3(0.0f, 1.0f, 0.0f);

        int iIndex = 0;
        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          // caps
          Vector3 offset = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          aVert[iBottomCapStart + i] = bottom + offset;
          aVert[iTopCapStart + i] = top + offset;

          aNormal[iBottomCapStart + i] = new Vector3(0.0f, -1.0f, 0.0f);
          aNormal[iTopCapStart + i] = new Vector3(0.0f, 1.0f, 0.0f);

          aIndex[iIndex++] = iBottom;
          aIndex[iIndex++] = iBottomCapStart + i;
          aIndex[iIndex++] = iBottomCapStart + ((i + 1) % numSegments);

          aIndex[iIndex++] = iTop;
          aIndex[iIndex++] = iTopCapStart + ((i + 1) % numSegments);
          aIndex[iIndex++] = iTopCapStart + i;

          angle += angleIncrement;

          // sides
          Vector3 offsetNext = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          aVert[iSideStart + i * 4    ] = bottom + offset;
          aVert[iSideStart + i * 4 + 1] = top + offset;
          aVert[iSideStart + i * 4 + 2] = bottom + offsetNext;
          aVert[iSideStart + i * 4 + 3] = top + offsetNext;

          Vector3 sideNormal = Vector3.Cross(top - bottom, offsetNext - offset).normalized;
          aNormal[iSideStart + i * 4    ] = sideNormal;
          aNormal[iSideStart + i * 4 + 1] = sideNormal;
          aNormal[iSideStart + i * 4 + 2] = sideNormal;
          aNormal[iSideStart + i * 4 + 3] = sideNormal;

          aIndex[iIndex++] = iSideStart + i * 4;
          aIndex[iIndex++] = iSideStart + i * 4 + 3;
          aIndex[iIndex++] = iSideStart + i * 4 + 2;

          aIndex[iIndex++] = iSideStart + i * 4;
          aIndex[iIndex++] = iSideStart + i * 4 + 1;
          aIndex[iIndex++] = iSideStart + i * 4 + 3;
        }

        mesh.vertices = aVert;
        mesh.normals = aNormal;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_cylinderFlatShadedMeshPool.ContainsKey(numSegments))
          s_cylinderFlatShadedMeshPool.Remove(numSegments);

        s_cylinderFlatShadedMeshPool.Add(numSegments, mesh);
      }

      return mesh;
    }

    public static Mesh CylinderSmoothShaded(int numSegments)
    {
      if (numSegments <= 1)
        return null;

      if (s_cylinderSmoothShadedMeshPool == null)
        s_cylinderSmoothShadedMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_cylinderSmoothShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[numSegments * 4 + 2];
        Vector3[] aNormal = new Vector3[aVert.Length];
        int[] aIndex = new int[numSegments * 12];

        Vector3 bottom = new Vector3(0.0f, -0.5f, 0.0f);
        Vector3 top = new Vector3(0.0f, 0.5f, 0.0f);

        int iBottomCapStart = 0;
        int iTopCapStart = numSegments;
        int iSideStart = numSegments * 2;
        int iBottom = numSegments * 4;
        int iTop = numSegments * 4 + 1;

        aVert[iBottom] = bottom;
        aVert[iTop] = top;

        aNormal[iBottom] = new Vector3(0.0f, -1.0f, 0.0f);
        aNormal[iTop] = new Vector3(0.0f, 1.0f, 0.0f);

        int iIndex = 0;
        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          // caps
          Vector3 offset = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          aVert[iBottomCapStart + i] = bottom + offset;
          aVert[iTopCapStart + i] = top + offset;

          aNormal[iBottomCapStart + i] = new Vector3(0.0f, -1.0f, 0.0f);
          aNormal[iTopCapStart + i] = new Vector3(0.0f, 1.0f, 0.0f);

          aIndex[iIndex++] = iBottom;
          aIndex[iIndex++] = iBottomCapStart + i;
          aIndex[iIndex++] = iBottomCapStart + ((i + 1) % numSegments);

          aIndex[iIndex++] = iTop;
          aIndex[iIndex++] = iTopCapStart + ((i + 1) % numSegments);
          aIndex[iIndex++] = iTopCapStart + i;

          angle += angleIncrement;

          // sides
          aVert[iSideStart + i * 2    ] = bottom + offset;
          aVert[iSideStart + i * 2 + 1] = top + offset;

          aNormal[iSideStart + i * 2    ] = offset;
          aNormal[iSideStart + i * 2 + 1] = offset;

          aIndex[iIndex++] = iSideStart + i * 2;
          aIndex[iIndex++] = iSideStart + ((i * 2 + 3) % (numSegments * 2));
          aIndex[iIndex++] = iSideStart + ((i * 2 + 2) % (numSegments * 2));

          aIndex[iIndex++] = iSideStart + i * 2;
          aIndex[iIndex++] = iSideStart + ((i * 2 + 1) % (numSegments * 2));
          aIndex[iIndex++] = iSideStart + ((i * 2 + 3) % (numSegments * 2));
        }

        mesh.vertices = aVert;
        mesh.normals = aNormal;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_cylinderSmoothShadedMeshPool.ContainsKey(numSegments))
          s_cylinderSmoothShadedMeshPool.Remove(numSegments);

        s_cylinderSmoothShadedMeshPool.Add(numSegments, mesh);
      }

      return mesh;
    }

    // ------------------------------------------------------------------------
    // end: cylinder


    // sphere
    // ------------------------------------------------------------------------

    private static Dictionary<int, Mesh> s_sphereWireframeMeshPool;
    private static Dictionary<int, Mesh> s_sphereSolidColorMeshPool;
    private static Dictionary<int, Mesh> s_sphereFlatShadedMeshPool;
    private static Dictionary<int, Mesh> s_sphereSmoothShadedMeshPool;

    public static Mesh SphereWireframe(int latSegments, int longSegments)
    {
      if (latSegments <= 0 || longSegments <= 1)
        return null;

      if (s_sphereWireframeMeshPool == null)
        s_sphereWireframeMeshPool = new Dictionary<int, Mesh>();

      int meshKey = (latSegments << 16 ^ longSegments);
      Mesh mesh;
      if (!s_sphereWireframeMeshPool.TryGetValue(meshKey, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[longSegments * (latSegments - 1) + 2];
        int[] aIndex = new int[(longSegments * (latSegments * 2 - 1)) * 2];

        Vector3 top = new Vector3(0.0f, 1.0f, 0.0f);
        Vector3 bottom = new Vector3(0.0f, -1.0f, 0.0f);
        int iTop = aVert.Length - 2;
        int iBottom = aVert.Length - 1;
        aVert[iTop] = top;
        aVert[iBottom] = bottom;

        float[] aLatSin = new float[latSegments];
        float[] aLatCos = new float[latSegments];
        {
          float latAngleIncrement = Mathf.PI / latSegments;
          float latAngle = 0.0f;
          for (int iLat = 0; iLat < latSegments; ++iLat)
          {
            latAngle += latAngleIncrement;
            aLatSin[iLat] = Mathf.Sin(latAngle);
            aLatCos[iLat] = Mathf.Cos(latAngle);
          }
        }

        float[] aLongSin = new float[longSegments];
        float[] aLongCos = new float[longSegments];
        {
          float longAngleIncrement = 2.0f * Mathf.PI / longSegments;
          float longAngle = 0.0f;
          for (int iLong = 0; iLong < longSegments; ++iLong)
          {
            longAngle += longAngleIncrement;
            aLongSin[iLong] = Mathf.Sin(longAngle);
            aLongCos[iLong] = Mathf.Cos(longAngle);
          }
        }

        int iVert = 0;
        int iIndex = 0;
        for (int iLong = 0; iLong < longSegments; ++iLong)
        {
          float longSin = aLongSin[iLong];
          float longCos = aLongCos[iLong];

          for (int iLat = 0; iLat < latSegments - 1; ++iLat)
          {
            float latSin = aLatSin[iLat];
            float latCos = aLatCos[iLat];

            aVert[iVert] = new Vector3(longCos * latSin, latCos, longSin * latSin);

            if (iLat == 0)
            {
              aIndex[iIndex++] = iTop;
              aIndex[iIndex++] = iVert;
            }
            else
            {
              aIndex[iIndex++] = iVert - 1;
              aIndex[iIndex++] = iVert;
            }

            aIndex[iIndex++] = iVert;
            aIndex[iIndex++] = (iVert + latSegments - 1) % (longSegments * (latSegments - 1));
            
            if (iLat == latSegments - 2)
            {
              aIndex[iIndex++] = iVert;
              aIndex[iIndex++] = iBottom;
            }

            ++iVert;
          }
        }

        mesh.vertices = aVert;
        mesh.normals = aVert; // for GizmosUtil
        mesh.SetIndices(aIndex, MeshTopology.Lines, 0);

        if (s_sphereWireframeMeshPool.ContainsKey(meshKey))
          s_sphereWireframeMeshPool.Remove(meshKey);

        s_sphereWireframeMeshPool.Add(meshKey, mesh);
      }

      return mesh;
    }

    public static Mesh SphereSolidColor(int latSegments, int longSegments)
    {
      if (latSegments <= 0 || longSegments <= 1)
        return null;

      if (s_sphereSolidColorMeshPool == null)
        s_sphereSolidColorMeshPool = new Dictionary<int, Mesh>();

      int meshKey = (latSegments << 16 ^ longSegments);
      Mesh mesh;
      if (!s_sphereSolidColorMeshPool.TryGetValue(meshKey, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[longSegments * (latSegments - 1) + 2];
        int[] aIndex = new int[(longSegments * (latSegments - 1) * 2) * 3];

        Vector3 top = new Vector3(0.0f, 1.0f, 0.0f);
        Vector3 bottom = new Vector3(0.0f, -1.0f, 0.0f);
        int iTop = aVert.Length - 2;
        int iBottom = aVert.Length - 1;
        aVert[iTop] = top;
        aVert[iBottom] = bottom;

        float[] aLatSin = new float[latSegments];
        float[] aLatCos = new float[latSegments];
        {
          float latAngleIncrement = Mathf.PI / latSegments;
          float latAngle = 0.0f;
          for (int iLat = 0; iLat < latSegments; ++iLat)
          {
            latAngle += latAngleIncrement;
            aLatSin[iLat] = Mathf.Sin(latAngle);
            aLatCos[iLat] = Mathf.Cos(latAngle);
          }
        }

        float[] aLongSin = new float[longSegments];
        float[] aLongCos = new float[longSegments];
        {
          float longAngleIncrement = 2.0f * Mathf.PI / longSegments;
          float longAngle = 0.0f;
          for (int iLong = 0; iLong < longSegments; ++iLong)
          {
            longAngle += longAngleIncrement;
            aLongSin[iLong] = Mathf.Sin(longAngle);
            aLongCos[iLong] = Mathf.Cos(longAngle);
          }
        }

        int iVert = 0;
        int iIndex = 0;
        for (int iLong = 0; iLong < longSegments; ++iLong)
        {
          float longSin = aLongSin[iLong];
          float longCos = aLongCos[iLong];

          for (int iLat = 0; iLat < latSegments - 1; ++iLat)
          {
            float latSin = aLatSin[iLat];
            float latCos = aLatCos[iLat];

            aVert[iVert] = new Vector3(longCos * latSin, latCos, longSin * latSin);

            if (iLat == 0)
            {
              aIndex[iIndex++] = iTop;
              aIndex[iIndex++] = (iVert + latSegments - 1) % (longSegments * (latSegments - 1));
              aIndex[iIndex++] = iVert;
            }

            if (iLat < latSegments - 2)
            {
              aIndex[iIndex++] = iVert + 1;
              aIndex[iIndex++] = iVert;
              aIndex[iIndex++] = (iVert + latSegments - 1) % (longSegments * (latSegments - 1));

              aIndex[iIndex++] = iVert + 1;
              aIndex[iIndex++] = (iVert + latSegments - 1) % (longSegments * (latSegments - 1));
              aIndex[iIndex++] = (iVert + latSegments) % (longSegments * (latSegments - 1));
            }

            if (iLat == latSegments - 2)
            {
              aIndex[iIndex++] = iVert;
              aIndex[iIndex++] = (iVert + latSegments - 1) % (longSegments * (latSegments - 1));
              aIndex[iIndex++] = iBottom;
            }

            ++iVert;
          }
        }

        mesh.vertices = aVert;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_sphereSolidColorMeshPool.ContainsKey(meshKey))
          s_sphereSolidColorMeshPool.Remove(meshKey);

        s_sphereSolidColorMeshPool.Add(meshKey, mesh);
      }

      return mesh;
    }

    public static Mesh SphereFlatShaded(int latSegments, int longSegments)
    {
      if (latSegments <= 1 || longSegments <= 1)
        return null;

      if (s_sphereFlatShadedMeshPool == null)
        s_sphereFlatShadedMeshPool = new Dictionary<int, Mesh>();

      int meshKey = (latSegments << 16 ^ longSegments);
      Mesh mesh;
      if (!s_sphereFlatShadedMeshPool.TryGetValue(meshKey, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        int numVertsPerLong = (latSegments - 2) * 4 + 6;
        int numTrisPerLong = (latSegments - 2) * 2 + 2;

        Vector3[] aVert = new Vector3[longSegments * numVertsPerLong];
        Vector3[] aNormal = new Vector3[aVert.Length];
        int[] aIndex = new int[longSegments * numTrisPerLong * 3];

        Vector3 top = new Vector3(0.0f, 1.0f, 0.0f);
        Vector3 bottom = new Vector3(0.0f, -1.0f, 0.0f);

        float[] aLatSin = new float[latSegments];
        float[] aLatCos = new float[latSegments];
        {
          float latAngleIncrement = Mathf.PI / latSegments;
          float latAngle = 0.0f;
          for (int iLat = 0; iLat < latSegments; ++iLat)
          {
            latAngle += latAngleIncrement;
            aLatSin[iLat] = Mathf.Sin(latAngle);
            aLatCos[iLat] = Mathf.Cos(latAngle);
          }
        }

        float[] aLongSin = new float[longSegments];
        float[] aLongCos = new float[longSegments];
        {
          float longAngleIncrement = 2.0f * Mathf.PI / longSegments;
          float longAngle = 0.0f;
          for (int iLong = 0; iLong < longSegments; ++iLong)
          {
            longAngle += longAngleIncrement;
            aLongSin[iLong] = Mathf.Sin(longAngle);
            aLongCos[iLong] = Mathf.Cos(longAngle);
          }
        }

        int iVert = 0;
        int iNormal = 0;
        int iIndex = 0;
        for (int iLong = 0; iLong < longSegments; ++iLong)
        {
          float longSin = aLongSin[iLong];
          float longCos = aLongCos[iLong];
          float longSinNext = aLongSin[(iLong + 1) % longSegments];
          float longCosNext = aLongCos[(iLong + 1) % longSegments];

          int iTop = iVert;

          aVert[iVert++] = top;
          aVert[iVert++] = new Vector3(longCos * aLatSin[0], aLatCos[0], longSin * aLatSin[0]);
          aVert[iVert++] = new Vector3(longCosNext * aLatSin[0], aLatCos[0], longSinNext * aLatSin[0]);

          int iBottom = iVert;

          aVert[iVert++] = bottom;
          aVert[iVert++] = new Vector3(longCos * aLatSin[latSegments - 2], aLatCos[latSegments - 2], longSin * aLatSin[latSegments - 2]);
          aVert[iVert++] = new Vector3(longCosNext * aLatSin[latSegments - 2], aLatCos[latSegments - 2], longSinNext * aLatSin[latSegments - 2]);

          Vector3 topNormal = Vector3.Cross(aVert[iTop + 2] - aVert[iTop], aVert[iTop + 1] - aVert[iTop]).normalized;
          aNormal[iNormal++] = topNormal;
          aNormal[iNormal++] = topNormal;
          aNormal[iNormal++] = topNormal;

          Vector3 bottomNormal = Vector3.Cross(aVert[iBottom + 1] - aVert[iBottom], aVert[iBottom + 2] - aVert[iBottom]).normalized;
          aNormal[iNormal++] = bottomNormal;
          aNormal[iNormal++] = bottomNormal;
          aNormal[iNormal++] = bottomNormal;

          aIndex[iIndex++] = iTop;
          aIndex[iIndex++] = iTop + 2;
          aIndex[iIndex++] = iTop + 1;

          aIndex[iIndex++] = iBottom;
          aIndex[iIndex++] = iBottom + 1;
          aIndex[iIndex++] = iBottom + 2;

          for (int iLat = 0; iLat < latSegments - 2; ++iLat)
          {
            float latSin = aLatSin[iLat];
            float latCos = aLatCos[iLat];
            float latSinNext = aLatSin[iLat + 1];
            float latCosNext = aLatCos[iLat + 1];

            int iQuadStart = iVert;

            aVert[iVert++] = new Vector3(longCos * latSin, latCos, longSin * latSin);
            aVert[iVert++] = new Vector3(longCosNext * latSin, latCos, longSinNext * latSin);
            aVert[iVert++] = new Vector3(longCosNext * latSinNext, latCosNext, longSinNext * latSinNext);
            aVert[iVert++] = new Vector3(longCos * latSinNext, latCosNext, longSin * latSinNext);

            Vector3 quadNormal = Vector3.Cross(aVert[iQuadStart + 1] - aVert[iQuadStart], aVert[iQuadStart + 2] - aVert[iQuadStart]).normalized;
            aNormal[iNormal++] = quadNormal;
            aNormal[iNormal++] = quadNormal;
            aNormal[iNormal++] = quadNormal;
            aNormal[iNormal++] = quadNormal;

            aIndex[iIndex++] = iQuadStart;
            aIndex[iIndex++] = iQuadStart + 1;
            aIndex[iIndex++] = iQuadStart + 2;

            aIndex[iIndex++] = iQuadStart;
            aIndex[iIndex++] = iQuadStart + 2;
            aIndex[iIndex++] = iQuadStart + 3;
          }
        }

        mesh.vertices = aVert;
        mesh.normals = aNormal;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_sphereFlatShadedMeshPool.ContainsKey(meshKey))
          s_sphereFlatShadedMeshPool.Remove(meshKey);

        s_sphereFlatShadedMeshPool.Add(meshKey, mesh);
      }

      return mesh;
    }

    public static Mesh SphereSmoothShaded(int latSegments, int longSegments)
    {
      if (latSegments <= 1 || longSegments <= 1)
        return null;

      if (s_sphereSmoothShadedMeshPool == null)
        s_sphereSmoothShadedMeshPool = new Dictionary<int, Mesh>();

      int meshKey = (latSegments << 16 ^ longSegments);
      Mesh mesh;
      if (!s_sphereSmoothShadedMeshPool.TryGetValue(meshKey, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        int numVertsPerLong = (latSegments - 1);
        int numTrisPerLong = (latSegments - 2) * 2 + 2;

        Vector3[] aVert = new Vector3[longSegments * numVertsPerLong + 2];
        Vector3[] aNormal = new Vector3[aVert.Length];
        int[] aIndex = new int[longSegments * numTrisPerLong * 3];

        Vector3 top = new Vector3(0.0f, 1.0f, 0.0f);
        Vector3 bottom = new Vector3(0.0f, -1.0f, 0.0f);

        int iTop = longSegments * numVertsPerLong;
        int iBottom = iTop + 1;

        aVert[iTop] = top;
        aVert[iBottom] = bottom;

        aNormal[iTop] = new Vector3(0.0f, 1.0f, 0.0f);
        aNormal[iBottom] = new Vector3(0.0f, -1.0f, 0.0f);

        float[] aLatSin = new float[latSegments];
        float[] aLatCos = new float[latSegments];
        {
          float latAngleIncrement = Mathf.PI / latSegments;
          float latAngle = 0.0f;
          for (int iLat = 0; iLat < latSegments; ++iLat)
          {
            latAngle += latAngleIncrement;
            aLatSin[iLat] = Mathf.Sin(latAngle);
            aLatCos[iLat] = Mathf.Cos(latAngle);
          }
        }

        float[] aLongSin = new float[longSegments];
        float[] aLongCos = new float[longSegments];
        {
          float longAngleIncrement = 2.0f * Mathf.PI / longSegments;
          float longAngle = 0.0f;
          for (int iLong = 0; iLong < longSegments; ++iLong)
          {
            longAngle += longAngleIncrement;
            aLongSin[iLong] = Mathf.Sin(longAngle);
            aLongCos[iLong] = Mathf.Cos(longAngle);
          }
        }

        int iVert = 0;
        int iNormal = 0;
        int iIndex = 0;
        for (int iLong = 0; iLong < longSegments; ++iLong)
        {
          float longSin = aLongSin[iLong];
          float longCos = aLongCos[iLong];

          for (int iLat = 0; iLat < latSegments - 1; ++iLat)
          {
            float latSin = aLatSin[iLat];
            float latCos = aLatCos[iLat];

            Vector3 vert = new Vector3(longCos * latSin, latCos, longSin * latSin);

            aVert[iVert++] = vert;

            aNormal[iNormal++] = vert;

            int iQuad0 = iVert - 1;
            int iQuad1 = iQuad0 + 1;
            int iQuad2 = (iQuad0 + numVertsPerLong) % (longSegments * numVertsPerLong);
            int iQuad3 = (iQuad0 + numVertsPerLong + 1) % (longSegments * numVertsPerLong);

            if (latSegments == 2)
            {
              aIndex[iIndex++] = iTop;
              aIndex[iIndex++] = iQuad2;
              aIndex[iIndex++] = iQuad0;

              aIndex[iIndex++] = iBottom;
              aIndex[iIndex++] = iQuad1;
              aIndex[iIndex++] = iQuad3;
            }
            else if (iLat < latSegments - 2)
            {
              if (iLat == 0)
              {
                aIndex[iIndex++] = iTop;
                aIndex[iIndex++] = iQuad2;
                aIndex[iIndex++] = iQuad0;
              }
              else if (iLat == latSegments - 3)
              {
                aIndex[iIndex++] = iBottom;
                aIndex[iIndex++] = iQuad1;
                aIndex[iIndex++] = iQuad3;
              }

              aIndex[iIndex++] = iQuad0;
              aIndex[iIndex++] = iQuad3;
              aIndex[iIndex++] = iQuad1;

              aIndex[iIndex++] = iQuad0;
              aIndex[iIndex++] = iQuad2;
              aIndex[iIndex++] = iQuad3;
            }
          }
        }

        mesh.vertices = aVert;
        mesh.normals = aNormal;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_sphereSmoothShadedMeshPool.ContainsKey(meshKey))
          s_sphereSmoothShadedMeshPool.Remove(meshKey);

        s_sphereSmoothShadedMeshPool.Add(meshKey, mesh);
      }

      return mesh;
    }

    // ------------------------------------------------------------------------
    // end: sphere


    // capsule
    // ------------------------------------------------------------------------

    private static Dictionary<int, Mesh> s_capsuleWireframeMeshPool;
    private static Dictionary<int, Mesh> s_capsuleSolidColorMeshPool;
    private static Dictionary<int, Mesh> s_capsuleFlatShadedMeshPool;
    private static Dictionary<int, Mesh> s_capsuleSmoothShadedMeshPool;

    public static Mesh CapsuleWireframe(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
    {
      if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
        return null;

      if (!caps && !sides)
        return null;

      if (s_capsuleWireframeMeshPool == null)
        s_capsuleWireframeMeshPool = new Dictionary<int, Mesh>();

      int meshKey = (latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 1 << 28 : 0) ^ (topCapOnly ? 1 << 29 : 0) ^ (sides ? 1 << 30 : 0));
      Mesh mesh;
      if (!s_capsuleWireframeMeshPool.TryGetValue(meshKey, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
        int[] aIndex = new int[longSegmentsPerCap * (latSegmentsPerCap * 4 + 1) * 2];

        Vector3 top = new Vector3(0.0f, 1.5f, 0.0f);
        Vector3 bottom = new Vector3(0.0f, -1.5f, 0.0f);
        int iTop = aVert.Length - 2;
        int iBottom = aVert.Length - 1;
        aVert[iTop] = top;
        aVert[iBottom] = bottom;

        float[] aLatSin = new float[latSegmentsPerCap];
        float[] aLatCos = new float[latSegmentsPerCap];
        {
          float latAngleIncrement = 0.5f * Mathf.PI / latSegmentsPerCap;
          float latAngle = 0.0f;
          for (int iLat = 0; iLat < latSegmentsPerCap; ++iLat)
          {
            latAngle += latAngleIncrement;
            aLatSin[iLat] = Mathf.Sin(latAngle);
            aLatCos[iLat] = Mathf.Cos(latAngle);
          }
        }

        float[] aLongSin = new float[longSegmentsPerCap];
        float[] aLongCos = new float[longSegmentsPerCap];
        {
          float longAngleIncrement = 2.0f * Mathf.PI / longSegmentsPerCap;
          float longAngle = 0.0f;
          for (int iLong = 0; iLong < longSegmentsPerCap; ++iLong)
          {
            longAngle += longAngleIncrement;
            aLongSin[iLong] = Mathf.Sin(longAngle);
            aLongCos[iLong] = Mathf.Cos(longAngle);
          }
        }

        int iVert = 0;
        int iIndex = 0;
        for (int iLong = 0; iLong < longSegmentsPerCap; ++iLong)
        {
          float longSin = aLongSin[iLong];
          float longCos = aLongCos[iLong];

          for (int iLat = 0; iLat < latSegmentsPerCap; ++iLat)
          {
            float latSin = aLatSin[iLat];
            float latCos = aLatCos[iLat];

            aVert[iVert    ] = new Vector3(longCos * latSin,  latCos + 0.5f, longSin * latSin);
            aVert[iVert + 1] = new Vector3(longCos * latSin, -latCos - 0.5f, longSin * latSin);

            if (caps)
            {
              if (iLat == 0)
              {
                aIndex[iIndex++] = iTop;
                aIndex[iIndex++] = iVert;

                if (!topCapOnly)
                {
                  aIndex[iIndex++] = iBottom;
                  aIndex[iIndex++] = iVert + 1;
                }
              }
              else
              {
                aIndex[iIndex++] = iVert - 2;
                aIndex[iIndex++] = iVert;

                if (!topCapOnly)
                {
                  aIndex[iIndex++] = iVert - 1;
                  aIndex[iIndex++] = iVert + 1;
                }
              }
            }

            if (caps || iLat == latSegmentsPerCap - 1)
            {
              aIndex[iIndex++] = iVert; 
              aIndex[iIndex++] = (iVert + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);

              if (!topCapOnly)
              {
                aIndex[iIndex++] = iVert + 1;
                aIndex[iIndex++] = (iVert + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
              }
            }

            if (sides && iLat == latSegmentsPerCap - 1)
            {
              aIndex[iIndex++] = iVert;
              aIndex[iIndex++] = iVert + 1;
            }

            iVert += 2;
          }
        }

        Array.Resize(ref aIndex, iIndex);

        mesh.vertices = aVert;
        mesh.normals = aVert; // for GizmosUtil
        mesh.SetIndices(aIndex, MeshTopology.Lines, 0);

        if (s_capsuleWireframeMeshPool.ContainsKey(meshKey))
          s_capsuleWireframeMeshPool.Remove(meshKey);

        s_capsuleWireframeMeshPool.Add(meshKey, mesh);
      }

      return mesh;
    }

    public static Mesh CapsuleSolidColor(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
    {
      if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
        return null;

      if (!caps && !sides)
        return null;

      if (s_capsuleSolidColorMeshPool == null)
        s_capsuleSolidColorMeshPool = new Dictionary<int, Mesh>();

      int meshKey = (latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 1 << 28 : 0) ^ (topCapOnly ? 1 << 29 : 0) ^ (sides ? 1 << 30 : 0));
      Mesh mesh;
      if (!s_capsuleSolidColorMeshPool.TryGetValue(meshKey, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
        int[] aIndex = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];

        Vector3 top = new Vector3(0.0f, 1.5f, 0.0f);
        Vector3 bottom = new Vector3(0.0f, -1.5f, 0.0f);
        int iTop = aVert.Length - 2;
        int iBottom = aVert.Length - 1;
        aVert[iTop] = top;
        aVert[iBottom] = bottom;

        float[] aLatSin = new float[latSegmentsPerCap];
        float[] aLatCos = new float[latSegmentsPerCap];
        {
          float latAngleIncrement = 0.5f * Mathf.PI / latSegmentsPerCap;
          float latAngle = 0.0f;
          for (int iLat = 0; iLat < latSegmentsPerCap; ++iLat)
          {
            latAngle += latAngleIncrement;
            aLatSin[iLat] = Mathf.Sin(latAngle);
            aLatCos[iLat] = Mathf.Cos(latAngle);
          }
        }

        float[] aLongSin = new float[longSegmentsPerCap];
        float[] aLongCos = new float[longSegmentsPerCap];
        {
          float longAngleIncrement = 2.0f * Mathf.PI / longSegmentsPerCap;
          float longAngle = 0.0f;
          for (int iLong = 0; iLong < longSegmentsPerCap; ++iLong)
          {
            longAngle += longAngleIncrement;
            aLongSin[iLong] = Mathf.Sin(longAngle);
            aLongCos[iLong] = Mathf.Cos(longAngle);
          }
        }

        int iVert = 0;
        int iIndex = 0;
        for (int iLong = 0; iLong < longSegmentsPerCap; ++iLong)
        {
          float longSin = aLongSin[iLong];
          float longCos = aLongCos[iLong];

          for (int iLat = 0; iLat < latSegmentsPerCap; ++iLat)
          {
            float latSin = aLatSin[iLat];
            float latCos = aLatCos[iLat];

            aVert[iVert    ] = new Vector3(longCos * latSin,  latCos + 0.5f, longSin * latSin);
            aVert[iVert + 1] = new Vector3(longCos * latSin, -latCos - 0.5f, longSin * latSin);

            if (iLat == 0)
            {
              if (caps)
              {
                aIndex[iIndex++] = iTop;
                aIndex[iIndex++] = (iVert + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                aIndex[iIndex++] = iVert;

                if (!topCapOnly)
                {
                  aIndex[iIndex++] = iBottom;
                  aIndex[iIndex++] = iVert + 1;
                  aIndex[iIndex++] = (iVert + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                }
              }
            }
            else
            {
              if (caps)
              {
                aIndex[iIndex++] = iVert - 2;
                aIndex[iIndex++] = (iVert + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                aIndex[iIndex++] = iVert;

                aIndex[iIndex++] = iVert - 2;
                aIndex[iIndex++] = (iVert - 2 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                aIndex[iIndex++] = (iVert + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);

                if (!topCapOnly)
                {
                  aIndex[iIndex++] = iVert - 1;
                  aIndex[iIndex++] = iVert + 1;
                  aIndex[iIndex++] = (iVert + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);

                  aIndex[iIndex++] = iVert - 1;
                  aIndex[iIndex++] = (iVert + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                  aIndex[iIndex++] = (iVert - 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                }
              }

              if (sides && iLat == latSegmentsPerCap - 1)
              {
                aIndex[iIndex++] = iVert;
                aIndex[iIndex++] = (iVert + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                aIndex[iIndex++] = iVert + 1;

                aIndex[iIndex++] = iVert;
                aIndex[iIndex++] = (iVert+ latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                aIndex[iIndex++] = (iVert + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
              }
            }

            iVert += 2;
          }
        }

        Array.Resize(ref aIndex, iIndex);

        mesh.vertices = aVert;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_capsuleSolidColorMeshPool.ContainsKey(meshKey))
          s_capsuleSolidColorMeshPool.Remove(meshKey);

        s_capsuleSolidColorMeshPool.Add(meshKey, mesh);
      }

      return mesh;
    }

    public static Mesh CapsuleFlatShaded(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
    {
      if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
        return null;

      if (!caps && !sides)
        return null;

      if (s_capsuleFlatShadedMeshPool == null)
        s_capsuleFlatShadedMeshPool = new Dictionary<int, Mesh>();

      int meshKey = (latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 1 << 28 : 0) ^ (topCapOnly ? 1 << 29 : 0) ^ (sides ? 1 << 30 : 0));
      Mesh mesh;
      if (!s_capsuleFlatShadedMeshPool.TryGetValue(meshKey, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[longSegmentsPerCap * (latSegmentsPerCap - 1) * 8 + longSegmentsPerCap * 10];
        Vector3[] aNormal = new Vector3[aVert.Length];
        int[] aIndex = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];

        Vector3 top = new Vector3(0.0f, 1.5f, 0.0f);
        Vector3 bottom = new Vector3(0.0f, -1.5f, 0.0f);
        int iTop = aVert.Length - 2;
        int iBottom = aVert.Length - 1;
        aVert[iTop] = top;
        aVert[iBottom] = bottom;

        aNormal[iTop] = new Vector3(0.0f, 1.0f, 0.0f);
        aNormal[iBottom] = new Vector3(0.0f, -1.0f, 0.0f);

        float[] aLatSin = new float[latSegmentsPerCap];
        float[] aLatCos = new float[latSegmentsPerCap];
        {
          float latAngleIncrement = 0.5f * Mathf.PI / latSegmentsPerCap;
          float latAngle = 0.0f;
          for (int iLat = 0; iLat < latSegmentsPerCap; ++iLat)
          {
            latAngle += latAngleIncrement;
            aLatSin[iLat] = Mathf.Sin(latAngle);
            aLatCos[iLat] = Mathf.Cos(latAngle);
          }
        }

        float[] aLongSin = new float[longSegmentsPerCap];
        float[] aLongCos = new float[longSegmentsPerCap];
        {
          float longAngleIncrement = 2.0f * Mathf.PI / longSegmentsPerCap;
          float longAngle = 0.0f;
          for (int iLong = 0; iLong < longSegmentsPerCap; ++iLong)
          {
            longAngle += longAngleIncrement;
            aLongSin[iLong] = Mathf.Sin(longAngle);
            aLongCos[iLong] = Mathf.Cos(longAngle);
          }
        }

        int iVert = 0;
        int iNormal = 0;
        int iIndex = 0;
        for (int iLong = 0; iLong < longSegmentsPerCap; ++iLong)
        {
          float longSin = aLongSin[iLong];
          float longCos = aLongCos[iLong];
          float longSinNext = aLongSin[(iLong + 1) % longSegmentsPerCap];
          float longCosNext = aLongCos[(iLong + 1) % longSegmentsPerCap];

          for (int iLat = 0; iLat < latSegmentsPerCap; ++iLat)
          {
            float latSin = aLatSin[iLat];
            float latCos = aLatCos[iLat];

            if (caps && iLat < latSegmentsPerCap - 1)
            {
              if (iLat == 0)
              {
                int iTopTriStart = iVert;

                aVert[iVert++] = top;
                aVert[iVert++] = new Vector3(longCos * latSin, latCos + 0.5f, longSin * latSin);
                aVert[iVert++] = new Vector3(longCosNext * latSin, latCos + 0.5f, longSinNext * latSin);

                Vector3 topTriNormal = Vector3.Cross(aVert[iTopTriStart + 2] - aVert[iTopTriStart], aVert[iTopTriStart + 1] - aVert[iTopTriStart]);
                aNormal[iNormal++] = topTriNormal;
                aNormal[iNormal++] = topTriNormal;
                aNormal[iNormal++] = topTriNormal;

                aIndex[iIndex++] = iTopTriStart;
                aIndex[iIndex++] = iTopTriStart + 2;
                aIndex[iIndex++] = iTopTriStart + 1;

                if (!topCapOnly)
                {
                  int iBottomTriStart = iVert;

                  aVert[iVert++] = bottom;
                  aVert[iVert++] = new Vector3(longCos * latSin, -latCos - 0.5f, longSin * latSin);
                  aVert[iVert++] = new Vector3(longCosNext * latSin, -latCos - 0.5f, longSinNext * latSin);

                  Vector3 bottomTriNormal = Vector3.Cross(aVert[iBottomTriStart + 1] - aVert[iBottomTriStart], aVert[iBottomTriStart + 2] - aVert[iBottomTriStart]).normalized;
                  aNormal[iNormal++] = bottomTriNormal;
                  aNormal[iNormal++] = bottomTriNormal;
                  aNormal[iNormal++] = bottomTriNormal;

                  aIndex[iIndex++] = iBottomTriStart;
                  aIndex[iIndex++] = iBottomTriStart + 1;
                  aIndex[iIndex++] = iBottomTriStart + 2;
                }
              }

              float latSinNext = aLatSin[iLat + 1];
              float latCosNext = aLatCos[iLat + 1];

              if (caps)
              {
                int iTopQuadStart = iVert;

                aVert[iVert++] = new Vector3(longCos * latSin, latCos + 0.5f, longSin * latSin);
                aVert[iVert++] = new Vector3(longCos * latSinNext, latCosNext + 0.5f, longSin * latSinNext);
                aVert[iVert++] = new Vector3(longCosNext * latSinNext, latCosNext + 0.5f, longSinNext * latSinNext);
                aVert[iVert++] = new Vector3(longCosNext * latSin, latCos + 0.5f, longSinNext * latSin);

                Vector3 topQuadNormal = Vector3.Cross(aVert[iTopQuadStart + 3] - aVert[iTopQuadStart], aVert[iTopQuadStart + 1] - aVert[iTopQuadStart]);
                aNormal[iNormal++] = topQuadNormal;
                aNormal[iNormal++] = topQuadNormal;
                aNormal[iNormal++] = topQuadNormal;
                aNormal[iNormal++] = topQuadNormal;

                aIndex[iIndex++] = iTopQuadStart;
                aIndex[iIndex++] = iTopQuadStart + 2;
                aIndex[iIndex++] = iTopQuadStart + 1;

                aIndex[iIndex++] = iTopQuadStart;
                aIndex[iIndex++] = iTopQuadStart + 3;
                aIndex[iIndex++] = iTopQuadStart + 2;

                if (!topCapOnly)
                {
                  int iBottomQuadStart = iVert;

                  aVert[iVert++] = new Vector3(longCos * latSin, -latCos - 0.5f, longSin * latSin);
                  aVert[iVert++] = new Vector3(longCos * latSinNext, -latCosNext - 0.5f, longSin * latSinNext);
                  aVert[iVert++] = new Vector3(longCosNext * latSinNext, -latCosNext - 0.5f, longSinNext * latSinNext);
                  aVert[iVert++] = new Vector3(longCosNext * latSin, -latCos - 0.5f, longSinNext * latSin);

                  Vector3 bottomQuadNormal = Vector3.Cross(aVert[iBottomQuadStart + 1] - aVert[iBottomQuadStart], aVert[iBottomQuadStart + 3] - aVert[iBottomQuadStart]);
                  aNormal[iNormal++] = bottomQuadNormal;
                  aNormal[iNormal++] = bottomQuadNormal;
                  aNormal[iNormal++] = bottomQuadNormal;
                  aNormal[iNormal++] = bottomQuadNormal;

                  aIndex[iIndex++] = iBottomQuadStart;
                  aIndex[iIndex++] = iBottomQuadStart + 1;
                  aIndex[iIndex++] = iBottomQuadStart + 2;

                  aIndex[iIndex++] = iBottomQuadStart;
                  aIndex[iIndex++] = iBottomQuadStart + 2;
                  aIndex[iIndex++] = iBottomQuadStart + 3;
                }
              }
            }
            else if (sides && iLat == latSegmentsPerCap - 1)
            {
              int iSideQuadStart = iVert;

              aVert[iVert++] = new Vector3(longCos * latSin, latCos + 0.5f, longSin * latSin);
              aVert[iVert++] = new Vector3(longCos * latSin, -latCos - 0.5f, longSin * latSin);
              aVert[iVert++] = new Vector3(longCosNext * latSin, -latCos - 0.5f, longSinNext * latSin);
              aVert[iVert++] = new Vector3(longCosNext * latSin, latCos + 0.5f, longSinNext * latSin);

              Vector3 sideNormal = Vector3.Cross(aVert[iSideQuadStart + 3] - aVert[iSideQuadStart], aVert[iSideQuadStart + 1] - aVert[iSideQuadStart]).normalized;
              aNormal[iNormal++] = sideNormal;
              aNormal[iNormal++] = sideNormal;
              aNormal[iNormal++] = sideNormal;
              aNormal[iNormal++] = sideNormal;

              aIndex[iIndex++] = iSideQuadStart;
              aIndex[iIndex++] = iSideQuadStart + 2;
              aIndex[iIndex++] = iSideQuadStart + 1;

              aIndex[iIndex++] = iSideQuadStart;
              aIndex[iIndex++] = iSideQuadStart + 3;
              aIndex[iIndex++] = iSideQuadStart + 2;
            }
          }
        }

        Array.Resize(ref aIndex, iIndex);

        mesh.vertices = aVert;
        mesh.normals = aNormal;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_capsuleFlatShadedMeshPool.ContainsKey(meshKey))
          s_capsuleFlatShadedMeshPool.Remove(meshKey);

        s_capsuleFlatShadedMeshPool.Add(meshKey, mesh);
      }

      return mesh;
    }

    public static Mesh CapsuleSmoothShaded(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
    {
      if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
        return null;

      if (!caps && !sides)
        return null;

      if (s_capsuleSmoothShadedMeshPool == null)
        s_capsuleSmoothShadedMeshPool = new Dictionary<int, Mesh>();

      int meshKey = (latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 1 << 28 : 0) ^ (topCapOnly ? 1 << 29 : 0) ^ (sides ? 1 << 30 : 0));
      Mesh mesh;
      if (!s_capsuleSmoothShadedMeshPool.TryGetValue(meshKey, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
        Vector3[] aNormal = new Vector3[aVert.Length];
        int[] aIndex = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];

        Vector3 top = new Vector3(0.0f, 1.5f, 0.0f);
        Vector3 bottom = new Vector3(0.0f, -1.5f, 0.0f);
        int iTop = aVert.Length - 2;
        int iBottom = aVert.Length - 1;
        aVert[iTop] = top;
        aVert[iBottom] = bottom;

        aNormal[iTop] = new Vector3(0.0f, 1.0f, 0.0f);
        aNormal[iBottom] = new Vector3(0.0f, -1.0f, 0.0f);

        float[] aLatSin = new float[latSegmentsPerCap];
        float[] aLatCos = new float[latSegmentsPerCap];
        {
          float latAngleIncrement = 0.5f * Mathf.PI / latSegmentsPerCap;
          float latAngle = 0.0f;
          for (int iLat = 0; iLat < latSegmentsPerCap; ++iLat)
          {
            latAngle += latAngleIncrement;
            aLatSin[iLat] = Mathf.Sin(latAngle);
            aLatCos[iLat] = Mathf.Cos(latAngle);
          }
        }

        float[] aLongSin = new float[longSegmentsPerCap];
        float[] aLongCos = new float[longSegmentsPerCap];
        {
          float longAngleIncrement = 2.0f * Mathf.PI / longSegmentsPerCap;
          float longAngle = 0.0f;
          for (int iLong = 0; iLong < longSegmentsPerCap; ++iLong)
          {
            longAngle += longAngleIncrement;
            aLongSin[iLong] = Mathf.Sin(longAngle);
            aLongCos[iLong] = Mathf.Cos(longAngle);
          }
        }

        int iVert = 0;
        int iNormal = 0;
        int iIndex = 0;
        for (int iLong = 0; iLong < longSegmentsPerCap; ++iLong)
        {
          float longSin = aLongSin[iLong];
          float longCos = aLongCos[iLong];

          for (int iLat = 0; iLat < latSegmentsPerCap; ++iLat)
          {
            float latSin = aLatSin[iLat];
            float latCos = aLatCos[iLat];

            aVert[iVert    ] = new Vector3(longCos * latSin,  latCos + 0.5f, longSin * latSin);
            aVert[iVert + 1] = new Vector3(longCos * latSin, -latCos - 0.5f, longSin * latSin);

            aNormal[iNormal    ] = new Vector3(longCos * latSin,  latCos, longSin * latSin);
            aNormal[iNormal + 1] = new Vector3(longCos * latSin, -latCos, longSin * latSin);

            if (caps && iLat == 0)
            {
              aIndex[iIndex++] = iTop;
              aIndex[iIndex++] = (iVert + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
              aIndex[iIndex++] = iVert;

              if (!topCapOnly)
              {
                aIndex[iIndex++] = iBottom;
                aIndex[iIndex++] = iVert + 1;
                aIndex[iIndex++] = (iVert + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
              }
            }
            else
            {
              if (caps)
              {
                aIndex[iIndex++] = iVert - 2;
                aIndex[iIndex++] = (iVert + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                aIndex[iIndex++] = iVert;

                aIndex[iIndex++] = iVert - 2;
                aIndex[iIndex++] = (iVert - 2 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                aIndex[iIndex++] = (iVert + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);

                if (!topCapOnly)
                {
                  aIndex[iIndex++] = iVert - 1;
                  aIndex[iIndex++] = iVert + 1;
                  aIndex[iIndex++] = (iVert + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);

                  aIndex[iIndex++] = iVert - 1;
                  aIndex[iIndex++] = (iVert + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                  aIndex[iIndex++] = (iVert - 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                }
              }

              if (sides && iLat == latSegmentsPerCap - 1)
              {
                aIndex[iIndex++] = iVert;
                aIndex[iIndex++] = (iVert + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                aIndex[iIndex++] = iVert + 1;

                aIndex[iIndex++] = iVert;
                aIndex[iIndex++] = (iVert+ latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
                aIndex[iIndex++] = (iVert + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
              }
            }

            iVert += 2;
            iNormal += 2;
          }
        }

        Array.Resize(ref aIndex, iIndex);

        mesh.vertices = aVert;
        mesh.normals = aNormal;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_capsuleSmoothShadedMeshPool.ContainsKey(meshKey))
          s_capsuleSmoothShadedMeshPool.Remove(meshKey);

        s_capsuleSmoothShadedMeshPool.Add(meshKey, mesh);
      }

      return mesh;
    }

    private static Dictionary<int, Mesh> s_capsule2dWireframeMeshPool;
    private static Dictionary<int, Mesh> s_capsule2dSolidColorMeshPool;
    private static Dictionary<int, Mesh> s_capsule2dFlatShadedMeshPool;

    public static Mesh Capsule2DWireframe(int capSegments)
    {
      if (capSegments <= 0)
        return null;

      if (s_capsule2dWireframeMeshPool == null)
        s_capsule2dWireframeMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_capsule2dWireframeMeshPool.TryGetValue(capSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[(capSegments + 1) * 2];
        int[] aIndex = new int[(capSegments + 1) * 4];

        int iVert = 0;
        int iIndex = 0;
        float angleIncrement = Mathf.PI / capSegments;
        float angle = 0.0f;
        for (int i = 0; i < capSegments; ++i)
        {
          aVert[iVert++] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) + 0.5f, 0.0f);
          angle += angleIncrement;
        }
        aVert[iVert++] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) + 0.5f, 0.0f);
        for (int i = 0; i < capSegments; ++i)
        {
          aVert[iVert++] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) - 0.5f, 0.0f);
          angle += angleIncrement;
        }
        aVert[iVert++] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) - 0.5f, 0.0f);

        for (int i = 0; i < aVert.Length - 1; ++i)
        {
          aIndex[iIndex++] = i;
          aIndex[iIndex++] = (i + 1) % aVert.Length;
        }

        mesh.vertices = aVert;
        mesh.normals = aVert; // for GizmosUtil
        mesh.SetIndices(aIndex, MeshTopology.LineStrip, 0);

        if (s_capsule2dWireframeMeshPool.ContainsKey(capSegments))
          s_capsule2dWireframeMeshPool.Remove(capSegments);

        s_capsule2dWireframeMeshPool.Add(capSegments, mesh);
      }

      return mesh;
    }

    public static Mesh Capsule2DSolidColor(int capSegments)
    {
      if (capSegments <= 0)
        return null;

      if (s_capsule2dSolidColorMeshPool == null)
        s_capsule2dSolidColorMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_capsule2dSolidColorMeshPool.TryGetValue(capSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[(capSegments + 1) * 2];
        int[] aIndex = new int[(capSegments + 1) * 12];

        int iVert = 0;
        int iIndex = 0;
        float angleIncrement = Mathf.PI / capSegments;
        float angle = 0.0f;
        for (int i = 0; i < capSegments; ++i)
        {
          aVert[iVert++] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) + 0.5f, 0.0f);
          angle += angleIncrement;
        }
        aVert[iVert++] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) + 0.5f, 0.0f);
        for (int i = 0; i < capSegments; ++i)
        {
          aVert[iVert++] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) - 0.5f, 0.0f);
          angle += angleIncrement;
        }
        aVert[iVert++] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) - 0.5f, 0.0f);

        for (int i = 1; i < aVert.Length; ++i)
        {
          aIndex[iIndex++] = 0;
          aIndex[iIndex++] = (i + 1) % aVert.Length;
          aIndex[iIndex++] = i;

          aIndex[iIndex++] = 0;
          aIndex[iIndex++] = i;
          aIndex[iIndex++] = (i + 1) % aVert.Length;
        }

        mesh.vertices = aVert;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_capsule2dSolidColorMeshPool.ContainsKey(capSegments))
          s_capsule2dSolidColorMeshPool.Remove(capSegments);

        s_capsule2dSolidColorMeshPool.Add(capSegments, mesh);
      }

      return mesh;
    }

    public static Mesh Capsule2DFlatShaded(int capSegments)
    {
      if (capSegments <= 0)
        return null;

      if (s_capsule2dFlatShadedMeshPool == null)
        s_capsule2dFlatShadedMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_capsule2dFlatShadedMeshPool.TryGetValue(capSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        int numVertsPerSide = (capSegments + 1) * 2;
        Vector3[] aVert = new Vector3[numVertsPerSide * 2];
        Vector3[] aNormal = new Vector3[aVert.Length];
        int[] aIndex = new int[numVertsPerSide * 6];

        int iVert = 0;
        int iNormal = 0;
        int iIndex = 0;
        float angleIncrement = Mathf.PI / capSegments;
        float angle = 0.0f;
        for (int iSide = 0; iSide < 2; ++iSide)
        {
          for (int i = 0; i < capSegments; ++i)
          {
            aVert[iVert++] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) + 0.5f, 0.0f);
            angle += angleIncrement;
          }
          aVert[iVert++] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) + 0.5f, 0.0f);
          for (int i = 0; i < capSegments; ++i)
          {
            aVert[iVert++] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) - 0.5f, 0.0f);
            angle += angleIncrement;
          }
          aVert[iVert++] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) - 0.5f, 0.0f);

          Vector3 sideNormal = new Vector3(0.0f, 0.0f, (iSide == 0) ? -1.0f : 1.0f);
          for (int i = 0; i < numVertsPerSide; ++i)
          {
            aNormal[iNormal++] = sideNormal;
          }
        }

        for (int i = 1; i < numVertsPerSide; ++i)
        {
          aIndex[iIndex++] = 0;
          aIndex[iIndex++] = (i + 1) % numVertsPerSide;
          aIndex[iIndex++] = i;

          aIndex[iIndex++] = numVertsPerSide;
          aIndex[iIndex++] = numVertsPerSide + i;
          aIndex[iIndex++] = numVertsPerSide + ((i + 1) % numVertsPerSide);
        }

        mesh.vertices = aVert;
        mesh.normals = aNormal;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_capsule2dFlatShadedMeshPool.ContainsKey(capSegments))
          s_capsule2dFlatShadedMeshPool.Remove(capSegments);

        s_capsule2dFlatShadedMeshPool.Add(capSegments, mesh);
      }

      return mesh;
    }

    // ------------------------------------------------------------------------
    // end: capsule


    // cone
    // ------------------------------------------------------------------------

    private static Dictionary<int, Mesh> s_coneWireframeMeshPool;
    private static Dictionary<int, Mesh> s_coneSolidColorMeshPool;
    private static Dictionary<int, Mesh> s_coneFlatShadedMeshPool;
    private static Dictionary<int, Mesh> s_coneSmoothhadedMeshPool;

    public static Mesh ConeWireframe(int numSegments)
    {
      if (numSegments <= 1)
        return null;

      if (s_coneWireframeMeshPool == null)
        s_coneWireframeMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_coneWireframeMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[numSegments + 1];
        int[] aIndex = new int[numSegments * 4];

        int iTop = numSegments;

        aVert[iTop] = new Vector3(0.0f, 1.0f, 0.0f);

        int iIndex = 0;
        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          aVert[i] = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;

          aIndex[iIndex++] = i;
          aIndex[iIndex++] = (i + 1) % numSegments;

          aIndex[iIndex++] = i;
          aIndex[iIndex++] = iTop;

          angle += angleIncrement;
        }

        mesh.vertices = aVert;
        mesh.normals = aVert; // for GizmosUtil
        mesh.SetIndices(aIndex, MeshTopology.Lines, 0);

        if (s_coneWireframeMeshPool.ContainsKey(numSegments))
          s_coneWireframeMeshPool.Remove(numSegments);

        s_coneWireframeMeshPool.Add(numSegments, mesh);
      }

      return mesh;
    }

    public static Mesh ConeSolidColor(int numSegments)
    {
      if (numSegments <= 1)
        return null;

      if (s_coneSolidColorMeshPool == null)
        s_coneSolidColorMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_coneSolidColorMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[numSegments + 1];
        int[] aIndex = new int[numSegments * 3 + (numSegments - 2) * 3];

        int iTop = numSegments;

        aVert[iTop] = new Vector3(0.0f, 1.0f, 0.0f);

        int iIndex = 0;
        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          aVert[i] = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;

          aIndex[iIndex++] = iTop;
          aIndex[iIndex++] = (i + 1) % numSegments;
          aIndex[iIndex++] = i;

          if (i >= 2)
          {
            aIndex[iIndex++] = 0;
            aIndex[iIndex++] = i - 1;
            aIndex[iIndex++] = i;
          }

          angle += angleIncrement;
        }

        mesh.vertices = aVert;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_coneSolidColorMeshPool.ContainsKey(numSegments))
          s_coneSolidColorMeshPool.Remove(numSegments);

        s_coneSolidColorMeshPool.Add(numSegments, mesh);
      }

      return mesh;
    }

    public static Mesh ConeFlatShaded(int numSegments)
    {
      if (numSegments <= 1)
        return null;

      if (s_coneFlatShadedMeshPool == null)
        s_coneFlatShadedMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_coneFlatShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[numSegments * 3 + numSegments];
        Vector3[] aNormal = new Vector3[aVert.Length];
        int[] aIndex = new int[numSegments * 3 + (numSegments - 2) * 3];

        Vector3 top = new Vector3(0.0f, 1.0f, 0.0f);

        Vector3[] aBaseVert = new Vector3[numSegments];
        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          aBaseVert[i] = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          angle += angleIncrement;
        }

        int iVert = 0;
        int iIndex = 0;
        int iNormal = 0;
        for (int i = 0; i < numSegments; ++i)
        {
          int iSideTriStart = iVert;

          aVert[iVert++] = top;
          aVert[iVert++] = aBaseVert[i];
          aVert[iVert++] = aBaseVert[(i + 1) % numSegments];

          Vector3 sideTriNormal = Vector3.Cross(aVert[iSideTriStart + 2] - aVert[iSideTriStart], aVert[iSideTriStart + 1] - aVert[iSideTriStart]).normalized;
          aNormal[iNormal++] = sideTriNormal;
          aNormal[iNormal++] = sideTriNormal;
          aNormal[iNormal++] = sideTriNormal;

          aIndex[iIndex++] = iSideTriStart;
          aIndex[iIndex++] = iSideTriStart + 2;
          aIndex[iIndex++] = iSideTriStart + 1;
        }

        int iBaseStart = iVert;
        for (int i = 0; i < numSegments; ++i)
        {
          aVert[iVert++] = aBaseVert[i];

          aNormal[iNormal++] = new Vector3(0.0f, -1.0f, 0.0f);

          if (i >= 2)
          {
            aIndex[iIndex++] = iBaseStart;
            aIndex[iIndex++] = iBaseStart + i - 1;
            aIndex[iIndex++] = iBaseStart + i;
          }
        }

        mesh.vertices = aVert;
        mesh.normals = aNormal;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_coneFlatShadedMeshPool.ContainsKey(numSegments))
          s_coneFlatShadedMeshPool.Remove(numSegments);

        s_coneFlatShadedMeshPool.Add(numSegments, mesh);
      }

      return mesh;
    }

    public static Mesh ConeSmoothShaded(int numSegments)
    {
      if (numSegments <= 1)
        return null;

      if (s_coneSmoothhadedMeshPool == null)
        s_coneSmoothhadedMeshPool = new Dictionary<int, Mesh>();

      Mesh mesh;
      if (!s_coneSmoothhadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
      {
        mesh = new Mesh();

        Vector3[] aVert = new Vector3[numSegments * 2 + 1];
        Vector3[] aNormal = new Vector3[aVert.Length];
        int[] aIndex = new int[numSegments * 3 + (numSegments - 2) * 3];

        int iTop = aVert.Length - 1;

        aVert[iTop] = new Vector3(0.0f, 1.0f, 0.0f);
        aNormal[iTop] = new Vector3(0.0f, 0.0f, 0.0f);

        float sqrt2Inv = Mathf.Sqrt(0.5f);

        int iIndex = 0;
        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          float cos = Mathf.Cos(angle);
          float sin = Mathf.Sin(angle);

          Vector3 baseVert = cos * Vector3.right + sin * Vector3.forward;
          aVert[i] = baseVert;
          aVert[numSegments + i] = baseVert;

          aNormal[i] = new Vector3(cos * sqrt2Inv, sqrt2Inv, sin * sqrt2Inv);
          aNormal[numSegments + i] = new Vector3(0.0f, -1.0f, 0.0f);

          aIndex[iIndex++] = iTop;
          aIndex[iIndex++] = (i + 1) % numSegments;
          aIndex[iIndex++] = i;

          if (i >= 2)
          {
            aIndex[iIndex++] = numSegments;
            aIndex[iIndex++] = numSegments + i - 1;
            aIndex[iIndex++] = numSegments + i;
          }

          angle += angleIncrement;
        }

        mesh.vertices = aVert;
        mesh.normals = aNormal;
        mesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        if (s_coneSmoothhadedMeshPool.ContainsKey(numSegments))
          s_coneSmoothhadedMeshPool.Remove(numSegments);

        s_coneSmoothhadedMeshPool.Add(numSegments, mesh);
      }

      return mesh;
    }

    // ------------------------------------------------------------------------
    // end: cone
  }
}
