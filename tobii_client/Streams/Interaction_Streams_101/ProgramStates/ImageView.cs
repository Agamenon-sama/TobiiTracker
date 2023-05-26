using SDL2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker.ProgramStates
{
    class ImageView : ProgramState
    {
        private IntPtr texture;
        private int textureWidth, textureHeight;
        private SDL.SDL_Rect drawRect;

        private const string imageDir = "images/";
        private string[] imagePaths;
        private static int currentImageIndex;
        public ImageView(IntPtr Renderer, int width, int height)
        {
            this.Renderer = Renderer;
            winWidth = width;
            winHeight = height;

            imagePaths = Directory.GetFiles(imageDir); // todo: catch exceptions

            loadTexture();
            currentImageIndex++;
            if (currentImageIndex >= imagePaths.Length)
            {
                currentImageIndex = 0;
            }
        }
        public override void HandleEvents(SDL.SDL_Event ev)
        {
            if (ev.type == SDL.SDL_EventType.SDL_KEYDOWN)
            {
                switch (ev.key.keysym.sym)
                {
                    case SDL.SDL_Keycode.SDLK_RIGHT:
                        if (currentImageIndex < imagePaths.Count() - 1)
                        {
                            currentImageIndex++;
                            loadTexture();
                        }
                        break;
                    case SDL.SDL_Keycode.SDLK_LEFT:
                        if (currentImageIndex > 0)
                        {
                            currentImageIndex--;
                            loadTexture();
                        }
                        break;
                }
            }
        }

        public override void Render()
        {
            SDL.SDL_RenderCopy(Renderer, texture, IntPtr.Zero, ref drawRect);
        }

        public override void Update()
        {
            // nothing to update for now ?
            return;
        }

        private bool loadTexture()
        {
            // free any previously allocated texture. Yes in C#
            if (texture != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(texture);
            }
            // load image
            texture = SDL_image.IMG_LoadTexture(Renderer, imagePaths[currentImageIndex]);
            if (texture == IntPtr.Zero) // report if fail
            {
                // Could change this to a messagebox
                Console.WriteLine("Failed to load image \"{0}\" : {1}", imagePaths[currentImageIndex], SDL.SDL_GetError());
                return false; // means fail
            }

            // get image dimension
            uint udumb = 0;
            int dumb = 0;
            SDL.SDL_QueryTexture(texture, out udumb, out dumb, out textureWidth, out textureHeight);

            // set the size at which we're gonna draw on screen
            if (textureWidth <= winWidth && textureHeight <= winHeight)
            {
                drawRect = new SDL.SDL_Rect
                {
                    x = (winWidth - textureWidth) / 2,
                    y = (winHeight - textureHeight) / 2,
                    w = textureWidth,
                    h = textureHeight
                };
            }
            else
            {
                double imageRatio = (double)textureWidth / textureHeight;
                double winRation = (double)winWidth / winHeight;
                int displayedWidth, displayedHeight; // dimensions used when displaying the image
                if (winRation > imageRatio)
                {
                    displayedWidth = (int)(textureWidth * ((double)winHeight / textureHeight));
                    displayedHeight = winHeight;
                }
                else
                {
                    displayedWidth = winWidth;
                    displayedHeight = (int)(textureHeight * ((double)winWidth / textureWidth));
                }

                drawRect = new SDL.SDL_Rect
                {
                    x = (winWidth - displayedWidth) / 2,
                    y = (winHeight - displayedHeight) / 2,
                    w = displayedWidth,
                    h = displayedHeight
                };
            }

            return true;
        }

        public override void Destroy()
        {
            if (texture != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(texture);
            }
        }
    }
}
