using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp.Reactive.Touch
{
    public class Velocity
    {
        public float VelocityX { get; private set; }
        public float VelocityY { get; private set; }
        public float VelocityD { get; private set; }

        public Velocity(float pVelocityX, float pVelocityY, float pVelocityD)
        {
            VelocityX = pVelocityX;
            VelocityY = pVelocityY;
            VelocityD = pVelocityD;
        }
    }
}
