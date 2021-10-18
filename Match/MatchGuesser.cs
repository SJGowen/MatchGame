using Emoji.Wpf;

namespace Match
{
    internal class MatchGuesser
    {
        private TextBlock EmojiToFind { get; set; } = new();
        private TextBlock EmojiGuessed { get; set; } = new();

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
                EmojiToFind.Text = "❓";
                EmojiGuessed.Text = "❓";
            }
            else if (showTicks)
            {
                EmojiToFind.Text = "✔";
                EmojiGuessed.Text = "✔";
            }
        }
    }
}