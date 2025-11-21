# Documentación del Endpoint `infoUsuario`

## Descripción General
El endpoint `infoUsuario` permite obtener información detallada del usuario autenticado basándose en el token JWT proporcionado en la solicitud. Este endpoint no requiere parámetros adicionales ya que el usuario se obtiene automáticamente del token de autenticación.

## Endpoint

### URL
```
GET /api/UserInfo/infoUsuario
```

### Método HTTP
`GET`

### Autenticación
**Requerida**: Sí  
**Tipo**: Bearer Token (JWT)

El token JWT debe ser enviado en el header `Authorization` con el formato:
```
Authorization: Bearer {token}
```

## Parámetros

### Headers
| Header | Tipo | Requerido | Descripción |
|--------|------|-----------|-------------|
| Authorization | string | Sí | Token JWT con formato "Bearer {token}" obtenido del endpoint de login |

### Query Parameters
Ninguno

### Body Parameters
Ninguno

## Respuesta Exitosa

### Código de Estado
`200 OK`

### Estructura de la Respuesta

```json
{
  "credencialId": 1,
  "nombreCompleto": "Braulio Emiliano Vazquez Valdez",
  "correo": "baemvav@gmail.com",
  "telefono": "5655139308",
  "nivel": 6,
  "tipoUsuario": "Devs"
}
```

### Campos de la Respuesta

| Campo | Tipo de Dato | Tipo SQL | Descripción |
|-------|--------------|----------|-------------|
| `credencialId` | int | int | ID único de la credencial del usuario |
| `nombreCompleto` | string | nvarchar(max) | Nombre completo del usuario (concatenación de nombre y apellido) |
| `correo` | string | nvarchar(100) | Dirección de correo electrónico del usuario |
| `telefono` | string | nvarchar(100) | Número de teléfono del usuario |
| `nivel` | int | int | Nivel de acceso del usuario |
| `tipoUsuario` | string | nvarchar(100) | Tipo o rol del usuario (ej: "Devs", "Admin", etc.) |

## Códigos de Respuesta

| Código | Descripción | Ejemplo de Respuesta |
|--------|-------------|----------------------|
| 200 | Éxito - Retorna la información del usuario | Ver estructura arriba |
| 401 | No autorizado - Token inválido, expirado o no proporcionado | `{"message": "Token inválido o no contiene información de usuario"}` |
| 404 | Usuario no encontrado en la base de datos | `{"message": "Usuario no encontrado"}` |
| 500 | Error interno del servidor | `{"message": "Error interno del servidor."}` |

## Funcionamiento Interno

1. **Autenticación**: El middleware de autenticación JWT valida el token proporcionado en el header Authorization.
2. **Extracción del Usuario**: Se extrae el nombre de usuario (claim `sub`) del token JWT decodificado.
3. **Consulta a la Base de Datos**: Se ejecuta el procedimiento almacenado `sp_contacto_usuario_select` con el nombre de usuario como parámetro.
4. **Respuesta**: Se retorna la información del usuario en formato JSON.

### Procedimiento Almacenado Utilizado

```sql
PROCEDURE [dbo].[sp_contacto_usuario_select]
    @usuario nvarchar(100)    
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c_r.credencial_id,
        CONCAT(nombre,' ',apellido) AS nombreCompleto,
        correo,
        telefono,
        nivel,
        tipoUsuario 
    FROM contacto c_o 
    LEFT JOIN credenciales c_r ON c_r.credencial_id = c_o.credencial_id 
    LEFT JOIN tipo_usuario t_u ON t_u.idTipoUsuario = c_r.nivel
    WHERE c_r.usuario = @usuario  
END
```

## Implementación en Cliente

### Ejemplo en JavaScript/TypeScript

#### Usando Fetch API

```javascript
// URL base del API
const API_BASE_URL = 'https://your-api-domain.com/api';

/**
 * Obtiene la información del usuario autenticado
 * @param {string} token - Token JWT obtenido del login
 * @returns {Promise<Object>} Información del usuario
 */
async function getInfoUsuario(token) {
    try {
        const response = await fetch(`${API_BASE_URL}/UserInfo/infoUsuario`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            if (response.status === 401) {
                throw new Error('Token inválido o expirado');
            } else if (response.status === 404) {
                throw new Error('Usuario no encontrado');
            } else {
                throw new Error('Error al obtener información del usuario');
            }
        }

        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error en getInfoUsuario:', error);
        throw error;
    }
}

// Uso del método
const token = 'tu-jwt-token-aqui';
getInfoUsuario(token)
    .then(userInfo => {
        console.log('Información del usuario:', userInfo);
        console.log('Nombre:', userInfo.nombreCompleto);
        console.log('Correo:', userInfo.correo);
        console.log('Teléfono:', userInfo.telefono);
        console.log('Nivel:', userInfo.nivel);
        console.log('Tipo:', userInfo.tipoUsuario);
    })
    .catch(error => {
        console.error('Error:', error.message);
    });
```

