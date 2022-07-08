CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(250) NOT NULL,
    [Cognome] nvarchar(250) NOT NULL,
    [Username] nvarchar(250) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [UserRefreshToken] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [UserName] nvarchar(max) NOT NULL,
    [RefreshToken] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT (((1))),
    CONSTRAINT [PK_UserRefreshToken] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserRefreshToken_UserRefreshToken] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO


CREATE INDEX [IX_UserRefreshToken_UserId] ON [UserRefreshToken] ([UserId]);
GO


