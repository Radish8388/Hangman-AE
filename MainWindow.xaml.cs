using System.IO;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hangman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> words = new List<string>();
        private List<string> defintions = new List<string>();
        private List<string> sentences = new List<string>();
        private Random rand = new Random();
        private int choice = 0; // random choice in the above Lists
        private char[] chars = { };
        private List<TextBlock> letters = new List<TextBlock>();
        private TextBlock definition = new TextBlock();
        private TextBlock sentence = new TextBlock();
        private bool[] letterFound = new bool[15];
        private int missedLetters = 0;
        private bool endOfGame = false;
        private bool[] letterGuessed = new bool[26];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Letter_Click(object sender, RoutedEventArgs e)
        {
            if (endOfGame) return;

            Button btn = (Button)sender;
            btn.IsEnabled = false;
            btn.Opacity = 0.4;
            char chosenLetter = ((string)btn.Content)[0];

            CheckLetter(chosenLetter);
        }

        private void CheckLetter(char chosenLetter)
        {
            int letterNumber = chosenLetter - 'A'; // get letter number 0-25
            if (letterGuessed[letterNumber]) return; // if player already guessed this letter, do nothing
            letterGuessed[letterNumber] = true;
            bool currentLetterFound = false;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chosenLetter == chars[i])
                {
                    currentLetterFound = true;
                    letterFound[i] = true;
                    letters[i].Opacity = 1;
                }
            }
            if (!currentLetterFound)
            {
                missedLetters++;
                UpdateHangman();
            }
            CheckForEndOfGame();
        }

        private void NewWord_Click(object sender, RoutedEventArgs e)
        {
            // start a new word
            NewWord();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double screenWidth = SystemParameters.WorkArea.Width;
            double screenHeight = SystemParameters.WorkArea.Height;

            // ensure window size doesn't exceed screen size
            if (this.Width > screenWidth) this.Width = screenWidth;
            if (this.Height > screenHeight) this.Height = screenHeight;

            // ensure window is not off the left or top
            if (this.Left < 0) this.Left = 0;
            if (this.Top < 0) this.Top = 0;

            // ensure window is not off the right or bottom
            if (this.Left + this.Width > screenWidth)
                this.Left = screenWidth - this.Width;
            if (this.Top + this.Height > screenHeight)
                this.Top = screenHeight - this.Height;

            string[] lines;
            string[] values;

            // read word/definition list from words.txt
            var uri = new Uri("pack://application:,,,/words.txt", UriKind.Absolute);
            var stream = Application.GetResourceStream(uri);
            using (StreamReader reader = new StreamReader(stream.Stream))
            {
                string content = reader.ReadToEnd();
                lines = content.Split('\n');
            }

            // split each line into word, definition, sentence
            for (int i = 0; i < lines.Length; i++)
            {
                values = lines[i].Split('\t');
                words.Add(values[0]);
                defintions.Add(values[1]);
                sentences.Add(values[2]);
            }

            // start a new word
            NewWord();
        }

        private void NewWord()
        {
            double wordWidth = 0;
            double textWidth = 0;

            canvas.Children.Clear();
            choice = rand.Next(words.Count);
            //choice = 575;
            chars = words[choice].ToCharArray();
            letters.Clear();
            missedLetters = 0;
            endOfGame = false;
            UpdateHangman();

            foreach (Button btn in Letters.Children)
            {
                btn.IsEnabled = true;
                btn.Opacity = 1.0;
            }

            for (int i = 0; i < letterGuessed.Length; i++)
                letterGuessed[i] = false;

            double canvasCenter = canvas.ActualWidth / 2;

            for (int i = 0; i < letterFound.Length; i++)
                letterFound[i] = true;

            for (int i = 0; i < chars.Length; i++)
            {
                TextBlock letter = new TextBlock();
                letter.Text = chars[i].ToString();
                letter.FontSize = 24;
                textWidth = 30;
                wordWidth = (textWidth + 5) * chars.Length;
                Canvas.SetLeft(letter, canvasCenter - wordWidth / 2 + i * (textWidth + 5)); // centered horizontally
                Canvas.SetTop(letter, 5); // vertically at the top
                letter.Opacity = 0;
                letters.Add(letter);
                canvas.Children.Add(letters[i]);

                TextBlock blank = new TextBlock();
                blank.Text = "—";
                blank.FontSize = 24;
                Canvas.SetLeft(blank, canvasCenter - wordWidth / 2 + i * (textWidth + 5) - 4); // centered horizontally
                Canvas.SetTop(blank, 20);
                canvas.Children.Add(blank);

                letterFound[i] = false;
                if (chars[i] == ' ' || chars[i] == '-') // show space and dash
                {
                    letters[i].Opacity = 1;
                    blank.Opacity = 0;
                    letterFound[i] = true;
                }
            }

            definition.Text = defintions[choice];
            definition.FontSize = 18;
            canvas.Children.Add(definition);
            definition.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            wordWidth = definition.DesiredSize.Width;
            Canvas.SetLeft(definition, canvasCenter - wordWidth / 2); // centered horizontally
            Canvas.SetTop(definition, 55);
            definition.Opacity = 0;

            sentence.Text = sentences[choice];
            sentence.FontSize = 18;
            canvas.Children.Add(sentence);
            sentence.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            wordWidth = sentence.DesiredSize.Width;
            Canvas.SetLeft(sentence, canvasCenter - wordWidth / 2); // centered horizontally
            Canvas.SetTop(sentence, 85);
            sentence.Opacity = 0;

        }

        private void UpdateHangman()
        {
            if (missedLetters == 6)
                definition.Opacity = 1;

            if (missedLetters >= 0 && missedLetters <= 7)
                hangman.Source = new BitmapImage(new Uri($"pack://application:,,,/hm{missedLetters}.png", UriKind.Absolute));
        }

        private void CheckForEndOfGame()
        {
            bool wordComplete = true;
            for (int i = 0; i < letterFound.Length; i++) // check if all letters found
            {
                if (letterFound[i] == false) wordComplete = false;
            }

            if (wordComplete || (missedLetters >= 7)) // found end of game
            {
                for (int i = 0; i < chars.Length; i++)
                {
                    letters[i].Opacity = 1;
                }
                definition.Opacity = 1;
                sentence.Opacity = 1;
                endOfGame = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Button btn = new Button();
            if (endOfGame) return;

            switch (e.Key)
            {
                case Key.A: btn = btnA; CheckLetter('A'); break;
                case Key.B: btn = btnB; CheckLetter('B'); break;
                case Key.C: btn = btnC; CheckLetter('C'); break;
                case Key.D: btn = btnD; CheckLetter('D'); break;
                case Key.E: btn = btnE; CheckLetter('E'); break;
                case Key.F: btn = btnF; CheckLetter('F'); break;
                case Key.G: btn = btnG; CheckLetter('G'); break;
                case Key.H: btn = btnH; CheckLetter('H'); break;
                case Key.I: btn = btnI; CheckLetter('I'); break;
                case Key.J: btn = btnJ; CheckLetter('J'); break;
                case Key.K: btn = btnK; CheckLetter('K'); break;
                case Key.L: btn = btnL; CheckLetter('L'); break;
                case Key.M: btn = btnM; CheckLetter('M'); break;
                case Key.N: btn = btnN; CheckLetter('N'); break;
                case Key.O: btn = btnO; CheckLetter('O'); break;
                case Key.P: btn = btnP; CheckLetter('P'); break;
                case Key.Q: btn = btnQ; CheckLetter('Q'); break;
                case Key.R: btn = btnR; CheckLetter('R'); break;
                case Key.S: btn = btnS; CheckLetter('S'); break;
                case Key.T: btn = btnT; CheckLetter('T'); break;
                case Key.U: btn = btnU; CheckLetter('U'); break;
                case Key.V: btn = btnV; CheckLetter('V'); break;
                case Key.W: btn = btnW; CheckLetter('W'); break;
                case Key.X: btn = btnX; CheckLetter('X'); break;
                case Key.Y: btn = btnY; CheckLetter('Y'); break;
                case Key.Z: btn = btnZ; CheckLetter('Z'); break;
                case Key.Enter: NewWord(); break;
            }
            btn.IsEnabled = false;
            btn.Opacity = 0.4;

            e.Handled = true;
        }
    }
}