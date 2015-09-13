using System;
using System.Collections.Generic;

namespace Eto.Forms
{
    /// <summary>
    /// CommandEventArgs
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// CommandParameter
        /// </summary>
        public object CommandParameter { get; private set; }

        /// <summary>
        /// CommandEventArgs
        /// </summary>
        public CommandEventArgs(object parameter)
        {
            CommandParameter = parameter;
        }
    }
}
