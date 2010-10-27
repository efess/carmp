using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using System.Xml;
using CarMP.Reactive.Messaging;

namespace CarMP.ViewControls
{
    public class AnimationContainer : Container, ISkinable, IMessageObserver
    {
        private const string XPATH_ANIMATION_POINT = "AnimationPath/*";
        private readonly List<AnimationPath> AnimationPaths;

        private AnimationPath _currentPath;
        private int direction = 0;

        public AnimationContainer()
        {
            AnimationPaths = new List<AnimationPath>();
            _currentPath = null;
        }

        public void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            base.ApplySkin(pSkinNode, pSkinPath);
            ClearAnimations();
            AnimationPath path = this.CreateAnimationPath();
            foreach (XmlNode pointNode in pSkinNode.SelectNodes(XPATH_ANIMATION_POINT))
            {
                AnimationPathPoint point = XmlHelper.GetAnimationPathPoint(pointNode.InnerText);
                if (path.AnimationPointCount == 0)
                    SetLocation(point.Location);
                path.AddAnimationPoint(point);
            }
            this.SetAnimation(-1);
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

        public void ClearAnimations()
        {
            _currentPath = null;
            AnimationPaths.Clear();
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

        protected void SetLocation(Point2F pPoint)
        {
            Bounds = new RectF(pPoint.X, pPoint.Y, Bounds.Width + pPoint.X, Bounds.Height + pPoint.Y);
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);
        }

        protected override void PreRender()
        {
            Point2F currentPoint = _currentPath.GetCurrentPoint();
            SetLocation(currentPoint);
        }

        public IDisposable DisposeUnsubscriber { get; set; }

        public virtual void ProcessMessage(Message pMessage)
        {
            if (pMessage.Type == MessageType.Trigger
                && pMessage.Recipient.Contains(Name))
            {
                SetAnimation(direction);
                StartAnimation();
                //infoBar.SetAnimation(i);
                //infoBar.StartAnimation();
                if (direction > 0)
                    direction = -1;
                else direction = 1;
            }
        }
    }
}
