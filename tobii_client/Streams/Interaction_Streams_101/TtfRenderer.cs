using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SDL2;

namespace EyeTracker
{
    public class TtfRenderer
    {
        private IntPtr font;
        private IntPtr Renderer;
        private int size;
        private IntPtr textTexture;
        private string text;
        private SDL.SDL_Color color;
        private uint wrapWidth;
        private int width;
        private int height;
        public TtfRenderer(IntPtr renderer, string fontPath, int size)
        {
            Renderer = renderer;
            font = SDL_ttf.TTF_OpenFont(fontPath, size);
            if (font == IntPtr.Zero)
            {
                Console.WriteLine("Can't load ttf font {0}: {1}", fontPath, SDL.SDL_GetError());
            }
            this.size = size;
        }

        public void RenderText(string text, uint wrapWidth)
        {
            if (textTexture == IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(textTexture);
            }

            color = new SDL.SDL_Color
            {
                a = 0xff,
                r = 0xff,
                g = 0xff,
                b = 0xff
            };

            // var textSurface = SDL_ttf.TTF_RenderText_Blended_Wrapped(font, text, color, wrapWidth);
            var textSurface = SDL_ttf.TTF_RenderUTF8_Blended_Wrapped(font, text, color, wrapWidth);
            if ((textTexture = SDL.SDL_CreateTextureFromSurface(Renderer, textSurface)) == IntPtr.Zero)
            {
                Console.WriteLine("Can't create text texture: {0}", SDL.SDL_GetError());
            }

            uint udumb = 0;
            int dumb = 0;
            SDL.SDL_QueryTexture(textTexture, out udumb, out dumb, out width, out height);
            this.text = text;
            this.wrapWidth = wrapWidth;
        }

        public void LoadFont(string fontPath)
        {
            if (font != IntPtr.Zero) // should always be the case actually but just in case
            {
                SDL_ttf.TTF_CloseFont(font);
            }

            font = SDL_ttf.TTF_OpenFont(fontPath, size);
            if (font == IntPtr.Zero)
            {
                Console.WriteLine("Can't load ttf font");
            }
        }

        public void Resize(int size)
        {
            this.size = size;
        }

        public void RerenderText()
        {
            if (textTexture != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(textTexture);
            }

            var textSurface = SDL_ttf.TTF_RenderText_Blended_Wrapped(font, text, color, wrapWidth);
            if ((textTexture = SDL.SDL_CreateTextureFromSurface(Renderer, textSurface)) == IntPtr.Zero)
            {
                Console.WriteLine("Can't create text texture: {0}", SDL.SDL_GetError());
            }

            uint udumb = 0;
            int dumb = 0;
            SDL.SDL_QueryTexture(textTexture, out udumb, out dumb, out width, out height);
        }

        public IntPtr getTexture()
        {
            return textTexture;
        }
        public int getWidth() { return width; }
        public int getHeight() { return height; }
    }
}
