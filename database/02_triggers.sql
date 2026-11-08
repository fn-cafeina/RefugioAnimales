USE refugio;

DROP TRIGGER IF EXISTS trg_animal_after_insert;
DROP TRIGGER IF EXISTS trg_animal_after_update;
DROP TRIGGER IF EXISTS trg_solicitud_before_insert;
DROP TRIGGER IF EXISTS trg_solicitud_before_update;
DROP TRIGGER IF EXISTS trg_solicitud_after_update;
DROP TRIGGER IF EXISTS trg_donacion_before_insert;

DELIMITER $$

CREATE TRIGGER trg_animal_after_insert
AFTER INSERT ON animales
FOR EACH ROW
BEGIN
  INSERT INTO historial_estados_animal (animal_id, estado_anterior, estado_nuevo, motivo)
  VALUES (NEW.id, NULL, NEW.estado, 'Ingreso al refugio');
END$$

CREATE TRIGGER trg_animal_after_update
AFTER UPDATE ON animales
FOR EACH ROW
BEGIN
  IF NEW.estado <> OLD.estado THEN
    INSERT INTO historial_estados_animal (animal_id, estado_anterior, estado_nuevo, motivo)
    VALUES (NEW.id, OLD.estado, NEW.estado, 'Cambio de estado');
  END IF;
END$$

CREATE TRIGGER trg_solicitud_before_insert
BEFORE INSERT ON solicitudes_adopcion
FOR EACH ROW
BEGIN
  DECLARE v_estado VARCHAR(20) DEFAULT NULL;
  DECLARE v_duplicadas INT DEFAULT 0;
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_estado = NULL;

  IF NEW.estado <> 'pendiente' THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'Una solicitud nueva debe crearse en estado pendiente';
  END IF;

  SELECT estado INTO v_estado FROM animales WHERE id = NEW.animal_id;

  IF v_estado IS NULL THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'El animal indicado no existe';
  END IF;

  IF v_estado <> 'disponible' THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'Solo se puede solicitar la adopción de animales disponibles';
  END IF;

  SELECT COUNT(*) INTO v_duplicadas
  FROM solicitudes_adopcion
  WHERE animal_id = NEW.animal_id
    AND adoptante_id = NEW.adoptante_id
    AND estado = 'pendiente';

  IF v_duplicadas > 0 THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'Ya existe una solicitud pendiente de este adoptante para el animal';
  END IF;
END$$

CREATE TRIGGER trg_solicitud_before_update
BEFORE UPDATE ON solicitudes_adopcion
FOR EACH ROW
BEGIN
  DECLARE v_estado_animal VARCHAR(20) DEFAULT NULL;
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_estado_animal = NULL;

  IF OLD.estado IN ('aprobada', 'rechazada', 'cancelada') AND NEW.estado <> OLD.estado THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'Una solicitud resuelta no puede volver a cambiar de estado';
  END IF;

  IF NEW.estado IN ('aprobada', 'rechazada', 'cancelada') AND NEW.resuelta_en IS NULL THEN
    SET NEW.resuelta_en = CURRENT_TIMESTAMP;
  END IF;

  IF NEW.estado = 'aprobada' AND OLD.estado <> 'aprobada' THEN
    SELECT estado INTO v_estado_animal FROM animales WHERE id = NEW.animal_id;

    IF v_estado_animal IS NULL OR v_estado_animal <> 'disponible' THEN
      SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El animal ya no está disponible para adopción';
    END IF;
  END IF;
END$$

CREATE TRIGGER trg_solicitud_after_update
AFTER UPDATE ON solicitudes_adopcion
FOR EACH ROW
BEGIN
  IF NEW.estado = 'aprobada' AND OLD.estado <> 'aprobada' THEN
    UPDATE animales
    SET estado = 'adoptado'
    WHERE id = NEW.animal_id
      AND estado <> 'adoptado';
  END IF;
END$$

CREATE TRIGGER trg_donacion_before_insert
BEFORE INSERT ON donaciones
FOR EACH ROW
BEGIN
  IF NEW.monto <= 0 THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'El monto de la donación debe ser mayor que cero';
  END IF;
END$$

DELIMITER ;
