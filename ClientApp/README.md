# ClientApp - Componentes del Cliente para AdvanceControlApi

Este directorio contiene los componentes del cliente para consumir la API de AdvanceControl, específicamente el módulo de Mantenimientos.

## Estructura

```
ClientApp/
├── index.js                          # Exportaciones principales
├── services/
│   └── MantenimientoService.js       # Servicio para consumir API de Mantenimientos
└── views/
    └── MantenimientoView.jsx         # Componente React para gestión de mantenimientos
```

## Instalación

Estos componentes requieren React 16.8+ para funcionar correctamente (para el uso de hooks).

```bash
npm install react react-dom
```

## Uso

### MantenimientoService

El servicio `MantenimientoService` proporciona métodos para interactuar con el API de Mantenimientos.

```javascript
import MantenimientoService from './ClientApp/services/MantenimientoService';

// Crear instancia del servicio
const mantenimientoService = new MantenimientoService('/api', authService);

// Obtener mantenimientos con filtros
const result = await mantenimientoService.getMantenimientos({
    identificador: 'EQ-001',  // Búsqueda parcial por identificador
    idCliente: 5              // Filtro por cliente (0 para todos)
});

if (result.success) {
    console.log('Mantenimientos:', result.data);
} else {
    console.error('Error:', result.error);
}

// Crear un nuevo mantenimiento
const createResult = await mantenimientoService.createMantenimiento({
    idTipoMantenimiento: 1,  // ID del tipo de mantenimiento
    idCliente: 5,             // ID del cliente
    idEquipo: 10,             // ID del equipo
    costo: 1500.00,           // Costo del mantenimiento
    nota: 'Mantenimiento preventivo'  // Nota opcional
});

if (createResult.success) {
    console.log('Mantenimiento creado:', createResult.data);
}

// Eliminar un mantenimiento
const deleteResult = await mantenimientoService.deleteMantenimiento(123);

if (deleteResult.success) {
    console.log('Mantenimiento eliminado');
}
```

### MantenimientoView (React)

El componente `MantenimientoView` proporciona una interfaz completa para gestionar mantenimientos.

```jsx
import React from 'react';
import { MantenimientoView } from './ClientApp';
// O: import MantenimientoView from './ClientApp/views/MantenimientoView';

function App() {
    // authService opcional para manejo de autenticación
    const authService = {
        getAccessToken: () => sessionStorage.getItem('accessToken'),
        refresh: async () => { /* ... */ }
    };

    return (
        <div className="app">
            <MantenimientoView authService={authService} />
        </div>
    );
}

export default App;
```

## Características del MantenimientoView

- **Búsqueda de Mantenimientos**: Filtra por identificador del equipo y/o ID de cliente
- **Crear Mantenimiento**: Formulario para crear nuevos registros de mantenimiento
- **Eliminar Mantenimiento**: Elimina registros con confirmación
- **Responsive**: Diseño adaptable a diferentes tamaños de pantalla
- **Mensajes de Estado**: Muestra mensajes de éxito y error

## API Endpoints Consumidos

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/Mantenimiento` | Obtiene mantenimientos con filtros opcionales |
| POST | `/api/Mantenimiento` | Crea un nuevo mantenimiento |
| DELETE | `/api/Mantenimiento` | Elimina un mantenimiento |

### Parámetros GET

- `identificador` (string, opcional): Búsqueda parcial por identificador del equipo
- `idCliente` (int, opcional): Filtro exacto por ID de cliente (0 para no filtrar)

### Parámetros POST

- `idTipoMantenimiento` (int, requerido): ID del tipo de mantenimiento
- `idCliente` (int, requerido): ID del cliente
- `idEquipo` (int, requerido): ID del equipo
- `costo` (double, requerido): Costo del mantenimiento
- `nota` (string, opcional): Nota asociada

### Parámetros DELETE

- `idMantenimiento` (int, requerido): ID del mantenimiento a eliminar

## Modelo de Datos - Mantenimiento

```typescript
interface Mantenimiento {
    idMantenimiento: number;
    tipoMantenimiento: string;
    nombreComercial: string;
    razonSocial: string;
    nota: string;
    identificador: string;
    costo: number;
    costoTotal: number;
}
```

## Autenticación

El servicio soporta autenticación mediante Bearer tokens. Si se proporciona un `authService`, intentará obtener el token de acceso y renovarlo automáticamente si expira (respuesta 401).

El token se busca en este orden:
1. Del `authService.getAccessToken()` si está disponible
2. Del `sessionStorage.getItem('accessToken')`

## Personalización de Estilos

Los estilos están incluidos como CSS-in-JS en el componente. Para personalizar:

1. Sobrescribe las clases CSS existentes en tu hoja de estilos
2. O modifica directamente el bloque `<style>` en `MantenimientoView.jsx`

Clases principales:
- `.mantenimiento-view` - Contenedor principal
- `.filtros-section` - Sección de filtros
- `.crear-section` - Formulario de creación
- `.tabla-section` - Tabla de mantenimientos
- `.btn-*` - Botones (primary, secondary, success, danger)
- `.alert-*` - Alertas (error, success)
