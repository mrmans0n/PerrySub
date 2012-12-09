using System;
using System.Collections.Generic;
using System.Text;
using DirectShowLib;

namespace scriptASS
{
    class VideoUnitConversion
    {
        #region métodos privados
        private static double convFactor = 10000000;

        private static Boolean isInFrames(IMediaSeeking mediaSeeking)
        {
            return (mediaSeeking.IsFormatSupported(DirectShowLib.TimeFormat.Frame) == 0);
        }

        private static int refTime2frame(long refTime, double fps)
        {
            double inSec = (double)refTime / (convFactor);
            return (int)(inSec * fps);
        }

        private static long frame2refTime(long nFrame, double fps)
        {
            double inSec = (double)nFrame / fps;
            return (long)(inSec * convFactor);
        }
        #endregion
        
        #region métodos públicos

        public static int getCurPos(IMediaSeeking mediaSeeking, double fps)
        {
            if (mediaSeeking == null) return 1;
            long cur;
            mediaSeeking.GetCurrentPosition(out cur);

            if (!isInFrames(mediaSeeking))
                cur = refTime2frame(cur,fps);

            return (int)cur;
        }

        public static int getTotal(IMediaSeeking mediaSeeking, double fps)
        {
            if (mediaSeeking == null) return 1;
            long dur;
            mediaSeeking.GetDuration(out dur);
            if (!isInFrames(mediaSeeking))
                dur = refTime2frame(dur,fps);
            return (int)dur;
        }

        public static void setNewPos(IMediaSeeking mediaSeeking, long newPos, double fps)
        {
            if (mediaSeeking == null) return;
            if (!isInFrames(mediaSeeking))
                newPos = frame2refTime(newPos,fps);
            mediaSeeking.SetPositions(newPos, AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);
        }

    }
    #endregion
}
