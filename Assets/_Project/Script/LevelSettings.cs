using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class LevelSettings : ScriptableObject
{
    public List<CharacterSetting> Characters;
}
