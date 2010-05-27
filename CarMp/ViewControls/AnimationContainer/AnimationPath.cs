using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CarMp.ViewControls
{
    public class AnimationPath
    {
        private AnimationPathPoint _currentStartPoint;
        private System.Drawing.PointF _lastPoint;
        private long _lastTicks;
        private int _increment;
        private AnimationPathPoint _currentEndPoint;
        private readonly List<AnimationPathPoint> PointCollection;
        
        internal AnimationPath()
        {
            PointCollection = new List<AnimationPathPoint>();
            CurrentState = AnimationState.Stopped;
        }

        internal AnimationState CurrentState { get; private set; }

        public void AddAnimationPoint(AnimationPathPoint pAnimationPoint)
        {
            PointCollection.Add(pAnimationPoint);
        }

        public PointF GetCurrentPoint()
        {
            if(CurrentState == AnimationState.Started)
                IncrementAnimation();

            return _lastPoint;
        }

        private AnimationPathPoint NextPoint(AnimationPathPoint pPreviousPoint)
        {
            int index = PointCollection.IndexOf(pPreviousPoint);
            if (index < 0)
                throw new IndexOutOfRangeException("Invalid Point or Point not part of this Animation Path");

            index += _increment;

            if (index < PointCollection.Count)
                return PointCollection[index];
            
            return null;            
        }

        public void AnimationSetForward()
        {
            if (PointCollection.Count < 2)
            {
                throw new Exception("Not enough points to set animation. PointCount: " + PointCollection.Count.ToString());
            }
            _increment = 1;
            _currentEndPoint = PointCollection[1];
            _currentStartPoint = PointCollection[0];
            _lastPoint = _currentStartPoint.Location;
            CurrentState = AnimationState.Set;
        }

        public void AnimationSetReverse()
        {
            if (PointCollection.Count < 2)
            {
                throw new Exception("Not enough points to set animation. PointCount: " + PointCollection.Count.ToString());
            }
            _increment = -1;
            _currentEndPoint = PointCollection[PointCollection.Count - 2];
            _currentStartPoint = PointCollection[PointCollection.Count - 1];
            _lastPoint = _currentStartPoint.Location;
            CurrentState = AnimationState.Set;
        }

        public void AnimationStart()
        {
            CurrentState = AnimationState.Started;
        }

        private void IncrementAnimation()
        {
            if (_currentStartPoint == null
                || _currentEndPoint == null)
            {
                throw new Exception("Animation cannot animate when not fully initialized");
            }

            long currentTicks = DateTime.Now.Ticks;
            _lastPoint = CalculateCurrentPoint(currentTicks);
            _lastTicks = currentTicks;
        }

        private PointF CalculateCurrentPoint(long pTicks)
        {
            // are we there yet?
            if (System.Math.Sqrt(
                System.Math.Pow(_currentEndPoint.Location.X - _lastPoint.X, 2) +
                System.Math.Pow(_currentEndPoint.Location.Y - _lastPoint.Y, 2)
                ) < 2)
            {
                CurrentState = AnimationState.Stopped;
                return _lastPoint;
            }

            //// GEOMETRY AND SHIT
            long diffMs = (pTicks - _lastTicks) / 10000; // 10000 ticks in a ms

            float totalDistance = (float)Math.Sqrt(
                Math.Pow(_currentEndPoint.Location.X - _currentStartPoint.Location.X, 2) +
                Math.Pow(_currentEndPoint.Location.Y - _currentStartPoint.Location.Y, 2)
                );

            float slope = 1;
            if (_currentStartPoint.Location.X != _currentEndPoint.Location.X)
            {
                slope = (_currentEndPoint.Location.Y - _currentStartPoint.Location.Y) /
                 (_currentEndPoint.Location.X - _currentStartPoint.Location.X);
            }

            float angle = (float)Math.Atan(slope);

            float velocity = totalDistance / _currentEndPoint.MoveTime; // pixels per ms (ppm)
            float additionalDistance = velocity * diffMs;

            System.Drawing.PointF point = new System.Drawing.PointF(
                _lastPoint.X + additionalDistance * (float)Math.Cos(angle),
                _lastPoint.Y + additionalDistance * (float)Math.Sin(angle));
            
            return point;
        }

    }
}
