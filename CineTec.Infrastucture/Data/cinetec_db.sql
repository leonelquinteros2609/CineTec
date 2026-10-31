-- Base de datos: cinetec_db
CREATE DATABASE IF NOT EXISTS cinetec_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE cinetec_db;

-- 1. Tabla: usuarios
DROP TABLE IF EXISTS pagos;
DROP TABLE IF EXISTS entradas;
DROP TABLE IF EXISTS compras;
DROP TABLE IF EXISTS funciones;
DROP TABLE IF EXISTS butacas;
DROP TABLE IF EXISTS salas;
DROP TABLE IF EXISTS peliculas;
DROP TABLE IF EXISTS usuarios;

CREATE TABLE usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    rol VARCHAR(20) NOT NULL DEFAULT 'Cliente',
    fecha_creacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 2. Tabla: peliculas
CREATE TABLE peliculas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    titulo VARCHAR(150) NOT NULL,
    sinopsis TEXT NULL,
    duracion_min INT NOT NULL,
    clasificacion VARCHAR(20) NOT NULL,
    genero VARCHAR(50) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'Activa'
);

-- 3. Tabla: salas
CREATE TABLE salas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    filas INT NOT NULL,
    columnas INT NOT NULL,
    capacidad INT NOT NULL
);

-- 4. Tabla: butacas
CREATE TABLE butacas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    sala_id INT NOT NULL,
    fila VARCHAR(5) NOT NULL,
    columna INT NOT NULL,
    codigo VARCHAR(10) NOT NULL,
    CONSTRAINT FK_butacas_salas FOREIGN KEY (sala_id) REFERENCES salas(id) ON DELETE CASCADE
);

-- 5. Tabla: funciones
CREATE TABLE funciones (
    id INT AUTO_INCREMENT PRIMARY KEY,
    pelicula_id INT NOT NULL,
    sala_id INT NOT NULL,
    fecha_hora_inicio DATETIME NOT NULL,
    fecha_hora_fin DATETIME NOT NULL,
    precio_base DECIMAL(10, 2) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'Activa',
    CONSTRAINT FK_funciones_peliculas FOREIGN KEY (pelicula_id) REFERENCES peliculas(id),
    CONSTRAINT FK_funciones_salas FOREIGN KEY (sala_id) REFERENCES salas(id)
);

-- 6. Tabla: compras
CREATE TABLE compras (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario_id INT NOT NULL,
    funcion_id INT NOT NULL,
    fecha_compra DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_expiracion DATETIME NOT NULL,
    total DECIMAL(10, 2) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'Pendiente',
    CONSTRAINT FK_compras_usuarios FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
    CONSTRAINT FK_compras_funciones FOREIGN KEY (funcion_id) REFERENCES funciones(id)
);

-- 7. Tabla: entradas
CREATE TABLE entradas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    compra_id INT NOT NULL,
    funcion_id INT NOT NULL,
    butaca_id INT NOT NULL,
    precio DECIMAL(10, 2) NOT NULL,
    codigo_unico VARCHAR(64) NULL UNIQUE,
    estado VARCHAR(20) NOT NULL DEFAULT 'Reservada',
    CONSTRAINT FK_entradas_compras FOREIGN KEY (compra_id) REFERENCES compras(id) ON DELETE CASCADE,
    CONSTRAINT FK_entradas_funciones FOREIGN KEY (funcion_id) REFERENCES funciones(id),
    CONSTRAINT FK_entradas_butacas FOREIGN KEY (butaca_id) REFERENCES butacas(id)
);

-- 8. Tabla: pagos
CREATE TABLE pagos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    compra_id INT NOT NULL,
    monto DECIMAL(10, 2) NOT NULL,
    metodo_pago VARCHAR(30) NOT NULL,
    fecha_pago DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    estado VARCHAR(20) NOT NULL DEFAULT 'Completado',
    CONSTRAINT FK_pagos_compras FOREIGN KEY (compra_id) REFERENCES compras(id)
);

-- ============================================================
-- SEED DATA (Datos iniciales para pruebas según el documento)
-- ============================================================

