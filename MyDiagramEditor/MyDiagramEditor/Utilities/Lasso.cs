using MyDiagramEditor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MyDiagramEditor.Utilities
{
    public class Lasso
    {
        private PathGeometry _zoneSelection;
        private PathFigure _pathFigure;
        private double _minX;
        private double _maxX;
        private double _minY;
        private double _maxY;
        private ItemModel _rectangleSelection;
        public Lasso()
        {
            _zoneSelection = new PathGeometry();
            _pathFigure = new PathFigure();
            _rectangleSelection = new ItemModel();
        }

        public void Start(Point mousePosition, ObservableCollection<ItemModel> items)
        {
            _zoneSelection = new PathGeometry();
            _pathFigure = new PathFigure();
            _pathFigure.StartPoint = new Point(mousePosition.X, mousePosition.Y);
            _pathFigure.IsClosed = true;
            _minX = mousePosition.X;
            _maxX = mousePosition.X;
            _minY = mousePosition.Y;
            _maxY = mousePosition.Y;

            ItemModel item = new ItemModel()
            {
                Type = "lasso",
                Left = 0,
                Top = 0,
                Geometry = ZoneSelection.ToString(),

                Visibility1 = "Hidden",
                StrokeDashArray = "4, 2",
                StrokeThickness = "3"
            };
            items.Add(item);
            RectangleSelection = item;
        }

        public void Make(Point mousePosition)
        {
            // erreur ici occasionnellement avec pathfigure object null?
            LineSegment ls = new LineSegment();
            ls.Point = new Point(mousePosition.X, mousePosition.Y);
            _pathFigure.Segments.Add(ls);
            this._zoneSelection.Figures.Add(_pathFigure);
            RectangleSelection.Geometry = _zoneSelection.ToString();
            if (_minX > mousePosition.X) { _minX = mousePosition.X; }
            if (_maxX < mousePosition.X) { _maxX = mousePosition.X; }
            if (_minY > mousePosition.Y) { _minY = mousePosition.Y; }
            if (_maxY < mousePosition.Y) { _maxY = mousePosition.Y; }
        }

        public void SetRectangleSize(ObservableCollection<ItemModel> selectedItems)
        {
            double minx = MaxX;
            double maxx = MinX;
            double miny = MaxY;
            double maxy = MinY;
            foreach (ItemModel s in selectedItems)
            {
                if (minx > s.Left) { minx = s.Left; }
                if (maxx < s.Left + s.ItemWidth) { maxx = s.Left + s.ItemWidth; }
                if (miny > s.Top) { miny = s.Top; }
                if (maxy < s.Top + s.ItemHeight) { maxy = s.Top + s.ItemHeight; }
            }
            MaxX = maxx;
            MinX = minx;
            MaxY = maxy;
            MinY = miny;

            RectangleSelection.Geometry = "";
            RectangleSelection.SelectionWidth = MaxX - MinX;
            RectangleSelection.SelectionHeight = MaxY - MinY;
            RectangleSelection.Left = MinX;
            RectangleSelection.Top = MinY;
        }

        public double MinX { get => _minX; set => _minX = value; }
        public double MaxX { get => _maxX; set => _maxX = value; }
        public double MinY { get => _minY; set => _minY = value; }
        public double MaxY { get => _maxY; set => _maxY = value; }
        public PathGeometry ZoneSelection { get => _zoneSelection; set => _zoneSelection = value; }
        public ItemModel RectangleSelection { get => _rectangleSelection; set => _rectangleSelection = value; }
    }
}
