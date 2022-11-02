using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "Items")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
}
