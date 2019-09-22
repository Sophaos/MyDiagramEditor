using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using Caliburn.Micro;
using Microsoft.Win32;
using MyDiagramEditor.Models;
using MyDiagramEditor.Utilities;
//using static PolyPaint.Models.ItemModel;
namespace MyDiagramEditor.ViewModels
{
    public class EditorViewModel : PropertyChangedBase,
        //IHandle<TextMessage>, IHandle<TutorialMessage>, 
        IHandle<CanvasCreationMessage>,
        IHandle<CanvasLoadingMessage>//, IHandle<TextAlternationMessage>
    {
        #region attributs
        private ObservableCollection<ItemModel> _items = new ObservableCollection<ItemModel>();         // items to be shown in the canvas
        private ObservableCollection<ItemModel> _stack = new ObservableCollection<ItemModel>();         // personal stack
        private ObservableCollection<ItemModel> _selectedItems = new ObservableCollection<ItemModel>(); // selected items
        private ObservableCollection<ItemModel> _copiedItems = new ObservableCollection<ItemModel>();   // copied items
        private ObservableCollection<ItemModel> _createdItems = new ObservableCollection<ItemModel>();

        private EditorModel _editor = new EditorModel();    // unique specification for the user
        private bool _isDoneAddingCanvas = false;
        private ConnexionGenerator _connexionGenerator = new ConnexionGenerator();  // will generate the shape of a connexion
        private string _textBlockPosition = "";
        private Point _mousePosition;

        // canvas related informations

        private CanvasModel _canvas = new CanvasModel();
        private int _midCanvasX = 0;
        private int _midCanvasY = 0;

        private Lasso _lasso = new Lasso();
        private IWindowManager _windowManager;
        // attributes to create a connexion
        private ItemModel _clParent = new ItemModel();
        private ItemModel _clStart = null;
        private ItemModel _clEnd = null;
        string StartOrientation = "";
        string EndOrientation = "";
        private int _id = 0;     // temporaire, sera remplacer par lui du serveur
        #endregion
        #region server-related-stuff
        List<Pair<string, string>> UserColors = new List<Pair<string, string>>();
        int itemCounter = 0;
        private string _user;

        public string User
        {
            get { return _user; }
            set { _user = value; }
        }


        public EditorViewModel(IEventAggregator eventAggregator, IWindowManager windowManager)
        {


            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

        }

        public ItemModel GetItemFromItemsWithIdAndType(int id, string type)
        {
            foreach (ItemModel s in Items)
            {
                if (s.Id == id && s.Type == type)
                {
                    return s;
                }
            }
            return null;
        }

        public ItemModel GetItemFromItemsWithId(int id)
        {
            foreach (ItemModel s in Items)
            {
                if (s.Id == id)
                {
                    return s;
                }
            }
            return null;
        }

        public ItemModel GetItemFromSelectedWithId(int id)
        {
            foreach (ItemModel s in SelectedItems)
            {
                if (s.Id == id)
                {
                    return s;
                }
            }
            return null;
        }

        public string getSelectColor(string username)
        {
            foreach (Pair<string, string> pair in UserColors)
            {
                if (pair.First == username)
                {
                    return pair.Second;
                }
            }
            return "Transparent";
        }

        public void Filling(int id, string color)
        {
            if (GetItemFromItemsWithId(id) != null)
                GetItemFromItemsWithId(id).Fill = color;
        }

        public void SetColorBorder(int id, string color)
        {
            if (GetItemFromItemsWithId(id) != null)
                GetItemFromItemsWithId(id).Stroke = color;
        }

        public void SetBorderStyle(int id, string style)
        {
            if (GetItemFromItemsWithId(id) != null)
                GetItemFromItemsWithId(id).StrokeDashArray = style;
        }

        public void SetBorderThickness(int id, string thickness)
        {
            if (GetItemFromItemsWithId(id) != null)
                GetItemFromItemsWithId(id).StrokeThickness = thickness;
        }


        public void Cutting(int id)
        {
            if (GetItemFromItemsWithId(id) != null)
            {
                ItemModel item = GetItemFromItemsWithId(id);
                Items.Remove(item);

                if (_createdItems.Contains(item))
                {
                    _createdItems.Remove(item);
                }

            }
        }

        #endregion

        public bool IsForm()
        {
            return (
                SelectedTool == "artefact" ||
                SelectedTool == "activite" ||
                SelectedTool == "phase" ||
                SelectedTool == "commentaire" ||
                SelectedTool == "role" ||
                SelectedTool == "classe");
        }
        #region Control points
        // Any user can control the control points. (default min values are = 50)
        // note petit bug semble: depasser la barre en bas -> effet indesiree
        public void ChangeControlPoints(int horizontalChange, int verticalChange, string orientation)
        {
            if (!IsDoneAddingCanvas) return;
            if (orientation == "horizontal" || orientation == "diagonal")
            {
                if (CanvasWidth + horizontalChange < 50)
                {
                    CanvasWidth = 50;
                    return;
                }
                CanvasWidth += horizontalChange;
            }
            if (orientation == "vertical" || orientation == "diagonal")
            {
                if (CanvasHeight + verticalChange < 50)
                {
                    CanvasHeight = 50;
                    return;
                }
                CanvasHeight += verticalChange;
            }
        }
        #endregion

        public void LeftMouseButtonUp()
        {
            _isMouseDown = false;
        }

        public void MouseMoveCanvas(Point p)
        {
            MousePosition = p;
            TextBlockPosition = Math.Round(p.X) + ", " + Math.Round(p.Y) + "px";
        }
        public void MouseLeaveCanvas(Point p)
        {
            MousePosition = p;
            TextBlockPosition = "";
        }
        // Remove the current selection (to be verified for connexions)
        public void Cut()
        {

            if (SelectedItems.Count != 0)
            {
                _copiedItems.Clear(); // test
                foreach (ItemModel s in _selectedItems)
                {
                    if (!s.IsConnexion)
                    {
                        ItemModel CopiedItem = new ItemModel(s);
                        CopiedItems.Add(CopiedItem);
                    }
                }
                ObservableCollection<ItemModel> connexionsToRemove = new ObservableCollection<ItemModel>();
                // we are looking at every selected items

                foreach (ItemModel s in _selectedItems)
                {
                    // when we remove an item (shape/class) we want to remove the connected connexions
                    foreach (ItemModel i in _items)
                    {
                        if (i.IsConnexion)
                            if (i.InitialItem == s || i.FinalItem == s) { connexionsToRemove.Add(i); }
                    }
                    _items.Remove(s);
                    if (_createdItems.Contains(s))
                    {
                        _createdItems.Remove(s);
                    }
                }
                // we can now remove the connected connexions
                if (connexionsToRemove.Count() != 0)
                    foreach (ItemModel r in connexionsToRemove)
                    {
                        _items.Remove(r);
                        if (_createdItems.Contains(r))
                        {
                            _createdItems.Remove(r);
                        }
                    }
                ClearSelection();
            }
        }
        #region Style for shape/class
        // Fill the current selection (can only fill a shape/class)
        public void Fill()
        {
            foreach (ItemModel s in SelectedItems) { if (!s.IsConnexion) { s.Fill = SelectedColor; } }
        }

        // Color the border of the current selection (shape/class)
        public void ColorBorderShape()
        {
            foreach (ItemModel s in SelectedItems) { if (!s.IsConnexion) { s.Stroke = SelectedColor; } }
        }

        // Apply the border style of the current selection (shape/class)
        public void ApplyStyleShape(string style)
        {
            foreach (ItemModel s in SelectedItems) { if (!s.IsConnexion) { s.StrokeDashArray = style; } }
        }
        #endregion
        #region Style for connexion
        // Color the connexion (to be fixed: only for non-class)
        public void ColorConnexion()
        {
            foreach (ItemModel s in SelectedItems)
            {
                if (s.IsConnexion && s.InitialItem.Type != "classe")
                {
                    s.Stroke = SelectedColor;
                }
            }
        }

        // Apply the border style of the current selection (connexion) (to be fixed: only for non-class)
        public void ApplyStyleConnexion(string style)
        {
            foreach (ItemModel s in SelectedItems)
            {
                if (s.IsConnexion && s.InitialItem.Type != "classe")
                {
                    s.StrokeDashArray = style;
                }
            }
        }

        // Apply the value of the ConnexionThickness
        public void ApplyThickness()
        {
            foreach (ItemModel s in SelectedItems)
            {
                if (s.IsConnexion && s.InitialItem.Type != "classe")
                {
                    s.StrokeThickness = ConnexionThicknessValue.ToString();
                }
            }
        }

        // Up the value of the ConnexionThickness
        public void UpThickness()
        {
            string sNumber = ConnexionThickness.Remove(ConnexionThickness.IndexOf(" "));
            int thickness = Int32.Parse(sNumber);
            thickness++;
            ConnexionThickness = thickness.ToString() + " px";
        }

        // Down the value of the ConnexionThickness
        public void DownThickness()
        {
            string sNumber = ConnexionThickness.Remove(ConnexionThickness.IndexOf(" "));
            int thickness = Int32.Parse(sNumber);
            thickness--;
            if (thickness < 1) { thickness = 1; }
            ConnexionThickness = thickness.ToString() + " px";
        }
        #endregion
        // Select the current tool to use
        public void SelectTool(string outil)
        {
            SelectedTool = outil;
            if (SelectedTool == "resize")
            {
                if (_selectedItems.Count == 0) return;
                else if (_selectedItems.Count == 1)
                {
                    if (!_selectedItems.ElementAt(0).IsConnexion)
                    {
                        _selectedItems.ElementAt(0).ResizeVisibility = "Visible";
                    }
                }
                else if (_selectedItems.Contains(_lasso.RectangleSelection))
                {
                    _lasso.RectangleSelection.ResizeVisibility = "Visible";
                }
            }
            else
            {

                foreach (ItemModel i in _selectedItems)
                {
                    if (!i.IsConnexion)
                    {
                        i.ResizeVisibility = "Hidden";
                    }
                }


            }
            if (SelectedTool == "deplacement")
            {
                foreach (ItemModel s in _selectedItems)
                {
                    s.IsTextEnabled = false;
                    // ajout model
                }
            }
            else
            {
                foreach (ItemModel s in _selectedItems)
                {
                    s.IsTextEnabled = true;
                    //ajout model
                }
            }
        }
        private bool _isMouseDown = false;          // a deplacer
        private IEventAggregator _eventAggregator;  // a deplacer

        // Begin the drag/moe operation
        public void StartMove(ItemModel item)
        {
            if (item.IsConnexion) return;
            if (SelectedTool != "deplacement") return;
            if (!_selectedItems.Contains(item)) return;
            _isMouseDown = true;
        }

        private bool _isMoving = false;
        // Move or drag one or multiple items
        public void Move()
        {
            if (SelectedTool != "deplacement") return;
            if (!_isMouseDown || _selectedItems.Count() == 0) return;
            if (_selectedItems.Count == 1)
            {
                foreach (ItemModel s in _selectedItems)
                {
                    s.Left = MousePosition.X - s.ItemCenterX;
                    s.Top = MousePosition.Y - s.ItemCenterY;

                    //sendMovement(s.Id, s.Left, s.Top);  // provenance du server
                }
            }

            if (Items.Contains(_lasso.RectangleSelection))
            {
                foreach (ItemModel s in SelectedItems)
                {
                    double deltaY = s.Top - _lasso.RectangleSelection.Top - _lasso.RectangleSelection.SelectionHeight / 2;
                    double deltaX = s.Left - _lasso.RectangleSelection.Left - _lasso.RectangleSelection.SelectionWidth / 2;
                    s.Top = MousePosition.Y + deltaY;
                    s.Left = MousePosition.X + deltaX;
                }
                _lasso.RectangleSelection.Top = MousePosition.Y - _lasso.RectangleSelection.SelectionHeight / 2;
                _lasso.RectangleSelection.Left = MousePosition.X - _lasso.RectangleSelection.SelectionWidth / 2;
            }
            foreach (ItemModel cl in _items)
            {
                if (cl.IsConnexion)
                {
                    cl.Update();
                    cl.SetSideText();
                }
            }
            _isMoving = true;
        }

        // End the move/drag operation
        public void EndMove()
        {
            if (SelectedTool != "deplacement") return;
            if (_isMoving)
            {
                _isMouseDown = false;
                _isMoving = false;
            }

        }
        // Resize selected items (shape/class)
        public void Resize(int horizontalChange, int verticalChange, string orientation)
        {
            if (orientation == "left")
            {
                foreach (ItemModel i in _selectedItems)
                {
                    if (!i.IsConnexion)
                    {
                        if (i.ItemWidth - horizontalChange < 30)
                        {
                            i.ItemWidth = 30;
                            return;
                        }

                        if (!(i.ItemWidth < 30))
                        {

                            i.Left += horizontalChange;
                            i.ItemWidth -= horizontalChange;
                        }
                    }
                }
            }
            else if (orientation == "top-left")
            {
                foreach (ItemModel i in _selectedItems)
                {
                    if (!i.IsConnexion)
                    {
                        if (i.ItemWidth - horizontalChange < 30)
                        {
                            i.ItemWidth = 30;
                            return;
                        }
                        if (i.ItemHeight - verticalChange < 30)
                        {
                            i.ItemHeight = 30;
                            return;
                        }

                        if (!(i.ItemWidth < 30))
                        {
                            i.Left += horizontalChange;
                            i.ItemWidth -= horizontalChange;

                        }
                        if (!(i.ItemHeight < 30))
                        {
                            i.Top += verticalChange;
                            i.ItemHeight -= verticalChange;
                        }
                    }

                }
            }
            else if (orientation == "top")
            {
                foreach (ItemModel i in _selectedItems)
                {
                    if (!i.IsConnexion)
                    {
                        if (i.ItemHeight - verticalChange < 30)
                        {
                            i.ItemHeight = 30;
                            return;
                        }

                        if (!(i.ItemHeight < 30))
                        {
                            i.Top += verticalChange;
                            i.ItemHeight -= verticalChange;
                        }
                    }

                }
            }
            else if (orientation == "top-right")
            {
                foreach (ItemModel i in _selectedItems)
                {
                    if (!i.IsConnexion)
                    {
                        if (i.ItemWidth + horizontalChange < 30)
                        {
                            i.ItemWidth = 30;
                            return;
                        }
                        if (i.ItemHeight - verticalChange < 30)
                        {
                            i.ItemHeight = 30;
                            return;
                        }

                        if (!(i.ItemWidth < 30))
                        {
                            i.ItemWidth += horizontalChange;
                        }
                        if (!(i.ItemHeight < 30))
                        {
                            i.Top += verticalChange;
                            i.ItemHeight -= verticalChange;
                        }
                    }

                }
            }
            else if (orientation == "right")
            {
                foreach (ItemModel i in _selectedItems)
                {
                    if (!i.IsConnexion)
                    {
                        if (!(i.ItemWidth < 30))
                        {
                            if (i.ItemWidth + horizontalChange < 30)
                            {
                                i.ItemWidth = 30;
                                return;
                            }
                            i.ItemWidth += horizontalChange;
                        }


                    }

                }

            }
            else if (orientation == "bottom-right")
            {
                foreach (ItemModel i in _selectedItems)
                {
                    if (!i.IsConnexion)
                    {
                        if (i.ItemWidth + horizontalChange < 30)
                        {
                            i.ItemWidth = 30;
                            return;
                        }
                        if (i.ItemHeight + verticalChange < 30)
                        {
                            i.ItemHeight = 30;
                            return;
                        }

                        if (!(i.ItemWidth < 30))
                        {
                            i.ItemWidth += horizontalChange;
                        }
                        if (!(i.ItemHeight < 30))
                        {
                            i.ItemHeight += verticalChange;
                        }
                    }

                }
            }
            else if (orientation == "bottom")
            {
                foreach (ItemModel i in _selectedItems)
                {
                    if (!i.IsConnexion)
                    {
                        if (i.ItemHeight + verticalChange < 30)
                        {
                            i.ItemHeight = 30;
                            return;
                        }
                        if (!(i.ItemHeight < 30))
                        {
                            i.ItemHeight += verticalChange;
                        }
                    }

                }
            }
            else if (orientation == "bottom-left")
            {
                foreach (ItemModel i in _selectedItems)
                {
                    if (!i.IsConnexion)
                    {
                        if (i.ItemWidth - horizontalChange < 30)
                        {
                            i.ItemWidth = 30;
                            return;
                        }
                        if (i.ItemHeight + verticalChange < 30)
                        {
                            i.ItemHeight = 30;
                            return;
                        }
                        if (!(i.ItemWidth < 30))
                        {
                            i.Left += horizontalChange;
                            i.ItemWidth -= horizontalChange;
                        }
                        if (!(i.ItemHeight < 30))
                        {
                            i.ItemHeight += verticalChange;
                        }
                    }

                }
            }

            // serveur en lien avec multiplicité
            foreach (ItemModel cl in _items)
            {
                if (cl.IsConnexion)
                {
                    cl.Update();
                    cl.SetSideText();
                }

            }

        }

        // rotate a shape right (shape/class) (incomplete)
        public void RotateRight()
        {
            foreach (ItemModel s in _selectedItems)
            {
                if (s.IsConnexion) continue;
                s.RotationAngle += 90;
                if (s.RotationAngle > 360) { s.RotationAngle = 90; }
            }

            foreach (ItemModel cl in _items)
            {
                if (cl.IsConnexion)
                {
                    cl.Update();
                    cl.SetSideText();

                }
            }

        }
        // rotate a shape left (shape/class) (incomplete)
        public void RotateLeft()
        {

            foreach (ItemModel s in _selectedItems)
            {
                if (s.IsConnexion) continue;
                s.RotationAngle -= 90;
                if (s.RotationAngle < 0) { s.RotationAngle = 270; }

            }
            foreach (ItemModel cl in _items)
            {
                if (cl.IsConnexion)
                {
                    cl.Update();
                    cl.SetSideText();

                }
            }
        }

        // Select the current connexion to use between shapes
        public void SelectShapeConnexion(string shapeConnexion) => SelectedShapeConnexion = shapeConnexion;

        // Select the current connexion to use between classes
        public void SelectClassInteraction(string classInteraction) => SelectedClassInteraction = classInteraction;

        #region Stack operations
        // Update every guards
        public void UpdateCans()
        {
            this.NotifyOfPropertyChange(() => CanPop);
            this.NotifyOfPropertyChange(() => CanPush);
        }

        // Guard for pushing to the stack
        public bool CanPush
        { get { return (Items.Count > 0 && _createdItems.Count > 0); } }

        // Push to the stack
        public void Push()
        {
            try
            {
                ItemModel item = _createdItems.Last();

                _stack.Add(item);
                _items.Remove(item);
                _createdItems.Remove(item);

                item.FillConnector = "Transparent";
                item.FillLineConnector = "Transparent";
                item.ResizeVisibility = "Hidden";
                item.IsTextEnabled = true;  // itemModel
                item.User = "";


                if (_selectedItems.Contains(item))
                {
                    _selectedItems.Remove(item);
                }


                this.NotifyOfPropertyChange(() => CanPush);
                this.NotifyOfPropertyChange(() => CanPop);
            }
            catch { }
        }
        // Guard for pushing to the stack


        // Guard for popping the stack
        public bool CanPop { get { return Stack.Count > 0; } }

        // Pop from the stack
        public void Pop()
        {
            try
            {
                ItemModel item = Stack.Last();
                Stack.Remove(item);
                Items.Add(item);
                item.Id = ++itemCounter;
                _createdItems.Add(item);
                this.NotifyOfPropertyChange(() => CanPush);
                this.NotifyOfPropertyChange(() => CanPop);
            }
            catch { }
        }
        #endregion
        // Clean the canvas of all items (will not remove items from the stack or the copied items)
        public void Clean()
        {
            Items.Clear();
            _selectedItems.Clear();
            _createdItems.Clear();
            Stack.Clear();
            UpdateCans();
            itemCounter = 0;
        }

        // cette methode est fautif (a corriger)
        public void Duplicate()
        {
            bool isThereDiagram = false;
            foreach (ItemModel s in _selectedItems)
            {
                if (!s.IsConnexion)
                {
                    isThereDiagram = true;
                    break;
                }
            }
            if (SelectedItems.Count != 0 && isThereDiagram)   // if there are selected items: copy and paste them
            {
                
                #region nouvelle version
                CopiedItems.Clear();
                foreach (ItemModel s in SelectedItems)
                {
                    if (s.IsConnexion) continue;
                    ItemModel CopiedItem = new ItemModel(s);
                    CopiedItems.Add(CopiedItem);
                }
                ClearSelection();
                foreach (ItemModel c in CopiedItems)
                {
                    ItemModel pastedItem = new ItemModel(c);
                    Items.Add(pastedItem);
                    _selectedItems.Add(pastedItem);
                    if (pastedItem.Type == "lasso")
                    {
                        _lasso.RectangleSelection = pastedItem;
                        pastedItem.Top += 10;
                        pastedItem.Left += 10;
                    }
                    if (pastedItem.Type != "lasso")
                    {
                        pastedItem.Id = ++itemCounter;
                        pastedItem.Top += 10;
                        pastedItem.Left += 10;
                        pastedItem.FillConnector = OnSelectionColor;
                        pastedItem.User = User;
                    }
                }

                #endregion

            }
            else if (CopiedItems.Count != 0 && SelectedItems.Count == 0) // if there are no selected items: paste the last copied items
            {

                bool isLassoInside = false;
                foreach (ItemModel c in CopiedItems)
                {
                    c.User = User;
                    ItemModel pastedItem = new ItemModel(c);
                    Items.Add(pastedItem);
                    _selectedItems.Add(pastedItem);
                    if (pastedItem.Type == "lasso")
                    {
                        _lasso.RectangleSelection = pastedItem;
                        isLassoInside = true;

                    }
                    if (pastedItem.Type != "lasso")
                    {
                        pastedItem.Id = ++itemCounter;

                    }
                    pastedItem.ResizeVisibility = "Hidden";
                    pastedItem.IsTextEnabled = true;
                }
                if (SelectedTool == "resize")
                {
                    if (isLassoInside)
                    {
                        foreach (ItemModel s in _selectedItems)
                        {
                            if (s.Type == "lasso")
                            {
                                s.ResizeVisibility = "Visible";
                            }
                            else
                            {
                                s.ResizeVisibility = "Hidden";
                            }
                        }
                    }
                    else if (_selectedItems.Count() == 1)
                    {
                        _selectedItems.ElementAt(0).ResizeVisibility = "Visible";
                    }
                }
                if (SelectedTool == "deplacement")
                {
                    if (isLassoInside)
                    {
                        foreach (ItemModel s in _selectedItems)
                        {
                            s.IsTextEnabled = false;
                        }
                    }
                    else if (_selectedItems.Count() == 1)
                    {
                        _selectedItems.ElementAt(0).IsTextEnabled = false;
                    }
                }
            }
        }


        // Select a single item or a collection of connexion (left click) (to be fixed overall)
        public void Select(ItemModel item)
        {
            #region version desiree
            if (item.User == "" || item.User == User)
            {
                if (item == _lasso.RectangleSelection) return;
                //if (_selectedItems.Contains(item) && SelectedTool == "resize") return;
                ClearSelection();
                item.IsTextHittable = true;
                if (!item.IsConnexion)
                {
                    item.FillConnector = OnSelectionColor;
                    SelectedItems.Add(item);
                    item.User = User;
                    if (SelectedTool == "resize")
                    {
                        item.ResizeVisibility = "Visible";
                    }
                    if (SelectedTool == "deplacement")
                    {
                        item.IsTextEnabled = false;  // ItemModel
                    }

                }
                if (item.IsConnexion)
                {
                    item.FillLineConnector = OnSelectionColor;
                    foreach (ItemModel cl in _items)
                    {
                        if (cl.IsConnexion)
                        {
                            foreach (ItemModel cl2 in _items)
                            {
                                if (cl2.IsConnexion)
                                {
                                    if (item.FinalItem == cl2.FinalItem && item.InitialItem == cl2.InitialItem)
                                    {
                                        cl2.FillLineConnector = OnSelectionColor;
                                        cl2.User = User;
                                        SelectedItems.Add(cl2);
                                    }
                                }

                            }
                        }
                    }

                }
            }


            #endregion
        }

        // Clear the current selection (to be updated)
        public void ClearSelection()
        {
            foreach (ItemModel s in SelectedItems)
            {
                s.FillConnector = "Transparent";
                s.FillLineConnector = "Transparent";
                s.ResizeVisibility = "Hidden";
                s.IsTextEnabled = true;  // itemModel
                s.User = "";
            }

            Items.Remove(_lasso.RectangleSelection);
            _selectedItems.Remove(_lasso.RectangleSelection);
            SelectedItems.Clear();
        }

        // Create an item (shapes and class)
        public void Create(string type)
        {
            ItemModel item = new ItemModel()
            {
                Id = ++itemCounter,             // temporaire
                Top = MousePosition.Y,
                Left = MousePosition.X,
                Type = type,
            };

            item.User = User;
            item.Creator = User;
            _createdItems.Add(item);

            Items.Add(item);
            Select(item);
        }


        // insert an image in the canvas (works offline) (Byte[] must match with thin client)
        
        public void InsertImage()
        {
            ClearSelection();
            string ImageSourceFile = "";
            byte[] byteImageData = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.bmp;*.tiff)|*.png;*.jpeg;*.jpg;*.bmp;*.tiff|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                ImageSourceFile = openFileDialog.FileName;
                byteImageData = File.ReadAllBytes(ImageSourceFile);
            }
            else { return; }
            System.Drawing.Bitmap img = new System.Drawing.Bitmap(ImageSourceFile);

