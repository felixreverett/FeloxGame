﻿using FeloxGame.Utilities;
using FeloxGame.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using FeloxGame.World;

namespace FeloxGame.GUI
{
    public class UI
    {
        // Position
        public eAnchor Anchor { get; set; }
        protected float KoWidth { get; set; }
        protected float KoHeight { get; set; }
        protected float AspectRatio { get; set; }
        protected float Scale { get; set; }
        protected RPC KoPosition { get; set; }
        protected NDC KoNDCs { get; set; }

        // Kodomo
        public Dictionary<string, UI> Kodomo { get; set; }

        // Rendering
        protected PrecisionTextureAtlas InventoryAtlas { get; set; }

        protected float[] Vertices =
        {
            //Vertices          //texCoords //texColors       
             1.0f,  1.0f, 0.3f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //top right (1,1)
             1.0f, -1.0f, 0.3f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom right (1, 0)
            -1.0f, -1.0f, 0.3f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom left (0, 0)
            -1.0f,  1.0f, 0.3f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f  //top left (0, 1)
        };
        protected uint[] Indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };
        protected bool IsDrawable { get; set; }
        public bool ToggleDraw { get; set; }

        protected bool IsClickable { get; set; }

        protected VertexBuffer _vertexBuffer;
        protected VertexArray _vertexArray;
        protected IndexBuffer _indexBuffer;

        // Constructor
        public UI(float koWidth, float koHeight, eAnchor anchor, float scale, bool isDrawable = false, bool toggleDraw = true, bool isClickable = false)
        {
            this.KoWidth = koWidth;
            this.KoHeight = koHeight;
            this.AspectRatio = koWidth / koHeight;
            this.Anchor = anchor;
            this.Scale = Math.Clamp(scale, 0.0f, 1.0f);
            this.KoPosition = new();
            this.KoNDCs = new();
            this.Kodomo = new Dictionary<string, UI>();
            this.IsDrawable = isDrawable;
            this.ToggleDraw = toggleDraw;
            this.IsClickable = isClickable;
            OnLoad();
        }

        public void OnLoad()
        {
            if (IsDrawable)
            {
                if (this.InventoryAtlas is null)
                {
                    this.InventoryAtlas = (PrecisionTextureAtlas)AssetLibrary.TextureAtlasList["Inventory Atlas"];
                }

                _vertexArray = new();
                _vertexBuffer = new VertexBuffer(Vertices);

                BufferLayout layout = new();
                layout.Add<float>(3); // Positions
                layout.Add<float>(2); // Texture Coords
                layout.Add<float>(3); // Texture Color

                _vertexArray.AddBuffer(_vertexBuffer, layout);
                _indexBuffer = new IndexBuffer(Indices);
            }
        }

        public virtual void Draw()
        {
            if (IsDrawable && ToggleDraw)
            {
                InventoryAtlas.Texture.Use();

                _vertexArray.Bind();
                _vertexBuffer.Bind();
                _indexBuffer.Bind();

                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * Vertices.Length, Vertices, BufferUsageHint.DynamicDraw);
                GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements
            }
            
