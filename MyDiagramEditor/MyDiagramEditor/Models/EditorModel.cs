using Caliburn.Micro;
using System;

namespace MyDiagramEditor.Models
{
    /// <summary>
    /// Specifications that are unique to this user
    /// </summary>
    public class EditorModel : PropertyChangedBase
    {
        // The user that is using the application; default value = "Jon Snow"
        private string _userName = "Jon Snow";
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        // The selected tool; default value = "artefact"
        private string _selectedTool = "artefact";
        public string SelectedTool
        {
            get { return _selectedTool; }
            set
            {
                _selectedTool = value;
                NotifyOfPropertyChange(() => SelectedTool);
            }
        }

        // The selected color of the color picker; default value = "Black"
        private string _selectedColor = "Black";
        public string SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                _selectedColor = value;
                NotifyOfPropertyChange(() => SelectedTool);
            }
        }

        // The selected item connectors color; default value = "CadetBlue"
        private string _onSelectionColor = "CadetBlue";
        public string OnSelectionColor
        {
            get { return _onSelectionColor; }
            set
            {
                _onSelectionColor = value;
                NotifyOfPropertyChange(() => OnSelectionColor);
            }
        }

        // The selected interaction between classes
        private string _selectedClassInteraction = "unidirectionnalLink";
        public string SelectedClassInteraction
        {
            get { return _selectedClassInteraction; }
            set
            {
                _selectedClassInteraction = value;
                NotifyOfPropertyChange(() => SelectedClassInteraction);
            }
        }

        // The selected shape connexion between shapes
        private string _selectedShapeConnexion = "directionnalArrow";
        public string SelectedShapeConnexion
        {
            get { return _selectedShapeConnexion; }
            set
            {
                _selectedShapeConnexion = value;
                NotifyOfPropertyChange(() => SelectedShapeConnexion);
            }
        }

        // the thickness of a connexion
        private string _connexionThickness = "1 px";
        public string ConnexionThickness
        {
            get { return _connexionThickness; }
            set
            {
                _connexionThickness = value;
                NotifyOfPropertyChange(() => ConnexionThickness);
                ConnexionThicknessValue = Int32.Parse(ConnexionThickness.Remove(ConnexionThickness.IndexOf(" ")));
            }
        }
        private int _connexionThicknessValue = 1;
        public int ConnexionThicknessValue
        {
            get { return _connexionThicknessValue; }
            set
            {
                _connexionThicknessValue = value;
                NotifyOfPropertyChange(() => ConnexionThicknessValue);
            }
        }

        private bool _isTutorialOn = false;
        public bool IsTutorialOn
        {
            get { return _isTutorialOn; }
            set
            {
                _isTutorialOn = value;
                NotifyOfPropertyChange(() => IsTutorialOn);
            }
        }
    }

    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    };
}