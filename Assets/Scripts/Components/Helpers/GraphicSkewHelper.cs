// @Author: tanjinhua
// @Date: 2021/1/10  9:17


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicSkewHelper
{
    public static void PopulateMeshWithSkew(VertexHelper toFill, Vector2 skew)
    {
        Mesh mesh = new Mesh();
        toFill.FillMesh(mesh);
        List<Vector3> positions = GetSkewedPositions(mesh.vertices, skew);

        toFill.Clear();
        List<Vector2> uv0s = new List<Vector2>();
        List<Vector2> uv1s = new List<Vector2>();
        mesh.GetUVs(0, uv0s);
        mesh.GetUVs(1, uv1s);
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 position = positions[i];
            Color colour = mesh.colors[i];
            Vector2 uv0 = uv0s[i];
            Vector2 uv1 = uv1s[i];
            Vector3 normal = mesh.normals[i];
            Vector4 tangent = mesh.tangents[i];
            toFill.AddVert(position, colour, uv0, uv1, normal, tangent);
        }

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int idx0 = mesh.triangles[i];
            int idx1 = mesh.triangles[i + 1];
            int idx2 = mesh.triangles[i + 2];
            toFill.AddTriangle(idx0, idx1, idx2);
        }
    }

    private static List<Vector3> GetSkewedPositions(Vector3[] vertices, Vector2 skew)
    {
        List<Vector3> result = new List<Vector3>();
        float[] ys = new float[vertices.Length];
        float[] xs = new float[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 position = vertices[i];
            ys[i] = position.y;
            xs[i] = position.x;
        }

        float tanX = Mathf.Tan(skew.x * Mathf.Deg2Rad);
        float tanY = Mathf.Tan(skew.y * Mathf.Deg2Rad);
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].x += ys[i] * tanX;
            vertices[i].y += xs[i] * tanY;

            result.Add(vertices[i]);
        }

        return result;
    }
}