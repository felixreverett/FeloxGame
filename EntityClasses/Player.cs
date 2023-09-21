﻿using OpenTK.Mathematics;
using FeloxGame.Core.Rendering;
using RectangleF = System.Drawing.RectangleF;
using OpenTK.Windowing.Common;
using FeloxGame.EntityClasses;

namespace FeloxGame
{
    public class Player : Entity // ICollidable, 
    {
        public RectangleF ColRec
        {
            get
            {
                return new System.Drawing.RectangleF(Position.X - Size.X / 2f, Position.Y - Size.Y / 2f, Size.X, Size.Y);
            }
        } // collision rectangle
        public RectangleF DrawRec
        {
            get
            {
                RectangleF colRec = ColRec;
                colRec.X = colRec.X - 5;
                colRec.Width = colRec.Width + 10;
                return colRec;
            }
        }// draw rectangle

        // Properties
        public Inventory Inventory { get; set; }
        public float Reach { get; set; }
        public int RenderDistance { get; set; } = 2;
        public eFacing Facing { get; set; } = eFacing.South;

        public Player(Vector2 startPos, Vector2 size, string textureAtlasName, int textureUnit)
            : base (startPos, size, textureAtlasName, textureUnit)
        {
            this.Inventory = new Inventory(5, 10);
            Reach = 5f;
        }

        public void Update(FrameEventArgs args)
        {
            HandleInput(args);

            ResolveCollision();
        }

        public void HandleInput(FrameEventArgs args) 
        {
        }

        public void ResolveCollision() { }
    }
}
