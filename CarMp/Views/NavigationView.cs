using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.ViewControls;
using CarMP.Graphics.Geometry;

namespace CarMP.Views
{
    public abstract class NavigationView : D2DView, ISkinable
    {
        private const string XPATH_NAVIGATION_BACKGROUND = "BackgroundImg";
        private const string XPATH_NAVIGATION_NODE = "NavigationBar";
        private const string XPATH_BUTTONS = "Buttons/*";

        public NavigationView(Size pWindowSize)
            : base(pWindowSize) { }

        //public virtual void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        //{
        //    XmlNode navNode = pSkinNode.SelectSingleNode(XPATH_NAVIGATION_NODE);
        //    if (navNode != null)
        //    {
        //        foreach (XmlNode buttonNode in navNode.SelectNodes(XPATH_BUTTONS))
        //        {
        //            GraphicalButton button = new GraphicalButton();
        //            button.ApplySkin(buttonNode, pSkinPath);
        //            AddViewControl(button);
        //            button.StartRender();
        //            switch (buttonNode.Name)
        //            {
        //                case D2DViewFactory.MEDIA:
        //                case D2DViewFactory.HOME:
        //                case D2DViewFactory.OPTIONS:
        //                case D2DViewFactory.NAVIGATION:
        //                    {
        //                        string localVar = buttonNode.Name;
        //                        button.Click += (sender, e) => AppMain.AppFormHost.ShowView(localVar);
        //                        break;
        //                    }
        //            }
        //        }

        //        XmlNode xmlNode = navNode.SelectSingleNode(XPATH_NAVIGATION_BACKGROUND);
                
        //    }
        //}
    }
}
