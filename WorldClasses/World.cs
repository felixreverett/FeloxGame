﻿using FeloxGame.Core.Management;
using FeloxGame.Core.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpNoise;

namespace FeloxGame.WorldClasses // rename this later?
{
    /// <summary>
    /// Class to encapsulate everything a "world" would need.
    /// Currently active chunks + entities
    /// </summary>
    public class World
    {
        public Dictionary<string, Chunk> LoadedChunks { get; private set; }
        private string _worldFolderPath = @"../../../Resources/World/WorldFiles";
        public int Seed { get; private set; }
        private Noise _noise;

        // Rendering
        private readonly float[] _vertices =
        {   //Vertices        //texCoords //texColors       //texUnit
            1.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, //top right (1,1)
            1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, //bottom right (1, 0)
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, //bottom left (0, 0)
            0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f  //top left (0, 1)
        };

        private uint[] _indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };

        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;
        private Texture2D WorldTexture { get; set; }

        public World(int seed = 1)
        {
            LoadedChunks = new Dictionary<string, Chunk>();
            Seed = seed;
            _noise = new Noise(seed, 4);
            GenerateOctaveOffsets();
            OnLoad();
        }

        public void OnLoad()
        {
            WorldTexture = ResourceManager.Instance.LoadTexture(@"../../../Resources/Textures/WorldTextures.png");

            _vertexArray = new();
            _vertexBuffer = new VertexBuffer(_vertices);

            BufferLayout layout = new();
            layout.Add<float>(3); // Positions
            layout.Add<float>(2); // Texture Coords
            layout.Add<float>(3); // Texture Color
            layout.Add<float>(1); // Texture Slot

            _vertexArray.AddBuffer(_vertexBuffer, layout);
            _indexBuffer = new IndexBuffer(_indices);
        }
        
        public void Update(Player player)
        {
            // load chunks around the player
            for (int x = (int)player.Position.X / 16 - player.RenderDistance; x <= (int)player.Position.X / 16 + player.RenderDistance; x++)
            {
                for (int y = (int)player.Position.Y / 16 - player.RenderDistance; y <= (int)player.Position.Y / 16 + player.RenderDistance; y++)
                {
                    string chunkID = $"x{x}y{y}";
                    if (!LoadedChunks.ContainsKey(chunkID))
                    {
                        Chunk newChunk = LoadOrGenerateChunk($"{_worldFolderPath}/{chunkID}.txt", x, y); // load the chunk
                        LoadedChunks.Add(chunkID, newChunk);
                    }
                }
            }

            // unload chunks around the player
            foreach (Chunk chunk in LoadedChunks.Values)
            {
                if (Math.Abs(chunk.ChunkPosX - (int)player.Position.X / 16) > player.RenderDistance || Math.Abs(chunk.ChunkPosY - (int)player.Position.Y / 16) > player.RenderDistance)
                {
                    LoadedChunks.Remove($"x{chunk.ChunkPosX}y{chunk.ChunkPosY}");
                }
            }
        }

