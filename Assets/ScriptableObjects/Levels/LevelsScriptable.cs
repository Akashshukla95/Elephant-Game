using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level", menuName = "Level Data/Levels")]

public class LevelsScriptable : ScriptableObject
{
    public List<MechanismsScriptable> mechanismScriptables;
}
