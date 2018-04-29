using UnityEngine;

namespace Cube.Voxelworld {
    public class VoxelworldViewer : MonoBehaviour {
        void Start() {
            var voxelworld = gameObject.GetSystem<VoxelworldSystem>();
            voxelworld.viewers.Add(this);
        }
    }
}