            int imageHeight = img.Height;
            int imageWidth = img.Width;
            ItemModel image = new ItemModel(imageWidth, imageHeight, byteImageData);
            _items.Add(image);
            Select(image);
            SelectTool("deplacement");
        }
        
        // Every right click operation (adding shapes/class mostly)
        public void RightClickOperation()
        {
            if (SelectedTool == "lasso") { return; }
            else if (SelectedTool == "image") { return; }
            else if (SelectedTool == "resize") { return; }
            else if (SelectedTool == "deplacement") { return; }
            else { Create(SelectedTool); }
            UpdateCans();
        }

        #region Lasso selection
        // Start a lasso selection (left click down)
        public void StartLasso()
        {
            if (SelectedTool == "lasso")
            {
                ClearSelection();
                _lasso.Start(MousePosition, Items);
                //_isMouseDown = true;
            }
        }

        // Make the lasso selection (left click pressed)
        public void MakeLasso(MouseEventArgs e)
        {
            //if (!_isMouseDown) return;
            if (SelectedTool == "lasso")
            {
                if (e.LeftButton == MouseButtonState.Pressed) { _lasso.Make(MousePosition); }
            }
        }

        // End the lasso selection (left click up)
        public void EndLasso()
        {
            if (SelectedTool == "lasso")
            {
                foreach (ItemModel i in Items)
                {
                    if (i.Left > _lasso.MinX && i.Left + i.ItemWidth < _lasso.MaxX
                        && i.Top > _lasso.MinY && i.Top + i.ItemHeight < _lasso.MaxY)
                    {
                        if (i.User == User || i.User == "")
                        {
                            SelectedItems.Add(i);
                            i.User = User;
                            if (!i.IsConnexion)
                            {
                                i.FillConnector = OnSelectionColor;
                            }

                            if (i.IsConnexion)
                            {
                                i.FillLineConnector = OnSelectionColor;

                                foreach (ItemModel cl in _items)
                                {
                                    if (cl.IsConnexion)
                                    {
                                        foreach (ItemModel cl2 in _items)
                                        {
                                            if (cl2.IsConnexion)
                                            {
                                                if (i.FinalItem == cl2.FinalItem && i.InitialItem == cl2.InitialItem)
                                                {
                                                    cl2.FillLineConnector = OnSelectionColor;
                                                    cl2.User = User;
                                                    SelectedItems.Add(cl2);
                                                }
                                            }

                                        }
                                    }
                                }

                            }
                        }

                    }
                }
                if (SelectedItems.Count() == 0)
                {
                    Items.Remove(_lasso.RectangleSelection);
                    ClearSelection();
                }
                else
                {
                    _lasso.SetRectangleSize(SelectedItems);
                    _selectedItems.Add(_lasso.RectangleSelection);
                    SelectTool("deplacement");
                }
            }
        }
        #endregion
        #region connexions manipulations
        #region linking two items
        // to be refactored
        // when we are starting a connexion from a connector (set values and prepare for drag)
        public void StartConnexion(ItemModel item, string orientation)
        {
            // initial restriction
            if (item.User != User) return;
            if (!IsForm()) return; // can only start connexion if
            if (item.Type == "texte" || item.Type == "image") { return; }
            // initial restriction (thick-client only)
            if ((item.Type == "role" || item.Type == "artefact" || item.Type == "activite")
                && (SelectedShapeConnexion != "directionnalArrow"))
                return;
            // creating the item: a link 
            _clParent = new ItemModel();
            PathGeometry pg = new PathGeometry();
            ItemModel linkItem = new ItemModel()
            {
                Top = 0,
                Left = 0,
                Type = "",
                Geometry = pg.ToString(),

                Visibility1 = "Hidden",
                IsConnexion = true,
                IsConnectorHittable = false,
                IsLineConnectorHittable = true,
            };
            _items.Add(linkItem);    // adding it to _items to show it in the canvas

            _clParent = linkItem;                       // save the link item for use
            _clParent.StartItem = item;                 // the initial item is the first item(connector) we clicked
            _clParent.StartOrientation = orientation;   // the orientation of the connector that we clicked
            _clParent.IsConnecting = true;
            _clParent.IsMatch = false;
            // setting the starting point of the link with the orientation
            if (orientation == "right") { _clParent.StartPoint = _clParent.StartItem.RightConnectorAbsolute; }
            if (orientation == "top") { _clParent.StartPoint = _clParent.StartItem.TopConnectorAbsolute; }
            if (orientation == "left") { _clParent.StartPoint = _clParent.StartItem.LeftConnectorAbsolute; }
            if (orientation == "bottom") { _clParent.StartPoint = _clParent.StartItem.BottomConnectorAbsolute; }

            // ?
            StartOrientation = orientation;     // ajout server
            // a non-class item can only be linked to a non-class item
            if (_clParent.StartItem.Type != "classe")
            {
                _clParent.IsClass = false;
                _clParent.Type = SelectedShapeConnexion;
                _clParent.Geometry = _connexionGenerator.Create(SelectedShapeConnexion, _clParent.StartPoint, MousePosition);   // creating the geometry
                _clParent.Fill = "Black";

                if (_clParent.Type == "directionnalArrow")
                {
                    _clParent.Fill = "Black";
                }
                if (_clParent.Type == "bidirectionnalArrow")
                {
                    _clParent.Fill = "Black";
                }
            }
            // a class item can only be linked to a class item
            else if (_clParent.StartItem.Type == "classe")
            {
                _clParent.IsClass = true;
                _clParent.Type = SelectedClassInteraction;
                //_clParent.Type = SelectedClassInteraction;
                _clParent.Geometry = _connexionGenerator.Create(SelectedClassInteraction, _clParent.StartPoint, MousePosition);

                if (_clParent.Type == "compositionLink")
                {
                    _clParent.Fill = "Black";
                }
                if (_clParent.Type == "aggregationLink")
                {
                    _clParent.Fill = "White";
                }
            }
        }
        // when we are currently linking the first item to something (preview)
        public void Link(MouseEventArgs e)
        {
            if (_clParent.IsConnecting)
            {
                if (!_clParent.IsMatch)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        // we are now creating the link (the preview) while we are dragging
                        if (_clParent.StartItem.Type != "classe")
                        {
                            _clParent.Type = SelectedShapeConnexion;
                            _clParent.Geometry = _connexionGenerator.Create(SelectedShapeConnexion, _clParent.StartPoint, MousePosition);
                        }
                        else if (_clParent.StartItem.Type == "classe")
                        {

                            _clParent.Type = SelectedClassInteraction;
                            _clParent.Geometry = _connexionGenerator.Create(SelectedClassInteraction, _clParent.StartPoint, MousePosition);
                        }
                        foreach (ItemModel i in _items) // peut etre ajout de restriction pour mieux identifier
                        {
                            if (!i.IsConnexion)
                            {

                                if (_clParent.StartItem.Type == "role")
                                {
                                    if (i.Type == "activite") { i.MovingStrokeColor = OnSelectionColor; }
                                }
                                else if (_clParent.StartItem.Type == "artefact")
                                {
                                    if (i.Type == "activite") { i.MovingStrokeColor = OnSelectionColor; }
                                }
                                else if (_clParent.StartItem.Type == "activite")
                                {
                                    if (i.Type == "artefact") { i.MovingStrokeColor = OnSelectionColor; }
                                }
                                else if (_clParent.StartItem.Type == "classe")
                                {
                                    if (i.Type == "classe") { i.MovingStrokeColor = OnSelectionColor; }
                                }
                                else
                                {
                                    i.MovingStrokeColor = OnSelectionColor;
                                }
                            }
                        }
                    }
                }
            }
        }
        // when we are done connecting (if hit add it to the canvs/ when miss remove it from the canvas)
        public void StopConnecting()
        {
            foreach (ItemModel i in _items)
            {
                if (!i.IsConnexion)
                {
                    i.MovingStrokeColor = "Transparent";
                }
            }
            _clParent.User = User;
            // on hit sucess we are now setting every necessary attributes
            if (_clParent.IsConnecting && _clParent.IsMatch)
            {
                _clParent.IsConnecting = false;
                _clParent.LineConnectorLeft = ((_clParent.EndPoint.X - _clParent.StartPoint.X) / 2) + _clParent.StartPoint.X - 5;
                _clParent.LineConnectorTop = ((_clParent.EndPoint.Y - _clParent.StartPoint.Y) / 2) + _clParent.StartPoint.Y - 5;
                _clParent.InitialItem = _clParent.StartItem;
                _clParent.FinalItem = _clParent.EndItem;

                _clParent.Visibility1 = "Visible";
                _clParent.TextWidth1 = 100;
                _clParent.Text1Height = 20;
                _clParent.Text1 = "text";

                if (_clParent.InitialItem.Type == "classe")
                {
                    _clParent.SetSideText();
                    _clParent.Visibility2 = "Visible";
                    _clParent.Visibility3 = "Visible";
                    _clParent.TextWidthBoth = 40;
                    _clParent.Text2Height = 20;
                    _clParent.Text3Height = 20;
                    _clParent.Text2 = "#";
                    _clParent.Text3 = "#";
                    //_clParent.IsReadOnly = true;  // test
                }
                _clParent.Id = ++itemCounter;                       // temp serialize
                _clParent.EndItemId = _clParent.EndItem.Id; // temp serialize
                _clParent.StartItemId = _clParent.StartItem.Id;    // temp serialize
                _clParent.InitialItemId = _clParent.StartItem.Id;   // temp serialize
                _clParent.FinalItemId = _clParent.EndItem.Id;       // temp serialize

                // fin ajout serveur
                ClearSelection();


                Select(_clParent);
            }
            // on hit failure we remove the previewed item
            else if (_clParent.IsConnecting)
            {
                _clParent.IsConnecting = false;
                Items.Remove(_clParent);
            }
        }
        // when we are moving to a connector it will check the restriction and show the connectors colors
        public void MovingInConnector(ItemModel item, string orientation)
        {
            if (_clParent.IsConnecting)
            {
                if (_clParent.StartItem == item) return;
                // CORRECTION: RESTRICTION SUPPLEMENTAIRE ICI !!!!!!!!!!!!!!! -stpham
                // cannot match again to the same initial to final item
                foreach (ItemModel c in _items)
                {
                    if (c.IsConnexion && c != _clParent)
                        if (_clParent.StartItem == c.InitialItem && c.FinalItem == item) return;
                }
                // cannot match to a text or an image
                if (item.Type == "texte" || item.Type == "image") { return; }
                // restriction class-class non-class - non-class for the final item
                if (_clParent.IsClass && item.Type != "classe") { return; }
                if (!_clParent.IsClass && item.Type == "classe")
                {
                    if (_clParent.StartItem.Type == "commentaire") { }
                    else { return; }
                }

                // restriction (only for the thick client)
                if ((_clParent.StartItem.Type == "role" && item.Type != "activite")
                    || (_clParent.StartItem.Type == "artefact" && item.Type != "activite")
                    || (_clParent.StartItem.Type == "activite" && item.Type != "artefact")
                    )
                    return;
                // prepare the settings for the final item
                _clParent.EndItem = item;
                _clParent.EndOrientation = orientation;
                if (orientation == "right") { _clParent.EndPoint = _clParent.EndItem.RightConnectorAbsolute; }
                if (orientation == "top") { _clParent.EndPoint = _clParent.EndItem.TopConnectorAbsolute; }
                if (orientation == "left") { _clParent.EndPoint = _clParent.EndItem.LeftConnectorAbsolute; }
                if (orientation == "bottom") { _clParent.EndPoint = _clParent.EndItem.BottomConnectorAbsolute; }
                EndOrientation = orientation;   // ajout serveur

                if (_clParent.StartItem.Type != "classe")
                {
                    _clParent.Type = SelectedShapeConnexion;
                    _clParent.Geometry = _connexionGenerator.Create(SelectedShapeConnexion, _clParent.StartPoint, _clParent.EndPoint);
                }
                else if (_clParent.StartItem.Type == "classe")
                {
                    _clParent.Type = SelectedClassInteraction;
                    _clParent.Geometry = _connexionGenerator.Create(SelectedClassInteraction, _clParent.StartPoint, _clParent.EndPoint);
                }
                _clParent.IsMatch = true;
            }
        }
        // when we are leaving the connector it will hide the connectors of the final item
        public void LeaveConnector()
        {
            if (_clParent.IsConnecting)
            {
                // will not link if its the same item
                if (_clParent.EndItem != null && _clParent.EndItem != _clParent.StartItem)
                    //_clParent.EndItem.= "Transparent";

                    _clParent.IsMatch = false;
            }
        }
        // return a connexion
        public ItemModel GetConnexionLink(ItemModel itemLink)
        {
            // will only return a link
            if (itemLink.IsConnexion)
                return itemLink;
            return null;
        }
        #endregion
        #region Angling and resizing an angle
        // to be refactored
        // when we are starting a division (set or get all the necessary values)
        public void StartAngle(ItemModel itemLink)
        {
            if (!IsForm()) return;
            _clParent = GetConnexionLink(itemLink);
            if (_clParent.IsSeparated)          // when resizing a link: we get the link and his start and end items
            {
                _clParent.IsAlterning = true;
                _clStart = _clParent.StartItem;
                _clEnd = _clParent.EndItem;
                _isMouseDown = true;
                return;
            }
            _clStart = new ItemModel()
            {
                Top = 0,
                Left = 0,
                Type = "",

                Visibility1 = "Hidden",
                IsConnexion = true,
                IsConnectorHittable = false,
                IsLineConnectorHittable = true,
                // the new links will have the same style as the parent
                Stroke = _clParent.Stroke,
                StrokeDashArray = _clParent.StrokeDashArray,
                StrokeThickness = _clParent.StrokeThickness,
                Fill = _clParent.Fill,
            };
            _clEnd = new ItemModel()
            {
                Top = 0,
                Left = 0,
                Type = "",

                Visibility1 = "Hidden",
                IsConnexion = true,
                IsConnectorHittable = false,
                IsLineConnectorHittable = true,
                Stroke = _clParent.Stroke,
                StrokeDashArray = _clParent.StrokeDashArray,
                StrokeThickness = _clParent.StrokeThickness,
                Fill = _clParent.Fill,
            };
            // setting the starting item
            _clStart.Id = ++itemCounter;        // temporaire
            _clEnd.Id = ++itemCounter;          // temporaire
            _clStart.StartItem = _clParent.StartItem;
            _clStart.StartItemId = _clParent.StartItem.Id;        // temp serialize
            _clStart.EndItem = _clParent;
            _clStart.EndItemId = _clParent.Id;            // temp serialize
            _clStart.StartPoint = _clParent.StartPoint;
            _clStart.Type = _clParent.Type;
            _clStart.InitialItem = _clParent.InitialItem;
            _clStart.FinalItem = _clParent.FinalItem;
            _clStart.StartOrientation = _clParent.StartOrientation;
            _clStart.EndOrientation = _clParent.EndOrientation;
            _clStart.InitialItemId = _clParent.InitialItemId;   // temp serialize
            _clStart.FinalItemId = _clParent.FinalItemId;   // temp serialize

            // setting the ending item
            _clEnd.StartItem = _clParent;
            _clEnd.StartItemId = _clParent.Id;      // temp serialize
            _clEnd.EndItem = _clParent.EndItem;
            _clEnd.EndItemId = _clParent.EndItem.Id;    // temp serialize
            _clEnd.EndPoint = _clParent.EndPoint;
            _clEnd.Type = _clParent.Type;
            _clEnd.InitialItem = _clParent.InitialItem;
            _clEnd.FinalItem = _clParent.FinalItem;
            _clEnd.StartOrientation = _clParent.StartOrientation;
            _clEnd.EndOrientation = _clParent.EndOrientation;
            _clEnd.InitialItemId = _clParent.InitialItemId;   // temp serialize
            _clEnd.FinalItemId = _clParent.FinalItemId;   // temp serialize

            // once the setting of the new links are done: we need to change the parent link because he is not linked to the same items
            _clParent.StartItem = _clStart;
            _clParent.StartItemId = _clStart.Id;    // temp serialize
            _clParent.EndItem = _clEnd;
            _clParent.EndItemId = _clEnd.Id; // temp serialize
            _clParent.IsAngling = true;
            _clParent.IsAdding = true;
            _isMouseDown = true;
        }
        // when we are currently modifying a connexion (dividing a connexion / resizing a connexion)
        public void Angling(MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                // we are now breaking a line to have 2 new lines (can only be done once)
                if (_clParent.IsAngling)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (_clParent.IsAdding)
                        {
                            // we can now show the item to the canvas
                            Items.Add(_clStart);
                            Items.Add(_clEnd);
                            _clParent.IsAdding = false;

                        }
                        // the middle point now control the end point of the start item and the starting point of the end item
                        _clStart.EndPoint = MousePosition;
                        _clEnd.StartPoint = MousePosition;
                        _clStart.Geometry = _connexionGenerator.Create(_clStart.SetInitialItemType(), _clStart.StartPoint, _clStart.EndPoint);
                        _clEnd.Geometry = _connexionGenerator.Create(_clEnd.SetFinalItemType(), _clEnd.StartPoint, _clEnd.EndPoint);

                    }
                }
                // we are resizing the breaking point of the line (can only be done once we went to the Angling once)
                if (_clParent.IsSeparated && _clParent.IsAlterning)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        _clParent.IsDoneAlterning = false;
                        _clStart.EndPoint = MousePosition;
                        _clEnd.StartPoint = MousePosition;
                        _clStart.Geometry = _connexionGenerator.Create(_clStart.SetInitialItemType(), _clStart.StartPoint, _clStart.EndPoint);
                        _clEnd.Geometry = _connexionGenerator.Create(_clEnd.SetFinalItemType(), _clEnd.StartPoint, _clEnd.EndPoint);
                    }
                }
            }
        }


        // when we are stopping the angling (dividing a connexion/ resizing a connexion)
        public void StopAngling()
        {
            // we broke the link into two links, we must set the line connector 
            // (the line connector is important to break those links into two other links)
            if (_clParent.IsAngling && !_clParent.IsAdding && !_clParent.IsSeparated)
            {
                _clStart.LineConnectorLeft = ((_clStart.EndPoint.X - _clStart.StartPoint.X) / 2) + _clStart.StartPoint.X - 5;
                _clStart.LineConnectorTop = ((_clStart.EndPoint.Y - _clStart.StartPoint.Y) / 2) + _clStart.StartPoint.Y - 5;

                _clEnd.LineConnectorLeft = ((_clEnd.EndPoint.X - _clEnd.StartPoint.X) / 2) + _clEnd.StartPoint.X - 5;
                _clEnd.LineConnectorTop = ((_clEnd.EndPoint.Y - _clEnd.StartPoint.Y) / 2) + _clEnd.StartPoint.Y - 5;

                _clParent.Geometry = "";

                _clParent.LineConnectorLeft = _clStart.EndPoint.X - 5;
                _clParent.LineConnectorTop = _clStart.EndPoint.Y - 5;
                _clParent.IsAngling = false;
                _clParent.IsSeparated = true;

                // important: we must check the start item of the start item and change the reference if necessary
                foreach (ItemModel cl in _items)
                {

                    if (_clStart.StartItem == cl)
                    {
                        cl.EndItem = _clStart;
                        cl.EndItemId = cl.EndItem.Id;   // temp serialize
                        break;
                    }
                }
                // important: we must check the end item of the end item and change the reference if necessary
                foreach (ItemModel cl in _items)
                {
                    if (_clEnd.EndItem == cl)
                    {

                        cl.StartItem = _clEnd;
                        cl.StartItemId = cl.StartItem.Id;   // temp serialize
                        break;
                    }
                }
                // we can now clear the selection and select the newly creating line
                ClearSelection();
                Select(_clParent);
            }

            // the resizing of the links, follow the same steps as above except the references part
            if (!_clParent.IsDoneAlterning && _clParent.IsSeparated)
            {
                _clStart.LineConnectorLeft = ((_clStart.EndPoint.X - _clStart.StartPoint.X) / 2) + _clStart.StartPoint.X - 5;
                _clStart.LineConnectorTop = ((_clStart.EndPoint.Y - _clStart.StartPoint.Y) / 2) + _clStart.StartPoint.Y - 5;

                _clEnd.LineConnectorLeft = ((_clEnd.EndPoint.X - _clEnd.StartPoint.X) / 2) + _clEnd.StartPoint.X - 5;
                _clEnd.LineConnectorTop = ((_clEnd.EndPoint.Y - _clEnd.StartPoint.Y) / 2) + _clEnd.StartPoint.Y - 5;

                _clParent.LineConnectorLeft = _clStart.EndPoint.X - 5;
                _clParent.LineConnectorTop = _clStart.EndPoint.Y - 5;
                _clParent.IsDoneAlterning = true;
                _clParent.IsAlterning = false;
            }

        }
        #endregion
        #endregion
        #region Saving and Loading
        // Save locally to an xml file
        public void Save()
        {
            // copy every item without reference
            ObservableCollection<ItemModel> im = new ObservableCollection<ItemModel>();
            foreach (ItemModel i in _items)
            {
                ItemModel item = new ItemModel(i);
                im.Add(item);
            }
            im.ElementAt(0).CanvasModel = _canvas;

            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<ItemModel>));
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML-File | *.xml";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (StreamWriter wr = new StreamWriter(saveFileDialog.FileName))
                {
                    xs.Serialize(wr, im);
                }
            }
        }
        // Load locally an xml file
        public void Load()
        {
            ObservableCollection<ItemModel> im = new ObservableCollection<ItemModel>();
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<ItemModel>));
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files | *.xml";
            // load the specified file
            if (openFileDialog.ShowDialog() == true)
            {
                _items.Clear();
                using (StreamReader rd = new StreamReader(openFileDialog.FileName))
                {
                    im = xs.Deserialize(rd) as ObservableCollection<ItemModel>;
                }
                if (im.Count != 0)
                {
                    CanvasName = im.ElementAt(0).CanvasModel.Name;
                    CanvasWidth = im.ElementAt(0).CanvasModel.Width;
                    CanvasHeight = im.ElementAt(0).CanvasModel.Height;
                    _canvas.Creator = im.ElementAt(0).CanvasModel.Creator;
                    IsDoneAddingCanvas = true;
                    im.ElementAt(0).CanvasModel = null;
                }
            }
            else return;

            // we can now add every item that we loaded to our current items
            foreach (ItemModel i in im)
            {
                _items.Add(i);
            }
            int count = 0;
            // remove any selection color
            foreach (ItemModel i in _items)
            {
                i.FillConnector = "Transparent";
                i.FillLineConnector = "Transparent";
                if (i.IsConnexion)
                {
                    // we must re add those items because of the circular reference problem (we must use the id)
                    foreach (ItemModel i2 in _items)
                    {
                        if (i.StartItemId == i2.Id) { i.StartItem = i2; }
                        if (i.EndItemId == i2.Id) { i.EndItem = i2; }
                        if (i.InitialItemId == i2.Id) { i.InitialItem = i2; }
                        if (i.FinalItemId == i2.Id) { i.FinalItem = i2; }
                        if (i.Id > count) { count = i.Id; }     // we must reset the count to match the loaded file
                    }
                }
                _id = count;
            }
            MessageBox.Show("Loaded");
        }
        public void LoadTutorial()
        {
            dynamic settings = new ExpandoObject();
            settings.Title = "Tutorial";
            //_windowManager.ShowWindow(new TutorialViewModel(_eventAggregator), null, settings);
            IsTutorialOn = true;
            NotifyOfPropertyChange(() => CanLoadTutorial);
        }

        public void LoadCanvasCreation()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            settings.MinWidth = 500;
            settings.Width = 400;
            settings.Title = "Create";
            settings.ResizeMode = ResizeMode.NoResize;
            _windowManager.ShowDialog(new CanvasCreationViewModel(_eventAggregator), null, settings);

        }


        public bool CanLoadTutorial
        {
            get
            {
                return !IsTutorialOn;
            }
        }

        public string NameSetter(string type)
        {
            if (type == "artefact") { return "Work Product"; }
            else if (type == "activite") { return "Activity"; }
            else if (type == "classe") { return "Class"; }
            else if (type == "commentaire") { return "Annotation"; }
            else if (type == "phase") { return "Phase"; }
            else if (type == "role") { return "Role"; }
            else if (type == "unidirectionnalLink") { return "Unidirectionnal Link"; }
            else if (type == "inheritanceLink") { return "Inheritance Link"; }
            else if (type == "bidirectionnalLink") { return "Bidirectionnal Link"; }
            else if (type == "aggregationLink") { return "Aggregation Link"; }
            else if (type == "compositionLink") { return "Composition Link"; }
            else if (type == "lineConnexion") { return "Bidirectionnal Link"; }
            else if (type == "directionnalArrow") { return "Directionnal Arrow"; }
            else if (type == "bidirectionnalArrow") { return "Bidirectionnal Arrow"; }
            return "Item";
        }
        #endregion

        #region test methods
        public void getItemsCount() => MessageBox.Show(Items.Count().ToString());
        public void getCopiedItemsCount() { MessageBox.Show(CopiedItems.Count().ToString()); }
        public void getSelectedItemsCount() { MessageBox.Show(SelectedItems.Count().ToString()); }
        public void getStackItemsCount() { MessageBox.Show(Stack.Count().ToString()); }
        public void getConnexionLinksCount()
        {
            int count = 0;
            foreach (ItemModel i in _items)
            {
                if (i.IsConnexion)
                {
                    count++;
                }
            }
            MessageBox.Show(count.ToString());
        }


        /*
        public void Handle(TutorialMessage message)
        {
            if (message.Message == "closing tutorial")
            {
                IsTutorialOn = false;
                NotifyOfPropertyChange(() => CanLoadTutorial);
            }
        }
        */

        public void Handle(CanvasCreationMessage message)
        {
            IsDoneAddingCanvas = message.IsDoneAdding;
            _canvas = message.Canvas;
            _canvas.Creator = User;

            NotifyOfPropertyChange(() => CanvasName);
            NotifyOfPropertyChange(() => CanvasWidth);
            NotifyOfPropertyChange(() => CanvasHeight);
            MidCanvasX = Convert.ToInt32(CanvasWidth / 2);
            MidCanvasY = Convert.ToInt32(CanvasHeight / 2);

        }

        public void Handle(CanvasLoadingMessage message)
        {
            if (message.Loading == "Load Canvas")
            {
                Load();
            }
        }


        #endregion

        #region getters / setters
        public int MidCanvasX
        {
            get { return _midCanvasX; }
            set
            {
                if (_midCanvasX != value)
                {
                    _midCanvasX = value;
                    NotifyOfPropertyChange(() => MidCanvasX);
                }
            }
        }

        public int MidCanvasY
        {
            get { return _midCanvasY; }
            set
            {
                if (_midCanvasY != value)
                {
                    _midCanvasY = value;
                    NotifyOfPropertyChange(() => MidCanvasY);
                }
            }
        }

        public ObservableCollection<ItemModel> CopiedItems
        {
            get { return _copiedItems; }
            set
            {
                if (_copiedItems != value)
                {
                    _copiedItems = value;
                    NotifyOfPropertyChange(() => CopiedItems);
                }
            }
        }
        public ObservableCollection<ItemModel> Items
        {
            get { return _items; }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    NotifyOfPropertyChange(() => Items);
                }
            }
        }
        public ObservableCollection<ItemModel> SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                if (_selectedItems != value)
                {
                    _selectedItems = value;
                    NotifyOfPropertyChange(() => SelectedItems);
                }
            }
        }
        public ObservableCollection<ItemModel> Stack
        {
            get { return _stack; }
            set
            {
                if (_stack != value)
                {
                    _stack = value;
                    NotifyOfPropertyChange(() => Stack);
                }
            }
        }

        public string CanvasName
        {
            get { return _canvas.Name; }
            set
            {
                if (_canvas.Name != value)
                {
                    _canvas.Name = value;
                    NotifyOfPropertyChange(() => CanvasName);
                }
            }
        }




        public Point MousePosition
        {
            get { return _mousePosition; }
            set { _mousePosition = value; }
        }
        public string TextBlockPosition
        {
            get { return _textBlockPosition; }
            set
            {
                _textBlockPosition = value;
                NotifyOfPropertyChange(() => TextBlockPosition);
            }
        }
        public string SelectedTool
        {
            get { return _editor.SelectedTool; }
            set
            {
                _editor.SelectedTool = value;
                NotifyOfPropertyChange(() => SelectedTool);
            }
        }
        public string SelectedColor
        {
            get { return _editor.SelectedColor; }
            set
            {
                _editor.SelectedColor = value;
                NotifyOfPropertyChange(() => SelectedColor);
            }
        }
        public string UserName
        {
            get { return _editor.UserName; }
            set
            {
                _editor.UserName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }
        public string OnSelectionColor
        {
            get { return _editor.OnSelectionColor; }
            set
            {
                _editor.OnSelectionColor = value;
                NotifyOfPropertyChange(() => OnSelectionColor);
            }
        }
        public string SelectedClassInteraction
        {
            get { return _editor.SelectedClassInteraction; }
            set
            {
                _editor.SelectedClassInteraction = value;
                NotifyOfPropertyChange(() => SelectedClassInteraction);
            }
        }
        public string SelectedShapeConnexion
        {
            get { return _editor.SelectedShapeConnexion; }
            set
            {
                _editor.SelectedShapeConnexion = value;
                NotifyOfPropertyChange(() => SelectedShapeConnexion);
            }
        }
        public string ConnexionThickness
        {
            get { return _editor.ConnexionThickness; }
            set
            {
                _editor.ConnexionThickness = value;
                NotifyOfPropertyChange(() => ConnexionThickness);
            }
        }
        public int ConnexionThicknessValue
        {
            get { return _editor.ConnexionThicknessValue; }
            set
            {
                _editor.ConnexionThicknessValue = value;
                NotifyOfPropertyChange(() => ConnexionThicknessValue);
            }
        }
        public bool IsTutorialOn
        {
            get { return _editor.IsTutorialOn; }
            set
            {
                _editor.IsTutorialOn = value;
                NotifyOfPropertyChange(() => _editor.IsTutorialOn);
            }
        }

        public double CanvasWidth
        {
            get { return _canvas.Width; }
            set
            {
                _canvas.Width = value;
                NotifyOfPropertyChange(() => CanvasWidth);
                MidCanvasX = Convert.ToInt32(CanvasWidth / 2);
            }
        }
        public double CanvasHeight
        {
            get { return _canvas.Height; }
            set
            {
                _canvas.Height = value;
                NotifyOfPropertyChange(() => CanvasHeight);
                MidCanvasY = Convert.ToInt32(CanvasHeight / 2);
            }
        }



        public bool IsDoneAddingCanvas { get => _isDoneAddingCanvas; set => _isDoneAddingCanvas = value; }
        #endregion
    }
}
