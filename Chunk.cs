using System.Collections.Generic;
using UnityEngine;

namespace Cube.Voxelworld {
    public class ChunkVoxelData {
        public static readonly int dataSize = VoxelworldSystem.chunkSize * VoxelworldSystem.chunkSize * VoxelworldSystem.chunkSize;

        Voxel[] _data;
        public Voxel[] data {
            get { return _data; }
        }

        public ChunkVoxelData() {
            _data = new Voxel[dataSize];
        }

        public Voxel Get(int x, int y, int z) {
            return _data[x + VoxelworldSystem.chunkSize * (y + VoxelworldSystem.chunkSize * z)];
        }

        public void Set(int x, int y, int z, Voxel voxel) {
            _data[x + VoxelworldSystem.chunkSize * (y + VoxelworldSystem.chunkSize * z)] = voxel;
        }
    }

    public class Chunk {
        struct UserDataPair {
            public ulong key;
            public object value;
        }

        public ChunkVoxelData voxelData;
        public GameObject gameObject;

        List<UserDataPair> _userData = new List<UserDataPair>();

        public Chunk(ChunkVoxelData voxelData) {
            this.voxelData = voxelData;
        }

        public T GetUserData<T>(string role) where T : class {
            var roleHash = FastStringHash(role);

            for (int i = 0; i < _userData.Count; ++i) {
                var pair = _userData[i];
                if (pair.key == roleHash)
                    return (T)pair.value;
            }
            return null;
        }

        public T GetOrCreateUserData<T>(string role) where T : class, new() {
            var roleHash = FastStringHash(role);

            for (int i = 0; i < _userData.Count; ++i) {
                var pair = _userData[i];
                if (pair.key == roleHash)
                    return (T)pair.value;
            }

            var newData = new T();
            var newPair = new UserDataPair() {
                key = roleHash,
                value = newData
            };
            _userData.Add(newPair);

            return newData;
        }

        static ulong FastStringHash(string read) {
            ulong hashedValue = 3074457345618258791ul;
            for (int i = 0; i < read.Length; i++) {
                hashedValue += read[i];
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }
    }
}