using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Sandbox
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Texture declarations
        private Texture2D background;

        private Texture2D shuttle;
        private Texture2D earth;

        private Texture2D blueTrail;
        private float angle = 0;

        private Texture2D blue;
        private Texture2D green;
        private Texture2D red;

        //Texture Atlas
        private AnimatedSprite animatedSmiley;

        //Spritefont stuff
        private SpriteFont font;

        private int score = 0;

        private int fpsCounter = 0;
        private double fpsUpdateRate = 0;
        private int fpsSum = 0;
        private int fps = 0;

        //Color Rotation Vars
        private float blueAngle = 0;

        private float greenAngle = 0;
        private float redAngle = 0;

        private float blueSpeed = 1.5f;
        private float greenSpeed = 1.02f;
        private float redSpeed = 1.32f;

        private float distance = 100;

        private RenderTarget2D renderTarget;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

//          graphics.IsFullScreen = true;
//          graphics.ApplyChanges();

            //Fixed Timestep is the default, but not preferred
            //IsFixedTimeStep = true;
            //TargetElapsedTime = TimeSpan.FromMilliseconds(20); // 20 milliseconds, or 50 FPS.

            IsFixedTimeStep = false; // Setting this to true makes it fixed time step, false is variable time step.
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = this.Content.Load<Texture2D>("sprites/stars");
            shuttle = this.Content.Load<Texture2D>("sprites/shuttle");
            earth = this.Content.Load<Texture2D>("sprites/earth");

            blueTrail = this.Content.Load<Texture2D>("sprites/trail_00");

            red = this.Content.Load<Texture2D>("sprites/red");
            green = this.Content.Load<Texture2D>("sprites/green");
            blue = this.Content.Load<Texture2D>("sprites/blue");

            Texture2D smileyWalk = this.Content.Load<Texture2D>("sprites/SmileyWalk");
            animatedSmiley = new AnimatedSprite(smileyWalk, 4, 4);

            font = this.Content.Load<SpriteFont>("fonts/Kootenay");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            score++;
            angle += 0.6f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            animatedSmiley.Update();

            blueAngle += blueSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            greenAngle += greenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            redAngle += redSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            DrawSceneToTexture(this.renderTarget);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
            SamplerState.LinearClamp, DepthStencilState.Default,
            RasterizerState.CullNone);

            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, 800, 480), Color.White);
            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, 400, 240), Color.Red);

            spriteBatch.End();

            Vector2 bluePosition = new Vector2(
                (float)Math.Cos(blueAngle) * distance,
                (float)Math.Sin(blueAngle) * distance);
            Vector2 greenPosition = new Vector2(
                (float)Math.Cos(greenAngle) * distance,
                (float)Math.Sin(greenAngle) * distance);
            Vector2 redPosition = new Vector2(
                (float)Math.Cos(redAngle) * distance,
                (float)Math.Sin(redAngle) * distance);

            Vector2 center = new Vector2(300, 140);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            spriteBatch.Draw(blue, center + bluePosition, Color.White);
            spriteBatch.Draw(green, center + greenPosition, Color.White);
            spriteBatch.Draw(red, center + redPosition, Color.White);

            spriteBatch.End();

            spriteBatch.Begin();

            fpsCounter++;
            fpsUpdateRate -= gameTime.ElapsedGameTime.TotalSeconds;
            fpsSum += (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            if (fpsUpdateRate <= 0)
            {
                fpsUpdateRate = 0.1;
                fps = fpsSum / fpsCounter;
                fpsCounter = 0;
                fpsSum = 0;
            }
            spriteBatch.DrawString(font, "FPS: " + fps, new Vector2(0, 400), Color.Red);

            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws the entire scene in the given render target.
        /// </summary>
        /// <returns>A texture2D with the scene drawn in it.</returns>
        protected void DrawSceneToTexture(RenderTarget2D renderTarget)
        {
            // Set the render target
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);
            spriteBatch.Draw(earth, new Vector2(400, 240), Color.White);
            spriteBatch.Draw(shuttle, new Vector2(450, 240), Color.White);

            animatedSmiley.Draw(spriteBatch, new Vector2(400, 200));

            Vector2 location = new Vector2(400, 240);
            Rectangle sourceRectangle = new Rectangle(0, 0, blueTrail.Width, blueTrail.Height);
            Vector2 origin = new Vector2(blueTrail.Width, blueTrail.Height / 2);

            spriteBatch.Draw(blueTrail, location, sourceRectangle, Color.White, angle, origin, 0.5f, SpriteEffects.None, 1);

            spriteBatch.DrawString(font, "Score: " + score, new Vector2(100, 10), Color.White);

            spriteBatch.End();

            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);
        }
    }
}