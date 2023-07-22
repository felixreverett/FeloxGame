﻿using FeloxGame.Core.Rendering;

namespace FeloxGame.Core.Management
{
    public class ResourceManager
    {
        private static ResourceManager _instance = null;
        private static readonly object _loc = new();
        private IDictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>(); // Dictionary for texture cache

        public static ResourceManager Instance
        {
            get
            {
                lock(_loc)
                {
                    if (_instance == null)
                    {
                        _instance = new ResourceManager();
                    }
                    return _instance;
                }
            }
        }

        public Texture2D LoadTexture(string textureName, int textureUnit)
        {
            _textureCache.TryGetValue(textureName, out var value);
            if (value is not null)
            {
                return value;
            }
            value = TextureFactory.Load(textureName, textureUnit);
            _textureCache.Add(textureName, value);
            return value;
        }
    }
}
