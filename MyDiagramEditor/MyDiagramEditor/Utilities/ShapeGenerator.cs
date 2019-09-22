using System;
using System.Windows;
using System.Windows.Media;

namespace MyDiagramEditor.Utilities
{
    public class ShapeGenerator
    {
        public ShapeGenerator() { }
        public string Create(string type)
        {
            if (type == "artefact") { return CreateArtefact(); }
            else if (type == "activite") { return CreateActivite(); }
            else if (type == "classe") { return CreateClasse(); }
            else if (type == "commentaire") { return CreateCommentaire(); }
            else if (type == "phase") { return CreatePhase(); }
            else if (type == "role") { return CreateRole(); }
            else { return null; }
        }
        public string CreateArtefact(int width = 80, int height = 100)
        {
            PathGeometry pg = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(0, 0);
            pathFigure.IsClosed = true;

            LineSegment ls = new LineSegment();
            ls.Point = new Point(5 * width / 8, 0);
            pathFigure.Segments.Add(ls);

            //
            LineSegment ls11 = new LineSegment();
            ls11.Point = new Point(width, 3 * height / 10);
            pathFigure.Segments.Add(ls11);

            LineSegment ls3 = new LineSegment();
            ls3.Point = new Point(width, height);
            pathFigure.Segments.Add(ls3);

            LineSegment ls4 = new LineSegment();
            ls4.Point = new Point(0, height);
            pathFigure.Segments.Add(ls4);

            LineGeometry myLineGeometry12 = new LineGeometry();
            myLineGeometry12.StartPoint = new Point(5 * width / 8, 0);
            myLineGeometry12.EndPoint = new Point(5 * width / 8, 3 * height / 10);

            LineGeometry myLineGeometry13 = new LineGeometry();
            myLineGeometry13.StartPoint = new Point(5 * width / 8, 3 * height / 10);
            myLineGeometry13.EndPoint = new Point(width, 3 * height / 10);
            pg.Figures.Add(pathFigure);
            pg.AddGeometry(myLineGeometry12);
            pg.AddGeometry(myLineGeometry13);
            return pg.ToString();
        }
        public string CreateActivite(int width = 100, int height = 60)
        {
            PathGeometry pg = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(0, 0);
            pathFigure.IsClosed = true;

            LineSegment ls = new LineSegment();
            ls.Point = new Point(8 * width / 10, 0);
            pathFigure.Segments.Add(ls);

            LineSegment ls1 = new LineSegment();
            ls1.Point = new Point(width, height / 2);
            pathFigure.Segments.Add(ls1);

            LineSegment ls2 = new LineSegment();
            ls2.Point = new Point(8 * width / 10, height);
            pathFigure.Segments.Add(ls2);

            LineSegment ls3 = new LineSegment();
            ls3.Point = new Point(0, height);
            pathFigure.Segments.Add(ls3);

            LineSegment ls4 = new LineSegment();
            ls4.Point = new Point(width / 5, height / 2);
            pathFigure.Segments.Add(ls4);


            pg.Figures.Add(pathFigure);
            return pg.ToString();
        }
        public string CreateRole(int width = 40, int height = 60)
        {
            EllipseGeometry myEllipseGeometry = new EllipseGeometry();
            myEllipseGeometry.Center = new System.Windows.Point(width / 2, height / 6);
            myEllipseGeometry.RadiusX = width / 4;
            myEllipseGeometry.RadiusY = height / 6;

            LineGeometry myLineGeometry1 = new LineGeometry();
            myLineGeometry1.StartPoint = new Point(width / 2, height / 3);
            myLineGeometry1.EndPoint = new Point(width / 2, 5 * height / 6);

            LineGeometry myLineGeometry2 = new LineGeometry();
            myLineGeometry2.StartPoint = new Point(0, height / 2);
            myLineGeometry2.EndPoint = new Point(width, height / 2);

            LineGeometry myLineGeometry3 = new LineGeometry();
            myLineGeometry3.StartPoint = new Point(width / 2, 5 * height / 6);
            myLineGeometry3.EndPoint = new Point(width / 8, height);

            LineGeometry myLineGeometry4 = new LineGeometry();
            myLineGeometry4.StartPoint = new Point(width / 2, 5 * height / 6);
            myLineGeometry4.EndPoint = new Point(7 * width / 8, height);

            PathGeometry pg = new PathGeometry();
            pg.AddGeometry(myLineGeometry1);
            pg.AddGeometry(myLineGeometry2);
            pg.AddGeometry(myLineGeometry3);
            pg.AddGeometry(myLineGeometry4);
            pg.AddGeometry(myEllipseGeometry);
            return pg.ToString();
        }
        public string CreateCommentaire(int width = 200, int height = 100)
        {
            PathGeometry pg = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(0, 0);
            pathFigure.IsClosed = true;

            LineSegment ls = new LineSegment();
            ls.Point = new Point(width, 0);
            pathFigure.Segments.Add(ls);

            LineSegment ls1 = new LineSegment();
            ls1.Point = new Point(width, height);
            pathFigure.Segments.Add(ls1);

            LineSegment ls2 = new LineSegment();
            ls2.Point = new Point(0, height);
            pathFigure.Segments.Add(ls2);
            pg.Figures.Add(pathFigure);

            return pg.ToString();
        }
        public string CreatePhase(int width = 400, int height = 200)
        {
            PathGeometry pg = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(0, 0);
            pathFigure.IsClosed = true;

            LineSegment ls = new LineSegment();
            ls.Point = new Point(width, 0);
            pathFigure.Segments.Add(ls);

            LineSegment ls1 = new LineSegment();
            ls1.Point = new Point(width, height);
            pathFigure.Segments.Add(ls1);

            LineSegment ls2 = new LineSegment();
            ls2.Point = new Point(0, height);
            pathFigure.Segments.Add(ls2);
            pg.Figures.Add(pathFigure);

            LineGeometry myLineGeometry = new LineGeometry();
            myLineGeometry.StartPoint = new Point(0, height / 10);
            myLineGeometry.EndPoint = new Point(width, height / 10);
            pg.AddGeometry(myLineGeometry);
            return pg.ToString();
        }
        public string CreateClasse(int width = 150, int height = 90)
        {
            PathGeometry pg = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(0, 0);
            pathFigure.IsClosed = true;

            LineSegment ls = new LineSegment();
            ls.Point = new Point(width, 0);
            pathFigure.Segments.Add(ls);

            LineSegment ls1 = new LineSegment();
            ls1.Point = new Point(width, height);
            pathFigure.Segments.Add(ls1);

            LineSegment ls2 = new LineSegment();
            ls2.Point = new Point(0, height);
            pathFigure.Segments.Add(ls2);
            pg.Figures.Add(pathFigure);

            LineGeometry myLineGeometry4 = new LineGeometry();
            myLineGeometry4.StartPoint = new Point(0, height / 3);
            myLineGeometry4.EndPoint = new Point(width, height / 3);

            LineGeometry myLineGeometry5 = new LineGeometry();
            myLineGeometry5.StartPoint = new Point(0, 2 * height / 3);
            myLineGeometry5.EndPoint = new Point(width, 2 * height / 3);

            pg.AddGeometry(myLineGeometry4);
            pg.AddGeometry(myLineGeometry5);
            return pg.ToString();
        }
    }
}
