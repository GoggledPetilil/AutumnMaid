using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogues")]
public class Dialogue : ScriptableObject
{
    [System.Serializable]
    public class Info
    {
        public CharacterData character;
        [Multiline(3)] public string sentence;
    }

    public bool increasesHappiness;
    public Info[] dialogueInfo;
}
