USE [Advance]
GO

-- =============================================
-- PROCEDIMIENTOS PARA ESTADO DE CUENTA
-- =============================================

-- Crear Estado de Cuenta
CREATE OR ALTER PROCEDURE [dbo].[sp_CrearEstadoCuenta]
    @numeroCuenta NVARCHAR(50),
    @clabe NVARCHAR(18),
    @tipoCuenta NVARCHAR(100) = NULL,
    @tipoMoneda NVARCHAR(50) = 'MXN',
    @fechaInicio DATE,
    @fechaFin DATE,
    @fechaCorte DATE,
    @saldoInicial DECIMAL(18,2) = 0,
    @totalCargos DECIMAL(18,2) = 0,
    @totalAbonos DECIMAL(18,2) = 0,
    @saldoFinal DECIMAL(18,2) = 0,
    @totalComisiones DECIMAL(18,2) = 0,
    @totalISR DECIMAL(18,2) = 0,
    @totalIVA DECIMAL(18,2) = 0,
    @idEstadoCuenta INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
            INSERT INTO [dbo].[EstadoCuenta] (
                numeroCuenta, clabe, tipoCuenta, tipoMoneda,
                fechaInicio, fechaFin, fechaCorte,
                saldoInicial, totalCargos, totalAbonos, saldoFinal,
                totalComisiones, totalISR, totalIVA
            )
            VALUES (
                @numeroCuenta, @clabe, @tipoCuenta, @tipoMoneda,
                @fechaInicio, @fechaFin, @fechaCorte,
                @saldoInicial, @totalCargos, @totalAbonos, @saldoFinal,
                @totalComisiones, @totalISR, @totalIVA
            )
            
            SET @idEstadoCuenta = SCOPE_IDENTITY()
        COMMIT TRANSACTION
        
        SELECT @idEstadoCuenta AS idEstadoCuenta, 'Estado de cuenta creado exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
GO