-- Usuarios (contraseña predeterminada con hash BCrypt y soporte en código)
-- Las contraseñas en texto claro correspondientes son:
-- admin@cinetec.com -> Admin123! o 1234
-- taquilla@cinetec.com -> Taquilla123! o 1234
-- cliente@cinetec.com -> Cliente123! o 1234
INSERT INTO usuarios (nombre, email, password_hash, rol) VALUES
('Administrador CineTec', 'admin@cinetec.com', '1234', 'Administrador'),
('Taquillero Central', 'taquilla@cinetec.com', '1234', 'Taquillero'),
('Cliente Frecuente', 'cliente@cinetec.com', '1234', 'Cliente');

-- Películas
INSERT INTO peliculas (titulo, sinopsis, duracion_min, clasificacion, genero, estado) VALUES
('Inception', 'Un ladrón que roba secretos corporativos a través del uso de la tecnología de compartir sueños.', 148, 'PG-13', 'Ciencia Ficción', 'Activa'),
('Interstellar', 'Un equipo de exploradores viaja a través de un agujero de gusano en el espacio en un intento por asegurar la supervivencia de la humanidad.', 169, 'PG-13', 'Aventura', 'Activa'),
('El Viaje de Chihiro', 'Una niña de diez años queda atrapada en un mundo gobernado por dioses, brujas y espíritus.', 125, 'ATP', 'Animación', 'Activa');

-- Salas
INSERT INTO salas (nombre, filas, columnas, capacidad) VALUES
('Sala 1 - Principal', 4, 5, 20),
('Sala 2 - VIP', 3, 4, 12);

-- Butacas Sala 1 (4x5 = 20)
INSERT INTO butacas (sala_id, fila, columna, codigo) VALUES
(1, 'A', 1, 'A1'), (1, 'A', 2, 'A2'), (1, 'A', 3, 'A3'), (1, 'A', 4, 'A4'), (1, 'A', 5, 'A5'),
(1, 'B', 1, 'B1'), (1, 'B', 2, 'B2'), (1, 'B', 3, 'B3'), (1, 'B', 4, 'B4'), (1, 'B', 5, 'B5'),
(1, 'C', 1, 'C1'), (1, 'C', 2, 'C2'), (1, 'C', 3, 'C3'), (1, 'C', 4, 'C4'), (1, 'C', 5, 'C5'),
(1, 'D', 1, 'D1'), (1, 'D', 2, 'D2'), (1, 'D', 3, 'D3'), (1, 'D', 4, 'D4'), (1, 'D', 5, 'D5');

-- Butacas Sala 2 (3x4 = 12)
INSERT INTO butacas (sala_id, fila, columna, codigo) VALUES
(2, 'A', 1, 'A1'), (2, 'A', 2, 'A2'), (2, 'A', 3, 'A3'), (2, 'A', 4, 'A4'),
(2, 'B', 1, 'B1'), (2, 'B', 2, 'B2'), (2, 'B', 3, 'B3'), (2, 'B', 4, 'B4'),
(2, 'C', 1, 'C1'), (2, 'C', 2, 'C2'), (2, 'C', 3, 'C3'), (2, 'C', 4, 'C4');

-- Funciones (para hoy y días futuros)
INSERT INTO funciones (pelicula_id, sala_id, fecha_hora_inicio, fecha_hora_fin, precio_base, estado) VALUES
(1, 1, DATE_ADD(NOW(), INTERVAL 2 HOUR), DATE_ADD(DATE_ADD(NOW(), INTERVAL 2 HOUR), INTERVAL 148 MINUTE), 35.00, 'Activa'),
(2, 1, DATE_ADD(NOW(), INTERVAL 6 HOUR), DATE_ADD(DATE_ADD(NOW(), INTERVAL 6 HOUR), INTERVAL 169 MINUTE), 40.00, 'Activa'),
(3, 2, DATE_ADD(NOW(), INTERVAL 3 HOUR), DATE_ADD(DATE_ADD(NOW(), INTERVAL 3 HOUR), INTERVAL 125 MINUTE), 45.00, 'Activa'),
(1, 2, DATE_ADD(DATE_ADD(NOW(), INTERVAL 1 DAY), INTERVAL 4 HOUR), DATE_ADD(DATE_ADD(DATE_ADD(NOW(), INTERVAL 1 DAY), INTERVAL 4 HOUR), INTERVAL 148 MINUTE), 45.00, 'Activa');
