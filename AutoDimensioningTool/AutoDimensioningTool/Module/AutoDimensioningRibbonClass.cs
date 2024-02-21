using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;



namespace AutoDimensioningTool.Module
{
    /// <summary>
    /// Ribbon class to create a new tab, panel and push button for Auto Dimension tool.
    /// </summary>
    public class AutoDimensioningRibbonClass : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        public Result OnStartup(UIControlledApplication application)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);



            string assemblyName = String.Format("{0}\\AutoDim.dll", path);



            application.CreateRibbonTab("Auto Dimension");



            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Auto Dimension", "Auto Dimension Panel");



            PushButtonData pushButtonData = new PushButtonData("autoDimBtn", "Auto Dimension", assemblyName, "AutoDim.EngineAutoDim");



            PushButton pushButton = ribbonPanel.AddItem(pushButtonData) as PushButton;



            pushButton.LargeImage = BmpImageSource("AutoDim.Image.AutoDim.bmp");



            return Result.Succeeded;
        }
        private System.Windows.Media.ImageSource BmpImageSource(string embeddedPath)
        {
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedPath);
            var decoder = new System.Windows.Media.Imaging.BmpBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);



            return decoder.Frames[0];
        }



    }
}