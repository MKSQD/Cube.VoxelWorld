using System;

namespace Cube.Voxelworld {
    public struct Voxel : IEquatable<Voxel> {
        public byte type;

        public static Voxel empty = new Voxel();

        public Voxel(byte type) {
            this.type = type;
        }

        public bool Equals(Voxel other) {
            return type == other.type;
        }

        public override bool Equals(object other) {
            return Equals((Voxel)other);
        }

        public override int GetHashCode() {
            return type.GetHashCode();
        }
    }
}