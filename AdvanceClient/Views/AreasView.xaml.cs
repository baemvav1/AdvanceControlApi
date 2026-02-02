using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvanceClient.Services;

namespace AdvanceClient.Views
{
    /// <summary>
    /// Vista para administrar áreas/zonas en el mapa de Google Maps
    /// </summary>
    public sealed partial class AreasView : Page
    {
        private readonly ILoggingService _loggingService;
        private readonly IAreaApiService _areaApiService;
        
        // Shape state management
        private string? _currentShapeType;
        private List<Coordinate>? _currentCoordinates;
        private bool _isEditMode;
        private int? _editingAreaId;
        
        // Drawing state
        private bool _isDrawing;
        private string? _pendingShapeType;

        /// <summary>
        /// Constructor with dependency injection support
        /// </summary>
        /// <param name="loggingService">Optional logging service (defaults to LoggingService if not provided)</param>
        /// <param name="areaApiService">Optional area API service (defaults to AreaApiService if not provided)</param>
        public AreasView(ILoggingService? loggingService = null, IAreaApiService? areaApiService = null)
        {
            this.InitializeComponent();
            
            // Use injected services or create default implementations
            _loggingService = loggingService ?? new LoggingService();
            _areaApiService = areaApiService ?? AreaApiService.Instance;
            
            _currentCoordinates = new List<Coordinate>();
            _isEditMode = false;
        }

        #region Shape Drawing Methods
        
        /// <summary>
        /// Initiates polygon drawing mode on the map
        /// </summary>
        private void DrawPolygon_Click(object sender, RoutedEventArgs e)
        {
            StartDrawingMode("Polygon");
        }

        /// <summary>
        /// Initiates rectangle drawing mode on the map
        /// </summary>
        private void DrawRectangle_Click(object sender, RoutedEventArgs e)
        {
            StartDrawingMode("Rectangle");
        }

        /// <summary>
        /// Initiates circle drawing mode on the map
        /// </summary>
        private void DrawCircle_Click(object sender, RoutedEventArgs e)
        {
            StartDrawingMode("Circle");
        }

        /// <summary>
        /// Initiates polyline drawing mode on the map
        /// </summary>
        private void DrawPolyline_Click(object sender, RoutedEventArgs e)
        {
            StartDrawingMode("Polyline");
        }

        /// <summary>
        /// Starts the drawing mode for the specified shape type
        /// This sets the _pendingShapeType which will be confirmed when the shape is completed
        /// </summary>
        private void StartDrawingMode(string shapeType)
        {
            _isDrawing = true;
            _pendingShapeType = shapeType;
            
            // Update UI to show which shape is being drawn
            UpdateDrawingUI(shapeType);
            
            // TODO: Send message to WebView2 to enable drawing mode for this shape type
            // Example: await _mapWebView.ExecuteScriptAsync($"startDrawingMode('{shapeType}')");
            
            _loggingService.LogInfoAsync($"Drawing mode started for shape type: {shapeType}", "AreasView", "StartDrawingMode");
        }

        /// <summary>
        /// Called when a shape has been completed on the map.
        /// This is the critical method that sets _currentShapeType when a shape is drawn.
        /// </summary>
        /// <param name="shapeType">The type of shape that was drawn (Polygon, Rectangle, Circle, Polyline)</param>
        /// <param name="coordinates">The coordinates of the shape</param>
        public void OnShapeDrawnComplete(string shapeType, List<Coordinate> coordinates)
        {
            if (string.IsNullOrEmpty(shapeType))
            {
                _loggingService.LogErrorAsync(
                    "OnShapeDrawnComplete called with null or empty shapeType",
                    null,
                    "AreasView",
                    "OnShapeDrawnComplete");
                return;
            }

            if (coordinates == null || coordinates.Count == 0)
            {
                _loggingService.LogErrorAsync(
                    "OnShapeDrawnComplete called with null or empty coordinates",
                    null,
                    "AreasView",
                    "OnShapeDrawnComplete");
                return;
            }

            // FIX: Set _currentShapeType when a shape is drawn on the map
            // This is the critical line that was missing/not being called properly
            _currentShapeType = shapeType;
            _currentCoordinates = new List<Coordinate>(coordinates);
            _isDrawing = false;
            _pendingShapeType = null;

            // Update UI to reflect the completed shape
            UpdateShapeTypeDisplay(shapeType);
            
            _loggingService.LogInfoAsync(
                $"Shape drawing completed. Type: {shapeType}, Coordinates: {coordinates.Count}",
                "AreasView",
                "OnShapeDrawnComplete");
        }

