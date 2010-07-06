using System;

namespace CarMp
{
    public class DebugHandler
    {
        public delegate void DebugString(string text);
        public delegate void DebugException(Exception e);

        public static DebugString Ds;
        public static DebugException De;

        public static void DebugPrint(String debugString)
        {
            if(Ds != null)
            {
                return;
                Ds.Invoke(debugString);
            }
        }

        public static void HandleException(Exception debugException)
        {
            if (De != null)
            {
                De.Invoke(debugException);
            }
        }
    }
}
