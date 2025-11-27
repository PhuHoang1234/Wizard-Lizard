using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    public bool illusionUnlock = false;
    public bool castUnlock = false;


    public bool[] abilityList;

    private void OnEnable()
    {
        abilityList = new bool[] { illusionUnlock, castUnlock };
    }
}
