using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace GravityMan
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        static Game tehGame;
        static Player gravman;
        static Score score;

        public enum States { START, PLAYING, PAUSED, GAME_OVER };
        States gameState, oldState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Camera2D camera;

        List<Background> backgrounds;
        Texture2D infiniteBackground, closeStarBackground, farStarBackground;

        public struct HighScoreData
        {
            public string[] PlayerName;
            public int[] Score;

            public int Count;

            public HighScoreData(int count)
            {
                PlayerName = new string[count];
                Score = new int[count];
                Count = count;
            }
        }

        //HighScoreData data;
        //NetworkSession session;
        //string playerName = "NearSingularity";
        //public string HighScoresFileName = "highscores.dat";
        
        public Game()
        {
#if XBOX
            this.Components.Add(new GamerServicesComponent(this));
#endif
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            tehGame = this;

            Components.Add(new InputComponent(this));
            Components.Add(new AudioComponent(this, "Audio"));

            MediaPlayer.Volume = .1f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Content.Load<Song>("LongestRoad"));
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
             //TODO: Add your initialization logic here
//            string fullPath = "highscores.dat";
//#if WINDOWS
//                if(!File.Exists(fullPath))
//                {
//                    data = new HighScoreData(5);
//                    data.PlayerName[0] = "Near";
//                    data.Score[0] = 9001;
//                    data.PlayerName[1] = "Singularity";
//                    data.Score[1] = 9000;
//                    data.PlayerName[2] = "Pshquit";
//                    data.Score[2] = 5000;
//                    data.PlayerName[3] = "Allabye";
//                    data.Score[3] = 3000;
//                    data.PlayerName[4] = "Loner";
//                    data.Score[4] = 1000;

//                    //SaveHighScores(data, HighScoresFileName, null);
//                }
//#elif XBOX
//            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
//            {
//                if(!iso.FileExists(fullPath))
//                {
//                    data = new HighScoreData(5);
//                    data.PlayerName[0] = "Near";
//                    data.Score[0] = 9001;
//                    data.PlayerName[1] = "Singularity";
//                    data.Score[1] = 9000;
//                    data.PlayerName[2] = "Pshquit";
//                    data.Score[2] = 5000;
//                    data.PlayerName[3] = "Allabye";
//                    data.Score[3] = 3000;
//                    data.PlayerName[4] = "Loner";
//                    data.Score[4] = 1000;

//                    //SaveHighScores(data, HighScoresFilename, device);
//                }
//            }
//#endif
            gameState = States.START;

            camera = new Camera2D(GraphicsDevice.Viewport) { Limits = null }; 

            score = new Score();

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
            // TODO: use this.Content to load your game content here

            infiniteBackground = Content.Load<Texture2D>("Background_Space");
            closeStarBackground = Content.Load<Texture2D>("Background_CloseStars");
            farStarBackground = Content.Load<Texture2D>("Background_FarStars");
            font = Content.Load<SpriteFont>("Font");

            backgrounds = new List<Background>
            {
                new Background(camera, infiniteBackground) { Parallax = new Vector2(0.1f, 0f)},
                new Background(camera, farStarBackground) { Parallax = new Vector2(0.5f, 0f)},
                new Background(camera, closeStarBackground) { Parallax = new Vector2(0.8f, 0f)}
            };

            AudioComponent.Get().LoadBank("WaveBank", "SoundBank");  
            Score.LoadContent(Content, "Font");
            Sprite.Load(Content);
            Player.LoadContent(Content);
            Heart.LoadContent(Content);
            Bullet.LoadContent(Content);
            Meteor.LoadContent(Content);
            Star.LoadContent(Content);
            Coin.LoadContent(Content);
            
            gravman = new Player();
            Meteor.SetPlayer(gravman);
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            Score.GET();
            InputComponent input = InputComponent.Get();
            AudioComponent.Get().Update(gameTime);
            
            switch (gameState)
            {
                case States.START:
                    if (input.IsButtonDown(PlayerIndex.One, Buttons.Start))
                        SetState(States.PLAYING);
                    break;
                case States.PLAYING:                 
                    if (input.IsButtonDown(PlayerIndex.One, Buttons.Start) || input.IsKeyHit(Keys.P))
                        SetState(States.PAUSED);
                    if (gravman.IsDead)
                        SetState(States.GAME_OVER);
                    UpdateAll(gameTime);
                    break;
                case States.PAUSED:
                    if (input.IsButtonDown(PlayerIndex.One, Buttons.A) || input.IsKeyHit(Keys.Enter))
                        SetState(States.PLAYING);
                    break;
                case States.GAME_OVER:
                    if (input.IsButtonDown(PlayerIndex.One, Buttons.Start) || input.IsKeyHit(Keys.Enter))
                    {
                        Restart();
                        SetState(States.START);
                    }
                    break;
            }
            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            InputComponent input = InputComponent.Get();

            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            switch (gameState)
            {       
                case States.START:
                    if (input.IsButtonDown(PlayerIndex.One, Buttons.A) || input.IsKeyHit(Keys.Enter))
                        SetState(States.PLAYING);
                    foreach (Background _background in backgrounds)
                        _background.Draw(spriteBatch);
                    spriteBatch.Begin();
                    DrawTextCenter("GravMan\n\n");
                    DrawTextCenter("A/Enter to Play");
                    spriteBatch.End();
                    break;
                case States.PLAYING:
                    if (input.IsButtonDown(PlayerIndex.One, Buttons.Start) || input.IsKeyHit(Keys.P))
                        SetState(States.PAUSED);
                    DrawAll();
                    break;
                case States.PAUSED:
                    if (input.IsButtonDown(PlayerIndex.One, Buttons.A) || input.IsKeyHit(Keys.Enter))
                        SetState(States.PLAYING);
                    DrawAll();
                    spriteBatch.Begin();
                    DrawTextCenter("Paused");
                    spriteBatch.End();
                    break;
                case States.GAME_OVER:
                    if (input.IsButtonDown(PlayerIndex.One, Buttons.A) || input.IsKeyHit(Keys.Enter))
                    {
                        Restart();
                        SetState(States.START);
                    }
                    foreach (Background _background in backgrounds)
                        _background.Draw(spriteBatch);
                    spriteBatch.Begin();
                    //spriteBatch.DrawString(font, MakeHighScore(), new Vector2(500, 250), Color.Red);
                    DrawTextCenter("Game Over! Play Again?\n\n");
                    DrawTextCenter("Press A/Enter");
                    spriteBatch.End();
                    break;
            }
            base.Draw(gameTime);
        }

        public static float GetBot()
        {
            return tehGame.GraphicsDevice.Viewport.Height;
        }
        public static float GetSide()
        {
            return tehGame.GraphicsDevice.Viewport.Width;
        }

