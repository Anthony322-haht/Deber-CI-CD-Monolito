USE Monolito4am;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_imagen_producto')
BEGIN
    CREATE TABLE tbl_imagen_producto (
        imgp_id INT IDENTITY(1,1) PRIMARY KEY,
        pro_id INT NOT NULL,
        imgp_ruta VARCHAR(500) NOT NULL,
        imgp_nombre VARCHAR(200),
        imgp_orden INT DEFAULT 0,
        imgp_estado CHAR(1) DEFAULT 'A',
        FOREIGN KEY (pro_id) REFERENCES tbl_producto(pro_id)
    );
END
GO
