using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Two_XNA_Client
{
    
    public interface IClickable
    {
        void Click(TwoClient twoClient);
        bool GetMouseOver();
        void SetMouseOver(bool mouseOver);
    }
}
