using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace PluginForm
{
    using InvokeExtension;

    public partial class Form1 : Form
    {
        bool isRunning;
        bool firstKeyDown;
        bool set;
        Thread t;
        Plugin p;
        Point[] initPoints;

        public Form1()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            Bounds = Screen.PrimaryScreen.Bounds;
            TopMost = true;
            isRunning = false;
            firstKeyDown = false;
            set = false;
            p = new Plugin(this.CreateGraphics());
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
        }

        public void Initialize()
        {
            while (!set) ;

            p.Initialize(initPoints[0], initPoints[1]);
        }

        public void MainLoop()
        {
            Initialize();
            SetBar(100, "Running...");
            
            p.Loop();
        }

        public void SetBar(int i, string s)
        {
            progressBar1.SynchronizedInvoke(() =>
            {
                progressBar1.Value = i;
                label1.Text = s;
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                isRunning = true;
                firstKeyDown = true;
                initPoints = new Point[2];
                t = new Thread(MainLoop);
                t.IsBackground = true;
                t.Start();
                SetBar(25, "Initializing...");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                isRunning = false;
                t.Abort();
                SetBar(0, "Stopped");
                p.Abort();
                Invalidate();
                Refresh();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (isRunning && firstKeyDown && e.KeyCode == Keys.Q)
            {
                initPoints[0].X = MousePosition.X;
                initPoints[0].Y = MousePosition.Y;
                SetBar(50, "X: " + initPoints[0].X + " Y: " + initPoints[0].Y);

                firstKeyDown = false;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (isRunning && e.KeyCode == Keys.Q)
            {
                initPoints[1].X = MousePosition.X;
                initPoints[1].Y = MousePosition.Y;
                SetBar(75, "X: " + initPoints[1].X + " Y: " + initPoints[1].Y);
                
                set = true;
            }
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            Form1_KeyDown(sender, e);
        }

        private void button1_KeyUp(object sender, KeyEventArgs e)
        {
            Form1_KeyUp(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
            Application.Exit();
        }
    }
}

namespace InvokeExtension
{
    public static class MainFormExtension
    {
        public static void SynchronizedInvoke(this Control sync, Action action)
        {
            if (!sync.InvokeRequired)
            {
                action();

                return;
            }

            sync.Invoke(action, new object[] { });
        }
    }
}
