using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public abstract class Pane : Asset
    {
        public bool IsTopPane()
        {
            var panes = PaneStack.Instance.panes;
            return panes[panes.Count - 1] == this;
        }
    }
}
