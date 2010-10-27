﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.ViewControls;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMP.Views
{
    public class HomeView : D2DView
    {
        
        internal HomeView(SizeF pWindowSize)
            : base(pWindowSize) {}
        
        public override string Name
        {
            get { return D2DViewFactory.HOME; }
        }
    }
}
