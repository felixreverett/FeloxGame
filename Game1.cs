﻿using FeloxGame.Core.Rendering;
using FeloxGame.Core.Management;
using FeloxGame.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using FeloxGame.WorldClasses;
using OpenTK.Input;
using OpenTK.Windowing.Common.Input;

namespace FeloxGame
{
    public class Game1 : GameWindow
    {
        public Game1(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title, NumberOfSamples = 24 })
        {
        }
        
        private Shader _shader;
        
        // Camera
        float speed = 5.5f;
        private Camera _camera;

        // world data
        private World _world;
        private readonly string tileListFolderPath = @"../../../Resources/Tiles";
        private List<Tile> _tileList; // will contain all tiles

        // player data
        private Player _player;

        // Cursor data
        private GameCursor _cursor;

        protected override void OnLoad()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _shader = new(Shader.ParseShader(@"../../../Resources/Shaders/TextureWithColorAndTextureSlotAndUniforms.glsl"));
            if (!_shader.CompileShader())
            {
                Console.WriteLine("Failed to compile shader.");
                return;
            }

            // World
            _world = new World();

            // Player
            _player = new Player(new Vector2(0, 0), new Vector2(1, 2));

            _shader.Use();

            var textureSampleUniformLocation = _shader.GetUniformLocation("u_Texture[0]"); // ??
            int[] samplers = new int[2] { 0, 1 }; // ??
            GL.Uniform1(textureSampleUniformLocation, 2, samplers); // ??

            // Camera
            _camera = new Camera(Vector3.UnitZ * 10, Size.X / (float)Size.Y);

            //GameCursor
            _cursor = new GameCursor();

            // Resource loading
            _tileList = Loading.LoadAllObjects<Tile>(tileListFolderPath);
        }
        
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (!IsFocused) // check to see if the window is focused
            {
                return;
            }

            _player.Update(args);

            _world.Update(_player);

            // Keyboard movement
            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (input.IsKeyDown(Keys.A) | input.IsKeyDown(Keys.Left))
            {
                _player.Position -= new Vector2(speed * (float)args.Time, 0);
            }

            if (input.IsKeyDown(Keys.D) | input.IsKeyDown(Keys.Right))
            {
                _player.Position += new Vector2(speed * (float)args.Time, 0);
            }

            if (input.IsKeyDown(Keys.W) | input.IsKeyDown(Keys.Up))
            {
                _player.Position += new Vector2(0, speed * (float)args.Time);
            }

            if (input.IsKeyDown(Keys.S) | input.IsKeyDown(Keys.Down))
            {
                _player.Position -= new Vector2(0, speed * (float)args.Time);
            }

            // Track player with camera
            Vector3 cameraMoveDirection = new Vector3(_player.Position.X - _camera.Position.X, _player.Position.Y - _camera.Position.Y, 0f);
            _camera.Position += cameraMoveDirection * 0.05f;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            // matrices for camera
            var model = Matrix4.Identity;

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _world.Draw(_tileList);

            _player.Draw();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            _camera.AspectRatio = (float)e.Width / e.Height;
            _camera.UpdateCameraDimensions();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            float ndcX = (2.0f * MousePosition.X) / Size.X - 1.0f;
            float ndcY = 1.0f - (2.0f * MousePosition.Y) / Size.Y;
            Vector2 ndcCursorPos = new Vector2(ndcX, ndcY);

            _cursor.Position = (
                _camera.Position.X + (ndcCursorPos.X * _camera.Width / 2.0f),
                _camera.Position.Y + (ndcCursorPos.Y * _camera.Height / 2.0f));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Console.WriteLine($"{_cursor.Position.X} => {_cursor.Rounded(_cursor.Position.X)}, {_cursor.Position.Y} => {_cursor.Rounded(_cursor.Position.Y)}");
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

    }
}
