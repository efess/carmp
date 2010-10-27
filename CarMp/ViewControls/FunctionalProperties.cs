using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Callbacks;
using CarMP.ViewControls.Interfaces;
using CarMP.Reactive.Messaging;

namespace CarMP.ViewControls
{
    public class FunctionalProperties
    {
        public void ApplyFunction(ViewControlFunction pFuncion, string pParameter, D2DViewControl pViewControl)
        {
            switch(pFuncion)
            {
                case ViewControlFunction.MediaProgressBar:
                    ApplyProgressBarFunction(pViewControl);
                    break;
                case ViewControlFunction.MediaProgressText:
                    ApplyProgressTextFunction(pViewControl);
                    break;
                case ViewControlFunction.MediaPause:
                    ApplyMediaChangeStateFunction(pViewControl, () => AppMain.MediaManager.PausePlayback());
                    break;
                case ViewControlFunction.MediaPlay:
                    ApplyMediaChangeStateFunction(pViewControl, () => AppMain.MediaManager.StartPlayback());
                    break;
                case ViewControlFunction.MediaStop:
                    ApplyMediaChangeStateFunction(pViewControl, () => AppMain.MediaManager.StopPlayback());
                    break;
                case ViewControlFunction.MediaNext:
                    ApplyMediaChangeStateFunction(pViewControl, () => AppMain.MediaManager.MediaNext());
                    break;
                case ViewControlFunction.MediaPrevious:
                    ApplyMediaChangeStateFunction(pViewControl, () => AppMain.MediaManager.MediaPrevious());
                    break;
                case ViewControlFunction.MediaDisplayName:
                    ApplyMediaDisplayName(pViewControl);
                    break;
                case ViewControlFunction.MediaArt:
                    ApplyMediaArt(pViewControl);
                    break;
                case ViewControlFunction.TriggerToggle:
                    ApplyTriggerToggle(pViewControl, pParameter);
                    break;
                case ViewControlFunction.SwitchView:
                    ApplySwitchView(pViewControl, pParameter);
                    break;
                case ViewControlFunction.MediaGroupHistory:
                    ApplyMediaGroupHistory(pViewControl);
                    break;
                
            }
        }

        private void ApplyMediaGroupHistory(D2DViewControl pViewControl)
        {
            var navigationHistory = pViewControl as INavigationHistory;
            if (navigationHistory == null) return;

            navigationHistory.GetHistorySource = () =>
                {
                    return AppMain.MediaManager.MediaListHistory.Select<MediaHistory, KeyValuePair<int, string>>(
                        (mh) => new KeyValuePair<int, string>(mh.ListIndex, mh.DisplayString));
                };
            navigationHistory.HistoryClick += AppMain.MediaManager.SetList;
        }

        private void ApplySwitchView(D2DViewControl pViewControl, string pParameter)
        {
            var trigger = pViewControl as ITrigger;
            if (trigger == null) return;

            trigger.Trigger += (obj) => AppMain.Messanger.SendMessage(
                new Message(null,
                    MessageType.SwitchView,
                    pParameter));
        }

        private void ApplyTriggerToggle(D2DViewControl pViewControl, string pParameter)
        {
            var trigger = pViewControl as ITrigger;
            if (trigger == null || pParameter == null) return;

            trigger.Trigger += (obj) => AppMain.Messanger.SendMessage(
                new Message(XmlHelper.GetSeparatedList(pParameter),
                    MessageType.Trigger,
                    obj));
        }

        private void ApplyProgressBarFunction(D2DViewControl pViewControl)
        {
            var progressBar = pViewControl as ThermometerProgressBar;
            if (progressBar == null)
                return;

            progressBar.ScrollChanged += (sender, e) =>
            {
                AppMain.MediaManager.SetCurrentPos(Convert.ToInt32(progressBar.Value));
            };

            AppMain.MediaManager.MediaProgressChanged += new MediaProgressChangedHandler(
                (sender, eventArgs) =>
                {
                    progressBar.Value = eventArgs.MediaPosition;
                });

            AppMain.MediaManager.MediaChanged += new MediaChangedHandler(
                (sender, eventArgs) =>
                {
                    progressBar.Value = 0;
                    progressBar.MaximumValue = AppMain.MediaManager.GetSongLength();
                });
            
        }
        private void ApplyProgressTextFunction(D2DViewControl pViewControl)
        {
            var text = pViewControl as Text;
            if (text == null) return;

            AppMain.MediaManager.MediaProgressChanged += new MediaProgressChangedHandler(
                (sender, eventArgs) =>
                {
                    text.TextString = ((eventArgs.MediaPosition / 1000) / 60)
                        + ":"
                        + ((eventArgs.MediaPosition / 1000) % 60).ToString().PadLeft(2, '0');
                });
        }
        private void ApplyMediaChangeStateFunction(D2DViewControl pViewControl, Action pStateChanger)
        {
            var button = pViewControl as IButton;
            if (button == null) return;

            button.Click += (s, e) => pStateChanger();
        }
        private void ApplyMediaDisplayName(D2DViewControl pViewControl)
        {
            var text = pViewControl as IText;
            if (text == null) return;
            AppMain.MediaManager.MediaChanged += (sender, e) =>
            {
                text.TextString = e.MediaDetail.DisplayName; ;
            };
        }
        private void ApplyMediaArt(D2DViewControl pViewControl)
        {
            var commonbase = pViewControl as ViewControlCommonBase;
            if (commonbase == null) return;

            AppMain.MediaManager.MediaChanged += (sender, e) =>
            {
                Direct2D.BitmapData art;
                var artPath = AppMain.MediaManager.GetCurrentSmallAlbumArt();
                if (!string.IsNullOrEmpty(artPath))
                    art = new Direct2D.BitmapData(artPath);
                else art = new Direct2D.BitmapData();

                commonbase.SetBackground(art);
            };
        }
    }
}
