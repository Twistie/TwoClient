using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.VisualStyles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Two_XNA_Client
{
    public class Player : IDrawable
    {
        public int Number;
        public string Name;
        public int NumberOfCards = 0;
        public Boolean LightOn = false;
        public int NumberOfDrinks = 0;
        private TwoClient _twoClient;
        public PlayerSelectButton PlayerSelect;
        public Texture2D CurrentPlateTexture;
        public int PlayerPos;
        public Player(int n, string name, TwoClient twoClient)
        {
            Number = n;
            Name = name;
            _twoClient = twoClient;
            PlayerSelect = new PlayerSelectButton(Number);
            CurrentPlateTexture = _twoClient.PlayerTexture;
        }

        public void DrawAsPlayerCards()
        {
            int i = 0;
            int n = NumberOfCards;
            int offset = 40;
            if (_twoClient.GameState == 2 )
            {
                    
                _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(150, _twoClient.WindowHeight - 276,
                                                                128, 256),
                                                new Rectangle(0, 256, 128, 256), Color.White);
                
                _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(_twoClient.WindowWidth - 214,
                                                              _twoClient.WindowHeight - 276,
                                                              64, 256),
                                                new Rectangle(960, 256, 64, 256), Color.White);
                _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(278, _twoClient.WindowHeight - 276,
                                                              _twoClient.WindowWidth - 492, 256),
                                                new Rectangle(128, 256, 832, 256), Color.White);
                
            }
            else
            {
                if (_twoClient.GameState == 3)
                {
                    _twoClient.ClickableList.Add(new Rectangle(150,_twoClient.WindowHeight - 276,_twoClient.WindowWidth - 300,256),PlayerSelect );
                }
                if (!PlayerSelect.IsMouseOver)
                {
                    _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(150, _twoClient.WindowHeight - 276,
                                                              128, 256),
                                                new Rectangle(0, 0, 128, 256), Color.White);
                    _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(_twoClient.WindowWidth - 214,
                                                              _twoClient.WindowHeight - 276,
                                                              64, 256),
                                                new Rectangle(960, 0, 64, 256), Color.White);

                    _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(278, _twoClient.WindowHeight - 276,
                                                              _twoClient.WindowWidth - 492, 256),
                                                new Rectangle(128, 0, 832, 256), Color.White);
                }
                else
                {
                    _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(150, _twoClient.WindowHeight - 276,
                                                              128, 256),
                                                new Rectangle(0, 768, 128, 256), Color.White);
                    _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(_twoClient.WindowWidth - 214,
                                                              _twoClient.WindowHeight - 276,
                                                              64, 256),
                                                new Rectangle(960,768, 64, 256), Color.White);

                    _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(278, _twoClient.WindowHeight - 276,
                                                              _twoClient.WindowWidth - 492, 256),
                                                new Rectangle(128, 768, 832, 256), Color.White);
                }

                
            }
            if (LightOn)
                _twoClient.DrawToken(2, 255, _twoClient.WindowHeight - 265, 0);
            else
                _twoClient.DrawToken(1, 255 , _twoClient.WindowHeight - 265, 0);
            _twoClient.DrawString(Name, _twoClient.WindowWidth - 250, _twoClient.WindowHeight - 280, 0, 1f);
            _twoClient.DrawString(String.Format("Cards: {0}\r\nDrinks: {1}", NumberOfCards.ToString(), NumberOfDrinks.ToString()), _twoClient.WindowWidth - 250, _twoClient.WindowHeight - 260, 0, .66f);

            if (n != 0)
            {
                
                offset = Math.Max(Math.Min((int)(_twoClient.WindowWidth/2 +20 - 300)/((int)Math.Ceiling((decimal)n/2)),50),1);
                
                int y = _twoClient.WindowHeight - 210;
                // ReSharper disable PossibleLossOfFraction
                int leftBoundryCards = (int) ((_twoClient.WindowWidth/2)+20 - offset*((float) n/2) + 25);
                while (i < n)
                {
                    try
                    {
                        if (_twoClient.GameState == 2)
                        {
                            if (_twoClient.PlayerCards[i].CanBePlayed(_twoClient.TopCard))
                            {
                                _twoClient.ClickableList.Add(new Rectangle(leftBoundryCards, y, 80, 120),
                                                             _twoClient.PlayerCards[i]);
                                _twoClient.DrawCard(leftBoundryCards, y, _twoClient.PlayerCards[i], Color.White);
                            }
                            else
                            {
                                _twoClient.DrawCard(leftBoundryCards, y, _twoClient.PlayerCards[i], Color.Gray);
                            }
                        }
                        else
                            _twoClient.DrawCard(leftBoundryCards, y, _twoClient.PlayerCards[i], Color.Gray);
                        i++;
                        leftBoundryCards += offset;
                    }

                    catch (Exception e)
                    {
                        return;
                    }
                }
                _twoClient.DrawMouseoverCard();
            }
            _twoClient.SpriteBatch.DrawString(_twoClient.Sf, _twoClient.PlayerCards.Count.ToString(),
                                   new Vector2(_twoClient.WindowWidth / 2, _twoClient.WindowHeight - 20),
                                   Color.Black);
        }
        public void DrawVerticalPlayer(int xOffset)
        {
            int i = 0;
            int yTop = _twoClient.WindowHeight / 2 - 190;
            int yBot = yTop + 332;
            //DrawBorderedRect(1, 10, 100, yTop, 350);
            if (_twoClient.Turn == Number)
            {
                _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(xOffset, _twoClient.WindowHeight/2 - 190,
                                                                128, 342),
                                                new Rectangle(1280, 0, 192, 512), Color.White);
            }
            else
            {
                if (_twoClient.GameState == 3 || _twoClient.GameState == -2)
                {
                    _twoClient.ClickableList.Add(new Rectangle(xOffset, _twoClient.WindowHeight/2 - 190, 128, 342), PlayerSelect);
                }
                if (!PlayerSelect.IsMouseOver)
                {
                    _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(xOffset, _twoClient.WindowHeight / 2 - 190,
                                                                128, 342),
                                                new Rectangle(1024, 0, 192, 512), Color.White);
                }
                else
                {
                    _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(xOffset, _twoClient.WindowHeight / 2 - 190,
                                                                128, 342),
                                                new Rectangle(1280, 512, 192, 512), Color.White);
                }
            }

            

            if (LightOn)
                _twoClient.DrawToken(2,xOffset + 70, yBot - 50, 0f);
            else
                _twoClient.DrawToken(1, xOffset + 70, yBot - 50, 0f);
            _twoClient.DrawString(Name, xOffset + 1, yBot - 70, 0, 1f, Color.White);
            _twoClient.DrawString(String.Format("Cards: {0}\r\nDrinks: {1}", NumberOfCards.ToString(), NumberOfDrinks.ToString()), xOffset + 1, yBot - 39, 0, .66f, Color.White);
            int n = Math.Min(NumberOfCards, 15);
            int offset = 20;
            if (n > 9)
                offset = 10;
            int x = xOffset + 105;

            int topBoundryCards = (int)((_twoClient.WindowHeight / 2) - offset * ((float)n / 2) - 90);
            while (i < n)
            {
                _twoClient.DrawCard(x, topBoundryCards, _twoClient.TopDown, Color.White, 1.57079633f, 1.2f);

                i++;
                topBoundryCards += offset;
            }
            
        }
        public void DrawOppositePlayer(int x)
        {
            int i = 0;

            if (_twoClient.Turn == Number)
            {

                _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(x-172, 10,
                                                                342, 128),
                                                new Rectangle(1536, 512, 512, 192), Color.White);

            }
            else
            {
                if (_twoClient.GameState == 3 || _twoClient.GameState == -2 )
                {
                    _twoClient.ClickableList.Add(new Rectangle(x-172, 10, 342, 128), PlayerSelect);
                }
                if (!PlayerSelect.IsMouseOver)
                {
                    _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(x-172, 10,
                                                                342, 128),
                                                new Rectangle(1536, 0, 512, 192), Color.White);
                }
                else
                {
                    _twoClient.SpriteBatch.Draw(CurrentPlateTexture,
                                                new Rectangle(x-172, 10,
                                                                342, 128),
                                                new Rectangle(1536, 768, 512, 192), Color.White);
                }


            }
            if (LightOn)
                _twoClient.DrawToken(2, x + 115, 70, 0f);
            else
                _twoClient.DrawToken(1, x + 115, 70, 0f);
            _twoClient.DrawString(Name, x + 48, 45, 0, 1f);
            _twoClient.DrawString(String.Format("Cards: {0}\r\nDrinks: {1}", NumberOfCards.ToString(), NumberOfDrinks.ToString()), x + 48, 75, 0, .66f);

            int n = Math.Min(NumberOfCards, 15);
            int offset = 20;
            if (n > 9)
                offset = 10;
            int y = 115;

            int leftBoundryCards = (int)((x) - offset * ((float)n / 2)-25);
            while (i < n)
            {
                _twoClient.DrawCard(leftBoundryCards, y, _twoClient.TopDown, Color.White, 3.14159265f, 1.2f);

                i++;
                leftBoundryCards += offset;
            }
        }

        public void Draw()
        {
            if (PlayerPos == null)
                return;
            switch(PlayerPos)
            {
                case 0:
                    DrawAsPlayerCards();
                    return;
                case 1:
                    DrawVerticalPlayer(_twoClient.WindowWidth - 138);
                    return;
                case -1:
                    DrawVerticalPlayer(10);
                    return;
                case 2:
                    DrawOppositePlayer(_twoClient.WindowWidth/2);
                    return;
                case 3:
                    DrawOppositePlayer(200);
                    return;
                case 4:
                    DrawOppositePlayer(_twoClient.WindowWidth-200);
                    return;
            }
        }
    }
}