using System;
using UnityEngine;

namespace Cube.Voxelworld {
    public struct Voxel {
        public VoxelType type;

        public static Voxel empty = new Voxel(VoxelType.None);

        public Voxel(VoxelType type) {
            this.type = type;
        }
    }
}