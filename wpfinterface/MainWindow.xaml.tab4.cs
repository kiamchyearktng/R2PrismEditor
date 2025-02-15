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
    // this file is for "risky" tab
    public partial class MainWindow : Window
    {
        private List<Label> segmentLabels = new();
        private List<ComboBox> segmentTypeComboBoxes = new();
        private List<ComboBox> segmentComboBoxes = new();

        private void TabRisky_Selected(object sender, RoutedEventArgs e)
        {
            if (selectedPrism != null)
            {
                PopulateSegments();
            }
            else
            {
                SegmentGrid.Children.Clear();
                segmentLabels.Clear();
                segmentTypeComboBoxes.Clear();
                segmentComboBoxes.Clear();
            }
        }

        private void PopulateSegments()
        {
            SegmentGrid.Children.Clear();
            segmentLabels.Clear();
            segmentTypeComboBoxes.Clear();
            segmentComboBoxes.Clear();

            for (int i = 0; i < selectedPrism!.segments.Count; i++)
            {
                var (label, typeComboBox, comboBox, changeButton) = CreateSegmentRow(SegmentGrid, 0, i);
                label.Content = $"Segment {i}: ";
                if (Segment.SegmentNames.Contains(selectedPrism!.segments[i].Name))
                {
                    typeComboBox.ItemsSource = Segment.SegmentTypes;
                    typeComboBox.SelectedIndex = 0;
                    comboBox.ItemsSource = Segment.SegmentNames;
                    comboBox.SelectedIndex = Segment.SegmentNames.IndexOf(selectedPrism!.segments[i].Name);
                }
                else if (Segment.FusionNames.Contains(selectedPrism!.segments[i].Name))
                {
                    typeComboBox.ItemsSource = Segment.SegmentTypes;
                    typeComboBox.SelectedIndex = 1;
                    comboBox.ItemsSource = Segment.FusionNames;
                    comboBox.SelectedIndex = Segment.FusionNames.IndexOf(selectedPrism!.segments[i].Name);
                }
                else if (Segment.LegsNames.Contains(selectedPrism!.segments[i].Name))
                {
                    typeComboBox.ItemsSource = Segment.LegsTypes;
                    typeComboBox.SelectedIndex = 0;
                    typeComboBox.IsEnabled = false;
                    comboBox.ItemsSource = Segment.LegsNames;
                    comboBox.SelectedIndex = Segment.LegsNames.IndexOf(selectedPrism!.segments[i].Name);
                }
                int index = i;
                changeButton.Click += (object sender, RoutedEventArgs e) => { ChangeSegmentButton_Click(sender, e, index); };
            }
        }

        private void ChangeSegmentButton_Click(object sender, RoutedEventArgs e, int i)
        {
            if (selectedPrism == null)
            {
                LogToOutput($"No prism selected");
                return;
            }

            switch (segmentTypeComboBoxes[i].SelectedValue.ToString()!)
            {
                case "Standard":
                    if (Segment.SegmentNames.Contains(segmentComboBoxes[i].SelectedValue.ToString()!))
                        break;
                    LogToOutput($"Error validating segment");
                    return;
                case "Fusion":
                    if (Segment.FusionNames.Contains(segmentComboBoxes[i].SelectedValue.ToString()!))
                        break;
                    LogToOutput($"Error validating segment");
                    return;
                case "Legendary":
                    if (Segment.LegsNames.Contains(segmentComboBoxes[i].SelectedValue.ToString()!))
                        break;
                    LogToOutput($"Error validating segment");
                    return;
                default:
                    LogToOutput($"Error validating segment");
                    return;
            }

            string oldname = selectedPrism.segments[i].Name;
            selectedPrism.segments[i].Name = segmentComboBoxes[i].SelectedValue.ToString()!;
            PopulateSegments();
            LogToOutput($"{GetPrismText(selectedPrism)}: {oldname} changed to {selectedPrism.segments[i].Name}");
        }

        private (Label label, ComboBox typeComboBox, ComboBox comboBox, Button changeButton) CreateSegmentRow(Grid grid, int col, int row)
        {
            Label label = new() {
            };
            Grid.SetColumn(label, col);
            Grid.SetRow(label, row);
            grid.Children.Add(label);
            fragmentLabels.Add(label);

            ComboBox comboTypeBox = new()
            {
                Width = 85,
                Height = 25,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new(5, 0, 0, 0),
                //SelectedValuePath = "Content",
            };
            Grid.SetColumn(comboTypeBox, col + 1);
            Grid.SetRow(comboTypeBox, row);
            grid.Children.Add(comboTypeBox);
            segmentTypeComboBoxes.Add(comboTypeBox);

            ComboBox comboBox = new() {
                Width = 220,
                Height = 25,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new(5, 0, 0, 0),
                //SelectedValuePath = "Content",
            };
            Grid.SetColumn(comboBox, col + 2);
            Grid.SetRow(comboBox, row);
            grid.Children.Add(comboBox);
            segmentComboBoxes.Add(comboBox);

            comboTypeBox.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                comboBox.ItemsSource = comboTypeBox.SelectedIndex > 0 ? Segment.FusionNames : Segment.SegmentNames;
                comboBox.SelectedIndex = 0;
            };

            Button changeButton = new()
            {
                MinWidth = 65,
                Width = 65,
                Height = 25,
                Margin = new(5, 0, 0, 0),
                Content = new Label() { Style = Resources["ButtonText"] as Style, Content = "Change", },
            };
            Grid.SetColumn(changeButton, col + 3);
            Grid.SetRow(changeButton, row);
            grid.Children.Add(changeButton);

            return (label, comboTypeBox, comboBox, changeButton);
        }
    }
}