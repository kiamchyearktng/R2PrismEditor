using prismeditor.scripts;
using prismeditor.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wpfinterface
{
    // this file is for "safe" tab
    public partial class MainWindow : Window
    {
        private void TabSafe_Selected(object sender, RoutedEventArgs e)
        {
            ReseedAllButton.IsEnabled = prisms.Count > 0;
            if (selectedPrism != null)
            {
                safe_selected.Content = GetPrismText(selectedPrism);
                safe_seed.Content = selectedPrism.currentSeed.ToString();
                SeedInputBox.Text = selectedPrism.currentSeed.ToString();
                safe_pending.Content = selectedPrism.pendingRoll.ToString();
                ChangeSeedButton.IsEnabled = true;
                RandomSeedButton.IsEnabled = true;
                ResetPendingButton.IsEnabled = selectedPrism.pendingRoll;
            }
            else
            {
                safe_selected.Content = BLANK_INDICATOR;
                safe_seed.Content = BLANK_INDICATOR;
                SeedInputBox.Text = "";
                safe_pending.Content = BLANK_INDICATOR;
                ChangeSeedButton.IsEnabled = false;
                RandomSeedButton.IsEnabled = false;
                ResetPendingButton.IsEnabled = false;
            }
        }

        private void ResetPending()
        {
            if (selectedPrism!.pendingRoll)
            {
                selectedPrism.pendingRoll = false;
                safe_pending.Content = false.ToString();
                ResetPendingButton.IsEnabled = false;
            }
        }

        private void ChangeSeed(int? newSeed = null)
        {
            ResetPending();
            selectedPrism!.currentSeed = newSeed ?? Random.Shared.Next();
            safe_seed.Content = selectedPrism.currentSeed.ToString();
            SeedInputBox.Text = selectedPrism.currentSeed.ToString();
        }

        private void ReseedAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (prisms.Count == 0)
            {
                LogToOutput($"No prisms found");
                return;
            }
            int rollCount = 0;
            foreach (PrismData prismData in prisms)
            {
                if (prismData.pendingRoll) {
                    rollCount++;
                    prismData.pendingRoll = false;
                }
                prismData.currentSeed = Random.Shared.Next();
            }
            if (selectedPrism != null)
            {
                safe_seed.Content = selectedPrism.currentSeed.ToString();
                SeedInputBox.Text = selectedPrism.currentSeed.ToString();
            }
            else
            {
                safe_seed.Content = BLANK_INDICATOR;
                SeedInputBox.Text = "";
            }
            safe_pending.Content = false.ToString();
            ResetPendingButton.IsEnabled = false;
            LogToOutput($"{prisms.Count} prisms randomised ({rollCount} pending choices removed)");
        }

        private void ChangeSeedButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPrism == null)
            {
                LogToOutput($"No prism selected");
                return;
            }
            if (!int.TryParse(SeedInputBox.Text, out int newSeed))
            {
                LogToOutput($"Invalid input, choose an integer between {int.MinValue} and {int.MaxValue}");
                return;
            }
            if (newSeed == selectedPrism.currentSeed)
            {
                LogToOutput($"New seed is same as current seed");
                return;
            }
            ChangeSeed(newSeed);
            LogToOutput($"{GetPrismText(selectedPrism)}: seed changed to {selectedPrism.currentSeed}");
        }

        private void RandomSeedButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPrism == null)
            {
                LogToOutput($"No prism selected");
                return;
            }
            ChangeSeed(null);
            LogToOutput($"{GetPrismText(selectedPrism)}: seed randomised to {selectedPrism.currentSeed}");
        }

        private void ResetPendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPrism == null)
            {
                LogToOutput($"No prism selected");
                return;
            }
            ResetPending();
            LogToOutput($"{GetPrismText(selectedPrism)}: pending roll reset");
        }
    }
}
