using System;
using Tobii.Interaction;
using SDL2;
using SDLCS;

namespace EyeTracker
{
    /// <summary>
    /// The data streams provide nicely filtered eye-gaze data from the eye tracker 
    /// transformed to a convenient coordinate system. The point on the screen where 
    /// your eyes are looking (gaze point), and the points on the screen where your 
    /// eyes linger to focus on something (fixations) are given as pixel coordinates 
    /// on the screen. The positions of your eyeballs (eye positions) are given in 
    /// space coordinates in millimeters relative to the center of the screen.
    /// 
    /// Let's see how is simple to find out where are you looking at the screen
    /// using GazePoint data stream, accessible from Streams property of Host instance.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
           {
            // initilize SDL subsystems
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine("Can't initilize SDL: {0}", SDL.SDL_GetError());
                return;
            }

            if (SDL_ttf.TTF_Init() < 0)
            {
                Console.WriteLine("Can't initilize SDL_ttf: {0}", SDL.SDL_GetError());
                return;
            }

            // for SDL_image, with the "< 0", I assume that either both png and jpeg load correctly
            // or both fail. This initialization test isn't perfect for my use but now that SDL_image
            // uses stb_image, IMG_init shouldn't fail at all for jpeg and png so I don't care
            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG) <= 0)
            {
                Console.WriteLine("Can't initilize SDL_image: {0}", SDL.SDL_GetError());
                return;
            }

            // create window
            var window = new Window("Application");

            // start the control panel window
            var controlPanel = new ControlPanel(ref window);
            controlPanel.Show();

            // run it
            window.Loop();

            // clean up
            window.Destroy();
            SDL_image.IMG_Quit();
            SDL_ttf.TTF_Quit();
            SDL.SDL_Quit();
        }
    }
}
