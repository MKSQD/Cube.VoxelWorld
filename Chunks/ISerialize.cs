using System.IO;

namespace Core.Voxelworld
{
    interface ISerializable
    {
        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }
}