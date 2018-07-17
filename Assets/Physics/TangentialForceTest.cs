using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TangentialForceTest : MonoBehaviour
{
    public GameObject m_particleOne;
    public GameObject m_particleTwo;
    public GameObject m_particleOneVelocity;
    public GameObject m_particleTwoVelocity;
    public Material m_velocityMaterial;
    public Material m_tangentialMaterial;
    public Material m_springMaterial;
    public Material m_dampingMaterial;
    public float m_tangentialScalar;
    public float m_springForceScalar;
    public float m_dampingForceScalar;

    struct MeshAndMat {
        public Mesh m_mesh;
        public Material m_material;
    }
    private List<MeshAndMat> m_drawThese;
    void Awake() {
        Application.targetFrameRate = 60;
    }
    void OnEnable() {
        m_drawThese = new List<MeshAndMat>();
        Camera.onPreCull += Render;
    }
    void OnDisable() {
        m_drawThese.Clear();
        m_drawThese = null;
        Camera.onPreCull -= Render;
    }
    void Render(Camera c)
    {
        foreach (var mm in m_drawThese) {
            Graphics.DrawMesh(mm.m_mesh, Matrix4x4.identity, mm.m_material, 1, c, 0, null, false, false, false);
            //Graphics.DrawMesh(mesh, Matrix4x4.identity, m, 1, Camera.main, 0, null, false, false, false);
        }
    }
    void DrawLine(Vector3 start, Vector3 end, Material m) {
        var mesh = CjLib.PrimitiveMeshFactory.Line(start,end);
        var meshAndMat = new MeshAndMat();
        meshAndMat.m_mesh = mesh;
        meshAndMat.m_material = m;
        m_drawThese.Add(meshAndMat);
    }
    void Update() {
        m_drawThese.Clear();
        //draw velocities
        var posOne = m_particleOne.transform.position;
        var velOne = m_particleOneVelocity.transform.position - posOne;
        DrawLine(posOne, posOne + velOne, m_velocityMaterial);

        var posTwo = m_particleTwo.transform.position;
        var velTwo = m_particleTwoVelocity.transform.position - posTwo;
        DrawLine(posTwo, posTwo + velTwo, m_velocityMaterial);

        // draw spring force vector
        var springIJ = m_springForceScalar * SpringForceVector(posOne, posTwo);
        var springJI = m_springForceScalar * SpringForceVector(posTwo, posOne);
        DrawLine(posOne, posOne + springIJ, m_springMaterial);
        DrawLine(posTwo, posTwo + springJI, m_springMaterial);

        // draw damping force vector
        var dampingIJ = m_dampingForceScalar * DampingForceVector(velOne, velTwo);
        var dampingJI = m_dampingForceScalar * DampingForceVector(velTwo, velOne);
        DrawLine(posOne, posOne + dampingIJ, m_dampingMaterial);
        DrawLine(posTwo, posTwo + dampingJI, m_dampingMaterial);

        //draw tangentialforce vector
        var vijt = m_tangentialScalar * TangentialForceVector(posOne, posTwo, velOne, velTwo);
        var vjit = m_tangentialScalar * TangentialForceVector(posTwo, posOne, velTwo, velOne);
        DrawLine(posOne, posOne + vijt, m_tangentialMaterial);
        DrawLine(posTwo, posTwo + vjit, m_tangentialMaterial);
    }

    Vector3 TangentialForceVector(Vector3 pi, Vector3 pj, Vector3 vi, Vector3 vj) {
        var pij = pj - pi;
        var vij = vj - vi;
        var pijn = pij.normalized;
        var vijt = vij - pijn * Vector3.Dot(vij, pijn);
        return vijt;
    }
    Vector3 SpringForceVector(Vector3 pi, Vector3 pj) {
        var pij = pj - pi;
        var pijn = pij.normalized;
        float diameter = 1.0f;
        float penetration = pij.magnitude;
        if (diameter >= penetration)
            return -1.0f * (diameter - penetration) * pijn;
        return Vector3.zero;
    }
    
    Vector3 DampingForceVector(Vector3 vi, Vector3 vj) {
        var vij = vj - vi;
        return vij;
    }
}