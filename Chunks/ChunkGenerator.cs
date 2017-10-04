using System;
using UnityEngine;

namespace Core.Voxelworld
{
    public class ChunkGenerator
    {
        public static ChunkVoxelData Generate(IntVector3 position)
        {
            var voxelData = new ChunkVoxelData();

            // Generate
            var worldPosition = position * WorldManager.chunkSize;

            Voxel voxel;
            for (int z = 0; z < WorldManager.chunkSize; ++z) {
                for (int y = 0; y < WorldManager.chunkSize; ++y) {
                    for (int x = 0; x < WorldManager.chunkSize; ++x) {
                        var density = Density(worldPosition.x + x, worldPosition.y + y, worldPosition.z + z);
                        voxel.type = density > 0.5f ? Type(worldPosition.x + x, worldPosition.y + y, worldPosition.z + z) : VoxelType.None;

                        // Trees
                        if (x % 4 == 0 && z % 4 == 0 && density <= 0.5f) {
                            if (Density(worldPosition.x + x, worldPosition.y + y - 1, worldPosition.z + z) > 0.5f) {
                                bool tree = true;
                                for (int i = 1; i < 8; ++i) {
                                    if (Density(worldPosition.x + x, worldPosition.y + i, worldPosition.z + z) > 0.5f) {
                                        tree = false;
                                        break;
                                    }
                                }

                                if (tree && SimplexNoise.Noise.Generate((worldPosition.x + x) * 0.01f, (worldPosition.z + z) * 0.01f) > 0.5f) {
                                    voxel.type = VoxelType.Tree;
                                }
                            }
                        }

                        voxelData.Set(x, y, z, voxel);
                    }
                }
            }

            return voxelData;
        }

        static float Density(float x, float y, float z)
        {
            var density = SimplexNoise.Noise.Generate(x * 0.01f, y * 0.01f, z * 0.01f);
            density += Mathf.Max(-y, 0) * 0.5f;
            density *= 8 / Mathf.Max(y, 1);
            return density;
        }

        static VoxelType Type(float x, float y, float z)
        {
            if (y < 10) {
                if (SimplexNoise.Noise.Generate(x * 0.01f, y * 0.01f, z * 0.02f) < 0.5f)
                    return VoxelType.Dirt;

                if (SimplexNoise.Noise.Generate(x * 0.08f, y * 0.08f, z * 0.08f) > 0.2f)
                    return VoxelType.Iron;
            }
            return VoxelType.Concrete;
        }
    }
}