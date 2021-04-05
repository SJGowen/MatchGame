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
        private readonly GameGuesser GameGuesser = new();
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
        private readonly DispatcherTimer Timer = new();
        private readonly DispatcherTimer Delay = new();
        private readonly List<string> GameEmoji = new();
        private int TenthsOfSecondsElapsed;
        private int MatchesFound;
        private bool FindingMatch = false;

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
            }
        }

        private void Delay_Tick(object sender, EventArgs e)
        {
            ProcessDelay();
        }

        private void SetUpGame()
        {
            Timer.Stop();
            GameEmoji.Clear();

            Random random = new();
            List<string> emojisToDisplay = new();

            while (emojisToDisplay.Count < EmojisToGuess * 2)
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
                GameEmoji.Add(emojisToDisplay[index]);
                emojisToDisplay.RemoveAt(index);
            }

            FindingMatch = false;
            GameGuesser.Reinitialise();
            TimeTextBlock.Text = "To Start, click ❓ above";
        }

        private void Emoji_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock cellClicked = sender as TextBlock;
            if (cellClicked.Text == "❓")
            {
                ProcessTimerEvents();

                var pos = int.Parse(cellClicked.Tag.ToString());
                string emojiClicked = GameEmoji[pos];
                cellClicked.Text = emojiClicked;

                if (FindingMatch == false)
                {
                    GameGuesser.RecordToFind(cellClicked);
                } 
                else
                {
                    if (GameGuesser.RecordGuess(cellClicked))
                    {
                        MatchesFound++;
                    }

                    Delay.Start();
                }

                FindingMatch = !FindingMatch;
            }
        }

        private void ProcessTimerEvents()
        {
            ProcessStart();
            ProcessDelay();
        }

        private void ProcessStart()
        {
            if (!Timer.IsEnabled)
            {
                if (EmojisToGuess * 2 != GameEmoji.Count) 
                    SetUpGame();

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
                GameGuesser.ProcessGuess();
            }
        }

        private void TimeTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUpGame();
        }

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!Timer.IsEnabled)
            {
                Col8.Width = e.NewSize.Width < 692 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col7.Width = e.NewSize.Width < 608 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col6.Width = e.NewSize.Width < 524 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                Col5.Width = e.NewSize.Width < 440 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
                EmojisToGuess = 16;
                if (e.NewSize.Width < 692) EmojisToGuess = 14;
                if (e.NewSize.Width < 608) EmojisToGuess = 12;
                if (e.NewSize.Width < 524) EmojisToGuess = 10;
                if (e.NewSize.Width < 440) EmojisToGuess = 8;
            }
        }
    }

    public class GameGuesser
    {
        private TextBlock EmojiToFind { get; set; }
        private TextBlock EmojiGuessed { get; set; }

        public GameGuesser()
        {
            EmojiToFind = new TextBlock();
            EmojiGuessed = new TextBlock();
        }

        public void Reinitialise()
        {
            EmojiToFind.Text = "❓";
            EmojiGuessed.Text = "❓";
        }

        public void RecordToFind(TextBlock emojiToFind)
        {
            EmojiToFind = emojiToFind;
        }

        public bool RecordGuess(TextBlock emojiGuessed)
        {
            EmojiGuessed = emojiGuessed;
            return EmojiToFind.Text == EmojiGuessed.Text;
        }

        public void ProcessGuess()
        {
            var guessCorrect = EmojiToFind.Text == EmojiGuessed.Text;
            EmojiToFind.Text = guessCorrect ? "✔" : "❓";
            EmojiGuessed.Text = guessCorrect ? "✔" : "❓";
        }
    }
}
