using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Texture2D texSlot;
    public int width = 9;
    public int height = 5;

    bool showInventory = true;
    public Block[,] InventoryItem;
    public int[,] InventoryNum;

    public Texture2D texDirt;
    public float itemOffset = 5;

    private void Start()
    {
        InventoryItem = new Block[width, height];
        InventoryNum = new int[width, height];

        BlockMap.GetBlock("Dirt").SetTexture(texDirt);

        InventoryItem[0, 0] = BlockMap.GetBlock("Dirt");
        InventoryNum[0, 0] = 64;

        InventoryItem[5, 1] = BlockMap.GetBlock("Dirt");
        InventoryNum[5, 1] = 32;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            showInventory = !showInventory;
        }
    }

    private void OnGUI()
    {
        if (!showInventory)
            return;

        int inventoryWidth = width * texSlot.width;
        int inventoryHeight = height * texSlot.height;

        Rect offset = new Rect((Screen.width / 2 - inventoryWidth / 2),
            (Screen.height - inventoryHeight), inventoryWidth, inventoryHeight);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Rect slotPos = new Rect(offset.x + texSlot.width * x, 
                    offset.y + texSlot.height * y, texSlot.width, texSlot.height);

                GUI.DrawTexture(slotPos, texSlot);

                Block block = InventoryItem[x, y];
                int num = InventoryNum[x, y];
                if (block != null)
                {
                    Rect blokPos = new Rect(slotPos.x + itemOffset/2, slotPos.y + itemOffset/2,
                        slotPos.width - itemOffset, slotPos.height - itemOffset);
                    GUI.DrawTexture(blokPos, block.texItem);
                    GUI.Label(slotPos, num.ToString());

                    if (slotPos.Contains(Event.current.mousePosition) &&
                        Event.current.type == EventType.mouseDown && Event.current.button == 0)
                    {
                        DragItem(x, y);
                    }
                }
                else
                {
                    if (slotPos.Contains(Event.current.mousePosition) &&
                        Event.current.type == EventType.mouseDown && Event.current.button == 0)
                    {
                        MoveItem(x, y);
                    }
                }
            }
        }

        DraggingItem();
    }

    private Block _draggingItem;
    private int _draggingNum;
    private void DragItem(int x, int y)
    {
        if (_draggingItem != null)
            return;

        _draggingItem = InventoryItem[x, y];
        _draggingNum = InventoryNum[x, y];
        InventoryItem[x, y] = null;
        InventoryNum[x, y] = 0;
    }

    private void DraggingItem()
    {
        if (_draggingItem == null)
            return;

        Event e = Event.current;
        GUI.DrawTexture(new Rect(e.mousePosition.x, e.mousePosition.y,
            texSlot.width - itemOffset, texSlot.height - itemOffset), _draggingItem.texItem);
        GUI.Label(new Rect(e.mousePosition.x, e.mousePosition.y, 
            texSlot.width - itemOffset, texSlot.height - itemOffset), _draggingNum.ToString());
    }

    private void MoveItem(int x, int y)
    {
        if (_draggingItem == null)
            return;

        InventoryItem[x, y] = _draggingItem;
        InventoryNum[x, y] = _draggingNum;

        _draggingItem = null;
        _draggingNum = 0;
    }
}
