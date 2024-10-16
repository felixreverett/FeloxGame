﻿using FeloxGame.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace FeloxGame.GUI
{
    public class MouseSlotUI : SlotUI
    {
        private Vector2 MouseNDCs { get; set; }
        protected Vector2 NDCDimensions;

        public MouseSlotUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            int itemSlotID, Inventory inventory, RPC koPosition
        )
            : base(koWidth, koHeight, anchor, scale, isDrawable, toggleDraw, isClickable,
                  itemSlotID, inventory, koPosition)
        {
            //UpdateItem(new ItemStack("Persimmon", 1));
            //ToggleDraw = true;
        }

        public override void OnMouseMove(Vector2 mouseNDCs)
        {
            MouseNDCs = mouseNDCs;
            SetNDCs();

            if (Kodomo.Count > 0)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.OnMouseMove(mouseNDCs);
                }
            }
        }

        public override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            
        }

        // This one updates the NDCs when the parent UI element calls it
        public override void SetNDCs(float oyaWidth, float oyaHeight, NDC oyaNDCs)
        {
            NDCDimensions.X = (KoWidth / oyaWidth) * (oyaNDCs.MaxX - oyaNDCs.MinX);
            NDCDimensions.Y = (KoHeight / oyaHeight) * (oyaNDCs.MaxY - oyaNDCs.MinY);
        }

        // This one updates the NDCs when the mouse moves
        protected void SetNDCs()
        {
            KoNDCs.MaxX = MouseNDCs.X + NDCDimensions.X / 2.0f;
            KoNDCs.MinX = MouseNDCs.X - NDCDimensions.X / 2.0f;
            KoNDCs.MaxY = MouseNDCs.Y + NDCDimensions.Y / 2.0f;
            KoNDCs.MinY = MouseNDCs.Y - NDCDimensions.Y / 2.0f;
        }

    }
}
