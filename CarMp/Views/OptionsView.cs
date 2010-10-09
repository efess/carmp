using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using CarMP.ViewControls;
using CarMP.ViewControls.OptionControls;

namespace CarMP.Views
{
    public class OptionsView : NavigationView
    {
        private const string OPTION_DETAILS = "OptionDetails";
        private const string OPTIONS_LIST = "OptionsList";
        private const string OPTION_VIEWS = "OptionViews";

        private DragableList _dragableList = new DragableList();
        private ViewControlCommonBase _currentOptionsView;

        private List<D2DViewControl> _optionsViewList;

        public OptionsView(SizeF pWindowSize) : base(pWindowSize)
        {
            _currentOptionsView = new Container();
            _optionsViewList = new List<D2DViewControl>();
            _dragableList = new DragableList();
            _dragableList.SelectedItemChanged += (sender, e) =>
                {
                    SetOptionsControl(_optionsViewList[e.SelectedItem.Index]);
                };

            this.AddViewControl(_currentOptionsView);
            this.AddViewControl(_dragableList);

            PopulateOptions();
            _currentOptionsView.StartRender();
            _dragableList.StartRender();
        }       

        public override string Name
        {
            get { return D2DViewFactory.OPTIONS; }
        }

        public override void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            SkinningHelper.ApplySkinNodeIfExists(OPTIONS_LIST, pSkinNode, pSkinPath, _dragableList);
            SkinningHelper.ApplySkinNodeIfExists(OPTION_DETAILS, pSkinNode, pSkinPath, _currentOptionsView);
            XmlNode node = pSkinNode.SelectSingleNode(OPTION_VIEWS);
            
            if(node != null)
                foreach (D2DViewControl control in _optionsViewList)
                    if(control is ISkinable)
                        SkinningHelper.ApplySkinNodeIfExists((control as IOptionControl).OptionElement, node, pSkinPath, (control as ISkinable));

            base.ApplySkin(pSkinNode, pSkinPath);
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);
        }

        private void SetOptionsControl(D2DViewControl pControl)
        {
            _currentOptionsView.Clear();
            _currentOptionsView.AddViewControl(pControl);
            pControl.Bounds = new RectF(0, 0, _currentOptionsView.Width, _currentOptionsView.Height);
            pControl.StartRender();
        }

        private void PopulateOptions()
        {
            _optionsViewList.Clear();
            _dragableList.Clear();

            var typeOc = typeof(IOptionControl);
            var typeVc = typeof(D2DViewControl);

            foreach (var ctrl in AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeOc.IsAssignableFrom(p)
                    && typeVc.IsAssignableFrom(p)
                    && !p.IsInterface))
            {
                var optionControl = Activator.CreateInstance(ctrl)
                    as IOptionControl;

                _dragableList.InsertNextIntoCurrent(new DragableListTextItem()
                {
                    DisplayString = optionControl.OptionName
                });

                _optionsViewList.Add(optionControl as D2DViewControl);
            }
        }
    }
}
