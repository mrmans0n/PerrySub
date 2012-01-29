using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;

namespace scriptASS
{
    public class VideoInfo
    {
        private string fileName;
        private double frameRate;
        private int frameTotal;
        private int frameIndex;
        private Size resolution;
        private ArrayList keyFrames;

        public int FrameIndex
        {
            get { return frameIndex; }
            set { frameIndex = value; }
        }

        public double Length
        {
            get { return Math.Round(frameTotal * frameRate,2); }
        }

        public ArrayList KeyFrames
        {
            get { return keyFrames; }
            set { keyFrames = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public double FrameRate
        {
            get { return frameRate; }
            set { frameRate = value; }
        }

        public int FrameTotal
        {
            get { return frameTotal; }
            set { frameTotal = value; }
        }

        public Size Resolution
        {
            get { return resolution; }
            set { resolution = value; }
        }

        public VideoInfo(string Name)
        {
            fileName = Name;
            keyFrames = new ArrayList();
            frameTotal = 1;
            frameIndex = 0;
            resolution = new Size(0, 0);
            frameRate = 0.0;
        }

    }
}
