using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using valorant_instalock.Classes;
using valorant_instalock.Models;
using Timer = System.Timers.Timer;

namespace valorant_instalock
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            textbox1.Visibility = Visibility.Hidden;
            btn_Start_Copy1.Visibility = Visibility.Hidden;
        }

        Timer recognitionTimer = new Timer() { Interval = 300 };
        Timer valorantTimer = new Timer() { Interval = 5000 };
        
        IniFile MyIni = new IniFile("agent.ini");

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            recognitionTimer.Elapsed += RecognitionTimer_Elapsed;
            valorantTimer.Elapsed += ValorantTimer_Elapsed;

            if (System.IO.File.Exists("agent.ini"))
            {
                string[] keys = MyIni.ReadAllKeysInSection("Valorant Instalocker");
                foreach (string key in keys)
                {
                    lb_Agents.Items.Add(key);
                }
                //string[] agents = data;
                //foreach (var agent in data)
                //{
                //    lb_Agents.Items.Add(agent);
                //}
            }
        }

        private void ValorantTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Process.GetProcessesByName("VALORANT-Win64-Shipping").Length <= 0)
            {
                StopVoid(false);
            }
        }

        private void RecognitionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var coords = ImageRecognition.GetCoordinates(Properties.Resources.lockImage);
            if (coords != null)
            {
                StopVoid(true);
                MouseController.MoveAndLeftClick(Agent.SelectedAgent.X, Agent.SelectedAgent.Y);
                Thread.Sleep(10);
                for (int i = 0; i < 5; i++)
                {
                    MouseController.LeftClick();
                }
                MouseController.MoveAndLeftClick(coords.X, coords.Y);
                Thread.Sleep(10);
                for (int i = 0; i < 5; i++)
                {
                    MouseController.LeftClick();
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

            Agent.SelectedAgent = Agent.GetAgentCoordinatesByName(lb_Agents.SelectedItem.ToString().ToLower());
            labelXY.Text = "X: " + Agent.SelectedAgent.X + " Y: " + Agent.SelectedAgent.Y;
            labelagent.Text = "Selected Agent: " + Agent.SelectedagentName;
            //lbl_SelectedAgent.Text = Agent.GetAgentNameByCoordinates(Agent.SelectedAgent.X, Agent.SelectedAgent.Y);


        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {

            if (lb_Agents.Items.Count == 0)
            {
                return;
            }

            if (Process.GetProcessesByName("VALORANT-Win64-Shipping").Length > 0)
            {
                valorantTimer.Start();
                btn_Start.IsEnabled = false;
                btn_Stop.IsEnabled = true;
                lbl_Status.Text = "Watching the screen...";
                lbl_Status.Foreground = Brushes.ForestGreen;
                recognitionTimer.Start();
            }
            else MessageBox.Show("Please run Valorant before starting...");
        }

        private void btn_new_Click(object sender, RoutedEventArgs e)
        {
            if (Process.GetProcessesByName("VALORANT-Win64-Shipping").Length > 0)
            {
                btn_Start.IsEnabled = false;
                btn_Stop.IsEnabled = true;
                lbl_Status.Text = "Watching the screen...";
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

                textbox1.Visibility = Visibility.Visible;
                btn_Start_Copy1.Visibility = Visibility.Visible;

                var position = MouseController.GetCursorPosition();

                //Agent.agentCoordinates.Add("default", new Coordinate(position.X, position.Y));

                //string[] agents = Agent.getAgents();
                //foreach (string agent in agents)
                //{
                //    lb_Agents.Items.Add(agent);
                //}

                //if (System.IO.File.Exists("config.ini"))
                //{
                //    System.IO.File.WriteAllText("config.ini", "default=" + position.X + "," + position.Y);
                //}

                Agent.SelectedAgent = new Coordinate(position.X, position.Y);

                labelXY.Text = Agent.SelectedAgent.X + ", " + Agent.SelectedAgent.Y;

                btn_Start.IsEnabled = true;
                btn_Stop.IsEnabled = true;
                lbl_Status.Text = "Done";
                lbl_Status.Foreground = Brushes.Green;

                this.PreviewKeyDown -= MainWindow_PreviewKeyDown;
            }
        }

        private void StopVoid(bool isSuccessful)
        {
            valorantTimer.Stop();
            recognitionTimer.Stop();
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
                    lbl_Status.Text = "Selected and stopped...";
                    lbl_Status.Foreground = Brushes.CornflowerBlue;
                }
            });
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        => StopVoid(false);

        private void textbox1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textbox1.Text != "")
            {
                textbox1.Clear();
            }
        }

        private void textbox1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textbox1.Text == "")
            {
                textbox1.Text = "Agent name";
            }
        }

        private void btn_Start_Copy1_Click(object sender, RoutedEventArgs e)
        {
            //save position
            MyIni.Write(textbox1.Text, Agent.SelectedAgent.X + "," + Agent.SelectedAgent.Y);
            lb_Agents.Items.Clear();

            string[] keys = MyIni.ReadAllKeysInSection("Valorant Instalocker");
            foreach (string key in keys)
            {
                lb_Agents.Items.Add(key);
            }

            textbox1.Visibility = Visibility.Hidden;
            btn_Start_Copy1.Visibility = Visibility.Hidden;
        }
    }
}
