using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Emoji.Wpf;
using System.Windows.Input;
using System.Windows.Threading;

namespace MatchGame
{
    public partial class MainWindow : Window
    {
        private int EmojisToGuess;
        private readonly MatchGuesser MatchGuesser = new();
        private readonly List<string> Emojis = new() { 
            "🌰", "🌱", "🌴", "🌵", "🌷", "🌸", "🌹", "🌺", "🌻", "🌼", "🌽", "🌾", 
            "🌿", "🍀", "🍁", "🍂", "🍃", "🍄", "🍅", "🍆", "🍇", "🍈", "🍉", "🍊", 
            "🍌", "🍍", "🍎", "🍏", "🍑", "🍒", "🍓", "🎃", "🎅", "🎠", "🎪", "🎷", 
            "🎸", "🎹", "🎺", "🎻", "🏰", "🐌", "🐍", "🐎", "🐑", "🐒", "🐔", "🐗", 
            "🐘", "🐙", "🐚", "🐛", "🐜", "🐝", "🐞", "🐟", "🐠", "🐡", "🐢", "🐣", 
            "🐤", "🐥", "🐦", "🐧", "🐨", "🐩", "🐫", "🐬", "🐭", "🐮", "🐯", "🐰", 
            "🐱", "🐲", "🐳", "🐴", "🐵", "🐶", "🐷", "🐸", "🐹", "🐺", "🐻", "🐼", 
            "🙈", "🙉", "🙊", "🚑", "🚒", "🚓", "🚲", "🚛", "🍐", "🚴", "🐇", "📯", 
            "🐅", "🚊", "🐪", "🐓", "🐐", "🐖", "🐋", "🚂", "🐈", "🚜", "🐁", "🐀", 
            "🐏", "🐕", "🚁", "🐂", "🐄", "🐊", "🐆", "🌳", "🍋", "🌲", "🐃", "🏋"};
        private readonly DispatcherTimer Timer = new();
        private readonly DispatcherTimer Delay = new();
        private readonly List<string> GameEmojis = new();
        private int TenthsOfSecondsElapsed;
        private int MatchesFound;
        private bool FindingMatch;
        private readonly TimeRecorder TimeRecorder = new();

        public MainWindow()
        {
            InitializeComponent();

            Timer.Interval = TimeSpan.FromSeconds(.1);
            Timer.Tick += Timer_Tick;

            Delay.Interval = TimeSpan.FromSeconds(1);
            Delay.Tick += Delay_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TenthsOfSecondsElapsed++;
            TimeTextBlock.Text = (TenthsOfSecondsElapsed / 10F).ToString("0.0s");

            if (MatchesFound == EmojisToGuess)
            {
                Timer.Stop();
                TimeTextBlock.Text = "Click Me to Beat - " + TimeTextBlock.Text;
                ResizeMode = ResizeMode.CanResize;
                TimeRecorder.RecordBestTimes(EmojisToGuess, TenthsOfSecondsElapsed, Top, Left);
                TimeTextBlock.ToolTip = TimeRecorder.GetBestTimes();
            }
        }

        private void Delay_Tick(object sender, EventArgs e)
        {
            ProcessDelay();
        }

        private void SetUpGame()
        {
            Timer.Stop();
            GameEmojis.Clear();

            Random random = new();
            List<string> emojisToDisplay = new();

            while (emojisToDisplay.Count < EmojisToGuess * 2)
            {
                var character = random.Next(Emojis.Count);
                if (!emojisToDisplay.Contains(Emojis[character]))
                {
                    emojisToDisplay.Add(Emojis[character]);
                    emojisToDisplay.Add(Emojis[character]);
                }
            }

            foreach (var textBlock in MainGrid.Children.OfType<TextBlock>().Where(x => x.Name == "" && x.ActualWidth > 0))
            {
                textBlock.Text = "❓";
                int index = random.Next(emojisToDisplay.Count);
                GameEmojis.Add(emojisToDisplay[index]);
                emojisToDisplay.RemoveAt(index);
            }

            FindingMatch = false;
            MatchGuesser.Reinitialise();
            TimeTextBlock.Text = "To Start, Click ❓ Above";
        }

        private void Emoji_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock cellClicked = sender as TextBlock;
            if (cellClicked.Text == "❓")
            {
                ProcessStart();
                ProcessDelay();

                var pos = int.Parse(cellClicked.Tag.ToString());
                string emojiClicked = GameEmojis[pos];
                cellClicked.Text = emojiClicked;

                if (!FindingMatch)
                {
                    MatchGuesser.RecordToFind(cellClicked);
                } 
                else
                {
                    if (MatchGuesser.RecordGuess(cellClicked))
                    {
                        MatchesFound++;
                    }

                    Delay.Start();
                }

                FindingMatch = !FindingMatch;
            }
        }

        private void ProcessStart()
        {
            if (!Timer.IsEnabled)
            {
                if (EmojisToGuess * 2 != GameEmojis.Count)
                {
                    SetUpGame();
                }

                ResizeMode = ResizeMode.NoResize;
                Timer.Start();
                TenthsOfSecondsElapsed = 0;
                MatchesFound = 0;
            }
        }

        private void ProcessDelay()
        {
            if (Delay.IsEnabled)
            {
                Delay.Stop();
                MatchGuesser.ProcessGuess();
            }
        }

        private void TimeTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ProcessDelay();
            SetUpGame();
        }

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!Timer.IsEnabled)
            {
                Col10.Width = e.NewSize.Width < 860 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col9.Width = e.NewSize.Width < 776 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col8.Width = e.NewSize.Width < 692 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col7.Width = e.NewSize.Width < 608 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col6.Width = e.NewSize.Width < 524 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col5.Width = e.NewSize.Width < 440 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                EmojisToGuess = 20;
                if (e.NewSize.Width < 860) EmojisToGuess = 18;
                if (e.NewSize.Width < 776) EmojisToGuess = 16;
                if (e.NewSize.Width < 692) EmojisToGuess = 14;
                if (e.NewSize.Width < 608) EmojisToGuess = 12;
                if (e.NewSize.Width < 524) EmojisToGuess = 10;
                if (e.NewSize.Width < 440) EmojisToGuess = 8;
            }
        }
    }
}
