using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "Items")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite sprite;
}