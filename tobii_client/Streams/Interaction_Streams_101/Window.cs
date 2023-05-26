using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SDL2;
using Tobii.Interaction;
using EyeTracker;
using EyeTracker.ProgramStates;

namespace SDLCS
{
    public class Window
    {
        public IntPtr SDLWindow { get; set; }
        public IntPtr Renderer { get; set; }
        // public SDL.SDL_Event Event { get; set; }
        public int Width;
        public int Height;
        public bool Running;
        public bool DrawGazeRect;
        public SDL.SDL_Rect GazeRect;
        public TobiiDevice tobiiDevice;
        public ProgramState progState;


        public int currentState; // index of current state
        public ProgramStateType[] states = // a list that shows the succession we will follow
        {
            // start with calibration then preparation
            ProgramStateType.Calibration, ProgramStateType.NullState,
            // then 3 sequences of (text, image, image)
            ProgramStateType.TextView, ProgramStateType.ImageView, ProgramStateType.ImageView,
            ProgramStateType.TextView, ProgramStateType.ImageView, ProgramStateType.ImageView,
            ProgramStateType.TextView, ProgramStateType.ImageView, ProgramStateType.ImageView,
            // go back to null state to end tour
            ProgramStateType.NullState
        };

        public TtfRenderer ttfRenderer;

        public Window(string title)
        {
            // create window
            SDLWindow = SDL.SDL_CreateWindow(title,
                SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, // we don't care about position in screen
                1, 1, // default width and height. this will change in fullscreen mode
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP
                );
            if (SDLWindow == IntPtr.Zero)
            {
                Console.WriteLine("Failed to create window");
                return;
            }
            SDL.SDL_GetWindowSize(SDLWindow, out Width, out Height);

            // create renderer
            Renderer = SDL.SDL_CreateRenderer(SDLWindow, -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
                );
            if (Renderer == IntPtr.Zero)
            {
                Console.WriteLine("Failed to create renderer");
                return;
            }
            SDL.SDL_SetRenderDrawBlendMode(Renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
            
            Running = true;

            tobiiDevice = new TobiiDevice(Width);

            GazeRect = new SDL.SDL_Rect
            {
                x = 0,
                y = 0,
                w = 50,
                h = 50
            };
            DrawGazeRect = false;

            // load and render some text to display
            ttfRenderer = new TtfRenderer(Renderer, "fonts/Ubuntu-L.ttf", 22);


            currentState = 0; // set the current state to calibration
            progState = new Calibration(Renderer, ref tobiiDevice, ref ttfRenderer, Width, Height);
        }

        public void Loop()
        {
            while (Running)
            {
                // event handling
                HandleEvents();

                // update
                progState.Update();

                // render
                Render();
            }
        }

        public void HandleEvents()
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event ev) == 1)
            {
                // handle windowing events
                if (ev.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    Running = false;
                }
                else if (ev.type == SDL.SDL_EventType.SDL_KEYDOWN)
                {
                    if (ev.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                    {
                        Running = false;
                    }
                    else if (ev.key.keysym.sym == SDL.SDL_Keycode.SDLK_SPACE)
                    {
                        // if in image view don't react else move to the next state
                        if (states[currentState] != ProgramStateType.ImageView)
                        {
                            currentState++;
                            changeState();
                        }
                    }
                }
                else if (ev.type == SDL.SDL_EventType.SDL_USEREVENT)
                {
                    switch (ev.user.code)
                    {
                        case (int)ProgramStateType.NullState:
                            progState.Destroy();
                            currentState++;
                            progState = new NullState(Renderer, ttfRenderer, Width, Height);
                            break;
                        case (int)ProgramStateType.Calibration:
                            progState.Destroy();
                            currentState = 0;
                            progState = new Calibration(Renderer, ref tobiiDevice, ref ttfRenderer, Width, Height);
                            break;
                        case (int)ProgramStateType.TextView:
                            progState.Destroy();
                            progState = new TextView(Renderer, Width, Height);
                            break;
                        case (int)ProgramStateType.ImageView:
                            progState.Destroy();
                            progState = new ImageView(Renderer, Width, Height);
                            break;
                    }
                }

                // handle the specific events of the state
                progState.HandleEvents(ev);
            }
        }

        public void Render()
        {
            // clear screen
            SDL.SDL_SetRenderDrawColor(Renderer, 0x1e, 0x1e, 0x1e, 0xff);
            SDL.SDL_RenderClear(Renderer);

            // render what the state needs
            progState.Render();

            // draw GazeRect
            /*if (DrawGazeRect)
            {
                GazeRect.x = (int)tobiiDevice.GazePointX - 50 / 2;
                GazeRect.y = (int)tobiiDevice.GazePointY - 50 / 2;
                SDL.SDL_SetRenderDrawColor(Renderer, 0x30, 0xc2, 0xe3, 0x90);
                SDL.SDL_RenderFillRect(Renderer, ref GazeRect);
            }*/

            // swap buffers
            SDL.SDL_RenderPresent(Renderer);
        }
        

        private void changeState()
        {
            if (currentState >= states.Length)
            {
                tobiiDevice.CreateNewItem(ItemDataType.TextData, "nothing really");
                return; // if we reach the end, block at the current state
            }

            if (states[currentState] == ProgramStateType.NullState)
            {
                progState.Destroy(); // destroy the current state
                progState = new NullState(Renderer, ttfRenderer, Width, Height);
                if (tobiiDevice.currentItem >= 9)
                {
                    tobiiDevice.StopRecording();
                }
            }
            else if (states[currentState] == ProgramStateType.TextView)
            {
                progState.Destroy();
                progState = new TextView(Renderer, Width, Height);
                tobiiDevice.CreateNewItem(ItemDataType.TextData, $"{tobiiDevice.currentItem}");
            }
            else if (states[currentState] == ProgramStateType.ImageView)
            {
                progState.Destroy();
                progState = new ImageView(Renderer, Width, Height);
                // wait for 5 seconds then move to next state
                Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(o =>
                {
                    currentState++;
                    changeState();
                });
                tobiiDevice.CreateNewItem(ItemDataType.ImageData, $"{tobiiDevice.currentItem}");
            }
        }
        public void Destroy() {
            tobiiDevice.Destroy();
            SDL.SDL_DestroyRenderer(Renderer);
            SDL.SDL_DestroyWindow(SDLWindow);
        }
    }
}
