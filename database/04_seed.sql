USE refugio;

SET NAMES utf8mb4;

INSERT INTO usuarios (nombre, email, password_hash, rol, activo) VALUES
('Ana Martínez', 'admin@refugio.local', '100000.D0Nn1OogENYiQD2VJ6ElOA==.fBbuRt1YLBVC4Ikv5fYK09nYG7yJ8VIG8wWAdbS+8J8=', 'admin', 1),
('Dr. Pablo Ruiz', 'vet@refugio.local', '100000.QIKOnT8MZf/bqxNPzxuigA==.98HWIdeIN3g7jhbf8UGr7MtoW4y7P00hhCHDOVFiVtc=', 'veterinario', 1),
('Carla Gómez', 'voluntario@refugio.local', '100000.4JyY6FSTIrUmM49yaPLO3g==.zAIsB14jbIvjxVdQt3QsbUm2hZ+sLZJ2CRjfYm4YGt4=', 'voluntario', 1);

INSERT INTO animales
  (nombre, especie, raza, sexo, fecha_nacimiento, descripcion, foto_url, estado, fecha_ingreso)
VALUES
('Firulais', 'perro', 'Mestizo', 'macho', DATE_SUB(CURDATE(), INTERVAL 5 YEAR),
 'Muy sociable y juguetón. Se lleva bien con niños y otros perros.', NULL, 'disponible', DATE_SUB(CURDATE(), INTERVAL 300 DAY)),
('Luna', 'gato', 'Siamés', 'hembra', DATE_SUB(CURDATE(), INTERVAL 3 YEAR),
 'Tranquila y cariñosa. Busca un hogar con espacio para descansar.', NULL, 'disponible', DATE_SUB(CURDATE(), INTERVAL 260 DAY)),
('Rocky', 'perro', 'Labrador', 'macho', DATE_SUB(CURDATE(), INTERVAL 6 YEAR),
 'En tratamiento por una lesión en la pata. Es dócil y obediente.', NULL, 'en_tratamiento', DATE_SUB(CURDATE(), INTERVAL 210 DAY)),
('Misifú', 'gato', 'Común', 'macho', DATE_SUB(CURDATE(), INTERVAL 2 YEAR),
 'Curioso y activo. Ideal para departamento con protección en ventanas.', NULL, 'disponible', DATE_SUB(CURDATE(), INTERVAL 190 DAY)),
('Canela', 'perro', 'Beagle', 'hembra', DATE_SUB(CURDATE(), INTERVAL 4 YEAR),
 'Muy compañera y energética. Disfruta de los paseos largos.', NULL, 'disponible', DATE_SUB(CURDATE(), INTERVAL 180 DAY)),
('Toby', 'perro', 'Caniche', 'macho', DATE_SUB(CURDATE(), INTERVAL 7 YEAR),
 'Adulto mayor, tranquilo y agradecido. Necesita compañía.', NULL, 'disponible', DATE_SUB(CURDATE(), INTERVAL 150 DAY)),
('Nala', 'gato', 'Atigrada', 'hembra', DATE_SUB(CURDATE(), INTERVAL 3 YEAR),
 'Independiente pero afectuosa cuando toma confianza.', NULL, 'disponible', DATE_SUB(CURDATE(), INTERVAL 140 DAY)),
('Simba', 'gato', 'Naranja', 'macho', DATE_SUB(CURDATE(), INTERVAL 5 YEAR),
 'En tratamiento por problemas digestivos. Muy manso.', NULL, 'en_tratamiento', DATE_SUB(CURDATE(), INTERVAL 120 DAY)),
('Kira', 'perro', 'Border Collie', 'hembra', DATE_SUB(CURDATE(), INTERVAL 2 YEAR),
 'Inteligente y con mucha energía. Necesita estímulo diario.', NULL, 'disponible', DATE_SUB(CURDATE(), INTERVAL 110 DAY)),
