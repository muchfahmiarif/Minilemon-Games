using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object Item/Item")]
public class Item : ScriptableObject
{
    [Header("Only gameplay")]
    public ItemType type;
    public ActionType actionType;
    public Vector2Int range = new Vector2Int(5, 4);

    [Header("Only UI")]
    public bool stackable = true;
    public int maxStackedItems = 1;

    [Header("Both")]
    public Sprite image;
    public GameObject prefab; 

    [Header("Item Transform Settings")]
    public Vector3 holdPosition;
    public Vector3 holdRotation;
    public Vector3 holdScale = Vector3.one;

    [Header("Item Information")]
    public string name;
    [TextArea]
    public string itemDescription;

    public enum ItemType
    {
        BuildingBlock,
        Tool,
        Food,
        Seed 
    }

    public enum ActionType
    {
        Dig,
        Mine,
        Eat,
        Plant
    }
}
