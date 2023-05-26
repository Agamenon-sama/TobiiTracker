using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker.ProgramStates
{
    class Calibration : ProgramState
    {

        public SDL.SDL_Rect CalibrationPoint;
        public double[] calibrationX, calibrationY;
        private int currentCalibrationIndex;
        public List<double> calibratedXValues, calibratedYValues;
        public TobiiDevice tobiiDevice;
        public TtfRenderer ttfRenderer;

        public Calibration(IntPtr Renderer, ref TobiiDevice tobiiDevice, ref TtfRenderer ttfRenderer, int width, int height) {
            this.Renderer = Renderer;
            this.tobiiDevice = tobiiDevice;
            this.ttfRenderer = ttfRenderer;
            winWidth = width;
            winHeight = height;

            ttfRenderer.RenderText("Look at the center of the square", (uint)(winWidth * 0.66));

            CalibrationPoint = new SDL.SDL_Rect
            {
                w = 50,
                h = 50,
                x = winWidth  / 2 - 25,
                y = winHeight / 2 - 25
            };
            calibratedXValues = new List<double>();
            calibratedYValues = new List<double>();
            calibrationX = new double[3];
            calibrationY = new double[3];
            currentCalibrationIndex = 0;
        }
        public override void HandleEvents(SDL.SDL_Event ev)
        {
            // no events to handle
            return;
        }

        private bool calibrate()
        {
            var xpos = tobiiDevice.GazePointX;
            var ypos = tobiiDevice.GazePointY;
            if ((Math.Abs(xpos - CalibrationPoint.x) < 50) && (Math.Abs(ypos - CalibrationPoint.y) < 50))
            {
                calibratedXValues.Add((xpos - CalibrationPoint.x) / 2);
                calibratedYValues.Add((ypos - CalibrationPoint.y) / 2);
            }
            if (calibratedXValues.Count == 100)
            {
                foreach (var value in calibratedXValues)
                {
                    calibrationX[currentCalibrationIndex] += value;
                }
                calibrationX[currentCalibrationIndex] /= calibratedXValues.Count;
                // Console.WriteLine("Calibration value x is {0}", calibrationX[currentCalibrationIndex]);

                foreach (var value in calibratedYValues)
                {
                    calibrationY[currentCalibrationIndex] += value;
                }
                calibrationY[currentCalibrationIndex] /= calibratedYValues.Count;
                // Console.WriteLine("Calibration value y is {0}", calibrationY[currentCalibrationIndex]);

                return true; // means we have finished sampling for this point and can move to the next
            }

            return false; // means that we still need more loop iterations
        }
        public override void Update()
        {
            if (calibrate())
            {
                currentCalibrationIndex++;
                if (currentCalibrationIndex == 1)
                {
                    CalibrationPoint = new SDL.SDL_Rect
                    {
                        w = 50,
                        h = 50,
                        x = (int) ((winWidth * 0.2) / 2 - 25), // center it in the left-most 20% of the screen
                        y = winHeight / 2 - 25
                    };
                    calibratedXValues = new List<double>();
                    calibratedYValues = new List<double>();
                }
                else if (currentCalibrationIndex == 2)
                {
                    CalibrationPoint = new SDL.SDL_Rect
                    {
                        w = 50,
                        h = 50,
                        x = (int)((winWidth * 0.2) / 2 - 25 + (winWidth * 0.8)), // center it in the right-most 20% of the screen
                        y = winHeight / 2 - 25
                    };
                    calibratedXValues = new List<double>();
                    calibratedYValues = new List<double>();
                }
                else if (currentCalibrationIndex > 2)
                {
                    // push event to the global event handler that we're finished with this state
                    SDL.SDL_Event sdlevent = new SDL.SDL_Event();
                    sdlevent.type = SDL.SDL_EventType.SDL_USEREVENT;
                    sdlevent.user.code = (int)ProgramStateType.NullState; // finish calibration state
                    SDL.SDL_PushEvent(ref sdlevent);

                    // update tobii device with the calibration values
                    tobiiDevice.calibrationX = calibrationX;
                    tobiiDevice.calibrationY = calibrationY;
                }
            }
        }

        public override void Render()
        {
            // draw square
            SDL.SDL_SetRenderDrawColor(Renderer, 0xe1, 0xe1, 0xe1, 0xff);
            SDL.SDL_RenderFillRect(Renderer, ref CalibrationPoint);


            // draw the text
            SDL.SDL_Rect textRect = new SDL.SDL_Rect
            {
                x = (winWidth - ttfRenderer.getWidth()) / 2, // center the text
                y = (int) (winHeight * 0.8), // put at 80% height
                w = ttfRenderer.getWidth(),
                h = ttfRenderer.getHeight()
            };
            SDL.SDL_RenderCopy(Renderer, ttfRenderer.getTexture(), IntPtr.Zero, ref textRect);
        }

        public override void Destroy()
        {
            // nothing to destroy
            return;
        }
    }
}
