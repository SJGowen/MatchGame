using Emoji.Wpf;

namespace MatchGame
{
    internal class MatchGuesser
    {
        private TextBlock EmojiToFind { get; set; } = new();
        private TextBlock EmojiGuessed { get; set; } = new();

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
            bool guessCorrect = EmojiToFind.Text == EmojiGuessed.Text;
            EmojiToFind.Text = guessCorrect ? "✔" : "❓";
            EmojiGuessed.Text = guessCorrect ? "✔" : "❓";
        }
    }
}
