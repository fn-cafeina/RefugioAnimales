CREATE DATABASE IF NOT EXISTS refugio
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE refugio;

DROP TABLE IF EXISTS historial_estados_animal;
DROP TABLE IF EXISTS donaciones;
DROP TABLE IF EXISTS tratamientos;
DROP TABLE IF EXISTS solicitudes_adopcion;
DROP TABLE IF EXISTS adoptantes;
DROP TABLE IF EXISTS animales;
DROP TABLE IF EXISTS usuarios;

CREATE TABLE usuarios (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(100) NOT NULL,
  email VARCHAR(150) NOT NULL UNIQUE,
  password_hash VARCHAR(255) NOT NULL,
  rol ENUM('voluntario', 'veterinario', 'admin') NOT NULL DEFAULT 'voluntario',
  activo BOOLEAN NOT NULL DEFAULT TRUE,
  creado_en TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE = InnoDB;

CREATE TABLE animales (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(100) NOT NULL,
  especie ENUM('perro', 'gato', 'conejo', 'ave', 'otro') NOT NULL,
  raza VARCHAR(100) NULL,
  sexo ENUM('macho', 'hembra') NOT NULL,
  fecha_nacimiento DATE NULL,
  descripcion TEXT NULL,
  foto_url VARCHAR(500) NULL,
  estado ENUM('disponible', 'en_tratamiento', 'adoptado') NOT NULL DEFAULT 'disponible',
  fecha_ingreso DATE NOT NULL,
  creado_en TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX idx_animales_estado (estado),
  INDEX idx_animales_especie (especie),
  INDEX idx_animales_nombre (nombre)
) ENGINE = InnoDB;

CREATE TABLE adoptantes (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(100) NOT NULL,
  telefono VARCHAR(30) NULL,
  email VARCHAR(150) NULL,
  direccion VARCHAR(200) NULL,
  notas TEXT NULL,
  creado_en TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX idx_adoptantes_nombre (nombre),
  INDEX idx_adoptantes_email (email)
) ENGINE = InnoDB;

CREATE TABLE solicitudes_adopcion (
  id INT AUTO_INCREMENT PRIMARY KEY,
  animal_id INT NOT NULL,
  adoptante_id INT NOT NULL,
  estado ENUM('pendiente', 'aprobada', 'rechazada', 'cancelada') NOT NULL DEFAULT 'pendiente',
  motivo VARCHAR(255) NULL,
  fecha_solicitud TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  resuelta_en TIMESTAMP NULL,
  resuelta_por INT NULL,
  CONSTRAINT fk_solicitud_animal FOREIGN KEY (animal_id) REFERENCES animales (id),
  CONSTRAINT fk_solicitud_adoptante FOREIGN KEY (adoptante_id) REFERENCES adoptantes (id),
  CONSTRAINT fk_solicitud_usuario FOREIGN KEY (resuelta_por) REFERENCES usuarios (id),
  INDEX idx_solicitudes_estado (estado),
  INDEX idx_solicitudes_animal (animal_id),
  INDEX idx_solicitudes_adoptante (adoptante_id)
) ENGINE = InnoDB;

CREATE TABLE tratamientos (
  id INT AUTO_INCREMENT PRIMARY KEY,
  animal_id INT NOT NULL,
  veterinario_id INT NULL,
  tipo ENUM('consulta', 'vacunacion', 'desparasitacion', 'cirugia', 'otro') NOT NULL,
  descripcion VARCHAR(300) NOT NULL,
  fecha DATE NOT NULL,
  costo DECIMAL(10, 2) NOT NULL DEFAULT 0,
  estado ENUM('pendiente', 'completado') NOT NULL DEFAULT 'pendiente',
  CONSTRAINT fk_tratamiento_animal FOREIGN KEY (animal_id) REFERENCES animales (id),
  CONSTRAINT fk_tratamiento_veterinario FOREIGN KEY (veterinario_id) REFERENCES usuarios (id),
  INDEX idx_tratamientos_animal (animal_id),
  INDEX idx_tratamientos_estado (estado)
) ENGINE = InnoDB;

CREATE TABLE donaciones (
  id INT AUTO_INCREMENT PRIMARY KEY,
  donante VARCHAR(100) NOT NULL,
  monto DECIMAL(10, 2) NOT NULL,
  fecha DATE NOT NULL,
  metodo ENUM('efectivo', 'transferencia', 'tarjeta', 'otro') NOT NULL DEFAULT 'efectivo',
  nota VARCHAR(300) NULL,
  registrado_por INT NULL,
  CONSTRAINT fk_donacion_usuario FOREIGN KEY (registrado_por) REFERENCES usuarios (id),
  INDEX idx_donaciones_fecha (fecha)
) ENGINE = InnoDB;

CREATE TABLE historial_estados_animal (
  id INT AUTO_INCREMENT PRIMARY KEY,
  animal_id INT NOT NULL,
  estado_anterior ENUM('disponible', 'en_tratamiento', 'adoptado') NULL,
  estado_nuevo ENUM('disponible', 'en_tratamiento', 'adoptado') NOT NULL,
  fecha TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  motivo VARCHAR(255) NULL,
  CONSTRAINT fk_historial_animal FOREIGN KEY (animal_id) REFERENCES animales (id),
  INDEX idx_historial_animal (animal_id)
) ENGINE = InnoDB;
