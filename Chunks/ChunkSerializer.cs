using System;
using System.IO;
using UnityEngine;

namespace Core.Voxelworld
{
    public class ChunkSerializer
    {
        const int MAX_SERIALIZED_DATA_LENGTH = 1024 * 10;    //10KB

        int _currentVersion = 1;

        public bool TryLoad(IntVector3 pos, out ChunkVoxelData voxelData, out ChunkFunctionalBlocksDeferredDeserialization functionalBlocks)
        {
            voxelData = null;
            functionalBlocks = new ChunkFunctionalBlocksDeferredDeserialization();

            try {
                using (var fileStream = new FileStream(GetFilePathForChunkPosition(pos), FileMode.Open)) {
                    var reader = new BinaryReader(fileStream);

                    var version = reader.ReadInt32();
                    if (version != _currentVersion) {
                        Debug.Log("Chunk data outdated, loading failed (old_version=" + version + ", current_version=" + _currentVersion + ", position=" + pos + ")");
                        return false;
                    }

                    // Voxel data
                    voxelData = new ChunkVoxelData();
                    for (int i = 0; i < ChunkVoxelData.dataSize; ++i) {
                        voxelData.data[i].type = (VoxelType)reader.ReadByte();
                    }

                    // Functional blocks
                    var numFunctionalBlocks = reader.ReadInt32();
                    for (int i = 0; i < numFunctionalBlocks; ++i) {
                        var functionalBlockPos = new IntVector3() {
                            x = reader.ReadInt32(),
                            y = reader.ReadInt32(),
                            z = reader.ReadInt32()
                        };

                        int dataLength = reader.ReadInt32();
                        if (dataLength > MAX_SERIALIZED_DATA_LENGTH)
                            throw new Exception("Serialized data exceeds MAX_SERIALIZED_DATA_LENGTH( " + MAX_SERIALIZED_DATA_LENGTH + " bytes)");

                        if (dataLength > 0) {
                            byte[] buffer = new byte[dataLength];
                            reader.Read(buffer, 0, dataLength);
                            functionalBlocks.data.Add(functionalBlockPos, buffer);
                        }
                    }

                    return true;
                }
            }
            catch (FileNotFoundException) {
                return false;
            }
        }

        public void Save(IntVector3 pos, Chunk chunk)
        {
            var memoryStream = new MemoryStream();
            using (var writer = new BinaryWriter(memoryStream)) {
                writer.Write(_currentVersion);

                // Voxel data
                for (int i = 0; i < ChunkVoxelData.dataSize; ++i) {
                    writer.Write((byte)chunk.voxelData.data[i].type);
                }

                // Functional blocks
                writer.Write(chunk.functionalBlocks.Count);

                foreach (var pair in chunk.functionalBlocks) {
                    var functionalBlockPos = pair.Key;
                    writer.Write(functionalBlockPos.x);
                    writer.Write(functionalBlockPos.y);
                    writer.Write(functionalBlockPos.z);

                    var serializableGameObjects = pair.Value.GetComponentsInChildren<ISerializable>();
                    int serializedLength = 0;

                    var serializableMemoryStream = new MemoryStream();
                    using (var serializableWriter = new BinaryWriter(serializableMemoryStream)) {
                        foreach (var serializable in serializableGameObjects)
                            serializable.Serialize(serializableWriter);

                        serializedLength = (int)serializableMemoryStream.Length;
                    }

                    if (serializedLength > MAX_SERIALIZED_DATA_LENGTH)
                        throw new Exception("Serialized data exceeds MAX_SERIALIZED_DATA_LENGTH( " + MAX_SERIALIZED_DATA_LENGTH + " bytes)");

                    writer.Write(serializedLength);
                    writer.Write(serializableMemoryStream.ToArray());
                }
            }

            File.WriteAllBytes(GetFilePathForChunkPosition(pos), memoryStream.ToArray());
        }

        string GetFilePathForChunkPosition(IntVector3 pos)
        {
            return string.Format("Save/{0}_{1}_{2}.Chunk", pos.x, pos.y, pos.z);
        }
    }
}