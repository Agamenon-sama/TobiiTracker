using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker
{
    public class TrackingPoint
    {
        public double xpos;
        public double ypos;
        public double timestamp;

        public TrackingPoint() { }
        public TrackingPoint(double x, double y, double ts)
        {
            xpos = x;
            ypos = y;
            timestamp = ts;
        }
    }
    public class FixationData
    {
        public TrackingPoint begin;
        public TrackingPoint end;
        public List<TrackingPoint> data;

        public FixationData()
        {
            begin = new TrackingPoint();
            end = new TrackingPoint();
            data = new List<TrackingPoint>();
        }
    }

    public class SaccadeData
    {
        public TrackingPoint begin;
        public TrackingPoint end;

        public SaccadeData()
        {
            begin = new TrackingPoint();
            end = new TrackingPoint();
        }
    }

    public enum ItemDataType
    {
        TextData, ImageData
    }
    public class ItemData
    {
        public List<FixationData> fixations;
        public List<SaccadeData> saccades;
        public ItemDataType type;
        public string filename;

        public ItemData(ItemDataType type, string filename)
        {
            this.type = type;
            this.filename = filename;
            fixations = new List<FixationData>();
            saccades = new List<SaccadeData>();
        }
    }
}