-- Editar Estado de Cuenta
CREATE OR ALTER PROCEDURE [dbo].[sp_EditarEstadoCuenta]
    @idEstadoCuenta INT,
    @numeroCuenta NVARCHAR(50) = NULL,
    @clabe NVARCHAR(18) = NULL,
    @tipoCuenta NVARCHAR(100) = NULL,
    @tipoMoneda NVARCHAR(50) = NULL,
    @fechaInicio DATE = NULL,
    @fechaFin DATE = NULL,
    @fechaCorte DATE = NULL,
    @saldoInicial DECIMAL(18,2) = NULL,
    @totalCargos DECIMAL(18,2) = NULL,
    @totalAbonos DECIMAL(18,2) = NULL,
    @saldoFinal DECIMAL(18,2) = NULL,
    @totalComisiones DECIMAL(18,2) = NULL,
    @totalISR DECIMAL(18,2) = NULL,
    @totalIVA DECIMAL(18,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
            UPDATE [dbo].[EstadoCuenta]
            SET 
                numeroCuenta = ISNULL(@numeroCuenta, numeroCuenta),
                clabe = ISNULL(@clabe, clabe),
                tipoCuenta = ISNULL(@tipoCuenta, tipoCuenta),
                tipoMoneda = ISNULL(@tipoMoneda, tipoMoneda),
                fechaInicio = ISNULL(@fechaInicio, fechaInicio),
                fechaFin = ISNULL(@fechaFin, fechaFin),
                fechaCorte = ISNULL(@fechaCorte, fechaCorte),
                saldoInicial = ISNULL(@saldoInicial, saldoInicial),
                totalCargos = ISNULL(@totalCargos, totalCargos),
                totalAbonos = ISNULL(@totalAbonos, totalAbonos),
                saldoFinal = ISNULL(@saldoFinal, saldoFinal),
                totalComisiones = ISNULL(@totalComisiones, totalComisiones),
                totalISR = ISNULL(@totalISR, totalISR),
                totalIVA = ISNULL(@totalIVA, totalIVA)
            WHERE idEstadoCuenta = @idEstadoCuenta
            
            IF @@ROWCOUNT = 0
                RAISERROR('No se encontró el estado de cuenta con el ID especificado', 16, 1)
        COMMIT TRANSACTION
        
        SELECT 'Estado de cuenta actualizado exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
GO

-- Consultar Estado de Cuenta por ID
CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarEstadoCuenta]
    @idEstadoCuenta INT = NULL,
    @numeroCuenta NVARCHAR(50) = NULL,
    @fechaInicio DATE = NULL,
    @fechaFin DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        idEstadoCuenta,
        numeroCuenta,
        clabe,
        tipoCuenta,
        tipoMoneda,
        fechaInicio,
        fechaFin,
        fechaCorte,
        saldoInicial,
        totalCargos,
        totalAbonos,
        saldoFinal,
        totalComisiones,
        totalISR,
        totalIVA,
        fechaCarga
    FROM [dbo].[EstadoCuenta]
    WHERE 
        (@idEstadoCuenta IS NULL OR idEstadoCuenta = @idEstadoCuenta)
        AND (@numeroCuenta IS NULL OR numeroCuenta = @numeroCuenta)
        AND (@fechaInicio IS NULL OR fechaCorte >= @fechaInicio)
        AND (@fechaFin IS NULL OR fechaCorte <= @fechaFin)
    ORDER BY fechaCorte DESC
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA BANCO
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[sp_CrearBanco]
    @nombreBanco NVARCHAR(200),
    @rfc NVARCHAR(13),
    @nombreSucursal NVARCHAR(200) = NULL,
    @direccion NVARCHAR(500) = NULL,
    @idBanco INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[Banco] (nombreBanco, rfc, nombreSucursal, direccion)
        VALUES (@nombreBanco, @rfc, @nombreSucursal, @direccion)
        
        SET @idBanco = SCOPE_IDENTITY()
        SELECT @idBanco AS idBanco, 'Banco creado exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarBanco]
    @idBanco INT = NULL,
    @rfc NVARCHAR(13) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT idBanco, nombreBanco, rfc, nombreSucursal, direccion
    FROM [dbo].[Banco]
    WHERE 
        (@idBanco IS NULL OR idBanco = @idBanco)
        AND (@rfc IS NULL OR rfc = @rfc)
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA CUENTA HABIENTE
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[sp_CrearCuentaHabiente]
    @nombre NVARCHAR(200),
    @rfc NVARCHAR(13),
    @numeroCuenta NVARCHAR(50),
    @direccion NVARCHAR(500) = NULL,
    @idCuentaHabiente INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[CuentaHabiente] (nombre, rfc, numeroCuenta, direccion)
        VALUES (@nombre, @rfc, @numeroCuenta, @direccion)
        
        SET @idCuentaHabiente = SCOPE_IDENTITY()
        SELECT @idCuentaHabiente AS idCuentaHabiente, 'Cuenta habiente creado exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarCuentaHabiente]
    @idCuentaHabiente INT = NULL,
    @numeroCuenta NVARCHAR(50) = NULL,
    @rfc NVARCHAR(13) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT idCuentaHabiente, nombre, rfc, numeroCuenta, direccion
    FROM [dbo].[CuentaHabiente]
    WHERE 
        (@idCuentaHabiente IS NULL OR idCuentaHabiente = @idCuentaHabiente)
        AND (@numeroCuenta IS NULL OR numeroCuenta = @numeroCuenta)
        AND (@rfc IS NULL OR rfc = @rfc)
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA MOVIMIENTO
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[sp_CrearMovimiento]
    @idEstadoCuenta INT,
    @fecha DATE,
    @descripcion NVARCHAR(MAX),
    @referencia NVARCHAR(100) = NULL,
    @cargo DECIMAL(18,2) = NULL,
    @abono DECIMAL(18,2) = NULL,
    @saldo DECIMAL(18,2),
    @tipoOperacion NVARCHAR(50) = NULL,
    @idMovimiento INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
            -- Validar que el estado de cuenta existe
            IF NOT EXISTS (SELECT 1 FROM [dbo].[EstadoCuenta] WHERE idEstadoCuenta = @idEstadoCuenta)
                RAISERROR('El estado de cuenta especificado no existe', 16, 1)
            
            INSERT INTO [dbo].[Movimiento] (
                idEstadoCuenta, fecha, descripcion, referencia,
                cargo, abono, saldo, tipoOperacion
            )
            VALUES (
                @idEstadoCuenta, @fecha, @descripcion, @referencia,
                @cargo, @abono, @saldo, @tipoOperacion
            )
            
            SET @idMovimiento = SCOPE_IDENTITY()
        COMMIT TRANSACTION
        
        SELECT @idMovimiento AS idMovimiento, 'Movimiento creado exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_EditarMovimiento]
    @idMovimiento INT,
    @fecha DATE = NULL,
    @descripcion NVARCHAR(MAX) = NULL,
    @referencia NVARCHAR(100) = NULL,
    @cargo DECIMAL(18,2) = NULL,
    @abono DECIMAL(18,2) = NULL,
    @saldo DECIMAL(18,2) = NULL,
    @tipoOperacion NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
            UPDATE [dbo].[Movimiento]
            SET 
                fecha = ISNULL(@fecha, fecha),
                descripcion = ISNULL(@descripcion, descripcion),
                referencia = ISNULL(@referencia, referencia),
                cargo = ISNULL(@cargo, cargo),
                abono = ISNULL(@abono, abono),
                saldo = ISNULL(@saldo, saldo),
                tipoOperacion = ISNULL(@tipoOperacion, tipoOperacion)
            WHERE idMovimiento = @idMovimiento
            
            IF @@ROWCOUNT = 0
                RAISERROR('No se encontró el movimiento con el ID especificado', 16, 1)
        COMMIT TRANSACTION
        
        SELECT 'Movimiento actualizado exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarMovimientos]
    @idEstadoCuenta INT = NULL,
    @fechaInicio DATE = NULL,
    @fechaFin DATE = NULL,
    @tipoOperacion NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.idMovimiento,
        m.idEstadoCuenta,
        m.fecha,
        m.descripcion,
        m.referencia,
        m.cargo,
        m.abono,
        m.saldo,
        m.tipoOperacion,
        m.fechaCarga,
        ec.numeroCuenta,
        ec.clabe
    FROM [dbo].[Movimiento] m
    INNER JOIN [dbo].[EstadoCuenta] ec ON m.idEstadoCuenta = ec.idEstadoCuenta
    WHERE 
        (@idEstadoCuenta IS NULL OR m.idEstadoCuenta = @idEstadoCuenta)
        AND (@fechaInicio IS NULL OR m.fecha >= @fechaInicio)
        AND (@fechaFin IS NULL OR m.fecha <= @fechaFin)
        AND (@tipoOperacion IS NULL OR m.tipoOperacion = @tipoOperacion)
    ORDER BY m.fecha DESC, m.idMovimiento DESC
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA TRANSFERENCIA SPEI
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[sp_CrearTransferenciaSPEI]
    @idMovimiento INT,
    @tipoTransferencia NVARCHAR(20),
    @bancoClave NVARCHAR(10) = NULL,
    @bancoNombre NVARCHAR(200) = NULL,
    @cuentaOrigen NVARCHAR(50) = NULL,
    @cuentaDestino NVARCHAR(50) = NULL,
    @nombreEmisor NVARCHAR(200) = NULL,
    @nombreDestinatario NVARCHAR(200) = NULL,
    @rfcEmisor NVARCHAR(13) = NULL,
    @rfcDestinatario NVARCHAR(13) = NULL,
    @claveRastreo NVARCHAR(100) = NULL,
    @concepto NVARCHAR(MAX) = NULL,
    @hora TIME = NULL,
    @monto DECIMAL(18,2),
    @idTransferencia INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
            -- Validar que el movimiento existe
            IF NOT EXISTS (SELECT 1 FROM [dbo].[Movimiento] WHERE idMovimiento = @idMovimiento)
                RAISERROR('El movimiento especificado no existe', 16, 1)
            
            -- Validar tipo de transferencia
            IF @tipoTransferencia NOT IN ('ENVIADA', 'RECIBIDA')
                RAISERROR('El tipo de transferencia debe ser ENVIADA o RECIBIDA', 16, 1)
            
            INSERT INTO [dbo].[TransferenciaSPEI] (
                idMovimiento, tipoTransferencia, bancoClave, bancoNombre,
                cuentaOrigen, cuentaDestino, nombreEmisor, nombreDestinatario,
                rfcEmisor, rfcDestinatario, claveRastreo, concepto, hora, monto
            )
            VALUES (
                @idMovimiento, @tipoTransferencia, @bancoClave, @bancoNombre,
                @cuentaOrigen, @cuentaDestino, @nombreEmisor, @nombreDestinatario,
                @rfcEmisor, @rfcDestinatario, @claveRastreo, @concepto, @hora, @monto
            )
            
            SET @idTransferencia = SCOPE_IDENTITY()
        COMMIT TRANSACTION
        
        SELECT @idTransferencia AS idTransferencia, 'Transferencia SPEI creada exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarTransferenciasSPEI]
    @idMovimiento INT = NULL,
    @tipoTransferencia NVARCHAR(20) = NULL,
    @claveRastreo NVARCHAR(100) = NULL,
    @rfcEmisor NVARCHAR(13) = NULL,
    @rfcDestinatario NVARCHAR(13) = NULL,
    @fechaInicio DATE = NULL,
    @fechaFin DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ts.idTransferencia,
        ts.idMovimiento,
        ts.tipoTransferencia,
        ts.bancoClave,
        ts.bancoNombre,
        ts.cuentaOrigen,
        ts.cuentaDestino,
        ts.nombreEmisor,
        ts.nombreDestinatario,
        ts.rfcEmisor,
        ts.rfcDestinatario,
        ts.claveRastreo,
        ts.concepto,
        ts.hora,
        ts.monto,
        m.fecha,
        m.referencia
    FROM [dbo].[TransferenciaSPEI] ts
    INNER JOIN [dbo].[Movimiento] m ON ts.idMovimiento = m.idMovimiento
    WHERE 
        (@idMovimiento IS NULL OR ts.idMovimiento = @idMovimiento)
        AND (@tipoTransferencia IS NULL OR ts.tipoTransferencia = @tipoTransferencia)
        AND (@claveRastreo IS NULL OR ts.claveRastreo = @claveRastreo)
        AND (@rfcEmisor IS NULL OR ts.rfcEmisor = @rfcEmisor)
        AND (@rfcDestinatario IS NULL OR ts.rfcDestinatario = @rfcDestinatario)
        AND (@fechaInicio IS NULL OR m.fecha >= @fechaInicio)
        AND (@fechaFin IS NULL OR m.fecha <= @fechaFin)
    ORDER BY m.fecha DESC
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA IMPUESTO MOVIMIENTO
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[sp_CrearImpuestoMovimiento]
    @idMovimiento INT,
    @tipoImpuesto NVARCHAR(20),
    @rfc NVARCHAR(13) = NULL,
    @monto DECIMAL(18,2),
    @idImpuesto INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
            -- Validar que el movimiento existe
            IF NOT EXISTS (SELECT 1 FROM [dbo].[Movimiento] WHERE idMovimiento = @idMovimiento)
                RAISERROR('El movimiento especificado no existe', 16, 1)
            
            INSERT INTO [dbo].[ImpuestoMovimiento] (idMovimiento, tipoImpuesto, rfc, monto)
            VALUES (@idMovimiento, @tipoImpuesto, @rfc, @monto)
            
            SET @idImpuesto = SCOPE_IDENTITY()
        COMMIT TRANSACTION
        
        SELECT @idImpuesto AS idImpuesto, 'Impuesto creado exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarImpuestosMovimiento]
    @idMovimiento INT = NULL,
    @tipoImpuesto NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        im.idImpuesto,
        im.idMovimiento,
        im.tipoImpuesto,
        im.rfc,
        im.monto,
        m.fecha,
        m.descripcion
    FROM [dbo].[ImpuestoMovimiento] im
    INNER JOIN [dbo].[Movimiento] m ON im.idMovimiento = m.idMovimiento
    WHERE 
        (@idMovimiento IS NULL OR im.idMovimiento = @idMovimiento)
        AND (@tipoImpuesto IS NULL OR im.tipoImpuesto = @tipoImpuesto)
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA COMISION BANCARIA
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[sp_CrearComisionBancaria]
    @idMovimiento INT,
    @tipoComision NVARCHAR(100),
    @monto DECIMAL(18,2),
    @iva DECIMAL(18,2) = NULL,
    @referencia NVARCHAR(100) = NULL,
    @idComision INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
            -- Validar que el movimiento existe
            IF NOT EXISTS (SELECT 1 FROM [dbo].[Movimiento] WHERE idMovimiento = @idMovimiento)
                RAISERROR('El movimiento especificado no existe', 16, 1)
            
            INSERT INTO [dbo].[ComisionBancaria] (idMovimiento, tipoComision, monto, iva, referencia)
            VALUES (@idMovimiento, @tipoComision, @monto, @iva, @referencia)
            
            SET @idComision = SCOPE_IDENTITY()
        COMMIT TRANSACTION
        
        SELECT @idComision AS idComision, 'Comisión bancaria creada exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarComisionesBancarias]
    @idMovimiento INT = NULL,
    @tipoComision NVARCHAR(100) = NULL,
    @fechaInicio DATE = NULL,
    @fechaFin DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        cb.idComision,
        cb.idMovimiento,
        cb.tipoComision,
        cb.monto,
        cb.iva,
        cb.referencia,
        m.fecha,
        m.descripcion
    FROM [dbo].[ComisionBancaria] cb
    INNER JOIN [dbo].[Movimiento] m ON cb.idMovimiento = m.idMovimiento
    WHERE 
        (@idMovimiento IS NULL OR cb.idMovimiento = @idMovimiento)
        AND (@tipoComision IS NULL OR cb.tipoComision = @tipoComision)
        AND (@fechaInicio IS NULL OR m.fecha >= @fechaInicio)
        AND (@fechaFin IS NULL OR m.fecha <= @fechaFin)
    ORDER BY m.fecha DESC
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA PAGO SERVICIO
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[sp_CrearPagoServicio]
    @idMovimiento INT,
    @tipoServicio NVARCHAR(50),
    @referencia NVARCHAR(100) = NULL,
    @monto DECIMAL(18,2),
    @idPago INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
            -- Validar que el movimiento existe
            IF NOT EXISTS (SELECT 1 FROM [dbo].[Movimiento] WHERE idMovimiento = @idMovimiento)
                RAISERROR('El movimiento especificado no existe', 16, 1)
            
            INSERT INTO [dbo].[PagoServicio] (idMovimiento, tipoServicio, referencia, monto)
            VALUES (@idMovimiento, @tipoServicio, @referencia, @monto)
            
            SET @idPago = SCOPE_IDENTITY()
        COMMIT TRANSACTION
        
        SELECT @idPago AS idPago, 'Pago de servicio creado exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarPagosServicio]
    @idMovimiento INT = NULL,
    @tipoServicio NVARCHAR(50) = NULL,
    @fechaInicio DATE = NULL,
    @fechaFin DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ps.idPago,
        ps.idMovimiento,
        ps.tipoServicio,
        ps.referencia,
        ps.monto,
        m.fecha,
        m.descripcion
    FROM [dbo].[PagoServicio] ps
    INNER JOIN [dbo].[Movimiento] m ON ps.idMovimiento = m.idMovimiento
    WHERE 
        (@idMovimiento IS NULL OR ps.idMovimiento = @idMovimiento)
        AND (@tipoServicio IS NULL OR ps.tipoServicio = @tipoServicio)
        AND (@fechaInicio IS NULL OR m.fecha >= @fechaInicio)
        AND (@fechaFin IS NULL OR m.fecha <= @fechaFin)
    ORDER BY m.fecha DESC
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA DEPOSITO
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[sp_CrearDeposito]
    @idMovimiento INT,
    @tipoDeposito NVARCHAR(50),
    @referencia NVARCHAR(100) = NULL,
    @monto DECIMAL(18,2),
    @idDeposito INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
            -- Validar que el movimiento existe
            IF NOT EXISTS (SELECT 1 FROM [dbo].[Movimiento] WHERE idMovimiento = @idMovimiento)
                RAISERROR('El movimiento especificado no existe', 16, 1)
            
            INSERT INTO [dbo].[Deposito] (idMovimiento, tipoDeposito, referencia, monto)
            VALUES (@idMovimiento, @tipoDeposito, @referencia, @monto)
            
            SET @idDeposito = SCOPE_IDENTITY()
        COMMIT TRANSACTION
        
        SELECT @idDeposito AS idDeposito, 'Depósito creado exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarDepositos]
    @idMovimiento INT = NULL,
    @tipoDeposito NVARCHAR(50) = NULL,
    @fechaInicio DATE = NULL,
    @fechaFin DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        d.idDeposito,
        d.idMovimiento,
        d.tipoDeposito,
        d.referencia,
        d.monto,
        m.fecha,
        m.descripcion
    FROM [dbo].[Deposito] d
    INNER JOIN [dbo].[Movimiento] m ON d.idMovimiento = m.idMovimiento
    WHERE 
        (@idMovimiento IS NULL OR d.idMovimiento = @idMovimiento)
        AND (@tipoDeposito IS NULL OR d.tipoDeposito = @tipoDeposito)
        AND (@fechaInicio IS NULL OR m.fecha >= @fechaInicio)
        AND (@fechaFin IS NULL OR m.fecha <= @fechaFin)
    ORDER BY m.fecha DESC
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA TIMBRE FISCAL
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[sp_CrearTimbreFiscal]
    @idEstadoCuenta INT,
    @uuid NVARCHAR(50),
    @fechaTimbrado DATETIME,
    @numeroProveedor NVARCHAR(50) = NULL,
    @idTimbre INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
            -- Validar que el estado de cuenta existe
            IF NOT EXISTS (SELECT 1 FROM [dbo].[EstadoCuenta] WHERE idEstadoCuenta = @idEstadoCuenta)
                RAISERROR('El estado de cuenta especificado no existe', 16, 1)
            
            INSERT INTO [dbo].[TimbreFiscal] (idEstadoCuenta, uuid, fechaTimbrado, numeroProveedor)
            VALUES (@idEstadoCuenta, @uuid, @fechaTimbrado, @numeroProveedor)
            
            SET @idTimbre = SCOPE_IDENTITY()
        COMMIT TRANSACTION
        
        SELECT @idTimbre AS idTimbre, 'Timbre fiscal creado exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarTimbresFiscales]
    @idEstadoCuenta INT = NULL,
    @uuid NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        tf.idTimbre,
        tf.idEstadoCuenta,
        tf.uuid,
        tf.fechaTimbrado,
        tf.numeroProveedor,
        ec.numeroCuenta,
        ec.fechaCorte
    FROM [dbo].[TimbreFiscal] tf
    INNER JOIN [dbo].[EstadoCuenta] ec ON tf.idEstadoCuenta = ec.idEstadoCuenta
    WHERE 
        (@idEstadoCuenta IS NULL OR tf.idEstadoCuenta = @idEstadoCuenta)
        AND (@uuid IS NULL OR tf.uuid = @uuid)
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA COMPLEMENTO FISCAL
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[sp_CrearComplementoFiscal]
    @idEstadoCuenta INT,
    @rfc NVARCHAR(13),
    @formaPago NVARCHAR(10) = NULL,
    @metodoPago NVARCHAR(10) = NULL,
    @usoCFDI NVARCHAR(10) = NULL,
    @claveProducto NVARCHAR(20) = NULL,
    @codigoPostal NVARCHAR(10) = NULL,
    @idComplemento INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
            -- Validar que el estado de cuenta existe
            IF NOT EXISTS (SELECT 1 FROM [dbo].[EstadoCuenta] WHERE idEstadoCuenta = @idEstadoCuenta)
                RAISERROR('El estado de cuenta especificado no existe', 16, 1)
            
            INSERT INTO [dbo].[ComplementoFiscal] (
                idEstadoCuenta, rfc, formaPago, metodoPago, 
                usoCFDI, claveProducto, codigoPostal
            )
            VALUES (
                @idEstadoCuenta, @rfc, @formaPago, @metodoPago,
                @usoCFDI, @claveProducto, @codigoPostal
            )
            
            SET @idComplemento = SCOPE_IDENTITY()
        COMMIT TRANSACTION
        
        SELECT @idComplemento AS idComplemento, 'Complemento fiscal creado exitosamente' AS Mensaje
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarComplementosFiscales]
    @idEstadoCuenta INT = NULL,
    @rfc NVARCHAR(13) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        cf.idComplemento,
        cf.idEstadoCuenta,
        cf.rfc,
        cf.formaPago,
        cf.metodoPago,
        cf.usoCFDI,
        cf.claveProducto,
        cf.codigoPostal,
        ec.numeroCuenta,
        ec.fechaCorte
    FROM [dbo].[ComplementoFiscal] cf
    INNER JOIN [dbo].[EstadoCuenta] ec ON cf.idEstadoCuenta = ec.idEstadoCuenta
    WHERE 
        (@idEstadoCuenta IS NULL OR cf.idEstadoCuenta = @idEstadoCuenta)
        AND (@rfc IS NULL OR cf.rfc = @rfc)
