using UnityEngine;

[System.Serializable]
public enum Direction
{
    Up,
    Right,
    Down,
    Left
}

[System.Serializable]
public class IndicatorOption
{
    public bool Active;
    public Direction Direction;
}

[System.Serializable]
public class RuneData : ScriptableObject
{
    public string Name;
    public Sprite Image;
    public Color Color;
    public AudioClip EnemyClip;
    public IndicatorOption[] Options = new IndicatorOption[8];
    public AudioClip[] PlayerClips = new AudioClip[8];
}
