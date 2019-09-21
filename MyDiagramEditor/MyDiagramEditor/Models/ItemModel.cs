using Caliburn.Micro;
using MyDiagramEditor.Utilities;
using System;
using System.Windows;
using System.Xml.Serialization;

namespace MyDiagramEditor.Models
{
    public class ItemModel : PropertyChangedBase
    {
        #region constructors
        public ItemModel() { }
        public ItemModel(int id, string creator) { }      // Constructor (must have parameter when sharing) // useless for now
        // Duplication constructor ****(to be updated)
        public ItemModel(ItemModel i)
        {
            _top = i.Top;
            _left = i.Left;
            _type = i.Type;
            _itemWidth = i.ItemWidth;
            _itemHeight = i.ItemHeight;
            _id = i.Id;
            _isConnexion = i.IsConnexion;
            _user = i.User;
            _text1 = i.Text1;
            _topText1 = i.TopText1;
            _leftText1 = i.LeftText1;
            _textWidth1 = i.TextWidth1;
            _textWidthBoth = i.TextWidthBoth;
            _text1Height = i.Text1Height;
            _text2 = i.Text2;
            _topText2 = i.TopText2;
            _leftText2 = i.LeftText2;
            _text2Height = i.Text2Height;
            _text3 = i.Text3;
            _topText3 = i.TopText3;
            _leftText3 = i.LeftText3;
            _text3Height = i.Text3Height;
            _geometry = i.Geometry;
            _fill = i.Fill;
            _stroke = i.Stroke;
            _strokeThickness = i.StrokeThickness;
            _strokeDashArray = i.StrokeDashArray;
            _visibility1 = i.Visibility1;
            _visibility2 = i.Visibility2;
            _visibility3 = i.Visibility3;
            _leftConnectorTop = i.LeftConnectorTop;
            _leftConnectorLeft = i.LeftConnectorLeft;
            _topConnectorLeft = i.TopConnectorLeft;
            _topConnectorTop = i.TopConnectorTop;
            _rightConnectorTop = i.RightConnectorTop;
            _rightConnectorLeft = i.RightConnectorLeft;
            _bottomConnectorTop = i.BottomConnectorTop;
            _bottomConnectorLeft = i.BottomConnectorLeft;
            _fillConnector = i.FillConnector;
            _isConnectorHittable = i.IsConnectorHittable;
            _isLineConnectorHittable = i.IsLineConnectorHittable;
            _selectionWidth = i.SelectionWidth;
            _selectionHeight = i.SelectionHeight;
            _rightConnectorAbsolute = i.RightConnectorAbsolute;
            _rightConnectorRelative = i.RightConnectorRelative;
            _topConnectorAbsolute = i.TopConnectorAbsolute;
            _topConnectorRelative = i.TopConnectorRelative;
            _leftConnectorAbsolute = i.LeftConnectorAbsolute;
            _leftConnectorRelative = i.LeftConnectorRelative;
            _bottomConnectorAbsolute = i.BottomConnectorAbsolute;
            _bottomConnectorRelative = i.BottomConnectorRelative;
            _lineConnectorTop = i.LineConnectorTop;
            _lineConnectorLeft = i.LineConnectorLeft;
            _fillLineConnector = i.FillLineConnector;
            _resizeVisibility = i.ResizeVisibility;
            _itemCenterX = i.ItemCenterX;
            _itemCenterY = i.ItemCenterY;
            _rotationAngle = i.RotationAngle;

            _startItem = null;      // important pour ne pas avoir de copy circulaire
            _endItem = null;        // important pour ne pas avoir de copy circulaire
            _startItemId = i.StartItemId;   // important pour ne pas avoir de copy circulaire
            _endItemId = i.EndItemId;       // important pour ne pas avoir de copy circulaire


            _startOrientation = i.StartOrientation;
            _endOrientation = i.EndOrientation;
            _startPoint = i.StartPoint;
            _endPoint = i.EndPoint;
            _isConnecting = i.IsConnecting;
            _isMatch = i.IsMatch;
            _isClass = i.IsClass;
            _isAngling = i.IsAngling;
            _isSeparated = i.IsSeparated;
            _isAdding = i.IsAdding;
            _isAlterning = i.IsAlterning;
            _isDoneAlterning = i.IsDoneAlterning;
            _typeLink = i.TypeLink;

            _byteImageData = i.ByteImageData;

            _initialItem = null;        // important pour ne pas avoir de copy circulaire
            _finalItem = null;          // important pour ne pas avoir de copy circulaire
            _initialItemId = i.InitialItemId;   // important pour ne pas avoir de copy circulaire
            _finalItemId = i.FinalItemId;   // important pour ne pas avoir de copy circulaire

            _creator = i.Creator;
            _isTextEnabled = i.IsTextEnabled;
            //_isReadOnly = i.IsReadOnly;
            _zIndex = i.ZIndex;
            _movingStrokeColor = i.MovingStrokeColor;
        }
        public ItemModel(int width, int height, Byte[] byteImageData)
        {
            ItemWidth = width;
            ItemHeight = height;
            ItemCenterX = width / 2;
            ItemCenterY = height / 2;
            ByteImageData = byteImageData;
            _visibility1 = "Hidden";
            _top = 10;
            _left = 10;
            _type = "image";
            LeftConnectorLeft = -5;
            LeftConnectorTop = _itemHeight / 2 - 5;
            LeftConnectorRelative = new Point(0, _itemHeight / 2);
            LeftConnectorAbsolute = new Point(_left, _top + _itemHeight / 2);

            TopConnectorLeft = _itemWidth / 2 - 5;
            TopConnectorTop = -5;
            TopConnectorRelative = new Point(_itemWidth / 2, 0);
            TopConnectorAbsolute = new Point(_left + _itemWidth / 2, _top);

            RightConnectorLeft = _itemWidth - 5;
            RightConnectorTop = _itemHeight / 2 - 5;
            RightConnectorRelative = new Point(_itemWidth, _itemHeight / 2);
            RightConnectorAbsolute = new Point(_left + _itemWidth, _top + _itemHeight / 2);

            BottomConnectorLeft = _itemWidth / 2 - 5;
            BottomConnectorTop = _itemHeight - 5;
            BottomConnectorRelative = new Point(_itemWidth / 2, _itemHeight);
            BottomConnectorAbsolute = new Point(_left + _itemWidth / 2, _top + _itemHeight);
        }    // usage uniquement pour image
        private ShapeGenerator _shapeGenerator = new ShapeGenerator();      // to generate default shapes (and class)
        #endregion
        #region attributes and getters
        #region most important values

