using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterSetting : ScriptableObject
{
    public GameObject CharacterPrefab;
    public Vector2 Level1Spawn;
    public Vector2 Level2Spawn;
    public Vector2 Level3Spawn;
    public Vector2 Level4Spawn;

}
