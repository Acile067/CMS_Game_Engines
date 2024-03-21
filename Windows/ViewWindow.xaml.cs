using CMS_Game_Engines.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace CMS_Game_Engines.Windows
{
    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {
        public ViewWindow(GameEngine engine)
        {
            InitializeComponent();
            NameLabel.Content = engine.RtfFilePath;

            ImagePreview.Source = new BitmapImage(new Uri(engine.ImagePath, UriKind.Relative));
            string imageName = Path.GetFileName(engine.ImagePath);
            SelectedImageNameLabel.Content = imageName;

            NumberOfUsersLabel.Content = engine.ActiveUsersField.ToString();

            try
            {
                string rtfFilePath = "../../RTF/" + engine.RtfFilePath + ".rtf";

                if (File.Exists(rtfFilePath))
                {

                    TextRange range = new TextRange(RichTextBoxView.Document.ContentStart, RichTextBoxView.Document.ContentEnd);
                    using (FileStream fileStream = new FileStream(rtfFilePath, FileMode.Open))
                    {
                        range.Load(fileStream, DataFormats.Rtf);
                    }
                }
                else
                {
                    MessageBox.Show("RTF file not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading RTF file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            VisitorWindow visitorWindow = new VisitorWindow();
            visitorWindow.Show();
            this.Close();
        }
    }
}
