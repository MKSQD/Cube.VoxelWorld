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

        public static GameObject GetFunctionalBlockPrefabForType(VoxelType type)
        {
//             switch (type) {
//                 case VoxelType.Belt:
//                     return Prefabs.Belt;
// 
//                 case VoxelType.Exporter:
//                     return Prefabs.Exporter;
// 
//                 case VoxelType.Miner:
//                     return Prefabs.Miner;
// 
//                 case VoxelType.Furnace:
//                     return Prefabs.Furnace;
// 
//                 case VoxelType.Press:
//                     return Prefabs.Press;
// 
//                 case VoxelType.RoboticArm:
//                     return Prefabs.RoboticArm;
// 
//                 case VoxelType.Plc:
//                     return Prefabs.Plc;
// 
//                 case VoxelType.Lamp:
//                     return Prefabs.Lamp;
// 
//                 case VoxelType.Switch:
//                     return Prefabs.Switch;
// 
//                 case VoxelType.Storage:
//                     return Prefabs.Storage;
// 
//                 case VoxelType.Tree:
//                     return Prefabs.Tree;
//             }
            return null;
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