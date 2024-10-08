﻿using OpenTK.Mathematics;
using FeloxGame.Entities;
using System.Text.Json;

namespace FeloxGame
{
    public class PlantTileEntity : TileEntity
    {
        public int MaximumGrowthStages { get; set; } = 1; //todo: remove this and shift over to EntityStaticData
        public int GrowthStage { get; set; } = 1;

        // Initialize TileEntity from save data
        public PlantTileEntity(PlantTileEntitySaveData saveData)
            : base(saveData)
        {
        }

        // Default constructor
        public PlantTileEntity(eEntityType entityType, Vector2 position, int growthStage = 0)
            : base(entityType, position)
        {
            GrowthStage = growthStage;
        }

        // Export entity save data
        public override EntitySaveDataObject GetSaveData()
        {
            PlantTileEntitySaveData data = new
                (
                    new float[] { Position.X, Position.Y },                     // 0
                    new float[] { Size.X, Size.Y },                             // 1
                    new float[] { DrawPositionOffset.X, DrawPositionOffset.Y }, // 2
                    GrowthStage                                                 // 3
                );

            return new EntitySaveDataObject(EntityType, JsonSerializer.Serialize(data));
        }
    }
}
