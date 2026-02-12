# Endpoint: impuesto_comision

Este endpoint proporciona funcionalidad para gestionar impuestos de movimientos y comisiones bancarias.

## Base URL
`/api/impuesto_comision`

## Autenticación
Todos los endpoints requieren autenticación JWT mediante el header `Authorization: Bearer {token}`

---

## Endpoints para Impuestos de Movimiento

### 1. Consultar Impuestos de Movimiento
**GET** `/api/impuesto_comision/impuestos`

Consulta impuestos de movimiento según los criterios especificados.

**Parámetros de Query (opcionales):**
- `idMovimiento` (int): ID del movimiento
- `tipoImpuesto` (string): Tipo de impuesto

**Ejemplo de Request:**
```
GET /api/impuesto_comision/impuestos?idMovimiento=123&tipoImpuesto=ISR
Authorization: Bearer {token}
```

**Respuesta Exitosa (200):**
```json
[
  {
    "idImpuesto": 1,
    "idMovimiento": 123,
    "tipoImpuesto": "ISR",
    "rfc": "XAXX010101000",
    "monto": 150.50,
    "fecha": "2024-01-15T10:30:00",
    "descripcion": "Pago mensual"
  }
]
```

---

### 2. Crear Impuesto de Movimiento
**POST** `/api/impuesto_comision/impuestos`

Crea un nuevo impuesto asociado a un movimiento.

**Request Body (JSON):**
```json
{
  "idMovimiento": 123,
  "tipoImpuesto": "ISR",
  "monto": 150.50,
  "rfc": "XAXX010101000"
}
```

**Campos Obligatorios:**
- `idMovimiento` (int): ID del movimiento (debe ser mayor a 0)
- `tipoImpuesto` (string): Tipo de impuesto
- `monto` (decimal): Monto del impuesto (debe ser mayor a 0)

**Campos Opcionales:**
- `rfc` (string): RFC del contribuyente

**Ejemplo de Request:**
```
POST /api/impuesto_comision/impuestos
Authorization: Bearer {token}
Content-Type: application/json

{
  "idMovimiento": 123,
  "tipoImpuesto": "ISR",
  "monto": 150.50,
  "rfc": "XAXX010101000"
}
```

**Respuesta Exitosa (200):**
```json
{
  "success": true,
  "idImpuesto": 1,
  "message": "Impuesto creado exitosamente"
}
```

**Respuestas de Error:**
- **400 Bad Request:** Campos obligatorios faltantes o inválidos
- **500 Internal Server Error:** Error al acceder a la base de datos

---

## Endpoints para Comisiones Bancarias

### 3. Consultar Comisiones Bancarias
**GET** `/api/impuesto_comision/comisiones`

Consulta comisiones bancarias según los criterios especificados.

**Parámetros de Query (opcionales):**
- `idMovimiento` (int): ID del movimiento
- `tipoComision` (string): Tipo de comisión
- `fechaInicio` (DateTime): Fecha inicio del período (formato: YYYY-MM-DD)
- `fechaFin` (DateTime): Fecha fin del período (formato: YYYY-MM-DD)

**Ejemplo de Request:**
```
GET /api/impuesto_comision/comisiones?tipoComision=Transferencia&fechaInicio=2024-01-01&fechaFin=2024-12-31
Authorization: Bearer {token}
```

**Respuesta Exitosa (200):**
```json
[
  {
    "idComision": 1,
    "idMovimiento": 123,
    "tipoComision": "Transferencia",
    "monto": 25.00,
    "iva": 4.00,
    "referencia": "REF-12345",
    "fecha": "2024-01-15T10:30:00",
    "descripcion": "Transferencia SPEI"
  }
]
```

---

### 4. Crear Comisión Bancaria
**POST** `/api/impuesto_comision/comisiones`

Crea una nueva comisión bancaria asociada a un movimiento.

**Request Body (JSON):**
```json
{
  "idMovimiento": 123,
  "tipoComision": "Transferencia",
  "monto": 25.00,
  "iva": 4.00,
  "referencia": "REF-12345"
}
```

**Campos Obligatorios:**
- `idMovimiento` (int): ID del movimiento (debe ser mayor a 0)
- `tipoComision` (string): Tipo de comisión
- `monto` (decimal): Monto de la comisión (debe ser mayor a 0)

**Campos Opcionales:**
- `iva` (decimal): Monto del IVA
- `referencia` (string): Referencia de la comisión

**Ejemplo de Request:**
```
POST /api/impuesto_comision/comisiones
Authorization: Bearer {token}
Content-Type: application/json

{
  "idMovimiento": 123,
  "tipoComision": "Transferencia",
  "monto": 25.00,
  "iva": 4.00,
  "referencia": "REF-12345"
}
```

**Respuesta Exitosa (200):**
```json
{
  "success": true,
  "idComision": 1,
  "message": "Comisión bancaria creada exitosamente"
}
```

**Respuestas de Error:**
- **400 Bad Request:** Campos obligatorios faltantes o inválidos
- **500 Internal Server Error:** Error al acceder a la base de datos

---

## Procedimientos Almacenados Utilizados

El endpoint utiliza los siguientes procedimientos almacenados:

### Para Impuestos de Movimiento:
- `sp_CrearImpuestoMovimiento`: Crea un nuevo impuesto de movimiento
- `sp_ConsultarImpuestosMovimiento`: Consulta impuestos de movimiento

### Para Comisiones Bancarias:
- `sp_CrearComisionBancaria`: Crea una nueva comisión bancaria
- `sp_ConsultarComisionesBancarias`: Consulta comisiones bancarias

---

## Notas Importantes

1. **Validación del Movimiento:** Los procedimientos almacenados validan que el movimiento exista antes de crear un impuesto o comisión.
2. **Manejo de Errores:** Todos los errores SQL se capturan y se devuelven como errores 500 con mensajes apropiados.
3. **Formato de Fechas:** Las fechas deben enviarse en formato ISO 8601 (YYYY-MM-DD o YYYY-MM-DDTHH:mm:ss).
4. **Autenticación:** Es necesario proporcionar un token JWT válido en todas las peticiones.
5. **Parámetros Opcionales:** Los parámetros opcionales pueden omitirse o enviarse como `null`.

---

## Estructura de Modelos

### ImpuestoMovimiento
```csharp
{
  "idImpuesto": int,
  "idMovimiento": int,
  "tipoImpuesto": string (nullable),
  "rfc": string (nullable),
  "monto": decimal,
  "fecha": DateTime (nullable),
  "descripcion": string (nullable)
}
```

### ComisionBancaria
```csharp
{
  "idComision": int,
  "idMovimiento": int,
  "tipoComision": string (nullable),
  "monto": decimal,
  "iva": decimal (nullable),
  "referencia": string (nullable),
  "fecha": DateTime (nullable),
  "descripcion": string (nullable)
}
```
