using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTerrain : MonoBehaviour {

    public Terrain Terrain;

    private float[,] originalHeights;

    private void OnDestroy()
    {
        this.Terrain.terrainData.SetHeights(0, 0, this.originalHeights);
    }

    private void Awake()
    {
        this.originalHeights = this.Terrain.terrainData.GetHeights(
            0, 0, this.Terrain.terrainData.heightmapWidth, this.Terrain.terrainData.heightmapHeight);
    }

}
