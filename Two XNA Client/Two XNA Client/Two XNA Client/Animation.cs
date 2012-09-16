using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Two_XNA_Client
{
    public abstract class Animation : IDrawable
    {
        public bool Finished = false;
        public virtual void Draw()
        {
            
        }
    }
    public class StartGame : Animation
    {
        public StartGame( TwoClient twoClient )
        {
            twoClient.SoundDict["SHUFFLE"].Play();
            Finished = true;
        }
    }
    public class CardMove : Animation
    {
        private readonly int _startX;
        private readonly int _startY;
        private readonly int _xDist;
        private readonly int _yDist;
        private readonly int _duration;
        private readonly long _startTime;
        private readonly Card _card;
        private readonly TwoClient _twoClient;
        public CardMove( TwoClient twoClient, Card card, int startX, int startY, int endX, int endY, int duration )
        {
            _twoClient = twoClient;
            _startX = startX;
            _startY = startY;
            _card = card;
            _xDist = startX - endX;
            _yDist = startY - endY;
            _duration = duration*10000000;
            _startTime = System.DateTime.Now.Ticks;
        }
        override public void Draw()
        {
            int curX = (int) (_startX - ((System.DateTime.Now.Ticks - _startTime) / _duration) * _xDist);
            float temp = (float)(System.DateTime.Now.Ticks - _startTime)/_duration;
            int curY = (int)(_startY-temp*_yDist); 

            _twoClient.DrawCard(curX, curY, _card, Color.White, 0f, 1.8f);
            if (System.DateTime.Now.Ticks > _startTime + _duration)
                Finished = true;
        }
    }
    public class DelayedCardMove : Animation
    {
        private readonly int _startX;
        private readonly int _startY;
        private readonly int _xDist;
        private readonly int _yDist;
        private readonly int _duration;
        private readonly long _startTime;
        private readonly Card _card;
        private readonly TwoClient _twoClient;
        public DelayedCardMove(TwoClient twoClient, Card card, int startX, int startY, int endX, int endY, float duration, float delay)
        {
            _twoClient = twoClient;
            _startX = startX;
            _startY = startY;
            _card = card;
            _xDist = startX - endX;
            _yDist = startY - endY;
            _duration = (int)(duration * 10000000);
            _startTime = System.DateTime.Now.Ticks + (int)(delay * 10000000);
        }
        override public void Draw()
        {
            if (_startTime > System.DateTime.Now.Ticks)
                return;
            if (System.DateTime.Now.Ticks > _startTime + _duration)
            {
                Finished = true;
                return;
            }
            float temp = (float)(System.DateTime.Now.Ticks - _startTime) / _duration;
            int curX = (int)(_startX - temp * _xDist);
            temp = (float)(System.DateTime.Now.Ticks - _startTime) / _duration;
            int curY = (int)(_startY - temp * _yDist);

            _twoClient.DrawCard(curX, curY, _card, Color.White, 0f, 2.0f);
            if (System.DateTime.Now.Ticks > _startTime + _duration)
                Finished = true;
        }
    }
}
