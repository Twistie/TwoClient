using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Two_XNA_Client
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TwoClient : Microsoft.Xna.Framework.Game
    {
        private FindServer _findServer; 
        private readonly Thread _udpThread;
        private readonly GraphicsDeviceManager _graphics;
        public readonly Card TopDown = new Card("Top Down", 0.0, 4, 0);
        public int GameState;
        public int LightMaster = -1;
        public Boolean IsRunning = true;
        public List<Card> PlayerCards = new List<Card>();
        public List<Player> PlayerList = new List<Player>();
        public Texture2D ButtonTexture, PlayerTexture, PlayerPinkTexture,TableTexture;
        private Dictionary<double, Card> _cardList;
        private Texture2D _cardTexture;
        public Dictionary<Rectangle, IClickable> ClickableList = new Dictionary<Rectangle, IClickable>();
        public List<IDrawable> DrawableList = new List<IDrawable>(); 
        public List<Animation> AnimationList = new List<Animation>(); 
        private Boolean _hasClicked;
        private string _mouseOverText = "";
        private MouseState _ms = Mouse.GetState();
        public int PlayerNumber;
        private IPEndPoint _serverEndPoint;
        public SpriteFont Sf;
        public SpriteBatch SpriteBatch;
        private IPEndPoint _thisEndPoint;
        private Texture2D _tokenTexture;
        public List<Texture2D> BlankTextures;
        public Card TopCard;
        public int Turn;
        private Socket _udpSocket;
        private string _announce = "";
        private string _gameAnnounce = "";
        private long _lastAnnounce;
        private long _lastGameAnnounce;
        private KeyValuePair<Rectangle, Card> _mouseOverCard = new KeyValuePair<Rectangle, Card>();
        private Color _bgColour = Color.Black;
        private double _drinkStart;
        public int WindowWidth, WindowHeight;
        public TimedDrinkDialog TimedDialog;
        private int _windowHeight, _windowWidth;
        private IClickable _oldTop;
        private Mutex _isDrawing = new Mutex(false);
        public ServerDialog ServDialog;
        public KeyboardState _keyboardState;
        private bool _isKeyDown = false;
        public IAcceptKeyboard keyboardObject;
        public Dictionary<string, SoundEffect> SoundDict; 
        public TwoClient()
        {
            
            
            _findServer = new FindServer(this);
            _lastAnnounce = DateTime.Now.Ticks;
            _lastGameAnnounce = DateTime.Now.Ticks;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            CardList();
            _udpThread = new Thread(RecieveUdp);
            _udpThread.Start();
            //_findServer.TopMost = true;
            //_findServer.Show();

            Form f = (Form)Form.FromHandle(Window.Handle);
            f.Location = new System.Drawing.Point(0, 0);
            _graphics.PreferredBackBufferHeight = Screen.PrimaryScreen.WorkingArea.Size.Height;
            _graphics.PreferredBackBufferWidth = Screen.PrimaryScreen.WorkingArea.Size.Width;
            f.Size = Screen.PrimaryScreen.WorkingArea.Size;

            _windowHeight = this.Window.ClientBounds.Height;
            _windowWidth = this.Window.ClientBounds.Width;


        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            IsMouseVisible = true;
            base.Initialize();
            Window.AllowUserResizing = true;
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _keyboardState = new KeyboardState();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _cardTexture = Content.Load<Texture2D>("Cards2");
            ButtonTexture = Content.Load<Texture2D>("Buttons");
            _tokenTexture = Content.Load<Texture2D>("Tokens");
            PlayerTexture = Content.Load<Texture2D>("PlayerPlates");
            PlayerPinkTexture = Content.Load<Texture2D>("PinkPlayerPlates");
            TableTexture = Content.Load<Texture2D>("Table");
            BlankTextures = new List<Texture2D>();
            BlankTextures.Add(new Texture2D(GraphicsDevice, 1, 1));
            BlankTextures.Add(new Texture2D(GraphicsDevice, 1, 1));
            BlankTextures.Add(new Texture2D(GraphicsDevice, 1, 1));
            BlankTextures[1].SetData(new Color[] {Color.DarkGray});
            BlankTextures[0].SetData(new Color[] {Color.Black});
            BlankTextures[2].SetData(new Color[] { Color.White });
            Sf = Content.Load<SpriteFont>("normFont");
            SoundDict = new Dictionary<string, SoundEffect>();
            SoundDict.Add("CLICK",Content.Load<SoundEffect>("Click"));
            SoundDict.Add("CARD", Content.Load<SoundEffect>("cardflip"));
            SoundDict.Add("SHUFFLE",Content.Load<SoundEffect>("cardshuffle"));
           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            IsRunning = false;
            byte[] b = new byte[1];
            b[0] = 1;

            _udpSocket.SendTo(b, _thisEndPoint);

            _udpThread.Abort();
        }
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit

            MouseCheck();
            KeyboardHandler();

            base.Update(gameTime);
        }

        public void MouseCheck()
        {
            _ms = Mouse.GetState();
            if( _oldTop != null)
                _oldTop.SetMouseOver(false);
            int x = _ms.X;
            int y = _ms.Y;
            if (_hasClicked)
            {
                if (_ms.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
                    return;
                else
                {
                    _hasClicked = false;
                    return;
                }
            }
            _mouseOverText = "";
            IClickable topClickable = null;
            if (!IsActive)
                return;
            _mouseOverCard = new KeyValuePair<Rectangle, Card>(new Rectangle(0, 0, 0, 0), null);
            foreach (KeyValuePair<Rectangle, IClickable> c in ClickableList.ToList())
            {
                Rectangle r = c.Key;

                
                if (x > r.Left && x < r.Right && y > r.Top && y < r.Bottom)
                {
                    _mouseOverText = "True";
                    Card cd = c.Value as Card;
                    if (cd != null)
                    {
                        _mouseOverCard = new KeyValuePair<Rectangle, Card>(c.Key, cd);
                    }
                    topClickable = c.Value;
                }
                
            }
            if (topClickable == null)
                return;

            topClickable.SetMouseOver( true );
            _oldTop = topClickable;
            if( _ms.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed) return;
                topClickable.Click(this);
            _hasClicked = true;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            KeyboardHandler();
            _isDrawing.WaitOne();
            WindowHeight = Window.ClientBounds.Height;
            WindowWidth = Window.ClientBounds.Width;
            if (ServDialog == null)
                ServDialog = new ServerDialog(this);
            ClickableList = new Dictionary<Rectangle, IClickable>();
            GraphicsDevice.Clear(_bgColour);
            SpriteBatch.Begin();
            DrawMouse();
            DrawAnnouncement();
            DrawGameAnnouncement();
            DrawDrawables();
            switch (GameState)
            {
                case -2:
                    DrawString("Waiting for the server to start game!",(int)(this.Window.ClientBounds.Width/2-Sf.MeasureString("Waiting for the server to start game!").X/2),this.Window.ClientBounds.Height/2,0f,1f);
                    //DrawPlayerCards();
                    //DrawOtherPlayers();
                    break;
                case -1:
                    DrawString("Waiting for player list", (int)(this.Window.ClientBounds.Width / 2 - Sf.MeasureString("Waiting For Game to Start").X/2), this.Window.ClientBounds.Height, 0f, 1f);
                    
                    break;
                case 0:
                    //DrawString("Waiting To Join Game", (int)(this.Window.ClientBounds.Width / 2 - Sf.MeasureString("Waiting To Join Game").X/2), this.Window.ClientBounds.Height / 2, 0f, 1f);
                    ServDialog.Draw();
                    break;
                case 1:
                   // DrawPlayerCards();
                    DrawTopCard();
                    //DrawOtherPlayers();
                    break;
                case 2:
                    //DrawPlayerCards();
                    DrawTopCard();
                    //DrawOtherPlayers();
                    DrawMouseoverCard();
                    break;
                case 3:
                    //DrawOtherPlayers();
                    //DrawPlayerCards();
                    DrawString("Target a player!", Window.ClientBounds.Width/2 - 90,
                               Window.ClientBounds.Height/2, 0f, 1.5f);
                    break;
                case 4:
                    //DrawOtherPlayers();
                    //DrawPlayerCards();
                    DrawTimedForm(PlayerList[PlayerNumber].NumberOfDrinks,"HAH-HA");
                    break;
            }
            for( int i = 0; i < AnimationList.Count; i++ )
            {
                AnimationList[i].Draw();
                if (AnimationList[i].Finished)
                    AnimationList.RemoveAt(i);
            }
            SpriteBatch.End();
            _isDrawing.ReleaseMutex();
            base.Draw(gameTime);
        }

        public void SetGameState(int i)
        {
            if (i == GameState)
                return;
            GameState = i;
            DrawableList = new List<IDrawable>();
            if (GameState == 0)
                return;
            foreach (Player p in PlayerList)
            {
                DrawableList.Add(p);
            }
            DrawButtons();
        }
        public void RedoDrawableList()
        {
            DrawableList = new List<IDrawable>();
            if (GameState == 0)
                return;
            foreach (Player p in PlayerList)
            {
                DrawableList.Add(p);
            }
            DrawButtons();
        }
        public void DrawDrawables()
        {
            foreach (IDrawable d in DrawableList)
            {
                if( d != null)
                    d.Draw();
            }
        }
        public void DrawMouseoverCard()
        {
            if( _mouseOverCard.Value != null)
            {
                DrawCard(_mouseOverCard.Key.X - 5, _mouseOverCard.Key.Y - 5, _mouseOverCard.Value, Color.LightYellow,0f,2.2f);
            }
        }
        public void DrawTopCard()
        {
            SpriteBatch.Draw(TableTexture,new Rectangle(WindowWidth/2-160,WindowHeight/2-140,372,210),Color.White );
            if (TopCard != null)
                DrawCard((Window.ClientBounds.Width/2) -120, (Window.ClientBounds.Height/2) - 130, TopCard, Color.White,0f,2.6f);
            for (int i = 0; i < 11; i++)
            {
                DrawCard((int)((Window.ClientBounds.Width / 2) + 40+.36*i), (int)((Window.ClientBounds.Height / 2) - 130 + .65*i), TopDown, Color.White,0f,2.6f);

            }
            if (GameState == 2)
            {
                DrawCard((Window.ClientBounds.Width/2) + 44, (Window.ClientBounds.Height/2) - 125, TopDown, Color.White,0f,2.6f);
                //ClickableList.Add(
                //    new Rectangle((Window.ClientBounds.Width / 2) + 23, (Window.ClientBounds.Height / 2) - 54, 80, 120),
                //    new DrawButton());
            }
            else
            {
                DrawCard((Window.ClientBounds.Width / 2) + 44, (Window.ClientBounds.Height / 2) - 125, TopDown, Color.LightGray,0f, 2.6f);
            }
        }

        public void DrawButtons()
        {
            if (GameState == 0)
            {
                //ClickableList.Add(new Rectangle(Window.ClientBounds.X/2, Window.ClientBounds.Y/2, 120, 100),
                //                   new StartButton());
// ReSharper disable PossibleLossOfFraction
                //DrawableList.Add(new StartButton());
                SpriteBatch.Draw(ButtonTexture, new Vector2(Window.ClientBounds.X/2, Window.ClientBounds.Y/2),
                                  new Rectangle(0, 0, 120, 100), Color.White, 0, new Vector2(0, 0), 1f,
                                  new SpriteEffects(), 0);
// ReSharper restore PossibleLossOfFraction
            }
            if (GameState == 2)
            {
                DrawButton d = new DrawButton(160, WindowHeight - 125, 128, 1f, "Draw a Card", this, Sf, ButtonTexture);
                d.IsActive = true;
                
                DrawableList.Add(d);
                LightButton l = new LightButton(160, WindowHeight - 225, 128, 1f, "LightMaster", this, Sf,
                                                ButtonTexture);
                l.IsActive = true;
                DrawableList.Add(l);
            }
            if(GameState == 1)
            {
                DrawButton d = new DrawButton(160, WindowHeight - 125, 128, 1f, "Draw a Card", this, Sf, ButtonTexture);
                d.IsActive = false;
                DrawableList.Add(d);
                LightButton l = new LightButton(160, WindowHeight - 225, 128, 1f, "LightMaster", this, Sf,
                                                ButtonTexture);
                l.IsActive = true;
                DrawableList.Add(l);
            }
        }
        public void DrawPlayerList()
        {
            string playerString = "";

            foreach (Player p in PlayerList)
            {
                playerString += p.Name + "\r\n";
            }
            SpriteBatch.DrawString(Sf, playerString, new Vector2(300, 0), Color.Black);
        }

        public void DrawMouse()
        {
            string mouseString = "x: " + _ms.X + " y: " + _ms.Y + " " + _mouseOverText;
            SpriteBatch.DrawString(Sf, mouseString, new Vector2(0, 0), Color.Black);
        }
        public void SetAnnouncement(string inString)
        {
            _lastAnnounce = DateTime.Now.Ticks;
            _announce = inString;
        }
        public void SetGameAnnouncement(string inString)
        {
            _lastGameAnnounce = DateTime.Now.Ticks;
            _gameAnnounce = inString;
        }
        public void DrawAnnouncement()
        {
            float vis = 5 - (DateTime.Now.Ticks - _lastAnnounce) / 10000000f;
            if (DateTime.Now.Ticks - _lastAnnounce <= 60000000)
                SpriteBatch.DrawString(Sf, _announce, new Vector2((Window.ClientBounds.Width / 2) - Sf.MeasureString(_announce).X / 2, (Window.ClientBounds.Height / 2) - 180), Color.FromNonPremultiplied(255, 255, 255, (int)(255 * vis)));
        }
        public void DrawGameAnnouncement()
        {
            float vis = 12 - (DateTime.Now.Ticks - _lastGameAnnounce) / 10000000f;
            if( DateTime.Now.Ticks - _lastGameAnnounce  <= 130000000)
                SpriteBatch.DrawString(Sf, _gameAnnounce, new Vector2((Window.ClientBounds.Width / 2) - Sf.MeasureString(_gameAnnounce).X/2, (Window.ClientBounds.Height / 2)-200), Color.FromNonPremultiplied(255,255,255,(int)(255*vis) ));
        }
        public void DrawString(string inString, int x, int y, float rotation, float scale)
        {
            SpriteBatch.DrawString(Sf, inString, new Vector2(x, y), Color.White, rotation, new Vector2(0, 0), scale,
                                    new SpriteEffects(), 0);
        }
        public void DrawString(string inString, int x, int y, float rotation, float scale, Color colour)
        {
            SpriteBatch.DrawString(Sf, inString, new Vector2(x, y), colour, rotation, new Vector2(0, 0), scale,
                                    new SpriteEffects(), 0);
        }
        public void DrawPlayerCards()
        {
            PlayerList[PlayerNumber].DrawAsPlayerCards();
            
        }

        public void DrawToken(int tokenNumber, int x, int y, float rotation)
        {
            Rectangle r = new Rectangle(40*tokenNumber, 0, 40, 40);
            SpriteBatch.Draw(_tokenTexture, new Vector2(x, y), r, Color.White, rotation, new Vector2(0, 0), 1,
                              new SpriteEffects(), 0);
        }

        public void DrawOtherPlayers()
        {
            PlayerList[PlayerNumber].PlayerPos = 0;
            switch (PlayerList.Count)
            {
                case 0:
                    return;
                case 1:
                    //DrawLeftPlayer(0);
                    //DrawOppositePlayer(0, (int) this.Window.ClientBounds.Width/2);
                    //DrawOppositePlayer(0, (int) (this.Window.ClientBounds.Width/2 - 170)/2);
                    //DrawOppositePlayer(0,
                    //                   (int)
                    //                   ((this.Window.ClientBounds.Width/2 + 170) + (this.Window.ClientBounds.Width))/2);
                    //DrawRightPlayer(0);
                    break;
                case 2:
                    if (PlayerNumber == 0)
                        PlayerList[1].PlayerPos = 2;
                    else
                        PlayerList[0].PlayerPos = 2;
                    break;
                default:
                    int leftPlayer = PlayerNumber + 1;
                    int rightPlayer = PlayerNumber - 1;
                    if (leftPlayer > PlayerList.Count - 1)
                        leftPlayer = 0;
                    if (rightPlayer < 0)
                        rightPlayer = PlayerList.Count - 1;
                    PlayerList[leftPlayer].PlayerPos = -1;
                    PlayerList[rightPlayer].PlayerPos = 1;
                    if (PlayerList.Count == 4)
                    {
                        int oppositePlayer = PlayerNumber + 2;
                        if (oppositePlayer > PlayerList.Count - 1)
                            oppositePlayer = oppositePlayer - PlayerList.Count;
                        PlayerList[oppositePlayer].PlayerPos = 2;
                    }
                    break;
            }
            RedoDrawableList();
        }

        public void Animation_CardToPlayer(int player, int noCards, Card card)
        {
            switch( PlayerList[player].PlayerPos )
            {
                case 0:
                    for (int i = 0; i < noCards; i++)
                    {
                        AnimationList.Add(new DelayedCardMove(this, TopDown, WindowWidth / 2 + 30, WindowHeight / 2 - 40,
                                                       WindowWidth / 2, WindowHeight - 220, .2f,.07f*i));
                    }
                    break;
                case 2:
                    for (int i = 0; i < noCards; i++)
                    {
                        AnimationList.Add(new DelayedCardMove(this, TopDown, WindowWidth / 2 + 30, WindowHeight / 2 - 90,
                                                       WindowWidth / 2 - 100, 120, .2f, .07f * i));
                    }
                    break;
                case -1:
                    for (int i = 0; i < noCards; i++)
                    {
                        AnimationList.Add(new DelayedCardMove(this, TopDown, WindowWidth / 2 - 30, WindowHeight / 2 - 40,
                                                       100, WindowHeight / 2 - 40, .2f, .07f * i));
                    }
                    break;
                case 1:
                    for (int i = 0; i < noCards; i++)
                    {
                        AnimationList.Add(new DelayedCardMove(this, TopDown, WindowWidth / 2 + 60, WindowHeight / 2 - 40,
                                                       WindowWidth - 100, WindowHeight / 2 - 40, .2f, .07f * i));
                    }
                    break;
                default:
                    return;
            }
        }
        public void Animation_CardFromPlayer(int player, int noCards, Card card)
        {
            switch (PlayerList[player].PlayerPos)
            {
                case 0:
                    for (int i = 0; i < noCards; i++)
                    {
                        AnimationList.Add(new DelayedCardMove(this, TopCard, WindowWidth / 2, WindowHeight - 220, WindowWidth / 2 - 60, WindowHeight / 2 - 40, .2f, .07f * i));


                    }
                    break;
                case 2:
                    for (int i = 0; i < noCards; i++)
                    {
                        AnimationList.Add(new DelayedCardMove(this, TopCard, WindowWidth / 2 - 100, 120, WindowWidth / 2 - 60, WindowHeight / 2 - 90,
                                                        .2f, .07f * i));
                    }
                    break;
                case -1:
                    for (int i = 0; i < noCards; i++)
                    {
                        AnimationList.Add(new DelayedCardMove(this, TopCard, 100, WindowHeight / 2 - 40, WindowWidth / 2 - 60, WindowHeight / 2 - 40,
                                                        .2f, .07f * i));
                    }
                    break;
                case 1:
                    for (int i = 0; i < noCards; i++)
                    {
                        AnimationList.Add(new DelayedCardMove(this, TopCard, WindowWidth - 100, WindowHeight / 2 - 40, WindowWidth / 2 - 60, WindowHeight / 2 - 40,
                                                        .2f, .07f * i));
                    }
                    break;
                default:
                    return;
            }
        }
        public void DrawOppositePlayer(int player, int x)
        {
            PlayerList[player].DrawOppositePlayer(x);
        }
        public void DrawBorderedRect(int col, int x, int sizeX, int y, int sizeY )
        {
            Rectangle r = new Rectangle(x,y,sizeX,sizeY);
            Rectangle b = new Rectangle(x-5,y-5,sizeX+10,sizeY+10);
            SpriteBatch.Draw(BlankTextures[0],b,Color.White);
            SpriteBatch.Draw(BlankTextures[1],r, Color.White);
        }
        public void DrawCard(int x, int y, Card c, Color colour)
        {
            Vector2 zeroVect = new Vector2(0, 0);

            SpriteBatch.Draw(_cardTexture, new Vector2(x, y), c.CardRect, colour, 0, zeroVect, .75f, new SpriteEffects(),
                              0);
        }

        public void DrawCard(int x, int y, Card c, Color colour, float rotation)
        {
            Vector2 zeroVect = new Vector2(0, 0);

            SpriteBatch.Draw(_cardTexture, new Vector2(x, y), c.CardRect, colour, rotation, zeroVect, .75f,
                              new SpriteEffects(), 0);
        }

        public void DrawCard(int x, int y, Card c, Color colour, float rotation, float scale)
        {
            Vector2 zeroVect = new Vector2(0, 0);

            SpriteBatch.Draw(_cardTexture, new Vector2(x, y), c.CardRect, colour, rotation, zeroVect, scale/2.6f,
                              new SpriteEffects(), 0);
        }

        public void DrawLeftPlayer(int player)
        {
            PlayerList[player].DrawVerticalPlayer(10);
        }

        public void DrawRightPlayer(int player)
        {
            PlayerList[player].DrawVerticalPlayer(WindowWidth-138);
        }

        public void SendCard(Card _c)
        {
            int n = PlayerCards.Count;
            for (int i = 0; i < n; i++)
            {
                if (PlayerCards[i] == _c)
                {
                    string toSend = ("CARD " + i);
                    SendMessage(toSend);
                    return;
                }
            }
        }

        private void RecieveUdp()
        {

            IPHostEntry localHostEntry;
            try
            {
                //Create a UDP socket.
                _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                localHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            }
            catch (Exception)
            {
                AddText("Local Host not found"); // fail
                return;
            }

            IPAddress localIp = null;
            foreach (IPAddress ip in localHostEntry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    localIp = ip;
            }
            if( localIp == null )
            {
                throw new Exception( "No compatible network device found!");
            }
            int port = 48485;
            while (!_udpSocket.IsBound)
            {
                try
                {
                    _thisEndPoint = new IPEndPoint(localIp, port);
                    _udpSocket.Bind(_thisEndPoint);
                }catch(Exception e )
                {
                    port++;
                }
            }
            _serverEndPoint = new IPEndPoint(localIp, 48484);
            while (IsRunning)
            {
                Byte[] received = new Byte[1024];
                IPEndPoint tmpIpEndPoint = new IPEndPoint(IPAddress.Any, 48485);
                EndPoint remoteEp = (tmpIpEndPoint);
                int bytesReceived = _udpSocket.ReceiveFrom(received, ref remoteEp);
                String dataReceived = System.Text.Encoding.ASCII.GetString(received);
                _isDrawing.WaitOne();
                if(GameState == 0 || remoteEp.Equals( _serverEndPoint))
                    MessageParser(dataReceived.Replace("\0", ""), (IPEndPoint) remoteEp);
                _isDrawing.ReleaseMutex();
            }
        }

        public void AddText(string toAdd)
        {
            //logBox.Text = logBox.Text + "\r\n" + toAdd;
        }

        public void SendJoin(IPEndPoint inAddr, string inName)
        {
            _serverEndPoint = inAddr;
            _serverEndPoint.Port = 48484;
            AddText(_serverEndPoint.ToString());
            string toSend = "JOIN " + inName;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] sendByte = encoding.GetBytes(toSend);
            _udpSocket.SendTo(sendByte, _serverEndPoint);
            GameState = -1;
            SetAnnouncement( "Waiting for Server to begin");
        }

        public void SendMessage(string toSend)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] sendByte = encoding.GetBytes(toSend);
            _udpSocket.SendTo(sendByte, _serverEndPoint);
        }

        public void MessageParser(string inMessage, IPEndPoint inAddr)
        {
            AddTextFromThread(inMessage);
            string[] messageArray = inMessage.Split(' ');
            if (GameState == 0)
                if (messageArray[0] == "GAME")
                {
                    ServDialog.AddServer("Game",inAddr);
                    return;
                }
                else
                    return;

            if (messageArray[0] == "GAME")
                return;
           

    switch (messageArray[0])
            {
                case "PLAYERNUMBER":
                    PlayerNumber = int.Parse(messageArray[1]);
                    SetAnnouncement("Player Number: " + messageArray[1]);
                    AddTextFromThread("Our player number: " + messageArray[1]);
                    break;
                case "CARDLIST":
                    SetPlayerCards(messageArray);
                    break;
                
                case "TURN":
                    Turn = int.Parse(messageArray[1]);
                    if (TimedDialog != null)
                        if (TimedDialog.IsActive)
                            return;
                    if (int.Parse(messageArray[1]) == PlayerNumber)
                        if (PlayerList[PlayerNumber].NumberOfDrinks < 5)
                        {
                            SetGameState(2);
                            GameState = 2;
                        }
                        else
                        {
                            TimedDialog = new TimedDrinkDialog(this, PlayerList[PlayerNumber].NumberOfDrinks * .75f, "NOT SO FAST. Too many drinks backed up.");
                            GameState = 4;
                        }
                    else
                        GameState = 1;
                        
                    
                    break;
                case "TOPCARD":
                    TopCard = _cardList[double.Parse(messageArray[1])];
                    break;
                case "PLAYERLIST":
                    SetPlayerList(messageArray);
                    DrawOtherPlayers();
                    break;
                case "ANIMATION":
                    switch( messageArray[1])
                    {
                        case "CARDSTO":
                                Animation_CardToPlayer(int.Parse(messageArray[2]), int.Parse(messageArray[3]), TopDown);
                                

                            break;
                        case "CARDSFROM":
                            Animation_CardFromPlayer(int.Parse(messageArray[2]), int.Parse(messageArray[3]), TopDown);
                            break;
                        default:
                            break;
                    }
                    break;
                case "NUMBERCARDS":
                    PlayerList[int.Parse(messageArray[1])].NumberOfCards = int.Parse(messageArray[2]);
                    break;
                case "TARGETPLAYER":
                    DoTargetPlayer();
                    break;
                case "ANNOUNCE":
                    SetAnnouncement(inMessage.Remove(0,8));
                    break;
                case "GAMEANNOUNCE":
                    SetGameAnnouncement(inMessage.Remove(0, 12));
                    break;
                case "LIGHTON":
                    PlayerList[int.Parse(messageArray[1])].LightOn = true;
                    break;
                case "LIGHTOFF":
                    PlayerList[int.Parse(messageArray[1])].LightOn = false;
                    break;
                case "TIMEDFORM":
                    if( TimedDialog != null)
                        if (TimedDialog.IsActive)
                            return;
                    TimedDialog = new TimedDrinkDialog(this, int.Parse(messageArray[1]), inMessage.Remove(0, 12));
                    TimedDialog.ResetDrinks = false;
                    
                    break;
                case "NAME":
                    PlayerList[int.Parse(messageArray[1])].Name = inMessage.Remove(0,7);
                    break;
                case "COLOUR":
                    DoColour(int.Parse(messageArray[1]),int.Parse(messageArray[2]));
                    break;
                case "LOBBY":
                    GameState = -2;
                    break;
                case "DRINK":
                    PlayerList[int.Parse(messageArray[1])].NumberOfDrinks += int.Parse(messageArray[2]);
                    break;
                case "SETDRINK":
                    PlayerList[int.Parse(messageArray[1])].NumberOfDrinks = int.Parse(messageArray[2]);
                    break;
                case "STARTGAME":
                    StartGame sg = new StartGame(this);
                    break;
            }
        }
        public void DoColour(int player, int col)
        {

            switch(col)
            {
                case 0:
                    PlayerList[player].CurrentPlateTexture = PlayerTexture;
                    break;
                case 1:
                    PlayerList[player].CurrentPlateTexture = PlayerPinkTexture;
                    break;
            }
        }
        public List<string> SplitText( int maxWidth, string inString )
        {
            string outString = "";
            List<string> outList = new List<string>();
            float curLength = 0;
            foreach (string s in inString.Split(' ') )
            {
                if( curLength + Sf.MeasureString(s).X < 300 )
                {
                    outString += s + " ";
                    curLength += Sf.MeasureString(s).X;
                } else
                {
                    outList.Add(outString);
                    curLength = Sf.MeasureString(s).X;
                    outString = s + " ";
                }
            }
            outList.Add(outString);
            return outList;
        }
        public void DrawTimedForm( int time, string message)
        {
            TimedDialog.Draw();
        }

        private void DoTargetPlayer()
        {
            GameState = 3;
        }
        public void SetPlayerList(string[] inList)
        {
            PlayerList = new List<Player>();
            for (int i = 1; i < inList.Count() - 1; i++)
            {
                PlayerList.Add(new Player(i - 1, inList[i].Replace('+',' '),this));
            }
            DrawOtherPlayers();
        }

        public void SetPlayerCards(string[] cardList)
        {
            PlayerCards = new List<Card>();
            for (int i = 2; i < int.Parse(cardList[1]) + 2; i++)
            {
                PlayerCards.Add(_cardList[double.Parse(cardList[i])]);
            }
        }
        
        private void AddTextFromThread(string message)
        {
            //if (this.logBox.InvokeRequired)
            //    this.logBox.BeginInvoke(new MethodInvoker(delegate() { AddTextFromThread(Message); }));
            //else
            //    AddText(Message);
        }


        public void CardList()
        {
            _cardList = new Dictionary<double, Card>();
            for (int i = 0; i < 4; i++)
            {
                string colour = "";
                switch (i)
                {
                    case 0:
                        colour = "Red";
                        break;
                    case 1:
                        colour = "Blue";
                        break;
                    case 2:
                        colour = "Yellow";
                        break;
                    case 3:
                        colour = "Green";
                        break;
                }
                Card c;
                for (int n = 0; n < 10; n++)
                {
                    c = new Card(colour + " " + n, double.Parse(i + ".0" + n), i, n);
                    c.Name = colour + " " + n;
                    c.SortValue = double.Parse(i + ".0" + n);
                    c.Types.Add(colour);
                    c.Types.Add(n.ToString());
                    c.ValidTypes.Add(colour);
                    c.ValidTypes.Add(n.ToString());
                    _cardList.Add(c.SortValue, c);
                }
                for (int n = 0; n < 2; n++)
                {
                    int draw = n + 2;

                    c = new Card(colour + " Draw " + draw, double.Parse(i + ".1" + draw), i, 10 + n);
                    c.Types.Add(colour);
                    c.Types.Add("DRAW " + draw);
                    c.ValidTypes.Add("TARGET " + draw);
                    c.ValidTypes.Add("DRAW " + draw);
                    _cardList.Add(c.SortValue, c);

                    c = new Card(colour + " Draw " + draw, double.Parse(i + ".1" + draw + "1"), i, 10 + n);
                    c.Types.Add(colour);
                    c.Types.Add("DRAW " + draw);
                    c.ValidTypes.Add("DRAW " + draw);
                    c.ValidTypes.Add(colour);
                    _cardList.Add(c.SortValue, c);
                }
                c = new Card("Targetted Draw 2", i+ .22,i,12);
                c.Types.Add(colour);
                c.Types.Add("TARGET 2");
                c.ValidTypes.Add("TARGET 2");
                _cardList.Add(c.SortValue,c);

                c = new Card("Targetted Draw 2", i + .221, i, 12);
                c.Types.Add(colour);
                c.Types.Add("TARGET 2");
                c.ValidTypes.Add(colour);
                c.ValidTypes.Add("TARGET 2");
                _cardList.Add(c.SortValue, c);

                c = new Card("Light Master", double.Parse(i + ".3"),i,13);
                c.Types.Add(colour);
                c.Types.Add("LIGHTMASTER");
                c.ValidTypes.Add(colour);
                c.ValidTypes.Add("LIGHTMASTER");
                _cardList.Add(c.SortValue, c);

                c = new Card("Double Down", double.Parse(i + ".4"),i,15);
                c.Types.Add(colour);
                c.Types.Add("DOUBLEDOWN");
                c.ValidTypes.Add(colour);
                c.ValidTypes.Add("DOUBLEDOWN");
                _cardList.Add(c.SortValue,c);

                c = new Card("Skip", double.Parse(i + ".5"),i,14);
                c.Types.Add(colour);
                c.Types.Add("SKIP");
                c.ValidTypes.Add(colour);
                c.ValidTypes.Add("SKIP");
                _cardList.Add(c.SortValue,c);


            }
            Card pf = new Card("Princess Fufu", 5.01, 4, 1);
            pf.Types.Add("Red");
            pf.Types.Add("Blue");
            pf.Types.Add("Green");
            pf.Types.Add("Yellow");
            pf.ValidTypes.Add("Red");
            pf.ValidTypes.Add("Blue");
            pf.ValidTypes.Add("Green");
            pf.ValidTypes.Add("Yellow");
            _cardList.Add(5.01,pf);

            Card bc = new Card("BoomCard", 5.02, 4, 2);
            bc.Types.Add("Red");
            bc.Types.Add("Blue");
            bc.Types.Add("Green");
            bc.Types.Add("Yellow");
            bc.ValidTypes.Add("Red");
            bc.ValidTypes.Add("Blue");
            bc.ValidTypes.Add("Green");
            bc.ValidTypes.Add("Yellow");
            _cardList.Add(5.02, bc);

            bc = new Card("Grey", 5.03, 4, 3);
            bc.Types.Add("Red");
            bc.Types.Add("Blue");
            bc.Types.Add("Green");
            bc.Types.Add("Yellow");
            bc.ValidTypes.Add("Red");
            bc.ValidTypes.Add("Blue");
            bc.ValidTypes.Add("Green");
            bc.ValidTypes.Add("Yellow");
            _cardList.Add(5.03, bc);

        }


        public void DrawCard()
        {
            if (GameState == 2)
            {
                GameState = 1;
                SendMessage("DRAWCARD");
            }
        }
        List<Keys> PreviousKeys = new List<Keys>();
        long lastTime;
        bool fastTimer = false;
        public void KeyboardHandler()
        {
            if (keyboardObject == null)
                return;
            long currentTime = System.Environment.TickCount;
            _keyboardState = Keyboard.GetState();
            Keys[] keyArray = _keyboardState.GetPressedKeys();
            List<Keys> keys = keyArray.ToList();
            keys.Remove(Keys.None);
            if (keys.Count() == 0)
            {
                fastTimer = false;

            }
            if (keys.Count() == 1 && (keys[0] == Keys.Back))
                if (!PreviousKeys.Contains(Keys.Back) || currentTime - lastTime > (fastTimer ? 100 : 500))
                {
                    _isKeyDown = true;
                    keyboardObject.GetKey("BackSpace");

                    lastTime = currentTime;
                    if (PreviousKeys.Contains(Keys.Back))
                        fastTimer = true;
                }

            string s = "";
            bool isShift = keys.Contains(Keys.LeftShift) || keys.Contains(Keys.RightShift);
            keys.Remove(Keys.LeftShift);
            keys.Remove(Keys.RightShift);

            foreach (Keys key in keys)
            {
                if (!PreviousKeys.Contains(key) || currentTime - lastTime > (fastTimer ? 50 : 400))
                {
                    s = key.ToString();
                   switch( s)
                    {
                        case "Space":
                            s = " ";
                            break;
                        case "OemMinus":
                            s = "-";
                            break;
                        case "OemPeriod":
                            s = ".";
                            break;
                       case "OemQuestion":
                            s = "?";
                            break;
                       case "D1":
                            s = "!";
                            break;
                       case "D2":
                            s = "@";
                            break;
                       case "D3":
                            s = "#";
                            break;
                       case "D4":
                            s = "$";
                            break;
                       case "D5":
                            s = "%";
                            break;
                       case "D6":
                            s = "^";
                            break;
                       case "D7":
                            s = "&";
                            break;
                       case "D8":
                            s = "*";
                            break;
                    }
                    if (s.Length > 1)
                        s = "";
                    lastTime = currentTime;
                    if (PreviousKeys.Contains(key))
                        fastTimer = true;
                }
            }
            PreviousKeys = keys;
            if (s == "")
                return;

            _isKeyDown = true;
            if (!isShift)
                s = s.ToLower();

            keyboardObject.GetKey(s);

        }
    }
}