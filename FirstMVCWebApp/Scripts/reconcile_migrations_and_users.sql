-- Reconciliation script: safe, idempotent.
-- Usage: review then run against the FirstMVC database (e.g., via SSMS or sqlcmd).
-- This script will:
-- 1) Ensure the __EFMigrationsHistory table exists (schema matching EF Core expectations).
-- 2) If dbo.Users exists but the initial migration '20260523051502_dbinit' is not recorded, insert the migration row.
-- 3) If the Users table is missing the Username column, add it safely (nullable -> populate -> make NOT NULL).

SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Configure these values to match your migration file
DECLARE @MigrationId NVARCHAR(150) = N'20260523051502_dbinit';
DECLARE @ProductVersion NVARCHAR(32) = N'10.0.8';

BEGIN TRY
    -- Ensure migrations history table exists
    IF NOT EXISTS (SELECT 1 FROM sys.tables t WHERE t.name = '__EFMigrationsHistory' AND SCHEMA_NAME(t.schema_id) = 'dbo')
    BEGIN
        CREATE TABLE [dbo].[__EFMigrationsHistory] (
            [MigrationId] NVARCHAR(150) NOT NULL CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY,
            [ProductVersion] NVARCHAR(32) NOT NULL
        );
    END

    -- Record initial migration if Users table exists but migration row missing
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Users')
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = @MigrationId)
        BEGIN
            INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
            VALUES (@MigrationId, @ProductVersion);
        END
    END

    -- Add Username column if missing
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Users')
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Users' AND COLUMN_NAME = 'Username')
        BEGIN
            ALTER TABLE dbo.Users ADD Username NVARCHAR(MAX) NULL;
            -- Set a default non-empty value for existing rows; adjust as appropriate.
            UPDATE dbo.Users SET Username = '' WHERE Username IS NULL;
            ALTER TABLE dbo.Users ALTER COLUMN Username NVARCHAR(MAX) NOT NULL;
        END
    END

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrNum INT = ERROR_NUMBER();
    ROLLBACK TRANSACTION;
    RAISERROR('Reconciliation script failed with error %d: %s', 16, 1, @ErrNum, @ErrMsg);
END CATCH