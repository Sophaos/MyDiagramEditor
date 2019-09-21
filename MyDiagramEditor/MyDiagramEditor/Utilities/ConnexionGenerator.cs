using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MyDiagramEditor.Utilities
{
    public class ConnexionGenerator
    {
        public ConnexionGenerator() { }

        public string Create(string type, Point p1, Point p2)
        {
            if (type == "unidirectionnalLink") { return CreateUnidirectionalLink(p1, p2); }
            else if (type == "inheritanceLink") { return CreateInheritanceLink(p1, p2); }
            else if (type == "bidirectionnalLink") { return CreateBidirectionalLink(p1, p2); }
            else if (type == "aggregationLink" || type == "compositionLink") { return CreateAggregationCompositionLink(p1, p2); }
            else if (type == "lineConnexion") { return CreateLine(p1, p2); }
            else if (type == "directionnalArrow") { return CreateDirectionnalArrow(p1, p2); }
            else if (type == "bidirectionnalArrow") { return CreateBidirectionnalArrow(p1, p2); }
            else if (type == "reverseDirectionnalArrow") { return CreateReverseDirectionalArrow(p1, p2); }
            else { return null; }
        }

        public string CreateUnidirectionalLink(Point p1, Point p2)
        {
            PathGeometry pg = new PathGeometry();
            RotateTransform transform = new RotateTransform();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            transform.Angle = theta + 90;
            transform.CenterX = p2.X;
            transform.CenterY = p2.Y;

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(p1.X, p1.Y);

            LineSegment ls = new LineSegment(); //
            ls.Point = new Point(p2.X, p2.Y); //
            pathFigure.Segments.Add(ls);//
            pg.Figures.Add(pathFigure); //

            PathGeometry pg2 = new PathGeometry();
            PathFigure pathFigure2 = new PathFigure();
            pathFigure2.StartPoint = new Point(p2.X + 10, p2.Y + 15);

            LineSegment ls1 = new LineSegment();
            ls1.Point = new Point(p2.X, p2.Y);
            pathFigure2.Segments.Add(ls1);
            LineSegment ls2 = new LineSegment();
            ls2.Point = new Point(p2.X - 10, p2.Y + 15);
            pathFigure2.Segments.Add(ls2);
            pg2.Figures.Add(pathFigure2);

            pg2.Transform = transform;
            pg.AddGeometry(pg2);
            return pg.ToString();
        }

        public string CreateInheritanceLink(Point p1, Point p2)
        {
            PathGeometry pg = new PathGeometry();
            RotateTransform transform = new RotateTransform();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            transform.Angle = theta + 90;
            transform.CenterX = p2.X;
            transform.CenterY = p2.Y;

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(p1.X, p1.Y);

            LineSegment ls = new LineSegment(); //
            ls.Point = new Point(p2.X, p2.Y); //
            pathFigure.Segments.Add(ls);//
            pg.Figures.Add(pathFigure); //

            PathGeometry pg2 = new PathGeometry();
            PathFigure pathFigure2 = new PathFigure();
            pathFigure2.IsClosed = true;
            pathFigure2.StartPoint = new Point(p2.X + 10, p2.Y + 15);

            LineSegment ls1 = new LineSegment();
            ls1.Point = new Point(p2.X, p2.Y);
            pathFigure2.Segments.Add(ls1);
            LineSegment ls2 = new LineSegment();
            ls2.Point = new Point(p2.X - 10, p2.Y + 15);
            pathFigure2.Segments.Add(ls2);
            pg2.Figures.Add(pathFigure2);

            pg2.Transform = transform;
            pg.AddGeometry(pg2);

            // a updater
            string finalGeometry = pg.ToString();

            finalGeometry = finalGeometry.Remove(finalGeometry.IndexOf("L") + 1, finalGeometry.IndexOf(" ") - finalGeometry.IndexOf("L") - 1);

            double x = 15 * Math.Cos(theta * Math.PI / 180);
            double y = 15 * Math.Sin(theta * Math.PI / 180);
            double newX = p2.X - x;
            double newY = p2.Y - y;
            string fusion = newX.ToString() + "," + newY.ToString();
            finalGeometry = finalGeometry.Insert(finalGeometry.IndexOf("L") + 1, fusion);

            return finalGeometry;
        }

        public string CreateBidirectionalLink(Point p1, Point p2) { return CreateLine(p1, p2); }

        public string CreateAggregationCompositionLink(Point p1, Point p2)
        {
            PathGeometry pg = new PathGeometry();
            RotateTransform transform = new RotateTransform();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            transform.Angle = theta + 90;
            transform.CenterX = p1.X;
            transform.CenterY = p1.Y;

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(p1.X, p1.Y);

            LineSegment ls = new LineSegment(); //
            ls.Point = new Point(p2.X, p2.Y); //-30
            pathFigure.Segments.Add(ls);//
            pg.Figures.Add(pathFigure); //

            PathGeometry pg2 = new PathGeometry();
            PathFigure pathFigure2 = new PathFigure();
            pathFigure2.IsClosed = true;
            pathFigure2.StartPoint = new Point(p1.X + 10, p1.Y - 15);

            LineSegment ls1 = new LineSegment();
            ls1.Point = new Point(p1.X, p1.Y);
            pathFigure2.Segments.Add(ls1);

            LineSegment ls2 = new LineSegment();
            ls2.Point = new Point(p1.X - 10, p1.Y - 15);
            pathFigure2.Segments.Add(ls2);

            LineSegment ls3 = new LineSegment();
            ls3.Point = new Point(p1.X, p1.Y - 30);
            pathFigure2.Segments.Add(ls3);
            pg2.Figures.Add(pathFigure2);
            pg2.Transform = transform;

            pg.AddGeometry(pg2);

            // a updater
            string finalGeometry = pg.ToString();

            finalGeometry = finalGeometry.Remove(finalGeometry.IndexOf("M") + 1, finalGeometry.IndexOf("L") - finalGeometry.IndexOf("M") - 1);

            double x = 30 * Math.Cos(theta * Math.PI / 180);
            double y = 30 * Math.Sin(theta * Math.PI / 180);
            double newX = p1.X + x;
            double newY = p1.Y + y;
            string fusion = newX.ToString() + "," + newY.ToString();
            finalGeometry = finalGeometry.Insert(finalGeometry.IndexOf("M") + 1, fusion);
            return finalGeometry;
        }

        public string CreateLine(Point p1, Point p2)
        {
            PathGeometry pg = new PathGeometry();

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(p1.X, p1.Y);

            LineSegment ls = new LineSegment(); //
            ls.Point = new Point(p2.X, p2.Y); //
            pathFigure.Segments.Add(ls);//
            pg.Figures.Add(pathFigure); //

            return pg.ToString();
        }

        public string CreateDirectionnalArrow(Point p1, Point p2)
        {
            PathGeometry pg = new PathGeometry();
            RotateTransform transform = new RotateTransform();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            transform.Angle = theta + 90;
            transform.CenterX = p2.X;
            transform.CenterY = p2.Y;

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(p1.X, p1.Y);

            LineSegment ls = new LineSegment(); //
            ls.Point = new Point(p2.X, p2.Y); //
            pathFigure.Segments.Add(ls);//
            pg.Figures.Add(pathFigure); //

            PathGeometry pg2 = new PathGeometry();
            PathFigure pathFigure2 = new PathFigure();
            pathFigure2.IsClosed = true;
            pathFigure2.StartPoint = new Point(p2.X + 6, p2.Y + 11);
            LineSegment ls1 = new LineSegment();
            ls1.Point = new Point(p2.X, p2.Y);
            pathFigure2.Segments.Add(ls1);

            LineSegment ls2 = new LineSegment();
            ls2.Point = new Point(p2.X - 6, p2.Y + 11);
            pathFigure2.Segments.Add(ls2);

            //pg2.Figures.Add(pathFigure2);

            LineSegment ls3 = new LineSegment();
            ls3.Point = new Point(p2.X, p2.Y + 8);
            pathFigure2.Segments.Add(ls3);

            pg2.Figures.Add(pathFigure2);
            pg2.Transform = transform;
            pg.AddGeometry(pg2);

            return pg.ToString();
        }

        public string CreateBidirectionnalArrow(Point p1, Point p2)
        {
            PathGeometry pg = new PathGeometry();
            RotateTransform transform = new RotateTransform();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            transform.Angle = theta + 90;
            transform.CenterX = p2.X;
            transform.CenterY = p2.Y;

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(p1.X, p1.Y);

            LineSegment ls = new LineSegment(); //
            ls.Point = new Point(p2.X, p2.Y); //
            pathFigure.Segments.Add(ls);//
            pg.Figures.Add(pathFigure); //

            PathGeometry pg2 = new PathGeometry();
            PathFigure pathFigure2 = new PathFigure();
            pathFigure2.IsClosed = true;
            pathFigure2.StartPoint = new Point(p2.X + 6, p2.Y + 11);
            LineSegment ls1 = new LineSegment();
            ls1.Point = new Point(p2.X, p2.Y);
            pathFigure2.Segments.Add(ls1);
            LineSegment ls2 = new LineSegment();
            ls2.Point = new Point(p2.X - 6, p2.Y + 11);
            pathFigure2.Segments.Add(ls2);

            LineSegment ls3 = new LineSegment();
            ls3.Point = new Point(p2.X, p2.Y + 8);
            pathFigure2.Segments.Add(ls3);
            pg2.Figures.Add(pathFigure2);
            pg2.Transform = transform;
            pg.AddGeometry(pg2);

            transform.Angle = theta + 90;
            transform.CenterX = p1.X;
            transform.CenterY = p1.Y;

            PathGeometry pg3 = new PathGeometry();
            PathFigure pathFigure3 = new PathFigure();
            pathFigure3.IsClosed = true;
            pathFigure3.StartPoint = new Point(p1.X + 6, p1.Y - 11);
            LineSegment ls4 = new LineSegment();
            ls4.Point = new Point(p1.X, p1.Y);
            pathFigure3.Segments.Add(ls4);
            LineSegment ls5 = new LineSegment();
            ls5.Point = new Point(p1.X - 6, p1.Y - 11);
            pathFigure3.Segments.Add(ls5);

            LineSegment ls6 = new LineSegment();
            ls6.Point = new Point(p1.X, p1.Y - 8);
            pathFigure3.Segments.Add(ls6);
            pg3.Figures.Add(pathFigure3);
            pg3.Transform = transform;

            pg.AddGeometry(pg3);

            return pg.ToString();
        }

        public string CreateReverseDirectionalArrow(Point p1, Point p2)
        {
            PathGeometry pg = new PathGeometry();
            RotateTransform transform = new RotateTransform();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;


            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(p1.X, p1.Y);

            LineSegment ls = new LineSegment(); //
            ls.Point = new Point(p2.X, p2.Y); //
            pathFigure.Segments.Add(ls);//
            pg.Figures.Add(pathFigure); //

            transform.Angle = theta + 90;
            transform.CenterX = p1.X;
            transform.CenterY = p1.Y;

            PathGeometry pg3 = new PathGeometry();
            PathFigure pathFigure3 = new PathFigure();
            pathFigure3.IsClosed = true;
            pathFigure3.StartPoint = new Point(p1.X + 6, p1.Y - 11);
            LineSegment ls4 = new LineSegment();
            ls4.Point = new Point(p1.X, p1.Y);
            pathFigure3.Segments.Add(ls4);
            LineSegment ls5 = new LineSegment();
            ls5.Point = new Point(p1.X - 6, p1.Y - 11);
            pathFigure3.Segments.Add(ls5);

            LineSegment ls6 = new LineSegment();
            ls6.Point = new Point(p1.X, p1.Y - 8);
            pathFigure3.Segments.Add(ls6);
            pg3.Figures.Add(pathFigure3);
            pg3.Transform = transform;

            pg.AddGeometry(pg3);

            return pg.ToString();
        }
    }
}