#### Usando Axios

```javascript
import axios from 'axios';

const API_BASE_URL = 'https://your-api-domain.com/api';

/**
 * Servicio para obtener información del usuario
 */
class UserInfoService {
    /**
     * Obtiene la información del usuario autenticado
     * @param {string} token - Token JWT
     * @returns {Promise<UserInfo>} Información del usuario
     */
    static async getInfoUsuario(token) {
        try {
            const response = await axios.get(
                `${API_BASE_URL}/UserInfo/infoUsuario`,
                {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                }
            );
            return response.data;
        } catch (error) {
            if (error.response) {
                // El servidor respondió con un código de error
                throw new Error(error.response.data.message || 'Error al obtener información del usuario');
            } else if (error.request) {
                // La petición fue hecha pero no hubo respuesta
                throw new Error('No se recibió respuesta del servidor');
            } else {
                // Algo pasó al configurar la petición
                throw new Error('Error al realizar la petición');
            }
        }
    }
}

// Uso del servicio
const token = localStorage.getItem('access_token');
UserInfoService.getInfoUsuario(token)
    .then(userInfo => {
        console.log('Información del usuario:', userInfo);
    })
    .catch(error => {
        console.error('Error:', error.message);
    });
```

#### TypeScript con Tipos Definidos

```typescript
// Definición de tipos
interface UserInfo {
    credencialId: number;
    nombreCompleto: string;
    correo: string;
    telefono: string;
    nivel: number;
    tipoUsuario: string;
}

interface ApiError {
    message: string;
}

// Servicio de información de usuario
class UserInfoService {
    private static readonly API_BASE_URL = 'https://your-api-domain.com/api';

    /**
     * Obtiene la información del usuario autenticado
     * @param token Token JWT de autenticación
     * @returns Promise con la información del usuario
     * @throws Error si la petición falla
     */
    static async getInfoUsuario(token: string): Promise<UserInfo> {
        const response = await fetch(
            `${this.API_BASE_URL}/UserInfo/infoUsuario`,
            {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            }
        );

        if (!response.ok) {
            const errorData: ApiError = await response.json();
            throw new Error(errorData.message || 'Error al obtener información del usuario');
        }

        const data: UserInfo = await response.json();
        return data;
    }
}

// Uso con async/await
async function displayUserInfo(): Promise<void> {
    try {
        const token = localStorage.getItem('access_token');
        if (!token) {
            throw new Error('No hay token de autenticación');
        }

        const userInfo = await UserInfoService.getInfoUsuario(token);
        
        console.log('Usuario:', userInfo);
        console.log(`ID: ${userInfo.credencialId}`);
        console.log(`Nombre: ${userInfo.nombreCompleto}`);
        console.log(`Email: ${userInfo.correo}`);
        console.log(`Teléfono: ${userInfo.telefono}`);
        console.log(`Nivel: ${userInfo.nivel}`);
        console.log(`Tipo: ${userInfo.tipoUsuario}`);
    } catch (error) {
        console.error('Error al obtener información del usuario:', error);
    }
}

displayUserInfo();
```

### Ejemplo en React

