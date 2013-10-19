using Caliburn.Micro;
using System;
using System.Linq;

namespace ThePaperWall.WinRT.Fixins
{
    public static class CaliburnEx
    {
        public static T GetInstance<T>(this WinRTContainer This, string key = null)
        {
            return (T)This.GetInstance(typeof(T), key);
        }
    }

}
