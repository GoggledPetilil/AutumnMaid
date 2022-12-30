using UnityEngine;

[CreateAssetMenu(fileName = "New Quest",menuName = "Quests")]
public class Quest : ScriptableObject
{
    public int id;
    public string description;
}
