using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct2D;
using System.Drawing;

namespace CarMp.ViewControls
{
    public class AnimationContainer : D2DViewControl
    {
        private readonly List<AnimationPath> AnimationPaths;

        private AnimationPath _currentPath;

        public AnimationContainer()
        {
            AnimationPaths = new List<AnimationPath>();
            _currentPath = null;
        }

        public AnimationPath CreateAnimationPath()
        {
            return CreateAnimationPath(AnimationPaths.Count);
        }

        public AnimationPath CreateAnimationPath(int pInsertionIndex)
        {
            AnimationPaths.Insert(pInsertionIndex, new AnimationPath());

            if (_currentPath == null)
                _currentPath = AnimationPaths[0];
            
            return AnimationPaths[pInsertionIndex];
        }

        public void StartAnimation()
        {
            _currentPath.AnimationStart();
        }

        public void SetAnimation(int pDirection)
        {
            if (pDirection > 0)
                _currentPath.AnimationSetForward();
            else
                _currentPath.AnimationSetReverse();
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            PointF currentPoint = _currentPath.GetCurrentPoint();
            Bounds = new RectangleF(currentPoint.X, currentPoint.Y, Bounds.Width, Bounds.Height);
        }
    }
}
