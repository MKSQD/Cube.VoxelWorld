using UnityEngine;
using System.Collections.Generic;

namespace Cube.Voxelworld {
    public class ChunkMesherResult {
        public List<Vector3> vertices = new List<Vector3>();
        public List<int> indices = new List<int>();
        public List<Vector2> uvs = new List<Vector2>();
        public List<Vector3> normals = new List<Vector3>();

        public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 normal, byte voxelType) {
            int baseIndice = vertices.Count;

            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);

            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);

            int voxelTypeCount = 5; // Texture tile count
            float offsetPerVoxelType = 1.0f / voxelTypeCount;

            float startY = 1 - offsetPerVoxelType * (float)(voxelType - 1);
            float endY = 1 - offsetPerVoxelType * (float)voxelType;

            uvs.Add(new Vector2(0, startY));
            uvs.Add(new Vector2(1, startY));
            uvs.Add(new Vector2(1, endY));
            uvs.Add(new Vector2(0, endY));

            indices.Add(baseIndice + 2);
            indices.Add(baseIndice + 1);
            indices.Add(baseIndice + 0);
            indices.Add(baseIndice + 0);
            indices.Add(baseIndice + 3);
            indices.Add(baseIndice + 2);
        }
    }
}