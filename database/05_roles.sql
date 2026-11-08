USE refugio;

CREATE ROLE IF NOT EXISTS 'rol_voluntario';
CREATE ROLE IF NOT EXISTS 'rol_veterinario';
CREATE ROLE IF NOT EXISTS 'rol_admin';

GRANT SELECT ON refugio.* TO 'rol_voluntario';

GRANT SELECT ON refugio.* TO 'rol_veterinario';
GRANT INSERT, UPDATE ON refugio.animales TO 'rol_veterinario';
GRANT INSERT, UPDATE ON refugio.tratamientos TO 'rol_veterinario';

GRANT ALL PRIVILEGES ON refugio.* TO 'rol_admin';

CREATE USER IF NOT EXISTS 'refugio_app'@'%' IDENTIFIED BY 'RefugioApp2026';
CREATE USER IF NOT EXISTS 'refugio_app'@'localhost' IDENTIFIED BY 'RefugioApp2026';

GRANT ALL PRIVILEGES ON refugio.* TO 'refugio_app'@'%';
GRANT ALL PRIVILEGES ON refugio.* TO 'refugio_app'@'localhost';

FLUSH PRIVILEGES;
