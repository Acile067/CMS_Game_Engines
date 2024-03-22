using CMS_Game_Engines.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Serialization;
using Path = System.IO.Path;

namespace CMS_Game_Engines.Windows
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public string savedPath = "";
        public string savedImageName = "";
        public User savedUser = new User();
        public AddWindow(User user)
        {
            InitializeComponent();
            savedUser = user;
            txbFilePathRtf.Text = "Input file name";
            var bc = new BrushConverter();
            txbFilePathRtf.Foreground = (Brush)bc.ConvertFrom("#717286");

            txbActiveUsers.Text = "Input number of users";
            txbActiveUsers.Foreground = (Brush)bc.ConvertFrom("#717286");

            FontFamilyComboBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            ColorComboBox.ItemsSource = typeof(Colors).GetProperties()
                                            .Where(p => p.PropertyType == typeof(Color))
                                            .OrderBy(p => p.Name)
                                            .Select(p => (Color)p.GetValue(null))
                                            .ToList();
            FontSizeComboBox.ItemsSource = Enumerable.Range(1, 30).Select(i => (double)i).ToList();
            
            //UpdateWordCount();
            this.DataContext = this;
        }
        

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to cancel?", "Cancel Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                TableWindow tableWindow = new TableWindow(savedUser);
                tableWindow.Show();
                this.Close();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFormData())
            {
                string validName = txbFilePathRtf.Text.Trim();

                string xmlFilePath = "../../DataBase/game_engine.xml";
                int counter = 0;

                if (File.Exists(xmlFilePath))
                {
                    List<GameEngine> enginesCheck;
                    XmlSerializer serializer = new XmlSerializer(typeof(List<GameEngine>));
                    using (FileStream fileStream = new FileStream(xmlFilePath, FileMode.Open))
                    {
                        enginesCheck = (List<GameEngine>)serializer.Deserialize(fileStream);
                    }

                    foreach(var existingEngine in enginesCheck)
                    {
                        if(existingEngine.RtfFilePath == validName)
                        {
                            counter++;
                            validName = $"{txbFilePathRtf.Text.Trim()}({counter})";
                        }
                    }

                }


                GameEngine engine = new GameEngine(
                Convert.ToInt32(txbActiveUsers.Text),
                new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd).Text,
                savedImageName,
                validName
            );


                

                
                if (File.Exists(xmlFilePath))
                {
                    List<GameEngine> engines;
                    XmlSerializer serializer = new XmlSerializer(typeof(List<GameEngine>));
                    using (FileStream fileStream = new FileStream(xmlFilePath, FileMode.Open))
                    {
                        engines = (List<GameEngine>)serializer.Deserialize(fileStream);
                    }

                    
                    engines.Add(engine);
                    

                    
                    using (FileStream fileStream = new FileStream(xmlFilePath, FileMode.Create))
                    {
                        serializer.Serialize(fileStream, engines);
                    }
                }
                else
                {
                    
                    using (TextWriter writer = new StreamWriter(xmlFilePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<GameEngine>));
                        serializer.Serialize(writer, new List<GameEngine> { engine });
                    }
                }


                string folderName = "../../RTF"; 
                string folderPath = Path.Combine(Environment.CurrentDirectory, folderName); 

                
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string rtfFilePath = Path.Combine(folderPath, validName);

                if (!rtfFilePath.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
                {
                    rtfFilePath += ".rtf";
                }

                TextRange range = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd);
                using (FileStream fileStream = new FileStream(rtfFilePath, FileMode.Create))
                {
                    range.Save(fileStream, DataFormats.Rtf);
                }

                txbActiveUsers.Text = "";
                txbFilePathRtf.Text = "";
                EditorRichTextBox.Document.Blocks.Clear();
                ImagePreview.Source = null;
                SelectedImageNameLabel.Content = "";
                MessageBox.Show("New Game Engine successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddImgBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif, *.svg) | *.jpg; *.jpeg; *.png; *.gif; *.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
                savedPath = selectedImagePath;
                BitmapImage bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                ImagePreview.Source = bitmapImage;
                //SelectedImagePathLabel.Content = selectedImagePath;
                string selectedImageName = System.IO.Path.GetFileName(selectedImagePath);
                SelectedImageNameLabel.Content = selectedImageName;
                savedImageName = "../Images/" + selectedImageName;
                //BorderForImage.BorderThickness = new Thickness(0);
                SelectedImageNameLabel.Foreground = Brushes.Black;
                BorderForImage.BorderBrush = Brushes.Black;
            }
        }
        private void EditorRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object fontWeight = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            BoldToggleButton.IsChecked = (fontWeight != DependencyProperty.UnsetValue) && (fontWeight.Equals(FontWeights.Bold));

            object fontStyle = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            ItalicToggleButton.IsChecked = (fontStyle != DependencyProperty.UnsetValue) && (fontStyle.Equals(FontStyles.Italic));

            object textDecoration = EditorRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineToggleButton.IsChecked = (textDecoration != DependencyProperty.UnsetValue) && (textDecoration.Equals(TextDecorations.Underline));

            object fontFamily = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            FontFamilyComboBox.SelectedItem = fontFamily;

            object foregroundColor = EditorRichTextBox.Selection.GetPropertyValue(Inline.ForegroundProperty);
            if (foregroundColor is SolidColorBrush brush)
            {
                Color selectedColor = brush.Color;
                ColorComboBox.SelectedItem = selectedColor;
            }

            object fontSize = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty);
            if (fontSize != DependencyProperty.UnsetValue)
            {
                FontSizeComboBox.SelectedItem = (double)fontSize;
            }


        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorComboBox.SelectedItem != null && !EditorRichTextBox.Selection.IsEmpty)
            {
                if (ColorComboBox.SelectedItem is Color selectedColor)
                {
                    
                    SolidColorBrush brush = new SolidColorBrush(selectedColor);
                    EditorRichTextBox.Selection.ApplyPropertyValue(Inline.ForegroundProperty, brush);
                }
            }
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null && !EditorRichTextBox.Selection.IsEmpty)
            {
                EditorRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
            }
        }
        private bool ValidateFormData()
        {
            bool isValid = true;


            string input = txbActiveUsers.Text;
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    int result = int.Parse(input);

                }
                catch (FormatException)
                {
                    isValid = false;
                    ActiveUsersErrorLable.Content = "Not a number!";
                    txbActiveUsers.BorderBrush = Brushes.Red;
                }
            }

            if (txbFilePathRtf.Text.Trim().Equals(string.Empty) || txbFilePathRtf.Text.Trim().Equals("Input file name"))
            {
                isValid = false;
                FilePathRtfErrorLable.Content = "Field cannot be left empty!";
                txbFilePathRtf.BorderBrush = Brushes.Red;
            }
            else
            {
                FilePathRtfErrorLable.Content = string.Empty;
                txbFilePathRtf.BorderBrush = Brushes.Black;
            }

            if (txbActiveUsers.Text.Trim().Equals(string.Empty) || txbActiveUsers.Text.Trim().Equals("Input number of users"))
            {
                isValid = false;
                ActiveUsersErrorLable.Content = "Field cannot be left empty!";
                txbActiveUsers.BorderBrush = Brushes.Red;
            }
            else
            {
                ActiveUsersErrorLable.Content = string.Empty;
                txbActiveUsers.BorderBrush = Brushes.Black;
            }


            if (SelectedImageNameLabel.Content.ToString().Trim() == string.Empty || SelectedImageNameLabel.Content.ToString() == "Image must be added!")
            {
                isValid = false;
                SelectedImageNameLabel.Content = "Image must be added!";
                SelectedImageNameLabel.Foreground = Brushes.Red;
                BorderForImage.BorderBrush = Brushes.Red;
            }

            return isValid;
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeComboBox.SelectedItem != null && !EditorRichTextBox.Selection.IsEmpty)
            {
                EditorRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, FontSizeComboBox.SelectedItem);
            }
        }

        private void txbFilePathRtf_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbFilePathRtf.Text.Trim().Equals("Input file name"))
            {
                txbFilePathRtf.Text = "";
                txbFilePathRtf.Foreground = Brushes.Black;
            }
            FilePathRtfErrorLable.Content = "";
            txbFilePathRtf.BorderBrush = Brushes.Black;
        }

        private void txbFilePathRtf_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbFilePathRtf.Text.Trim().Equals(string.Empty))
            {
                txbFilePathRtf.Text = "Input file name";
                var bc = new BrushConverter();
                txbFilePathRtf.Foreground = (Brush)bc.ConvertFrom("#717286");

            }
        }

        private void txbActiveUsers_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbActiveUsers.Text.Trim().Equals("Input number of users"))
            {
                txbActiveUsers.Text = "";
                txbActiveUsers.Foreground = Brushes.Black;
            }
            ActiveUsersErrorLable.Content = "";
            txbActiveUsers.BorderBrush = Brushes.Black;
        }

        private void txbActiveUsers_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbActiveUsers.Text.Trim().Equals(string.Empty))
            {
                txbActiveUsers.Text = "Input number of users";
                var bc = new BrushConverter();
                txbActiveUsers.Foreground = (Brush)bc.ConvertFrom("#717286");

            }
        }

        private void txbActiveUsers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            int value;
            if (!int.TryParse(newText, out value))
            {
                e.Handled = true;
            }
        }

        private void UpdateWordCount()
        {
            
            string text = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd).Text;
            int wordCount = text.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            wordCountTextBlock.Text = $"Word Count: {wordCount}";
        }
        private void EditorRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWordCount();
        }

        private void txbFilePathRtf_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z0-9():_-]");
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }
    }
}
