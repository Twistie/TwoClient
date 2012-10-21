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
    /// <summary>
    /// Card class to store information about card types
    /// </summary>
    public class Card : IComparable, IClickable
    {
        public string Name;
        public double SortValue;
        public List<string> Types = new List<string>();
        public List<string> ValidTypes = new List<string>();
        public Rectangle CardRect;
        public int NumberInDeck;
        private bool MouseOver = false;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of card</param>
        /// <param name="sort">Sort value</param>
        /// <param name="row">Row in graphic file</param>
        /// <param name="column">column in graphic file</param>
        public Card(string name, double sort, int row, int column)
        {
            Name = name;
            SortValue = sort;
            CardRect = new Rectangle(column*128, row*192, 128, 192);
        }
        /// <summary>
        /// Null constructor
        /// </summary>
        public Card()
        {
        }
        /// <summary>
        /// Comparable interface
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns></returns>
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
        /// <summary>
        /// Returns a string of the name of the card
        /// </summary>
        /// <returns>String contains the name of the card</returns>
        public override string ToString()
        {
            return Name;
        }
        /// <summary>
        /// The on click method for the card
        /// </summary>
        /// <param name="twoClient">Needs a link to the client for interaction</param>
        public void Click(TwoClient twoClient)
        {
            twoClient.SendCard(this);
            twoClient.SoundDict["CARD"].Play();
            twoClient.SetGameState(1);
        }
        /// <summary>
        /// Returns the mouseover status of the card button
        /// </summary>
        /// <returns>Boolean mouseover status</returns>
        public bool GetMouseOver()
        {
            return MouseOver;
        }
        /// <summary>
        /// Set the mouseover status of the card
        /// </summary>
        /// <param name="mouseOver">Boolean to set the mouseover status to</param>
        public void SetMouseOver(bool mouseOver)
        {
            MouseOver = mouseOver;
        }
        /// <summary>
        /// Checks whether the card can be played on top of a given card
        /// </summary>
        /// <param name="c">Card to be compared to</param>
        /// <returns>Boolean value of whether a card can be played</returns>
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