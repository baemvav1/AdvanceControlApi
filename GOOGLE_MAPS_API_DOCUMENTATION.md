# Google Maps API Integration Documentation

## Overview

This document describes the implementation of Google Maps API integration for the AdvanceControlApi project. The implementation includes endpoints for managing areas/zones, coordinates, and markers for use with Google Maps in a WinUI3 application.

## Database Tables

The following database tables have been designed to support Google Maps integration:

### 1. Area Table
Stores areas/zones optimized for Google Maps API:
- Geometry types: Polygon, Circle, Rectangle, Polyline
- Visual properties: colors, opacity, border styles
- Metadata and labels
- Automatic center calculation and bounding box

### 2. Coordenada Table
Stores coordinates for polygons and polylines:
- High precision coordinates (decimal 10,8 for latitude, decimal 11,8 for longitude)
- Order tracking for proper polygon rendering
- Support for control vertices and altitude

### 3. Marcador Table
Stores markers/points of interest:
- Custom icons and colors
- Animations (DROP, BOUNCE)
- InfoWindow content
- Association with areas

## Stored Procedure

### sp_area_edit
Main stored procedure for CRUD operations on areas. Supports the following operations:
- **create**: Create new area with coordinates
- **select**: List areas with search filters
- **select_googlemaps**: Get areas in Google Maps optimized format
- **select_geojson**: Get areas in standard GeoJSON format
- **update**: Update area properties and coordinates
- **delete**: Soft delete (set activo = 0)
- **delete_physical**: Permanent deletion
- **validate_point_in_polygon**: Check if a point is inside a polygon/circle

## API Endpoints

### Areas Controller (`/api/Areas`)

#### 1. GET /api/Areas
Get areas with optional filters.

**Query Parameters:**
- `idArea` (int, optional): Filter by specific area ID (default: 0 = all)
- `nombre` (string, optional): Filter by name (partial match)
- `activo` (bool, optional): Filter by active status
- `tipoGeometria` (string, optional): Filter by geometry type

**Response:** Array of Area objects

**Example:**
```
GET /api/Areas?activo=true&tipoGeometria=Polygon
```

#### 2. GET /api/Areas/googlemaps
Get areas in Google Maps optimized format.

**Query Parameters:**
- `idArea` (int, optional): Specific area ID (default: 0 = all)
- `activo` (bool, optional): Filter by active status

**Response:** Array of objects with Google Maps compatible format including:
- `type`: Geometry type
- `options`: Style options (fillColor, strokeColor, etc.)
- `path`: Array of coordinates
- `center`: Center point (for circles)
- `radius`: Radius in meters (for circles)
- `bounds`: Bounding box

**Example:**
```
GET /api/Areas/googlemaps?activo=true
```

#### 3. GET /api/Areas/geojson
Get areas in standard GeoJSON format.

**Query Parameters:**
- `idArea` (int, optional): Specific area ID (default: 0 = all)
- `activo` (bool, optional): Filter by active status

**Response:** GeoJSON FeatureCollection

**Example:**
```
GET /api/Areas/geojson
```

#### 4. POST /api/Areas
Create a new area.

