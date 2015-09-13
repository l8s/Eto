using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Forms
{
    public interface ICommandSource
    {
        ICommand Command { get; set; }
        object CommandParameter { get; set; }
        Control CommandTarget { get; set; }
    }
}
