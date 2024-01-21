using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;

public class UIInventorySlot : MonoBehaviour
{
    public int InventoryID { get; set; } = -1;

    [SerializeField]
    Image iconItem;
    [SerializeField]
    Sprite defaultSprite;
    public Text ItemCount;
    Item m_item;
    CharacterData m_CharacterData;

    public Item item
    {
        get { return m_item; }
        set
        {
            m_item = value;
            iconItem.sprite = m_item ? m_item.ItemSprite : defaultSprite;
        }
    }

    public void UpdateEntry(CharacterData data)
    {
        m_CharacterData = data;
        var entry = m_CharacterData.Inventory.Entries[InventoryID];

        //gameObject.SetActive(isEnabled);
        item = entry?.Item;

        if (item && ItemCount)
        {
            //iconItem.sprite = entry.Item.ItemSprite;

            if (entry?.Count > 1)
            {
                ItemCount.text = entry.Count.ToString();
            }
            else
            {
                ItemCount.text = "";
            }
        }
    }
}
