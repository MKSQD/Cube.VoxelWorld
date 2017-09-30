using System;
using UnityEngine;

namespace Core.Voxelworld
{
    public enum VoxelType : byte
    {
        None,
        Dirt,
        Concrete,
        Iron,
        Miner,
        Plc,
        Belt,
        Exporter,
        Furnace,
        Press,
        RoboticArm,
        Lamp,
        Switch,
        Storage,
        Tree,

        COUNT
    }

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
            switch (type) {
                case VoxelType.Belt:
                    return Prefabs.Belt;

                case VoxelType.Exporter:
                    return Prefabs.Exporter;

                case VoxelType.Miner:
                    return Prefabs.Miner;

                case VoxelType.Furnace:
                    return Prefabs.Furnace;

                case VoxelType.Press:
                    return Prefabs.Press;

                case VoxelType.RoboticArm:
                    return Prefabs.RoboticArm;

                case VoxelType.Plc:
                    return Prefabs.Plc;

                case VoxelType.Lamp:
                    return Prefabs.Lamp;

                case VoxelType.Switch:
                    return Prefabs.Switch;

                case VoxelType.Storage:
                    return Prefabs.Storage;

                case VoxelType.Tree:
                    return Prefabs.Tree;
            }
            return null;
        }

        public static bool IsTypeMergable(VoxelType type)
        {
            switch (type) {
                case VoxelType.Plc:
                case VoxelType.Miner:
                    return false;

                default:
                    return true;
            }
        }

        public static bool IsTypeTransparent(VoxelType type)
        {
            switch (type) {
                case VoxelType.None:
                case VoxelType.Belt:
                case VoxelType.Exporter:
                case VoxelType.Furnace:
                case VoxelType.Press:
                case VoxelType.RoboticArm:
                case VoxelType.Lamp:
                case VoxelType.Switch:
                case VoxelType.Storage:
                case VoxelType.Tree:
                    return true;

                default:
                    return false;
            }
        }

        public static Vector3 GetTypeBuildOffset(VoxelType type)
        {
            switch (type) {
                case VoxelType.Press:
                    return new Vector3(0, 1, 0);
            }
            return Vector3.zero;
        }
    }
}