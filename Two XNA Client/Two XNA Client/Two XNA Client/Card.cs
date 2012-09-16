using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Two_XNA_Client
{
    public class Card : IComparable, IClickable
    {
        public string Name;
        public double SortValue;
        public List<string> Types = new List<string>();
        public List<string> ValidTypes = new List<string>();
        public Rectangle CardRect;
        public int NumberInDeck;
        private bool MouseOver = false;
        public Card(string name, double sort, int row, int column)
        {
            Name = name;
            SortValue = sort;
            CardRect = new Rectangle(column*128, row*192, 128, 192);
        }

        public Card()
        {
        }

        int IComparable.CompareTo(object obj)
        {
            Card c = (Card)obj;
            if (SortValue < c.SortValue)
                return 1;

            if (SortValue > c.SortValue)
                return -1;
            else
                return 0;
        }
        public override string ToString()
        {
            return Name;
        }
        public void Click(TwoClient twoClient)
        {
            twoClient.SendCard(this);
            twoClient.SoundDict["CARD"].Play();
            twoClient.SetGameState(1);
        }

        public bool GetMouseOver()
        {
            return MouseOver;
        }

        public void SetMouseOver(bool mouseOver)
        {
            MouseOver = mouseOver;
        }

        public Boolean CanBePlayed(Card c)
        {
            foreach (string s in Types)
            {
                foreach (string vType in c.ValidTypes)
                {
                    if (s == vType)
                        return true;
                }
            }
            return false;
        }
        
    }
}