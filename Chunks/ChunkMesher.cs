using UnityEngine;


namespace Core.Voxelworld
{
    public enum Side
    {
        Top,
        Bottom,
        Center,
        Left,
        Right,
        Front,
        Back
    }

    public class ChunkMesher
    {
        public static unsafe ChunkMesherResult Generate(IntVector3 position, ChunkVoxelData voxelData)
        {
            var result = new ChunkMesherResult();

            for (bool backFace = true, b = false; b != backFace; backFace = backFace && b, b = !b) {
                for (int dimension = 0; dimension < 3; dimension++) {
                    int i, j, k, l, w, h;
                    int u = (dimension + 1) % 3;
                    int v = (dimension + 2) % 3;
                    var x = new int[3];
                    var q = new int[3];
                    var mask = new VoxelType[WorldManager.chunkSize * WorldManager.chunkSize];

                    q[dimension] = 1;

                    for (x[dimension] = -1; x[dimension] < WorldManager.chunkSize;) {
                        // Compute the mask
                        int n = 0;
                        for (x[v] = 0; x[v] < WorldManager.chunkSize; ++x[v]) {
                            for (x[u] = 0; x[u] < WorldManager.chunkSize; ++x[u]) {
                                VoxelType v1 = x[dimension] >= 0 ? voxelData.Get(x[0], x[1], x[2]).type : VoxelType.None;
                                VoxelType v2 = (x[dimension] < WorldManager.chunkSize - 1) ? voxelData.Get(x[0] + q[0], x[1] + q[1], x[2] + q[2]).type : VoxelType.None;

                                if (v1 == v2) {
                                    mask[n++] = VoxelType.None;
                                }
                                else {
                                    mask[n++] = backFace ? v2 : v1;
                                }
                            }
                        }

                        // Increment x[d]
                        ++x[dimension];

                        // Generate mesh for mask using lexicographic ordering
                        n = 0;
                        for (j = 0; j < WorldManager.chunkSize; ++j) {
                            for (i = 0; i < WorldManager.chunkSize;) {
                                var maskVoxelType = mask[n];
                                if (VoxelTypes.IsTransparent(maskVoxelType)) {
                                    ++i;
                                    ++n;
                                    continue;
                                }

                                if (VoxelTypes.IsMergable(maskVoxelType)) {
                                    // Compute width
                                    for (w = 1; i + w < WorldManager.chunkSize && mask[n + w] == maskVoxelType; ++w)
                                        ;

                                    // Compute height (this is slightly awkward)
                                    var done = false;
                                    for (h = 1; j + h < WorldManager.chunkSize; ++h) {
                                        for (k = 0; k < w; ++k) {
                                            if (mask[n + k + h * WorldManager.chunkSize] != maskVoxelType) {
                                                done = true;
                                                break;
                                            }
                                        }
                                        if (done)
                                            break;
                                    }
                                }
                                else {
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
                                Vector3 normal = Vector3.zero;

                                if (!backFace) {
                                    switch (dimension) {
                                        case 0: normal = Vector3.left; break;
                                        case 1: normal = Vector3.up; break;
                                        case 2: normal = Vector3.forward; break;
                                    }

                                    result.BuildTriangle(
                                        new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]),
                                        new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                        new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                        new Vector3(x[0], x[1], x[2]),
                                        normal,
                                        maskVoxelType);
                                }
                                else {
                                    switch (dimension) {
                                        case 0: normal = Vector3.right; break;
                                        case 1: normal = Vector3.down; break;
                                        case 2: normal = Vector3.back; break;
                                    }

                                    result.BuildTriangle(
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
                                        mask[n + k + l * WorldManager.chunkSize] = VoxelType.None;
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