using System;
using System.IO;
using System.Collections.Generic;

interface ISerializable {
    void Serialize(BinaryWriter writer);
    void Deserialize(BinaryReader reader);
}
