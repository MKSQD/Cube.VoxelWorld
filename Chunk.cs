using UnityEngine;

namespace Cube.Voxelworld {
    public class ChunkVoxelData {
        public static readonly int dataSize = VoxelworldSystem.chunkSize * VoxelworldSystem.chunkSize * VoxelworldSystem.chunkSize;

        Voxel[] _data;
        public Voxel[] data {
            get { return _data; }
        }

        public ChunkVoxelData() {
            _data = new Voxel[dataSize];
        }

        public unsafe Voxel Get(int x, int y, int z) {
            try {
                return _data[x + VoxelworldSystem.chunkSize * (y + VoxelworldSystem.chunkSize * z)];
            }
            catch (System.IndexOutOfRangeException) {
                Debug.LogError("IndexOutOfRangeException: (" + x + "," + y + "," + z + ")");
                throw;
            }
        }

        public unsafe void Set(int x, int y, int z, Voxel voxel) {
            try {
                _data[x + VoxelworldSystem.chunkSize * (y + VoxelworldSystem.chunkSize * z)] = voxel;
            }
            catch (System.IndexOutOfRangeException) {
                Debug.LogError("IndexOutOfRangeException: (" + x + "," + y + "," + z + ")");
                throw;
            }
        }
    }

    public class Chunk {
        public ChunkVoxelData voxelData;
        public GameObject gameObject;

        public Chunk(ChunkVoxelData voxelData) {
            this.voxelData = voxelData;
        }
    }
}