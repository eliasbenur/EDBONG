using UnityEngine;

[CreateAssetMenu(fileName = "New Stuff", menuName ="Stuff")]
public class Stuff : ScriptableObject
{
    public string stuff_name;
    public int life;
    public int shield;
    public Sprite sprite;
    public int speedBoost;
    public int cost;
}
