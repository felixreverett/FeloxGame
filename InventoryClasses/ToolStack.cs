﻿namespace FeloxGame.InventoryClasses
{
    internal class ToolStack : ItemStack
    {
        public ToolStack() : base()
        {
            
        }

        public override void Use()
        {
            // Reduce durability
            // possibly change from override to use base functionality
        }
    }
}