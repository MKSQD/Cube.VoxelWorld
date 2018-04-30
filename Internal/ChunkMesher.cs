using UnityEngine;

namespace Cube.Voxelworld {
    public class ChunkMesher {
        enum Side {
            Top,
            Bottom,
            Center,
            Left,
            Right,
            Front,
            Back
        }

        public static unsafe ChunkMesherResult Generate(IntVector3 position, ChunkVoxelData voxelData) {
            var result = new ChunkMesherResult();

            for (bool backFace = true, b = false; b != backFace; backFace = backFace && b, b = !b) {
                for (int dimension = 0; dimension < 3; ++dimension) {
                    int i, j, k, l, w, h;
                    int u = (dimension + 1) % 3;
                    int v = (dimension + 2) % 3;
                    var x = new int[3];
                    var q = new int[3];
                    var mask = new byte[VoxelworldSystem.chunkSize * VoxelworldSystem.chunkSize];

                    q[dimension] = 1;

                    for (x[dimension] = -1; x[dimension] < VoxelworldSystem.chunkSize;) {
                        // Compute the mask
                        int n = 0;
                        for (x[v] = 0; x[v] < VoxelworldSystem.chunkSize; ++x[v]) {
                            for (x[u] = 0; x[u] < VoxelworldSystem.chunkSize; ++x[u]) {
                                byte voxelType1 = x[dimension] >= 0 ? voxelData.Get(x[0], x[1], x[2]).type : (byte)0;
                                byte voxelType2 = (x[dimension] < VoxelworldSystem.chunkSize - 1) ? voxelData.Get(x[0] + q[0], x[1] + q[1], x[2] + q[2]).type : (byte)0;

                                var faceNotVisible = voxelType1 != 0 && voxelType2 != 0;
                                if (voxelType1 == voxelType2 || faceNotVisible) {
                                    mask[n++] = 0;
                                } else {
                                    mask[n++] = backFace ? voxelType2 : voxelType1;
                                }
                            }
                        }

                        ++x[dimension];

                        // Generate mesh for mask using lexicographic ordering
                        n = 0;
                        for (j = 0; j < VoxelworldSystem.chunkSize; ++j) {
                            for (i = 0; i < VoxelworldSystem.chunkSize;) {
                                var maskVoxelType = mask[n];
                                if (maskVoxelType == 0) {
                                    ++i;
                                    ++n;
                                    continue;
                                }

                                var isVoxelTypeMergable = true; // maskVoxelType
                                if (isVoxelTypeMergable) {
                                    // Compute width
                                    for (w = 1; i + w < VoxelworldSystem.chunkSize && mask[n + w] == maskVoxelType; ++w)
                                        ;

                                    // Compute height (this is slightly awkward)
                                    var done = false;
                                    for (h = 1; j + h < VoxelworldSystem.chunkSize; ++h) {
                                        for (k = 0; k < w; ++k) {
                                            if (mask[n + k + h * VoxelworldSystem.chunkSize] != maskVoxelType) {
                                                done = true;
                                                break;
                                            }
                                        }
                                        if (done)
                                            break;
                                    }
                                } else {
                                    w = 1;
                                    h = 1;
                                }

                                // Add quad
                                x[u] = i;
                                x[v] = j;
                                var du = new int[3];
                                var dv = new int[3];
                                du[u] = w;
                                dv[v] = h;
                                var normal = Vector3.zero;

                                if (!backFace) {
                                    switch (dimension) {
                                        case 0: normal = Vector3.left; break;
                                        case 1: normal = Vector3.up; break;
                                        case 2: normal = Vector3.forward; break;
                                    }

                                    result.AddQuad(
                                        new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]),
                                        new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                        new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                        new Vector3(x[0], x[1], x[2]),
                                        normal,
                                        maskVoxelType);
                                } else {
                                    switch (dimension) {
                                        case 0: normal = Vector3.right; break;
                                        case 1: normal = Vector3.down; break;
                                        case 2: normal = Vector3.back; break;
                                    }

                                    result.AddQuad(
                                       new Vector3(x[0], x[1], x[2]),
                                        new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                        new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                        new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]),
                                        normal,
                                        maskVoxelType);
                                }

                                // Zero-out mask
                                for (l = 0; l < h; ++l) {
                                    for (k = 0; k < w; ++k) {
                                        mask[n + k + l * VoxelworldSystem.chunkSize] = 0;
                                    }
                                }

                                i += w;
                                n += w;
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}