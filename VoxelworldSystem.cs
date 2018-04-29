using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Cube.Voxelworld {
    public class VoxelworldSystem : MonoBehaviour {
        public static int chunkSize = 16;
        public Material chunkMaterial;

        public int chunkLoadRangeXZ = 1;
        public int chunkLoadRangeY = 1;
        public int chunkMaxLoaded = 100;

        public List<VoxelworldViewer> viewers = new List<VoxelworldViewer>();

        ChunkProvider _chunkProvider = new ChunkProvider();

        Dictionary<IntVector3, Chunk> _chunks = new Dictionary<IntVector3, Chunk>();
        HashSet<IntVector3> _dirtyChunks = new HashSet<IntVector3>();
        Dictionary<IntVector3, ChunkProvider.Request> _loadingChunks = new Dictionary<IntVector3, ChunkProvider.Request>();
        Dictionary<IntVector3, float> _positionAgeMap = new Dictionary<IntVector3, float>();

        void Awake() {
            gameObject.SetSystem(this);
        }

        void Update() {
            RebuildDirtyChunks();
            UpdatePendingChunks();
            LoadNewChunks();
            //UnloadOldChunks();
        }

        public Voxel GetVoxelAtWorldPosition(Vector3 worldPosition) {
            var chunkPosition = WorldToChunkPosition(worldPosition);

            Chunk chunk;
            if (!_chunks.TryGetValue(chunkPosition, out chunk))
                return Voxel.empty;

            var blockPosition = WorldToBlockLocalPosition(worldPosition);
            return chunk.voxelData.Get(blockPosition.x, blockPosition.y, blockPosition.z);
        }

        public Voxel PlaceVoxelAtWorldPosition(Vector3 worldPosition, Voxel voxel) {
            var chunkPosition = WorldToChunkPosition(worldPosition);

            Chunk chunk;
            if (!_chunks.TryGetValue(chunkPosition, out chunk))
                return Voxel.empty;

            var blockLocalPosition = WorldToBlockLocalPosition(worldPosition);
            var prevVoxel = chunk.voxelData.Get(blockLocalPosition.x, blockLocalPosition.y, blockLocalPosition.z);
            chunk.voxelData.Set(blockLocalPosition.x, blockLocalPosition.y, blockLocalPosition.z, voxel);

            TouchChunk(chunkPosition, blockLocalPosition);

            return prevVoxel;
        }

        void TouchChunk(IntVector3 chunkPosition, IntVector3 blockLocalPosition) {
            _dirtyChunks.Add(chunkPosition);

            if (blockLocalPosition.x == 0) {
                _dirtyChunks.Add(chunkPosition + new IntVector3(-1, 0, 0));
            } else if (blockLocalPosition.x == chunkSize - 1) {
                _dirtyChunks.Add(chunkPosition + new IntVector3(1, 0, 0));
            }
            if (blockLocalPosition.y == 0) {
                _dirtyChunks.Add(chunkPosition + new IntVector3(0, -1, 0));
            } else if (blockLocalPosition.y == chunkSize - 1) {
                _dirtyChunks.Add(chunkPosition + new IntVector3(0, 1, 0));
            }
            if (blockLocalPosition.z == 0) {
                _dirtyChunks.Add(chunkPosition + new IntVector3(0, 0, -1));
            } else if (blockLocalPosition.z == chunkSize - 1) {
                _dirtyChunks.Add(chunkPosition + new IntVector3(0, 0, 1));
            }
        }

        static public IntVector3 WorldToChunkPosition(Vector3 worldPositon) {
            return new IntVector3() {
                x = Mathf.FloorToInt(worldPositon.x / chunkSize),
                y = Mathf.FloorToInt(worldPositon.y / chunkSize),
                z = Mathf.FloorToInt(worldPositon.z / chunkSize)
            };
        }

        static public IntVector3 WorldToBlockWorldPosition(Vector3 worldPositon) {
            return new IntVector3(Mathf.FloorToInt(worldPositon.x),
                Mathf.FloorToInt(worldPositon.y),
                Mathf.FloorToInt(worldPositon.z));
        }

        static public IntVector3 WorldToBlockLocalPosition(Vector3 worldPositon) {
            var chunkPosition = WorldToChunkPosition(worldPositon);
            var blockPosition = WorldToBlockWorldPosition(worldPositon);

            return new IntVector3(blockPosition.x - chunkPosition.x * chunkSize,
                blockPosition.y - chunkPosition.y * chunkSize,
                blockPosition.z - chunkPosition.z * chunkSize);
        }

        void RebuildDirtyChunks() {
            var newDirtyChunks = new HashSet<IntVector3>();

            foreach (var pos in _dirtyChunks) {
                // Chunks currently loading are still dirty
                if (_loadingChunks.ContainsKey(pos)) {
                    newDirtyChunks.Add(pos);
                    continue;
                }
                if (!_chunks.ContainsKey(pos))
                    continue;

                ReloadChunk(pos, 0);
            }
            _dirtyChunks = newDirtyChunks;
        }

        void LoadNewChunks() {
            var chunkPriorityPairsToLoad = new List<KeyValuePair<int, IntVector3>>();

            foreach (var viewer in viewers) {
                var viewerPosition = WorldToChunkPosition(viewer.transform.position);

                for (int x = viewerPosition.x - chunkLoadRangeXZ; x <= viewerPosition.x + chunkLoadRangeXZ; ++x) {
                    for (int y = viewerPosition.y - chunkLoadRangeY; y <= viewerPosition.y + chunkLoadRangeY; ++y) {
                        for (int z = viewerPosition.z - chunkLoadRangeXZ; z <= viewerPosition.z + chunkLoadRangeXZ; ++z) {
                            var chunkPosition = new IntVector3(x, y, z);
                            _positionAgeMap[chunkPosition] = Time.time;

                            var chunkLoadedOrLoading = _chunks.ContainsKey(chunkPosition)
                                || _loadingChunks.ContainsKey(chunkPosition);
                            if (chunkLoadedOrLoading)
                                continue;

                            var priority = (int)((viewerPosition - chunkPosition).magnitude * 10f) + 1;
                            chunkPriorityPairsToLoad.Add(new KeyValuePair<int, IntVector3>(priority, chunkPosition));
                        }
                    }
                }
            }

            foreach (var pair in chunkPriorityPairsToLoad.OrderBy(p => p.Key)) {
                LoadChunk(pair.Value, pair.Key);
            }
        }

        public void LoadChunk(IntVector3 chunkPosition, int priority) {
            Assert.IsTrue(!_loadingChunks.ContainsKey(chunkPosition));

            var request = _chunkProvider.Load(chunkPosition, priority);
            _loadingChunks.Add(chunkPosition, request);
        }

        public void ReloadChunk(IntVector3 chunkPosition, int priority) {
            var chunk = _chunks[chunkPosition];
            var request = _chunkProvider.Remesh(chunkPosition, chunk, priority);
            _loadingChunks.Add(chunkPosition, request);
        }

        void UnloadOldChunks() {
            while (_chunks.Keys.Count > chunkMaxLoaded) {
                IntVector3? oldestChunkPosition = null;
                float oldestChunkAge = float.MaxValue;
                foreach (var pair in _positionAgeMap) {
                    if (pair.Value < oldestChunkAge) {
                        oldestChunkPosition = pair.Key;
                        oldestChunkAge = pair.Value;
                    }
                }

                if (!oldestChunkPosition.HasValue)
                    break;

                GameObject.Destroy(_chunks[oldestChunkPosition.Value].gameObject);

                _dirtyChunks.Remove(oldestChunkPosition.Value);
                _chunks.Remove(oldestChunkPosition.Value);
                _positionAgeMap.Remove(oldestChunkPosition.Value);

                break; // One per frame
            }
        }

        void UpdatePendingChunks() {
            var loaded = new List<IntVector3>();
            foreach (var pair in _loadingChunks) {
                var request = pair.Value;
                if (!request.done)
                    continue;

                var chunk = _chunkProvider.GetResult(request);

                var chunkGO = chunk.gameObject;
                chunkGO.transform.parent = transform;

                var meshRenderer = chunkGO.GetComponent<MeshRenderer>();
                meshRenderer.material = chunkMaterial;

                loaded.Add(pair.Key);
                _chunks[pair.Key] = chunk;
                _positionAgeMap[pair.Key] = Time.time;

                break; // One per frame
            }

            foreach (var pos in loaded) {
                _loadingChunks.Remove(pos);
            }
        }
    }
}