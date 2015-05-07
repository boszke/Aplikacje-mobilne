using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

using System.IO;
using System.IO.IsolatedStorage;

namespace BoxerRun
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player;
        Texture2D background;//tekstura t�a

        Texture2D enemyTexture;//tekstura przeciwnika
        List<Enemy> enemies;//lista przechowuj�ca przeciwnik�w

        List<Animation> explosions; //lista przechowuj�ca zabitych przeciwnik�w

        TimeSpan enemySpawnTime; //
        TimeSpan previousSpawnTime; 

        Random random;

        //Efekty dzwiekowe 
        SoundEffect explosionSound; //d�wi�k zabijanego przeciwnika

        // muzyka w tle 
        Song gameplayMusic; //muzyka w grze
        Song menuMusic; //muzyka w menu

        // tekstura menu 
        Texture2D mainMenu;

        //zmienne pomocnicze 
        bool isTitleScreenShown; //czy pokaza� ekran menu
        bool isGameScreenShown; //czy pokaza� ekran gry

        int score; //punkty zdobyte
        int last_score; //poprzednie punkty
        int best_score; //najlepszy wynik w rundzie
        SpriteFont font; //czcionka

        Animation playerAnimation; //animacja playera
        public Texture2D playerTexture; //textura playera
        public Vector2 playerPosition; //pozycja playera

        //skalowanie postaci
        int skala;

        //wyciszanie
        int mute;
        //textury wyciszania
        Texture2D unmuteTexture;
        Texture2D muteTexture;

        Texture2D playgameTexture; //przycisk play

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //dost�pne gesty w grze
            TouchPanel.EnabledGestures = GestureType.Tap;

            // Frame rate is 30 fps by default for Windows Phone. 
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock. 
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        /// <summary> 
        /// Allows the game to perform any initialization it needs to before starting to run. 
        /// This is where it can query for any required services and load any non-graphic 
        /// related content.  Calling base.Initialize will enumerate through any components 
        /// and initialize them as well. 
        /// </summary> 
        /// 
                //
               //   INITIALIZE
              //
        protected override void Initialize()
        {
            // INICJALIZACJE OBIEKT�W
            player = new Player();
            enemies = new List<Enemy>();
            explosions = new List<Animation>();
            //projectiles = new List<Projectile>();

            previousSpawnTime = TimeSpan.Zero;
            enemySpawnTime = TimeSpan.FromSeconds(1.0f); //spawn przeciwnika co 1 sekund�
            
            random = new Random();
            
            //punkty pocz�tkowe
            score = 0;
            last_score = 0;

            //przypisywanie skali
            skala = 2;
            
            //dost�pne gesty w grze
            TouchPanel.EnabledGestures = GestureType.Tap;

            //odczyt rekordu (highscore)
            ReadScore();
            //odczyt wyciszenia
            ReadMute();

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;

            base.Initialize();
        }

        /// <summary> 
        /// LoadContent will be called once per game and is the place to load 
        /// all of your content. 
        /// </summary> 
        /// 
            //
           //   LOAD CONTENT
          //
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures. 
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            //�ADOWANIE OBIEKT�W W GRZE
            
            mainMenu = Content.Load<Texture2D>("menu"); //obraz menu

            isGameScreenShown = false; //niewidoczny ekran gry
            isTitleScreenShown = true; //widoczny ekran menu

            //muzyka w tle 
            gameplayMusic = Content.Load<Song>("music/gra");
            menuMusic = Content.Load<Song>("music/menu");

            //efekty dzwiekowe 
            explosionSound = Content.Load<SoundEffect>("music/death1"); //d�wi�k zabitego przeciwnika

            //animacja playera
            playerAnimation = new Animation();
            playerTexture = Content.Load<Texture2D>("boxer");
            playerAnimation.Initialize(playerTexture, new Vector2(0, GraphicsDevice.Viewport.Height - playerAnimation.FrameHeight * skala), 56, 80, 3, 150, Color.White, 1*skala, true, 0, 0, false);
            playerPosition = new Vector2(0, GraphicsDevice.Viewport.Height - playerAnimation.FrameHeight*skala); //pozycja playera, lewy dolny r�g
            player.Initialize(playerAnimation, playerPosition);

            //�adowanie obraz�w (textur)
            enemyTexture = Content.Load<Texture2D>("gnu1");
            background = Content.Load<Texture2D>("background");
            unmuteTexture = Content.Load<Texture2D>("unmute");
            muteTexture = Content.Load<Texture2D>("mute");
            playgameTexture = Content.Load<Texture2D>("play");

            //czcionka
            font = Content.Load<SpriteFont>("gameFont");

            //odtwarzanie muzyki
            PlayMusic(menuMusic);
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
        /// 

              //
             //   UPDATE
            //
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit 
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) 
            //    this.Exit(); 
            
            //je�eli wy�wietlony jest ekran menu
            if (isTitleScreenShown)
            {
                WriteMute();
                UpdateGameScreen(); //odwo�anie do metody

                //wyj�cie z gry, je�eli odpalony jest ekran startowy
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    this.Exit();
                }
            }
            //je�eli wy�wietlony jest ekran gry
            else if (isGameScreenShown)
            {
                UpdateTitleScreen();//odwo�anie do metody

                //uaktualnienie
                UpdatePlayer(gameTime);//gracza
                UpdateEnemies(gameTime);//przeciwnik�w
                UpdateCollision();//kolizji
                UpdateExplosions(gameTime);//zabi�
            }
            base.Update(gameTime);
        }

        /// <summary> 
        /// This is called when the game should draw itself. 
        /// </summary> 
        /// <param name="gameTime">Provides a snapshot of timing values.</param> 
        /// 

                //
               //   DRAW
              //
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //rozpocz�cie rysowania
            spriteBatch.Begin();

            //wy�wietlanie albo menu albo ekranu gry
            if (isTitleScreenShown)
            {
                DrawTitleScreenShown();
            }
            else if (isGameScreenShown)
            {
                DrawGameScreen();
            }

            //koniec rysowania
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

                //
               //   UAKTUALNIANIE
              //

        //uaktualnienie ekranu gry
        private void UpdateGameScreen()
        {
            //powr�t do menu na klikni�cie X
            int x = 0;
            int y = 0;
            bool isInputPressed = false;
            var touchPanelState = TouchPanel.GetState();

            //je�eli wci�ni�to co�
            if (touchPanelState.Count >= 1)
            {
                //przypisywanie miejsca wci�ni�cia
                var touch = touchPanelState[0];
                x = (int)touch.Position.X;
                y = (int)touch.Position.Y;

                //na zwolnienie przycisku wyj�cia
                isInputPressed = touch.State == TouchLocationState.Released;
            }

            //opakowanie przycisku mute w prostok�t
            var buttonRectangle = new Rectangle(GraphicsDevice.Viewport.Width - unmuteTexture.Width-50, 0, unmuteTexture.Width, unmuteTexture.Height);
            //opakowanie przycisku play game
            var playRectangle = new Rectangle(GraphicsDevice.Viewport.Width / 2 - playgameTexture.Width / 2, GraphicsDevice.Viewport.Height / 2 - playgameTexture.Height / 2, playgameTexture.Width, playgameTexture.Height);
            //opakowanie przycisku mute w prostok�t
            var buttonDelete = new Rectangle(0, GraphicsDevice.Viewport.Height - 50, 200, 100);
            //zmiana stanu puszczania muzyki po klikni�ciu przycisku wyciszania
            if (isInputPressed && buttonRectangle.Contains(x, y))
            {
                if (mute == 0)
                {
                    mute = 1;
                    PlayMusic(menuMusic);
                }
                else if (mute == 1)
                {
                    mute = 0;
                    PlayMusic(menuMusic);
                }
            }
            //odpalenie gry po klikni�ciu przycisku play
            else if (isInputPressed && playRectangle.Contains(x, y))
            {
                PlayMusic(gameplayMusic);
                isGameScreenShown = true;
                isTitleScreenShown = false;
                return;
            }
            //usuwanie rekodru przy klikni�ciu
            else if (isInputPressed && buttonDelete.Contains(x, y))
            {
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                if (file.FileExists("highScore"))
                {
                    file.DeleteFile("highScore");
                    ReadScore();
                }
            }
        }

        //uaktualnienie ekranu menu
        private void UpdateTitleScreen()
        {
            //strza�ka cofnij - cofni�cie do menu
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                PlayMusic(menuMusic);
                isGameScreenShown = false;
                isTitleScreenShown = true;
            }
        }

        //uaktualnienie gracza
        private void UpdatePlayer(GameTime gameTime)
        {
            //sprawdzanie poziomu �ycia, je�eli 0 to:         
            if (player.Health <= 0)
            {
                //je�eli punkty rundy s� wi�ksze ni� rekord
                if (score > best_score)
                {
                    best_score = score; //nowy best_score (rekord)
                    WriteScore(); //zapis do IsolatedStorage
                    ReadScore(); //odczyt nowych z IsolatedStorage

                }
            
                player.Health = 100; //regeneracja �ycia do 100
                last_score = score;
                score = 0; //zerowanie punkt�w
                
                    PlayMusic(menuMusic);
                
                isGameScreenShown = false;
                isTitleScreenShown = true; //wy�wietlenie menu
                enemies.Clear(); //czyszczenie listy przeciwnik�w
            }

            //zczytywanie gest�w
            while (TouchPanel.IsGestureAvailable)
            {

                GestureSample gesture = TouchPanel.ReadGesture();
                switch (gesture.GestureType)
                {
                    //je�eli tap'ni�cie to zmiana animacji na 5 od g�ry w sprites'ie (cios). Animacja nie powtarza si� ca�y czas (false)
                    case GestureType.Tap:
                        if (playerAnimation.Attack==false)
                        {
                            playerAnimation.Initialize(playerTexture, player.Position, 56, 80, 3, 100, Color.White, 1 * skala, false, 0, 4, true);
                        }
                        break;
                        
                }

                //nie wynoszenie postaci poza ekran (je�eli mamy opcj� FreeDrag)
                //player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
                //player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);

            }

            //je�eli koniec animacji ciosu, to zmiana na standardow� animacj�
            if (playerAnimation.Active == false)
                playerAnimation.Initialize(playerTexture, player.Position, 56, 80, 3, 150, Color.White, 1 * skala, true, 0, 0, false);

            //aktualizacja playera
            player.Update(gameTime);
        }

        //uaktualnienie zabi� przeciwnika
        private void UpdateExplosions(GameTime gameTime)
        {
            //sprawdzanie wszystkich zabi�, je�eli nie aktywne to usuni�cie z listy zabitych
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }

        //uaktualnienie przeciwnik�w
        private void UpdateEnemies(GameTime gameTime)
        {
            //losowanie co ile przeciwnik
            enemySpawnTime = TimeSpan.FromSeconds((random.Next(2, 20) / 2.7) * skala);
            //dodawanie przeciwnik�w
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;
                AddEnemy();
            }

            //sprawdzanie wszystkich przeciwnik�w i aktualizacja
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);

                //je�eli nieatywny to usu� z listy
                if (enemies[i].Active == false)
                {
                    //je�eli przeciwnik zgin�� to zmiana animacji, d�wi�k zabicia i dodanie punkt�w
                    if (enemies[i].Health <= 0)
                    {
                        AddExplosion(enemies[i].Position);
                        //puszczenie d�wi�ku zabicia
                        if (mute == 0)
                        {
                            explosionSound.Play();
                        }
                        score += enemies[i].Value; //warto�� 'Value' z klasy Enemy dodawana do punkt�w gracza
                    }
                    enemies.RemoveAt(i);
                }
            }
        }

        //wykrywanie kolizji
        private void UpdateCollision()
        {
            //tworzenie prostok�t�w
            Rectangle rectangle1;
            Rectangle rectangle2;

            //opakowanie playera w prostok�t
            rectangle1 = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width * skala - 20, player.Height * skala);

            //od�wie�anie dla ka�dego przeciwnika
            for (int i = 0; i < enemies.Count; i++)
            {
                //opakowywanie przeciwnik�w w prostok�ty
                rectangle2 = new Rectangle((int)enemies[i].Position.X, (int)enemies[i].Position.Y, enemies[i].Width * skala - 20, enemies[i].Height * skala);

                //je�eli kolizja to odj�cie �ycia gracza, zabicie przeciwnika
                if (rectangle1.Intersects(rectangle2))
                {
                    //je�eli atakujemy
                    if (playerAnimation.Attack == true)
                    {
                        //animacja jest w 2 i 3 klatce zabiajmy przeciwnika
                        if (playerAnimation.blokX > 1)
                        {
                            enemies[i].Health = 0;
                        }
                        //animacja jest w 1 klatce zostajemy zabici
                        else
                        {
                            player.Health -= enemies[i].Damage;
                            enemies[i].Health = 0;
                        }
                    }
                    //je�eli nie atakujemy to przeciwnik nas zabija
                    else
                    {
                        player.Health -= enemies[i].Damage;
                        enemies[i].Health = 0;
                    }

                    //je�eli �ycie gracza = 0, to player nieaktywny
                    if (player.Health <= 0)
                        player.Active = false;
                }
            }
        }


                //
               //   RYSOWANIE
              //

        //rysowanie ekranu gry
        private void DrawGameScreen()
        {
            spriteBatch.Draw(background, new Vector2(0, 0), Color.White); //t�o gry

            player.Draw(spriteBatch); //rysowanie playera

            //rysowanie przeciwnik�w
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }
            
            //rysowanie zabitych
            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch);
            }

            //wy�wietlanie punkt�w
            spriteBatch.DrawString(font, "Score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 25, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
        }

        //rysowanie menu
        private void DrawTitleScreenShown()
        {
            spriteBatch.Draw(mainMenu, Vector2.Zero, Color.White);
            //wy�wietlanie rekordu
            spriteBatch.DrawString(font, "Best Score: " + best_score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 25, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            //ostatnio uzyskany wynik
            spriteBatch.DrawString(font, "Last Round: " + last_score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 25, GraphicsDevice.Viewport.TitleSafeArea.Y+50), Color.White);
            //usuwanie rekordu
            spriteBatch.DrawString(font, "Delete Record!", new Vector2(0, GraphicsDevice.Viewport.Height-50), Color.White);
            if (mute == 0)
            {
                spriteBatch.Draw(unmuteTexture, new Vector2(GraphicsDevice.Viewport.Width-muteTexture.Width-50, 0), Color.White);
            }
            else if (mute == 1)
            {
                spriteBatch.Draw(muteTexture, new Vector2(GraphicsDevice.Viewport.Width - unmuteTexture.Width-50, 0), Color.White);
            }
            spriteBatch.Draw(playgameTexture, new Vector2(GraphicsDevice.Viewport.Width/2-playgameTexture.Width/2, GraphicsDevice.Viewport.Height/2 - playgameTexture.Height/2), Color.White);
        }

                //
               //  INNE
              //

        //dodanie przeciwnika
        private void AddEnemy()
        {
            Animation enemyAnimation = new Animation();
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 56, 80, 3, 30, Color.White, 1 * skala, true, 0, 0, false);
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width, GraphicsDevice.Viewport.Height - enemyAnimation.FrameHeight * skala);
            Enemy enemy = new Enemy();
            enemy.Initialize(enemyAnimation, position);
            enemies.Add(enemy);
        }

        //dodawanie zabitych
        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(enemyTexture, position, 56, 80, 3, 100, Color.White, 1*skala, false,0,8, false);
            explosions.Add(explosion);
        }

        //metoda odtwarzania muzyki
        private void PlayMusic(Song song)
        {
            if (mute == 0)
            {
                try
                {
                    MediaPlayer.Play(song); //odtwarzanie piosenki
                    MediaPlayer.IsRepeating = true; //powtarzanie piosenki
                }
                catch { }
            }
            else if (mute == 1)
            {
                MediaPlayer.Stop();
            }
        }

        //zapis rekorduc (highscore) i mute do isolated Storage
        public void WriteScore()
        {
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            
            IsolatedStorageFileStream fs = null;
            using (fs = fileStorage.CreateFile("highScore"))
            {
                if (fs != null)
                {
                    // just overwrite the existing info for this example.
                    byte[] bytes = BitConverter.GetBytes(best_score);
                    
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                }
            }            
        }

        //zapis wyciszania do pliku
        public void WriteMute()
        {
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();

            IsolatedStorageFileStream fs = null;
            using (fs = fileStorage.CreateFile("mute0"))
            {
                if (fs != null)
                {
                    // just overwrite the existing info for this example.
                    byte[] bytes = BitConverter.GetBytes(mute);

                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                }
            }
        }

        //odczyt rekordu z IsolatedStorage
        public void ReadScore()
        {
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

                if (file.FileExists("highScore"))
                {
                    using (IsolatedStorageFileStream fs = file.OpenFile("highScore", FileMode.Open))
                    {
                        if (fs != null)
                        {
                            // Reload the saved high-score data.
                            byte[] saveBytes = new byte[4];
                            int count = fs.Read(saveBytes, 0, 4);
                            if (count > 0)
                            {
                                best_score = System.BitConverter.ToInt32(saveBytes, 0);
                            }
                        }
                        fs.Close();
                    }
                }
                else
                {
                    best_score = 0;
                }
        }

        //odczyt wyciszania z pliku
        public void ReadMute()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

            if (file.FileExists("mute0"))
            {
                using (IsolatedStorageFileStream fs = file.OpenFile("mute0", FileMode.Open))
                {
                    if (fs != null)
                    {
                        // Reload the saved high-score data.
                        byte[] saveBytes = new byte[4];
                        int count = fs.Read(saveBytes, 0, 4);
                        if (count >= 0)
                        {
                            mute = System.BitConverter.ToInt32(saveBytes, 0);
                        }
                    }
                    fs.Close();
                }
            }
            else
            {
                best_score = 0;
            }
        }

    }
}