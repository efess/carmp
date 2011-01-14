using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics;

namespace CarMP.Helpers
{
    public static class GraphicsHelper
    {
        public static bool DisposeIfImplementsIDisposable(object pObjectToDispose)
        {
            if (pObjectToDispose is IDisposable)
            {
                (pObjectToDispose as IDisposable).Dispose();
                return true;
            }

            return false;
        }
        /// <summary>
        /// Converts an array of RGBA values (0-255) to a Color object
        /// </summary>
        /// <param name="pFloatArray">Array of four 0-255 values (RGBA)</param>
        /// <returns></returns>
        public static Color ConvertFloatArrayToColor(float[] pFloatArray)
        {
            return new Color(
                pFloatArray[0] / 256,
                pFloatArray[1] / 256,
                pFloatArray[2] / 256,
                pFloatArray[3] / 256);
        }
    }
}
