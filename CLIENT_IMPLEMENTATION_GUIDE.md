# Guía Rápida de Implementación del Cliente - Sistema de Autenticación

## Para Desarrolladores Frontend

Esta guía proporciona instrucciones paso a paso para implementar el sistema de autenticación en aplicaciones cliente.

---

## 1. Flujo Básico

```
1. Usuario ingresa credenciales
2. Cliente envía POST /api/Auth/login
3. Servidor valida y retorna tokens
4. Cliente guarda tokens
5. Cliente usa access token en peticiones
6. Si expira, usa refresh token para renovar
7. Al cerrar sesión, revoca refresh token
```

---

## 2. Endpoints Disponibles

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/Auth/login` | Iniciar sesión |
| POST | `/api/Auth/refresh` | Renovar tokens |
| POST | `/api/Auth/validate` | Validar token |
| POST | `/api/Auth/logout` | Cerrar sesión |

---

## 3. Implementación JavaScript/TypeScript

### Clase de Gestión de Autenticación

```javascript
class AuthService {
  constructor(baseUrl = '/api') {
    this.baseUrl = baseUrl;
    this.accessToken = null;
    this.refreshToken = null;
  }

  /**
   * Inicia sesión con credenciales
   * @param {string} username - Nombre de usuario
   * @param {string} password - Contraseña
   * @returns {Promise<boolean>} true si el login fue exitoso
   */
  async login(username, password) {
    try {
      const response = await fetch(`${this.baseUrl}/Auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username, password }),
      });

      if (response.ok) {
        const data = await response.json();
        this.accessToken = data.accessToken;
        this.refreshToken = data.refreshToken;
        
        // Guardar en localStorage (o mejor en memoria/sessionStorage)
        this._saveTokens();
        
        return true;
      } else {
        const error = await response.json();
        console.error('Login failed:', error.message);
        return false;
      }
    } catch (error) {
      console.error('Login error:', error);
      return false;
    }
  }

  /**
   * Renueva los tokens usando el refresh token
   * @returns {Promise<boolean>} true si la renovación fue exitosa
   */
  async refresh() {
    if (!this.refreshToken) {
      return false;
    }

    try {
      const response = await fetch(`${this.baseUrl}/Auth/refresh`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ refreshToken: this.refreshToken }),
      });

      if (response.ok) {
        const data = await response.json();
        this.accessToken = data.accessToken;
        this.refreshToken = data.refreshToken;
        this._saveTokens();
        return true;
      }
      
      // Si falla el refresh, limpiar tokens
      this._clearTokens();
      return false;
    } catch (error) {
      console.error('Refresh error:', error);
      this._clearTokens();
      return false;
    }
  }

  /**
   * Cierra la sesión
   */
  async logout() {
    if (this.refreshToken) {
      try {
        await fetch(`${this.baseUrl}/Auth/logout`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ refreshToken: this.refreshToken }),
        });
      } catch (error) {
        console.error('Logout error:', error);
      }
    }
    
    this._clearTokens();
  }

  /**
   * Realiza una petición autenticada con manejo automático de refresh
   * @param {string} url - URL relativa o absoluta
   * @param {Object} options - Opciones de fetch
   * @returns {Promise<Response>} Respuesta de la petición
   */
  async fetchWithAuth(url, options = {}) {
    // Cargar tokens si no están en memoria
    if (!this.accessToken) {
      this._loadTokens();
    }

    // Primera intento con el token actual
    let response = await this._makeAuthenticatedRequest(url, options);

    // Si es 401, intentar refresh y reintentar
    if (response.status === 401) {
      const refreshed = await this.refresh();
      if (refreshed) {
        response = await this._makeAuthenticatedRequest(url, options);
      }
    }

    return response;
  }

  /**
   * Realiza una petición con el access token
   * @private
   */
  async _makeAuthenticatedRequest(url, options = {}) {
    const headers = {
      ...options.headers,
      'Authorization': `Bearer ${this.accessToken}`,
    };

    return fetch(url, {
      ...options,
      headers,
    });
  }

  /**
   * Guarda los tokens (ajustar según necesidades de seguridad)
   * @private
   */
  _saveTokens() {
    // Opción 1: sessionStorage (se pierde al cerrar pestaña)
    sessionStorage.setItem('accessToken', this.accessToken);
    sessionStorage.setItem('refreshToken', this.refreshToken);
    
    // Opción 2: localStorage (persiste entre sesiones)
    // localStorage.setItem('accessToken', this.accessToken);
    // localStorage.setItem('refreshToken', this.refreshToken);
    
    // Opción 3: Solo en memoria (más seguro pero se pierde al recargar)
    // No guardar, solo mantener en this.accessToken y this.refreshToken
  }

  /**
   * Carga los tokens guardados
   * @private
   */
  _loadTokens() {
    this.accessToken = sessionStorage.getItem('accessToken');
    this.refreshToken = sessionStorage.getItem('refreshToken');
    // O de localStorage según la opción elegida
  }

  /**
   * Limpia los tokens
   * @private
   */
  _clearTokens() {
    this.accessToken = null;
    this.refreshToken = null;
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('refreshToken');
    // También limpiar localStorage si se usa
  }

  /**
   * Verifica si hay una sesión activa
   * @returns {boolean}
   */
  isAuthenticated() {
    if (!this.accessToken) {
      this._loadTokens();
    }
    return !!this.accessToken;
  }
}

// Uso global
const authService = new AuthService();

// Ejemplo de uso
async function ejemploDeUso() {
  // Login
  const loginExitoso = await authService.login('usuario', 'contraseña');
  if (loginExitoso) {
    console.log('Login exitoso');
    
    // Hacer peticiones autenticadas
    const response = await authService.fetchWithAuth('/api/Clientes');
    if (response.ok) {
      const clientes = await response.json();
      console.log('Clientes:', clientes);
    }
    
    // Logout
    await authService.logout();
  }
}
```

---

## 4. Implementación React

### Hooks Personalizados

```jsx
// useAuth.js
import { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [accessToken, setAccessToken] = useState(null);
  const [refreshToken, setRefreshToken] = useState(null);
  const [loading, setLoading] = useState(true);

  // Cargar tokens al iniciar
  useEffect(() => {
    const savedAccessToken = sessionStorage.getItem('accessToken');
    const savedRefreshToken = sessionStorage.getItem('refreshToken');
    
    if (savedAccessToken && savedRefreshToken) {
      setAccessToken(savedAccessToken);
      setRefreshToken(savedRefreshToken);
      // Opcionalmente, validar el token
    }
    
    setLoading(false);
  }, []);

  const login = async (username, password) => {
    try {
      const response = await fetch('/api/Auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password }),
      });

      if (response.ok) {
        const data = await response.json();
        setAccessToken(data.accessToken);
        setRefreshToken(data.refreshToken);
        setUser(data.user);
        
        sessionStorage.setItem('accessToken', data.accessToken);
        sessionStorage.setItem('refreshToken', data.refreshToken);
        
        return { success: true };
      } else {
        const error = await response.json();
        return { success: false, error: error.message };
      }
    } catch (error) {
      return { success: false, error: 'Error de conexión' };
    }
  };

  const logout = async () => {
    if (refreshToken) {
      try {
        await fetch('/api/Auth/logout', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ refreshToken }),
        });
      } catch (error) {
        console.error('Error al cerrar sesión:', error);
      }
    }

    setAccessToken(null);
    setRefreshToken(null);
    setUser(null);
    sessionStorage.clear();
  };

  const refreshAccessToken = async () => {
    if (!refreshToken) return false;

    try {
      const response = await fetch('/api/Auth/refresh', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken }),
      });

      if (response.ok) {
        const data = await response.json();
        setAccessToken(data.accessToken);
        setRefreshToken(data.refreshToken);
        
        sessionStorage.setItem('accessToken', data.accessToken);
        sessionStorage.setItem('refreshToken', data.refreshToken);
        
        return true;
      } else {
        await logout();
        return false;
      }
    } catch (error) {
      console.error('Error al renovar token:', error);
      await logout();
      return false;
    }
  };

  const value = {
    user,
    accessToken,
    login,
    logout,
    refreshAccessToken,
    isAuthenticated: !!accessToken,
    loading,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}
```

### Componente de Login

```jsx
// Login.jsx
import { useState } from 'react';
import { useAuth } from './useAuth';
import { useNavigate } from 'react-router-dom';

function Login() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    const result = await login(username, password);
    
    setLoading(false);

    if (result.success) {
      navigate('/dashboard');
    } else {
      setError(result.error || 'Error al iniciar sesión');
    }
  };

  return (
    <div className="login-container">
      <h2>Iniciar Sesión</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="username">Usuario:</label>
          <input
            id="username"
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            minLength={3}
            maxLength={150}
          />
        </div>
        
        <div>
          <label htmlFor="password">Contraseña:</label>
          <input
            id="password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            minLength={4}
            maxLength={100}
          />
        </div>

        {error && <div className="error">{error}</div>}

        <button type="submit" disabled={loading}>
          {loading ? 'Iniciando...' : 'Iniciar Sesión'}
        </button>
      </form>
    </div>
  );
}

export default Login;
```

### Interceptor de Axios (Alternativa)

```javascript
// axiosConfig.js
import axios from 'axios';

const api = axios.create({
  baseURL: '/api',
});

// Request interceptor - agregar token
api.interceptors.request.use(
  (config) => {
    const accessToken = sessionStorage.getItem('accessToken');
    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor - manejar refresh automático
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Si es 401 y no hemos reintentado
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = sessionStorage.getItem('refreshToken');
        
        if (!refreshToken) {
          throw new Error('No refresh token');
        }

        const response = await axios.post('/api/Auth/refresh', {
          refreshToken,
        });

        const { accessToken: newAccessToken, refreshToken: newRefreshToken } = response.data;
        
        sessionStorage.setItem('accessToken', newAccessToken);
        sessionStorage.setItem('refreshToken', newRefreshToken);

        // Reintentar la petición original
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
        return api(originalRequest);
      } catch (refreshError) {
        // Si falla el refresh, limpiar y redirigir a login
        sessionStorage.clear();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default api;

// Uso
// import api from './axiosConfig';
// const response = await api.get('/Clientes');
```

---

## 5. Implementación Angular

### Servicio de Autenticación

```typescript
// auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType: string;
  user: { username: string };
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private accessTokenSubject = new BehaviorSubject<string | null>(null);
  private refreshTokenSubject = new BehaviorSubject<string | null>(null);
  
  public accessToken$ = this.accessTokenSubject.asObservable();
  
  constructor(private http: HttpClient) {
    // Cargar tokens guardados
    const accessToken = sessionStorage.getItem('accessToken');
    const refreshToken = sessionStorage.getItem('refreshToken');
    
    if (accessToken) this.accessTokenSubject.next(accessToken);
    if (refreshToken) this.refreshTokenSubject.next(refreshToken);
  }

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>('/api/Auth/login', {
      username,
      password
    }).pipe(
      tap(response => {
        this.accessTokenSubject.next(response.accessToken);
        this.refreshTokenSubject.next(response.refreshToken);
        
        sessionStorage.setItem('accessToken', response.accessToken);
        sessionStorage.setItem('refreshToken', response.refreshToken);
      })
    );
  }

  refreshToken(): Observable<LoginResponse> {
    const refreshToken = this.refreshTokenSubject.value;
    
    return this.http.post<LoginResponse>('/api/Auth/refresh', {
      refreshToken
    }).pipe(
      tap(response => {
        this.accessTokenSubject.next(response.accessToken);
        this.refreshTokenSubject.next(response.refreshToken);
        
        sessionStorage.setItem('accessToken', response.accessToken);
        sessionStorage.setItem('refreshToken', response.refreshToken);
      })
    );
  }

  logout(): Observable<any> {
    const refreshToken = this.refreshTokenSubject.value;
    
    return this.http.post('/api/Auth/logout', { refreshToken }).pipe(
      tap(() => {
        this.accessTokenSubject.next(null);
        this.refreshTokenSubject.next(null);
        sessionStorage.clear();
      })
    );
  }

  getAccessToken(): string | null {
    return this.accessTokenSubject.value;
  }

  isAuthenticated(): boolean {
    return !!this.accessTokenSubject.value;
  }
}
```

### HTTP Interceptor

```typescript
// auth.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, take, switchMap } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const accessToken = this.authService.getAccessToken();
    
    if (accessToken) {
      request = this.addToken(request, accessToken);
    }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          return this.handle401Error(request, next);
        }
        return throwError(() => error);
      })
    );
  }

  private addToken(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((response: any) => {
          this.isRefreshing = false;
          this.refreshTokenSubject.next(response.accessToken);
          return next.handle(this.addToken(request, response.accessToken));
        }),
        catchError((err) => {
          this.isRefreshing = false;
          this.authService.logout().subscribe();
          return throwError(() => err);
        })
      );
    } else {
      return this.refreshTokenSubject.pipe(
        filter(token => token != null),
        take(1),
        switchMap(token => next.handle(this.addToken(request, token)))
      );
    }
  }
}
```

---

## 6. Consideraciones de Seguridad para el Cliente

### ✅ Almacenamiento de Tokens

**Opción 1: Solo en Memoria (Más Seguro)**
- Tokens solo en variables JavaScript
- Se pierde al recargar la página
- Requiere login frecuente
- Mejor contra XSS

**Opción 2: sessionStorage (Recomendado)**
- Se mantiene en la sesión actual
- Se pierde al cerrar la pestaña
- Balance entre seguridad y UX

**Opción 3: localStorage (Menos Seguro)**
- Persiste entre sesiones
- Vulnerable a XSS
- Conveniente pero menos seguro

**Opción 4: httpOnly Cookies (Ideal pero requiere backend)**
- Requiere modificar el backend
- Más seguro contra XSS
- Recomendado para producción

### ⚠️ Nunca Hacer

- ❌ No guardar tokens en URL
- ❌ No imprimir tokens en console.log
- ❌ No enviar tokens en query strings
- ❌ No compartir tokens entre dominios sin CORS apropiado

### ✅ Mejores Prácticas

1. **Siempre usar HTTPS** en producción
2. **Limpiar tokens** al cerrar sesión
3. **Implementar timeout** de inactividad
4. **Validar respuestas** del servidor
5. **Manejar errores** apropiadamente
6. **Refresh automático** antes de expiración

---

## 7. Solución de Problemas

### Problema: "Token inválido o expirado"
**Solución:** Usar el refresh token para obtener uno nuevo

### Problema: "Refresh token revocado"
**Solución:** Hacer login nuevamente, las sesiones fueron revocadas por seguridad

### Problema: CORS error
**Solución:** Verificar que el servidor tenga CORS configurado para tu dominio

### Problema: Token no se envía
**Solución:** Verificar que el header Authorization tenga formato `Bearer {token}`

---

## 8. Checklist de Implementación

- [ ] Implementar clase/servicio de autenticación
- [ ] Implementar formulario de login
- [ ] Agregar token a headers de peticiones
- [ ] Implementar manejo de refresh automático
- [ ] Implementar logout
- [ ] Manejar errores 401
- [ ] Proteger rutas que requieren autenticación
- [ ] Limpiar tokens al cerrar sesión
- [ ] Probar en diferentes navegadores
- [ ] Verificar que funcione con HTTPS

---

**Última actualización:** 2025-11-10