```typescript
import React, { useEffect, useState } from 'react';
import axios from 'axios';

interface UserInfo {
    credencialId: number;
    nombreCompleto: string;
    correo: string;
    telefono: string;
    nivel: number;
    tipoUsuario: string;
}

const UserProfile: React.FC = () => {
    const [userInfo, setUserInfo] = useState<UserInfo | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchUserInfo = async () => {
            try {
                const token = localStorage.getItem('access_token');
                if (!token) {
                    throw new Error('No hay token de autenticación');
                }

                const response = await axios.get(
                    'https://your-api-domain.com/api/UserInfo/infoUsuario',
                    {
                        headers: {
                            'Authorization': `Bearer ${token}`
                        }
                    }
                );

                setUserInfo(response.data);
                setLoading(false);
            } catch (err) {
                setError('Error al cargar información del usuario');
                setLoading(false);
                console.error(err);
            }
        };

        fetchUserInfo();
    }, []);

    if (loading) {
        return <div>Cargando...</div>;
    }

    if (error) {
        return <div className="error">{error}</div>;
    }

    if (!userInfo) {
        return <div>No se encontró información del usuario</div>;
    }

    return (
        <div className="user-profile">
            <h2>Perfil de Usuario</h2>
            <div className="user-info">
                <p><strong>ID:</strong> {userInfo.credencialId}</p>
                <p><strong>Nombre:</strong> {userInfo.nombreCompleto}</p>
                <p><strong>Correo:</strong> {userInfo.correo}</p>
                <p><strong>Teléfono:</strong> {userInfo.telefono}</p>
                <p><strong>Nivel:</strong> {userInfo.nivel}</p>
                <p><strong>Tipo de Usuario:</strong> {userInfo.tipoUsuario}</p>
            </div>
        </div>
    );
};

export default UserProfile;
```

### Ejemplo en C# (.NET)

```csharp
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

// Modelo de datos
public class UserInfo
{
    public int CredencialId { get; set; }
    public string NombreCompleto { get; set; }
    public string Correo { get; set; }
    public string Telefono { get; set; }
    public int Nivel { get; set; }
    public string TipoUsuario { get; set; }
}

// Servicio de cliente HTTP
public class UserInfoService
{
    private readonly HttpClient _httpClient;
    private const string API_BASE_URL = "https://your-api-domain.com/api";

    public UserInfoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado
    /// </summary>
    /// <param name="token">Token JWT de autenticación</param>
    /// <returns>Información del usuario</returns>
    public async Task<UserInfo> GetInfoUsuarioAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync($"{API_BASE_URL}/UserInfo/infoUsuario");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error al obtener información del usuario: {response.StatusCode}. {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        };
        
        var userInfo = JsonSerializer.Deserialize<UserInfo>(content, options);
        return userInfo;
    }
}

// Uso del servicio
public class Program
{
    public static async Task Main(string[] args)
    {
        using var httpClient = new HttpClient();
        var service = new UserInfoService(httpClient);

        try
        {
            var token = "tu-jwt-token-aqui";
            var userInfo = await service.GetInfoUsuarioAsync(token);

            Console.WriteLine($"ID: {userInfo.CredencialId}");
            Console.WriteLine($"Nombre: {userInfo.NombreCompleto}");
            Console.WriteLine($"Correo: {userInfo.Correo}");
            Console.WriteLine($"Teléfono: {userInfo.Telefono}");
            Console.WriteLine($"Nivel: {userInfo.Nivel}");
            Console.WriteLine($"Tipo: {userInfo.TipoUsuario}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

### Ejemplo en Python

```python
import requests
from typing import Optional, Dict
import json

class UserInfo:
    """Modelo de información de usuario"""
    def __init__(self, data: Dict):
        self.credencial_id: int = data.get('credencialId')
        self.nombre_completo: str = data.get('nombreCompleto')
        self.correo: str = data.get('correo')
        self.telefono: str = data.get('telefono')
        self.nivel: int = data.get('nivel')
        self.tipo_usuario: str = data.get('tipoUsuario')

    def __str__(self):
        return f"UserInfo(id={self.credencial_id}, nombre={self.nombre_completo}, correo={self.correo})"


class UserInfoService:
    """Servicio para obtener información del usuario"""
    
    API_BASE_URL = "https://your-api-domain.com/api"
    
    @staticmethod
    def get_info_usuario(token: str) -> Optional[UserInfo]:
        """
        Obtiene la información del usuario autenticado
        
        Args:
            token: Token JWT de autenticación
            
        Returns:
            UserInfo: Objeto con la información del usuario
            
        Raises:
            requests.exceptions.HTTPError: Si la petición falla
            ValueError: Si el token es inválido
        """
        if not token:
            raise ValueError("El token no puede estar vacío")
            
        url = f"{UserInfoService.API_BASE_URL}/UserInfo/infoUsuario"
        headers = {
            'Authorization': f'Bearer {token}',
            'Content-Type': 'application/json'
        }
        
        try:
            response = requests.get(url, headers=headers)
            response.raise_for_status()
            
            data = response.json()
            return UserInfo(data)
            
        except requests.exceptions.HTTPError as e:
            if e.response.status_code == 401:
                raise Exception("Token inválido o expirado")
            elif e.response.status_code == 404:
                raise Exception("Usuario no encontrado")
            else:
                raise Exception(f"Error al obtener información del usuario: {e}")
        except requests.exceptions.RequestException as e:
            raise Exception(f"Error de conexión: {e}")
        except json.JSONDecodeError:
            raise Exception("Error al decodificar la respuesta del servidor")


