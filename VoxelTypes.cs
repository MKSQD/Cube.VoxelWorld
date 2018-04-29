using UnityEngine;

public static class VoxelTypes {
    public static bool IsMergable(VoxelType type) {
        switch (type) {
            case VoxelType.Miner: return false;
            case VoxelType.Plc: return false;
            default: return true;
        }
    }

    public static bool IsTransparent(VoxelType type) {
        switch (type) {
            case VoxelType.None: return true;
            case VoxelType.Belt: return true;
            case VoxelType.Exporter: return true;
            case VoxelType.Furnace: return true;
            case VoxelType.Press: return true;
            case VoxelType.RoboticArm: return true;
            case VoxelType.Lamp: return true;
            case VoxelType.Switch: return true;
            case VoxelType.Storage: return true;
            case VoxelType.Tree: return true;
            default: return false;
        }
    }
    public static string GetDisplayName(VoxelType type) {
        switch (type) {
            case VoxelType.Dirt: return "Dirt";
            case VoxelType.Concrete: return "Concrete";
            case VoxelType.Iron: return "Iron";
            case VoxelType.Miner: return "Miner";
            case VoxelType.Plc: return "Plc";
            case VoxelType.Belt: return "Belt";
            case VoxelType.Exporter: return "Exporter";
            case VoxelType.Furnace: return "Furnace";
            case VoxelType.Press: return "Press";
            case VoxelType.RoboticArm: return "RoboticArm";
            case VoxelType.Lamp: return "Lamp";
            case VoxelType.Switch: return "Switch";
            case VoxelType.Storage: return "Storage";
            case VoxelType.Tree: return "Tree";
            default: return "Unknown";
        }
    }
    public static GameObject GetFunctionalBlockPrefabForType(VoxelType type) {
        switch (type) {
            case VoxelType.Miner: return (GameObject)Resources.Load("Assets/Blocks/Resources/Miner.prefab", typeof(GameObject));
            case VoxelType.Plc: return (GameObject)Resources.Load("Assets/Blocks/Resources/Plc.prefab", typeof(GameObject));
            case VoxelType.Belt: return (GameObject)Resources.Load("Assets/Blocks/Resources/Belt.prefab", typeof(GameObject));
            case VoxelType.Furnace: return (GameObject)Resources.Load("Assets/Blocks/Resources/Furnace.prefab", typeof(GameObject));
            case VoxelType.Press: return (GameObject)Resources.Load("Assets/Blocks/Resources/Press.prefab", typeof(GameObject));
            case VoxelType.RoboticArm: return (GameObject)Resources.Load("Assets/Blocks/Resources/RoboticArm.prefab", typeof(GameObject));
            case VoxelType.Lamp: return (GameObject)Resources.Load("Assets/Blocks/Resources/Lamp.prefab", typeof(GameObject));
            case VoxelType.Switch: return (GameObject)Resources.Load("Assets/Blocks/Resources/Switch.prefab", typeof(GameObject));
            case VoxelType.Storage: return (GameObject)Resources.Load("Assets/Blocks/Resources/Storage.prefab", typeof(GameObject));
            case VoxelType.Tree: return (GameObject)Resources.Load("Assets/Blocks/Resources/Tree.prefab", typeof(GameObject));
            default: return null;
        }
    }
}
