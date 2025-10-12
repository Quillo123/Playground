using System.Globalization;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ItemHolder))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5;
    public float dash = 10;
    public float acceleration = 10;
    public float deceleration = 1;
    public float rotateSpeed = 10;

    [Range(0, 100)]
    public float knockbackResistance = 10;

    public bool forwardMovement = false;
    public bool holdFire = false;

    public Sprite forwardSprite; // Sprite for facing downwards (forward)
    public Sprite leftSprite;    // Sprite for facing left (flip for right)
    public Sprite backSprite;    // Sprite for facing upwards (backwards)

    private ItemHolder itemHolder;

    public Vector2 Velocity { get { return rb.linearVelocity; } }

    // Cache 
    // Shooter shooter;

    private Rigidbody2D rb;
    public SpriteRenderer sr;
    private Vector2 movementInput;
    private bool dashInput;
    private Vector2 facingDir = Vector2.down; // Default facing forward (down)

    public Inventory inventory;
    private int i_item = 0;
    public bool equipped = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (sr == null)
        {
            Logger.LogError("SpriteRenderer component is missing on the player GameObject.", gameObject);
        }

        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Optionally add a collider if none exists (e.g., for basic collision detection)
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        itemHolder = GetComponent<ItemHolder>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            itemHolder.UpdateEquipped(true);
            itemHolder.equippedItem.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 20) * 20);
            movementInput = Vector2.zero;
        }
        else
        {
            itemHolder.UpdateEquipped(equipped);
            itemHolder.equippedItem.rotation = Quaternion.identity;
            movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        
        
        if (Input.GetMouseButtonDown(1))
        {
            equipped = !equipped;
        }

        // Swap items using scroll wheel
        if(Input.mouseScrollDelta.y != 0 && inventory.items.Length > 0)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                i_item--;
                if (i_item < 0)
                {
                    i_item = inventory.items.Length - 1;
                }
            }
            else
            {
                i_item++;
                if (i_item == inventory.items.Length)
                {
                    i_item = 0;
                }
            }
            SwapHeldItem(inventory.items[i_item].itemID);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            dashInput = true;
        }

        // Update facing direction based on movement input
        if (movementInput.sqrMagnitude > 0.01f) // Small threshold to avoid noise
        {
            facingDir = movementInput.normalized;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(GameController.Instance.ground.PlaceItemOnGround(itemHolder.item, pos))
            {
                //inventory.RemoveItem()
            }
        }


        UpdateSpriteFacing();
        // Rotate() is removed to avoid continuous rotation; sprite handling replaces it
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = Vector2.zero;
        float gain = deceleration;

        if (movementInput.sqrMagnitude > 0f)
        {
            if (forwardMovement)
            {
                targetVelocity = transform.up * movementInput.y + transform.right * movementInput.x;
            }
            else
            {
                targetVelocity = movementInput;
            }

            targetVelocity *= speed;
            gain = acceleration;
        }

        Vector2 force = (targetVelocity - rb.linearVelocity) * gain * rb.mass;
        rb.AddForce(force);

        if (dashInput)
        {
            rb.AddForce(facingDir * dash * speed, ForceMode2D.Impulse);
            dashInput = false;
        }
    }

    private void UpdateSpriteFacing()
    {
        if (sr == null) return;

        if (Mathf.Abs(facingDir.x) > Mathf.Abs(facingDir.y))
        {
            // Horizontal facing (left or right)
            sr.sprite = leftSprite;
            sr.flipX = facingDir.x > 0; // Flip for right if x > 0 (assuming leftSprite faces left)
            
            if(sr.flipX) itemHolder.UpdateHoldPosition(Direction.Right);
            else itemHolder.UpdateHoldPosition(Direction.Left);
        }
        else
        {
            // Vertical facing (up or down)
            if (facingDir.y > 0)
            {
                sr.sprite = backSprite;
                sr.flipX = false;
            }
            else
            {
                sr.sprite = forwardSprite;
                sr.flipX = false;
            }
        }
    }


    private void SwapHeldItem(string itemID)
    {

        itemHolder.SwapEquippedItem(itemID);
    }


    // Removed as sprite facing replaces mouse-based rotation
    // public void Rotate()
    // {
    //     var dir = (Camera.main.ScreenToViewportPoint(Input.mousePosition) - new Vector3(0.5f, 0.5f, 0)).ToVector2();
    //     var rotation = dir.LookDirection2D();
    //     transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
    // }
}
