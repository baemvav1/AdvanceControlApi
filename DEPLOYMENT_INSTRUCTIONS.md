# Deployment Instructions
## sp_relacionProveedorRefaccion_edit Update

### Quick Summary
This deployment updates the stored procedure `sp_relacionProveedorRefaccion_edit` with corrected validation logic. **No application code changes are required.**

### Pre-Deployment Checklist
- [ ] Review `SP_COMPATIBILITY_ANALYSIS.md` for complete technical details
- [ ] Backup current stored procedure
- [ ] Ensure database maintenance window is scheduled (if required)
- [ ] Have rollback plan ready

### Deployment Steps

#### 1. Backup Current Stored Procedure
```sql
-- Save the current stored procedure definition
USE [Advance]
GO

-- Generate script of current version for backup
SELECT OBJECT_DEFINITION(OBJECT_ID('dbo.sp_relacionProveedorRefaccion_edit'))

-- Or use:
sp_helptext 'sp_relacionProveedorRefaccion_edit'
```

Save the output to a backup file with timestamp (e.g., `sp_relacionProveedorRefaccion_edit_backup_20251214.sql`)

#### 2. Deploy New Stored Procedure
```sql
-- Execute the updated stored procedure script
-- File: sp_relacionProveedorRefaccion_edit.sql
USE [Advance]
GO

-- Run the ALTER PROCEDURE statement from the file
-- (Copy entire contents of sp_relacionProveedorRefaccion_edit.sql)
```

#### 3. Verify Deployment

Run these test queries to confirm the stored procedure works correctly:

```sql
-- Test 1: Select operation (should return refacciones)
EXEC [sp_relacionProveedorRefaccion_edit] 'select',0,0,0,null,0.0

-- Test 2: Invalid ID check (should return "Id Invalido")
EXEC [sp_relacionProveedorRefaccion_edit] 'put',0,0,0,null,0.0

-- Test 3: Update nota with null (should succeed)
-- Note: Use a valid idRelacionProveedor from your database
-- EXEC [sp_relacionProveedorRefaccion_edit] 'update_nota',1,0,0,null,0.0
```

#### 4. Verify Application Functionality

After deploying the stored procedure, verify the API endpoints work correctly:

**Test GET Endpoint:**
```bash
GET /api/RelacionProveedorRefaccion?idProveedor=1
```

**Test POST Endpoint:**
```bash
POST /api/RelacionProveedorRefaccion?idProveedor=1&idRefaccion=1&precio=100.0&nota=Test
```

**Test PUT Endpoints:**
```bash
PUT /api/RelacionProveedorRefaccion/nota?idRelacionProveedor=1&nota=Updated
PUT /api/RelacionProveedorRefaccion/precio?idRelacionProveedor=1&precio=150.0
```

**Test DELETE Endpoint:**
```bash
DELETE /api/RelacionProveedorRefaccion?idRelacionProveedor=1
```

### Rollback Plan

If issues are discovered after deployment:

```sql
-- Restore the previous version from backup
USE [Advance]
GO

-- Execute the backup script created in step 1
-- (Contents of sp_relacionProveedorRefaccion_edit_backup_YYYYMMDD.sql)
```

### What Changed

#### Fixed Issues:
1. **Type Mismatches**: Changed string comparisons (`''`) to proper integer comparisons (`0`) for int parameters
2. **Nota Filter Logic**: Improved from `rpr.nota = ISNULL(@nota,rpr.nota)` to `(@nota IS NULL OR rpr.nota = @nota)` for clarity
3. **Keyword Consistency**: Changed "And" to "AND" for consistency

#### Operations Affected:
- ✅ `select` - Improved nota filtering logic
- ✅ `put` - Fixed idProveedor validation
- ✅ `delete` - No changes (already correct)
- ✅ `update_nota` - Fixed idRelacionProveedor validation, allows null notes
- ✅ `update_precio` - Fixed idRelacionProveedor validation

### Application Code Changes
**None required.** The existing controller and service code are fully compatible with the updated stored procedure.

### Post-Deployment

- [ ] Monitor application logs for any SQL errors
- [ ] Verify no unexpected behavior in the RelacionProveedorRefaccion functionality
- [ ] Confirm all API endpoints respond correctly
- [ ] Keep backup file for at least 30 days

### Support

For detailed technical analysis, see:
- `SP_COMPATIBILITY_ANALYSIS.md` - Complete compatibility analysis
- `sp_relacionProveedorRefaccion_edit.sql` - Updated stored procedure

For issues or questions:
- Check application logs in `/var/log/` or application logging system
- Review controller: `AdvanceApi/Controllers/RelacionProveedorRefaccionController.cs`
- Review service: `AdvanceApi/Services/RelacionProveedorRefaccionService.cs`

### Notes

- The stored procedure maintains backward compatibility
- All parameter names and types remain unchanged
- All error messages remain unchanged ("Id Invalido", "Ya existe la relacion")
- The validation logic now correctly matches the data types
- The C# application does not need to be redeployed