//        public static void SaveHighScores(HighScoreData data, string filename, StorageDevice device)
//        {
//            string fullPath = "highscores.dat";
//#if WINDOWS
//            FileStream stream = File.Open(fullPath, FileMode.OpenOrCreate);
//            try
//            {
//                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
//                serializer.Serialize(stream, data);
//            }
//            finally
//            {
//                stream.Close();
//            }
//#elif XBOX
//            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
//            {
//                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fullPath, FileMode.Create, iso))
//                {
//                    XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
//                    serializer.Serialize(stream, data);
//                }
//            }
//#endif
//        }
//        public static HighScoreData LoadHighScores(string filename)
//        {
//            HighScoreData data;

//            string fullPath = "highscores.dat";
//#if WINDOWS
//            FileStream stream = File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.Read);
//            try
//            {
//                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
//                data = (HighScoreData)serializer.Deserialize(stream);
//            }
//            finally
//            {
//                stream.Close();
//            }
//            return (data);
//#elif XBOX
//            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
//            {
//                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fullPath, FileMode.Open, iso))
//                {
//                    XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
//                    data = (HighScoreData)serializer.Deserialize(stream);
//                }
//            }
//            return (data);
//#endif
//        }
        //private void SaveHighScore()
        //{
        //    HighScoreData data = LoadHighScores(HighScoresFileName);

        //    int scoreIndex = -1;
        //    for (int i = data.Count - 1; i > -1; i--)
        //    {
        //        if (Score.GET() >= data.Score[i])
        //        {
        //            scoreIndex = i;
        //        }
        //    }
        //    if (scoreIndex > -1)
        //    {
        //        for (int i = data.Count - 1; i > scoreIndex; i--)
        //        {
        //            data.PlayerName[i] = data.PlayerName[i - 1];
        //            data.Score[i] = data.Score[i - 1];
        //        }

        //        data.PlayerName[scoreIndex] = playerName;
        //        data.Score[scoreIndex] = Score.GET();

        //        SaveHighScores(data, HighScoresFileName, null);
        //    }
        //}
        //public string MakeHighScore()
        //{
        //    HighScoreData _data = LoadHighScores(HighScoresFileName);

        //    string scoreBoardString = "HighScores: \n\n";

        //    for (int i = 0; i < _data.PlayerName.Length; i++)
        //    {
        //        scoreBoardString = scoreBoardString + _data.PlayerName[i] + "-" + _data.Score[i] + "\n";
        //        if (Score.GET() > _data.Score[i])
        //        {
        //            foreach (NetworkGamer gamer in session.AllGamers)
        //            {
        //                _data.PlayerName[i] = gamer.Gamertag;
        //                _data.Score[i] = Score.GET();
        //            }
        //        }
        //    }
        //    return scoreBoardString;
        //}

        void Restart()
        {
            Meteor.Restart();
            Bullet.Restart();
            Star.Restart();
            Coin.Restart();
            score = new Score();
            gravman = new Player();
        }
        void UpdateAll(GameTime gameTime)
        {
            Meteor.UpdateAll(gameTime);
            Star.UpdateAll(gameTime);
            Bullet.UpdateAll(gameTime);
            Coin.UpdateAll(gameTime);
            gravman.Update(gameTime);
            camera.Update(gameTime, gravman);
        }
        void DrawAll()
        {
            foreach (Background _background in backgrounds)
                _background.Draw(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.get_transformation(Vector2.One));

            Bullet.DrawAll(this.spriteBatch);
            Meteor.DrawAll(this.spriteBatch);
            Star.DrawAll(this.spriteBatch);
            Coin.DrawAll(this.spriteBatch);
            gravman.Draw(this.spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin();

            Heart.DrawLives(this.spriteBatch);
            score.Draw(this.spriteBatch);

            spriteBatch.End();
        }
        void DrawTextCenter(string text)
        {
            Vector2 size = font.MeasureString(text);
            Vector2 pos = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            pos -= 0.5f * size;
            spriteBatch.DrawString(font, text, pos, Color.DarkTurquoise);
        }
        void SetState(States myState)
        {
            gameState = myState;

            switch (gameState)
            {
                case States.PAUSED:
                    gameState = States.PAUSED;
                    AudioComponent.Get().PlaySound("Pause");
                    break;
                case States.START:
                    gameState = States.START;
                    break;
                case States.PLAYING:
                    gameState = States.PLAYING;
                    break;
                case States.GAME_OVER:
                    gameState = States.GAME_OVER;
                    AudioComponent.Get().PlaySound("GameOver");
                    break;
            } 
            oldState = gameState;
            gameState = myState;
        }
    }
}
 