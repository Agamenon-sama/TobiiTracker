using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tobii.Interaction;
using Tobii.Interaction.Framework;

namespace EyeTracker
{
    public class TobiiDevice
    {
        public Host TobiiHost { get; set; }
        public GazePointDataStream GazePointDataStream { get; set; }
        public FixationDataStream fixationDataStream;
        public List<FixationData> fixationDatas;
        public List<SaccadeData> saccadeDatas;
        public ItemData[] itemDatas;
        public int currentItem;
        private bool recordData;
        public double GazePointX { get; set; }
        public double GazePointY { get; set; }
        public double[] calibrationX, calibrationY;
        public int winWidth;

        public TobiiDevice(int winWidth)
        {
            this.winWidth = winWidth;
            recordData = false;

            // create calibration arrays
            calibrationX = new double[3];
            calibrationY = new double[3];

            // create necessary tobii objects
            TobiiHost = new Host();
            GazePointDataStream = TobiiHost.Streams.CreateGazePointDataStream();
            fixationDataStream = TobiiHost.Streams.CreateFixationDataStream();

            // create lists of the data to save
            itemDatas = new ItemData[10];
            currentItem = 0;
            fixationDatas = new List<FixationData>();
            saccadeDatas = new List<SaccadeData>();

            // start getting fixation data
            var fixationBeginTime = 0d;
            var fixationPoint = new FixationData();
            var saccadeData = new SaccadeData();

            fixationDataStream.Next += (o, fixation) =>
            {
                var fixationPointX = fixation.Data.X;
                var fixationPointY = fixation.Data.Y;

                if (recordData)
                {
                    switch (fixation.Data.EventType)
                    {
                        case FixationDataEventType.Begin:
                            fixationBeginTime = fixation.Data.Timestamp;
                            Console.WriteLine("Begin fixation at X: {0}, Y: {1}", fixationPointX, fixationPointY);

                            // set new fixation point

                            // Doing this if statement because sometimes I get multiple begins
                            // which I think is a bug in tobii. So I'm keeping the first begin only.
                            if (fixationPoint.begin.timestamp == 0)
                            {
                                fixationPoint.begin.xpos = calibrateX(fixation.Data.X);
                                fixationPoint.begin.ypos = calibrateY(fixation.Data.X, fixation.Data.Y);
                                fixationPoint.begin.timestamp = fixation.Data.Timestamp;
                            }

                            // end previous saccade
                            if (saccadeData.begin.timestamp != 0)
                            {
                                saccadeData.end.xpos = calibrateX(fixation.Data.X);
                                saccadeData.end.ypos = calibrateY(fixation.Data.X, fixation.Data.Y);
                                saccadeData.end.timestamp = fixation.Data.Timestamp;

                                itemDatas[currentItem].saccades.Add(saccadeData);
                                saccadeData = new SaccadeData();
                            }
                            break;

                        case FixationDataEventType.Data:
                            Console.WriteLine("During fixation, currently at X: {0}, Y: {1}", fixationPointX, fixationPointY);
                            fixationPoint.data.Add(
                                new TrackingPoint(
                                    calibrateX(fixation.Data.X),
                                    calibrateY(fixation.Data.X, fixation.Data.Y),
                                    fixation.Data.Timestamp)
                                );
                            break;

                        case FixationDataEventType.End:
                            Console.WriteLine("End fixation at X: {0}, Y: {1}", fixationPointX, fixationPointY);
                            Console.WriteLine("Fixation duration: {0}",
                                fixationBeginTime > 0
                                    ? TimeSpan.FromMilliseconds(fixation.Data.Timestamp - fixationBeginTime)
                                    : TimeSpan.Zero);
                            Console.WriteLine();

                            // set end data
                            fixationPoint.end.xpos = calibrateX(fixation.Data.X);
                            fixationPoint.end.ypos = calibrateY(fixation.Data.X, fixation.Data.Y);
                            fixationPoint.end.timestamp = fixation.Data.Timestamp;

                            // push the total data of the fixation to the list of fixations
                            itemDatas[currentItem].fixations.Add(fixationPoint);

                            // create a new FixationData to store the new fixation
                            fixationPoint = new FixationData();

                            // start new saccade
                            saccadeData.begin.xpos = calibrateX(fixation.Data.X);
                            saccadeData.begin.ypos = calibrateY(fixation.Data.X, fixation.Data.Y);
                            saccadeData.begin.timestamp = fixation.Data.Timestamp;
                            break;
                    }
                }
            };

            // start getting gaze data asynchronously
            GazePointDataStream.GazePoint((x, y, ts) =>
            {
                GazePointX = calibrateX(x);
                GazePointY = calibrateY(x, y);
                /*if (x < (winWidth * 0.2))
                {
                    GazePointX = x - calibrationX[1];
                    GazePointY = y - calibrationX[1];
                }
                else if (x < (winWidth * 0.8))
                {
                    GazePointX = x - calibrationX[0];
                    GazePointY = y - calibrationX[0];
                }
                else
                {
                    GazePointX = x - calibrationX[2];
                    GazePointY = y - calibrationX[2];
                }*/
            });
        }

