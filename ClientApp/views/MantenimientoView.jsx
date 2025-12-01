import React, { useState, useEffect, useCallback } from 'react';
import MantenimientoService from '../services/MantenimientoService';

/**
 * MantenimientoView - Componente React para gestionar mantenimientos
 * 
 * Basado en el patrón de EquiposView
 * Consume el API MantenimientoController
 * 
 * Funcionalidades:
 * - Listar mantenimientos con filtros (identificador, cliente)
 * - Crear nuevo mantenimiento
 * - Eliminar mantenimiento existente
 */

const MantenimientoView = ({ authService }) => {
    // Estado para lista de mantenimientos
    const [mantenimientos, setMantenimientos] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [successMessage, setSuccessMessage] = useState('');

    // Estado para filtros de búsqueda
    const [filtros, setFiltros] = useState({
        identificador: '',
        idCliente: 0
    });

    // Estado para formulario de nuevo mantenimiento
    const [nuevoMantenimiento, setNuevoMantenimiento] = useState({
        idTipoMantenimiento: '',
        idCliente: '',
        idEquipo: '',
        costo: '',
        nota: ''
    });

    // Estado para modal de confirmación de eliminación
    const [deleteConfirm, setDeleteConfirm] = useState({
        show: false,
        idMantenimiento: null
    });

    // Estado para mostrar/ocultar formulario de crear
    const [showCreateForm, setShowCreateForm] = useState(false);

    // Instancia del servicio
    const mantenimientoService = new MantenimientoService('/api', authService);

    /**
     * Carga los mantenimientos aplicando los filtros actuales
     */
    const cargarMantenimientos = useCallback(async () => {
        setLoading(true);
        setError('');
        
        try {
            const result = await mantenimientoService.getMantenimientos({
                identificador: filtros.identificador || null,
                idCliente: parseInt(filtros.idCliente) || 0
            });

            if (result.success) {
                setMantenimientos(result.data || []);
            } else {
                setError(result.error || 'Error al cargar mantenimientos');
                setMantenimientos([]);
            }
        } catch (err) {
            setError('Error de conexión al cargar mantenimientos');
            setMantenimientos([]);
        } finally {
            setLoading(false);
        }
    }, [filtros.identificador, filtros.idCliente]);

    // Cargar mantenimientos al montar el componente
    useEffect(() => {
        cargarMantenimientos();
    }, [cargarMantenimientos]);

    /**
     * Maneja cambios en los filtros de búsqueda
     */
    const handleFiltroChange = (e) => {
        const { name, value } = e.target;
        setFiltros(prev => ({
            ...prev,
            [name]: value
        }));
    };

    /**
     * Ejecuta la búsqueda con los filtros actuales
     */
    const handleBuscar = (e) => {
        e.preventDefault();
        cargarMantenimientos();
    };

    /**
     * Limpia los filtros y recarga todos los mantenimientos
     */
    const handleLimpiarFiltros = () => {
        setFiltros({
            identificador: '',
            idCliente: 0
        });
    };

    /**
     * Maneja cambios en el formulario de nuevo mantenimiento
     */
    const handleNuevoMantenimientoChange = (e) => {
        const { name, value } = e.target;
        setNuevoMantenimiento(prev => ({
            ...prev,
            [name]: value
        }));
    };

    /**
     * Crea un nuevo mantenimiento
     */
    const handleCrearMantenimiento = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        setSuccessMessage('');

        try {
            const result = await mantenimientoService.createMantenimiento({
                idTipoMantenimiento: parseInt(nuevoMantenimiento.idTipoMantenimiento),
                idCliente: parseInt(nuevoMantenimiento.idCliente),
                idEquipo: parseInt(nuevoMantenimiento.idEquipo),
                costo: parseFloat(nuevoMantenimiento.costo),
                nota: nuevoMantenimiento.nota || null
            });

            if (result.success) {
                setSuccessMessage('Mantenimiento creado correctamente');
                setNuevoMantenimiento({
                    idTipoMantenimiento: '',
                    idCliente: '',
                    idEquipo: '',
                    costo: '',
                    nota: ''
                });
                setShowCreateForm(false);
                cargarMantenimientos();
            } else {
                setError(result.error || 'Error al crear mantenimiento');
            }
        } catch (err) {
            setError('Error de conexión al crear mantenimiento');
        } finally {
            setLoading(false);
        }
    };

    /**
     * Muestra el modal de confirmación para eliminar
     */
    const handleDeleteClick = (idMantenimiento) => {
        setDeleteConfirm({
            show: true,
            idMantenimiento
        });
    };

    /**
     * Cancela la eliminación
     */
    const handleDeleteCancel = () => {
        setDeleteConfirm({
            show: false,
            idMantenimiento: null
        });
    };

    /**
     * Confirma y ejecuta la eliminación
     */
    const handleDeleteConfirm = async () => {
        if (!deleteConfirm.idMantenimiento) return;

        setLoading(true);
        setError('');
        setSuccessMessage('');

        try {
            const result = await mantenimientoService.deleteMantenimiento(deleteConfirm.idMantenimiento);

            if (result.success) {
                setSuccessMessage('Mantenimiento eliminado correctamente');
                cargarMantenimientos();
            } else {
                setError(result.error || 'Error al eliminar mantenimiento');
            }
        } catch (err) {
            setError('Error de conexión al eliminar mantenimiento');
        } finally {
            setLoading(false);
            setDeleteConfirm({
                show: false,
                idMantenimiento: null
            });
        }
    };

    /**
     * Formatea el costo como moneda
     */
    const formatCurrency = (value) => {
        if (value == null) return '-';
        return new Intl.NumberFormat('es-MX', {
            style: 'currency',
            currency: 'MXN'
        }).format(value);
    };

    return (
        <div className="mantenimiento-view">
            <h2>Gestión de Mantenimientos</h2>

            {/* Mensajes de error y éxito */}
            {error && (
                <div className="alert alert-error" role="alert">
                    {error}
                    <button onClick={() => setError('')} className="close-btn">&times;</button>
                </div>
            )}
            {successMessage && (
                <div className="alert alert-success" role="alert">
                    {successMessage}
                    <button onClick={() => setSuccessMessage('')} className="close-btn">&times;</button>
                </div>
            )}

            {/* Sección de filtros */}
            <div className="filtros-section">
                <h3>Buscar Mantenimientos</h3>
                <form onSubmit={handleBuscar} className="filtros-form">
                    <div className="form-group">
                        <label htmlFor="identificador">Identificador del Equipo:</label>
                        <input
                            type="text"
                            id="identificador"
                            name="identificador"
                            value={filtros.identificador}
                            onChange={handleFiltroChange}
                            placeholder="Búsqueda parcial..."
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="idCliente">ID Cliente:</label>
                        <input
                            type="number"
                            id="idCliente"
                            name="idCliente"
                            value={filtros.idCliente || ''}
                            onChange={handleFiltroChange}
                            placeholder="0 para todos"
                            min="0"
                        />
                    </div>
                    <div className="form-actions">
                        <button type="submit" disabled={loading} className="btn btn-primary">
                            {loading ? 'Buscando...' : 'Buscar'}
                        </button>
                        <button type="button" onClick={handleLimpiarFiltros} className="btn btn-secondary">
                            Limpiar Filtros
                        </button>
                    </div>
                </form>
            </div>

            {/* Botón para mostrar formulario de crear */}
            <div className="actions-section">
                <button 
                    onClick={() => setShowCreateForm(!showCreateForm)} 
                    className="btn btn-success"
                >
                    {showCreateForm ? 'Cancelar' : 'Nuevo Mantenimiento'}
                </button>
            </div>

            {/* Formulario de crear mantenimiento */}
            {showCreateForm && (
                <div className="crear-section">
                    <h3>Crear Nuevo Mantenimiento</h3>
                    <form onSubmit={handleCrearMantenimiento} className="crear-form">
                        <div className="form-group">
                            <label htmlFor="idTipoMantenimiento">Tipo de Mantenimiento *:</label>
                            <input
                                type="number"
                                id="idTipoMantenimiento"
                                name="idTipoMantenimiento"
                                value={nuevoMantenimiento.idTipoMantenimiento}
                                onChange={handleNuevoMantenimientoChange}
                                required
                                min="1"
                                placeholder="ID del tipo de mantenimiento"
                            />
                        </div>
                        <div className="form-group">
                            <label htmlFor="idClienteNuevo">Cliente *:</label>
                            <input
                                type="number"
                                id="idClienteNuevo"
                                name="idCliente"
                                value={nuevoMantenimiento.idCliente}
                                onChange={handleNuevoMantenimientoChange}
                                required
                                min="1"
                                placeholder="ID del cliente"
                            />
                        </div>
                        <div className="form-group">
                            <label htmlFor="idEquipo">Equipo *:</label>
                            <input
                                type="number"
                                id="idEquipo"
                                name="idEquipo"
                                value={nuevoMantenimiento.idEquipo}
                                onChange={handleNuevoMantenimientoChange}
                                required
                                min="1"
                                placeholder="ID del equipo"
                            />
                        </div>
                        <div className="form-group">
                            <label htmlFor="costo">Costo *:</label>
                            <input
                                type="number"
                                id="costo"
                                name="costo"
                                value={nuevoMantenimiento.costo}
                                onChange={handleNuevoMantenimientoChange}
                                required
                                min="0.01"
                                step="0.01"
                                placeholder="Costo del mantenimiento"
                            />
                        </div>
                        <div className="form-group">
                            <label htmlFor="nota">Nota:</label>
                            <textarea
                                id="nota"
                                name="nota"
                                value={nuevoMantenimiento.nota}
                                onChange={handleNuevoMantenimientoChange}
                                placeholder="Nota opcional..."
                                rows="3"
                            />
                        </div>
                        <div className="form-actions">
                            <button type="submit" disabled={loading} className="btn btn-primary">
                                {loading ? 'Creando...' : 'Crear Mantenimiento'}
                            </button>
                            <button 
                                type="button" 
                                onClick={() => setShowCreateForm(false)} 
                                className="btn btn-secondary"
                            >
                                Cancelar
                            </button>
                        </div>
                    </form>
                </div>
            )}

            {/* Tabla de mantenimientos */}
            <div className="tabla-section">
                <h3>Lista de Mantenimientos ({mantenimientos.length})</h3>
                {loading && <div className="loading">Cargando...</div>}
                
                {!loading && mantenimientos.length === 0 && (
                    <p className="no-data">No se encontraron mantenimientos.</p>
                )}

                {!loading && mantenimientos.length > 0 && (
                    <table className="mantenimientos-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Tipo</th>
                                <th>Cliente</th>
                                <th>Razón Social</th>
                                <th>Equipo</th>
                                <th>Costo</th>
                                <th>Costo Total</th>
                                <th>Nota</th>
                                <th>Acciones</th>
                            </tr>
                        </thead>
                        <tbody>
                            {mantenimientos.map((m) => (
                                <tr key={m.idMantenimiento}>
                                    <td>{m.idMantenimiento}</td>
                                    <td>{m.tipoMantenimiento || '-'}</td>
                                    <td>{m.nombreComercial || '-'}</td>
                                    <td>{m.razonSocial || '-'}</td>
                                    <td>{m.identificador || '-'}</td>
                                    <td>{formatCurrency(m.costo)}</td>
                                    <td>{formatCurrency(m.costoTotal)}</td>
                                    <td>{m.nota || '-'}</td>
                                    <td>
                                        <button 
                                            onClick={() => handleDeleteClick(m.idMantenimiento)}
                                            className="btn btn-danger btn-sm"
                                            title="Eliminar mantenimiento"
                                        >
                                            Eliminar
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>

            {/* Modal de confirmación de eliminación */}
            {deleteConfirm.show && (
                <div className="modal-overlay">
                    <div className="modal">
                        <h4>Confirmar Eliminación</h4>
                        <p>¿Está seguro que desea eliminar el mantenimiento #{deleteConfirm.idMantenimiento}?</p>
                        <div className="modal-actions">
                            <button 
                                onClick={handleDeleteConfirm} 
                                className="btn btn-danger"
                                disabled={loading}
                            >
                                {loading ? 'Eliminando...' : 'Eliminar'}
                            </button>
                            <button 
                                onClick={handleDeleteCancel} 
                                className="btn btn-secondary"
                                disabled={loading}
                            >
                                Cancelar
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Estilos inline (pueden moverse a un archivo CSS separado) */}
            <style>{`
                .mantenimiento-view {
                    padding: 20px;
                    max-width: 1200px;
                    margin: 0 auto;
                }

                .mantenimiento-view h2 {
                    color: #333;
                    margin-bottom: 20px;
                }

                .mantenimiento-view h3 {
                    color: #555;
                    margin-bottom: 15px;
                    border-bottom: 1px solid #ddd;
                    padding-bottom: 10px;
                }

                .alert {
                    padding: 15px;
                    margin-bottom: 20px;
                    border-radius: 4px;
                    position: relative;
                }

                .alert-error {
                    background-color: #f8d7da;
                    color: #721c24;
                    border: 1px solid #f5c6cb;
                }

                .alert-success {
                    background-color: #d4edda;
                    color: #155724;
                    border: 1px solid #c3e6cb;
                }

                .close-btn {
                    position: absolute;
                    right: 10px;
                    top: 10px;
                    background: none;
                    border: none;
                    font-size: 20px;
                    cursor: pointer;
                }

                .filtros-section, .crear-section, .tabla-section {
                    background: #f9f9f9;
                    padding: 20px;
                    border-radius: 8px;
                    margin-bottom: 20px;
                }

                .form-group {
                    margin-bottom: 15px;
                }

                .form-group label {
                    display: block;
                    margin-bottom: 5px;
                    font-weight: bold;
                    color: #333;
                }

                .form-group input, .form-group textarea {
                    width: 100%;
                    padding: 10px;
                    border: 1px solid #ccc;
                    border-radius: 4px;
                    font-size: 14px;
                }

                .form-group input:focus, .form-group textarea:focus {
                    border-color: #007bff;
                    outline: none;
                    box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25);
                }

                .filtros-form {
                    display: grid;
                    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
                    gap: 15px;
                    align-items: end;
                }

                .crear-form {
                    max-width: 500px;
                }

                .form-actions {
                    display: flex;
                    gap: 10px;
                    margin-top: 15px;
                }

                .actions-section {
                    margin-bottom: 20px;
                }

                .btn {
                    padding: 10px 20px;
                    border: none;
                    border-radius: 4px;
                    cursor: pointer;
                    font-size: 14px;
                    transition: background-color 0.2s;
                }

                .btn:disabled {
                    opacity: 0.6;
                    cursor: not-allowed;
                }

                .btn-primary {
                    background-color: #007bff;
                    color: white;
                }

                .btn-primary:hover:not(:disabled) {
                    background-color: #0056b3;
                }

                .btn-secondary {
                    background-color: #6c757d;
                    color: white;
                }

                .btn-secondary:hover:not(:disabled) {
                    background-color: #545b62;
                }

                .btn-success {
                    background-color: #28a745;
                    color: white;
                }

                .btn-success:hover:not(:disabled) {
                    background-color: #1e7e34;
                }

                .btn-danger {
                    background-color: #dc3545;
                    color: white;
                }

                .btn-danger:hover:not(:disabled) {
                    background-color: #c82333;
                }

                .btn-sm {
                    padding: 5px 10px;
                    font-size: 12px;
                }

                .loading {
                    text-align: center;
                    padding: 20px;
                    color: #666;
                }

                .no-data {
                    text-align: center;
                    padding: 20px;
                    color: #666;
                    font-style: italic;
                }

                .mantenimientos-table {
                    width: 100%;
                    border-collapse: collapse;
                    margin-top: 15px;
                }

                .mantenimientos-table th, .mantenimientos-table td {
                    padding: 12px;
                    text-align: left;
                    border-bottom: 1px solid #ddd;
                }

                .mantenimientos-table th {
                    background-color: #f2f2f2;
                    font-weight: bold;
                }

                .mantenimientos-table tr:hover {
                    background-color: #f5f5f5;
                }

                .modal-overlay {
                    position: fixed;
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    background-color: rgba(0, 0, 0, 0.5);
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    z-index: 1000;
                }

                .modal {
                    background: white;
                    padding: 30px;
                    border-radius: 8px;
                    max-width: 400px;
                    width: 90%;
                }

                .modal h4 {
                    margin-top: 0;
                    color: #333;
                }

                .modal-actions {
                    display: flex;
                    gap: 10px;
                    justify-content: flex-end;
                    margin-top: 20px;
                }

                @media (max-width: 768px) {
                    .filtros-form {
                        grid-template-columns: 1fr;
                    }

                    .mantenimientos-table {
                        display: block;
                        overflow-x: auto;
                    }
                }
            `}</style>
        </div>
    );
};

export default MantenimientoView;
