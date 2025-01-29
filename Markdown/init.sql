CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "UserName" text NOT NULL,
    "Email" text NOT NULL,
    "PasswordHash" text NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250124190937_initial', '8.0.12');

COMMIT;

START TRANSACTION;

ALTER TABLE "Users" RENAME COLUMN "UserName" TO "Username";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250124194014_RenameUserNameInUsersToUsername', '8.0.12');

COMMIT;

START TRANSACTION;

CREATE TABLE "Documents" (
    id uuid NOT NULL,
    "Title" text NOT NULL,
    "Name" text NOT NULL,
    "ContentType" text NOT NULL,
    "FileSize" bigint NOT NULL,
    "StorageObjectId" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastModifiedAt" timestamp with time zone NOT NULL,
    "OwnerId" uuid NOT NULL,
    CONSTRAINT "PK_Documents" PRIMARY KEY (id),
    CONSTRAINT "FK_Documents_Users_OwnerId" FOREIGN KEY ("OwnerId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "DocumentPermissions" (
    "UserId" uuid NOT NULL,
    "DocumentId" uuid NOT NULL,
    "AccessLevel" integer NOT NULL,
    CONSTRAINT "PK_DocumentPermissions" PRIMARY KEY ("DocumentId", "UserId"),
    CONSTRAINT "FK_DocumentPermissions_Documents_DocumentId" FOREIGN KEY ("DocumentId") REFERENCES "Documents" (id) ON DELETE CASCADE,
    CONSTRAINT "FK_DocumentPermissions_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

CREATE INDEX "IX_DocumentPermissions_UserId" ON "DocumentPermissions" ("UserId");

CREATE INDEX "IX_Documents_OwnerId" ON "Documents" ("OwnerId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250126112757_DocumentsCreate', '8.0.12');

COMMIT;

START TRANSACTION;

ALTER TABLE "Documents" RENAME COLUMN id TO "Id";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250126224602_DocumentId', '8.0.12');

COMMIT;

START TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250128142122_InitialCreate', '8.0.12');

COMMIT;

START TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250128145927_Cry', '8.0.12');

COMMIT;

START TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250128154915_Cry2', '8.0.12');

COMMIT;

