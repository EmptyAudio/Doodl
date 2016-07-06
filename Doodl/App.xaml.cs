//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Ink;
    using Microsoft.Win32;
    using Model;
    using View;
    using ViewModel;

    /// <summary>
    /// Provides lifecycle management for the application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the application, loading any arguments specified on the command line.
        /// </summary>
        /// <param name="e">The startup event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            this.RegisterFileAssociation();

            var dialogService = new WindowsDialogService();
            var strokes = new StrokeCollection();
            var undoManager = new StrokeUndoManager(strokes);
            var doodlService = new AzureDoodlService();
            var viewModel = new DoodlWindowModel(undoManager, dialogService, doodlService);

            viewModel.Strokes = strokes;

            if (e.Args.Length != 0)
            {
                if (e.Args[0].StartsWith("doodl:edit:"))
                {
                    Guid id;

                    if (Guid.TryParseExact(e.Args[0].Substring(11), "D", out id))
                    {
                        using (var stream = doodlService.GetStrokesFor(id).Result)
                        {
                            strokes.Add(new StrokeCollection(stream));
                        }

                        viewModel.EditID = id;
                    }
                }
                else if (File.Exists(e.Args[0]))
                {
                    using (var stream = File.OpenRead(e.Args[0]))
                    {
                        strokes.Add(new StrokeCollection(stream));
                    }
                }
            }

            var view = new DoodlWindow();

            view.DataContext = viewModel;

            this.MainWindow = view;

            this.MainWindow.Show();

            base.OnStartup(e);
        }

        private void RegisterFileAssociation()
        {
            var fullPath = Assembly.GetCallingAssembly().Location;

            using (var doodlType = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\.doodl"))
            {
                doodlType.SetValue(null, "EmptyAudio.Doodl");
                doodlType.SetValue("ContentType", "application/x-ms-ink");
            }

            using (var doodlProtocol = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\doodl"))
            {
                doodlProtocol.SetValue(null, "Doodl Edit Protocol");
                doodlProtocol.SetValue("URL Protocol", string.Empty);

                using (var doodlIcon = doodlProtocol.CreateSubKey("DefaultIcon"))
                {
                    doodlIcon.SetValue(null, "doodl.exe,1");
                }

                using (var doodlCommand = doodlProtocol.CreateSubKey(@"shell\open\command"))
                {
                    doodlCommand.SetValue(null, "Doodl.exe \"%1\"");
                }
            }

            using (var progid = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\EmptyAudio.Doodl"))
            {
                progid.SetValue("FriendlyTypeName", "Doodl");

                using (var command = progid.CreateSubKey(@"shell\open\command"))
                {
                    command.SetValue(null, "Doodl.exe \"%1\"");
                }
            }

            using (var appPath = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Doodl.exe"))
            {
                appPath.SetValue(null, fullPath);
                appPath.SetValue("SupportedProtocols", "doodl");
                appPath.SetValue("UseUrl", 1);
            }

            using (var application = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\Applications\Doodl.exe"))
            {
                using (var supportedTypes = application.CreateSubKey("SupportedTypes"))
                {
                    supportedTypes.SetValue(".doodl", string.Empty);
                }
            }
        }
    }
}
