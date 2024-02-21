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
    /// Event handler class.
    /// </summary>
    public class ExternalEventClass : IExternalEventHandler
    { 

        private AutoDimensionUI autoDimensionUI;
        private Reference firstRef; 
        private Reference secondRef; 
        private Line directionLine;
        private Plane plane; 
        public ExternalEventClass(AutoDimensionUI autoDimensionUI)
        { 
            this.autoDimensionUI = autoDimensionUI; }
        public void Execute(UIApplication app)
        { 
            Document document = app.ActiveUIDocument.Document; 
            Selection selection = app.ActiveUIDocument.Selection;
            Reference gridReferences = selection.PickObject(ObjectType.Element, "Select Grid");
            Reference ductReference = selection.PickObject(ObjectType.Element, new SelectionFilter(), "Select Duct");
            List<Edge> edgesParallelToGrid = new List<Edge>(); 
            List<Edge> edgePerpendicularToGrid = new List<Edge>();
            using (Transaction transaction = new Transaction(document, "AutoDim"))
            {
                transaction.Start(); 
                Options opt = new Options();
                opt.ComputeReferences = true;
                opt.IncludeNonVisibleObjects = true; 
                opt.View = document.ActiveView;
                Element element = document.GetElement(gridReferences);
                XYZ gridDirection = null;
                if (element != null)
                { 
                    foreach (GeometryObject obj in element.get_Geometry(opt))
                    { 
                        if (obj is Line) 
                        {
                            Line l = obj as Line;
                            firstRef = l.Reference;
                        } 
                    }
                    Grid grid = element as Grid; 
                    Line gridLine = grid.Curve as Line;
                    gridDirection = gridLine.Direction;
                    XYZ normalOfPlane = gridLine.Direction.CrossProduct(document.ActiveView.ViewDirection);
                    plane = Plane.CreateByNormalAndOrigin(normalOfPlane, gridLine.GetEndPoint(0));
                }
                Element ductElement = document.GetElement(ductReference); 
                if (ductElement != null) 
                {
                    Options options = new Options();
                    options.ComputeReferences = true;
                    options.DetailLevel = ViewDetailLevel.Fine;
                    PlanarFace requiredFace = FindPlanarFace(ductElement, options, document); 
                    if (requiredFace != null)
                    { 
                        EdgeArrayArray edg = requiredFace.EdgeLoops;
                        foreach (Edge edge in edg.Cast<EdgeArray>().SelectMany(y => y.Cast<Edge>()).ToList()) 
                        { 
                            XYZ edgeDirection = (edge.AsCurve() as Line)?.Direction; 
                            if (edgeDirection != null && (edgeDirection.DotProduct(gridDirection) == 1) || edgeDirection.DotProduct(gridDirection.Negate()) == 1) 
                            { 
                                edgesParallelToGrid.Add(edge);
                            } 
                            else
                            {
                                edgePerpendicularToGrid.Add(edge);
                            }
                        }
                        if ((bool)autoDimensionUI.allDimrb.IsChecked)
                        { 
                            if (edgesParallelToGrid.Count > 0)
                            { 
                                ReferenceArray referenceArray = new ReferenceArray();
                                referenceArray.Append(firstRef); 
                                foreach (Edge edge in edgesParallelToGrid)
                                { 
                                    XYZ projectionPoint = plane.ProjectOnto(edge.AsCurve().GetEndPoint(0)); 
                                    directionLine = Line.CreateBound(projectionPoint, edge.AsCurve().GetEndPoint(0));
                                    secondRef = edge.Reference; referenceArray.Append(secondRef); 
                                    document.Create.NewDimension(document.ActiveView, directionLine, referenceArray);
                                }
                                ReferenceArray referenceArray1 = new ReferenceArray(); 
                                referenceArray1.Append(edgesParallelToGrid[0].Reference);
                                referenceArray1.Append(edgesParallelToGrid[1].Reference); 
                                directionLine = Line.CreateBound(edgesParallelToGrid[0].AsCurve().GetEndPoint(0) + edgesParallelToGrid[0].AsCurve().GetEndPoint(1) * 0.5, edgesParallelToGrid[1].AsCurve().GetEndPoint(0) + edgesParallelToGrid[1].AsCurve().GetEndPoint(1) * 0.5);
                                document.Create.NewDimension(document.ActiveView, directionLine, referenceArray1);
                            } 
                            if (edgePerpendicularToGrid.Count > 0)
                            { 
                                ReferenceArray referenceArray1 = new ReferenceArray(); 
                                referenceArray1.Append(edgePerpendicularToGrid[0].Reference);
                                referenceArray1.Append(edgePerpendicularToGrid[1].Reference);
                                directionLine = Line.CreateBound(edgePerpendicularToGrid[0].AsCurve().GetEndPoint(0) + edgePerpendicularToGrid[0].AsCurve().GetEndPoint(1) * 0.5, edgePerpendicularToGrid[1].AsCurve().GetEndPoint(0) + edgePerpendicularToGrid[1].AsCurve().GetEndPoint(1) * 0.5);
                                document.Create.NewDimension(document.ActiveView, directionLine, referenceArray1);
                            } 
                        } 
                        else
                        { 
                            if (autoDimensionUI.dimensionOptioncmb.SelectedItem == "Nearest")
                            {
                                double minimumDistance = double.MaxValue;
                                foreach (Edge edge in edgesParallelToGrid)
                                {
                                    XYZ projectionPoint = plane.ProjectOnto(edge.AsCurve().GetEndPoint(0)); 
                                    double distance = projectionPoint.DistanceTo(edge.AsCurve().GetEndPoint(0));
                                    if (distance < minimumDistance) { minimumDistance = distance; 
                                        secondRef = edge.Reference;
                                        directionLine = Line.CreateBound(projectionPoint, edge.AsCurve().GetEndPoint(0));
                                    }
                                }
                                ReferenceArray referenceArray = new ReferenceArray(); 
                                referenceArray.Append(firstRef); referenceArray.Append(secondRef);
                                document.Create.NewDimension(document.ActiveView, directionLine, referenceArray);
                            } 
                            else
                            { 
                                double maximumDistance = double.MinValue;
                                foreach (Edge edge in edgesParallelToGrid)
                                {
                                    XYZ projectionPoint = plane.ProjectOnto(edge.AsCurve().GetEndPoint(0)); 
                                    double distance = projectionPoint.DistanceTo(edge.AsCurve().GetEndPoint(0));
                                    if (distance > maximumDistance) 
                                    { 
                                        maximumDistance = distance;
                                        secondRef = edge.Reference;
                                        directionLine = Line.CreateBound(projectionPoint, edge.AsCurve().GetEndPoint(0));
                                    } 
                                } 
                                ReferenceArray referenceArray = new ReferenceArray(); 
                                referenceArray.Append(firstRef);
                                referenceArray.Append(secondRef);
                                document.Create.NewDimension(document.ActiveView, directionLine, referenceArray); 
                            }
                        } 
                    }
                    transaction.Commit();
                } 
            } 
        }
        public string GetName() 
        { 
            return "ExternalEventClass";
        }

        /// <summary>
        /// Finds the required planer face of the duct element.
        /// </summary>
        /// <param name="ductElement">Duct element</param>
        /// <param name="options"></param>
        /// <param name="document">Current revit document</param>
        /// <returns>the required planar face</returns>
        public PlanarFace FindPlanarFace(Element ductElement, Options options, Document document)
        {
            PlanarFace face = null; 
            ductElement.get_Geometry(options).Cast<Solid>().Select(x => x.Faces).Cast<FaceArray>().SelectMany(y => y.Cast<Face>()).ToList().ForEach(x =>
            { 
                if (x is PlanarFace planarFace)
                {
                    if (planarFace.FaceNormal.X == document.ActiveView.ViewDirection.X && planarFace.FaceNormal.Y == document.ActiveView.ViewDirection.Y && planarFace.FaceNormal.Z == document.ActiveView.ViewDirection.Z) 
                    { 
                        face = planarFace;
                    } 
                }
            });
            return face; 
        } 
    } 
}