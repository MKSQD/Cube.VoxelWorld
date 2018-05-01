using UnityEngine;

namespace Cube.Voxelworld {
    public interface IVoxelworldListener {
        void ChangedVoxel(Voxel voxel, Chunk chunk, IntVector3 blockLocalPosition);
    }
}