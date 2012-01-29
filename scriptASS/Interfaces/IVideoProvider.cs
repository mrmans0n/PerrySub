using System;
using System.Collections.Generic;
using System.Text;

namespace scriptASS
{
    interface IVideoProvider
    {
        void Play();
        void Pause();
        void Stop();
        int GetCurrentFrame();
        void SetCurrentFrame();
        bool HasAudio();
        bool HasVideo();
    }
}
