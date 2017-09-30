using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManagerRequest
{
    public IntVector3 position;
    public volatile bool done;
    public Chunk chunk;
    public ChunkMesherResult mesherResult;
    public ChunkFunctionalBlocksDeferredDeserialization functionalBlocksDeferredDeserialization;
}

public class ChunkManager : MonoBehaviourSingleton<ChunkManager>
{
    ThreadPool _threadPool = new ThreadPool();

    ChunkSerializer _serializer;

    public ChunkManagerRequest Load(IntVector3 chunkPosition, int priority)
    {
        var request = new ChunkManagerRequest();
        request.position = chunkPosition;
        _threadPool.QueueTask(() => { GenerateInBackground(chunkPosition, request); }, priority);
        return request;
    }

    public ChunkManagerRequest Remesh(IntVector3 chunkPosition, Chunk chunk, int priority)
    {
        var request = new ChunkManagerRequest() {
            position = chunkPosition,
            chunk = chunk
        };
        _threadPool.QueueTask(() => { GenerateInBackground(chunkPosition, request); }, priority);
        return request;
    }

    public Chunk GetResult(ChunkManagerRequest request)
    {
        if (!request.done)
            return null;

        var chunk = request.chunk;

        if (chunk.gameObject == null) {
            var name = string.Format("Chunk_{0}_{1}_{2}", request.position.x, request.position.y, request.position.z);
            chunk.gameObject = new GameObject(name);
            chunk.gameObject.transform.position = new Vector3(request.position.x * WorldManager.chunkSize,
                request.position.y * WorldManager.chunkSize,
                request.position.z * WorldManager.chunkSize);

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

        // Functional blocks
        foreach (var functionalBlock in chunk.functionalBlocks.Values) {
            var interface_ = functionalBlock.GetComponent<FunctionalBlock>();
            if (interface_ != null) {
                interface_.ChunkChanged();
            }
        }
        
        //
        var aliveFunctionalBlockPositions = new List<IntVector3>();
        for (int x = 0; x < WorldManager.chunkSize; ++x) {
            for (int y = 0; y < WorldManager.chunkSize; ++y) {
                for (int z = 0; z < WorldManager.chunkSize; ++z) {
                    var voxel = chunk.voxelData.Get(x, y, z);

                    var prefab = Voxel.GetFunctionalBlockPrefabForType(voxel.type);
                    if (prefab != null) {
                        var pos = new IntVector3(x, y, z);
                        aliveFunctionalBlockPositions.Add(pos);

                        // Assumes that the functional block type of a block never changes (so we don't check)
                        if (chunk.functionalBlocks.ContainsKey(pos))
                            continue;

                        var machineGO = Instantiate(prefab);
                        machineGO.name = voxel.type.ToString();
                        machineGO.transform.parent = chunk.gameObject.transform;
                        machineGO.transform.localPosition = new Vector3(x, y, z);
                        chunk.functionalBlocks.Add(pos, machineGO);
                    }
                }
            }
        }

        var positionsToRemove = new List<IntVector3>();
        foreach (var pair in chunk.functionalBlocks) {
            if (aliveFunctionalBlockPositions.Contains(pair.Key))
                continue;

            positionsToRemove.Add(pair.Key);
        }

        foreach (var pos in positionsToRemove) {
            var functionalBlock = chunk.functionalBlocks[pos];
            Destroy(functionalBlock);
            chunk.functionalBlocks.Remove(pos);
        }

        // Deserialize data
        if (request.functionalBlocksDeferredDeserialization != null) {
            foreach (var pair in request.functionalBlocksDeferredDeserialization.data) {
                byte[] data;
                if (!request.functionalBlocksDeferredDeserialization.data.TryGetValue(pair.Key, out data))
                    continue;

                GameObject functionalBlockGO;
                if (!chunk.functionalBlocks.TryGetValue(pair.Key, out functionalBlockGO))
                    continue;

                var deserializableGameObjects = functionalBlockGO.GetComponentsInChildren<ISerializable>();
                if (deserializableGameObjects.Length == 0)
                    continue;

                using (var memoryStream = new MemoryStream(data)) {
                    using (var reader = new BinaryReader(memoryStream)) {
                        foreach (var deserializable in deserializableGameObjects)
                            deserializable.Deserialize(reader);
                    }
                }
            }


        }
        
        return chunk;
    }

    ChunkManager()
    {
        _serializer = new ChunkSerializer();
    }

    void GenerateInBackground(IntVector3 chunkPosition, ChunkManagerRequest request)
    {
        try {
            if (request.chunk == null) {
                ChunkVoxelData voxelData;
                ChunkFunctionalBlocksDeferredDeserialization functionalBlocks;
                var deserialized = _serializer.TryLoad(chunkPosition, out voxelData, out functionalBlocks);
                if (!deserialized) {
                    voxelData = ChunkGenerator.Generate(chunkPosition);
                }

                request.chunk = new Chunk(voxelData);
                request.functionalBlocksDeferredDeserialization = functionalBlocks;

                if (!deserialized) {
                    SerializeChunkBlocking(chunkPosition, request.chunk);
                }
            }

            request.mesherResult = ChunkMesher.Generate(chunkPosition, request.chunk.voxelData);
            request.done = true;
        }
        catch (Exception e) {
            Debug.LogException(e);
        }
    }

    public void SerializeChunkBlocking(IntVector3 pos, Chunk chunk)
    {
        _serializer.Save(pos, chunk);
    }
}
