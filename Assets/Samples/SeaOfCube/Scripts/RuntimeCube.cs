// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using System.Collections.Generic;

public class RuntimeCube : MonoBehaviour {
    public int x = 13;
    public int y = 13;
    public int z = 13;
    public float size = 0.2f;
    public float gap = 2f;

    // Use this for initialization
    void OnEnable () {
        var meshfilter = GetComponent<MeshFilter>();
        meshfilter.mesh = createMesh(x, y, z, size, gap);
    }

    class MeshData
    {
        public int x;
        public int y;
        public int z;
        public float size;
        public float gap;
        public int currentIdx;
        public List<Vector3> vertices;
        public List<Vector2> uv;
        public List<Vector3> normals;
        public List<int> indices;
        public Vector3 position;
    }

    private static Mesh createMesh(int x, int y, int z, float size, float gap)
    {
        print("Create Mesh");
        Mesh mesh = new Mesh();
        const int verticesCount = 14;
        const int indicesCount = 18;
        int cubes = x * y * z;
        if (cubes == 0)
            return null;

        MeshData data = new MeshData();
        data.x = x;
        data.y = y;
        data.z = z;
        data.size = size;
        data.gap = gap;
        data.currentIdx = 0;
        data.vertices = new List<Vector3>(cubes * verticesCount);
        data.uv = new List<Vector2>(cubes * verticesCount);
        data.normals = new List<Vector3>(cubes * verticesCount);
        data.indices = new List<int>(cubes * indicesCount);

        print("Begin loop Mesh");
        for (int i = 0; i < x; i++)
        {
            float dx = (-((x - 1f) / 2f) + i) * gap;
            for (int j = 0; j < y; j++)
            {
                float dy = (-((y - 1f) / 2f) + j) * gap;
                for (int k = 0; k < z; k++)
                {
                    float dz = (-((z - 1f) / 2f) + k) * gap;
                    data.position = new Vector3(dx, dy, dz);
                    createACube(ref data);
                }
            }
        }

        mesh.vertices = data.vertices.ToArray();
        mesh.uv = data.uv.ToArray();
        mesh.normals = data.normals.ToArray();
        mesh.SetIndices(data.indices.ToArray(), MeshTopology.Triangles, 0);
        mesh.name = "CubemapCubes";
        return mesh;
    }

    private static void createACube(ref MeshData data)
    {
        /*
             H-----G
            /|    /|
           D-+---C |
           | |   | |
           | E---+-F
           |/    |/
           A-----B
        */
        float size = data.size;
        Vector3 pos = data.position;
        var A = pos + new Vector3(-size, -size, -size);
        var B = pos + new Vector3(size, -size, -size);
        var C = pos + new Vector3(size, size, -size);
        var D = pos + new Vector3(-size, size, -size);
        var E = pos + new Vector3(-size, -size, size);
        var F = pos + new Vector3(size, -size, size);
        var G = pos + new Vector3(size, size, size);
        var H = pos + new Vector3(-size, size, size);
        Debug.Log("A=" + A + " G=" + G);

        var NA = new Vector3(-1, -1, -1);
        var NB = new Vector3(1, -1, -1);
        var NC = new Vector3(1, 1, -1);
        var ND = new Vector3(-1, 1, -1);
        var NE = new Vector3(-1, -1, 1);
        var NF = new Vector3(1, -1, 1);
        var NG = new Vector3(1, 1, 1);
        var NH = new Vector3(-1, 1, 1);
        const float ustep = 1 / 4.0f;
        const float vstep = 1 / 3.0f;

        // Texture (0,0) is from left bottom.

        // 0 ~ 3
        data.vertices.Add(A);
        data.normals.Add(NA);
        data.uv.Add(new Vector2(ustep, vstep));
        data.vertices.Add(B);
        data.normals.Add(NB);
        data.uv.Add(new Vector2(2 * ustep, vstep));
        data.vertices.Add(C);
        data.normals.Add(NC);
        data.uv.Add(new Vector2(2 * ustep, 2 * vstep));
        data.vertices.Add(D);
        data.normals.Add(ND);
        data.uv.Add(new Vector2(ustep, 2 * vstep));

        // 4 ~ 7
        data.vertices.Add(E);
        data.normals.Add(NE);
        data.uv.Add(new Vector2(1, vstep));
        data.vertices.Add(F);
        data.normals.Add(NF);
        data.uv.Add(new Vector2(3 * ustep, vstep));
        data.vertices.Add(G);
        data.normals.Add(NG);
        data.uv.Add(new Vector2(3 * ustep, 2 * vstep));
        data.vertices.Add(H);
        data.normals.Add(NH);
        data.uv.Add(new Vector2(1, 2 * vstep));

        //// 8 ~ 13
        data.vertices.Add(E);
        data.normals.Add(NE);
        data.uv.Add(new Vector2(0, 1 * vstep));
        data.vertices.Add(H);
        data.normals.Add(NH);
        data.uv.Add(new Vector2(0, 2 * vstep));
        data.vertices.Add(H);
        data.normals.Add(NH);
        data.uv.Add(new Vector2(ustep, 1));
        data.vertices.Add(G);
        data.normals.Add(NG);
        data.uv.Add(new Vector2(2 * ustep, 1));
        data.vertices.Add(F);
        data.normals.Add(NF);
        data.uv.Add(new Vector2(2 * ustep, 0));
        data.vertices.Add(E);
        data.normals.Add(NA);
        data.uv.Add(new Vector2(ustep, 2 * 0));

        // Clockwise
        int[] indices = {
            0,2,1, 0,3,2, // Front C
            4,5,6, 4,6,7, // Back B
            8,9,3, 8,3,0, // Left U
            1,2,6, 1,6,5, // Right E
            3,10,11, 3,11,2, // Top
            0,1,12, 0,12,13 // Bottom
        };
        foreach (int i in indices)
        {
            data.indices.Add(i+data.currentIdx);
        }
        data.currentIdx += 14;
    }
}
