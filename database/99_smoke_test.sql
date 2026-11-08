USE refugio;

-- Prueba del flujo completo de aprobación de adopción.
-- Crea datos temporales, ejecuta el procedimiento y muestra el resultado.
-- Al final elimina los datos temporales, sin tocar el seed.

INSERT INTO animales
  (nombre, especie, raza, sexo, fecha_nacimiento, descripcion, foto_url, estado, fecha_ingreso)
VALUES
  ('Animal de prueba', 'perro', 'Mestizo', 'macho', NULL, 'Registro temporal', NULL, 'disponible', CURDATE());
SET @animal = LAST_INSERT_ID();

INSERT INTO adoptantes (nombre, telefono) VALUES ('Adoptante de prueba A', '000');
SET @adoptante_a = LAST_INSERT_ID();

INSERT INTO adoptantes (nombre, telefono) VALUES ('Adoptante de prueba B', '111');
SET @adoptante_b = LAST_INSERT_ID();

INSERT INTO solicitudes_adopcion (animal_id, adoptante_id, estado) VALUES
  (@animal, @adoptante_a, 'pendiente'),
  (@animal, @adoptante_b, 'pendiente');

SET @solicitud_aprobada = (
  SELECT id FROM solicitudes_adopcion
  WHERE animal_id = @animal AND adoptante_id = @adoptante_a
);

CALL sp_aprobar_solicitud(@solicitud_aprobada, 1);

SELECT 'Estado del animal' AS comprobacion, estado
FROM animales WHERE id = @animal;

SELECT 'Solicitudes del animal' AS comprobacion, estado, motivo
FROM solicitudes_adopcion
WHERE animal_id = @animal
ORDER BY id;

SELECT 'Historial de estados' AS comprobacion, estado_anterior, estado_nuevo, motivo
FROM historial_estados_animal
WHERE animal_id = @animal
ORDER BY id;

DELETE FROM historial_estados_animal WHERE animal_id = @animal;
DELETE FROM solicitudes_adopcion WHERE animal_id = @animal;
DELETE FROM animales WHERE id = @animal;
DELETE FROM adoptantes WHERE id IN (@adoptante_a, @adoptante_b);
