using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace EarthDefenders
{
    public class GameRoot : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public GameRoot()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            Window.AllowUserResizing = false;
            Window.Title = "Earth Defenders";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }




    }
}


//Entity///////////////////////////////////////////////////////////////////////

class Player
{



    private bool Alive;
    private bool Shooting;
    private double Speed;



    public Player()
    {
        Alive = false;
        Shooting = true;
        Speed = 0;
    }

    public void Shoot()
    {
        if (this.Shooting)
        {
            Debug.Print("Player is already Shooting");
        }
        else
        {
            Debug.Print("Shoot");
        }
    }

}

class Debug
{
    public static void Print(string args)
    {
        System.Console.WriteLine("Debug: ", args);
    }
}