        // absolute position of the item from the top (y) *** (will set absolutes values aka connectors)
        private double _top;
        public double Top
        {
            get { return _top; }
            set
            {
                if (_top != value)
                {
                    _top = value;
                    NotifyOfPropertyChange(() => Top);
                    SetAbsoluteValues();
                }
            }
        }
        // absolute position of the item from the left (x) *** (will set absolutes values aka connectors)
        private double _left;
        public double Left
        {
            get { return _left; }
            set
            {
                if (_left != value)
                {
                    _left = value;
                    NotifyOfPropertyChange(() => Left);
                    SetAbsoluteValues();
                }
            }
        }
        // the type of the item (will make the shape, set the size)
        private string _type;
        public string Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    NotifyOfPropertyChange(() => Type);
                    SetItemSize();
                    Geometry = _shapeGenerator.Create(Type);  // default values // return null if invalid
                }
            }
        }
        // the width of the item *** (will change depending on the type)
        private int _itemWidth = 0;
        public int ItemWidth
        {
            get { return _itemWidth; }
            set
            {
                if (_itemWidth != value)
                {
                    _itemWidth = value;
                    NotifyOfPropertyChange(() => ItemWidth);
                    SetItemSpecificationByType();
                    SetCenterValues();
                    SetSelectionSize();
                }
            }
        }
        // the height of the item *** (will change depending on the type)
        private int _itemHeight = 0;
        public int ItemHeight
        {
            get { return _itemHeight; }
            set
            {
                if (_itemHeight != value)
                {
                    _itemHeight = value;
                    NotifyOfPropertyChange(() => ItemHeight);
                    SetItemSpecificationByType();
                    SetCenterValues();
                    SetSelectionSize();
                }
            }
        }
        // the id of the item (to select a specific item, or for serialization/ deserialization)
        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyOfPropertyChange(() => Id);
                }
            }
        }
        #endregion
        #region other specifications
        private CanvasModel _canvasModel = null;
        public CanvasModel CanvasModel
        {
            get { return _canvasModel; }
            set
            {
                if (_canvasModel != value)
                {
                    _canvasModel = value;
                    NotifyOfPropertyChange(() => CanvasModel);
                }
            }
        }
        private string _creator;                  // the creator of the item (important for push and pop)
        public string Creator
        {
            get { return _creator; }
            set
            {
                if (_creator != value)
                {
                    _creator = value;
                    NotifyOfPropertyChange(() => Creator);
                }
            }
        }
        private bool _isTextEnabled = true;      // the text needs to be disabled or enables in some cases
        public bool IsTextEnabled
        {
            get { return _isTextEnabled; }
            set
            {
                if (_isTextEnabled != value)
                {
                    _isTextEnabled = value;
                    NotifyOfPropertyChange(() => IsTextEnabled);
                }
            }
        }
        /*
        public bool _isReadOnly = false;
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    NotifyOfPropertyChange(() => IsReadOnly);
                }
            }
        }
        */
        private bool _isConnexion = false;      // the item can be a connexion or a shape (shape/class)
        public bool IsConnexion
        {
            get { return _isConnexion; }
            set
            {
                if (_isConnexion != value)
                {
                    _isConnexion = value;
                    NotifyOfPropertyChange(() => IsConnexion);
                }
            }
        }
        private string _user = "";              // the user that is currently selecting the item
        public string User
        {
            get { return _user; }
            set
            {
                if (_user != value)
                {
                    _user = value;
                    NotifyOfPropertyChange(() => User);
                }
            }
        }
        private string _text1 = "";                  // the value of the first textbox
        public string Text1
        {
            get { return _text1; }
            set
            {
                if (_text1 != value)
                {
                    _text1 = value;
                    NotifyOfPropertyChange(() => Text1);
                }
            }
        }
        private double _topText1;               // relative top value of the text
        public double TopText1
        {
            get { return _topText1; }
            set
            {
                if (_topText1 != value)
                {
                    _topText1 = value;
                    NotifyOfPropertyChange(() => TopText1);
                }
            }
        }
        private double _leftText1;              // relative left value of the text
        public double LeftText1
        {
            get { return _leftText1; }
            set
            {
                if (_leftText1 != value)
                {
                    _leftText1 = value;
                    NotifyOfPropertyChange(() => LeftText1);
                }
            }
        }
        private int _textWidth1;                // width of the text
        public int TextWidth1
        {
            get { return _textWidth1; }
            set
            {
                if (_textWidth1 != value)
                {
                    _textWidth1 = value;
                    NotifyOfPropertyChange(() => TextWidth1);
                }
            }
        }
        private int _textWidthBoth;             // width of the second and third text
        public int TextWidthBoth
        {
            get { return _textWidthBoth; }
            set
            {
                if (_textWidthBoth != value)
                {
                    _textWidthBoth = value;
                    NotifyOfPropertyChange(() => TextWidthBoth);
                }
            }
        }
        private int _text1Height;               // width of all the first text
        public int Text1Height
        {
            get { return _text1Height; }
            set
            {
                if (_text1Height != value)
                {
                    _text1Height = value;
                    NotifyOfPropertyChange(() => Text1Height);
                }
            }
        }

        private string _text2 = "";
        public string Text2
        {
            get { return _text2; }
            set
            {
                if (_text2 != value)
                {
                    _text2 = value;
                    NotifyOfPropertyChange(() => Text2);
                }
            }
        }
        private double _topText2;
        public double TopText2
        {
            get { return _topText2; }
            set
            {
                if (_topText2 != value)
                {
                    _topText2 = value;
                    NotifyOfPropertyChange(() => TopText2);
                }
            }
        }
        private double _leftText2;
        public double LeftText2
        {
            get { return _leftText2; }
            set
            {
                if (_leftText2 != value)
                {
                    _leftText2 = value;
                    NotifyOfPropertyChange(() => LeftText2);
                }
            }
        }
        private int _text2Height;
        public int Text2Height
        {
            get { return _text2Height; }
            set
            {
                if (_text2Height != value)
                {
                    _text2Height = value;
                    NotifyOfPropertyChange(() => Text2Height);
                }
            }
        }
        private string _text3 = "";
        public string Text3
        {
            get { return _text3; }
            set
            {
                if (_text3 != value)
                {
                    _text3 = value;
                    NotifyOfPropertyChange(() => Text3);
                }
            }
        }
        private double _topText3;
        public double TopText3
        {
            get { return _topText3; }
            set
            {
                if (_topText3 != value)
                {
                    _topText3 = value;
                    NotifyOfPropertyChange(() => TopText3);
                }
            }
        }
        private double _leftText3;
        public double LeftText3
        {
            get { return _leftText3; }
            set
            {
                if (_leftText3 != value)
                {
                    _leftText3 = value;
                    NotifyOfPropertyChange(() => LeftText3);
                }
            }
        }
        private int _text3Height;
        public int Text3Height
        {
            get { return _text3Height; }
            set
            {
                if (_text3Height != value)
                {
                    _text3Height = value;
                    NotifyOfPropertyChange(() => Text3Height);
                }
            }
        }

        private string _geometry = "";          // the geometry (shape/class/connexion) of the item
        public string Geometry
        {
            get { return _geometry; }
            set
            {
                if (_geometry != value)
                {
                    _geometry = value;
                    NotifyOfPropertyChange(() => Geometry);
                }
            }
        }
        private string _fill = "Transparent";   // the item color fill
        public string Fill
        {
            get { return _fill; }
            set
            {
                if (_fill != value)
                {
                    _fill = value;
                    NotifyOfPropertyChange(() => Fill);
                }
            }
        }
        private string _stroke = "Black";       // the stroke color (border/ connexion)
        public string Stroke
        {
            get { return _stroke; }
            set
            {
                if (_stroke != value)
                {
                    _stroke = value;
                    NotifyOfPropertyChange(() => Stroke);
                }
            }
        }
        private string _strokeThickness = "1";  // the thickness of the connexion
        public string StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                if (_strokeThickness != value)
                {
                    _strokeThickness = value;
                    NotifyOfPropertyChange(() => StrokeThickness);
                }
            }
        }
        private string _strokeDashArray = "1, 0";       // the style of the border of an item
        public string StrokeDashArray
        {
            get { return _strokeDashArray; }
            set
            {
                if (_strokeDashArray != value)
                {
                    _strokeDashArray = value;
                    NotifyOfPropertyChange(() => StrokeDashArray);
                }
            }
        }
        private string _visibility1 = "Visible";        // show the visibility of the first text
        public string Visibility1
        {
            get { return _visibility1; }
            set
            {
                if (_visibility1 != value)
                {
                    _visibility1 = value;
                    NotifyOfPropertyChange(() => Visibility1);
                }
            }
        }
        private string _visibility2 = "Hidden";         // show the visibility of the second text
        public string Visibility2
        {
            get { return _visibility2; }
            set
            {
                if (_visibility2 != value)
                {
                    _visibility2 = value;
                    NotifyOfPropertyChange(() => Visibility2);
                }
            }
        }
        private string _visibility3 = "Hidden";         // show the visibility of the third text
        public string Visibility3
        {
            get { return _visibility3; }
            set
            {
                if (_visibility3 != value)
                {
                    _visibility3 = value;
                    NotifyOfPropertyChange(() => Visibility3);
                }
            }
        }

        private double _leftConnectorTop;               // relative position of the left connector invisible (top value) 
        public double LeftConnectorTop
        {
            get { return _leftConnectorTop; }
            set
            {
                if (_leftConnectorTop != value)
                {
                    _leftConnectorTop = value;
                    NotifyOfPropertyChange(() => LeftConnectorTop);
                }
            }
        }
        private double _leftConnectorLeft;              // relative position of the left connector invisible (left value)
        public double LeftConnectorLeft
        {
            get { return _leftConnectorLeft; }
            set
            {
                if (_leftConnectorLeft != value)
                {
                    _leftConnectorLeft = value;
                    NotifyOfPropertyChange(() => LeftConnectorLeft);
                }
            }
        }

        private double _topConnectorLeft;
        public double TopConnectorLeft
        {
            get { return _topConnectorLeft; }
            set
            {
                if (_topConnectorLeft != value)
                {
                    _topConnectorLeft = value;
                    NotifyOfPropertyChange(() => TopConnectorLeft);
                }
            }
        }
        private double _topConnectorTop;
        public double TopConnectorTop
        {
            get { return _topConnectorTop; }
            set
            {
                if (_topConnectorTop != value)
                {
                    _topConnectorTop = value;
                    NotifyOfPropertyChange(() => TopConnectorTop);
                }
            }
        }

        private double _rightConnectorLeft;
        public double RightConnectorLeft
        {
            get { return _rightConnectorLeft; }
            set
            {
                if (_rightConnectorLeft != value)
                {
                    _rightConnectorLeft = value;
                    NotifyOfPropertyChange(() => RightConnectorLeft);
                }
            }
        }
        private double _rightConnectorTop;
        public double RightConnectorTop
        {
            get { return _rightConnectorTop; }
            set
            {
                if (_rightConnectorTop != value)
                {
                    _rightConnectorTop = value;
                    NotifyOfPropertyChange(() => RightConnectorTop);
                }
            }
        }

        private double _bottomConnectorLeft;
        public double BottomConnectorLeft
        {
            get { return _bottomConnectorLeft; }
            set
            {
                if (_bottomConnectorLeft != value)
                {
                    _bottomConnectorLeft = value;
                    NotifyOfPropertyChange(() => BottomConnectorLeft);
                }
            }
        }
        private double _bottomConnectorTop;
        public double BottomConnectorTop
        {
            get { return _bottomConnectorTop; }
            set
            {
                if (_bottomConnectorTop != value)
                {
                    _bottomConnectorTop = value;
                    NotifyOfPropertyChange(() => BottomConnectorTop);
                }
            }
        }

        private string _fillConnector = "Transparent";      // the color of the connector (color change when selected/ selected color changes depending on user)
        public string FillConnector
        {
            get { return _fillConnector; }
            set
            {
                if (_fillConnector != value)
                {
                    _fillConnector = value;
                    NotifyOfPropertyChange(() => FillConnector);
                }
            }
        }
        private bool _isConnectorHittable = true;           // disable/enable the hit-test for the connector
        public bool IsConnectorHittable
        {
            get { return _isConnectorHittable; }
            set
            {
                if (_isConnectorHittable != value)
                {
                    _isConnectorHittable = value;
                    NotifyOfPropertyChange(() => IsConnectorHittable);
                }
            }
        }
        private bool _isLineConnectorHittable = false;      // disable/enable the hit-test for the connector inside the connexion
        public bool IsLineConnectorHittable
        {
            get { return _isLineConnectorHittable; }
            set
            {
                if (_isLineConnectorHittable != value)
                {
                    _isLineConnectorHittable = value;
                    NotifyOfPropertyChange(() => IsLineConnectorHittable);
                }
            }
        }

        private bool _isTextHittable = true;           // disable/enable the hit-test for the connector
        public bool IsTextHittable
        {
            get { return _isTextHittable; }
            set
            {
                if (_isTextHittable != value)
                {
                    _isTextHittable = value;
                    NotifyOfPropertyChange(() => IsTextHittable);
                }
            }
        }

        private double _selectionWidth = 0;                 // rectangle selection width (not important)
        public double SelectionWidth
        {
            get { return _selectionWidth; }
            set
            {
                if (_selectionWidth != value)
                {
                    _selectionWidth = value;
                    NotifyOfPropertyChange(() => SelectionWidth);
                    ItemWidth = Convert.ToInt32(_selectionWidth);
                }
            }
        }
        private double _selectionHeight = 0;                // rectangle selection height (not important)
        public double SelectionHeight
        {
            get { return _selectionHeight; }
            set
            {
                if (_selectionHeight != value)
                {
                    _selectionHeight = value;
                    NotifyOfPropertyChange(() => SelectionHeight);
                    ItemHeight = Convert.ToInt32(_selectionHeight);
                }
            }
        }

        private Point _rightConnectorAbsolute = new Point(0, 0);        // right connector absolute value (the colored circle)
        public Point RightConnectorAbsolute
        {
            get { return _rightConnectorAbsolute; }
            set
            {
                if (_rightConnectorAbsolute != value)
                {
                    _rightConnectorAbsolute = value;
                    NotifyOfPropertyChange(() => RightConnectorAbsolute);
                }
            }
        }
        private Point _rightConnectorRelative = new Point(0, 0);        // right connector relative value (the colored circle)
        public Point RightConnectorRelative
        {
            get { return _rightConnectorRelative; }
            set
            {
                if (_rightConnectorRelative != value)
                {
                    _rightConnectorRelative = value;
                    NotifyOfPropertyChange(() => RightConnectorRelative);
                }
            }
        }

        private Point _topConnectorAbsolute = new Point(0, 0);
        public Point TopConnectorAbsolute
        {
            get { return _topConnectorAbsolute; }
            set
            {
                if (_topConnectorAbsolute != value)
                {
                    _topConnectorAbsolute = value;
                    NotifyOfPropertyChange(() => TopConnectorAbsolute);
                }
            }
        }
        private Point _topConnectorRelative = new Point(0, 0);
        public Point TopConnectorRelative
        {
            get { return _topConnectorRelative; }
            set
            {
                if (_topConnectorRelative != value)
                {
                    _topConnectorRelative = value;
                    NotifyOfPropertyChange(() => TopConnectorRelative);
                }
            }
        }

        private Point _leftConnectorAbsolute = new Point(0, 0);
        public Point LeftConnectorAbsolute
        {
            get { return _leftConnectorAbsolute; }
            set
            {
                if (_leftConnectorAbsolute != value)
                {
                    _leftConnectorAbsolute = value;
                    NotifyOfPropertyChange(() => LeftConnectorAbsolute);
                }
            }
        }
        private Point _leftConnectorRelative = new Point(0, 0);
        public Point LeftConnectorRelative
        {
            get { return _leftConnectorRelative; }
            set
            {
                if (_leftConnectorRelative != value)
                {
                    _leftConnectorRelative = value;
                    NotifyOfPropertyChange(() => LeftConnectorRelative);
                }
            }
        }

        private Point _bottomConnectorAbsolute = new Point(0, 0);
        public Point BottomConnectorAbsolute
        {
            get { return _bottomConnectorAbsolute; }
            set
            {
                if (_bottomConnectorAbsolute != value)
                {
                    _bottomConnectorAbsolute = value;
                    NotifyOfPropertyChange(() => BottomConnectorAbsolute);
                }
            }
        }
        private Point _bottomConnectorRelative = new Point(0, 0);
        public Point BottomConnectorRelative
        {
            get { return _bottomConnectorRelative; }
            set
            {
                if (_bottomConnectorRelative != value)
                {
                    _bottomConnectorRelative = value;
                    NotifyOfPropertyChange(() => BottomConnectorRelative);
                }
            }
        }

        private double _lineConnectorTop;       // the angling position (connector) top value (important to divide a connexion)
        public double LineConnectorTop
        {
            get { return _lineConnectorTop; }
            set
            {
                if (_lineConnectorTop != value)
                {
                    _lineConnectorTop = value;
                    NotifyOfPropertyChange(() => LineConnectorTop);
                    // test
                    if (IsConnexion)
                    {
                        TopText1 = LineConnectorTop + 20;
                        NotifyOfPropertyChange(() => TopText1);
                    }
                }
            }
        }
        private double _lineConnectorLeft;      // the angling position (connector) left value (important to divide a connexion)
        public double LineConnectorLeft
        {
            get { return _lineConnectorLeft; }
            set
            {
                if (_lineConnectorLeft != value)
                {
                    _lineConnectorLeft = value;
                    NotifyOfPropertyChange(() => LineConnectorLeft);
                    if (IsConnexion)
                    {
                        LeftText1 = LineConnectorLeft - 50;
                        NotifyOfPropertyChange(() => LeftText1);
                    }
                }
            }
        }

        private string _fillLineConnector = "Transparent";      // the color of the angling connector
        public string FillLineConnector
        {
            get { return _fillLineConnector; }
            set
            {
                if (_fillLineConnector != value)
                {
                    _fillLineConnector = value;
                    NotifyOfPropertyChange(() => FillLineConnector);
                }
            }
        }
        private string _resizeVisibility = "Hidden";            // the resize thumbs visibilty
        public string ResizeVisibility
        {
            get { return _resizeVisibility; }
            set
            {
                _resizeVisibility = value;
                NotifyOfPropertyChange(() => ResizeVisibility);
            }
        }
        private int _itemCenterX = 0;                   // the center of masse of an item (x left)
        public int ItemCenterX
        {
            get { return _itemCenterX; }
            set
            {
                _itemCenterX = value;
                NotifyOfPropertyChange(() => ItemCenterX);
            }
        }
        private int _itemCenterY = 0;                   // the center of mass of an item (y top)
        public int ItemCenterY
        {
            get { return _itemCenterY; }
            set
            {
                _itemCenterY = value;
                NotifyOfPropertyChange(() => ItemCenterY);
            }
        }

        private double _rotationAngle = 0;              // the rotation angle of the item (0 to 360)
        public double RotationAngle
        {
            get { return _rotationAngle; }
            set
            {
                _rotationAngle = value;
                NotifyOfPropertyChange(() => RotationAngle);
                SetAbsoluteValues();
            }
        }

        private byte[] _byteImageData;
        public byte[] ByteImageData
        {
            get { return _byteImageData; }
            set
            {
                _byteImageData = value;
                NotifyOfPropertyChange(() => ByteImageData);
            }
        }

        private int _zIndex = 2;
        public int ZIndex
        {
            get { return _zIndex; }
            set
            {
                _zIndex = value;
                NotifyOfPropertyChange(() => ZIndex);
            }
        }

        private string _movingStrokeColor = "Transparent";
        public string MovingStrokeColor
        {
            get { return _movingStrokeColor; }
            set
            {
                _movingStrokeColor = value;
                NotifyOfPropertyChange(() => MovingStrokeColor);
            }
        }
        #endregion
        #endregion
        #region setting
        public void SetItemSize()
        {
            if (Type == "artefact")
            {
                ItemWidth = 80;
                ItemHeight = 100 + 20;
                Text1 = "Work Product";
            }
            else if (Type == "activite")
            {
                ItemWidth = 100;    // ajout
                ItemHeight = 60 + 20;
                Text1 = "Activity";
            }
            else if (Type == "classe")
            {
                ItemWidth = 150;    // ajout
                ItemHeight = 90;
                Text1 = "Class X";
                Text2 = "+ Attribut: type";
                Text3 = "+ Method(type): type";
            }
            else if (Type == "commentaire")
            {
                ItemWidth = 200;    // ajout
                ItemHeight = 100;
                Text1 = "Annotation";
            }
            else if (Type == "phase")
            {
                ItemWidth = 400;    // ajout
                ItemHeight = 200;
                Text1 = "Phase X";
                ZIndex = 0;
            }
            else if (Type == "role")
            {
                ItemWidth = 40;    // ajout
                ItemHeight = 60 + 20;
                Text1 = "Role";
            }
            else if (Type == "texte")
            {
                ItemWidth = 100;    // ajout
                ItemHeight = 20;
                Text1 = "Text";
            }
        }                // set item size by type
        public void SetItemSpecificationByType()    // set specification by type (text width height, connector pos ...)
        {
            if (Type == "artefact")
            {
                Geometry = this._shapeGenerator.CreateArtefact(_itemWidth, 5 * _itemHeight / 6);

                TopText1 = 5 * _itemHeight / 6;
                LeftText1 = _itemWidth * 0;
                TextWidth1 = _itemWidth;
                TextWidthBoth = _itemWidth;
                Text1Height = _itemHeight / 6;

                LeftConnectorLeft = -5;
                LeftConnectorTop = (5 * _itemHeight / 6) / 2 - 5;
                LeftConnectorRelative = new Point(0, (5 * _itemHeight / 6) / 2);
                LeftConnectorAbsolute = new Point(_left, _top + (5 * _itemHeight / 6) / 2);

                TopConnectorLeft = _itemWidth / 2 - 5;
                TopConnectorTop = -5;
                TopConnectorRelative = new Point(_itemWidth / 2, 0);
                TopConnectorAbsolute = new Point(_left + _itemWidth / 2, _top);

                RightConnectorLeft = _itemWidth - 5;
                RightConnectorTop = (5 * _itemHeight / 6) / 2 - 5;
                RightConnectorRelative = new Point(_itemWidth, (5 * _itemHeight / 6) / 2);
                RightConnectorAbsolute = new Point(_left + _itemWidth, _top + (5 * _itemHeight / 6) / 2);

                BottomConnectorLeft = _itemWidth / 2 - 5;
                BottomConnectorTop = (5 * _itemHeight / 6) - 5;
                BottomConnectorRelative = new Point(_itemWidth / 2, (5 * _itemHeight / 6));
                BottomConnectorAbsolute = new Point(_left + _itemWidth / 2, _top + (5 * _itemHeight / 6));

            }
            else if (Type == "activite")
            {
                Geometry = _shapeGenerator.CreateActivite(_itemWidth, 3 * _itemHeight / 4);

                TopText1 = (3 * _itemHeight / 4);
                LeftText1 = 0;
                TextWidth1 = _itemWidth;
                TextWidthBoth = _itemWidth;
                Text1Height = _itemHeight / 3;

                LeftConnectorLeft = _itemWidth / 5 - 5;
                LeftConnectorTop = (3 * _itemHeight / 4) / 2 - 5;
                LeftConnectorRelative = new Point(_itemWidth / 5, (3 * _itemHeight / 4) / 2);
                LeftConnectorAbsolute = new Point(_left + _itemWidth / 5, _top + (3 * _itemHeight / 4) / 2);

                TopConnectorLeft = _itemWidth / 2 - 5;
                TopConnectorTop = -5;
                TopConnectorRelative = new Point(_itemWidth / 2, 0);
                TopConnectorAbsolute = new Point(_left + _itemWidth / 2, _top);

                RightConnectorLeft = _itemWidth - 5;
                RightConnectorTop = (3 * _itemHeight / 4) / 2 - 5;
                RightConnectorRelative = new Point(_itemWidth, (3 * _itemHeight / 4) / 2);
                RightConnectorAbsolute = new Point(_left + _itemWidth, _top + (3 * _itemHeight / 4) / 2);

                BottomConnectorLeft = _itemWidth / 2 - 5;
                BottomConnectorTop = (3 * _itemHeight / 4) - 5;
                BottomConnectorRelative = new Point(_itemWidth / 2, (3 * _itemHeight / 4));
                BottomConnectorAbsolute = new Point(_left + _itemWidth / 2, _top + (3 * _itemHeight / 4));

            }
            else if (Type == "classe")
            {
                Geometry = _shapeGenerator.CreateClasse(_itemWidth, _itemHeight);

                TopText1 = 0;
                LeftText1 = 0;
                TextWidth1 = _itemWidth;
                TextWidthBoth = _itemWidth;
                Text1Height = (_itemHeight / 3);


                TopText2 = (_itemHeight / 3);
                LeftText2 = 0;
                Text2Height = (_itemHeight / 3);
                Visibility2 = "Visible";


                TopText3 = (2 * _itemHeight / 3);
                LeftText3 = 0;
                Text3Height = (_itemHeight / 3);
                Visibility3 = "Visible";


                LeftConnectorLeft = -5;
                LeftConnectorTop = _itemHeight / 2 - 5;
                LeftConnectorRelative = new Point(0, _itemHeight / 2);
                LeftConnectorAbsolute = new Point(_left, _top + _itemHeight / 2);

                TopConnectorLeft = _itemWidth / 2 - 5;
                TopConnectorTop = -5;
                TopConnectorRelative = new Point(_itemWidth / 2, 0);
                TopConnectorAbsolute = new Point(_left + _itemWidth / 2, _top);

                RightConnectorLeft = _itemWidth - 5;
                RightConnectorTop = _itemHeight / 2 - 5;
                RightConnectorRelative = new Point(_itemWidth, _itemHeight / 2);
                RightConnectorAbsolute = new Point(_left + _itemWidth, _top + _itemHeight / 2);

                BottomConnectorLeft = _itemWidth / 2 - 5;
                BottomConnectorTop = _itemHeight - 5;
                BottomConnectorRelative = new Point(_itemWidth / 2, _itemHeight);
                BottomConnectorAbsolute = new Point(_left + _itemWidth / 2, _top + _itemHeight);

            }
            else if (Type == "commentaire")
            {
                Geometry = _shapeGenerator.CreateCommentaire(_itemWidth, _itemHeight);

                TopText1 = 0;
                LeftText1 = 0;
                TextWidth1 = _itemWidth;
                TextWidthBoth = _itemWidth;
                Text1Height = _itemHeight;

                LeftConnectorLeft = -5;
                LeftConnectorTop = _itemHeight / 2 - 5;
                LeftConnectorRelative = new Point(0, _itemHeight / 2);
                LeftConnectorAbsolute = new Point(_left, _top + _itemHeight / 2);

                TopConnectorLeft = _itemWidth / 2 - 5;
                TopConnectorTop = -5;
                TopConnectorRelative = new Point(_itemWidth / 2, 0);
                TopConnectorAbsolute = new Point(_left + _itemWidth / 2, _top);

                RightConnectorLeft = _itemWidth - 5;
                RightConnectorTop = _itemHeight / 2 - 5;
                RightConnectorRelative = new Point(_itemWidth, _itemHeight / 2);
                RightConnectorAbsolute = new Point(_left + _itemWidth, _top + _itemHeight / 2);

                BottomConnectorLeft = _itemWidth / 2 - 5;
                BottomConnectorTop = _itemHeight - 5;
                BottomConnectorRelative = new Point(_itemWidth / 2, _itemHeight);
                BottomConnectorAbsolute = new Point(_left + _itemWidth / 2, _top + _itemHeight);

            }
            else if (Type == "phase")
            {
                Geometry = _shapeGenerator.CreatePhase(_itemWidth, _itemHeight);

                TopText1 = 0;
                LeftText1 = 0;
                TextWidth1 = _itemWidth;
                TextWidthBoth = _itemWidth;
                Text1Height = _itemHeight / 10;

                LeftConnectorLeft = -5;
                LeftConnectorTop = _itemHeight / 2 - 5;
                LeftConnectorRelative = new Point(0, _itemHeight / 2);
                LeftConnectorAbsolute = new Point(_left, _top + _itemHeight / 2);

                TopConnectorLeft = _itemWidth / 2 - 5;
                TopConnectorTop = -5;
                TopConnectorRelative = new Point(_itemWidth / 2, 0);
                TopConnectorAbsolute = new Point(_left + _itemWidth / 2, _top);

                RightConnectorLeft = _itemWidth - 5;
                RightConnectorTop = _itemHeight / 2 - 5;
                RightConnectorRelative = new Point(_itemWidth, _itemHeight / 2);
                RightConnectorAbsolute = new Point(_left + _itemWidth, _top + _itemHeight / 2);

                BottomConnectorLeft = _itemWidth / 2 - 5;
                BottomConnectorTop = _itemHeight - 5;
                BottomConnectorRelative = new Point(_itemWidth / 2, _itemHeight);
                BottomConnectorAbsolute = new Point(_left + _itemWidth / 2, _top + _itemHeight);

            }
            else if (Type == "role")
            {
                Geometry = _shapeGenerator.CreateRole(_itemWidth, 3 * _itemHeight / 4);

                TopText1 = (3 * _itemHeight / 4);
                LeftText1 = 0;
                TextWidth1 = _itemWidth;
                TextWidthBoth = _itemWidth;
                Text1Height = (_itemHeight / 4);

                LeftConnectorLeft = -5;
                LeftConnectorTop = (3 * _itemHeight / 4) / 2 - 5;
                LeftConnectorRelative = new Point(0, (3 * _itemHeight / 4) / 2);
                LeftConnectorAbsolute = new Point(_left, _top + (3 * _itemHeight / 4) / 2);

                TopConnectorLeft = _itemWidth / 2 - 5;
                TopConnectorTop = -5;
                TopConnectorRelative = new Point(_itemWidth / 2, 0);
                TopConnectorAbsolute = new Point(_left + _itemWidth / 2, _top);

                RightConnectorLeft = _itemWidth - 5;
                RightConnectorTop = (3 * _itemHeight / 4) / 2 - 5;
                RightConnectorRelative = new Point(_itemWidth, (3 * _itemHeight / 4) / 2);
                RightConnectorAbsolute = new Point(_left + _itemWidth, _top + (3 * _itemHeight / 4) / 2);

                BottomConnectorLeft = _itemWidth / 2 - 5;
                BottomConnectorTop = (3 * _itemHeight / 4) - 5;
                BottomConnectorRelative = new Point(_itemWidth / 2, (3 * _itemHeight / 4));
                BottomConnectorAbsolute = new Point(_left + _itemWidth / 2, _top + (3 * _itemHeight / 4));

            }
            else if (Type == "texte")
            {

                TopText1 = 0;
                LeftText1 = 0;
                TextWidth1 = _itemWidth;
                TextWidthBoth = _itemWidth;
                Text1Height = _itemHeight;

                LeftConnectorLeft = -5;
                LeftConnectorTop = _itemHeight / 2 - 5;
                LeftConnectorRelative = new Point(0, _itemHeight / 2);
                LeftConnectorAbsolute = new Point(_left, _top + _itemHeight / 2);

                TopConnectorLeft = _itemWidth / 2 - 5;
                TopConnectorTop = -5;
                TopConnectorRelative = new Point(_itemWidth / 2, 0);
                TopConnectorAbsolute = new Point(_left + _itemWidth / 2, _top);

                RightConnectorLeft = _itemWidth - 5;
                RightConnectorTop = _itemHeight / 2 - 5;
                RightConnectorRelative = new Point(_itemWidth, _itemHeight / 2);
                RightConnectorAbsolute = new Point(_left + _itemWidth, _top + _itemHeight / 2);

                BottomConnectorLeft = _itemWidth / 2 - 5;
                BottomConnectorTop = _itemHeight - 5;
                BottomConnectorRelative = new Point(_itemWidth / 2, _itemHeight);
                BottomConnectorAbsolute = new Point(_left + _itemWidth / 2, _top + _itemHeight);

            }
            else if (Type == "lasso")
            {
                LeftConnectorLeft = -5;
                LeftConnectorTop = _itemHeight / 2 - 5;
                LeftConnectorRelative = new Point(0, _itemHeight / 2);
                LeftConnectorAbsolute = new Point(_left, _top + _itemHeight / 2);

                TopConnectorLeft = _itemWidth / 2 - 5;
                TopConnectorTop = -5;
                TopConnectorRelative = new Point(_itemWidth / 2, 0);
                TopConnectorAbsolute = new Point(_left + _itemWidth / 2, _top);

                RightConnectorLeft = _itemWidth - 5;
                RightConnectorTop = _itemHeight / 2 - 5;
                RightConnectorRelative = new Point(_itemWidth, _itemHeight / 2);
                RightConnectorAbsolute = new Point(_left + _itemWidth, _top + _itemHeight / 2);

                BottomConnectorLeft = _itemWidth / 2 - 5;
                BottomConnectorTop = _itemHeight - 5;
                BottomConnectorRelative = new Point(_itemWidth / 2, _itemHeight);
                BottomConnectorAbsolute = new Point(_left + _itemWidth / 2, _top + _itemHeight);
            }
            else if (Type == "image")
            {
                LeftConnectorLeft = -5;
                LeftConnectorTop = _itemHeight / 2 - 5;
                LeftConnectorRelative = new Point(0, _itemHeight / 2);
                LeftConnectorAbsolute = new Point(_left, _top + _itemHeight / 2);

                TopConnectorLeft = _itemWidth / 2 - 5;
                TopConnectorTop = -5;
                TopConnectorRelative = new Point(_itemWidth / 2, 0);
                TopConnectorAbsolute = new Point(_left + _itemWidth / 2, _top);

                RightConnectorLeft = _itemWidth - 5;
                RightConnectorTop = _itemHeight / 2 - 5;
                RightConnectorRelative = new Point(_itemWidth, _itemHeight / 2);
                RightConnectorAbsolute = new Point(_left + _itemWidth, _top + _itemHeight / 2);

                BottomConnectorLeft = _itemWidth / 2 - 5;
                BottomConnectorTop = _itemHeight - 5;
                BottomConnectorRelative = new Point(_itemWidth / 2, _itemHeight);
                BottomConnectorAbsolute = new Point(_left + _itemWidth / 2, _top + _itemHeight);
            }
        }
        public void SetCenterValues()
        {
            ItemCenterX = _itemWidth / 2;
            ItemCenterY = _itemHeight / 2;
        }            // set the center of mass for an item
        public void SetAbsoluteValues()             // set the connector absolute value (when moving an item/ rotating)
        {

            double x = ItemCenterX;
            double y = ItemCenterY;
            double act = 0;
            double textH = 0;
            if (Type == "artefact" || Type == "role" || Type == "activite")
            {
                textH = Text1Height;
                if (Type == "activite")
                {
                    act = LeftConnectorRelative.X;
                }
            }

            double rcax = _left + x + (RightConnectorRelative.X - x) * Math.Cos((Math.PI / 180) * RotationAngle);
            double rcay = _top + y + (RightConnectorRelative.Y - y) * Math.Cos((Math.PI / 180) * RotationAngle);

            double lcax = _left + x + (LeftConnectorRelative.X - x) * Math.Cos((Math.PI / 180) * RotationAngle);
            double lcay = _top + y + (LeftConnectorRelative.Y - y) * Math.Cos((Math.PI / 180) * RotationAngle);

            double tcax = _left + x + (TopConnectorRelative.X - x) * Math.Cos((Math.PI / 180) * RotationAngle);
            double tcay = _top + y + (TopConnectorRelative.Y - y) * Math.Cos((Math.PI / 180) * RotationAngle);

            double bcax = _left + x + (BottomConnectorRelative.X - x) * Math.Cos((Math.PI / 180) * RotationAngle);
            double bcay = _top + y + (BottomConnectorRelative.Y - y) * Math.Cos((Math.PI / 180) * RotationAngle);

            if (RotationAngle == 90 || RotationAngle == 270)
            {
                rcax = _left + x + (textH / 2) * Math.Sin((Math.PI / 180) * RotationAngle);
                rcay = _top + y + (x) * Math.Sin((Math.PI / 180) * RotationAngle);

                lcax = _left + x + (textH / 2) * Math.Sin((Math.PI / 180) * RotationAngle);
                lcay = _top + y + (-x + act) * Math.Sin((Math.PI / 180) * RotationAngle);

                tcax = _left + x + (y) * Math.Sin((Math.PI / 180) * RotationAngle);
                tcay = _top + y;

                bcax = _left + x + (-y + textH) * Math.Sin((Math.PI / 180) * RotationAngle);
                bcay = _top + y;
            }


            RightConnectorAbsolute = new Point(rcax, rcay);
            LeftConnectorAbsolute = new Point(lcax, lcay);
            TopConnectorAbsolute = new Point(tcax, tcay);
            BottomConnectorAbsolute = new Point(bcax, bcay);
        }
        public void SetSelectionSize()
        {
            if (Type == "lasso")
            {
                SelectionWidth = ItemWidth;
                SelectionHeight = ItemHeight;
            }
        }
        #endregion
        #region connexion
        ///
        /// Connexion specifications
        ///
        #region attributes for a connexion
        private ConnexionGenerator _connexionGenerator = new ConnexionGenerator();  // generate connexions shapes (not important)

        private ItemModel _startItem;       // the first item following the orientation
                                            // the second item is this item (this)
        private ItemModel _endItem;         // the third item following the orientation
        private ItemModel _initialItem;     // the initial item (the item (shape/class) that the connexion(s) started from
        private ItemModel _finalItem;       // the final item (the item (shape/class) that the connexion(s) started from


        private int _startItemId = -1;      // the id of the first item (important for serialization/deserialization)
        private int _endItemId = -1;        // the id of the third item (important for serialization/deserialization)
        private int _initialItemId = -1;    // the id of initial item (important for serialization/deserialization)
        private int _finalItemId = -1;      // the id of the final item (important for serialization/deserialization)

        private string _startOrientation;   // the orientation of the initial item (four connectors/ four sides)
        private string _endOrientation;     // the orientation of the final item (four connectors/ four sides)
        private Point _startPoint;          // the starting point of a connexion    (absolute value)
        private Point _endPoint;            // the ending point of a connexion      (absolute value)

        // booleans for connecting angling etc (not important)
        private bool _isConnecting;
        private bool _isMatch;
        private bool _isAngling;
        private bool _isAdding = false;
        private bool _isAlterning = false;
        private bool _isDoneAlterning = false;

        private bool _isSeparated = false;      // will tell if a connexion is separated or not
        private bool _isClass;                  // will tell if a connexion is of type class or not
        private string _typeLink;               // the exact type of the link (mostyl used for shapes)

        #endregion

        // getters and setters for the connexions
        #region getters/setters
        public ItemModel StartItem { get => _startItem; set => _startItem = value; }
        public ItemModel EndItem { get => _endItem; set => _endItem = value; }
        public bool IsConnecting { get => _isConnecting; set => _isConnecting = value; }
        public Point StartPoint { get => _startPoint; set => _startPoint = value; }
        public Point EndPoint { get => _endPoint; set => _endPoint = value; }
        public string StartOrientation { get => _startOrientation; set => _startOrientation = value; }
        public string EndOrientation { get => _endOrientation; set => _endOrientation = value; }
        public bool IsMatch { get => _isMatch; set => _isMatch = value; }
        public bool IsClass { get => _isClass; set => _isClass = value; }
        public bool IsAngling { get => _isAngling; set => _isAngling = value; }
        public string TypeLink { get => _typeLink; set => _typeLink = value; }
        public bool IsSeparated { get => _isSeparated; set => _isSeparated = value; }
        public bool IsAdding { get => _isAdding; set => _isAdding = value; }
        public bool IsAlterning { get => _isAlterning; set => _isAlterning = value; }
        public ItemModel InitialItem { get => _initialItem; set => _initialItem = value; }
        public ItemModel FinalItem { get => _finalItem; set => _finalItem = value; }
        public bool IsDoneAlterning { get => _isDoneAlterning; set => _isDoneAlterning = value; }
        public int StartItemId { get => _startItemId; set => _startItemId = value; }
        public int EndItemId { get => _endItemId; set => _endItemId = value; }
        public int InitialItemId { get => _initialItemId; set => _initialItemId = value; }
        public int FinalItemId { get => _finalItemId; set => _finalItemId = value; }
        #endregion
        #region setting for a connexion
        // update the start point and endpoint (when moving an item, angling a connexion, rotation, etc.)
        public void Update()
        {
            if (_startItem == InitialItem)
            {
                if (_startOrientation == "left")
                {
                    _startPoint = _startItem.LeftConnectorAbsolute;
                }
                if (_startOrientation == "top")
                {
                    _startPoint = _startItem.TopConnectorAbsolute;
                }
                if (_startOrientation == "right")
                {
                    _startPoint = _startItem.RightConnectorAbsolute;
                }
                if (_startOrientation == "bottom")
                {
                    _startPoint = _startItem.BottomConnectorAbsolute;
                }
            }

            if (_endItem == FinalItem)
            {
                if (_endOrientation == "left")
                {
                    _endPoint = _finalItem.LeftConnectorAbsolute;
                }
                if (_endOrientation == "top")
                {
                    _endPoint = _finalItem.TopConnectorAbsolute;
                }
                if (_endOrientation == "right")
                {
                    _endPoint = _finalItem.RightConnectorAbsolute;
                }
                if (_endOrientation == "bottom")
                {
                    _endPoint = _finalItem.BottomConnectorAbsolute;
                }
            }

            if ((_endItem == FinalItem) && (_startItem == InitialItem))
            {
                Geometry = _connexionGenerator.Create(Type, _startPoint, _endPoint);
                LineConnectorLeft = ((_endPoint.X - _startPoint.X) / 2) + _startPoint.X - 5;
                LineConnectorTop = ((_endPoint.Y - _startPoint.Y) / 2) + _startPoint.Y - 5;
                return;
            }

            if ((_endItem == FinalItem))
            {
                Geometry = _connexionGenerator.Create(SetFinalItemType(), _startPoint, _endPoint);
                LineConnectorLeft = ((_endPoint.X - _startPoint.X) / 2) + _startPoint.X - 5;
                LineConnectorTop = ((_endPoint.Y - _startPoint.Y) / 2) + _startPoint.Y - 5;
            }

            if ((_startItem == InitialItem))
            {
                Geometry = _connexionGenerator.Create(SetInitialItemType(), _startPoint, _endPoint);
                LineConnectorLeft = ((_endPoint.X - _startPoint.X) / 2) + _startPoint.X - 5;
                LineConnectorTop = ((_endPoint.Y - _startPoint.Y) / 2) + _startPoint.Y - 5;
            }

        }

        // set the shape for the connexion that is linked to the final item
        public string SetFinalItemType()
        {
            if (EndItem == FinalItem)
            {
                if (Type == "unidirectionnalLink") { return "unidirectionnalLink"; }
                if (Type == "inheritanceLink") { return "inheritanceLink"; }
                if (Type == "bidirectionnalLink") { return "bidirectionnalLink"; }
                if (Type == "aggregationLink") { return "bidirectionnalLink"; }
                if (Type == "lineConnexion") { return "bidirectionnalLink"; }
                if (Type == "directionnalArrow") { return "directionnalArrow"; }
                if (Type == "compositionLink") { return "bidirectionnalLink"; }
                if (Type == "bidirectionnalArrow") { return "directionnalArrow"; }
            }
            return "bidirectionnalLink";
        }

        // set the shape for the connexion that is linked to the first item
        public string SetInitialItemType()
        {
            if (StartItem == InitialItem)
            {
                if (Type == "unidirectionnalLink") { return "bidirectionnalLink"; }
                if (Type == "inheritanceLink") { return "bidirectionnalLink"; }
                if (Type == "bidirectionnalLink") { return "bidirectionnalLink"; }
                if (Type == "aggregationLink") { return "aggregationLink"; }
                if (Type == "lineConnexion") { return "bidirectionnalLink"; }
                if (Type == "directionnalArrow") { return "bidirectionnalLink"; }
                if (Type == "compositionLink") { return "compositionLink"; }
                if (Type == "bidirectionnalArrow") { return "reverseDirectionnalArrow"; }
            }
            return "bidirectionnalLink";
        }

        // set the side texts of a class (when moving an item, angling a connexion, rotation, etc.)
        public void SetSideText()
        {
            if (_startOrientation == "left")
            {
                //_startPoint = _startItem.LeftConnectorAbsolute;
                LeftText2 = _initialItem.LeftConnectorAbsolute.X - 20;
                TopText2 = _initialItem.LeftConnectorAbsolute.Y - 20;
            }
            if (_startOrientation == "top")
            {
                LeftText2 = _initialItem.TopConnectorAbsolute.X - 20;
                TopText2 = _initialItem.TopConnectorAbsolute.Y - 20;
            }
            if (_startOrientation == "right")
            {
                LeftText2 = _initialItem.RightConnectorAbsolute.X;
                TopText2 = _initialItem.RightConnectorAbsolute.Y - 20;
            }
            if (_startOrientation == "bottom")
            {
                LeftText2 = _initialItem.BottomConnectorAbsolute.X - 20;
                TopText2 = _initialItem.BottomConnectorAbsolute.Y;
            }

            if (_endOrientation == "left")
            {
                LeftText3 = _finalItem.LeftConnectorAbsolute.X - 20;
                TopText3 = _finalItem.LeftConnectorAbsolute.Y - 20;
            }
            if (_endOrientation == "top")
            {
                LeftText3 = _finalItem.TopConnectorAbsolute.X;
                TopText3 = _finalItem.TopConnectorAbsolute.Y - 20;
            }
            if (_endOrientation == "right")
            {
                LeftText3 = _finalItem.RightConnectorAbsolute.X;
                TopText3 = _finalItem.RightConnectorAbsolute.Y - 20;
            }
            if (_endOrientation == "bottom")
            {
                LeftText3 = _finalItem.BottomConnectorAbsolute.X;
                TopText3 = _finalItem.BottomConnectorAbsolute.Y;
            }
        }
        #endregion
        #endregion

        public class TextMessage
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public string Type { get; set; }
            public string TextType { get; set; }

            public TextMessage(int id, string text, string type, string textType)
            {
                Id = id;
                Text = text;
                Type = type;
                TextType = textType;
            }
        }
    }
}
