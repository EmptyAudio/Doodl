//-----------------------------------------------------------------------
// <copyright file="DoodlWindowModel.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Ink;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Model;
    using Properties;

    /// <summary>
    /// View model for the <see cref="DoodlWindow"/> view.
    /// </summary>
    public class DoodlWindowModel : INotifyPropertyChanged
    {
        private readonly IDialogService dialogService;
        private readonly IDoodlService doodlService;

        private StrokeCollection strokes;
        private Guid editID;
        private bool showUploadConfirmation;
        private bool showUploadProgress;
        private bool showUploadError;
        private string uploadError;
        private CanvasTool selectedTool = CanvasTool.Ink;
        private Color inkColor;
        private Color highlighterColor;
        private PenTip inkTip;
        private PenTip highlighterTip;
        private PenTip eraserTip;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoodlWindowModel"/> class.
        /// </summary>
        /// <param name="undoManager">The undo manager to use.</param>
        /// <param name="dialogService">The dialog service to use.</param>
        /// <param name="doodlService">The upload service to use.</param>
        public DoodlWindowModel(IUndoManager undoManager, IDialogService dialogService, IDoodlService doodlService)
        {
            this.UndoManager = undoManager;
            this.dialogService = dialogService;
            this.doodlService = doodlService;

            this.InkPenTips = (PenTipCollection)Application.Current.TryFindResource("DefaultInkPenTips") ?? new PenTipCollection();
            this.HighlighterPenTips = (PenTipCollection)Application.Current.TryFindResource("DefaultHighlighterPenTips") ?? new PenTipCollection();
            this.EraserPenTips = (PenTipCollection)Application.Current.TryFindResource("DefaultEraserPenTips") ?? new PenTipCollection();

            this.InkColors = (ColorCollection)Application.Current.TryFindResource("DefaultColors") ?? new ColorCollection();
            this.HighlighterColors = (ColorCollection)Application.Current.TryFindResource("DefaultColors") ?? new ColorCollection();

            this.inkTip = this.InkPenTips.FirstOrDefault() ?? new PenTip();
            this.highlighterTip = this.HighlighterPenTips.FirstOrDefault() ?? new PenTip();
            this.eraserTip = this.EraserPenTips.FirstOrDefault() ?? new PenTip();

            this.inkColor = this.InkColors.DefaultIfEmpty(Colors.Black).First();
            this.highlighterColor = this.HighlighterColors.Skip(3).DefaultIfEmpty(Colors.Yellow).First();

            this.UndoCommand = new DelegateCommand("Undo", _ => this.UndoManager.Undo(), _ => this.UndoManager.CanUndo);
            this.RedoCommand = new DelegateCommand("Redo", _ => this.UndoManager.Redo(), _ => this.UndoManager.CanRedo);
            this.NewCommand = new DelegateCommand("New", _ => { this.Strokes.Clear(); });

            this.OpenCommand = new DelegateCommand("Open", this.Open);
            this.SaveCommand = new DelegateCommand("Save", this.Save);
            this.SaveAsImageCommand = new DelegateCommand("Save as Image", this.SaveAsImage);

            this.StartUploadCommand = new DelegateCommand("Upload", _ => { this.ShowUploadConfirmation = true; });
            this.FinishUploadCommand = new DelegateCommand("Upload", this.Upload);
            this.DismissDialogCommand = new DelegateCommand("Cancel", _ => { this.ShowUploadConfirmation = this.ShowUploadError = false; });

            this.UndoManager.UndoStateChanged += (sender, e) =>
            {
                this.UndoCommand.RaiseCanExecuteChanged();
                this.RedoCommand.RaiseCanExecuteChanged();
            };
        }

        /// <summary>
        /// Raised when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the ID of the doodl of which this is an edit.
        /// </summary>
        public Guid EditID
        {
            get
            {
                return this.editID;
            }

            set
            {
                this.editID = value;

                this.OnPropertyChanged();
                this.OnPropertyChanged("IsEdit");
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this doodl is an edit.
        /// </summary>
        public bool IsEdit
        {
            get
            {
                return this.EditID != Guid.Empty;
            }
        }

        /// <summary>
        /// Gets the undo manager for this view model.
        /// </summary>
        public IUndoManager UndoManager { get; }

        /// <summary>
        /// Gets the undo command.
        /// </summary>
        public DelegateCommand UndoCommand { get; }

        /// <summary>
        /// Gets the redo command.
        /// </summary>
        public DelegateCommand RedoCommand { get; }

        /// <summary>
        /// Gets the new command.
        /// </summary>
        public DelegateCommand NewCommand { get; }

        /// <summary>
        /// Gets the open command.
        /// </summary>
        public DelegateCommand OpenCommand { get; }

        /// <summary>
        /// Gets the save command.
        /// </summary>
        public DelegateCommand SaveCommand { get; }

        /// <summary>
        /// Gets the save as image command.
        /// </summary>
        public DelegateCommand SaveAsImageCommand { get; }

        /// <summary>
        /// Gets the start upload command.
        /// </summary>
        public DelegateCommand StartUploadCommand { get; }

        /// <summary>
        /// Gets the finish upload command.
        /// </summary>
        public DelegateCommand FinishUploadCommand { get; }

        /// <summary>
        /// Gets the dismiss dialog command.
        /// </summary>
        public DelegateCommand DismissDialogCommand { get; }

        /// <summary>
        /// Gets or sets the name to use when uploading doodls.
        /// </summary>
        public string DoodlName
        {
            get
            {
                return Settings.Default.DoodlName;
            }

            set
            {
                Settings.Default.DoodlName = value;

                Settings.Default.Save();

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether newly uploaded doodls should be opened immediately.
        /// </summary>
        public bool OpenDoodlOnUpload
        {
            get
            {
                return Settings.Default.OpenDoodlOnUpload;
            }

            set
            {
                Settings.Default.OpenDoodlOnUpload = value;

                Settings.Default.Save();

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the URL of newly uploaded doodls should be copied to the clipboard.
        /// </summary>
        public bool CopyDoodlUrl
        {
            get
            {
                return Settings.Default.CopyDoodlUrl;
            }

            set
            {
                Settings.Default.CopyDoodlUrl = value;

                Settings.Default.Save();

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the strokes in the current doodl.
        /// </summary>
        public StrokeCollection Strokes
        {
            get
            {
                return this.strokes;
            }

            set
            {
                this.strokes = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the upload confirmation screen should be shown.
        /// </summary>
        public bool ShowUploadConfirmation
        {
            get
            {
                return this.showUploadConfirmation;
            }

            set
            {
                this.showUploadConfirmation = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the upload confirmation screen should be shown.
        /// </summary>
        public bool ShowUploadProgress
        {
            get
            {
                return this.showUploadProgress;
            }

            set
            {
                this.showUploadProgress = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the upload error screen should be shown.
        /// </summary>
        public bool ShowUploadError
        {
            get
            {
                return this.showUploadError;
            }

            set
            {
                this.showUploadError = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the upload error message.
        /// </summary>
        public string UploadError
        {
            get
            {
                return this.uploadError;
            }

            set
            {
                this.uploadError = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected tool.
        /// </summary>
        public CanvasTool SelectedTool
        {
            get
            {
                return this.selectedTool;
            }

            set
            {
                this.selectedTool = value;

                this.OnPropertyChanged();
                this.OnPropertyChanged("CurrentToolColor");
            }
        }

        /// <summary>
        /// Gets or sets the current inking color.
        /// </summary>
        public Color InkColor
        {
            get
            {
                return this.inkColor;
            }

            set
            {
                this.inkColor = value;

                this.OnPropertyChanged();

                if (this.SelectedTool == CanvasTool.Ink)
                {
                    this.OnPropertyChanged("CurrentToolColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected tool.
        /// </summary>
        public Color HighlighterColor
        {
            get
            {
                return this.highlighterColor;
            }

            set
            {
                this.highlighterColor = value;

                this.OnPropertyChanged();

                if (this.SelectedTool == CanvasTool.Highlighter)
                {
                    this.OnPropertyChanged("CurrentToolColor");
                }
            }
        }

        /// <summary>
        /// Gets the color of the currently selected tool.
        /// </summary>
        public Color CurrentToolColor
        {
            get
            {
                switch (this.SelectedTool)
                {
                    case CanvasTool.Ink:
                        return this.InkColor;
                    case CanvasTool.Highlighter:
                        return this.HighlighterColor;
                    default:
                        return Colors.Transparent;
                }
            }
        }
    
        /// <summary>
        /// Gets or sets the currently selected ink pen tip.
        /// </summary>
        public PenTip InkTip
        {
            get
            {
                return this.inkTip;
            }

            set
            {
                this.inkTip = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected highlighter pen tip.
        /// </summary>
        public PenTip HighlighterTip
        {
            get
            {
                return this.highlighterTip;
            }

            set
            {
                this.highlighterTip = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected eraser pen tip.
        /// </summary>
        public PenTip EraserTip
        {
            get
            {
                return this.eraserTip;
            }

            set
            {
                this.eraserTip = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the gallery of ink pen tips.
        /// </summary>
        public PenTipCollection InkPenTips { get; private set; }

        /// <summary>
        /// Gets the gallery of highlighter pen tips.
        /// </summary>
        public PenTipCollection HighlighterPenTips { get; private set; }

        /// <summary>
        /// Gets the gallery of eraser pen tips.
        /// </summary>
        public PenTipCollection EraserPenTips { get; private set; }

        /// <summary>
        /// Gets the gallery of ink colors.
        /// </summary>
        public ColorCollection InkColors { get; private set; }

        /// <summary>
        /// Gets the gallery of highlighter colors.
        /// </summary>
        public ColorCollection HighlighterColors { get; private set; }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Open(object parameter)
        {
            using (var stream = this.dialogService.OpenFile())
            {
                if (stream != null)
                {
                    this.Strokes.Clear();
                    this.Strokes.Add(new StrokeCollection(stream));

                    this.UndoManager.Clear();

                    this.EditID = Guid.Empty;
                }
            }
        }

        private void Save(object obj)
        {
            using (var stream = this.dialogService.SaveFile())
            {
                if (stream != null)
                {
                    this.Strokes.Save(stream);
                }
            }
        }

        private void SaveAsImage(object parameter)
        {
            using (var stream = this.dialogService.SaveFileAsImage())
            {
                if (stream != null)
                {
                    this.RenderWithScale(parameter as UIElement, 1.0d, stream);
                }
            }
        }

        private async void Upload(object parameter)
        {
            this.ShowUploadProgress = true;
            this.ShowUploadConfirmation = false;

            try
            {
                var canvas = (UIElement)parameter;

                var imageStream = this.RenderImage(canvas);
                var thumbnailStream = this.RenderThumbnail(canvas);

                var inkStream = new MemoryStream();

                this.Strokes.Save(inkStream);

                inkStream.Position = 0;

                string url;

                if (this.IsEdit)
                {
                    url = await this.doodlService.UploadEdit(this.EditID, this.DoodlName, imageStream, thumbnailStream, inkStream);
                }
                else
                {
                    url = await this.doodlService.Upload(this.DoodlName, imageStream, thumbnailStream, inkStream);
                }

                if (this.CopyDoodlUrl)
                {
                    Clipboard.SetText(url);
                }

                if (this.OpenDoodlOnUpload)
                {
                    Process.Start(url);
                }
            }
            catch (Exception ex)
            {
                this.UploadError = ex.ToString();
                this.ShowUploadError = true;
            }
            finally
            {
                this.ShowUploadProgress = false;
            }
        }

        private Stream RenderImage(UIElement canvas)
        {
            var stream = new MemoryStream();

            this.RenderWithScale(canvas, 1, stream);

            stream.Position = 0;

            return stream;
        }

        private Stream RenderThumbnail(UIElement canvas)
        {
            var stream = new MemoryStream();

            if (canvas.RenderSize.Width > canvas.RenderSize.Height)
            {
                this.RenderWithScale(canvas, 72.0d / canvas.RenderSize.Width, stream);
            }
            else
            {
                this.RenderWithScale(canvas, 72.0d / canvas.RenderSize.Height, stream);
            }

            stream.Position = 0;

            return stream;
        }

        private void RenderWithScale(UIElement canvas, double scale, Stream stream)
        {
            var actualHeight = canvas.RenderSize.Height;
            var actualWidth = canvas.RenderSize.Width;
            var renderHeight = actualHeight * scale;
            var renderWidth = actualWidth * scale;
            var renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
            var drawingVisual = new DrawingVisual();
            
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));

                drawingContext.DrawRectangle(
                    new VisualBrush(canvas),
                    null,
                    new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }

            renderTarget.Render(drawingVisual);

            var pngEncoder = new PngBitmapEncoder();

            pngEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            pngEncoder.Save(stream);
        }
    }
}
