-- =============================================
-- Script de datos de ejemplo para Monolito4am
-- Base de datos: Monolito4am
-- Tablas: tbl_proveedor, tbl_producto
-- =============================================

USE Monolito4am;
GO

-- =============================================
-- 1. Insertar Proveedores
-- =============================================
INSERT INTO tbl_proveedor (prov_nombre, prov_estado) VALUES ('Distribuidora Nacional S.A.', 'A');
INSERT INTO tbl_proveedor (prov_nombre, prov_estado) VALUES ('Tech Global Corp', 'A');
INSERT INTO tbl_proveedor (prov_nombre, prov_estado) VALUES ('Alimentos del Valle', 'A');
INSERT INTO tbl_proveedor (prov_nombre, prov_estado) VALUES ('Importadora Pacific', 'A');
INSERT INTO tbl_proveedor (prov_nombre, prov_estado) VALUES ('Suministros Express', 'A');
GO

-- =============================================
-- 2. Insertar Productos
-- (prov_id depende de los IDs generados arriba)
-- =============================================

-- Verificar los IDs de proveedores insertados
-- SELECT prov_id, prov_nombre FROM tbl_proveedor;

DECLARE @prov1 INT, @prov2 INT, @prov3 INT, @prov4 INT, @prov5 INT;

SELECT @prov1 = prov_id FROM tbl_proveedor WHERE prov_nombre = 'Distribuidora Nacional S.A.';
SELECT @prov2 = prov_id FROM tbl_proveedor WHERE prov_nombre = 'Tech Global Corp';
SELECT @prov3 = prov_id FROM tbl_proveedor WHERE prov_nombre = 'Alimentos del Valle';
SELECT @prov4 = prov_id FROM tbl_proveedor WHERE prov_nombre = 'Importadora Pacific';
SELECT @prov5 = prov_id FROM tbl_proveedor WHERE prov_nombre = 'Suministros Express';

INSERT INTO tbl_producto (pro_nombre, pro_cantidad, pro_precio, pro_estado, prov_id) VALUES ('Laptop HP Pavilion 15',       12,  899.99, 'A', @prov2);
INSERT INTO tbl_producto (pro_nombre, pro_cantidad, pro_precio, pro_estado, prov_id) VALUES ('Mouse Logitech MX Master',    45,   79.50, 'A', @prov2);
INSERT INTO tbl_producto (pro_nombre, pro_cantidad, pro_precio, pro_estado, prov_id) VALUES ('Teclado Mecanico RGB',         30,   65.00, 'A', @prov2);
INSERT INTO tbl_producto (pro_nombre, pro_cantidad, pro_precio, pro_estado, prov_id) VALUES ('Monitor Samsung 27 4K',        8,   350.00, 'A', @prov4);
INSERT INTO tbl_producto (pro_nombre, pro_cantidad, pro_precio, pro_estado, prov_id) VALUES ('Arroz Premium 1kg',           200,    1.25, 'A', @prov3);
INSERT INTO tbl_producto (pro_nombre, pro_cantidad, pro_precio, pro_estado, prov_id) VALUES ('Aceite de Oliva 500ml',       150,    4.50, 'A', @prov3);
INSERT INTO tbl_producto (pro_nombre, pro_cantidad, pro_precio, pro_estado, prov_id) VALUES ('Cable HDMI 2m',               100,    8.99, 'A', @prov1);
INSERT INTO tbl_producto (pro_nombre, pro_cantidad, pro_precio, pro_estado, prov_id) VALUES ('Resma Papel A4 500 hojas',     80,    5.75, 'A', @prov5);
INSERT INTO tbl_producto (pro_nombre, pro_cantidad, pro_precio, pro_estado, prov_id) VALUES ('Silla Ergonomica Oficina',     15,  189.00, 'A', @prov1);
INSERT INTO tbl_producto (pro_nombre, pro_cantidad, pro_precio, pro_estado, prov_id) VALUES ('Audifonos Bluetooth Sony',     25,   45.00, 'A', @prov4);

GO

-- =============================================
-- 3. Verificar datos insertados
-- =============================================
SELECT 
    p.pro_id, 
    p.pro_nombre, 
    p.pro_cantidad, 
    p.pro_precio, 
    pr.prov_nombre AS Proveedor
FROM tbl_producto p
INNER JOIN tbl_proveedor pr ON p.prov_id = pr.prov_id
WHERE p.pro_estado = 'A'
ORDER BY p.pro_id;
GO