# Uso del servicio
if __name__ == "__main__":
    token = "tu-jwt-token-aqui"
    
    try:
        user_info = UserInfoService.get_info_usuario(token)
        
        print(f"ID: {user_info.credencial_id}")
        print(f"Nombre: {user_info.nombre_completo}")
        print(f"Correo: {user_info.correo}")
        print(f"Teléfono: {user_info.telefono}")
        print(f"Nivel: {user_info.nivel}")
        print(f"Tipo: {user_info.tipo_usuario}")
        
    except Exception as e:
        print(f"Error: {e}")
```

## Flujo Completo de Autenticación y Obtención de Información

### 1. Login
```javascript
// Primero, obtener el token mediante login
const loginResponse = await fetch('https://your-api-domain.com/api/Auth/login', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({
        username: 'tu-usuario',
        password: 'tu-contraseña'
    })
});

const loginData = await loginResponse.json();
const accessToken = loginData.accessToken;

// Guardar el token (ejemplo con localStorage)
localStorage.setItem('access_token', accessToken);
```

### 2. Obtener Información del Usuario
```javascript
// Usar el token para obtener información del usuario
const token = localStorage.getItem('access_token');
const userInfo = await getInfoUsuario(token);
```

## Manejo de Errores

### Errores Comunes

1. **401 Unauthorized**: 
   - Causa: Token inválido, expirado o no proporcionado
   - Solución: Realizar login nuevamente o refrescar el token

2. **404 Not Found**: 
   - Causa: El usuario no existe en la base de datos
   - Solución: Verificar que el usuario esté registrado correctamente

3. **500 Internal Server Error**: 
   - Causa: Error en el servidor o base de datos
   - Solución: Contactar al administrador del sistema

### Ejemplo de Manejo de Errores Robusto

```javascript
async function getUserInfoWithRetry(token, maxRetries = 3) {
    let retries = 0;
    
    while (retries < maxRetries) {
        try {
            const userInfo = await getInfoUsuario(token);
            return userInfo;
        } catch (error) {
            retries++;
            
            if (error.message.includes('Token inválido') || error.message.includes('401')) {
                // Token expirado, necesita reautenticación
                console.error('Token expirado. Por favor, inicie sesión nuevamente.');
                // Redirigir a login
                window.location.href = '/login';
                return null;
            }
            
            if (retries >= maxRetries) {
                console.error(`Falló después de ${maxRetries} intentos:`, error);
                throw error;
            }
            
            // Esperar antes de reintentar
            await new Promise(resolve => setTimeout(resolve, 1000 * retries));
        }
    }
}
```

## Consideraciones de Seguridad

1. **Almacenamiento del Token**: 
   - Usar `sessionStorage` o `localStorage` con precaución
   - En producción, considerar HttpOnly cookies

2. **HTTPS**: 
   - Siempre usar HTTPS en producción
   - El token contiene información sensible

3. **Expiración del Token**: 
   - El token tiene una duración limitada (60 minutos por defecto)
   - Implementar refresh token para renovar sin requerir login

4. **Validación**: 
   - Validar siempre las respuestas del servidor
   - No confiar en datos del cliente

## Pruebas con Postman/Thunder Client

### Configuración de la Petición

```
Method: GET
URL: https://your-api-domain.com/api/UserInfo/infoUsuario
Headers:
  Authorization: Bearer {tu-jwt-token}
  Content-Type: application/json
```

### Ejemplo de cURL

```bash
curl -X GET "https://your-api-domain.com/api/UserInfo/infoUsuario" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json"
```

## Soporte

Para problemas o preguntas sobre la implementación:
- Revisar los logs del servidor para detalles de errores
- Verificar que el procedimiento almacenado `sp_contacto_usuario_select` esté correctamente configurado
- Asegurar que las tablas `contacto`, `credenciales` y `tipo_usuario` tengan los datos correctos

## Changelog

### Versión 1.0.0
- Implementación inicial del endpoint `infoUsuario`
- Soporte para autenticación JWT
- Documentación completa de la API