        /// <summary>
        /// Called when a rectangle is drawn on the map.
        /// Rectangles use bounding box coordinates (NE and SW corners).
        /// </summary>
        /// <param name="northEast">North-East corner coordinate</param>
        /// <param name="southWest">South-West corner coordinate</param>
        public void OnRectangleDrawn(Coordinate northEast, Coordinate southWest)
        {
            if (northEast == null || southWest == null)
            {
                _loggingService.LogErrorAsync(
                    "OnRectangleDrawn called with null coordinates",
                    null,
                    "AreasView",
                    "OnRectangleDrawn");
                return;
            }

            // FIX: Explicitly set _currentShapeType for rectangles
            _currentShapeType = "Rectangle";
            
            // Convert rectangle bounds to coordinate list
            _currentCoordinates = new List<Coordinate>
            {
                new Coordinate { Lat = northEast.Lat, Lng = southWest.Lng }, // NW
                new Coordinate { Lat = northEast.Lat, Lng = northEast.Lng }, // NE
                new Coordinate { Lat = southWest.Lat, Lng = northEast.Lng }, // SE
                new Coordinate { Lat = southWest.Lat, Lng = southWest.Lng }, // SW
                new Coordinate { Lat = northEast.Lat, Lng = southWest.Lng }  // Close polygon (NW)
            };

            _isDrawing = false;
            _pendingShapeType = null;

            // Update UI
            UpdateShapeTypeDisplay("Rectangle");

            _loggingService.LogInfoAsync(
                $"Rectangle drawn. NE: ({northEast.Lat}, {northEast.Lng}), SW: ({southWest.Lat}, {southWest.Lng})",
                "AreasView",
                "OnRectangleDrawn");
        }

        /// <summary>
        /// Clears the current shape from the map
        /// </summary>
        private void ClearShape_Click(object sender, RoutedEventArgs e)
        {
            ClearCurrentShape();
        }

        /// <summary>
        /// Resets all shape-related state
        /// </summary>
        private void ClearCurrentShape()
        {
            _currentShapeType = null;
            _currentCoordinates = new List<Coordinate>();
            _isDrawing = false;
            _pendingShapeType = null;

            // Update UI
            UpdateShapeTypeDisplay(null);

            // TODO: Send message to WebView2 to clear shapes
            // Example: await _mapWebView.ExecuteScriptAsync("clearAllShapes()");

            _loggingService.LogInfoAsync("Shape cleared", "AreasView", "ClearCurrentShape");
        }

        #endregion

        #region Save and Cancel Methods

        /// <summary>
        /// Handles the Save button click event.
        /// Validates that a shape has been drawn before allowing save.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(NombreTextBox.Text))
                {
                    await ShowValidationDialog("El nombre del área es requerido.");
                    return;
                }

                // FIX: Improved validation with clear error message and logging
                // Check if shape type is set (this is the issue mentioned in the problem statement)
                if (!_isEditMode && string.IsNullOrEmpty(_currentShapeType))
                {
                    await _loggingService.LogErrorAsync(
                        "Validation failed: _currentShapeType is null or empty when creating a new area.",
                        null,
                        "AreasView",
                        "SaveButton_Click");
                    
                    await ShowValidationDialog("Debe dibujar un área en el mapa antes de guardar.");
                    return;
                }

                // Validate coordinates exist
                if (!_isEditMode && (_currentCoordinates == null || _currentCoordinates.Count == 0))
                {
                    await _loggingService.LogErrorAsync(
                        "Validation failed: No coordinates available for the shape.",
                        null,
                        "AreasView",
                        "SaveButton_Click");
                    
                    await ShowValidationDialog("El área dibujada no tiene coordenadas válidas.");
                    return;
                }

                // Proceed with save
                SaveButton.IsEnabled = false;
                
                var areaData = new AreaSaveData
                {
                    Nombre = NombreTextBox.Text,
                    Descripcion = DescripcionTextBox.Text,
                    TipoGeometria = _currentShapeType,
                    Activo = ActivoToggle.IsOn,
                    Coordenadas = _currentCoordinates
                };

