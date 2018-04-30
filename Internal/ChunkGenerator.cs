using UnityEngine;

namespace Cube.Voxelworld {
    public static class ChunkGenerator {
        public static ChunkVoxelData Generate(IntVector3 position, VoxelTypeManager voxelTypeManager) {
            var voxelData = new ChunkVoxelData();

            // Generate
            var worldPosition = position * VoxelworldSystem.chunkSize;

            Voxel voxel;
            for (int z = 0; z < VoxelworldSystem.chunkSize; ++z) {
                for (int y = 0; y < VoxelworldSystem.chunkSize; ++y) {
                    for (int x = 0; x < VoxelworldSystem.chunkSize; ++x) {
                        var density = Density(worldPosition.x + x, worldPosition.y + y, worldPosition.z + z);
                        voxel.type = density > 0.5f ? Type(voxelTypeManager, worldPosition.x + x, worldPosition.y + y, worldPosition.z + z) : (byte)0;

                        voxelData.Set(x, y, z, voxel);
                    }
                }
            }

            return voxelData;
        }

        static float Density(float x, float y, float z) {
            var density = SimplexNoise.Noise.Generate(x * 0.01f, y * 0.01f, z * 0.01f);
            density += Mathf.Max(-y, 0) * 0.5f;
            density *= 8 / Mathf.Max(y, 1);
            return density;
        }

        static byte Type(VoxelTypeManager voxelTypeManager, float x, float y, float z) {
            for (byte i = 2; i < voxelTypeManager.voxelTypes.Count; ++i) {
                var voxelType = voxelTypeManager.voxelTypes[i];
                if (!voxelType.generateInWorld)
                    continue;

                if (SimplexNoise.Noise.Generate(i * 100 + x * 0.03f, y * 0.01f, i * 300 + z * 0.02f) < 0.5f)
                    return i;
            }
            return 1;
        }
    }
}