using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Pico.Media
{
    //Width x Height x Framerate x BPP = bps
    public static class BitrateCalculator
    {
        /// <summary>
        /// Calculates the bits-per-second
        /// for the specified video parameters.
        /// </summary>
        /// <param name="width">Video width.</param>
        /// <param name="height">Video height.</param>
        /// <param name="framesPerSecond">Video frames-per-second. (FPS)</param>
        /// <param name="bitsPerPixel">Video bits-per-pixel. (BPP)</param>
        /// <returns>Video bits-per-second.</returns>
        public static long VideoBitrate(int width, int height, int framesPerSecond, float bitsPerPixel)
        {
            return (long)((float)width * (float)height * (float)framesPerSecond * bitsPerPixel);
        }
    }
}
