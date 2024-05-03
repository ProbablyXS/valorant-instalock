using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using valorant_instalock.Classes;
using valorant_instalock.Models;
using Brushes = System.Windows.Media.Brushes;
using Timer = System.Timers.Timer;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace valorant_instalock
{
    public partial class MainWindow : Window
    {
        public string processGameName = "VALORANT-Win64-Shipping";

        public PerformanceCounter cpuCounter;
        public PerformanceCounter ramCounter;

        // Get process by name
        private Process GetProcessByName(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            if (processes.Length > 0)
            {
                return processes[0];
            }
            return null;
        }

        // Get window rect
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rectangle rect);

        private static Rectangle GetWindowRect(IntPtr hwnd)
        {
            Rectangle rect = new Rectangle();
            GetWindowRect(hwnd, ref rect);
            return rect;
        }

        public MainWindow()
        {
            InitializeComponent();


            lb_Agents.Items.Clear();
            foreach (string agent in Agent.AgentsList.OrderBy(c => c).ToArray())
            {
                lb_Agents.Items.Add(agent);
            }
        }

        private Thread overlayThread;

        Timer valorantTimer = new Timer() { Interval = 5000 };

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            overlayThread = new Thread(getCpuAndResolutionGame);
            overlayThread.Start();

            lb_Agents.SelectedIndex = 0;
            Agent.SelectedagentName = lb_Agents.SelectedItem.ToString();
            Agent.SelectedAgent = Agent.GetAgentCoordinatesByName(lb_Agents.SelectedItem.ToString().ToLower());
            labelagent.Text = "Selected Agent: " + Agent.SelectedagentName;

            valorantTimer.Elapsed += ValorantTimer_Elapsed;

        }

        public int Xresolution { get; set; }
        public int Yresolution { get; set; }

        private void ValorantTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Process.GetProcessesByName(processGameName).Length <= 0)
            {
                StopVoidAsync(false);
            }
        }

        private async void getCpuAndResolutionGame()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            while (true)
            {
                // Get and display screen resolution of a specific process
                Process process = GetProcessByName(processGameName);
                if (process != null)
                {
                    Rectangle rect = GetWindowRect(process.MainWindowHandle);

                    if (rect.Height >= 1070 && rect.Height <= 1080)
                    {
                        rect.Height = 1080;
                    }

                    if (rect.Height >= 700 && rect.Height <= 720)
                    {
                        rect.Height = 720;
                    }

                    Xresolution = rect.Width;
                    Yresolution = rect.Height;

                    this.Dispatcher.Invoke(() =>
                    {
                        labelCPU.Text = "CPU: " + ((int)cpuCounter.NextValue()) + "%";
                        labelResolution.Foreground = Brushes.Gray;
                        labelResolution.Text = "Resolution: " + Xresolution.ToString() + "x" + Yresolution.ToString();
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        labelResolution.Foreground = Brushes.Red;
                        labelResolution.Text = "Valorant game is not running";
                    });
                }

                await Task.Delay(1000);
            }
        }

        private async void RecognitionTimer_ElapsedAsync()
        {
            await Task.Delay(10);

            while (true)
            {

                Thread.Sleep(500);

                //var bmp = new Bitmap(Image.FromFile("agents/neon/neon_1080p.png"));
                //MessageBox.Show(bmp.Size.ToString());

                var coords = ImageRecognition.GetCoordinates(Image.FromFile("agents/lockImage.png"));
                var agents = ImageRecognition.GetCoordinates(Image.FromFile("agents/" + Agent.SelectedagentName.ToLower() + "/" + Agent.SelectedagentName.ToLower() + "_" + Yresolution + "p.png"));
                if (coords != null && agents != null)
                {
                    MouseController.MoveAndLeftClick(agents.X, agents.Y);

                    Thread.Sleep(100);

                    MouseController.MoveAndLeftClick(coords.X, coords.Y);

                    Thread.Sleep(100);

                    StopVoidAsync(true);
                    return;
                }

            }
        }

        private void lb_Agents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lb_Agents.Items.Count == 0)
            {
                return;
            }

            Agent.SelectedagentName = lb_Agents.SelectedItem.ToString();

            labelagent.Text = "Selected Agent: " + Agent.SelectedagentName;

            var uriSource = new Uri(Environment.CurrentDirectory + "/agents/" + Agent.SelectedagentName.ToLower() + "/" + Agent.SelectedagentName.ToLower() + "_" + Yresolution + "p.png", UriKind.RelativeOrAbsolute);
            pictureBox.Source = new BitmapImage(uriSource);
            //pictureBox.Source = new BitmapImage(new Uri("agents/" + Agent.SelectedagentName.ToLower() + "/" + Agent.SelectedagentName.ToLower() + "_" + Yresolution + "p.png", UriKind.RelativeOrAbsolute));
        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {

            if (lb_Agents.Items.Count == 0)
            {
                return;
            }

            if (Process.GetProcessesByName(processGameName).Length > 0)
            {
                valorantTimer.Start();
                btn_Start.IsEnabled = false;
                btn_Stop.IsEnabled = true;
                lbl_Status.Text = "Watching the screen...";
                lbl_Status.Foreground = Brushes.ForestGreen;
                overlayThread = new Thread(RecognitionTimer_ElapsedAsync);
                overlayThread.Start();
            }
            else MessageBox.Show("Please run Valorant before starting...");
        }

        private void btn_new_Click(object sender, RoutedEventArgs e)
        {
            if (Process.GetProcessesByName(processGameName).Length > 0)
            {
                btn_Start.IsEnabled = false;
                btn_Stop.IsEnabled = false;
                lbl_Status.Text = "Press F10 for screen to point";
                lbl_Status.Foreground = Brushes.ForestGreen;

                // Subscribe to key down event
                this.PreviewKeyDown += MainWindow_PreviewKeyDown;

            }
            else MessageBox.Show("Please run Valorant before starting...");
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Check if F1 key is pressed
            if (e.Key == Key.F1)
            {

                var position = MouseController.GetCursorPosition();

                Agent.SelectedAgent = new Coordinate(position.X, position.Y);

                btn_Start.IsEnabled = true;
                lbl_Status.Text = "Done";
                lbl_Status.Foreground = Brushes.Green;

                this.PreviewKeyDown -= MainWindow_PreviewKeyDown;
            }
        }

        private async Task StopVoidAsync(bool isSuccessful)
        {

            this.Dispatcher.Invoke(() =>
            {
                btn_Stop.IsEnabled = false;
                lbl_Status.Foreground = Brushes.Gray;
                lbl_Status.Text = "Loading...";
            });

            await Task.Delay(500);
            valorantTimer.Stop();
            overlayThread.Join(2000);
            this.Dispatcher.Invoke(() =>
            {
                btn_Start.IsEnabled = true;
                btn_Stop.IsEnabled = false;
                if (!isSuccessful)
                {
                    lbl_Status.Text = "Stopped!";
                    lbl_Status.Foreground = Brushes.Red;
                }
                else
                {
                    lbl_Status.Text = "Sucessfully!";
                    lbl_Status.Foreground = Brushes.Green;
                }
            });
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            StopVoidAsync(false);
        }
    }
}
