using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bub_Dialogue{

    [TextArea(3, 10)]
    public string[] sentences;

    public float[] delays;

    public float[] Bubble_scale_x;
    public float[] Bubble_scale_y;

    public int[] font_size;

}
