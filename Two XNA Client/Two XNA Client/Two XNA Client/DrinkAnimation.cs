using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Two_XNA_Client
{
    public class DrinkAnimation : Animation
    {
        private Boolean _started = false;
        private long _startTime = 0;
        private readonly TwoClient _twoClient;
        private readonly int _x;
        private readonly int _y;
        private long _duration;
        public DrinkAnimation(TwoClient twoClient, int x, int y, float duration)
        {
            _x = x;
            _y = y;
            _duration = (long)(duration * 10000000);
            _twoClient = twoClient;
        }
        public Boolean Start()
        {
            if (_started != false)
            {
                return false;
            }
            else
            {
                _started = true;
                _startTime = System.DateTime.Now.Ticks;
                return true;
            }
        }

        public void AddTime( float timeToAdd )
        {
            _duration += (long) (timeToAdd*10000000);
        }
        public override void Draw()
        {
            

            _twoClient.SpriteBatch.Draw(_twoClient.DrinkTexture,
                                                new Rectangle(_x, _y,
                                                              250, 250),
                                                new Rectangle(500, 0, 250, 250), Color.White);
            if (!_started)
            {
                _twoClient.SpriteBatch.Draw(_twoClient.DrinkTexture,
                                                new Rectangle(_x, _y,
                                                              250, 250),
                                                new Rectangle(250, 0, 250, 250), Color.White);
            }
            else
            {
                float temp = (float)(System.DateTime.Now.Ticks - _startTime) / _duration;
                int yOffset = (int)(temp * 220);
                _twoClient.SpriteBatch.Draw(_twoClient.DrinkTexture,
                                                new Rectangle(_x, _y + yOffset + 15,
                                                              250, 220 - yOffset),
                                                new Rectangle(250, yOffset + 15, 250, 220 - yOffset), Color.White);
            }
            _twoClient.SpriteBatch.Draw(_twoClient.DrinkTexture,
                                                new Rectangle(_x, _y,
                                                              250, 250),
                                                new Rectangle(0, 0, 250, 250), Color.White);
        }
    }
    public class DrinkGraphic : IDrawable
    {
        private readonly TwoClient _twoClient;
        private readonly DrinkAnimation _drinkAnimation ;
        private int _x, _y;
        public Boolean IsActive { get; set; }
        public Boolean IsRunning { get; set; }
        private long _timeStarted, _duration;

        public DrinkGraphic(TwoClient twoClient, int x, int y, float duration )
        {
            _drinkAnimation = new DrinkAnimation(twoClient,x,y,duration);
            _twoClient = twoClient;
            _duration = (long)(duration*10000000);
            _x = x;
            _y = y;
            IsActive = true;

        }
        public void Draw()
        {
            if (IsRunning)
            {
                if ((System.DateTime.Now.Ticks - _timeStarted) > _duration + 100000)
                    IsActive = false;
            }
            _twoClient.SpriteBatch.Draw(_twoClient.DrinkPlateTexture,
                                                new Rectangle(_x, _y,
                                                              250, 250),
                                                new Rectangle(0, 0, 500, 500), Color.White);
            _drinkAnimation.Draw();
            
        }

        public bool Start()
        {
            _timeStarted = System.DateTime.Now.Ticks;
            IsRunning = true;
            return _drinkAnimation.Start();
        }
        public void AddTime( float timeToAdd )
        {
            _drinkAnimation.AddTime(timeToAdd);
            _duration = _duration + (long)(timeToAdd*10000000);
        }
    }
}
