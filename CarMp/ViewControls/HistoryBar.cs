using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using CarMP.Reactive.Messaging;

namespace CarMP.ViewControls
{
    public class HistoryBar : NavigationHistoryBase
    {
        private const string XPATH_TEXT_STYLE = "TextStyle";
        private const string XPATH_MAX_ENTRY_WIDTH = "MaxEntryWidth";
        private const string XPATH_SEPARATOR_IMAGE = "SeparatorImg";

        private readonly List<HistoryText> currentHistory =
            new List<HistoryText>();

        private TextStyle textStyle;
        public TextStyle TextStyle
        {
            get { return textStyle; }
            set { textStyle = value; }
        }
        public char Separater { get; set; }
        public int MaxItemPixelLength { get; set; }

        private Direct2D.BitmapData separatorImageData;
        private D2DBitmap separatorImage; 

        public HistoryBar()
        {
            MaxItemPixelLength = 100;
        }

        public override void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            base.ApplySkin(pSkinNode, pSkinPath);
            
            SkinningHelper.XmlTextStyleEntry(XPATH_TEXT_STYLE, pSkinNode, ref textStyle);
            SkinningHelper.XmlBitmapEntry(XPATH_SEPARATOR_IMAGE, pSkinNode, pSkinPath, ref separatorImageData);
        }

        protected override void Push(string pDisplayString, int pListIndex)
        {
            float left = 0.0f;
            foreach (HistoryText text in currentHistory)
            {
                left += text.Width + separatorImageData.Width;
            }
            var historyItem = new HistoryText(pListIndex, MaxItemPixelLength, pDisplayString, TextStyle, left);
            currentHistory.Add(historyItem);
            AddViewControl(historyItem);
            historyItem.Click = OnHistoryClick;
        }

        protected override void ClearHistory()
        {
            currentHistory.Clear();
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

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);

            if (separatorImage == null
                && separatorImageData.Data != null)
            {
                separatorImage = D2DStatic.GetBitmap(separatorImageData, pRenderTarget.Renderer);
            }

            for(int i = 1; i < currentHistory.Count; i++)
            {
                pRenderTarget.DrawBitmap(separatorImage,
                    new RectF(currentHistory[i - 1].Bounds.Right,
                        (currentHistory[i - 1].TextHeight - separatorImageData.Height) / 2,
                        separatorImageData.Width + currentHistory[i-1].Bounds.Right,
                        separatorImageData.Height));
            }
        }

        private class HistoryText : Text
        {
            protected override void OnTouchGesture(Reactive.Touch.TouchGesture pTouchGesture)
            {
                if (pTouchGesture.Gesture == Reactive.Touch.GestureType.Click)
                    OnClick();
            }
            public Action<int> Click { get; set; }
            private void OnClick()
            {
                if (Click != null)
                {
                    Click(ListIndex);
                }
            }

            public int ListIndex { get; private set; }
            public float MaxPixelLength { get; private set; }
            public float TextWidth { get; private set; }
            public float TextHeight { get; private set; }

            public HistoryText(int pListIndex, int pMaxPixelLength, string pText, TextStyle pTextStyle, float pLeftBounds)
            {
                ListIndex = pListIndex;
                MaxPixelLength = pMaxPixelLength;

                var textSize = D2DStatic.GetTextPixelSize(pText, pTextStyle);
                TextWidth =  Math.Min(textSize.Width, pMaxPixelLength);
                TextHeight = textSize.Height;
                TextStyle = pTextStyle;
                if (false && textSize.Width > pMaxPixelLength)
                {
                    // TODO: Trim  
                    TextString = pText;
                }
                else
                    TextString = pText;

                Bounds = new RectF(pLeftBounds, 
                    0,
                    pLeftBounds + TextWidth, 
                    textSize.Height);
            }
        }
    }
}
