using Caliburn.Micro;
using MyDiagramEditor.ViewModels;
using System;
using System.Xml.Linq;
using System.IO;

namespace MyDiagramEditor.ViewModels
{

    public class ShellViewModel : Conductor<object>, IShell
    {
        private EditorViewModel _editor;
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;

        public ShellViewModel(IWindowManager windowManager, IEventAggregator eventAggregator)
        {

            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            LoadEditor();
        }

        public void LoadEditor()
        {
            if (_editor == null)
            {
                _editor = new EditorViewModel(_eventAggregator, _windowManager);
            }
            ActivateItem(Editor);

            if (!File.Exists("FirstUse.xml"))
            {
                //Populate with data here if necessary, then save to make sure it exists
                new XDocument(new XElement("root",
                    new XElement("someNode", "someValue"))).Save("FirstUse.xml");
                Editor.LoadTutorial();
                Editor.IsTutorialOn = true;
            }
        }


        public EditorViewModel Editor
        {
            get { return _editor; }
            set { _editor = value; }
        }
    }
}
