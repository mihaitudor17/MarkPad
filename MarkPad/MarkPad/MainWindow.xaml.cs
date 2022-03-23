using MarkPad.ViewModels;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace MarkPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int count = 2;
        string pageTitle = "Untitled note ";
        string pageText = "";
        int Wcount = 1;
        private bool P1Up = false;
        private bool P1Down = false;
        private bool P2Up = false;
        private bool P2Down = false;

        //Timer
        private DispatcherTimer PlayerMovementTimer = new DispatcherTimer();
        private DispatcherTimer AnimationTimer = new DispatcherTimer();

        //BallSpeed
        private double BeginnSpeedX = 140;
        private double BeginnSpeedY = 220;
        private double SpeedX = 140;
        private double SpeedY = 220;

        //Player Counts
        private int PlayerLeftCount = 0;
        private int PlayerRightCount = 0;

        //Ball Starting Position
        private double BeginnBallX = 200;
        private double BeginnBallY = 150;

        //Cooldown to prevent recollision with paddle
        private bool LeftCooldown = false;
        private bool RightCooldown = false;

        public MainWindow()
        {

            InitializeComponent();
            pageTitle += count;
            DataContext = new DirectoryStructureViewModel();
        }

        private void Save_As(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".text";  // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            System.Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                var filename = dlg.FileName;
                TextBox temp = (TextBox)tabControl.SelectedContent;
                string content = temp.Text;
                using (StreamWriter sw = File.CreateText(filename))
                {
                    sw.Write(content);
                }
                TabItem tabitem = (TabItem)tabControl.SelectedItem;
                tabitem.Tag = filename;
                StackPanel st = (StackPanel)tabitem.Header;
                TextBlock tb = new TextBlock();
                tb.Margin = new Thickness { Right = 8 };
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.Text = System.IO.Path.GetFileName(dlg.FileName);
                tb.Text = tb.Text.Replace(".txt", "");
                tb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                st.Children.RemoveAt(0);
                st.Children.Insert(0, tb);
            }
        }
        private void Save(object sender, RoutedEventArgs e)
        {
            TabItem tabitem = (TabItem)tabControl.SelectedItem;
            StackPanel st = (StackPanel)tabitem.Header;
            TextBlock tb = (TextBlock)st.Children[0];
            if ((string)tabitem.Tag != "")
            {
                TextBox tb2 = new TextBox();
                tb2 = (TextBox)tabitem.Content;
                string content = tb2.Text;
                using (StreamWriter sw = File.CreateText((string)tabitem.Tag))
                {
                    sw.Write(content);
                }
            }
            else
            {
                Save_As(sender, e);
            }
        }
        private void Exit(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Do you want to save your work?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                Save_As(sender, e);
                System.Windows.Application.Current.Shutdown();
            }
        }
        private void Open(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt|All files (*.*)|*.*"; // Filter files by extension
            System.Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                using (StreamReader sr = new StreamReader(filename))
                {
                    pageTitle = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                    pageText = sr.ReadToEnd();
                    Add_Page(sender, e);
                }
            }
        }

        private void Add_Page(object sender, RoutedEventArgs e)
        {
            TabItem item = new TabItem();
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            TextBlock tb = new TextBlock();
            tb.Text = pageTitle;
            tb.Margin = new Thickness { Right = 8 };
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.CadetBlue);
            stack.Children.Add(tb);

            Button btn1 = new Button();
            btn1.Padding = new Thickness(2);
            btn1.Background = null;
            btn1.Content = " x ";
            btn1.FontSize = 15;
            btn1.BorderThickness = new Thickness(0);
            btn1.VerticalAlignment = VerticalAlignment.Top;
            btn1.Click += Delete_Page;
            stack.Children.Add(btn1);

            Button btn2 = new Button();
            btn2.Padding = new Thickness(2);
            btn2.Background = null;
            btn2.BorderThickness = new Thickness { Left = 1 };
            btn2.Content = "+";
            btn2.FontSize = 15;
            btn2.VerticalAlignment = VerticalAlignment.Top;
            btn2.Click += Add_Page;
            stack.Children.Add(btn2);
            item.Header = stack;

            TextBox tb2 = new TextBox();
            tb2.VerticalAlignment = VerticalAlignment.Stretch;
            tb2.HorizontalAlignment = HorizontalAlignment.Stretch;
            tb2.TextWrapping = TextWrapping.Wrap;
            tb2.Text = pageText;
            tb2.AcceptsTab = true;
            tb2.AcceptsReturn = true;
            item.Content = tb2;

            if (tb.Text.Contains("Untitled note"))
                count++;
            pageTitle = "Untitled note " + count;
            pageText = "";
            item.Tag = "";
            tabControl.Items.Add(item);
        }

        private void Delete_Page(object sender, RoutedEventArgs e)
        {
            if (tabControl.Items.Count > 1)
            {
                var target = (FrameworkElement)sender;
                while (target is TabItem == false)
                    target = (FrameworkElement)target.Parent;
                TabItem item = (TabItem)target;
                TextBox temp = (TextBox)item.Content;
                if (temp.Text != "" && item.Tag == "")
                {
                    if (MessageBox.Show("Do you want to save the document?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                        dlg.FileName = "Document"; // Default file name
                        dlg.DefaultExt = ".text";  // Default file extension
                        dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
                        System.Nullable<bool> result = dlg.ShowDialog();
                        if (result == true)
                        {
                            // Save document
                            var filename = dlg.FileName;
                            string content = temp.Text;
                            using (StreamWriter sw = File.CreateText(filename))
                            {
                                sw.Write(content);
                            }
                        }
                    }
                }
                tabControl.Items.Remove(item);
            }
        }
        private void StackPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(@"\(\w:\\\)");
                string filename = SelectedView.Text;
                if (!re.IsMatch(filename))
                {
                    FileAttributes attr = File.GetAttributes(filename);
                    if (!attr.HasFlag(FileAttributes.Directory))
                        using (StreamReader sr = new StreamReader(filename))
                        {
                            pageTitle = System.IO.Path.GetFileNameWithoutExtension(filename);
                            pageText = sr.ReadToEnd();
                            Add_Page(sender, e);
                        }
                }
            }
        }

        private void About(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to go to the developer's webpage?", "About", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start("https://github.com/mihaitudor17");
            }
        }
        private void Find(TextBox textBox, bool check)
        {
            if (check)
            {
                int ok = 0;
                foreach (TabItem item in tabControl.Items)
                {
                    TextBox temp = (TextBox)item.Content;
                    temp.IsInactiveSelectionHighlightEnabled = true;

                    bool found = temp.Text.ToLower().IndexOf(textBox.Text) >= 0;
                    int index = temp.Text.ToLower().IndexOf(textBox.Text);
                    int tempcount = 1;
                    do
                    { if (found)
                        {
                            temp.Focus();
                            temp.SelectionStart = index;
                            temp.SelectionLength = textBox.Text.Length;
                            temp.SelectionBrush = System.Windows.Media.Brushes.Red;
                            found = temp.Text.ToLower().IndexOf(textBox.Text, index + 1) >= 0;
                            index = temp.Text.ToLower().IndexOf(textBox.Text, index + 1);
                            tempcount++;
                        }
                    } while (found && tempcount < Wcount);
                    if (!found && tempcount < Wcount)
                        ok += 0;
                    else
                        ok += 1;
                    

                }
                if(ok== 0)
                    Wcount--;
            }
            else
            {
                TextBox temp = (TextBox)tabControl.SelectedContent;
                temp.IsInactiveSelectionHighlightEnabled = true;
                bool found = temp.Text.ToLower().IndexOf(textBox.Text) >= 0;
                int index = temp.Text.ToLower().IndexOf(textBox.Text);
                int tempcount = 1;
                do
                {
                    if (found)
                    {
                        temp.Focus();
                        temp.SelectionStart = index;
                        temp.SelectionLength = textBox.Text.Length;
                        temp.SelectionBrush = System.Windows.Media.Brushes.Red;
                        found = temp.Text.ToLower().IndexOf(textBox.Text, index + 1) >= 0;
                        index = temp.Text.ToLower().IndexOf(textBox.Text, index + 1);
                        tempcount++;
                    }
                } while (found && tempcount < Wcount);
                if (!found && tempcount < Wcount)
                    Wcount--;
            }
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Window dialog = (Window)sender;
            dialog.Close();
        }
        private void Find(object sender, RoutedEventArgs e)
        {
            var dialog = new Window();
            dialog.Width = 300;
            dialog.Height = 150;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            StackPanel sp = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Find: ";
            text.Margin = new Thickness(10, 0, 0, 0);
            sp.Children.Add(text);
            TextBox textBox = new TextBox();
            textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.Margin = new Thickness(10);
            sp.Children.Add(textBox);
            CheckBox checkBox = new CheckBox();
            checkBox.Content = "Search in all files";
            checkBox.Margin = new Thickness(10, 0, 0, 0);
            sp.Children.Add(checkBox);
            StackPanel sp2 = new StackPanel();
            sp2.Orientation = Orientation.Horizontal;
            var button = new Button();
            button.Content = "  Find Next  ";
            button.Padding = new Thickness(10, 5, 10, 5);
            button.Margin = new Thickness(35, 10, 40, 0);
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.Click += (s, f) => {
                textBox.TextChanged += (S, F) => Wcount = 1;
                Wcount++;
                Find(textBox, (bool)checkBox.IsChecked);

                textBox.TextChanged += (S, F) => Wcount = 1;
                dialog.Activate();
            };
            sp2.Children.Add(button);
            var button2 = new Button();
            button2.Content = "Find Previous";
            button2.Padding = new Thickness(10, 5, 10, 5);
            button2.Margin = new Thickness(0, 10, 80, 0);
            button2.HorizontalAlignment = HorizontalAlignment.Right;
            button2.Click += (s, f) => {
                textBox.TextChanged += (S, F) => Wcount = 1;
                if (Wcount > 2)
                    Wcount--;
                Find(textBox, (bool)checkBox.IsChecked);
                textBox.TextChanged += (S, F) => Wcount = 1;
                dialog.Activate();
            };
            sp2.Children.Add(button2);
            sp.Children.Add(sp2);
            dialog.Content = sp;
            dialog.Show();
        }
        private void Replace(TextBox old, TextBox nou, bool check)
        {
            if (check)
            {
                foreach (TabItem item in tabControl.Items)
                {
                    TextBox temp = (TextBox)item.Content;
                    bool found = temp.Text.ToLower().IndexOf(old.Text) >= 0;
                    if (found)
                    {
                        int index = temp.Text.ToLower().IndexOf(old.Text);
                        temp.Text = temp.Text.Remove(index, old.Text.Length).Insert(index, nou.Text);
                    }
                }
            }
            else
            {
                TextBox temp = (TextBox)tabControl.SelectedContent;
                temp.IsInactiveSelectionHighlightEnabled = true;
                bool found = temp.Text.ToLower().IndexOf(old.Text) >= 0;
                if (found)
                {
                    int index = temp.Text.ToLower().IndexOf(old.Text);
                    temp.Text = temp.Text.Remove(index, old.Text.Length).Insert(index, nou.Text);
                }
            }
        }
        private void Replace(object sender, RoutedEventArgs e)
        {
            var dialog = new Window();
            dialog.Width = 350;
            dialog.Height = 200;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            StackPanel sp = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Replace: ";
            text.Margin = new Thickness(10, 0, 0, 0);
            sp.Children.Add(text);
            TextBox textBox = new TextBox();
            textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.Margin = new Thickness(10);
            sp.Children.Add(textBox);
            StackPanel sp3 = new StackPanel();
            TextBlock text1 = new TextBlock();
            text1.Text = "With: ";
            text1.Margin = new Thickness(10, 0, 0, 0);
            sp3.Children.Add(text1);
            TextBox textBox1 = new TextBox();
            textBox1.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBox1.TextWrapping = TextWrapping.Wrap;
            textBox1.Margin = new Thickness(10);
            sp3.Children.Add(textBox1);
            sp.Children.Add(sp3);
            CheckBox checkBox = new CheckBox();
            checkBox.Content = "Replace in all files";
            checkBox.Margin = new Thickness(10, 0, 0, 0);
            sp.Children.Add(checkBox);
            StackPanel sp2 = new StackPanel();
            sp2.Orientation = Orientation.Horizontal;
            var button = new Button();
            button.Content = "Replace";
            button.Padding = new Thickness(10, 5, 10, 5);
            button.Margin = new Thickness(70, 10, 40, 0);
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.Click += (s, f) => Replace(textBox, textBox1, (bool)checkBox.IsChecked);
            sp2.Children.Add(button);
            var button1 = new Button();
            button1.Content = "Cancel ";
            button1.Padding = new Thickness(10, 5, 10, 5);
            button1.Margin = new Thickness(0, 10, 80, 0);
            button1.HorizontalAlignment = HorizontalAlignment.Right;
            button1.Click += (s, f) => Close(dialog, f);
            sp2.Children.Add(button1);
            sp.Children.Add(sp2);
            dialog.Content = sp;
            dialog.Show();
        }
        private void Replace_All(TextBox old, TextBox nou, bool check)
        {
            if (check)
            {
                foreach (TabItem item in tabControl.Items)
                {
                    TextBox temp = (TextBox)item.Content;
                    bool found = temp.Text.ToLower().IndexOf(old.Text) >= 0;
                    int index = temp.Text.ToLower().IndexOf(old.Text);
                    while (found)
                    {
                        temp.Text = temp.Text.Remove(index, old.Text.Length).Insert(index, nou.Text);
                        found = temp.Text.ToLower().IndexOf(old.Text, index + 1) >= 0;
                        index = temp.Text.ToLower().IndexOf(old.Text, index + 1);
                    }
                }
            }
            else
            {
                TextBox temp = (TextBox)tabControl.SelectedContent;
                temp.IsInactiveSelectionHighlightEnabled = true;
                bool found = temp.Text.ToLower().IndexOf(old.Text) >= 0;
                int index = temp.Text.ToLower().IndexOf(old.Text);
                while (found)
                {
                    temp.Text = temp.Text.Remove(index, old.Text.Length).Insert(index, nou.Text);
                    found = temp.Text.ToLower().IndexOf(old.Text, index + 1) >= 0;
                    index = temp.Text.ToLower().IndexOf(old.Text, index + 1);
                }
            }

        }
        private void Replace_All(object sender, RoutedEventArgs e)
        {
            var dialog = new Window();
            dialog.Width = 350;
            dialog.Height = 200;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            StackPanel sp = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Replace all: ";
            text.Margin = new Thickness(10, 0, 0, 0);
            sp.Children.Add(text);
            TextBox textBox = new TextBox();
            textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.Margin = new Thickness(10);
            sp.Children.Add(textBox);
            StackPanel sp3 = new StackPanel();
            TextBlock text1 = new TextBlock();
            text1.Text = "With: ";
            text1.Margin = new Thickness(10, 0, 0, 0);
            sp3.Children.Add(text1);
            TextBox textBox1 = new TextBox();
            textBox1.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBox1.TextWrapping = TextWrapping.Wrap;
            textBox1.Margin = new Thickness(10);
            sp3.Children.Add(textBox1);
            sp.Children.Add(sp3);
            CheckBox checkBox = new CheckBox();
            checkBox.Content = "Replace in all files";
            checkBox.Margin = new Thickness(10, 0, 0, 0);
            sp.Children.Add(checkBox);
            StackPanel sp2 = new StackPanel();
            sp2.Orientation = Orientation.Horizontal;
            var button = new Button();
            button.Content = "Replace";
            button.Padding = new Thickness(10, 5, 10, 5);
            button.Margin = new Thickness(70, 10, 40, 0);
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.Click += (s, f) => Replace_All(textBox, textBox1, (bool)checkBox.IsChecked);
            sp2.Children.Add(button);
            var button1 = new Button();
            button1.Content = "Cancel ";
            button1.Padding = new Thickness(10, 5, 10, 5);
            button1.Margin = new Thickness(0, 10, 80, 0);
            button1.HorizontalAlignment = HorizontalAlignment.Right;
            button1.Click += (s, f) => Close(dialog, f);
            sp2.Children.Add(button1);
            sp.Children.Add(sp2);
            dialog.Content = sp;
            dialog.Show();
        }
        private void To_Upper(object sender, RoutedEventArgs e)
        {
            TabItem item = (TabItem)tabControl.SelectedItem;
            TextBox textBox = (TextBox)item.Content;
            textBox.SelectedText = textBox.SelectedText.ToUpper();
        }
        private void To_Lower(object sender, RoutedEventArgs e)
        {
            TabItem item = (TabItem)tabControl.SelectedItem;
            TextBox textBox = (TextBox)item.Content;
            textBox.SelectedText = textBox.SelectedText.ToLower();
        }
        private void Set_RO(object sender, RoutedEventArgs e)
        {
            TabItem item = (TabItem)tabControl.SelectedItem;
            TextBox textBox = (TextBox)item.Content;
            textBox.IsReadOnly = true;
        }
        private void Unset_RO(object sender, RoutedEventArgs e)
        {
            TabItem item = (TabItem)tabControl.SelectedItem;
            TextBox textBox = (TextBox)item.Content;
            textBox.IsReadOnly = false;
        }
        private void Remove_empty(object sender, RoutedEventArgs e)
        {
            TabItem item = (TabItem)tabControl.SelectedItem;
            TextBox textBox = (TextBox)item.Content;
            textBox.Text = Regex.Replace(textBox.Text, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);

        }
        private void GoToLine(object sender, RoutedEventArgs e)
        {
            var dialog = new Window();
            dialog.Width = 350;
            dialog.Height = 150;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            StackPanel sp = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Go to line: ";
            text.Margin = new Thickness(10, 0, 0, 0);
            sp.Children.Add(text);
            TextBox textBox = new TextBox();
            textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.Margin = new Thickness(10);
            sp.Children.Add(textBox);
            Button button=new Button();
            button.Content = "  Go  ";
            button.Padding = new Thickness(10, 5, 10, 5);
            button.Margin = new Thickness(70, 10, 40, 0);
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.Click += (s, f) =>
            {
                TabItem item = (TabItem)tabControl.SelectedItem;
                TextBox txtBox = (TextBox)item.Content;
                string str = txtBox.Text;
                int Ncount = 1;
                for(int i=0; i<str.Length; i++)
                {
                    if(str[i] == '\n')
                        Ncount++;
                    int temp;
                    int.TryParse(textBox.Text, out temp);
                    if (temp-1 == Ncount)
                    {
                        
                        txtBox.Select (i + 1,0);
                        txtBox.ScrollToLine(temp - 1);
                        txtBox.Focus();
                    }
                }
            };
            StackPanel sp2 = new StackPanel();
            sp2.Orientation = Orientation.Horizontal;
            sp2.Children.Add(button);
            var button1 = new Button();
            button1.Content = "Cancel ";
            button1.Padding = new Thickness(10, 5, 10, 5);
            button1.Margin = new Thickness(25, 10, 80, 0);
            button1.HorizontalAlignment = HorizontalAlignment.Right;
            button1.Click += (s, f) => Close(dialog, f);
            sp2.Children.Add(button1);
            sp.Children.Add(sp2);
            dialog.Content = sp;
            dialog.Show();
            
        }
        private void Stop_Game(object sender, RoutedEventArgs e)
        {
            Gamefield.Visibility = Visibility.Hidden;
            TextItem.Focus();
            Keyboard.Focus(TextItem);
            PlayerMovementTimer.Stop();
            AnimationTimer.Stop();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!AnimationTimer.IsEnabled)
            {
                Gamefield.Visibility = Visibility.Visible;
                LeftCount.Content = 0;
                RightCount.Content = 0;
                Gamefield.Focus();
                Keyboard.Focus(Gamefield);
                PlayerMovementTimer.Interval = TimeSpan.FromMilliseconds(10);
                PlayerMovementTimer.Tick += MovePlayer;
                PlayerMovementTimer.Start();

                //AnimationTimer Initialization
                AnimationTimer.Interval = TimeSpan.FromSeconds(0.005);
                AnimationTimer.Tick += Animate;
                AnimationTimer.Start();
            }
            else
                Stop_Game(sender, e);
        }
        private void Animate(object sender, EventArgs e)
        {
            var x = Canvas.GetLeft(Ball);
            var y = Canvas.GetTop(Ball);

            //Ball collision with left paddle
            if (x <= LeftPaddle.Width
                && y >= Canvas.GetTop(LeftPaddle)
                && y + Ball.Height <= Canvas.GetTop(LeftPaddle) + LeftPaddle.Height
                && !LeftCooldown)
            {
                SpeedX = -SpeedX;
                LeftCooldown = true;
                RightCooldown = false;
            }
            //Ball collision with right paddle
            if (x + Ball.Width >= Gamefield.ActualWidth - (Canvas.GetRight(RightPaddle) + RightPaddle.Width)
                && y + RightPaddle.Width >= Canvas.GetTop(RightPaddle)
                && y + Ball.Height <= Canvas.GetTop(RightPaddle) + RightPaddle.Height
                && !RightCooldown)
            {
                SpeedX = -SpeedX;
                RightCooldown = true;
                LeftCooldown = false;
            }

            //Ball collision with walls
            if (y <= 0.0 || y >= Gamefield.ActualHeight - Ball.Height)
            {
                SpeedY = -SpeedY;
            }

            //Points for right Player
            if (x <= -10 - Ball.Width)
            {
                SpeedX = BeginnSpeedX;
                SpeedY = BeginnSpeedY;
                PlayerRightCount++;
                LeftCooldown = false;
                RightCooldown = false;
                Canvas.SetLeft(Ball, BeginnBallX);
                Canvas.SetTop(Ball, BeginnSpeedY);
                RightCount.Content = PlayerRightCount.ToString();
                return;
            }

            //Points for left Player
            if (x >= 10 + Gamefield.ActualWidth)
            {
                SpeedX = BeginnSpeedX;
                SpeedY = BeginnSpeedY;
                PlayerLeftCount++;
                LeftCooldown = false;
                RightCooldown = false;
                Canvas.SetLeft(Ball, BeginnBallX);
                Canvas.SetTop(Ball, BeginnSpeedY);
                LeftCount.Content = PlayerLeftCount.ToString();
                return;
            }

            //Speed up Ball
            SpeedX *= 1.001;
            SpeedY *= 1.001;

            //Ball Movement
            x += SpeedX * AnimationTimer.Interval.TotalSeconds;
            y += SpeedY * AnimationTimer.Interval.TotalSeconds;
            Canvas.SetLeft(Ball, x);
            Canvas.SetTop(Ball, y);
        }

        private void MovePlayer(object sender, EventArgs e)
        {
            if (P1Down)
            {
                if (Canvas.GetTop(LeftPaddle) + LeftPaddle.Height < Gamefield.ActualHeight)
                {
                    Canvas.SetTop(LeftPaddle, Canvas.GetTop(LeftPaddle) + 10);
                }
            }
            else if (P1Up)
            {
                if (Canvas.GetTop(LeftPaddle) > 0.0)
                {
                    Canvas.SetTop(LeftPaddle, Canvas.GetTop(LeftPaddle) - 10);
                }

            }
            if (P2Down)
            {
                if (Canvas.GetTop(RightPaddle) + 100 < Gamefield.ActualHeight)
                {
                    Canvas.SetTop(RightPaddle, Canvas.GetTop(RightPaddle) + 10);
                }
            }
            else if (P2Up)
            {
                if (Canvas.GetTop(RightPaddle) > 0.0)
                {
                    Canvas.SetTop(RightPaddle, Canvas.GetTop(RightPaddle) - 10);
                }
            }
        }

        #region Player Movement Events

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    {
                        P1Up = true;
                        P1Down = false;
                        break;
                    }

                case Key.S:
                    {
                        P1Up = false;
                        P1Down = true;
                        break;
                    }

                case Key.Up:
                    {
                        P2Up = true;
                        P2Down = false;
                        break;
                    }

                case Key.Down:
                    {
                        P2Up = false;
                        P2Down = true;
                        break;
                    }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    {
                        P1Up = false;
                        P1Down = false;
                        break;
                    }

                case Key.S:
                    {
                        P1Up = false;
                        P1Down = false;
                        break;
                    }

                case Key.Up:
                    {
                        P2Up = false;
                        P2Down = false;
                        break;
                    }

                case Key.Down:
                    {
                        P2Up = false;
                        P2Down = false;
                        break;
                    }
            }
        }

        #endregion

        
    }
}
