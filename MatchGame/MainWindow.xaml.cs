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
        private readonly int cellsToGuess = 10;
        private readonly DispatcherTimer timer = new();
        private readonly DispatcherTimer delay = new();
        private List<string> gameEmoji = new();
        private TextBlock pictureToFind;
        private TextBlock pictureGuessed;
        private int tenthsOfSecondsElapsed;
        private int matchesFound;
        private bool pictureMatch = false;
        private bool findingMatch = false;
        


        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(.1);
            timer.Tick += Timer_Tick;

            delay.Interval = TimeSpan.FromSeconds(1);
            delay.Tick += Delay_Tick;

            SetUpGame();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tenthsOfSecondsElapsed++;
            timeTextBlock.Text = (tenthsOfSecondsElapsed / 10F).ToString("0.0s");
            if (matchesFound == cellsToGuess)
            {
                timer.Stop();
                timeTextBlock.Text += " - Play Again?";
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
            List<string> animalEmoji = new()
            {
                "🐘", "🐘",
                "🐙", "🐙",
                "🐹", "🐹",
                "🐮", "🐮",
                "🐷", "🐷",
                "🙉", "🙉",
                "🦈", "🦈",
                "🦊", "🦊",
                "🐒", "🐒",
                "🦍", "🦍"
            };

            Random random = new();

            foreach (var textBlock in mainGrid.Children.OfType<TextBlock>().Where(x => x.Name == ""))
            {
                textBlock.Text ="❓";
                int index = random.Next(animalEmoji.Count);
                gameEmoji.Add(animalEmoji[index]);
                animalEmoji.RemoveAt(index);
            }

            findingMatch = false;
            timeTextBlock.Text = "To start, click on a ❓";
        }


        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            void ProcessClick(TextBlock textBlock, string emojiClicked, bool match)
            {
                textBlock.Text = emojiClicked;
                pictureMatch = match;
                pictureGuessed = textBlock;
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
                    pictureToFind = cellClicked;
                    findingMatch = true;
                }
                else if (emojiClicked == pictureToFind.Text)
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
                pictureToFind.Text = pictureMatch ? "✔" : "❓";
                pictureGuessed.Text = pictureMatch ? "✔" : "❓";
            }
        }

        private void TimeTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUpGame();
        }
    }
}
