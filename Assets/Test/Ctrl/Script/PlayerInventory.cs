using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private int width = 9;
    private int height = 1;

    bool showInventory = true;
    public Block[,] inventoryItem;
    public int[,] inventoryNum;
    public static int blockMaxStack = 64;

    public Texture2D texSlot;
    public Texture2D texDirt;
    public Texture2D texCursor;
    public float cursorScale = 0.5f;
    public float itemOffset = 5;

    private void Start()
    {
        inventoryItem = new Block[width, height];
        inventoryNum = new int[width, height];

        BlockMap.GetBlock("Dirt").SetTexture(texDirt);
        AddItem(BlockMap.GetBlock("Dirt"), 67);
        AddItem(BlockMap.GetBlock("Dirt"), 5);
        AddItem(BlockMap.GetBlock("Dirt"), 5);
        AddItem(BlockMap.GetBlock("Dirt"), 111);
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
        GUI.DrawTexture(new Rect(Screen.width / 2.0f, Screen.height / 2.0f, 
            texCursor.width * cursorScale, texCursor.height * cursorScale), texCursor);

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
                // 绘制格子
                Rect slotPos = new Rect(offset.x + texSlot.width * x,
                    offset.y + texSlot.height * y, texSlot.width, texSlot.height);

                GUI.color = _selectedIndex == x ? Color.gray : Color.white;
                GUI.DrawTexture(slotPos, texSlot);
                GUI.color = Color.white;

                Block block = inventoryItem[x, y];
                int num = inventoryNum[x, y];
                if (block != null)
                {
                    Rect blokPos = new Rect(slotPos.x + itemOffset / 2, slotPos.y + itemOffset / 2,
                        slotPos.width - itemOffset, slotPos.height - itemOffset);
                    GUI.DrawTexture(blokPos, block.texItem);
                    GUI.Label(slotPos, num.ToString());

                    if (slotPos.Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown
                        && Event.current.button == 0 && _draggingItem == null)
                    {
                        DragItem(x, y);
                        break;
                    }
                }

                // 鼠标左键
                if (slotPos.Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown
                    && Event.current.button == 0 && _draggingItem != null)
                {
                    MoveItem(x, y);
                    break;
                }

                // 鼠标右键
                if (slotPos.Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown
                  && Event.current.button == 1)
                {
                    SplitItem(x, y);
                    break;
                }
            }
        }

        SelectedItem();
        DraggingItem();
    }

    private Block _draggingItem;
    private int _draggingNum;
    private void DragItem(int x, int y)
    {
        if (_draggingItem != null)
            return;

        _draggingItem = inventoryItem[x, y];
        _draggingNum = inventoryNum[x, y];
        inventoryItem[x, y] = null;
        inventoryNum[x, y] = 0;
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

        if (inventoryItem[x, y] == null)
        {
            inventoryItem[x, y] = _draggingItem;
            inventoryNum[x, y] = _draggingNum;

            _draggingItem = null;
            _draggingNum = 0;
        }
        else if (inventoryItem[x, y] == _draggingItem)
        {
            if (inventoryNum[x, y] + _draggingNum > blockMaxStack)
            {
                inventoryNum[x, y] = blockMaxStack;
                _draggingNum = inventoryNum[x, y] + _draggingNum - blockMaxStack;
            }
            else
            {
                inventoryNum[x, y] += _draggingNum;
                _draggingItem = null;
                _draggingNum = 0;
            }
        }
    }

    private void SplitItem(int x, int y)
    {
        Block block = inventoryItem[x, y];
        int blockNum = inventoryNum[x, y];

        if (_draggingItem != null && inventoryItem[x, y] == _draggingItem)
        {
            //如果我们已经选择了Block，那么 我们能给我们Block类型相同Item进行增加操作
            if (inventoryNum[x, y] + 1 > blockMaxStack)
            {

            }
            else
            {
                inventoryNum[x, y]++;
                _draggingNum--;
            }
        }
        else if (_draggingItem == null)
        {
            if (inventoryNum[x, y] / 2 < 0)
                return;
            _draggingItem = block;
            _draggingNum = blockNum / 2;
            inventoryNum[x, y] -= _draggingNum;
        }
        else if (_draggingItem != null && inventoryItem[x, y] == null)
        {
            inventoryItem[x, y] = _draggingItem;
            inventoryNum[x, y]++;
            _draggingNum--;
        }
    }

    public void AddItem(Block block, int num)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (inventoryItem[x, y] == null)
                {
                    inventoryItem[x, y] = block;
                    if (num > blockMaxStack)
                    {
                        inventoryNum[x, y] = blockMaxStack;
                        AddItem(block, num - blockMaxStack);
                    }
                    else
                    {
                        inventoryNum[x, y] = num;
                    }
                    return;
                }
                else if (inventoryItem[x, y] == block && inventoryNum[x, y] < blockMaxStack)
                {
                    if (num > blockMaxStack)
                    {
                        inventoryItem[x, y] = block;
                        inventoryNum[x, y] = blockMaxStack;
                        AddItem(block, inventoryNum[x, y] + num - blockMaxStack);
                        return;
                    }
                    else
                    {
                        inventoryItem[x, y] = block;
                        if (inventoryNum[x, y] + num > blockMaxStack)
                        {
                            inventoryNum[x, y] = blockMaxStack;
                            AddItem(block, inventoryNum[x, y] + num - blockMaxStack);
                        }
                        else
                        {
                            inventoryNum[x, y] += num;
                        }
                        return;
                    }
                }
            }
        }
    }

    private int _selectedIndex;
    private void SelectedItem()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            int keyCode = Event.current.keyCode - KeyCode.Alpha1;
            if (keyCode >= 0 && keyCode <= 9)
            {
                _selectedIndex = keyCode;
            }
        }
    }
}
