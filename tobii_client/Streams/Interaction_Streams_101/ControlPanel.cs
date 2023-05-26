using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDL2;
using SDLCS;
using EyeTracker.ProgramStates;

namespace EyeTracker
{
    public partial class ControlPanel : Form
    {
        private Window window;

        public ControlPanel(ref Window window)
        {
            InitializeComponent();

            this.window = window;
            var fontfiles = Directory.GetFiles("fonts");
            foreach (var font in fontfiles)
            {
                cbxFonts.Items.Add(font);
            }
        }

        private void Recalibrate_Click(object sender, EventArgs e)
        {
            SDL.SDL_Event sdlevent = new SDL.SDL_Event();
            sdlevent.type = SDL.SDL_EventType.SDL_USEREVENT;
            sdlevent.user.code = (int)ProgramStateType.Calibration;
            SDL.SDL_PushEvent(ref sdlevent);
        }

        private void textView_Click(object sender, EventArgs e)
        {
            SDL.SDL_Event sdlevent = new SDL.SDL_Event();
            sdlevent.type = SDL.SDL_EventType.SDL_USEREVENT;
            sdlevent.user.code = (int)ProgramStateType.TextView;
            SDL.SDL_PushEvent(ref sdlevent);
        }

        private void imageView_Click(object sender, EventArgs e)
        {
            SDL.SDL_Event sdlevent = new SDL.SDL_Event();
            sdlevent.type = SDL.SDL_EventType.SDL_USEREVENT;
            sdlevent.user.code = (int)ProgramStateType.ImageView;
            SDL.SDL_PushEvent(ref sdlevent);
        }

        private void btnApplyFonts_Click(object sender, EventArgs e)
        {
            if (cbxFonts.Text != "")
            {
                window.ttfRenderer.LoadFont(cbxFonts.Text);
            }
            window.ttfRenderer.Resize((int)numFontSize.Value);
            window.ttfRenderer.RerenderText();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string filename = txtSaveFileName.Text == "" ? "save" : txtSaveFileName.Text;
            filename += ".etd";
            window.tobiiDevice.SaveData(filename);
        }

        private void ToggleGazeRect_Click(object sender, EventArgs e)
        {
            if (window.DrawGazeRect == true)
            {
                window.DrawGazeRect = false;
            }
            else
            {
                window.DrawGazeRect = true;
            }
        }
    }
}
