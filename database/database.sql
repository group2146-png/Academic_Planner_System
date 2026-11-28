-- Academic Planner: database setup para sa Laragon (MySQL)
-- Patakbuhin sa HeidiSQL o phpMyAdmin.

CREATE DATABASE IF NOT EXISTS academic_planner
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE academic_planner;

CREATE TABLE IF NOT EXISTS users (
    id            INT AUTO_INCREMENT PRIMARY KEY,
    username      VARCHAR(50)  NOT NULL UNIQUE,
    full_name     VARCHAR(100) NOT NULL,
    email         VARCHAR(100) NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    created_at    TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Test account:  username = student   password = student123
INSERT IGNORE INTO users (username, full_name, email, password_hash)
VALUES (
    'student',
    'Juan Delacruz',
    'student@example.com',
    '100000.D14vQ5d/phavjNg0K9Dnqg==.Ep/fksW0jzoXtVVem3+Bw6Taa7nZL/DCI2wHWXbFQzg='
);
