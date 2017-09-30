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
        public GameObject prefab;
    }

    [CreateAssetMenu]
    public class VoxelTypesDefinition : ScriptableObject
    {
        public List<VoxelTypeDefinition> types;
    }
}