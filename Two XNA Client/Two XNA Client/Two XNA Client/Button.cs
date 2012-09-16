using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Two_XNA_Client
{
    public abstract class Button :IClickable, IDrawable
    {

        

        private readonly int _width, _x, _y, _height;
        private float _scale;
        public bool MouseOver, IsActive;
        private readonly string _buttonText;
        private readonly TwoClient _twoClient;
        private readonly Texture2D _butTexture;
        private readonly SpriteFont _font;
        private readonly List<Rectangle> _buttonTextures;
        private Rectangle _boxPos;
        private readonly Vector2 _textPos;
        public Button(int x, int y, int width, float scale, string text, TwoClient twoClient, SpriteFont font, Texture2D butTexture)
        {
            _x = x;
            _y = y;
            _height = 72;
            _width = width;
            _scale = scale;
            MouseOver = false;
            IsActive = false;
            _twoClient = twoClient;
            _buttonText = text;
            _font = font;
            _butTexture = butTexture;
            _buttonTextures = new List<Rectangle>();
            for( int i = 0; i < 3; i ++)
            {
                _buttonTextures.Add(new Rectangle(0,i*128,256,128));
            }
            _boxPos =  new Rectangle(x,y,_width,72);
            _textPos =  new Vector2(_x + _width/2 - _font.MeasureString(_buttonText).X/3, y + 36 - _font.MeasureString(_buttonText).Y*.5f);
            
        }
        public Button(int x, int y, int width, int height, float scale, string text, TwoClient twoClient, SpriteFont font, Texture2D butTexture)
        {
            _x = x;
            _y = y;
            _width = width;
            _scale = scale;
            MouseOver = false;
            IsActive = false;
            _twoClient = twoClient;
            _buttonText = text;
            _font = font;
            _height = height;
            _butTexture = butTexture;
            _buttonTextures = new List<Rectangle>();
            for (int i = 0; i < 3; i++)
            {
                _buttonTextures.Add(new Rectangle(0, i * 128, 256, 128));
            }
            _boxPos = new Rectangle(x, y, _width, height);
            _textPos = new Vector2(_x + _width / 2 - _font.MeasureString(_buttonText).X / 3, y + height/2 - _font.MeasureString(_buttonText).Y * .5f);

        }
        public virtual void Click(TwoClient twoClient)
        {
            _twoClient.SoundDict["CLICK"].Play(.5f,0f,0f);
        }
        public bool GetMouseOver()
        {
            return MouseOver;
        }

        public void SetMouseOver(bool mouseOver)
        {
            MouseOver = mouseOver;
        }
        protected Button()
        {
            
        }

        public virtual void Draw()
        {
            if( IsActive && MouseOver)
            {
                _twoClient.SpriteBatch.Draw(_butTexture,_boxPos,_buttonTextures[1],Color.White );
                _twoClient.SpriteBatch.DrawString(_font, _buttonText,_textPos,Color.White,0f,new Vector2(0,0),new Vector2(.65f,1f),new SpriteEffects(),0 );
                _twoClient.ClickableList.Add(_boxPos,this);
            }
            if (IsActive && !MouseOver)
            {
                _twoClient.SpriteBatch.Draw(_butTexture, _boxPos, _buttonTextures[0], Color.White);
                _twoClient.SpriteBatch.DrawString(_font, _buttonText, _textPos, Color.White, 0f, new Vector2(0, 0), new Vector2(.65f, 1f), new SpriteEffects(), 0);
                _twoClient.ClickableList.Add(_boxPos, this);
            }

            if (!IsActive && !MouseOver)
            {
                _twoClient.SpriteBatch.Draw(_butTexture, _boxPos, _buttonTextures[2], Color.White);
                _twoClient.SpriteBatch.DrawString(_font, _buttonText, _textPos, Color.White, 0f, new Vector2(0, 0), new Vector2(.65f, 1f), new SpriteEffects(), 0);
            }
        }
    }

    internal class DrawButton : Button
    {
        public DrawButton(int x, int y, int width, float scale, string text, TwoClient twoClient, SpriteFont font,
                          Texture2D butTexture) : base(x, y, width, scale, text, twoClient, font, butTexture)
        {
        }

        public override void Click(TwoClient twoClient)

        {
            twoClient.DrawCard();
            base.Click(twoClient);
        }
    }
    class LightButton : Button
    {
        public LightButton(int x, int y, int width, float scale, string text, TwoClient twoClient, SpriteFont font, Texture2D butTexture) : base(x, y, width, scale, text, twoClient, font, butTexture)
        {
        }

        public override void Click(TwoClient twoClient)
        {
            twoClient.SendMessage("LIGHTON");
            base.Click(twoClient);
        }
    }
    public class PlayerSelectButton : IClickable
    {
        public int PlayerNumber;
        public bool IsMouseOver;
        public PlayerSelectButton(int pn)
        {
            IsMouseOver = false;
            PlayerNumber = pn;
        }
        public void Click(TwoClient twoClient)
        {
            twoClient.SendMessage(string.Format("CARDARGS {0}", PlayerNumber.ToString()));
            twoClient.GameState = 1;
            twoClient.SoundDict["CLICK"].Play(.9f,0f,0f);
            
        }

        public bool GetMouseOver()
        {
            return IsMouseOver;
        }

        public void SetMouseOver(bool mouseOver)
        {
            IsMouseOver = mouseOver;
        }
    }
}
