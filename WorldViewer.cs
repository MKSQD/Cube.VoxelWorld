using UnityEngine;
using System.Collections.Generic;

public class WorldViewer : MonoBehaviour {
    public static List<WorldViewer> all = new List<WorldViewer>();

    void Awake() {
        all.Add(this);
    }
}