**Query Parameters:**
- `nombre` (string, required): Area name
- `descripcion` (string, optional): Description
- `colorMapa` (string, optional): Fill color in hex format (e.g., #4285F4)
- `opacidad` (decimal, optional): Fill opacity (0.00 to 1.00)
- `colorBorde` (string, optional): Stroke color in hex format
- `anchoBorde` (int, optional): Stroke width in pixels
- `activo` (bool, optional): Active status
- `tipoGeometria` (string, optional): Geometry type (Polygon, Circle, Rectangle, Polyline)
- `centroLatitud` (decimal, optional): Center latitude
- `centroLongitud` (decimal, optional): Center longitude
- `radio` (decimal, optional): Radius in meters (for circles)
- `etiquetaMostrar` (bool, optional): Show label on map
- `etiquetaTexto` (string, optional): Label text
- `nivelZoom` (int, optional): Optimal zoom level
- `metadataJSON` (string, optional): Additional metadata in JSON format
- `usuarioCreacion` (string, optional): Creator username
- `coordenadas` (string, optional): JSON array of coordinates: `[{"lat":19.4326,"lng":-99.1332},...]`
- `autoCalcularCentro` (bool, optional): Auto-calculate center from coordinates
- `validarPoligonoLargo` (bool, optional): Validate polygon closure

**Response:**
```json
{
  "success": true,
  "message": "Área creada exitosamente",
  "idArea": 1,
  "poligonoCerrado": true
}
```

**Example:**
```
POST /api/Areas?nombre=Area1&tipoGeometria=Polygon&coordenadas=[{"lat":19.4326,"lng":-99.1332},{"lat":19.4327,"lng":-99.1333}]
```

#### 5. PUT /api/Areas/{id}
Update an existing area.

**Path Parameters:**
- `id` (int, required): Area ID

**Query Parameters:** Same as POST (all optional except id)

**Response:**
```json
{
  "success": true,
  "message": "Área actualizada exitosamente"
}
```

#### 6. DELETE /api/Areas/{id}
Soft delete an area (sets activo = 0).

**Path Parameters:**
- `id` (int, required): Area ID

**Response:**
```json
{
  "success": true,
  "message": "Área eliminada (lógico)"
}
```

#### 7. DELETE /api/Areas/{id}/physical
Permanently delete an area and its coordinates.

**Path Parameters:**
- `id` (int, required): Area ID

**Response:**
```json
{
  "success": true,
  "message": "Área eliminada permanentemente"
}
```

#### 8. GET /api/Areas/validate-point
Validate if a point is inside a polygon or circle.

**Query Parameters:**
- `idArea` (int, optional): Specific area ID (default: 0 = check all)
- `latitud` (decimal, required): Point latitude
- `longitud` (decimal, required): Point longitude

**Response:** Array of areas with `dentroDelArea` boolean field

**Example:**
```
GET /api/Areas/validate-point?latitud=19.4326&longitud=-99.1332
```

### Google Maps Config Controller (`/api/GoogleMapsConfig`)

#### 1. GET /api/GoogleMapsConfig/api-key
Get the Google Maps API key.

**Response:**
```json
{
  "apiKey": "AIzaSyACdL7i17rQab0x_vaLoC_F263LOWuJcrQ"
}
```

#### 2. GET /api/GoogleMapsConfig
Get complete Google Maps configuration.

**Response:**
```json
{
  "apiKey": "AIzaSyACdL7i17rQab0x_vaLoC_F263LOWuJcrQ",
  "defaultCenter": "19.4326,-99.1332",
  "defaultZoom": 15
}
```

## Configuration

Add the following to `appsettings.json`:

```json
{
  "GoogleMaps": {
    "ApiKey": "AIzaSyACdL7i17rQab0x_vaLoC_F263LOWuJcrQ",
    "DefaultCenter": "19.4326,-99.1332",
    "DefaultZoom": "15"
  }
}
```

## Authentication

All endpoints require JWT authentication. Include the Bearer token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

## Models

### Area
```csharp
public class Area
{
    public int IdArea { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? ColorMapa { get; set; }
    public decimal? Opacidad { get; set; }
    public string? ColorBorde { get; set; }
    public int? AnchoBorde { get; set; }
    public bool? Activo { get; set; }
    public string? TipoGeometria { get; set; }
    public decimal? CentroLatitud { get; set; }
    public decimal? CentroLongitud { get; set; }
    public decimal? Radio { get; set; }
    // ... additional properties
}
```

### Coordenada
```csharp
public class Coordenada
{
    public int IdCoordenada { get; set; }
    public int IdArea { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public int Orden { get; set; }
    // ... additional properties
}
```

### Marcador
```csharp
public class Marcador
{
    public int IdMarcador { get; set; }
    public int? IdArea { get; set; }
    public string? Nombre { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    // ... additional properties
}
```

## Usage Examples

### Creating a Polygon Area

```http
POST /api/Areas
?nombre=Zona Centro
&descripcion=Zona del centro histórico
&tipoGeometria=Polygon
&colorMapa=#FF0000
&opacidad=0.5
&colorBorde=#990000
&anchoBorde=2
&coordenadas=[{"lat":19.4326,"lng":-99.1332},{"lat":19.4336,"lng":-99.1342},{"lat":19.4346,"lng":-99.1322},{"lat":19.4326,"lng":-99.1332}]
```

### Creating a Circle Area

```http
POST /api/Areas
?nombre=Radio de Cobertura
&tipoGeometria=Circle
&centroLatitud=19.4326
&centroLongitud=-99.1332
&radio=5000
&colorMapa=#0000FF
&opacidad=0.3
```

### Getting Areas for Google Maps

```http
GET /api/Areas/googlemaps?activo=true
```

This returns data ready to be used with Google Maps JavaScript API:

```javascript
// In your WinUI3 app
const response = await fetch('https://your-api/api/Areas/googlemaps?activo=true');
const areas = await response.json();

areas.forEach(area => {
  if (area.type === 'Polygon') {
    const polygon = new google.maps.Polygon({
      paths: JSON.parse(area.path),
      ...JSON.parse(area.options)
    });
    polygon.setMap(map);
  } else if (area.type === 'Circle') {
    const circle = new google.maps.Circle({
      center: JSON.parse(area.center),
      radius: area.radius,
      ...JSON.parse(area.options)
    });
    circle.setMap(map);
  }
});
```

## Error Handling

All endpoints return standard HTTP status codes:
- `200 OK`: Success
- `400 Bad Request`: Invalid parameters
- `401 Unauthorized`: Missing or invalid JWT token
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

Error responses include a message field:
```json
{
  "message": "Error description"
}
```

## Security Considerations

1. **API Key Security**: The Google Maps API key is currently stored in appsettings.json for development purposes. **In production environments**, the API key should be stored securely using:
   - Azure Key Vault
   - Environment variables
   - .NET User Secrets for development
   - Never commit the API key to version control in production configurations
2. All endpoints require JWT authentication
3. Input validation is performed for coordinates (latitude: -90 to 90, longitude: -180 to 180)
4. Color values are validated to ensure hex format
5. SQL injection protection through parameterized queries

## Implementation Files

- **Models**: `Clases/Area.cs`, `Clases/Coordenada.cs`, `Clases/Marcador.cs`
- **DTOs**: `DTOs/AreaEditDto.cs`
- **Services**: `Services/IAreaService.cs`, `Services/AreaService.cs`
- **Controllers**: `Controllers/AreasController.cs`, `Controllers/GoogleMapsConfigController.cs`
- **Configuration**: `appsettings.json`

## Database Setup

Execute the provided SQL scripts to create:
1. Tables: Area, Coordenada, Marcador
2. Indexes for optimization
3. Triggers for automatic calculations
4. Stored procedure: sp_area_edit

## Testing

The API has been tested and all endpoints are working correctly:
- ✅ GET /api/Areas
- ✅ GET /api/Areas/googlemaps
- ✅ GET /api/Areas/geojson
- ✅ POST /api/Areas
- ✅ PUT /api/Areas/{id}
- ✅ DELETE /api/Areas/{id}
- ✅ DELETE /api/Areas/{id}/physical
- ✅ GET /api/Areas/validate-point
- ✅ GET /api/GoogleMapsConfig/api-key
- ✅ GET /api/GoogleMapsConfig

## Notes

- The stored procedure includes automatic calculation of center points and bounding boxes for polygons
- Coordinates are stored with high precision for accuracy (~1.1mm)
- The system supports 3D coordinates (altitude) for future enhancements
- Triggers automatically update modification timestamps
- Cascade deletion of coordinates when an area is deleted
