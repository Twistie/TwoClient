using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Two_XNA_Client
{
    /// <summary>
    /// Interface for objects that can be drawn
    /// </summary>
    public interface  IDrawable
    {
        void Draw();
    }
    public interface IAcceptKeyboard
    {
        void GetKey(String s);
    }
    public class CurvedRect : IDrawable
    {
        
        public int X, Width, Y, Height;
        public Color Colour;
        public Rectangle R;
        private TwoClient _twoClient;

        public CurvedRect(TwoClient twoClient, int x, int width, int y, int height, Color colour)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Colour = colour;
            _twoClient = twoClient;
            R = new Rectangle(x, y, width, height);
        }
        public void Draw()
        {
            _twoClient.SpriteBatch.Draw(_twoClient.TableTexture, R, Colour);
        }
    }
    public class SmallBorderRect : IDrawable
    {
        public int X, Width, Y, Height;
        public int Colour;
        public Rectangle R, B;
        private TwoClient _twoClient;
        public SmallBorderRect(TwoClient twoClient, int x, int width, int y, int height, int colour)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Colour = colour;
            _twoClient = twoClient;
            R = new Rectangle(x, y, width, height);
            B = new Rectangle(x - 2, y - 2, width + 4, height + 4);
        }
        public void Draw()
        {
            _twoClient.SpriteBatch.Draw(_twoClient.BlankTextures[0], B, Color.White);
            _twoClient.SpriteBatch.Draw(_twoClient.BlankTextures[Colour], R, Color.White);
        }
    }
    public class Rect : IDrawable
    {
        public int X, Width, Y, Height;
        public int Colour;
        public Rectangle R;
        private TwoClient _twoClient;
        public Rect(TwoClient twoClient, int x, int width, int y, int height, int colour)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Colour = colour;
            _twoClient = twoClient;
            R = new Rectangle(x, y, width, height);
        }
        public void Draw()
        {
            _twoClient.SpriteBatch.Draw(_twoClient.BlankTextures[Colour], R, Color.White);
        }
    }
    public class ProgressBar : IDrawable
    {
        public int X, Width, Y, Height;
        public int Colour;
        public Rectangle R, B;
        private TwoClient _twoClient;
        public float Progress = 0f;
        public ProgressBar(TwoClient twoClient, int x, int width, int y, int height, int colour)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Colour = colour;
            _twoClient = twoClient;
            R = new Rectangle(x, y, 1, height);
            B = new Rectangle(x - 2, y - 2, width + 4, height + 4);
        }
        public void SetProgress(float f)
        {
            R.Width = (int)(f*Width);
        }
        public void Draw()
        {
            _twoClient.SpriteBatch.Draw(_twoClient.BlankTextures[0], B, Color.White);
            _twoClient.SpriteBatch.Draw(_twoClient.BlankTextures[2], R, Color.White);
        }
    }
    public interface ITimedDialog : IDrawable
    {
        Boolean IsActive { get; set; }
    }
    public class TimedDrinkDialog : ITimedDialog
    {
        private TwoClient _twoClient;
        public double TimeStarted;
        public double TimeDelay;
        public double TimeFinish;
        private List<string> _dialogText;
        public bool IsRunning = false;
        private StartDrinkButton _startButton;
        private CloseDrinkButton _closeButton;
        private ProgressBar _progBar;
        public int OldGameState;
        public bool ResetDrinks = true;
        private CurvedRect _bgRect;
        private DrinkAnimation _drinkAnimation;
        public int PlayerNumber;
        public Boolean IsPlayer;
        private string _playerName;
        public TimedDrinkDialog( TwoClient twoClient, float delay, string toSay )
        {
            _twoClient = twoClient;
                
                IsPlayer = true;
                _startButton = new StartDrinkButton(_twoClient, this, -75, -10, _twoClient.Sf, twoClient.ButtonTexture);
                _closeButton = new CloseDrinkButton(_twoClient, this, -75, 90, _twoClient.Sf, twoClient.ButtonTexture);
                //_twoClient.SendMessage("DRINKSFORM " + delay);
                _drinkAnimation = new DrinkAnimation(twoClient, twoClient.WindowWidth / 2 - 215, twoClient.WindowHeight / 2 - 60, delay);
            IsActive = true;
            _playerName = _twoClient.PlayerList[_twoClient.PlayerNumber].Name;
            _dialogText = _twoClient.SplitText(385, toSay);
            TimeDelay = delay * 10000000;// NORMAL: 10000000;
            _progBar = new ProgressBar(_twoClient,_twoClient.WindowWidth/2-175,350,twoClient.WindowHeight /2, 15, 2);
            _bgRect = new CurvedRect(_twoClient, _twoClient.WindowWidth / 2 - 220, 440, _twoClient.WindowHeight / 2 - 40 - (25 * _dialogText.Count), 160 + (25 * _dialogText.Count), Color.LightBlue);
            OldGameState = twoClient.GameState;
            
            twoClient.GameState = 4;
        }
        public void AddDrink(float timeToAdd)
        {
            TimeDelay += (long) (timeToAdd*10000000);
            _drinkAnimation.AddTime( timeToAdd);
        }
        public void Start()
        {
            TimeStarted = DateTime.Now.Ticks;
            IsActive = true;
            IsRunning = true;
            _drinkAnimation.Start();
            _twoClient.SendMessage("STARTDRINK");

        }
        public void Close()
        {
            _twoClient.GameState = OldGameState;
            if (ResetDrinks)
            {
                _twoClient.SendMessage("DRINKRESET");
                _twoClient.PlayerList[_twoClient.PlayerNumber].NumberOfDrinks = 0;
            }
            IsActive = false;
        }
        public void Draw()
        {
            if( IsRunning )
            {
                if( (System.DateTime.Now.Ticks - TimeStarted) > TimeDelay  + 10000000 )
                    Close();
            }
            _twoClient.SpriteBatch.Draw(_twoClient.DrinkPlateTexture,
                                                new Rectangle(_twoClient.WindowWidth/2-250, _twoClient.WindowHeight/2-250,
                                                              500, 500),
                                                new Rectangle(0, 0, 500, 500), Color.White);
            _twoClient.DrawString(_playerName,(int)(_twoClient.WindowWidth / 2 - (_twoClient.Sf.MeasureString(_playerName).X) ),_twoClient.WindowHeight/2 - 232,0f,2.0f,Color.Yellow);
            int lineNumber = 0;
            foreach (string s in _dialogText)
            {
                _twoClient.DrawString(s, (int)(_twoClient.WindowWidth / 2 - _twoClient.Sf.MeasureString(s).X / 2), _twoClient.WindowHeight / 2 - 27 * (_dialogText.Count - lineNumber) - 96 ,0f,1f);
                lineNumber++;
            }

            _startButton.Draw();
            
            _drinkAnimation.Draw();
        }
        public class StartDrinkButton : Button
        {
            private readonly TimedDrinkDialog _dialog;
            private TwoClient _twoClient;
            
            public bool ResetDrinks = true;

            public StartDrinkButton(TwoClient twoClient, TimedDrinkDialog dialog, int xOffSet, int yOffSet, SpriteFont sf,Texture2D butTexture) : 
                base(twoClient.WindowWidth/2 - xOffSet,twoClient.WindowHeight/2 + yOffSet,100,1f,"Start Drinking!",twoClient,sf,butTexture)
            {
                IsActive = true;
                _dialog = dialog;
                _twoClient = twoClient;
            }

            public override void Click(TwoClient twoClient)
            {
                IsActive = false;
                _dialog.Start();
            }
        }
        public class CloseDrinkButton : Button
        {
            private readonly TimedDrinkDialog _dialog;
            private TwoClient _twoClient;
            public int XOffset, YOffset;
            
            public CloseDrinkButton(TwoClient twoClient, TimedDrinkDialog dialog, int xOffSet, int yOffSet, SpriteFont sf,Texture2D butTexture) : 
                base(twoClient.WindowWidth/2 - xOffSet,twoClient.WindowHeight/2 + yOffSet,100,1f,"Close Window",twoClient,sf,butTexture)
            {
                
                _dialog = dialog;
                _twoClient = twoClient;

            }
            public override void Click(TwoClient twoClient)
            {
                
                _dialog.Close();
                base.Click(twoClient);
            }
        }

        public bool IsActive
        {
            get; set; 
        }
    }
    public class MooseForm : ITimedDialog
    {
        private readonly TwoClient _twoClient;
        private readonly long _timeCreated;
        private readonly MooseButton _mBut;
        private readonly int _oldGameState;
        
        public MooseForm(TwoClient twoClient, int x, int y, int width, int height )
        {
            _twoClient = twoClient;
            _timeCreated = System.DateTime.Now.Ticks;
            _oldGameState = twoClient.GameState;
            _mBut = new MooseButton(x,y, width, height,
                                    twoClient,_twoClient.MOOSETexture, _oldGameState);
            _oldGameState = twoClient.GameState;
            _twoClient.SoundDict["MOOSE"].Play();
            twoClient.GameState = 4;
        }

        public void Draw()
        {
            _mBut.Draw();
                
            
        }

        public bool IsActive { get;
            set;
        }
        private class MooseButton : IClickable, IDrawable
        {
            private TwoClient _twoClient;
            private Texture2D _butTexture;
            private Rectangle _boxPos;
            public Boolean MouseOver { get; set; }
            public Boolean BeenClicked { get; set; }
            private int _oldState;
            public void Click(TwoClient twoClient)
            {
                twoClient.SendMessage("MOOSE");
                twoClient.TimedDialog.IsActive = false;
                _twoClient.GameState = _oldState;
            }

            public bool GetMouseOver()
            {
                return MouseOver;
            }

            public void SetMouseOver(bool mouseOver)
            {
                MouseOver = mouseOver;
            }

            public MooseButton(int x, int y, int width, int height, TwoClient twoClient, Texture2D butTexture, int oldState)
            {
                _twoClient = twoClient;
                _butTexture = butTexture;
                _boxPos = new Rectangle(x, y, width, height);
                MouseOver = false;
                BeenClicked = false;
                _oldState = oldState;
            }
            public void Draw()
            {
                _twoClient.ClickableList.Add(_boxPos, this);

                _twoClient.SpriteBatch.Draw(_butTexture, _boxPos, Color.White);
            }
        }
    }
    public class ServerDialog : IDrawable
    {
        private TwoClient _twoClient;
        private string _findServer;
        private int _x, _y;
        private Rectangle _bgRect;
        public ChooseList Cl;
        private TextField _tf;
        readonly Dictionary<int, IPEndPoint> _serverDictionary = new Dictionary<int, IPEndPoint>();
        private StartButton _startButton;
        public ServerDialog( TwoClient twoClient)
        {
            _twoClient = twoClient;
            _findServer = "Choose a server";
            _x = _twoClient.WindowWidth/2 - 150;
            _y = _twoClient.WindowHeight/2 - 170;
            _bgRect = new Rectangle(_x,_y,300,340);
            Cl = new ChooseList(_twoClient,11,180,_x+25,_y+85);
            try
            {
                StreamReader sr = new StreamReader("two.cfg");
                _tf = new TextField(_twoClient, sr.ReadLine(), _x + 65, _y + 45, 12);
                sr.Close();
            }catch( Exception e)
            {
                _tf = new TextField(_twoClient, "def", _x + 65, _y + 45, 12);
            }
           
            _twoClient.keyboardObject = _tf;
            _startButton = new StartButton(_x + 217,_y + 100,75,40,1f,"Join Game", _twoClient, _twoClient.Sf,_twoClient.ButtonTexture,this);
            _startButton.IsActive = true;
        }
        public void Draw()
        {
            _twoClient.SpriteBatch.Draw(_twoClient.TableTexture,_bgRect,Color.White );
            _twoClient.DrawString(_findServer, _x + 25, _y + 5, 0f, 1.25f, Color.Black);
            Cl.Draw();
            _tf.Draw();
            _startButton.Draw();

        }
        public void AddServer(string name, IPEndPoint inAddr)
        {
            if (_serverDictionary.ContainsValue(inAddr))
                return;
            _serverDictionary.Add(Cl.ItemList.Count, inAddr);
            Cl.ItemList.Add(name);
        }
        class StartButton : Button
        {
            private ServerDialog _serverDialog;
            public StartButton(int x, int y, int width, int height, float scale, string text, TwoClient twoClient, SpriteFont font, Texture2D butTexture, ServerDialog serverDialog)
                : base(x, y, width, height, scale, text, twoClient, font, butTexture)
            {
                _serverDialog = serverDialog;
            }

            public override void Click(TwoClient twoClient)
            {
                base.Click(twoClient);
                if(_serverDialog.Cl.CurSelect == -1 )
                    return;
                try
                {
                    StreamWriter sw = new StreamWriter("two.cfg");
                    sw.Write(_serverDialog._tf.Current);
                    sw.Close();
                    
                }catch( Exception e)
                {
                    twoClient.SetAnnouncement(e.ToString());
                }
                twoClient.SendJoin(_serverDialog._serverDictionary[_serverDialog.Cl.CurSelect],_serverDialog._tf.Current);
            }
        }
    }
    public class ChooseList : IDrawable
    {
        private TwoClient _twoClient;
        private int _size;
        private SmallBorderRect _backRec;
        public List<String> ItemList;
        private int _x, _y, _width;
        public int CurSelect;
        private Rect _selectRect;
        private List<Vector2> _stringPos;
        private List<IClickable> _clickList; 
        private List<Rectangle> _clickRect;
        
        public ChooseList(TwoClient twoClient, int size, int width, int x, int y)
        {
            _twoClient = twoClient;
            _size = size;
            _backRec = new SmallBorderRect(_twoClient,x,width,y,size*20,0);
            ItemList = new List<string>();
            _x = x;
            _y = y;
            _width = width;
            
            CurSelect = -1;
            _selectRect = new Rect(_twoClient,x+2,width-4,0,20,1);
            _stringPos = new List<Vector2>();
            _clickRect = new List<Rectangle>();
            _clickList = new List<IClickable>();
            for( int i = 0; i < size; i ++)
            {
                _stringPos.Add(new Vector2(x + 3,y + 2 + 20 * i));
                _clickRect.Add(new Rectangle(x,y + 20 * i,width,20));
                _clickList.Add(new SelectItem(i));
            }
        }
        public void Draw()
        {
            _backRec.Draw();
            int i = 0;
            foreach (string s in ItemList)
            {
                if(i == CurSelect)
                {
                    _selectRect.R.Y = _y + i*20 + 2;
                    _selectRect.Draw();
                    _twoClient.SpriteBatch.DrawString(_twoClient.Sf,s,_stringPos[i],Color.Black);

                }
                else
                {
                    _twoClient.SpriteBatch.DrawString(_twoClient.Sf, s, _stringPos[i], Color.White);
                }
                _twoClient.ClickableList.Add(_clickRect[i],_clickList[i]);
                
                i++;
            }
        }
        public class SelectItem : IClickable
        {
            private bool _isMouseOver;
            private int _itemNo;
            public SelectItem(int itemNumber)
            {
                _itemNo = itemNumber;
            }
            public void Click(TwoClient twoClient)
            {
                twoClient.ServDialog.Cl.CurSelect = _itemNo;
                twoClient.SoundDict["CLICK"].Play();
            }

            public bool GetMouseOver()
            {
                return _isMouseOver;
            }

            public void SetMouseOver(bool mouseOver)
            {
                _isMouseOver = mouseOver;
            }
        }
        
        
    }
    public class TextField : IClickable, IDrawable, IAcceptKeyboard
    {
        private TwoClient _twoClient;
        public string Current;
        private Rect _bgRect;
        private Vector2 _stringPos;
        public TextField(TwoClient twoClient, string def, int x, int y, int length)
        {
            _twoClient = twoClient;
            _bgRect = new Rect(_twoClient, x, length * 15, y, 20, 0);
            Current = def;
            _stringPos = new Vector2(x + 2, y);


        }
        public void Click(TwoClient twoClient)
        {

        }

        public bool GetMouseOver()
        {
            return false;
        }

        public void SetMouseOver(bool mouseOver)
        {

        }

        public void Draw()
        {
            _bgRect.Draw();
            _twoClient.SpriteBatch.DrawString(_twoClient.Sf, Current + "_", _stringPos, Color.White);
        }

        public void GetKey(string s)
        {
            if (s == "BackSpace")
            {
                if (Current.Count() == 0)
                    return;
                Current = Current.Remove(Math.Max(Current.Count() - 1,0));
                return;
            }
            Current += s;
        }
    }
}