END
GO

-- =============================================
-- PROCEDIMIENTOS DE CONSULTAS ESPECIALES
-- =============================================

-- Consultar Estado de Cuenta Completo con Movimientos
CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarEstadoCuentaCompleto]
    @idEstadoCuenta INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Estado de Cuenta
    SELECT 
        idEstadoCuenta,
        numeroCuenta,
        clabe,
        tipoCuenta,
        tipoMoneda,
        fechaInicio,
        fechaFin,
        fechaCorte,
        saldoInicial,
        totalCargos,
        totalAbonos,
        saldoFinal,
        totalComisiones,
        totalISR,
        totalIVA,
        fechaCarga
    FROM [dbo].[EstadoCuenta]
    WHERE idEstadoCuenta = @idEstadoCuenta
    
    -- Movimientos
    SELECT 
        idMovimiento,
        fecha,
        descripcion,
        referencia,
        cargo,
        abono,
        saldo,
        tipoOperacion
    FROM [dbo].[Movimiento]
    WHERE idEstadoCuenta = @idEstadoCuenta
    ORDER BY fecha, idMovimiento
    
    -- Transferencias SPEI
    SELECT 
        ts.*
    FROM [dbo].[TransferenciaSPEI] ts
    INNER JOIN [dbo].[Movimiento] m ON ts.idMovimiento = m.idMovimiento
    WHERE m.idEstadoCuenta = @idEstadoCuenta
    
    -- Comisiones
    SELECT 
        cb.*
    FROM [dbo].[ComisionBancaria] cb
    INNER JOIN [dbo].[Movimiento] m ON cb.idMovimiento = m.idMovimiento
    WHERE m.idEstadoCuenta = @idEstadoCuenta
    
    -- Impuestos
    SELECT 
        im.*
    FROM [dbo].[ImpuestoMovimiento] im
    INNER JOIN [dbo].[Movimiento] m ON im.idMovimiento = m.idMovimiento
    WHERE m.idEstadoCuenta = @idEstadoCuenta