('Bongo', 'conejo', 'Enano', 'macho', DATE_SUB(CURDATE(), INTERVAL 1 YEAR),
 'Tranquilo y limpio. Ideal para interior.', NULL, 'disponible', DATE_SUB(CURDATE(), INTERVAL 100 DAY)),
('Perla', 'ave', 'Loro', 'hembra', DATE_SUB(CURDATE(), INTERVAL 4 YEAR),
 'Sociable y conversadora. Requiere atención y espacio.', NULL, 'disponible', DATE_SUB(CURDATE(), INTERVAL 90 DAY)),
('Dante', 'perro', 'Mestizo', 'macho', DATE_SUB(CURDATE(), INTERVAL 8 YEAR),
 'Mayor y calmo. Le encanta dormir al sol.', NULL, 'disponible', DATE_SUB(CURDATE(), INTERVAL 210 DAY)),
('Miel', 'gato', 'Común', 'hembra', DATE_SUB(CURDATE(), INTERVAL 4 YEAR),
 'Dulce y hogareña. Se adapta bien a la vida en familia.', NULL, 'disponible', DATE_SUB(CURDATE(), INTERVAL 120 DAY)),
('Bruno', 'perro', 'Pastor Alemán', 'macho', DATE_SUB(CURDATE(), INTERVAL 6 YEAR),
 'En tratamiento por una cirugía de cadera. Buen temperamento.', NULL, 'en_tratamiento', DATE_SUB(CURDATE(), INTERVAL 80 DAY)),
('Azúcar', 'conejo', 'Enana', 'hembra', DATE_SUB(CURDATE(), INTERVAL 1 YEAR),
 'Muy mansa y curiosa. Comparte pacíficamente con otros conejos.', NULL, 'disponible', DATE_SUB(CURDATE(), INTERVAL 60 DAY));

INSERT INTO adoptantes (nombre, telefono, email, direccion, notas) VALUES
('María González', '11-5555-1001', 'maria.gonzalez@example.com', 'Av. Siempreviva 123', 'Tiene un perro adulto en casa.'),
('Jorge Ramírez', '11-5555-1002', 'jorge.ramirez@example.com', 'Calle Los Robles 456', 'Vive en casa con patio.'),
('Lucía Torres', '11-5555-1003', 'lucia.torres@example.com', 'Pasaje del Sol 789', 'Primera adopción.'),
('Pedro Sánchez', '11-5555-1004', 'pedro.sanchez@example.com', 'Av. Central 321', 'Familia con dos niños.'),
('Valentina Ríos', '11-5555-1005', 'valentina.rios@example.com', 'Calle Verde 654', 'Experiencia con gatos.'),
('Martín Acosta', '11-5555-1006', 'martin.acosta@example.com', 'Barrio Norte 987', 'Trabaja desde casa.'),
('Sofía Herrera', '11-5555-1007', 'sofia.herrera@example.com', 'Callejón 147', 'Interesada en un perro adulto.'),
('Diego Morales', '11-5555-1008', 'diego.morales@example.com', 'Av. del Parque 258', 'Vive en departamento.');

INSERT INTO solicitudes_adopcion (animal_id, adoptante_id, estado) VALUES
(1, 1, 'pendiente'),
(1, 2, 'pendiente'),
(4, 3, 'pendiente'),
(5, 4, 'pendiente'),
(7, 5, 'pendiente'),
(9, 6, 'pendiente'),
(12, 7, 'pendiente'),
(13, 8, 'pendiente');

