-- OPTIONAL: kusa nang gagawa ng table ang app, pero pwede mo rin i-run ito sa HeidiSQL/phpMyAdmin.
USE academic_planner;

CREATE TABLE IF NOT EXISTS activities (
  id           INT AUTO_INCREMENT PRIMARY KEY,
  user_id      INT NOT NULL,
  title        VARCHAR(150) NOT NULL,
  subject      VARCHAR(100) NOT NULL,
  priority     VARCHAR(10)  NOT NULL DEFAULT 'Medium',   -- High / Medium / Low
  deadline     DATE         NOT NULL,
  status       VARCHAR(15)  NOT NULL DEFAULT 'Pending',  -- Pending / Completed
  notes        TEXT NULL,
  created_at   DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  completed_at DATETIME NULL,
  INDEX idx_user_status (user_id, status)
);
