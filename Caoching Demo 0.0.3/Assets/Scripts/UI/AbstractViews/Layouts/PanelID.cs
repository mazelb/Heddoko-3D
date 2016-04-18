using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.UI.Layouts
{
    public class PanelID
    {
        private Guid mID = Guid.NewGuid();

        public string Id
        {
            get { return mID.ToString(); }
        }


        public override int GetHashCode()
        {
            return mID.GetHashCode();
        }
    }
}
