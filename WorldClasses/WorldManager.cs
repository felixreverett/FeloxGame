﻿using FeloxGame.Core.Management;

namespace FeloxGame.WorldClasses
{
    public class WorldManager
    {
        private static WorldManager _instance = null;
        private static readonly object _loc = new();
                
        public static WorldManager Instance
        {
            get
            {
                lock (_loc)
                {
                    if (_instance == null)
                    {
                        _instance = new WorldManager();
                    }
                    return _instance;
                }
            }
        }

        public TexCoords GetSubTextureCoordinates(int textureIndex)
        {
            TexCoords texCoords = new TexCoords();
            int superTexSize = 1024; // texture atlas dimensions
            int padding = 1; // padding between textures
            float normalPadding = (float)padding / superTexSize;
            int rowColLength = 1024 / (32 + padding); // how many textures per row/col
            float subTexSize = 32f / superTexSize;
            float pixelOffset = 1f / (1024 * 2);

            int col = textureIndex % rowColLength;
            texCoords.MinX = col * (subTexSize + normalPadding) + pixelOffset; // normalise it

            int row = textureIndex / rowColLength;
            texCoords.MinY = 1.0f - ((row + 1) * (subTexSize + normalPadding)) + pixelOffset; // normalise, offset, and "flip" it

            texCoords.MaxX = texCoords.MinX + subTexSize - (2 * pixelOffset);
            texCoords.MaxY = texCoords.MinY + subTexSize - (2 * pixelOffset);

            return texCoords;
        }

        public TexCoords GetTexCoordFromAtlas(float x, float y, float textureWidth, float textureHeight, float atlasWidth, float atlasHeight)
        {
            /*if (x < 0 || x > textureWidth || y < 0 || y > textureHeight)
            {
                throw new Exception("Values outside of acceptable range.");
            }*/
            TexCoords texCoords = new TexCoords();
            texCoords.MinX = x / atlasWidth;
            texCoords.MinY = y / atlasHeight;
            texCoords.MaxX = (x + textureWidth) / atlasWidth;
            texCoords.MaxY = (y + textureHeight) / atlasHeight;

            return texCoords;
        }


        // Old stuff I don't yet have the heart to delete

        public TexCoords GetSubTextureCoordinatesOLD(int textureIndex)
        {
            TexCoords texCoords = new TexCoords();
            int superTexSize = 1024;
            float subTexSize = 32f / superTexSize;

            int col = textureIndex % 32;
            texCoords.MinX = col * subTexSize; // normalise it
            int row = textureIndex / 32;
            texCoords.MinY = 1.0f - ((row + 1) * subTexSize); // normalise, offset, flip

            texCoords.MaxX = texCoords.MinX + subTexSize;
            texCoords.MaxY = texCoords.MinY + subTexSize;
            return texCoords;
        }
    }
}