                bool success;
                if (_isEditMode && _editingAreaId.HasValue)
                {
                    areaData.IdArea = _editingAreaId.Value;
                    success = await _areaApiService.UpdateAreaAsync(areaData);
                }
                else
                {
                    success = await _areaApiService.CreateAreaAsync(areaData);
                }

                if (success)
                {
                    await ShowSuccessDialog(_isEditMode ? "Área actualizada exitosamente." : "Área creada exitosamente.");
                    ClearForm();
                }
                else
                {
                    await ShowErrorDialog("Error al guardar el área. Por favor, intente nuevamente.");
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    $"Exception in SaveButton_Click: {ex.Message}",
                    ex,
                    "AreasView",
                    "SaveButton_Click");
                
                await ShowErrorDialog($"Error inesperado: {ex.Message}");
            }
            finally
            {
                SaveButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Handles the Cancel button click event
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        /// <summary>
        /// Clears all form fields and resets state
        /// </summary>
        private void ClearForm()
        {
            NombreTextBox.Text = string.Empty;
            DescripcionTextBox.Text = string.Empty;
            ActivoToggle.IsOn = true;
            
            _isEditMode = false;
            _editingAreaId = null;
            
            ClearCurrentShape();
        }

        #endregion

        #region Edit Mode Methods

        /// <summary>
        /// Loads an existing area for editing
        /// </summary>
        /// <param name="area">The area data to edit</param>
        public void LoadAreaForEdit(AreaData area)
        {
            if (area == null)
            {
                _loggingService.LogErrorAsync("LoadAreaForEdit called with null area", null, "AreasView", "LoadAreaForEdit");
                return;
            }

            _isEditMode = true;
            _editingAreaId = area.IdArea;
            
            NombreTextBox.Text = area.Nombre ?? string.Empty;
            DescripcionTextBox.Text = area.Descripcion ?? string.Empty;
            ActivoToggle.IsOn = area.Activo ?? true;
            
            // FIX: Set _currentShapeType when loading an area for edit
            _currentShapeType = area.TipoGeometria;
            _currentCoordinates = area.Coordenadas != null 
                ? new List<Coordinate>(area.Coordenadas) 
                : new List<Coordinate>();

            UpdateShapeTypeDisplay(_currentShapeType);

            _loggingService.LogInfoAsync(
                $"Area loaded for edit. ID: {area.IdArea}, Type: {area.TipoGeometria}",
                "AreasView",
                "LoadAreaForEdit");
        }

        #endregion

        #region UI Helper Methods

        private void UpdateDrawingUI(string shapeType)
        {
            TipoGeometriaTextBlock.Text = $"Dibujando: {GetShapeDisplayName(shapeType)}...";
            TipoGeometriaTextBlock.FontStyle = Windows.UI.Text.FontStyle.Italic;
        }

        private void UpdateShapeTypeDisplay(string? shapeType)
        {
            if (string.IsNullOrEmpty(shapeType))
            {
                TipoGeometriaTextBlock.Text = "(Dibuje en el mapa)";
                TipoGeometriaTextBlock.FontStyle = Windows.UI.Text.FontStyle.Italic;
            }
            else
            {
                TipoGeometriaTextBlock.Text = GetShapeDisplayName(shapeType);
                TipoGeometriaTextBlock.FontStyle = Windows.UI.Text.FontStyle.Normal;
            }
        }

        private static string GetShapeDisplayName(string shapeType)
        {
            return shapeType switch
            {
                "Polygon" => "Polígono",
                "Rectangle" => "Rectángulo",
                "Circle" => "Círculo",
                "Polyline" => "Línea",
                _ => shapeType ?? "Desconocido"
            };
        }

        private async Task ShowValidationDialog(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Validación",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async Task ShowSuccessDialog(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Éxito",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async Task ShowErrorDialog(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        #endregion
    }

    #region Data Models

    /// <summary>
    /// Represents a coordinate point
    /// </summary>
    public class Coordinate
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

    /// <summary>
    /// Data for saving an area
    /// </summary>
    public class AreaSaveData
    {
        public int? IdArea { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? TipoGeometria { get; set; }
        public bool Activo { get; set; }
        public List<Coordinate>? Coordenadas { get; set; }
    }

    /// <summary>
    /// Represents an area from the API
    /// </summary>
    public class AreaData
    {
        public int IdArea { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? TipoGeometria { get; set; }
        public bool? Activo { get; set; }
        public List<Coordinate>? Coordenadas { get; set; }
    }

    #endregion
}
