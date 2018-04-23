using Cube;
using UnityEngine;

namespace Core.Voxelworld
{
    public interface IWorldManagerListener
    {
        void Initialize();
        void Shutdown();
        void VoxelChanged(IntVector3 worldPosition, Voxel voxel);
    }
}