using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "CharacterLevel")]
public class CharacterLevel : ScriptableObject
{
    [Header("Level")]
    public int Level;
    public int XPToNextLevel;

    [Header("Attributes")]
    public int Strength;
    public int Dexterity;
    public int Constitution;
    public int Intelligence;
    public int Speed;
    public int Astral;
}