END
GO

-- Resumen de Movimientos por Tipo
CREATE OR ALTER PROCEDURE [dbo].[sp_ResumenMovimientosPorTipo]
    @idEstadoCuenta INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        tipoOperacion,
        COUNT(*) AS TotalMovimientos,
        SUM(ISNULL(cargo, 0)) AS TotalCargos,
        SUM(ISNULL(abono, 0)) AS TotalAbonos
    FROM [dbo].[Movimiento]
    WHERE idEstadoCuenta = @idEstadoCuenta
    GROUP BY tipoOperacion
    ORDER BY tipoOperacion
END
GO

-- Consultar Transferencias SPEI por RFC
CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultarTransferenciasPorRFC]
    @rfc NVARCHAR(13),
    @tipoTransferencia NVARCHAR(20) = NULL, -- 'ENVIADA', 'RECIBIDA', NULL (ambas)
    @fechaInicio DATE = NULL,
    @fechaFin DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ts.idTransferencia,
        ts.tipoTransferencia,
        ts.bancoClave,
        ts.bancoNombre,
        ts.cuentaOrigen,
        ts.cuentaDestino,
        ts.nombreEmisor,
        ts.nombreDestinatario,
        ts.rfcEmisor,
        ts.rfcDestinatario,
        ts.claveRastreo,
        ts.concepto,
        ts.monto,
        m.fecha,
        m.referencia
    FROM [dbo].[TransferenciaSPEI] ts
    INNER JOIN [dbo].[Movimiento] m ON ts.idMovimiento = m.idMovimiento
    WHERE 
        (ts.rfcEmisor = @rfc OR ts.rfcDestinatario = @rfc)
        AND (@tipoTransferencia IS NULL OR ts.tipoTransferencia = @tipoTransferencia)
        AND (@fechaInicio IS NULL OR m.fecha >= @fechaInicio)
        AND (@fechaFin IS NULL OR m.fecha <= @fechaFin)
    ORDER BY m.fecha DESC
END
GO

PRINT 'Todos los procedimientos almacenados han sido creados exitosamente'
GO