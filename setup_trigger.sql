USE db_aca15c_dbaca15cmonolito;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbl_proveedor_eliminado' AND xtype='U')
BEGIN
    CREATE TABLE tbl_proveedor_eliminado (
        prov_id INT PRIMARY KEY,
        prov_nombre VARCHAR(100) NOT NULL
    )
END
GO

IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'trg_RespaldarProveedor')
    DROP TRIGGER trg_RespaldarProveedor;
GO

CREATE OR ALTER TRIGGER trg_RespaldarProveedor
ON tbl_proveedor
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_proveedor_eliminado (prov_id, prov_nombre)
    SELECT prov_id, prov_nombre 
    FROM DELETED d
    WHERE NOT EXISTS (
        SELECT 1 FROM tbl_proveedor_eliminado e WHERE e.prov_id = d.prov_id
    );
END
GO
