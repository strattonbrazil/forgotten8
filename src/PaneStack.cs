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

        public void Push(Pane pane)
        {
            panes.Add(pane);
        }

        public void Pop()
        {
            panes.RemoveAt(panes.Count - 1);
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
