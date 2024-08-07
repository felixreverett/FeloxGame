﻿using FeloxGame.Entities;
using FeloxGame.Inventories;

namespace FeloxGame
{
    // Inventory items will only be accessed through methods like .Add() and .Remove()
    public class Inventory
    {
        public ItemStack[] _itemStackList;
        public ItemStack _mouseSlotItemStack;
        public PlayerEntity OwnerPlayer { get; set; }
        private int _rows;
        private int _cols;

        public Inventory(int rows, int cols, PlayerEntity ownerPlayer)
        {
            this._rows = rows;
            this._cols = cols;
            this._itemStackList = new ItemStack[rows * cols];
            OwnerPlayer = ownerPlayer;
        }

        public event Action<ItemStack[], ItemStack> InventoryChanged;

        public void Add(ItemStack itemStack)
        {
            var matchingItemStack = _itemStackList.FirstOrDefault(i => i is not null && i.ItemName == itemStack.ItemName);
            
            if (matchingItemStack is null)
            {
                if (FirstFreeIndex(out var index))
                {
                    _itemStackList[index] = itemStack;
                }
            }
            else
            {
                matchingItemStack.Amount += itemStack.Amount;
            }

            InventoryChanged?.Invoke(_itemStackList, _mouseSlotItemStack);
        }

        public void Remove(ItemStack itemStack)
        {
            var matchingItemStack = _itemStackList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);

            if ( matchingItemStack is null)
            {
                throw new ArgumentException("Error. Attempted to remove an item not in the inventory");
            }
            else if (matchingItemStack.Amount > itemStack.Amount)
            {
                matchingItemStack.Amount -= itemStack.Amount;
            }
            else if (matchingItemStack.Amount == itemStack.Amount)
            {
                int index = Array.IndexOf(_itemStackList, matchingItemStack);
                _itemStackList[index] = null;
            }
            else if (matchingItemStack.Amount < itemStack.Amount)
            {
                throw new ArgumentException("Error. Tried to remove more items than in inventory");
            }

            InventoryChanged?.Invoke(_itemStackList, _mouseSlotItemStack);
        }

        public void AddToSlotIndex(ItemStack itemStack, int slotIndex)
        {
            if (_itemStackList[slotIndex] is null)
            {
                _itemStackList[slotIndex] = itemStack;
            }

            InventoryChanged?.Invoke(_itemStackList, _mouseSlotItemStack);
        }

        public bool FirstFreeIndex(out int index)
        {
            index = -1;
            for (int i = 0; i < _itemStackList.Length; i++)
            {
                if (_itemStackList[i] is null)
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }

        public void Sort(eSortType sortType = eSortType.Alphabetical)
        {
            ItemStack[] sortedItems;
            switch (sortType)
            {
                case eSortType.Alphabetical:
                    sortedItems = _itemStackList.OrderBy(i => i.ItemName).ToArray();
                    _itemStackList = sortedItems;
                    break;
                case eSortType.Amount:
                    sortedItems = _itemStackList.OrderBy(i => i.Amount).ThenBy(i => i.ItemName).ToArray();
                    break;
                case eSortType.Category:
                    // not yet implemented
                    break;
            }

            InventoryChanged?.Invoke(_itemStackList, _mouseSlotItemStack);
        }

        // Todo: improve this method to solve the identified issues
        public void SwapSlots(int slotIndex)
        {
            if (_itemStackList[slotIndex] is not null)
            {
                // if both slots have itemstacks
                if (_mouseSlotItemStack is not null)
                {
                    // if items are the same, merge them to slot
                    if (_itemStackList[slotIndex].ItemName == _mouseSlotItemStack.ItemName)
                    {
                        // This will not work with stack limits.
                        _itemStackList[slotIndex].Amount += _mouseSlotItemStack.Amount;
                        _mouseSlotItemStack = null;
                    }
                    // if items are not the same, swap them
                    else
                    {
                        ItemStack temporaryItemStack = _mouseSlotItemStack;
                        _mouseSlotItemStack = _itemStackList[slotIndex];
                        _itemStackList[slotIndex] = temporaryItemStack;
                    }
                }
                // if only the itemslot has an itemstack
                else
                {
                    _mouseSlotItemStack = _itemStackList[slotIndex];
                    _itemStackList[slotIndex] = null;
                }
            }
            // if only the mouseSlot has an itemstack
            else if (_mouseSlotItemStack is not null)
            {
                _itemStackList[slotIndex] = _mouseSlotItemStack;
                _mouseSlotItemStack = null;
            }

            InventoryChanged?.Invoke(_itemStackList, _mouseSlotItemStack);
        }

        public void OnItemSlotLeftClick(int slotIndex)
        {
            Console.WriteLine($"Slot {slotIndex} was left clicked!");
            SwapSlots(slotIndex);
        }

        public void OnExternalClick()
        {
            if (_mouseSlotItemStack != null)
            {
                ItemStack itemStack = _mouseSlotItemStack;
                _mouseSlotItemStack = null;
                // Add a new item entity at the player's location
                OwnerPlayer.CurrentWorld.AddEntityToWorld(new ItemEntity(eEntityType.ItemEntity, OwnerPlayer.Position, itemStack.ItemName, itemStack.Amount));
                
                InventoryChanged?.Invoke(_itemStackList, _mouseSlotItemStack);
            }
        }

        public void DropItemAtIndex(int index)
        {
            if (_itemStackList[index] != null)
            {
            ItemStack itemStack = _itemStackList[index];
            _itemStackList[index] = null;
            OwnerPlayer.CurrentWorld.AddEntityToWorld(new ItemEntity(eEntityType.ItemEntity, OwnerPlayer.Position, itemStack.ItemName, itemStack.Amount));

            InventoryChanged?.Invoke(_itemStackList, _mouseSlotItemStack);
            }
        }

        public void DropItemAtMouseSlot()
        {
            if (_mouseSlotItemStack != null)
            {
                ItemStack itemStack = _mouseSlotItemStack;
                _mouseSlotItemStack = null;

                OwnerPlayer.CurrentWorld.AddEntityToWorld(new ItemEntity(eEntityType.ItemEntity, OwnerPlayer.Position, itemStack.ItemName, itemStack.Amount));

                InventoryChanged?.Invoke(_itemStackList, _mouseSlotItemStack);
            }
        }
    }
}
