using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public Dictionary<Vector2Int, ItemEntity> ItemsOnGround = new Dictionary<Vector2Int, ItemEntity>();
    public float scale = 0.5f;

    public bool PlaceItemOnGround(string itemID, Vector2 pos)
    {
        Vector2Int coord = new Vector2Int(Mathf.FloorToInt(pos.x * 2), Mathf.FloorToInt(pos.y * 2));

        if (ItemsOnGround.ContainsKey(coord))
        {
            return false;
        }

        

        var itemE = Instantiate(ItemDatabase.Instance.itemPrefab, new Vector2(coord.x * .5f + 0.25f, coord.y * .5f + 0.25f), Quaternion.identity);
        itemE.item = ItemDatabase.Instance.GetItemByID(itemID);
        ItemsOnGround.Add(coord, itemE);

        var itemRB = itemE.GetComponent<Rigidbody2D>();
        itemRB.simulated = false;
        return true;
    }

}
