using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDimensioningTool.Module
{
    /// <summary>
    /// Selection filter class to only enable the user to selects ducts.
    /// </summary>
    public class SelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (!(elem is null)  && elem.Category.Name=="Ducts")
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
