using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EyeTracker.ProgramStates
{
    class TextView : ProgramState
    {
        TtfRenderer ttfRenderer;
        private const string textDir = "text/";
        private string[] textPaths;
        private static int currentInput;
        public TextView(IntPtr Renderer, int width, int height)
        {
            this.Renderer = Renderer;
            winWidth = width;
            winHeight = height;
            ttfRenderer = new TtfRenderer(Renderer, "fonts/DejaVuSansMono.ttf", 30);

            textPaths = Directory.GetFiles(textDir);

            ttfRenderer.RenderText(
                File.ReadAllText(textPaths[currentInput]),
                (uint)(winWidth * 0.7) // adding some margin
                );
            currentInput++;
            if (currentInput >= textPaths.Length)
            {
                currentInput = 0;
            }
        }
        public override void HandleEvents(SDL.SDL_Event ev)
        {
            // no events to handle
            return;
        }

        public override void Render()
        {
            // draw text
            SDL.SDL_Rect drawRect = new SDL.SDL_Rect
            {
                x = (int)(winWidth * 0.15), // centering horizontally is easy here
                y = (winHeight - ttfRenderer.getHeight()) / 2, // center vertically the normal way
                w = ttfRenderer.getWidth(),
                h = ttfRenderer.getHeight()
            };
            if (SDL.SDL_RenderCopy(Renderer, ttfRenderer.getTexture(), IntPtr.Zero, ref drawRect) < 0)
            {
                Console.WriteLine("Can't draw text {0}", SDL.SDL_GetError());
            }
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
