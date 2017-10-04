using System;
using UnityEngine;

namespace Core.Voxelworld
{
    public struct Voxel
    {
        public VoxelType type;

        public static Voxel empty = new Voxel();

        public Voxel(VoxelType type)
        {
            this.type = type;
        }
        
        public static Vector3 GetTypeBuildOffset(VoxelType type)
        {
//             switch (type) {
//                 case VoxelType.Press:
//                     return new Vector3(0, 1, 0);
//             }
            return Vector3.zero;
        }
    }
}