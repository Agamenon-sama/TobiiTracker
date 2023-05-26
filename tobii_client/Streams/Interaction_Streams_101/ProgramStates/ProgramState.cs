using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace EyeTracker.ProgramStates
{
    public enum ProgramStateType
    {
        NullState,
        Calibration,
        TextView,
        ImageView
    }
    public abstract class ProgramState
    {
        public IntPtr Renderer;
        public int winWidth, winHeight;
        public abstract void HandleEvents(SDL.SDL_Event ev);
        public abstract void Update();
        public abstract void Render();

        public abstract void Destroy();
    }
}