            if (Kodomo.Count != 0 && ToggleDraw)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.Draw();
                }
            }
        }

        public virtual void SetTextureCoords(float x, float y, float textureWidth, float textureHeight)
        {
            TexCoords inventoryCoords = InventoryAtlas.GetPrecisionAtlasCoords(x, y, textureWidth, textureHeight);

            // Set texCoords of atlas
            Vertices[3]  = inventoryCoords.MaxX; Vertices[4]  = inventoryCoords.MaxY; // (1, 1)
            Vertices[11] = inventoryCoords.MaxX; Vertices[12] = inventoryCoords.MinY; // (1, 0)
            Vertices[19] = inventoryCoords.MinX; Vertices[20] = inventoryCoords.MinY; // (0, 0)
            Vertices[27] = inventoryCoords.MinX; Vertices[28] = inventoryCoords.MaxY; // (0, 1)
        }

        public void OnResize(float oyaWidth, float oyaHeight, NDC oyaNDCs)
        {
            this.KoWidth = oyaWidth;
            this.KoHeight = oyaHeight;
            this.AspectRatio = oyaWidth / oyaHeight;
            SetNDCs(oyaWidth, oyaHeight, oyaNDCs);
        }

        public virtual void OnMouseMove(Vector2 mouseNDCs)
        {
            if (Kodomo.Count > 0)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.OnMouseMove(mouseNDCs);
                }
            }
        }

        public virtual void OnLeftClick(Vector2 mousePosition, WorldManager world)
        {
            if (!IsMouseInBounds(mousePosition))
            {
                // out of bounds functionality
            }

            else
            {
                if (Kodomo.Count > 0)
                {
                    foreach (UI ui in Kodomo.Values)
                    {
                        ui.OnLeftClick(mousePosition, world);
                    }
                }

                if (this.IsClickable)
                {
                    // functionality here
                }
            }
        }

        public virtual void OnRightClick(Vector2 mousePosition, WorldManager world)
        {
            if (!IsMouseInBounds(mousePosition))
            {
                // out of bounds functionality
            }

            else
            {
                if (Kodomo.Count > 0)
                {
                    foreach (UI ui in Kodomo.Values)
                    {
                        ui.OnRightClick(mousePosition, world);
                    }
                }

                if (this.IsClickable)
                {
                    // functionality here
                }
            }
        }

        public virtual void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (Kodomo.Count > 0)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.OnMouseWheel(e);
                }
            }
        }

        public virtual void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (Kodomo.Count > 0)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.OnKeyDown(e);
                }
            }
        }

        public bool IsMouseInBounds(Vector2 mouseNDCs)
        {
            if (mouseNDCs.X >= KoNDCs.MinX && mouseNDCs.Y >= KoNDCs.MinY && mouseNDCs.X <= KoNDCs.MaxX && mouseNDCs.Y <= KoNDCs.MaxY) { return true; }
            else { return false; }
        }

        public virtual void SetNDCs(float oyaWidth, float oyaHeight, NDC oyaNDCs)
        {
            if (Anchor != eAnchor.None)
            {
                KoPosition = GetAnchoredDimensions(oyaWidth, oyaHeight);
            }

            // map anchored coordinates
            KoNDCs.MaxX = ((KoPosition.MaxX / oyaWidth) * (oyaNDCs.MaxX - oyaNDCs.MinX) + oyaNDCs.MinX);
            KoNDCs.MinX = ((KoPosition.MinX / oyaWidth) * (oyaNDCs.MaxX - oyaNDCs.MinX) + oyaNDCs.MinX);
            KoNDCs.MaxY = ((KoPosition.MaxY / oyaHeight) * (oyaNDCs.MaxY - oyaNDCs.MinY) + oyaNDCs.MinY);
            KoNDCs.MinY = ((KoPosition.MinY / oyaHeight) * (oyaNDCs.MaxY - oyaNDCs.MinY) + oyaNDCs.MinY);

            // Set screen position
            Vertices[0]  = KoNDCs.MaxX; Vertices[1]  = KoNDCs.MaxY; // ( 1,  1)
            Vertices[8]  = KoNDCs.MaxX; Vertices[9]  = KoNDCs.MinY; // ( 1, -1)
            Vertices[16] = KoNDCs.MinX; Vertices[17] = KoNDCs.MinY; // (-1, -1)
            Vertices[24] = KoNDCs.MinX; Vertices[25] = KoNDCs.MaxY; // (-1,  1)

            if (Kodomo.Count > 0)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.SetNDCs(KoWidth, KoHeight, KoNDCs);
                }
            }
        }

        /// <summary>
        /// Returns coordinates (0,0) to (1,1) relative to the parent container.
        /// </summary>
        /// <param name="OyaWidth"></param>
        /// <param name="OyaHeight"></param>
        /// <returns></returns>
        public Vector2 GetRelativeDimensions(float OyaWidth, float OyaHeight)
        {
            Vector2 relativeDimensions = new();
            float OyaAspectRatio = OyaWidth / OyaHeight;
                        
            if (OyaAspectRatio > AspectRatio) // koHeight is constraint
            {
                relativeDimensions.Y = OyaHeight * Scale;
                relativeDimensions.X = relativeDimensions.Y * AspectRatio;
            }
            else // koWidth is constraint
            {
                relativeDimensions.X = OyaWidth * Scale;
                relativeDimensions.Y = relativeDimensions.X / AspectRatio;
            }
            
            return relativeDimensions;
        }

        /// <summary>
        /// Returns coordinates (0, 0) to (1,1) anchored within the parent container.
        /// </summary>
        /// <param name="OyaWidth"></param>
        /// <param name="OyaHeight"></param>
        /// <returns></returns>
        public RPC GetAnchoredDimensions(float OyaWidth, float OyaHeight)
        {
            Vector2 relativeDimensions = GetRelativeDimensions(OyaWidth, OyaHeight);
            RPC anchoredDimensions = new();
            switch (this.Anchor)
            {
                case eAnchor.Middle:
                    anchoredDimensions.MaxX = (OyaWidth + relativeDimensions.X) / 2f;
                    anchoredDimensions.MaxY = (OyaHeight + relativeDimensions.Y) / 2f;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.Left:
                    anchoredDimensions.MaxX = relativeDimensions.X;
                    anchoredDimensions.MaxY = (OyaHeight + relativeDimensions.Y) / 2f;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.Top:
                    anchoredDimensions.MaxX = (OyaWidth + relativeDimensions.X) / 2f;
                    anchoredDimensions.MaxY = OyaHeight;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.Right:
                    anchoredDimensions.MaxX = OyaWidth;
                    anchoredDimensions.MaxY = (OyaHeight + relativeDimensions.Y) / 2f;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.Bottom:
                    anchoredDimensions.MaxX = (OyaWidth + relativeDimensions.X) / 2f;
                    anchoredDimensions.MaxY = relativeDimensions.Y;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.TopLeft:
                    anchoredDimensions.MaxX = relativeDimensions.X;
                    anchoredDimensions.MaxY = OyaHeight;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.TopRight:
                    anchoredDimensions.MaxX = OyaWidth;
                    anchoredDimensions.MaxY = OyaHeight;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.BottomRight:
                    anchoredDimensions.MaxX = OyaWidth;
                    anchoredDimensions.MaxY = relativeDimensions.Y;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.BottomLeft:
                    anchoredDimensions.MaxX = relativeDimensions.X;
                    anchoredDimensions.MaxY = relativeDimensions.Y;
                    anchoredDimensions.MinX = 0f;
                    anchoredDimensions.MinY = 0f;
                    break;
                default:
                    break;
            }
                        
            return anchoredDimensions;
        }
    }
}
