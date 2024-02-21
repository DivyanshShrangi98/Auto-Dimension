using Autodesk.Revit.UI;
using AutoDimensioningTool.Module;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoDimensioningTool.UI
{
    /// <summary>
    /// Interaction logic for AutoDimensionUI.xaml
    /// </summary>
    public partial class AutoDimensionUI : Window
    {
        public ExternalEvent externalEvent { get; set; }
        public ExternalEventClass externalEventManager { get; set; }
        public AutoDimensionUI()
        {
            InitializeComponent();
            externalEventManager = new ExternalEventClass(this); externalEvent = ExternalEvent.Create(externalEventManager);
            dimensionOptioncmb.Items.Add("Nearest");
            dimensionOptioncmb.Items.Add("OuterMost");
            dimensionOptioncmb.IsEnabled = false;
        }

        private void customDimrb_Checked(object sender, RoutedEventArgs e)
        {
            dimensionOptioncmb.IsEnabled = true;
        }

        private void processBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)allDimrb.IsChecked && !(bool)customDimrb.IsChecked)
            {
                TaskDialog.Show("Information", "Please choose either all dimension or custom dimension option to continue.");

                return;
            }
            if ((bool)customDimrb.IsChecked && dimensionOptioncmb.SelectedItem == null)
            {
                TaskDialog.Show("Information", "Please choose the dimension position from the drop down to continue.");
                return;
            }
            externalEvent.Raise();
        }
    }
}
