using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Newtonsoft.Json; // Install-Package Newtonsoft.Json

namespace NumpadShortcutsApp
{
    public partial class MainWindow : Window
    {
        private string selectedKey = "";
        private Dictionary<string, NumpadActions> keyActions = new Dictionary<string, NumpadActions>();
        private string saveFile = "keyActions.json";

        public MainWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;

            LoadKeyActions();
            UpdateNumpadStatusEllipse();
        }

        private void SelectKey(string key)
        {
            selectedKey = key;
            SelectedKeyBadge.Text = "Key: " + selectedKey;

            if (keyActions.ContainsKey(selectedKey))
            {
                NumpadActions action = keyActions[selectedKey];
                TargetTextBox.Text = action.Path;
                ArgumentsTextBox.Text = action.Arguments;
                if (action.ActionType == "Open App")
                    ActionTypeComboBox.SelectedIndex = 0;
                else
                    ActionTypeComboBox.SelectedIndex = 1;
            }
            else
            {
                TargetTextBox.Text = "";
                ArgumentsTextBox.Text = "";
                ActionTypeComboBox.SelectedIndex = 0;
            }
        }

        private void NumpadButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
                SelectKey(button.Content.ToString());
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            string key = e.Key.ToString();
            if (key.StartsWith("NumPad"))
            {
                string numKey = key.Replace("NumPad", "");
                SelectKey(numKey);
                e.Handled = true;

                if (keyActions.ContainsKey(numKey))
                    ExecuteAction(keyActions[numKey]);
            }
        }

        private void ExecuteAction(NumpadActions action)
        {
            if (action.ActionType == "Open App" && File.Exists(action.Path))
            {
                try
                {
                    Process.Start(action.Path, action.Arguments);
                }
                catch { }
            }
            else if (action.ActionType == "Play Sound" && File.Exists(action.Path))
            {
                try
                {
                    System.Windows.Media.MediaPlayer player = new System.Windows.Media.MediaPlayer();
                    player.Open(new Uri(action.Path));
                    player.Play();
                }
                catch { }
            }
        }

        private void ActionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selected = ActionTypeComboBox.SelectedItem as ComboBoxItem;
            if (selected != null)
            {
                if (selected.Content.ToString() == "Open App")
                    PathLabel.Text = "Select Application (.exe)";
                else
                    PathLabel.Text = "Select Sound (.mp3/.wav)";
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selected = ActionTypeComboBox.SelectedItem as ComboBoxItem;
            if (selected != null)
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                if (selected.Content.ToString() == "Open App")
                    dlg.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
                else
                    dlg.Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav|All files (*.*)|*.*";

                if (dlg.ShowDialog() == true)
                    TargetTextBox.Text = dlg.FileName;
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedKey)) return;
            if (keyActions.ContainsKey(selectedKey))
                ExecuteAction(keyActions[selectedKey]);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedKey)) return;

            ComboBoxItem selected = ActionTypeComboBox.SelectedItem as ComboBoxItem;
            string type = "Open App";
            if (selected != null)
                type = selected.Content.ToString();

            NumpadActions action = new NumpadActions();
            action.ActionType = type;
            action.Path = TargetTextBox.Text;
            action.Arguments = ArgumentsTextBox.Text;

            keyActions[selectedKey] = action;
            SaveKeyActions();
        }

        private void SaveKeyActions()
        {
            try
            {
                string json = JsonConvert.SerializeObject(keyActions, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(saveFile, json, Encoding.UTF8);
            }
            catch { }
        }

        private void LoadKeyActions()
        {
            try
            {
                if (File.Exists(saveFile))
                {
                    string json = File.ReadAllText(saveFile, Encoding.UTF8);
                    keyActions = JsonConvert.DeserializeObject<Dictionary<string, NumpadActions>>(json);
                }
            }
            catch { }
        }

        private void UpdateNumpadStatusEllipse()
        {
            // Simple detection: check if any NumPad keys exist in the current system (approximation)
            bool anyNumPad = false;
            foreach (Key k in Enum.GetValues(typeof(Key)))
            {
                if (k.ToString().StartsWith("NumPad"))
                {
                    anyNumPad = true;
                    break;
                }
            }

            if (anyNumPad)
                NumpadStatusEllipse.Fill = new SolidColorBrush(Colors.Green);
            else
                NumpadStatusEllipse.Fill = new SolidColorBrush(Colors.Red);
        }
    }
}
