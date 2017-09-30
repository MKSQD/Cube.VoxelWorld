using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Voxelworld
{
    [Serializable]
    public struct VoxelTypeDefinition
    {
        public string name;
        public bool isNotMergable;
        public bool isTransparent;
        public string prefabPath;
    }

    [CreateAssetMenu]
    public class VoxelTypesDefinition : ScriptableObject
    {
        public string prefabBasePath;
        public List<VoxelTypeDefinition> types;
    }
}