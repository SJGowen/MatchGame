using System;
using System.Drawing;
using System.Windows;

namespace WpfDialogs
{
    public partial class StringGetter : Window
    {
        public StringGetter(string title, string question, string defaultAnswer, double top, double left)
        {
            InitializeComponent();
            SetImageSource();
            Top = top + 20;
            Left = left + 20;
            ResizeMode = ResizeMode.NoResize;
            Title = title;
            lblQuestion.Content = question;
            txtAnswer.Text = defaultAnswer;
        }

        private void SetImageSource() => imageQuestion.Source = SystemIcons.Question.ToImageSource();

        private void btnDialogOk_Click(object sender, RoutedEventArgs e) => DialogResult = true;

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }

        public string Answer => txtAnswer.Text;
    }
}
