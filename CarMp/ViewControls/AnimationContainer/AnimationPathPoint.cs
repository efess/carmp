using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMP.ViewControls
{
    public class AnimationPathPoint
    {
        /// <summary>
        /// Location of control at this point
        /// </summary>
        public Point2F Location { get; private set;}
        /// <summary>
        /// Opacity of controls at this point
        /// </summary>
        public float Opacity { get; private set;}

        /// <summary>
        /// Time between moving from the start point to this end point
        /// </summary>
        public int MoveTime { get; private set; } 

        public AnimationPathPoint(float pX, float pY, int pMoveTime, float pOpacity)
        {
            Location = new Point2F(pX, pY);
            Opacity = pOpacity;
            MoveTime = pMoveTime;
        }
    }
}
