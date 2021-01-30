using UnityEngine;

[System.Serializable]
public enum PentagramOwner
{
    Player,
    Enemy,
    Both
}

[System.Serializable]
public class RuneOption
{
    public RuneData Data;
    public RuneData PlayerData;
    public PentagramOwner Owner;
}

[System.Serializable]
public class StageData : ScriptableObject
{
    public RuneOption[] Options;
}
