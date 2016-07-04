//-----------------------------------------------------------------------
// <copyright file="InkCanvasHelper.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.View.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Ink;
    using System.Windows.Input;
    using ViewModel;

    /// <summary>
    /// Provides attached properties to help with the <see cref="InkCanvas"/> control.
    /// </summary>
    public static class InkCanvasHelper
    {
        /// <summary>
        /// Attached property to allow binding to several <see cref="InkCanvas"/> properties with a single binding.
        /// </summary>
        public static readonly DependencyProperty PenDetailsProperty = DependencyProperty.RegisterAttached(
            "PenDetails",
            typeof(DoodlWindowModel),
            typeof(InkCanvasHelper),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, InkCanvasHelper.PenDetailsChanged));

        private static Dictionary<InkCanvas, DoodlWindowModel> canvasToModel = new Dictionary<InkCanvas, DoodlWindowModel>();
        private static Dictionary<DoodlWindowModel, InkCanvas> modelToCanvas = new Dictionary<DoodlWindowModel, InkCanvas>();

        /// <summary>
        /// Gets the value of the attached <see cref="PenDetailsProperty"/> property.
        /// </summary>
        /// <param name="target">The attached target to retrieve the property from.</param>
        /// <returns>The current value of the attached property.</returns>
        public static DoodlWindowModel GetPenDetails(InkCanvas target)
        {
            return (DoodlWindowModel)target.GetValue(PenDetailsProperty);
        }

        /// <summary>
        /// Sets the value of the attached <see cref="PenDetailsProperty"/> property.
        /// </summary>
        /// <param name="target">The attached target to set the property on.</param>
        /// <param name="value">The new value of the property.</param>
        public static void SetPenDetails(InkCanvas target, DoodlWindowModel value)
        {
            target.SetValue(PenDetailsProperty, value);
        }

        private static void PenDetailsChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var canvas = (InkCanvas)target;
            var oldModel = (DoodlWindowModel)e.OldValue;
            var newModel = (DoodlWindowModel)e.NewValue;

            if (oldModel != null)
            {
                oldModel.PropertyChanged -= ModelPropertyChanged;

                modelToCanvas.Remove(oldModel);
            }

            if (newModel != null)
            {
                modelToCanvas[newModel] = canvas;

                newModel.PropertyChanged += ModelPropertyChanged;

                if (!canvasToModel.ContainsKey(canvas))
                {
                    canvasToModel[canvas] = newModel;

                    canvas.StylusDown += StylusDown;
                    canvas.StylusUp += StylusUp;
                    canvas.PreviewMouseDown += StylusDown;
                    canvas.MouseUp += StylusUp;
                }
            }
            else
            {
                canvasToModel.Remove(canvas);

                canvas.StylusDown -= StylusDown;
                canvas.StylusUp -= StylusUp;
                canvas.PreviewMouseDown -= StylusDown;
                canvas.MouseUp -= StylusUp;
            }
        }

        private static void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var model = (DoodlWindowModel)sender;
            var canvas = modelToCanvas[model];

            UpdateCanvas(canvas, model);
        }

        private static void StylusDown(object sender, EventArgs e)
        {
            var canvas = (InkCanvas)sender;
            var model = canvasToModel[canvas];

            model.UndoManager.BeginOperation();
        }

        private static void StylusUp(object sender, EventArgs e)
        {
            var canvas = (InkCanvas)sender;
            var model = canvasToModel[canvas];

            model.UndoManager.EndOperation();
        }

        private static void UpdateCanvas(InkCanvas targetCanvas, DoodlWindowModel newModel)
        {
            if (targetCanvas == null)
            {
                return;
            }

            targetCanvas.EditingMode = InkCanvasEditingMode.Ink;

            switch (newModel.SelectedTool)
            {
                case CanvasTool.Eraser:
                    switch (newModel.EraserTip.Tip)
                    {
                        case StylusTip.Rectangle:
                            targetCanvas.EraserShape = new RectangleStylusShape(newModel.EraserTip.Width, newModel.EraserTip.Height);
                            break;
                        case StylusTip.Ellipse:
                            targetCanvas.EraserShape = new EllipseStylusShape(newModel.EraserTip.Width, newModel.EraserTip.Height);
                            break;
                    }

                    targetCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    break;
                case CanvasTool.StrokeEraser:
                    targetCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    break;
            }
        }
    }
}
