
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cube.Voxelworld {
    [Serializable]
    public struct VoxelTypeDescription {
        public string name;
        public bool generateInWorld;
    }

    [Serializable]
    public class VoxelTypeManager {
        public List<VoxelTypeDescription> voxelTypes = new List<VoxelTypeDescription>();
        
        public void AddVoxelType(VoxelTypeDescription description) {
            voxelTypes.Add(description);

            Debug.Log("Added VoxelType '" + description.name + "'");
        }
    }
}