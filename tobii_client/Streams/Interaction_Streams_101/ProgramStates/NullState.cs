using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker.ProgramStates
{
    class NullState : ProgramState
    {
        public TtfRenderer ttfRenderer;
        private string[] textsToDisplay =
        {
            "Press on the space button when you are ready",
            "End of test. You can close by pressing the escape button or you can restart from the control panel"
        };
        private static int currentText;
        public NullState(IntPtr Renderer, TtfRenderer ttfRenderer, int width, int height)
        {
            this.Renderer = Renderer;
            this.ttfRenderer = ttfRenderer;
            winWidth = width;
            winHeight = height;

            ttfRenderer.RenderText(textsToDisplay[currentText & 1], (uint)(winWidth * 0.8));
            currentText++;
        }
        public override void HandleEvents(SDL.SDL_Event ev)
        {
            // no events to handle
            return;
        }

        public override void Render()
        {
            // draw the text
            SDL.SDL_Rect textRect = new SDL.SDL_Rect
            {
                x = (winWidth - ttfRenderer.getWidth()) / 2, // center the text horizontally
                y = (winHeight - ttfRenderer.getHeight()) / 2, // center vertically
                w = ttfRenderer.getWidth(),
                h = ttfRenderer.getHeight()
            };
            SDL.SDL_RenderCopy(Renderer, ttfRenderer.getTexture(), IntPtr.Zero, ref textRect);
        }

        public override void Update()
        {
            // nothing to update
            return;
        }

        public override void Destroy()
        {
            // nothing to destroy
            return;
        }
    }
}
