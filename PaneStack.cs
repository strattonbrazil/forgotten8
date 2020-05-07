using System;
using System.Collections.Generic;

namespace forgotten.Desktop
{
    public class PaneStack
    {
        private static PaneStack instance;
        public List<Pane> panes = new List<Pane>();

        private PaneStack()
        {
        }

        public void push(Pane pane)
        {
            panes.Add(pane);
        }

        public static PaneStack Instance
        {
            get
            {
                if (instance == null)
                    instance = new PaneStack();
                return instance;
            }
        }
    }
}
