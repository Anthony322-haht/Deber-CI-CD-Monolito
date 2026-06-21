USE Monolito4am;
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_producto') AND name = 'cat_id')
BEGIN
    ALTER TABLE tbl_producto ADD cat_id INT NULL;
    ALTER TABLE tbl_producto ADD CONSTRAINT FK_producto_categoria FOREIGN KEY (cat_id) REFERENCES tbl_categoria(cat_id);
END
GO

UPDATE tbl_producto SET cat_id = 4 WHERE cat_id IS NULL;
GO
