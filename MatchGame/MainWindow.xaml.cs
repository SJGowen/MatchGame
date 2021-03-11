using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MatchGame
{
    public partial class MainWindow : Window
    {
        readonly DispatcherTimer timer = new();
        readonly DispatcherTimer delay = new();
        private List<string> gameEmoji = new();
        private TextBlock findingMatchFalseClicked;
        private TextBlock findingMatchTrueClicked;
        private int tenthsOfSecondsElapsed;
        private int matchesFound;
        private bool displayMatch = false;
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
            if (matchesFound == 8)
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
                "🦊", "🦊"
            };

            Random random = new();

            foreach (var textBlock in mainGrid.Children.OfType<TextBlock>().Where(x => x.Name == ""))
            {
                textBlock.Text ="?";
                int index = random.Next(animalEmoji.Count);
                gameEmoji.Add(animalEmoji[index]);
                animalEmoji.RemoveAt(index);
            }
        }


        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            void ProcessClick(TextBlock textBlock, string emojiClicked, bool match)
            {
                textBlock.Text = emojiClicked;
                displayMatch = match;
                findingMatchTrueClicked = textBlock;
                findingMatch = false;
                delay.Start();
            }

            ProcessTimerEvents();
            TextBlock textBlock = sender as TextBlock;

            if (textBlock.Text == "?")
            {
                var pos = int.Parse(textBlock.Tag.ToString());
                string emojiClicked = gameEmoji[pos];

                if (findingMatch == false)
                {
                    textBlock.Text = emojiClicked;
                    findingMatchFalseClicked = textBlock;
                    findingMatch = true;
                }
                else if (emojiClicked == findingMatchFalseClicked.Text)
                {
                    matchesFound++;
                    ProcessClick(textBlock, emojiClicked, true);
                }
                else
                {
                    ProcessClick(textBlock, emojiClicked, false);
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
                findingMatchFalseClicked.Text = displayMatch ? "✔" : "?";
                findingMatchTrueClicked.Text = displayMatch ? "✔" : "?";
            }
        }

        private void TimeTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (matchesFound == 8)
            {
                timer.Stop();
                SetUpGame();
            }
        }
    }
}
