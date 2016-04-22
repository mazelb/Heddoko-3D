using Assets.Scripts.UI.AbstractViews.Enums;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.Brainpack
{
    public class BrainpackControlPanel : AbstractControlPanel
    {
        public override ControlPanelType PanelType
        {
            get {return ControlPanelType.BrainpackControlPanel; }
        }

        public override void ReleaseResources()
        {
            
        }
    }
}