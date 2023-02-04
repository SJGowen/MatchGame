using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Emoji.Wpf;

namespace Match
{
    public partial class MainWindow : Window
    {
        private readonly string BestTimesSizeFile = "Match.ini";
        private int EmojisToGuess;
        private readonly MatchGuesser MatchGuesser = new();
        private readonly string QuestionMark = "❓";
        private readonly string Tick = "✔";
        private readonly List<string> Emojis = new()
        {
            "🌰", "🌱", "🌴", "🌵", "🌷", "🌸", "🌹", "🌺", "🌻", "🌼", "🌽", "🌾",
            "🌿", "🍀", "🍁", "🍂", "🍃", "🍄", "🍅", "🍆", "🍇", "🍈", "🍉", "🍊",
            "🍌", "🍍", "🍎", "🍏", "🍑", "🍒", "🍓", "🎃", "🎅", "🎠", "🎪", "🎷",
            "🎸", "🎹", "🎺", "🎻", "🏰", "🐌", "🐍", "🐎", "🐑", "🐒", "🐔", "🐗",
            "🐘", "🐙", "🐚", "🐛", "🐜", "🐝", "🐞", "🐟", "🐠", "🐡", "🐢", "🐣",
            "🐤", "🐥", "🐦", "🐧", "🐨", "🐩", "🐫", "🐬", "🐭", "🐮", "🐯", "🐰",
            "🐱", "🐲", "🐳", "🐴", "🐵", "🐶", "🐷", "🐸", "🐹", "🐺", "🐻", "🐼",
            "🙈", "🙉", "🙊", "🚑", "🚒", "🚓", "🚲", "🚛", "🍐", "🚴", "🐇", "📯",
            "🐅", "🚊", "🐪", "🐓", "🐐", "🐖", "🐋", "🚂", "🐈", "🚜", "🐁", "🐀",
            "🐏", "🐕", "🚁", "🐂", "🐄", "🐊", "🐆", "🌳", "🍋", "🌲", "🐃", "🏋"
        };

        private readonly DispatcherTimer Timer = new();
        private readonly DispatcherTimer Delay = new();
        private readonly List<string> GameEmojis = new();
        private int TenthsOfSecondsElapsed;
        private int MatchesFound;
        private bool FindingMatch;
        private bool GameOver;

        private readonly TimeRecorder TimeRecorder = new();
        private readonly string[] Args;

        private (int EmojiCount, string CompletionTime) EmojiTime = (0, "");

        public MainWindow()
        {
            InitializeComponent();
            Args = Environment.GetCommandLineArgs();

            Timer.Interval = TimeSpan.FromSeconds(.1);
            Timer.Tick += Timer_Tick;

            Delay.Interval = TimeSpan.FromSeconds(1);
            Delay.Tick += Delay_Tick;

            Width = GetFormWidth();
        }

