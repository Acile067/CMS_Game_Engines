﻿using CMS_Game_Engines.Common;
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
        #region Initialize
        public User savedUser = new User();
        public ViewWindow(GameEngine engine, User user)
        {
            InitializeComponent();
            NameLabel.Content = engine.RtfFilePath;
            savedUser = user;
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

            string formattedDate = engine.DateAdded.ToString("dd-MM-yyyy");
            DateAddedLabel.Content = formattedDate;

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion

        #region BackBtn
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            TableWindow tableWindow = new TableWindow(savedUser);
            tableWindow.Show();
            this.Close();
        }
        #endregion
    }
}
