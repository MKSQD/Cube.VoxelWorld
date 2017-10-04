using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Voxelworld
{
    public class ChunkVoxelData
    {
        public static readonly int dataSize = WorldManager.chunkSize * WorldManager.chunkSize * WorldManager.chunkSize;

        Voxel[] _data;
        public Voxel[] data {
            get { return _data; }
        }

        public ChunkVoxelData()
        {
            _data = new Voxel[dataSize];
        }

        public unsafe Voxel Get(int x, int y, int z)
        {
            try {
                return _data[x + WorldManager.chunkSize * (y + WorldManager.chunkSize * z)];
            }
            catch (System.IndexOutOfRangeException) {
                Debug.LogError("IndexOutOfRangeException: (" + x + "," + y + "," + z + ")");
                throw;
            }
        }

        public unsafe void Set(int x, int y, int z, Voxel voxel)
        {
            try {
                _data[x + WorldManager.chunkSize * (y + WorldManager.chunkSize * z)] = voxel;
            }
            catch (System.IndexOutOfRangeException) {
                Debug.LogError("IndexOutOfRangeException: (" + x + "," + y + "," + z + ")");
                throw;
            }
        }
    }

    public class ChunkFunctionalBlocksDeferredDeserialization
    {
        public Dictionary<IntVector3, byte[]> data = new Dictionary<IntVector3, byte[]>();
    }

    public class Chunk
    {
        ChunkVoxelData _voxelData;
        public ChunkVoxelData voxelData {
            get { return _voxelData; }
        }

        public Chunk(ChunkVoxelData voxelData)
        {
            _voxelData = voxelData;
        }

        public GameObject gameObject;
        public Dictionary<IntVector3, GameObject> functionalBlocks = new Dictionary<IntVector3, GameObject>();
    }
}