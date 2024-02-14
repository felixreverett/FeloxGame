﻿using FeloxGame.Rendering;
using FeloxGame.InventoryClasses;
using OpenTK.Windowing.Common;

namespace FeloxGame.GUI
{
    public class HotbarUI : UI
    {
        // fields
        private int _rows;
        private int _cols;
        private float _itemSlotHeight;
        private float _itemSlotWidth;
        private float _edgePadding;
        private float _itemSlotPadding;
        Inventory Inventory;

        private int _activeHotbarSlot = 0;

        /// <summary>
        /// Creates a HotbarUI class to display an associated inventory
        /// </summary>
        /// <param name="koWidth">The total width of the UI element</param>
        /// <param name="koHeight">The total height of the UI element</param>
        /// <param name="anchor">The eAnchor type</param>
        /// <param name="scale">The scale of the UI element</param>
        /// <param name="isDrawable">Whether the UI element will use a texture atlas</param>
        /// <param name="toggleDraw">Whether the UI element is currently drawn</param>
        /// <param name="isClickable">Whether the UI element subscribes to Mouse Click events</param>
        /// <param name="rows">The number of inventory rows</param>
        /// <param name="cols">the number of inventory columns</param>
        /// <param name="itemSlotHeight">The height of each item slot</param>
        /// <param name="itemSlotWidth">The width of each item slot</param>
        /// <param name="edgePadding">The amount of padding between the edge of the itemslots and the edge of the UI element</param>
        /// <param name="itemSlotPadding">The amount of padding between item slots</param>
        /// <param name="inventory">The associated inventory of the UI element</param>
        public HotbarUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            int rows, int cols, float itemSlotHeight, float itemSlotWidth, float edgePadding, float itemSlotPadding, Inventory inventory
        )
            : base(koWidth, koHeight, anchor, scale, isDrawable, toggleDraw, isClickable)
        {
            this._rows = rows;
            this._cols = cols;
            this._itemSlotHeight = itemSlotHeight;
            this._itemSlotWidth = itemSlotWidth;
            this._edgePadding = edgePadding;
            this._itemSlotPadding = itemSlotPadding;
            this.Inventory = inventory;
            GenerateKodomo();
        }

        public void GenerateKodomo()
        {
            // Set the width and height of a UI Slot
            float availableWidth = this.KoWidth - 2 * _edgePadding;
            float availableHeight = this.KoHeight - 2 * _edgePadding;
            
            int slotIndex = 0;

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    RPC koPosition = new();

                    koPosition.MinX = _edgePadding + col * (_itemSlotWidth + _itemSlotPadding);

                    if (row == 0)
                    {
                        koPosition.MinY = _edgePadding;
                    }
                    else
                    {
                        koPosition.MinY = KoHeight - _edgePadding - (row * _itemSlotHeight) - (row - 1) * _itemSlotPadding;
                    }

                    koPosition.MaxX = koPosition.MinX + _itemSlotWidth;
                    koPosition.MaxY = koPosition.MinY + _itemSlotHeight;

                    Kodomo.Add($"{slotIndex}", new ItemSlotUI(_itemSlotWidth, _itemSlotHeight, eAnchor.None, 1f, true, false, false, slotIndex, Inventory, koPosition));

                    slotIndex++;
                }
            }

            // Add "activeHotbarSlot" kodomo
            //Kodomo.Add("ActiveHotbarSlot", new ActiveHotbarSlotUI(_itemSlotHeight + 2f, _itemSlotWidth + 2f, eAnchor.None, 1f, true, true, false, 0));
        }

        public void SubscribeToInventory(Inventory inventory)
        {
            inventory.InventoryChanged += HandleInventoryChanged;
        }

        public override void OnMouseWheel(MouseWheelEventArgs e)
        {
            Console.WriteLine("Reached the hotbar UI");

            base.OnMouseWheel(e);
        }

        private void HandleInventoryChanged(ItemStack[] itemStackList, ItemStack mouseItemStack)
        {
            for (int i = 0; i < _rows * _cols; i++)
            {
                if (itemStackList[i] != null)
                {
                    Kodomo[$"{i}"].ToggleDraw = true;
                    ((ItemSlotUI)Kodomo[$"{i}"]).UpdateItem(itemStackList[i]);
                }
                else
                {
                    Kodomo[$"{i}"].ToggleDraw = false;
                }
            }
        }
    }
}
