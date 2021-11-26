using Emoji.Wpf;

namespace Match
{
    internal class MatchGuesser
    {
        private TextBlock EmojiToFind { get; set; } = new();
        private TextBlock EmojiGuessed { get; set; } = new();
        private readonly string QuestionMark = "❓";
        private readonly string Tick = "✔";

        public void RecordToFind(TextBlock emojiToFind)
        {
            EmojiToFind = emojiToFind;
        }

        public bool RecordGuess(TextBlock emojiGuessed)
        {
            EmojiGuessed = emojiGuessed;
            return EmojiToFind.Text == EmojiGuessed.Text;
        }

        public void ProcessGuess(bool showTicks)
        {
            if (EmojiToFind.Text != EmojiGuessed.Text)
            {
                EmojiToFind.Text = QuestionMark;
                EmojiGuessed.Text = QuestionMark;
            }
            else if (showTicks)
            {
                EmojiToFind.Text = Tick;
                EmojiGuessed.Text = Tick;
            }
        }
    }
}