USE [Advance]
GO
/****** Object:  StoredProcedure [dbo].[sp_relacionProveedorRefaccion_edit]    Script Date: 14/12/2025 01:38:45 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Corrected version with proper type validations
-- Compatible with RelacionProveedorRefaccionController

ALTER PROCEDURE [dbo].[sp_relacionProveedorRefaccion_edit]
    @operacion   nvarchar(100),
    @idRelacionProveedor int = 0,
    @idProveedor   int = 0,
    @idRefaccion int = 0,
    @nota nvarchar(max) = NULL,
    @precio float = 0.0
AS
BEGIN
    SET NOCOUNT ON;

    IF @operacion = 'select' 
    BEGIN
        select r.idRefaccion, r.marca, r.serie, r.costo, r.descripcion 
        from relacionProveedorRefaccion rpr 
        left join refaccion r on rpr.idRefaccion = r.idRefaccion
        left join proveedor p on p.idProveedor = rpr.idProveedor
        where r.estatus = 1 
        AND (
                @idProveedor = 0
                OR ISNULL(rpr.idProveedor, 0) = @idProveedor
              )
        AND (
                @idRefaccion = 0
                OR ISNULL(rpr.idRefaccion, 0) = @idRefaccion
              )
       
        and rpr.nota = ISNULL(@nota,rpr.nota) and rpr.estatus = 1
              
    END

    IF @operacion = 'delete' 
    BEGIN
        -- Fixed: Use proper int comparison instead of string comparison
        IF ISNULL(@idRelacionProveedor,0) != 0 
        BEGIN
            UPDATE relacionProveedorRefaccion 
            SET estatus = 0 
            WHERE idRelacionProveedor = @idRelacionProveedor
        END
        ELSE 
        BEGIN
            SELECT 'Id Invalido' AS Result;
        END
    END

    IF @operacion = 'put' 
    BEGIN
        -- Fixed: Use proper int comparison instead of string comparison
        IF ISNULL(@idProveedor,0) != 0 AND ISNULL(@idRefaccion,0) <> 0 And ISNULL(@precio,0.0) <> 0
        BEGIN
            IF NOT EXISTS (SELECT * FROM relacionProveedorRefaccion WHERE idProveedor = @idProveedor AND idRefaccion = @idRefaccion) 
            BEGIN
                INSERT INTO relacionProveedorRefaccion(idProveedor, idRefaccion, precio, nota, estatus)
                VALUES (@idProveedor,@idRefaccion,@precio,@nota,1);
            END
            ELSE
            BEGIN
                SELECT 'Ya existe la relacion' AS Result;
            END
        END
        ELSE 
        BEGIN
            SELECT 'Id Invalido' AS Result;
        END
    END

    IF @operacion = 'update_nota' 
    BEGIN
        -- Fixed: Use proper int comparison and allow null/empty notes
        -- The controller allows nota to be null to clear the note
        IF ISNULL(@idRelacionProveedor,0) != 0
        BEGIN
            UPDATE relacionProveedorRefaccion 
            SET nota = @nota 
            WHERE idRelacionProveedor = @idRelacionProveedor;
        END
        ELSE 
        BEGIN
            SELECT 'Id Invalido' AS Result;
        END
    END

    IF @operacion = 'update_precio' 
    BEGIN
        -- Fixed: Use proper int comparison instead of string comparison
        IF ISNULL(@idRelacionProveedor,0) != 0 AND ISNULL(@precio,0.0) <> 0.0
        BEGIN
            UPDATE relacionProveedorRefaccion 
            SET precio = @precio 
            WHERE idRelacionProveedor = @idRelacionProveedor;
        END
        ELSE 
        BEGIN
            SELECT 'Id Invalido' AS Result;
        END
    END
END

GO

-- Test queries
-- exec [sp_relacionProveedorRefaccion_edit] 'select',0,0,0,null,0.0
-- exec [sp_relacionProveedorRefaccion_edit] 'put',0,1,1,'Test note',100.0
-- exec [sp_relacionProveedorRefaccion_edit] 'update_nota',1,0,0,'Updated note',0.0
-- exec [sp_relacionProveedorRefaccion_edit] 'update_nota',1,0,0,null,0.0  -- Clear note
-- exec [sp_relacionProveedorRefaccion_edit] 'update_precio',1,0,0,null,150.0
-- exec [sp_relacionProveedorRefaccion_edit] 'delete',1,0,0,null,0.0
