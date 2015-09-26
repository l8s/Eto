using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Serialization.Xaml
{
    internal static class Xaml
    {
        public static bool IsMono
        {
            get { return true; }    // EtoEnvironment.Platform.IsMono true only is using custom System.Xaml. 
        }
    }
}
