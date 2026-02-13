IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Group] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Period] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Group] PRIMARY KEY ([Id])
);

CREATE TABLE [Role] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY ([Id])
);

CREATE TABLE [Subject] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Code] nvarchar(max) NOT NULL,
    [Credits] int NULL,
    CONSTRAINT [PK_Subject] PRIMARY KEY ([Id])
);

CREATE TABLE [User] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([Id])
);

CREATE TABLE [RoleUser] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [RoleId] bigint NOT NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_RoleUser] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RoleUser_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleUser_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Student] (
    [Id] bigint NOT NULL IDENTITY,
    [Phone] nvarchar(max) NULL,
    [Boleta] nvarchar(max) NOT NULL,
    [UserId] bigint NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Student] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Student_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Teacher] (
    [Id] bigint NOT NULL IDENTITY,
    [EmployeeNumber] nvarchar(max) NULL,
    [Phone] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [UserId] bigint NOT NULL,
    CONSTRAINT [PK_Teacher] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Teacher_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Enrollment] (
    [Id] bigint NOT NULL IDENTITY,
    [StudentId] bigint NOT NULL,
    [GroupId] bigint NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Enrollment] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Enrollment_Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Group] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Enrollment_Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Student] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [CourseAssignment] (
    [Id] bigint NOT NULL IDENTITY,
    [TeacherId] bigint NOT NULL,
    [SubjectId] bigint NOT NULL,
    [GroupId] bigint NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CourseAssignment] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CourseAssignment_Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Group] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseAssignment_Subject_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subject] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseAssignment_Teacher_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teacher] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Grade] (
    [Id] bigint NOT NULL IDENTITY,
    [EnrollmentId] bigint NOT NULL,
    [SubjectId] bigint NOT NULL,
    [TeacherId] bigint NOT NULL,
    [GradeValue] decimal(5,2) NULL,
    [Term] nvarchar(max) NULL,
    CONSTRAINT [PK_Grade] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Grade_Enrollment_EnrollmentId] FOREIGN KEY ([EnrollmentId]) REFERENCES [Enrollment] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Grade_Subject_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subject] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Grade_Teacher_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teacher] ([Id]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'Period') AND [object_id] = OBJECT_ID(N'[Group]'))
    SET IDENTITY_INSERT [Group] ON;
INSERT INTO [Group] ([Id], [Name], [Period])
VALUES (CAST(1 AS bigint), N'Grupo A', N'2026-1'),
(CAST(2 AS bigint), N'Grupo B', N'2026-1'),
(CAST(3 AS bigint), N'Grupo C', N'2026-1'),
(CAST(4 AS bigint), N'Grupo D', N'2026-1'),
(CAST(5 AS bigint), N'Grupo E', N'2026-1');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'Period') AND [object_id] = OBJECT_ID(N'[Group]'))
    SET IDENTITY_INSERT [Group] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Role]'))
    SET IDENTITY_INSERT [Role] ON;
INSERT INTO [Role] ([Id], [Description], [Name])
VALUES (CAST(1 AS bigint), N'Administrador del sistema', N'admin'),
(CAST(2 AS bigint), N'Cliente del sistema', N'student'),
(CAST(3 AS bigint), N'Docente del sistema', N'teacher');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Role]'))
    SET IDENTITY_INSERT [Role] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'Credits', N'Name') AND [object_id] = OBJECT_ID(N'[Subject]'))
    SET IDENTITY_INSERT [Subject] ON;
INSERT INTO [Subject] ([Id], [Code], [Credits], [Name])
VALUES (CAST(1 AS bigint), N'MAT', NULL, N'Matemáticas'),
(CAST(2 AS bigint), N'LEN', NULL, N'Lengua y Literatura'),
(CAST(3 AS bigint), N'SOC', NULL, N'Ciencias Sociales'),
(CAST(4 AS bigint), N'NAT', NULL, N'Ciencias Naturales'),
(CAST(5 AS bigint), N'FIS', NULL, N'Educación Física');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'Credits', N'Name') AND [object_id] = OBJECT_ID(N'[Subject]'))
    SET IDENTITY_INSERT [Subject] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Email', N'Name', N'Password', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[User]'))
    SET IDENTITY_INSERT [User] ON;
INSERT INTO [User] ([Id], [CreatedAt], [Email], [Name], [Password], [UpdatedAt])
VALUES (CAST(1 AS bigint), '2026-01-12T00:00:00.0000000', N'admin@campus.com', N'Administrador', N'$2a$11$SAZGNCx7o8A2IjWfJEBbwORDzAtvmV5qNiNyBe/nacO5X77lR3ikO', '2026-01-12T00:00:00.0000000');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Email', N'Name', N'Password', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[User]'))
    SET IDENTITY_INSERT [User] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'RoleId', N'UpdatedAt', N'UserId') AND [object_id] = OBJECT_ID(N'[RoleUser]'))
    SET IDENTITY_INSERT [RoleUser] ON;
INSERT INTO [RoleUser] ([Id], [CreatedAt], [RoleId], [UpdatedAt], [UserId])
VALUES (CAST(1 AS bigint), '2026-01-12T00:00:00.0000000', CAST(1 AS bigint), '2026-01-12T00:00:00.0000000', CAST(1 AS bigint));
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'RoleId', N'UpdatedAt', N'UserId') AND [object_id] = OBJECT_ID(N'[RoleUser]'))
    SET IDENTITY_INSERT [RoleUser] OFF;

CREATE INDEX [IX_CourseAssignment_GroupId] ON [CourseAssignment] ([GroupId]);

CREATE INDEX [IX_CourseAssignment_SubjectId] ON [CourseAssignment] ([SubjectId]);

CREATE INDEX [IX_CourseAssignment_TeacherId] ON [CourseAssignment] ([TeacherId]);

CREATE INDEX [IX_Enrollment_GroupId] ON [Enrollment] ([GroupId]);

CREATE INDEX [IX_Enrollment_StudentId] ON [Enrollment] ([StudentId]);

CREATE INDEX [IX_Grade_EnrollmentId] ON [Grade] ([EnrollmentId]);

CREATE INDEX [IX_Grade_SubjectId] ON [Grade] ([SubjectId]);

CREATE INDEX [IX_Grade_TeacherId] ON [Grade] ([TeacherId]);

CREATE INDEX [IX_RoleUser_RoleId] ON [RoleUser] ([RoleId]);

CREATE INDEX [IX_RoleUser_UserId] ON [RoleUser] ([UserId]);

CREATE UNIQUE INDEX [IX_Student_UserId] ON [Student] ([UserId]);

CREATE UNIQUE INDEX [IX_Teacher_UserId] ON [Teacher] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260212174449_InitialCreate', N'10.0.3');

COMMIT;
GO