        public void CreateNewItem(ItemDataType type, string filename)
        {
            currentItem++;
            itemDatas[currentItem] = new ItemData(type, filename);
            recordData = true;
        }
        public void StopRecording()
        {
            recordData = false;
        }

        public void SaveData(string filename) // may throw
        {
            string content = "";

            foreach (var item in itemDatas)
            {
                if (item == null) { continue; }
                // save item data
                content += $"#item {item.type} {item.filename}" + Environment.NewLine + Environment.NewLine;

                // save fixation data
                content += "##fixations" + Environment.NewLine + Environment.NewLine;
                foreach (var fixationPoint in item.fixations)
                {
                    // add the beginning of the fixation
                    content += $"begin x:{fixationPoint.begin.xpos.ToString("0.#####")} " +
                               $"y:{fixationPoint.begin.ypos.ToString("0.#####")} ts:{fixationPoint.begin.timestamp}" +
                               Environment.NewLine;
                    // add all fixation data in data section
                    foreach (var data in fixationPoint.data)
                    {
                        content += $"data x:{data.xpos.ToString("0.#####")} " +
                                   $"y:{data.ypos.ToString("0.#####")} ts:{data.timestamp}" +
                                   Environment.NewLine;
                    }
                    // add the ending of the fixation
                    content += $"end x:{fixationPoint.end.xpos.ToString("0.#####")} " +
                               $"y:{fixationPoint.end.ypos.ToString("0.#####")} ts:{fixationPoint.end.timestamp}" +
                               Environment.NewLine + Environment.NewLine;
                }

                // save saccade data
                content += "##saccades" + Environment.NewLine + Environment.NewLine;
                foreach (var saccade in item.saccades)
                {
                    // add beginning of saccade
                    content += $"begin x:{saccade.begin.xpos.ToString("0.#####")} " +
                               $"y:{saccade.begin.ypos.ToString("0.#####")} ts:{saccade.begin.timestamp}" +
                               Environment.NewLine;
                    // add end of saccade
                    content += $"end x:{saccade.end.xpos.ToString("0.#####")} " +
                               $"y:{saccade.end.ypos.ToString("0.#####")} ts:{saccade.end.timestamp}" +
                               Environment.NewLine + Environment.NewLine;
                }
            }

            // dump data to file
            File.WriteAllText(filename, content);
        }

        private double calibrateX(double x)
        {
            if (x < (winWidth * 0.2))
            {
                return x - calibrationX[1];
            }
            else if (x < (winWidth * 0.8))
            {
                return x - calibrationX[0];
            }
            else
            {
                return x - calibrationX[2];
            }
        }

        private double calibrateY(double x, double y)
        {
            if (x < (winWidth * 0.2))
            {
                return y - calibrationY[1];
            }
            else if (x < (winWidth * 0.8))
            {
                return y - calibrationY[0];
            }
            else
            {
                return y - calibrationY[2];
            }
        }

        public void Destroy()
        {
            TobiiHost.DisableConnection();
        }
    }
}
