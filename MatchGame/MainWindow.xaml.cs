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
        private int emojisToGuess;
        private readonly List<string> emojis = new() { 
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
        private readonly DispatcherTimer timer = new();
        private readonly DispatcherTimer delay = new();
        private List<string> gameEmoji = new();
        private TextBlock emojiToFind;
        private TextBlock emojiGuessed;
        private int tenthsOfSecondsElapsed;
        private int matchesFound;
        private bool emojiMatch = false;
        private bool findingMatch = false;


        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(.1);
            timer.Tick += Timer_Tick;

            delay.Interval = TimeSpan.FromSeconds(1);
            delay.Tick += Delay_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tenthsOfSecondsElapsed++;
            TimeTextBlock.Text = (tenthsOfSecondsElapsed / 10F).ToString("0.0s");
            if (matchesFound == emojisToGuess)
            {
                timer.Stop();
                TimeTextBlock.Text = "Click Me to Beat - " + TimeTextBlock.Text;
                ResizeMode = ResizeMode.CanResize;
            }
        }

        private void Delay_Tick(object sender, EventArgs e)
        {
            ProcessDelay();
        }

        private void SetUpGame()
        {
            timer.Stop();
            gameEmoji.Clear();

            Random random = new();
            List<string> emojisToDisplay = new();

            while (emojisToDisplay.Count < emojisToGuess * 2)
            {
                var character = random.Next(emojis.Count);
                if (!emojisToDisplay.Contains(emojis[character]))
                {
                    emojisToDisplay.Add(emojis[character]);
                    emojisToDisplay.Add(emojis[character]);
                }
            }

            foreach (var textBlock in MainGrid.Children.OfType<TextBlock>().Where(x => x.Name == "" && x.ActualWidth > 0))
            {
                textBlock.Text = "❓";
                int index = random.Next(emojisToDisplay.Count);
                gameEmoji.Add(emojisToDisplay[index]);
                emojisToDisplay.RemoveAt(index);
            }

            findingMatch = false;
            TimeTextBlock.Text = "To Start, click ❓ above";
        }


        private void Emoji_MouseDown(object sender, MouseButtonEventArgs e)
        {
            void ProcessClick(TextBlock textBlock, string emojiClicked, bool match)
            {
                textBlock.Text = emojiClicked;
                emojiMatch = match;
                emojiGuessed = textBlock;
                findingMatch = false;
                delay.Start();
            }

            TextBlock cellClicked = sender as TextBlock;
            if (cellClicked.Text == "❓")
            {
                ProcessTimerEvents();

                var pos = int.Parse(cellClicked.Tag.ToString());
                string emojiClicked = gameEmoji[pos];

                if (findingMatch == false)
                {
                    cellClicked.Text = emojiClicked;
                    emojiToFind = cellClicked;
                    findingMatch = true;
                }
                else if (emojiClicked == emojiToFind.Text)
                {
                    matchesFound++;
                    ProcessClick(cellClicked, emojiClicked, true);
                }
                else
                {
                    ProcessClick(cellClicked, emojiClicked, false);
                }
            }
        }

        private void ProcessTimerEvents()
        {
            ProcessStart();
            ProcessDelay();
        }

        private void ProcessStart()
        {
            if (!timer.IsEnabled)
            {
                if (emojisToGuess * 2 != gameEmoji.Count) 
                    SetUpGame();

                ResizeMode = ResizeMode.NoResize;
                timer.Start();
                tenthsOfSecondsElapsed = 0;
                matchesFound = 0;
            }
        }

        private void ProcessDelay()
        {
            if (delay.IsEnabled)
            {
                delay.Stop();
                emojiToFind.Text = emojiMatch ? "✔" : "❓";
                emojiGuessed.Text = emojiMatch ? "✔" : "❓";
            }
        }

        private void TimeTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUpGame();
        }

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!timer.IsEnabled)
            {
                Col8.Width = e.NewSize.Width < 692 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col7.Width = e.NewSize.Width < 608 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col6.Width = e.NewSize.Width < 524 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col5.Width = e.NewSize.Width < 440 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                emojisToGuess = 16;
                if (e.NewSize.Width < 692) emojisToGuess = 14;
                if (e.NewSize.Width < 608) emojisToGuess = 12;
                if (e.NewSize.Width < 524) emojisToGuess = 10;
                if (e.NewSize.Width < 440) emojisToGuess = 8;
            }
        }
    }
}