INSERT INTO tratamientos (animal_id, veterinario_id, tipo, descripcion, fecha, costo, estado) VALUES
(3, 2, 'consulta', 'Evaluación de lesión en pata trasera.', DATE_SUB(CURDATE(), INTERVAL 15 DAY), 4500, 'pendiente'),
(3, 2, 'vacunacion', 'Vacuna antirrábica anual.', DATE_SUB(CURDATE(), INTERVAL 40 DAY), 2500, 'completado'),
(8, 2, 'desparasitacion', 'Tratamiento digestivo y control de peso.', DATE_SUB(CURDATE(), INTERVAL 10 DAY), 3200, 'pendiente'),
(14, 2, 'cirugia', 'Cirugía de cadera programada.', DATE_SUB(CURDATE(), INTERVAL 5 DAY), 28000, 'pendiente'),
(14, 2, 'consulta', 'Control prequirúrgico.', DATE_SUB(CURDATE(), INTERVAL 20 DAY), 3800, 'completado'),
(2, 2, 'vacunacion', 'Refuerzo de vacunas.', DATE_SUB(CURDATE(), INTERVAL 30 DAY), 2500, 'completado'),
(15, 2, 'consulta', 'Chequeo general de ingreso.', DATE_SUB(CURDATE(), INTERVAL 55 DAY), 2000, 'completado');

INSERT INTO donaciones (donante, monto, fecha, metodo, nota, registrado_por) VALUES
('Clínica San Francisco', 12000, DATE_SUB(CURDATE(), INTERVAL 1 MONTH), 'transferencia', 'Aporte mensual', 1),
('Clínica San Francisco', 15000, DATE_SUB(CURDATE(), INTERVAL 2 MONTH), 'transferencia', 'Aporte mensual', 1),
('Clínica San Francisco', 12000, CURDATE(), 'transferencia', 'Aporte mensual', 1),
('María López', 5000, DATE_SUB(CURDATE(), INTERVAL 5 MONTH), 'efectivo', 'Donación de vecina', 3),
('María López', 8000, DATE_SUB(CURDATE(), INTERVAL 3 MONTH), 'transferencia', NULL, 3),
('María López', 10000, CURDATE(), 'transferencia', 'Para alimentos', 3),
('Juan Pérez', 3000, DATE_SUB(CURDATE(), INTERVAL 4 MONTH), 'efectivo', NULL, 3),
('Juan Pérez', 4500, DATE_SUB(CURDATE(), INTERVAL 2 MONTH), 'tarjeta', NULL, 3),
('Vecinos del barrio', 7000, DATE_SUB(CURDATE(), INTERVAL 5 MONTH), 'efectivo', 'Colecta barrial', 1),
('Vecinos del barrio', 6500, DATE_SUB(CURDATE(), INTERVAL 2 MONTH), 'efectivo', 'Colecta barrial', 1),
('Lucía Fernández', 2500, DATE_SUB(CURDATE(), INTERVAL 4 MONTH), 'efectivo', NULL, 3),
('Lucía Fernández', 3000, DATE_SUB(CURDATE(), INTERVAL 1 MONTH), 'transferencia', NULL, 3),
('Panadería La Espiga', 2000, DATE_SUB(CURDATE(), INTERVAL 3 MONTH), 'efectivo', 'Aporte mensual', 2),
('Panadería La Espiga', 2500, CURDATE(), 'efectivo', 'Aporte mensual', 2),
('Anónimo', 1000, DATE_SUB(CURDATE(), INTERVAL 2 MONTH), 'efectivo', 'Donación anónima', 2);

-- Aprobaciones de ejemplo: disparan el procedimiento y los triggers.
SET @sol_firulais = (SELECT id FROM solicitudes_adopcion WHERE animal_id = 1 AND adoptante_id = 1);
CALL sp_aprobar_solicitud(@sol_firulais, 1);
UPDATE solicitudes_adopcion
SET resuelta_en = DATE_SUB(NOW(), INTERVAL 60 DAY)
WHERE id = @sol_firulais;

SET @sol_dante = (SELECT id FROM solicitudes_adopcion WHERE animal_id = 12 AND adoptante_id = 7);
CALL sp_aprobar_solicitud(@sol_dante, 1);
UPDATE solicitudes_adopcion
SET resuelta_en = DATE_SUB(NOW(), INTERVAL 30 DAY)
WHERE id = @sol_dante;

SET @sol_miel = (SELECT id FROM solicitudes_adopcion WHERE animal_id = 13 AND adoptante_id = 8);
CALL sp_aprobar_solicitud(@sol_miel, 1);
