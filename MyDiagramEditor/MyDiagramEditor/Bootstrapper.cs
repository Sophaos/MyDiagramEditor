using Caliburn.Micro;
using System.Windows;
using MyDiagramEditor.ViewModels;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace MyDiagramEditor
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.PerRequest<IShell, ShellViewModel>();
            MessageBinder.SpecialValues.Add("$pressedkey", (context) =>
            {
                // NOTE: IMPORTANT - you MUST add the dictionary key as lowercase as CM
                // does a ToLower on the param string you add in the action message, in fact ideally
                // all your param messages should be lowercase just in case. I don't really like this
                // behaviour but that's how it is!
                var keyArgs = context.EventArgs as KeyEventArgs;

                if (keyArgs != null)
                    return keyArgs.Key;

                return null;
            });
            MessageBinder.SpecialValues.Add("$mousepoint", ctx =>
            {
                var e = ctx.EventArgs as MouseEventArgs;
                if (e == null)
                    return null;

                return e.GetPosition(ctx.Source);
            });
            MessageBinder.SpecialValues.Add("$horizontalchange", ctx =>
            {
                var e = ctx.EventArgs as DragDeltaEventArgs;
                if (e == null)
                    return null;

                return Convert.ToInt32(Math.Floor(e.HorizontalChange));
            });
            MessageBinder.SpecialValues.Add("$verticalchange", ctx =>
            {
                var e = ctx.EventArgs as DragDeltaEventArgs;
                if (e == null)
                    return null;

                return Convert.ToInt32(Math.Floor(e.VerticalChange));
            });
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var instance = _container.GetInstance(serviceType, key);
            if (instance != null)
                return _container.GetInstance(serviceType, key);

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //DisplayRootViewFor<ShellViewModel>();
            DisplayRootViewFor<IShell>();
        }
    }
}
