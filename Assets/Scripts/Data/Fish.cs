using UnityEngine;

[CreateAssetMenu(fileName = "New Fish",menuName = "Fishes")]
public class Fish : Item
{
    public float fishHealth;
    public float rageDuration;
    public float rageFrequency;
    public bool isTrash;
    public bool isSpecial;
}