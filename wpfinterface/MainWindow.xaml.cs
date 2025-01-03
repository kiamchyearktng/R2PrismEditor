using System.Diagnostics;
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
using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Parts;
using lib.remnant2.saves.Model.Properties;
using lib.remnant2.saves.Navigation;
using prismeditor.definitions;
using prismeditor.scripts;

namespace wpfinterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// this file is for main window shared functionality and "browse" tab
    public partial class MainWindow : Window
    {
        private SaveFile? saveFile;
        private Navigator? navigator;
        private List<PrismData> prisms = [];
        private PrismData? selectedPrism = null;

        private const string BLANK_INDICATOR = "-";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoadSteamButton_Click(object sender, RoutedEventArgs e)
        {
            var r = await Scripts.LoadSteam(LogToOutput);
            if (r.status == 0)
            {
                saveFile = r.sf;
                navigator = r.nav;
                PopulatePrisms();
            }
        }

        private async void LoadStaticButton_Click(object sender, RoutedEventArgs e)
        {
            var r = await Scripts.LoadStatic(LogToOutput);
            if (r.status == 0)
            {
                saveFile = r.sf;
                navigator = r.nav;
                PopulatePrisms();
            }
        }

        private async void SaveSteamButton_Click(object sender, RoutedEventArgs e)
        {
            if (saveFile == null)
            {
                LogToOutput("No save file loaded");
                return;
            }
            var r = await Scripts.SaveSteam(saveFile, LogToOutput);
        }

        private async void SaveStaticButton_Click(object sender, RoutedEventArgs e)
        {
            if (saveFile == null)
            {
                LogToOutput("No save file loaded");
                return;
            }
            var r = await Scripts.SaveStatic(saveFile, LogToOutput);
        }

        private async void PopulatePrisms()
        {
            prisms.Clear();
            if (!TabReadme.IsSelected && !TabBrowse.IsSelected)
            {
                TabBrowse.IsSelected = true;
            }

            var r = await Scripts.GetPrisms(navigator!);
            if (r.status != 0)
            {
                ResetBrowsePage();
                return;
            }

            prisms.AddRange(r.prismDatas!);

            LogToOutput($"{prisms.Count} prisms found");
            if (prisms.Count == 0)
            {
                ResetBrowsePage();
                return;
            }

            PrismListBox.Items.Clear();
            for(int i = 0; i < prisms.Count; i++)
            {
                ListBoxItem newItem = new ListBoxItem()
                {
                    Content = GetPrismText(prisms[i]),
                };
                PrismData thisPrism = prisms[i];
                newItem.Selected += (object sender, RoutedEventArgs e) =>
                {
                    selectedPrism = thisPrism;
                    PopulatePrismInfo(thisPrism);
                };
                PrismListBox.Items.Add(newItem);
            }

            (PrismListBox.Items[0] as ListBoxItem)!.IsSelected = true;
        }

        private void PopulatePrismInfo(PrismData prismData)
        {
            string internalLevel = NullToString(prismData.internalLevel);
            string experience = NullToString(prismData.experience);
            string currentSeed = NullToString(prismData.currentSeed);
            string pendingRoll = NullToString(prismData.pendingRoll);

            int uiLevelCount = 0;
            string segments = "";
            string fedFragments = "";

            for (int i = 0; i < prismData.segments.Count; i++)
            {
                int slevel = prismData.segments[i].Level;
                string sname = prismData.segments[i].Name;
                uiLevelCount += slevel;
                segments += $"{sname} ({slevel})\n";
            }

            for (int i = 0; i < prismData.fragments.Count; i++)
            {
                var flevel = prismData.fragments[i].Level;
                string fname = prismData.fragments[i].Name;
                fedFragments += $"{fname} ({flevel}/32)\n";
            }

            browse_level.Content = $"{uiLevelCount} ({internalLevel})";
            browse_exp.Content = experience;
            browse_seed.Content = currentSeed;
            browse_pending.Content = pendingRoll;
            browse_segments.Content = segments.Trim();
            browse_fragments.Content = fedFragments.Trim();
        }

        private void NotImplementedButton_Click(object sender, RoutedEventArgs e)
        {
            LogToOutput("This feature has not been implemented yet.");
        }
        private void TabBrowse_Selected(object sender, RoutedEventArgs e)
        {
            if (selectedPrism != null)
            {
                PopulatePrismInfo(selectedPrism);
            }
            else
            {
                ResetBrowsePage();
            }
        }

        public void ResetBrowsePage()
        {
            selectedPrism = null;
            browse_level.Content = BLANK_INDICATOR;
            browse_exp.Content = BLANK_INDICATOR;
            browse_seed.Content = BLANK_INDICATOR;
            browse_pending.Content = BLANK_INDICATOR;
            browse_segments.Content = BLANK_INDICATOR;
            browse_fragments.Content = BLANK_INDICATOR;
        }

        public static string NullToString(object? input)
        {
            if (input == null) return BLANK_INDICATOR;
            string? r = input.ToString();
            if (r == null) return BLANK_INDICATOR;
            return input.ToString()!;
        }

        public static string GetPrismText(PrismData prismData)
        {
            return $"char {prismData.charIndex + 1}, prism {prismData.prismIndex + 1}";
        }

        private void NumberBoxCheck(object sender, TextCompositionEventArgs e)
        {
            try
            {
                TextBox textBox = (TextBox)sender;
                var oldText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);
                string newText = oldText.Insert(textBox.CaretIndex, e.Text);
                e.Handled = !int.TryParse(newText, out var _);
            }
            catch (Exception ex)
            {
                LogToOutput($"Error ({ex.GetType()}): {ex.Message}");
                e.Handled = true;
            }
        }

        public void LogToOutput(string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                bool scroll = OutputBox.ExtentHeight < OutputBox.ViewportHeight
                    || OutputBox.VerticalOffset > OutputBox.ExtentHeight - OutputBox.ViewportHeight - OutputBox.FontSize * 0.5;
                OutputBox.Text += $"\n{message}";
                if (scroll) { OutputBox.ScrollToEnd(); }
            }));
        }
    }
}