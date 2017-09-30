using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Voxelworld
{
    public interface IWorldManagerListener
    {
        void Initialize();
        void Shutdown();
        void VoxelChanged(Vector3 worldPosition, Voxel voxel);
    }
}