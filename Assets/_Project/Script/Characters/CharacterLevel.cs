using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "CharacterLevel")]
public class CharacterLevel : ScriptableObject
{
    [Header("Level")]
    public int Level;
    public int XPToNextLevel;

    [Header("Attributes")]
    public int HP;
    public int Strength;
    public int Dexterity;
    public int Constitution;
    public int Intelligence;
    public int Speed;
    public int Astral;

    [Header("Attack Power")]
    public int BasicAttack;

    [Header("Barbarian Attacks Power")]
    public int SpinAttack;

    [Header("Ranger Attacks Power")]
    public int FrostShot;
    public int PetSummon;

    [Header("Mage Attacks Power")]
    public int Heal;
    public int Thunder;
}