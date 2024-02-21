using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDimensioningTool.Module
{
    /// <summary>
    /// Utility Class to perform supporting operations.
    /// </summary>
    public static class UtilityClass
    {
        /// <summary>
        /// Extension method to find the project poiint.
        /// </summary>
        /// <param name="plane">Plane on which the point is projected</param>
        /// <param name="point">Point to project on plane</param>
        /// <returns>the projected point</returns>
        public static XYZ ProjectOnto(this Plane plane, XYZ point)
        {
            return point-plane.Normal.DotProduct(point-plane.Origin)*plane.Normal;
        }
    }
}
