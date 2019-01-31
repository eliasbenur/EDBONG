using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileData2
{
    [System.Serializable]
    public struct rowData
    {
        public int[] row;
    }

    public rowData[] rows = new rowData[15];
}