        public void Draw(List<Tile> _tileList) // todo: remove inputs?
        {
            WorldTexture.Use();
            _vertexArray.Bind();
            _vertexBuffer.Bind();
            _indexBuffer.Bind();

            foreach (Chunk loadedChunk in LoadedChunks.Values)
            {
                for (int y = 0; y < loadedChunk.Tiles.GetLength(1); y++)
                {
                    for (int x = 0; x < loadedChunk.Tiles.GetLength(0); x++)
                    {
                        _vertices[0] = loadedChunk.ChunkPosX * 16 + x + 1; _vertices[1] = loadedChunk.ChunkPosY * 16 + y + 1; // top right (1, 1)
                        _vertices[9] = loadedChunk.ChunkPosX * 16 + x + 1; _vertices[10] = loadedChunk.ChunkPosY * 16 + y; // bottom right (1, 0)
                        _vertices[18] = loadedChunk.ChunkPosX * 16 + x; _vertices[19] = loadedChunk.ChunkPosY * 16 + y; // bottom left (0, 0)
                        _vertices[27] = loadedChunk.ChunkPosX * 16 + x; _vertices[28] = loadedChunk.ChunkPosY * 16 + y + 1; // top left (0, 1)
                        string textureName = loadedChunk.Tiles[x, y];
                        int textureIndex = _tileList.Where(t => t.Name.ToLower() == textureName.ToLower()).FirstOrDefault().TextureIndex;
                        TexCoords texCoords = WorldManager.Instance.GetSubTextureCoordinates(textureIndex);
                        _vertices[3] = texCoords.MaxX; _vertices[4] = texCoords.MaxY;   // (1, 1)
                        _vertices[12] = texCoords.MaxX; _vertices[13] = texCoords.MinY; // (1, 0)
                        _vertices[21] = texCoords.MinX; _vertices[22] = texCoords.MinY; // (0, 0)
                        _vertices[30] = texCoords.MinX; _vertices[31] = texCoords.MaxY; // (0, 1)
                        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, sizeof(float) * _vertices.Length, _vertices);
                        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements
                    }
                }
            }
        }

        // Terrain Generation code
        public void GenerateOctaveOffsets(int seed = 1)
        {
            // Generate octave offsets
            int octaves = 4; // Number of octaves
            Vector2[] octaveOffsets = new Vector2[octaves];
            var random = new Random(seed);
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = random.Next(-100000, 100000);
                float offsetY = random.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }
        }

        public Chunk LoadOrGenerateChunk(string filePath, int chunkPosX, int chunkPosY)
        {
            if (File.Exists(filePath))
            {
                return LoadChunk(filePath, chunkPosX, chunkPosY);
            }
            else
            {
                return GenerateChunk(chunkPosX, chunkPosY);
            }
        }
        public Chunk LoadChunk(string filePath, int chunkPosX, int chunkPosY)
        {
            string[] rows = File.ReadAllText(filePath).Trim().Replace("\r", "").Split("\n").ToArray();
            Chunk newChunk = new(chunkPosX, chunkPosY);

            for (int y = 0; y < rows.Length; y++)
            {
                string row = rows[y];
                string[] cols = row.Split(" ");
                for (int x = 0; x < cols.Length; x++)
                {
                    newChunk.Tiles[x, y] = cols[x];
                }
            }

            return newChunk;
        }

        public Chunk GenerateChunk(int chunkPosX, int chunkPosY, int seed = 1)
        {
            //float[,] noiseMap = _noise.GenerateNoiseMap(chunkPosX, chunkPosY, 16, 16, seed, 1000, 4, 2, 3);
            //Chunk newChunk = ApplyNoiseMapToChunk(noiseMap, new Chunk(chunkPosX, chunkPosY));
            NoiseMap noiseMap2 = NoiseGenerator.GenerateNoiseMap(chunkPosX, chunkPosY, 16, 3, 100f, 9);
            Chunk newChunk = ApplyNoiseMapToChunk2(noiseMap2, new Chunk(chunkPosX, chunkPosY));

            return newChunk;
        }

        public Chunk ApplyNoiseMapToChunk(float[,] noiseMap, Chunk chunk)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    float noiseValue = noiseMap[x, y];
                    if (noiseValue < 0.4f)
                    {
                        chunk.SetTile(x, y, "Water");
                    }
                    else if (noiseValue < 0.5f)
                    {
                        chunk.SetTile(x, y, "Sand");
                    }
                    else
                    {
                        chunk.SetTile(x, y, "Grass");
                    }
                }
            }

            return chunk;
        }

        public Chunk ApplyNoiseMapToChunk2(NoiseMap noiseMap2, Chunk chunk)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    float noiseValue = noiseMap2.GetValue(x, y);
                    if (noiseValue < -0.4f)
                    {
                        chunk.SetTile(x, y, "Water");
                    }
                    else if (noiseValue < 0.0f)
                    {
                        chunk.SetTile(x, y, "Sand");
                    }
                    else
                    {
                        chunk.SetTile(x, y, "Grass");
                    }
                }
            }

            return chunk;
        }


    }
}