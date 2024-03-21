﻿using CMS_Game_Engines.Common;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Xml.Serialization;

namespace CMS_Game_Engines.Windows
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        
        public ObservableCollection<GameEngine> GameEngines { get; set; }
        public AdminWindow()
        {
            InitializeComponent();
            DataContext = this; 
            LoadGameEnginesFromXml();
            
        }

        private void LoadGameEnginesFromXml()
        {
            string xmlFilePath = "../../DataBase/game_engine.xml";

            if (File.Exists(xmlFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<GameEngine>));
                using (FileStream fileStream = new FileStream(xmlFilePath, FileMode.Open))
                {
                    GameEngines = (ObservableCollection<GameEngine>)serializer.Deserialize(fileStream);
                }
            }
            else
            {
                
                GameEngines = new ObservableCollection<GameEngine>();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to sing out?", "Sing out Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void ADDBtn_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new AddWindow();
            addWindow.Show();
            this.Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {

            var textBlock = (TextBlock)sender;

            // Dobijanje stavke (objekta) koja je povezana sa tekstualnim blokom
            var item = textBlock.DataContext as GameEngine; // Zamijenite YourItemClass sa stvarnim imenom vaše klase

            // Provera da li je stavka pronađena
            if (item != null)
            {
                EditWindow editWindow = new EditWindow(item);
                editWindow.Show();
                this.Close();
                
            }
            else
            {
                // Ako nije pronađena stavka, možete prikazati odgovarajuću poruku ili rukovati drugačije
                MessageBox.Show("Item not found.");
            }

        }
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            int cnt = 0;
            foreach(GameEngine sectedEngine in GameEnginesDataGrid.Items) {
                if (sectedEngine.IsSelected)
                {
                    cnt++;
                    break;
                }   
            }

            if (cnt!= 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to remove items?", "Delete Items Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    AdminWindow adminWindow = (AdminWindow)Application.Current.MainWindow;
                    List<GameEngine> remainingEngines = new List<GameEngine>();

                    foreach (GameEngine engine in GameEnginesDataGrid.Items)
                    {
                        if (engine.IsSelected)
                        {

                            string rtfFilePath = "../../RTF/" + engine.RtfFilePath;
                            if (!rtfFilePath.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
                            {
                                rtfFilePath += ".rtf";
                            }

                            if (File.Exists(rtfFilePath))
                            {
                                File.Delete(rtfFilePath);
                            }
                            continue;
                        }
                        remainingEngines.Add(engine);
                    }

                    GameEnginesDataGrid.ItemsSource = remainingEngines;


                    XmlSerializer serializer = new XmlSerializer(typeof(List<GameEngine>));
                    using (TextWriter writer = new StreamWriter("../../DataBase/game_engine.xml"))
                    {
                        serializer.Serialize(writer, remainingEngines);
                    }

                    MessageBox.Show("All items have been successfully deleted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No item from the table is selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GameEnginesDataGrid.Items.SortDescriptions.Add(new SortDescription("DateAdded", ListSortDirection.Descending));
        }
    }
}
