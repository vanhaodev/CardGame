CREATE TABLE
    `user` (
        id BIGINT UNSIGNED PRIMARY KEY, -- Google ID (sub field từ token)
        displayName VARCHAR(255), -- Tên hiển thị trong game
        avatarId SMALLINT UNSIGNED DEFAULT 1, -- Link avatar nếu cần
        createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );
