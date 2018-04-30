using System;
using UnityEngine;

namespace Cube.Voxelworld {
    public struct Voxel {
        public byte type;

        public static Voxel empty = new Voxel();

        public Voxel(byte type) {
            this.type = type;
        }
    }
}