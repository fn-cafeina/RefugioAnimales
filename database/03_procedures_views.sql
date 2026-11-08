USE refugio;

CREATE OR REPLACE VIEW v_animales_catalogo AS
SELECT
  a.id,
  a.nombre,
  a.especie,
  a.raza,
  a.sexo,
  a.fecha_nacimiento,
  TIMESTAMPDIFF(YEAR, a.fecha_nacimiento, CURDATE()) AS edad_anios,
  a.descripcion,
  a.foto_url,
  a.estado,
  a.fecha_ingreso,
  (SELECT COUNT(*)
   FROM solicitudes_adopcion s
   WHERE s.animal_id = a.id
     AND s.estado = 'pendiente') AS solicitudes_pendientes
FROM animales a;

CREATE OR REPLACE VIEW v_solicitudes_detalle AS
SELECT
  s.id,
  s.animal_id,
  a.nombre AS animal,
  s.adoptante_id,
  ad.nombre AS adoptante,
  s.estado,
  s.motivo,
  s.fecha_solicitud,
  s.resuelta_en,
  u.nombre AS resuelta_por_nombre
FROM solicitudes_adopcion s
JOIN animales a ON a.id = s.animal_id
JOIN adoptantes ad ON ad.id = s.adoptante_id
LEFT JOIN usuarios u ON u.id = s.resuelta_por;

CREATE OR REPLACE VIEW v_donaciones_mensuales AS
SELECT
  DATE_FORMAT(fecha, '%Y-%m') AS mes,
  COUNT(*) AS cantidad,
  SUM(monto) AS total
FROM donaciones
GROUP BY DATE_FORMAT(fecha, '%Y-%m');

DROP PROCEDURE IF EXISTS sp_aprobar_solicitud;
DROP PROCEDURE IF EXISTS sp_adopciones_por_mes;
DROP PROCEDURE IF EXISTS sp_top_donantes;
DROP PROCEDURE IF EXISTS sp_tratamientos_pendientes;
DROP PROCEDURE IF EXISTS sp_resumen_dashboard;

DELIMITER $$

CREATE PROCEDURE sp_aprobar_solicitud(
  IN p_solicitud_id INT,
  IN p_usuario_id INT
)
BEGIN
  DECLARE v_animal_id INT DEFAULT NULL;
  DECLARE v_estado VARCHAR(20) DEFAULT NULL;
  DECLARE v_estado_animal VARCHAR(20) DEFAULT NULL;
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_estado = NULL;
  DECLARE EXIT HANDLER FOR SQLEXCEPTION
  BEGIN
    ROLLBACK;
    RESIGNAL;
  END;

  START TRANSACTION;

  SELECT animal_id, estado
  INTO v_animal_id, v_estado
  FROM solicitudes_adopcion
  WHERE id = p_solicitud_id
  FOR UPDATE;

  IF v_estado IS NULL THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'La solicitud indicada no existe';
  END IF;

  IF v_estado <> 'pendiente' THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'La solicitud ya fue resuelta';
  END IF;

  SELECT estado
  INTO v_estado_animal
  FROM animales
  WHERE id = v_animal_id
  FOR UPDATE;

  IF v_estado_animal <> 'disponible' THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'El animal ya no está disponible para adopción';
  END IF;

  UPDATE solicitudes_adopcion
  SET estado = 'aprobada',
      resuelta_por = p_usuario_id
  WHERE id = p_solicitud_id;

  UPDATE solicitudes_adopcion
  SET estado = 'rechazada',
      motivo = 'Adopción aprobada para otro solicitante',
      resuelta_por = p_usuario_id
  WHERE animal_id = v_animal_id
    AND id <> p_solicitud_id
    AND estado = 'pendiente';

  COMMIT;
END$$

CREATE PROCEDURE sp_adopciones_por_mes(IN p_anio INT)
BEGIN
  SELECT
    MONTH(resuelta_en) AS mes,
    COUNT(*) AS adopciones
  FROM solicitudes_adopcion
  WHERE estado = 'aprobada'
    AND resuelta_en IS NOT NULL
    AND YEAR(resuelta_en) = p_anio
  GROUP BY MONTH(resuelta_en)
  ORDER BY mes;
END$$

CREATE PROCEDURE sp_top_donantes(IN p_limite INT)
BEGIN
  SELECT
    donante,
    SUM(monto) AS total,
    COUNT(*) AS donaciones,
    RANK() OVER (ORDER BY SUM(monto) DESC) AS posicion
  FROM donaciones
  GROUP BY donante
  ORDER BY total DESC
  LIMIT p_limite;
END$$

CREATE PROCEDURE sp_tratamientos_pendientes()
BEGIN
  SELECT
    t.id,
    a.nombre AS animal,
    t.tipo,
    t.descripcion,
    t.fecha,
    t.costo
  FROM tratamientos t
  JOIN animales a ON a.id = t.animal_id
  WHERE t.estado = 'pendiente'
  ORDER BY t.fecha;
END$$

CREATE PROCEDURE sp_resumen_dashboard()
BEGIN
  SELECT
    (SELECT COUNT(*) FROM animales WHERE estado = 'disponible') AS animales_disponibles,
    (SELECT COUNT(*) FROM animales WHERE estado = 'en_tratamiento') AS animales_en_tratamiento,
    (SELECT COUNT(*) FROM solicitudes_adopcion WHERE estado = 'pendiente') AS solicitudes_pendientes,
    (SELECT COUNT(*)
     FROM solicitudes_adopcion
     WHERE estado = 'aprobada'
       AND YEAR(resuelta_en) = YEAR(CURDATE())
       AND MONTH(resuelta_en) = MONTH(CURDATE())) AS adopciones_mes,
    (SELECT COALESCE(SUM(monto), 0)
     FROM donaciones
     WHERE YEAR(fecha) = YEAR(CURDATE())
       AND MONTH(fecha) = MONTH(CURDATE())) AS donaciones_mes;
END$$

DELIMITER ;
