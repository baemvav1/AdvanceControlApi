/**
 * Servicio de Mantenimiento para consumir el API MantenimientoController
 * 
 * Este servicio proporciona métodos para:
 * - GET: Obtener mantenimientos con filtros (identificador, idCliente)
 * - POST: Crear un nuevo mantenimiento
 * - DELETE: Eliminar un mantenimiento
 * 
 * Basado en el patrón de EquipoService y CLIENT_IMPLEMENTATION_GUIDE.md
 */

class MantenimientoService {
    constructor(baseUrl = '/api', authService = null) {
        this.baseUrl = baseUrl;
        this.authService = authService;
    }

    /**
     * Obtiene el token de acceso para autenticación
     * @returns {string|null} Token de acceso
     * @private
     */
    _getAccessToken() {
        if (this.authService && typeof this.authService.getAccessToken === 'function') {
            return this.authService.getAccessToken();
        }
        return sessionStorage.getItem('accessToken');
    }

    /**
     * Obtiene los headers de autenticación
     * @returns {Object} Headers con Authorization
     * @private
     */
    _getHeaders() {
        const headers = {
            'Content-Type': 'application/json',
        };
        
        const token = this._getAccessToken();
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }
        
        return headers;
    }

    /**
     * Realiza una petición autenticada con manejo de errores
     * @param {string} url - URL de la petición
     * @param {Object} options - Opciones de fetch
     * @returns {Promise<Response>} Respuesta de la petición
     * @private
     */
    async _fetchWithAuth(url, options = {}) {
        const headers = this._getHeaders();
        
        const response = await fetch(url, {
            ...options,
            headers: {
                ...headers,
                ...options.headers,
            },
        });

        // Si es 401 y hay authService, intentar refresh
        if (response.status === 401 && this.authService && typeof this.authService.refresh === 'function') {
            const refreshed = await this.authService.refresh();
            if (refreshed) {
                const newHeaders = this._getHeaders();
                return fetch(url, {
                    ...options,
                    headers: {
                        ...newHeaders,
                        ...options.headers,
                    },
                });
            }
        }

        return response;
    }

    /**
     * Obtiene mantenimientos según los criterios de búsqueda
     * GET /api/Mantenimiento
     * 
     * @param {Object} params - Parámetros de búsqueda
     * @param {string} [params.identificador] - Búsqueda parcial por identificador del equipo
     * @param {number} [params.idCliente=0] - Filtro exacto por ID de cliente (0 para no filtrar)
     * @returns {Promise<{success: boolean, data?: Array, error?: string}>} Lista de mantenimientos
     */
    async getMantenimientos(params = {}) {
        try {
            const { identificador = null, idCliente = 0 } = params;
            
            // Construir query string
            const queryParams = new URLSearchParams();
            if (identificador) {
                queryParams.append('identificador', identificador);
            }
            if (idCliente > 0) {
                queryParams.append('idCliente', idCliente.toString());
            }
            
            const queryString = queryParams.toString();
            const url = `${this.baseUrl}/Mantenimiento${queryString ? `?${queryString}` : ''}`;
            
            const response = await this._fetchWithAuth(url, {
                method: 'GET',
            });

            if (response.ok) {
                const data = await response.json();
                return { success: true, data };
            } else {
                const error = await response.json().catch(() => ({ message: 'Error desconocido' }));
                return { success: false, error: error.message || 'Error al obtener mantenimientos' };
            }
        } catch (error) {
            console.error('Error en getMantenimientos:', error);
            return { success: false, error: error.message || 'Error de conexión' };
        }
    }

    /**
     * Crea un nuevo mantenimiento
     * POST /api/Mantenimiento
     * 
     * @param {Object} mantenimiento - Datos del mantenimiento a crear
     * @param {number} mantenimiento.idTipoMantenimiento - ID del tipo de mantenimiento (obligatorio, > 0)
     * @param {number} mantenimiento.idCliente - ID del cliente (obligatorio, > 0)
     * @param {number} mantenimiento.idEquipo - ID del equipo (obligatorio, > 0)
     * @param {number} mantenimiento.costo - Costo del mantenimiento (obligatorio, > 0)
     * @param {string} [mantenimiento.nota] - Nota asociada al mantenimiento (opcional)
     * @returns {Promise<{success: boolean, data?: Object, error?: string}>} Resultado de la operación
     */
    async createMantenimiento(mantenimiento) {
        try {
            const { idTipoMantenimiento, idCliente, idEquipo, costo, nota = null } = mantenimiento;
            
            // Validaciones del lado del cliente
            if (!idTipoMantenimiento || idTipoMantenimiento <= 0) {
                return { success: false, error: "El campo 'idTipoMantenimiento' debe ser mayor que 0." };
            }
            
            if (!idCliente || idCliente <= 0) {
                return { success: false, error: "El campo 'idCliente' debe ser mayor que 0." };
            }
            
            if (!idEquipo || idEquipo <= 0) {
                return { success: false, error: "El campo 'idEquipo' debe ser mayor que 0." };
            }
            
            if (!costo || costo <= 0) {
                return { success: false, error: "El campo 'costo' debe ser mayor que 0." };
            }
            
            // Construir query string
            const queryParams = new URLSearchParams();
            queryParams.append('idTipoMantenimiento', idTipoMantenimiento.toString());
            queryParams.append('idCliente', idCliente.toString());
            queryParams.append('idEquipo', idEquipo.toString());
            queryParams.append('costo', costo.toString());
            if (nota) {
                queryParams.append('nota', nota);
            }
            
            const url = `${this.baseUrl}/Mantenimiento?${queryParams.toString()}`;
            
            const response = await this._fetchWithAuth(url, {
                method: 'POST',
            });

            if (response.ok) {
                const data = await response.json();
                return { success: true, data };
            } else {
                const error = await response.json().catch(() => ({ message: 'Error desconocido' }));
                return { success: false, error: error.message || 'Error al crear mantenimiento' };
            }
        } catch (error) {
            console.error('Error en createMantenimiento:', error);
            return { success: false, error: error.message || 'Error de conexión' };
        }
    }

    /**
     * Elimina (soft delete) un mantenimiento
     * DELETE /api/Mantenimiento
     * 
     * @param {number} idMantenimiento - ID del mantenimiento a eliminar (obligatorio, > 0)
     * @returns {Promise<{success: boolean, data?: Object, error?: string}>} Resultado de la operación
     */
    async deleteMantenimiento(idMantenimiento) {
        try {
            // Validación del lado del cliente
            if (!idMantenimiento || idMantenimiento <= 0) {
                return { success: false, error: "El campo 'idMantenimiento' debe ser mayor que 0." };
            }
            
            const queryParams = new URLSearchParams();
            queryParams.append('idMantenimiento', idMantenimiento.toString());
            
            const url = `${this.baseUrl}/Mantenimiento?${queryParams.toString()}`;
            
            const response = await this._fetchWithAuth(url, {
                method: 'DELETE',
            });

            if (response.ok) {
                const data = await response.json();
                return { success: true, data };
            } else {
                const error = await response.json().catch(() => ({ message: 'Error desconocido' }));
                return { success: false, error: error.message || 'Error al eliminar mantenimiento' };
            }
        } catch (error) {
            console.error('Error en deleteMantenimiento:', error);
            return { success: false, error: error.message || 'Error de conexión' };
        }
    }
}

// Export para uso con ES modules
export default MantenimientoService;

// Export para uso con CommonJS
if (typeof module !== 'undefined' && module.exports) {
    module.exports = MantenimientoService;
}
