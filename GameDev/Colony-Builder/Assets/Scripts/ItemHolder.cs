using Mono.CSharp;
using System.Linq;
using UnityEngine;

public enum Direction { Down, Up, Left, Right } // Add more if needed, e.g., diagonals

public class ItemHolder : MonoBehaviour
{
    ItemDatabase itemDB => ItemDatabase.Instance;


    [SerializeField] public Transform equippedItem; // Reference to the item's Transform


    [SerializeField] private Vector2 offsetDown = new Vector2(0.5f, 0f); // Example defaults
    [SerializeField] private Vector2 offsetUp = new Vector2(0.5f, 0.5f);
    [SerializeField] private Vector2 offsetLeft = new Vector2(-0.5f, 0f);
    [SerializeField] private Vector2 offsetRight = new Vector2(0.5f, 0f);

    [SerializeField]
    private Direction currentDirection = Direction.Down; // Track current direction

    public bool equipped = false;    

    SpriteRenderer sr;
    public string item = null;

    private void OnDrawGizmosSelected()
    {
        // Approach 1: Draw offsets as spheres
        Gizmos.color = Color.green;
        switch (currentDirection)
        {
            case Direction.Down:
                Gizmos.DrawWireSphere(transform.position + (Vector3)offsetDown, 0.1f);
                break;
            case Direction.Up:
                Gizmos.DrawWireSphere(transform.position + (Vector3)offsetUp, 0.1f);
                break;
            case Direction.Left:
                Gizmos.DrawWireSphere(transform.position + (Vector3)offsetLeft, 0.1f);
                break;
            case Direction.Right:
                Gizmos.DrawWireSphere(transform.position + (Vector3)offsetRight, 0.1f);
                break;
        }        
    }

    private void Awake()
    {
        if(equippedItem == null)
        {
            equippedItem = new GameObject("Held_Item").transform;
            equippedItem.gameObject.AddComponent<SpriteRenderer>();
            equippedItem.parent = transform;
            equippedItem.position = transform.position;
            
        }
        sr = equippedItem.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateEquipped(equipped);
        SwapEquippedItem(item);
    }

    public void SwapEquippedItem(string itemID)
    {
        if(string.IsNullOrEmpty(itemID) || !itemDB.GetItemByID(itemID))
        {
            item = null;
        }
        else
        {
            item = itemID;
            sr.sprite = ItemDatabase.Instance.GetItemByID(item).sprite;
            UpdateHoldPosition(currentDirection); // Reposition based on current direction
        }
        equippedItem.gameObject.SetActive(equipped && !string.IsNullOrEmpty(item));
    }

#if UNITY_EDITOR

    [Button("Button_UpdateEquipped")]
    public bool button_UpdateEquipped;
    public void Button_UpdateEquipped()
    {
        UpdateEquipped(!equipped);
        UpdateHoldPosition(currentDirection);
    }

#endif

    public void UpdateEquipped(bool isEquipped)
    {
        equipped = isEquipped;
        equippedItem.gameObject.SetActive(equipped && !string.IsNullOrEmpty(item));
    }

    // Call this when direction changes (e.g., from input or animator)
    public void UpdateHoldPosition(Direction newDirection)
    {
        currentDirection = newDirection;
        Vector2 offset = GetOffsetForDirection();

        equippedItem.localPosition = new Vector3(offset.x, offset.y, equippedItem.localPosition.z );

        // Handle flipping for left/right if your character doesn't auto-flip the item
        if (currentDirection == Direction.Left)
        {
            equippedItem.localScale = new Vector2(-1f, 1f); // Flip item sprite horizontally
        }
        else if (currentDirection == Direction.Right)
        {
            equippedItem.localScale = new Vector2(1f, 1f); // Reset flip
        }
    }

    private Vector2 GetOffsetForDirection()
    {
        switch (currentDirection)
        {
            case Direction.Down: return offsetDown;
            case Direction.Up: return offsetUp;
            case Direction.Left: return offsetLeft;
            case Direction.Right: return offsetRight;
            default: return Vector2.zero;
        }
    }

    public Vector2 WorldHoldPosition()
    {
        return transform.position + GetOffsetForDirection().ToVector3();
    }
}

