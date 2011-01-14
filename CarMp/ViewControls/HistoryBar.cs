using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Geometry;
using CarMP.Reactive.Messaging;
using CarMP.Graphics.Interfaces;

namespace CarMP.ViewControls
{
    public class HistoryBar : NavigationHistoryBase
    {
        private const string XPATH_TEXT_STYLE = "TextStyle";
        private const string XPATH_MAX_ENTRY_WIDTH = "MaxEntryWidth";
        private const string XPATH_SEPARATOR_IMAGE = "SeparatorImg";

        //private readonly List<HistoryText> currentHistory =
        //    new List<HistoryText>();

        private TextStyle textStyle;
        public TextStyle TextStyle
        {
            get { return textStyle; }
            set { textStyle = value; }
        }
        public char Separater { get; set; }
        public int MaxItemPixelLength { get; set; }

        private IImage _separatorImage;
        private string _separatorImagePath; 

        public HistoryBar()
        {
            MaxItemPixelLength = 150;
        }

        public override void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            base.ApplySkin(pSkinNode, pSkinPath);
            
            Helpers.SkinningHelper.XmlTextStyleEntry(XPATH_TEXT_STYLE, pSkinNode, ref textStyle);
            Helpers.SkinningHelper.XmlValidFilePath(XPATH_SEPARATOR_IMAGE, pSkinNode, pSkinPath, ref _separatorImagePath);
        }

        protected override void Push(string pDisplayString, int pListIndex)
        {
            if (_separatorImage == null)
                return;

            float left = 0.0f;
            //foreach (HistoryText text in currentHistory)
            //{
            //    left += text.Width + _separatorImage.Size.Width;
            //}
            //var historyItem = new HistoryText(pListIndex, MaxItemPixelLength, pDisplayString, TextStyle, left);
            //currentHistory.Add(historyItem);
            //AddViewControl(historyItem);
            //historyItem.Click = OnHistoryClick;
        }

        protected override void ClearHistory()
        {
            //currentHistory.Clear();
            Clear();
        }

        public event Action<int> HistoryClick;
        private void OnHistoryClick(int pListIndex)
        {
            if (HistoryClick != null)
            {
                HistoryClick(pListIndex + 1);
            }
        }

        protected override void OnRender(IRenderer pRenderer)
        {
            base.OnRender(pRenderer);

            if (_separatorImage == null
                && !string.IsNullOrEmpty(_separatorImagePath))
            {
                _separatorImage = pRenderer.CreateImage(_separatorImagePath);
            }

            if (_separatorImage == null)
                return;

            //for(int i = 1; i < currentHistory.Count; i++)
            //{
            //    pRenderer.DrawImage(
            //        new Rectangle(currentHistory[i - 1].Bounds.Right,
            //            (currentHistory[i - 1].TextHeight - _separatorImage.Size.Height) / 2,
            //            _separatorImage.Size.Width,
            //            _separatorImage.Size.Height),
            //            _separatorImage,
            //            1f);
            //}
        }

        //private class HistoryText : Text
        //{
        //    protected override void OnTouchGesture(Reactive.Touch.TouchGesture pTouchGesture)
        //    {
        //        if (pTouchGesture.Gesture == Reactive.Touch.GestureType.Click)
        //            OnClick();
        //    }
        //    public Action<int> Click { get; set; }
        //    private void OnClick()
        //    {
        //        if (Click != null)
        //        {
        //            Click(ListIndex);
        //        }
        //    }

        //    public int ListIndex { get; private set; }
        //    public float MaxPixelLength { get; private set; }
        //    public float TextWidth { get; private set; }
        //    public float TextHeight { get; private set; }

        //    public HistoryText(int pListIndex, int pMaxPixelLength, string pText, TextStyle pTextStyle, float pLeftBounds)
        //    {
        //        ListIndex = pListIndex;
        //        MaxPixelLength = pMaxPixelLength;

        //        var textSize = D2DStatic.GetTextPixelSize(pText, pTextStyle);
        //        TextWidth =  Math.Min(textSize.Width, pMaxPixelLength);
        //        TextHeight = textSize.Height;
        //        TextStyle = pTextStyle;
        //        if (false && textSize.Width > pMaxPixelLength)
        //        {
        //            // TODO: Trim  
        //            TextString = pText;
        //        }
        //        else
        //            TextString = pText;

        //        Bounds = new RectF(pLeftBounds, 
        //            0,
        //            pLeftBounds + TextWidth, 
        //            textSize.Height);
        //    }
        //}
    }
}
