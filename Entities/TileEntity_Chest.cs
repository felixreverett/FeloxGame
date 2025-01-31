﻿using FeloxGame.Utilities;
using OpenTK.Mathematics;
using System.Text.Json;

namespace FeloxGame.Entities
{
    public class TileEntity_Chest : TileEntity
    {
        public Inventory Inventory { get; set; }
        public TileEntity_Chest(eEntityType entityType, Vector2 position, Vector2 drawPositionOffset)
            : base(entityType, position, drawPositionOffset)
        {
            Inventory = new Inventory(5, 10);
            SetTexCoords();
        }

        public TileEntity_Chest(TileEntity_Chest_SaveData saveData)
            : base(saveData)
        {
            Inventory = saveData.Inventory;
            SetTexCoords();
        }

        // Todo: adjust to use entityatlas
        private void SetTexCoords()
        {
            Items.Item? matchingItem = AssetLibrary.ItemList!.FirstOrDefault(i => i.ItemName == "Wood Chest")!;
            int index = matchingItem == null ? 0 : matchingItem.TextureIndex;
            Console.WriteLine($"Setting texcoords of new Chest Tile Entity from atlas index {index}");
            TexCoords = Utilities.Utilities.GetIndexedAtlasCoords(index, 16, 1024, 8);
        }

        public override EntitySaveDataObject GetSaveData()
        {
            TileEntity_Chest_SaveData data = new
                (
                    new float[] { Position.X, Position.Y },                     // 0
                    new float[] { Size.X, Size.Y },                             // 1
                    new float[] { DrawPositionOffset.X, DrawPositionOffset.Y }, // 2
                    Inventory                                                   // 3
                );

            return new EntitySaveDataObject(EntityType, JsonSerializer.Serialize(data));
        }
    }
}
