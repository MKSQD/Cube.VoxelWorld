using System;
using UnityEngine;

namespace Cube.Voxelworld {
    public class ChunkProvider {
        public class Request {
            public IntVector3 position;
            public volatile bool done;
            public Chunk chunk;
            public ChunkMesherResult mesherResult;
        }

        VoxelTypeManager _voxelTypeManager;
        ThreadPool _threadPool = new ThreadPool();

        public ChunkProvider(VoxelTypeManager voxelTypeManager) {
            _voxelTypeManager = voxelTypeManager;
        }

        public Request Load(IntVector3 chunkPosition, int priority) {
            var request = new Request {
                position = chunkPosition
            };
            _threadPool.QueueTask(() => { GenerateInBackground(chunkPosition, request); }, priority);
            return request;
        }

        public Request Remesh(IntVector3 chunkPosition, Chunk chunk, int priority) {
            var request = new Request() {
                position = chunkPosition,
                chunk = chunk
            };
            _threadPool.QueueTask(() => { GenerateInBackground(chunkPosition, request); }, priority);
            return request;
        }

        public Chunk GetResult(Request request) {
            if (!request.done)
                return null;

            var chunk = request.chunk;

            if (chunk.gameObject == null) {
                var name = string.Format("Chunk_{0}_{1}_{2}", request.position.x, request.position.y, request.position.z);
                chunk.gameObject = new GameObject(name);
                chunk.gameObject.transform.position = new Vector3(request.position.x * VoxelworldSystem.chunkSize,
                    request.position.y * VoxelworldSystem.chunkSize,
                    request.position.z * VoxelworldSystem.chunkSize);

                var newMeshFilter = chunk.gameObject.AddComponent<MeshFilter>();
                newMeshFilter.mesh = new Mesh();
                newMeshFilter.mesh.MarkDynamic();

                chunk.gameObject.AddComponent<MeshRenderer>();
                chunk.gameObject.AddComponent<MeshCollider>();
            }

            var meshFilter = chunk.gameObject.GetComponent<MeshFilter>();
            var meshCollider = chunk.gameObject.GetComponent<MeshCollider>();

            meshFilter.mesh.Clear();
            meshFilter.mesh.SetVertices(request.mesherResult.vertices);
            meshFilter.mesh.SetTriangles(request.mesherResult.indices, 0);
            meshFilter.mesh.SetNormals(request.mesherResult.normals);
            meshFilter.mesh.SetUVs(0, request.mesherResult.uvs);
            meshFilter.mesh.RecalculateNormals();

            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = meshFilter.mesh;
            
            return chunk;
        }
        
        void GenerateInBackground(IntVector3 chunkPosition, Request request) {
            try {
                if (request.chunk == null) {
                    var voxelData = ChunkGenerator.Generate(chunkPosition, _voxelTypeManager);
                    request.chunk = new Chunk(voxelData);
                }

                request.mesherResult = ChunkMesher.Generate(chunkPosition, request.chunk.voxelData);
                request.done = true;
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
        }
    }
}