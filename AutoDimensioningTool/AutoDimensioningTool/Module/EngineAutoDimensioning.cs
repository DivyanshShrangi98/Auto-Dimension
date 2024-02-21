using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using AutoDimensioningTool.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDimensioningTool.Module
{
    /// <summary>
    /// Engine or main class for Auto Dimensioning for ducts in revit.
    /// </summary>
    /// 

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class EngineAutoDimensioning : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            AutoDimensionUI autoDimensionUI = new AutoDimensionUI();
            autoDimensionUI.Height = 300;
            autoDimensionUI.Width = 325;
            autoDimensionUI.Topmost = true;
            autoDimensionUI.Show();

            return Result.Succeeded;
        }


    }
}