        private void SetGameOver(bool gameOver)
        {
            ResizeMode = gameOver ? ResizeMode.CanResize : ResizeMode.NoResize;
            if (!GameOver && gameOver && (MatchesFound == EmojisToGuess))
            {
                TimeRecorder.RecordBestTimes(EmojisToGuess, TenthsOfSecondsElapsed, Top, Left);
                TimeTextBlock.ToolTip = BestTimeTextBlock.ToolTip = TimeRecorder.GetBestTimes(EmojisToGuess);
                BestTimeTextBlock.Text = TimeRecorder.GeMinimumTime(EmojisToGuess);
            }
            TenthsOfSecondsElapsed = 0;
            MatchesFound = 0;
            GameOver = gameOver;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TenthsOfSecondsElapsed++;
            var elaspedTime = $"{TenthsOfSecondsElapsed / 10F:0.0s}";
            if (MatchesFound == EmojisToGuess)
            {
                Timer.Stop();
                TimeTextBlock.Text = $"{EmojisToGuess * 2} Emojis = {elaspedTime}";
                EmojiTime = (EmojisToGuess * 2, TimeTextBlock.Text);
                SetGameOver(true);
            }
            else
            {
                TimeTextBlock.Text = elaspedTime;
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
            EmojiTime = (0, "");
            SetGameOver(true);

            SetDisplayForAllTextBlocksWithNoName(QuestionMark);

            List<string> emojisToDisplay = new();
            SelectEmojisToUse(emojisToDisplay);
            PopulateGameEmojis(emojisToDisplay);

            FindingMatch = false;
            GameOver = false;
            TimeTextBlock.Text = $"To Start, Click {QuestionMark} Above";
        }

        private void SetDisplayForAllTextBlocksWithNoName(string display)
        {
            foreach (var textBlock in MainGrid.Children.OfType<TextBlock>().Where(x => x.Name == ""))
            {
                textBlock.Text = display;
            }
        }

        private void PopulateGameEmojis(List<string> emojisToDisplay)
        {
            Random random = new();
            // If the width of the Column is > 0 then it is in the new game so assign an emoji to it
            foreach (var textBlock in MainGrid.Children.OfType<TextBlock>().Where(x => x.Name == "" && x.ActualWidth > 0))
            {
                int index = random.Next(emojisToDisplay.Count);
                GameEmojis.Add(emojisToDisplay[index]);
                emojisToDisplay.RemoveAt(index); // Remove used emoji so that it isn't chosen again
            }
        }

        private void SelectEmojisToUse(List<string> emojisToDisplay)
        {
            Random random = new();
            while (emojisToDisplay.Count < EmojisToGuess * 2)
            {
                var character = random.Next(Emojis.Count);
                if (!emojisToDisplay.Contains(Emojis[character])) // Ensure emoji is not a duplicate
                {
                    emojisToDisplay.Add(Emojis[character]);
                    emojisToDisplay.Add(Emojis[character]);
                }
            }
        }

        private void Emoji_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock cellClicked = sender as TextBlock;
            if (cellClicked.Text == QuestionMark)
            {
                ProcessStart();
            }
            if (!GameOver && cellClicked.Text == QuestionMark)
            {
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
            else if (!Timer.IsEnabled)
            {
                TimeTextBlock_MouseDown(sender, e);
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

                SetGameOver(false);
                Timer.Start();
            }
        }

        private void ProcessDelay()
        {
            if (Delay.IsEnabled)
            {
                Delay.Stop();
                MatchGuesser.ProcessGuess(Args.Length > 1 && string.Equals(Args[1], "ShowTicks", StringComparison.OrdinalIgnoreCase));
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
                Col12.Width = e.NewSize.Width < 1028 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col11.Width = e.NewSize.Width < 944 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col10.Width = e.NewSize.Width < 860 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col9.Width = e.NewSize.Width < 776 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col8.Width = e.NewSize.Width < 692 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col7.Width = e.NewSize.Width < 608 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col6.Width = e.NewSize.Width < 524 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col5.Width = e.NewSize.Width < 440 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);

                var emojisToGuess = EmojisToGuess;
                EmojisToGuess = GetEmojisToGuess(e.NewSize.Width);

                if (emojisToGuess != EmojisToGuess)
                {
                    // TenthsOfSecondsElapsed = 0 when game hasn't been played
                    // Text for TimeTextBlock starts with "Time " once the below test has passed
                    if (TenthsOfSecondsElapsed != 0 && !TimeTextBlock.Text.StartsWith("Time "))
                    {
                        // Note use of emojisToGuess with lower case e, this is so old value is used
                        TimeTextBlock.Text = $"Time {emojisToGuess * 2} emojis = {TenthsOfSecondsElapsed / 10F:0.0s}";
                    }

                    // Note use of EmojisToGuess with uppper case E, this is so new value is used
                    TimeRecorder.ReadBestTimesFromFile(EmojisToGuess);
                    TimeTextBlock.ToolTip = BestTimeTextBlock.ToolTip = TimeRecorder.GetBestTimes(EmojisToGuess);
                    BestTimeTextBlock.Text = TimeRecorder.GeMinimumTime(EmojisToGuess);
                    TimeTextBlock.Text = $"To Start, Click {QuestionMark} Above";
                    ChangeDispayTextAndIcons();
                }
            }
        }

        private void ChangeDispayTextAndIcons()
        {
            if (EmojiTime.EmojiCount == EmojisToGuess * 2)
            {
                TimeTextBlock.Text = EmojiTime.CompletionTime;

                if (Args.Length > 1 && string.Equals(Args[1], "ShowTicks", StringComparison.OrdinalIgnoreCase))
                {
                    SetDisplayForAllTextBlocksWithNoName(Tick);
                }
                else
                {
                    var i = 0;
                    foreach (var textBlock in MainGrid.Children.OfType<TextBlock>().Take(EmojiTime.EmojiCount))
                    {
                        textBlock.Text = GameEmojis[i];
                        i++;
                    }
                }
            }
            else
            {
                SetDisplayForAllTextBlocksWithNoName(QuestionMark);
            }
        }

        private static int GetEmojisToGuess(double width)
        {
            return width switch
            {
                < 440 => 8,
                < 524 => 10,
                < 608 => 12,
                < 692 => 14,
                < 776 => 16,
                < 860 => 18,
                < 944 => 20,
                < 1028 => 22,
                _ => 24
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using FileStream fs = new(BestTimesSizeFile, FileMode.OpenOrCreate, FileAccess.Write);
            using StreamWriter sw = new(fs);
            sw.WriteLine($"WindowWidth={(int)Width}");
        }


        public int GetFormWidth()
        {
            using FileStream fs = new(BestTimesSizeFile, FileMode.OpenOrCreate, FileAccess.Read);
            using StreamReader sr = new(fs);
            var str = sr.ReadLine();
            while (!string.IsNullOrWhiteSpace(str))
            {
                if (str.StartsWith("WindowWidth"))
                {
                    var bits = str.Split('=');
                    return Convert.ToInt32(bits[1]);
                }
                str = sr.ReadLine();
            }

            return 585;
        }
    }
}
