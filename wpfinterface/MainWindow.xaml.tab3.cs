using prismeditor.scripts;
using prismeditor.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using lib.remnant2.saves.Model.Parts;
using System.Diagnostics;

namespace wpfinterface
{
    // this file is for "unsafe" tab
    public partial class MainWindow : Window
    {
        private List<Label> fragmentLabels = new();
        private List<TextBox> fragmentTextBoxes = new();

        private void TabUnsafe_Selected(object sender, RoutedEventArgs e)
        {
            if (selectedPrism != null)
            {
                unsafe_selected.Content = GetPrismText(selectedPrism, true);
                unsafe_exp.Content = selectedPrism.experience.ToString();
                ExpInputBox.Text = selectedPrism.experience.ToString();
                ChangeExpButton.IsEnabled = selectedPrism.uiLevel < 51;
                ZeroExpButton.IsEnabled = selectedPrism.experience > 0;
                PopulateFragmentStrengths();
            }
            else
            {
                unsafe_selected.Content = BLANK_INDICATOR;
                unsafe_exp.Content = BLANK_INDICATOR;
                ExpInputBox.Text = "";
                ChangeExpButton.IsEnabled = false;
                ZeroExpButton.IsEnabled = false;
                FragmentGrid.Children.Clear();
                fragmentLabels.Clear();
                fragmentTextBoxes.Clear();
            }
        }

        private void PopulateFragmentStrengths()
        {
            FragmentGrid.Children.Clear();
            fragmentLabels.Clear();
            fragmentTextBoxes.Clear();

            bool columns = selectedPrism!.fragments.Count > 5;
            int half = (selectedPrism!.fragments.Count + 1) / 2;
            for (int i = 0; i < selectedPrism!.fragments.Count; i++)
            {
                var (label, textBox, changeButton) = CreateFragmentStrengthRow(FragmentGrid, columns && i >= half ? 5 : 0, columns && i >= half ? i - half : i);
                label.Content = $"{selectedPrism.fragments[i].Name} ({selectedPrism.fragments[i].Level}/32)";
                textBox.Text = $"{selectedPrism.fragments[i].Level}";
                int index = i;
                changeButton.Click += (object sender, RoutedEventArgs e) => { ChangeFragmentStrengthButton_Click(sender, e, index); };
            }
        }

        private void ChangeFragmentStrengthButton_Click(object sender, RoutedEventArgs e, int i)
        {
            if (selectedPrism == null)
            {
                LogToOutput($"No prism selected");
                return;
            }
            if (!int.TryParse(fragmentTextBoxes[i].Text, out int newStr) || newStr < 0 || newStr > 32)
            {
                LogToOutput($"Invalid input, choose an integer between {0} and {32}");
                return;
            }
            if (newStr == selectedPrism.fragments[i].Level)
            {
                LogToOutput($"New value is same as current value");
                return;
            }
            selectedPrism.fragments[i].Level = newStr;
            PopulateFragmentStrengths();
            LogToOutput($"{GetPrismText(selectedPrism)}: {selectedPrism.fragments[i].Name} fed level changed to {selectedPrism.fragments[i].Level}");
        }

        private void ChangeExp(int newExp)
        {
            selectedPrism!.experience = newExp;
            unsafe_exp.Content = selectedPrism.experience.ToString();
            ExpInputBox.Text = selectedPrism.experience.ToString();
            ZeroExpButton.IsEnabled = selectedPrism.experience > 0;
        }

        private void ChangeExpButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPrism == null)
            {
                LogToOutput($"No prism selected");
                return;
            }
            if (!int.TryParse(ExpInputBox.Text, out int newExp) || newExp < 0)
            {
                LogToOutput($"Invalid input, choose an integer between {0} and {int.MaxValue}");
                return;
            }
            if (newExp == selectedPrism.experience)
            {
                LogToOutput($"New value is same as current value");
                return;
            }
            ChangeExp(newExp);
            LogToOutput($"{GetPrismText(selectedPrism)}: experience changed to {selectedPrism.experience}");
        }

        private void ZeroExpButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPrism == null)
            {
                LogToOutput($"No prism selected");
                return;
            }
            if (selectedPrism.experience == 0)
            {
                LogToOutput($"Experience is already zero");
                return;
            }
            ChangeExp(0);
            LogToOutput($"{GetPrismText(selectedPrism)}: experience changed to {selectedPrism.experience}");
        }

        private (Label label, TextBox textBox, Button changeButton) CreateFragmentStrengthRow(Grid grid, int col, int row)
        {
            Label label = new() {
            };
            Grid.SetColumn(label, col);
            Grid.SetRow(label, row);
            grid.Children.Add(label);
            fragmentLabels.Add(label);

            TextBox textBox = new() {
                Width = 30,
                Height = 25,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new(5, 0, 0, 0),
            };
            textBox.PreviewTextInput += NumberBoxCheckFragment;
            Grid.SetColumn(textBox, col + 1);
            Grid.SetRow(textBox, row);
            grid.Children.Add(textBox);
            fragmentTextBoxes.Add(textBox);

            Button changeButton = new()
            {
                MinWidth = 65,
                Width = 65,
                Height = 25,
                Margin = new(5, 0, 0, 0),
                Content = new Label() { Style = Resources["ButtonText"] as Style, Content = "Change", },
            };
            Grid.SetColumn(changeButton, col + 2);
            Grid.SetRow(changeButton, row);
            grid.Children.Add(changeButton);

            return (label, textBox, changeButton);
        }
    }
}