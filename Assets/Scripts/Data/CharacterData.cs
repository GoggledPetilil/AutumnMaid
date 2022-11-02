using UnityEngine;

[CreateAssetMenu(fileName = "New Character",menuName = "Characters")]
public class CharacterData : ScriptableObject
{
    public string charName;
    public AudioClip voiceClip;
    public Sprite portraitSprite;
}
