IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [accountType] (
        [Id] int NOT NULL IDENTITY,
        [AccountTypeName] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_accountType] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [brand] (
        [Id] int NOT NULL IDENTITY,
        [BrandName] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_brand] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [category] (
        [Id] int NOT NULL IDENTITY,
        [CategoryName] nvarchar(2) NOT NULL,
        CONSTRAINT [PK_category] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [city] (
        [Id] int NOT NULL IDENTITY,
        [CityName] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_city] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [contactType] (
        [Id] int NOT NULL IDENTITY,
        [ContactTypeName] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_contactType] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [gender] (
        [Id] int NOT NULL IDENTITY,
        [GenderName] nvarchar(8) NOT NULL,
        CONSTRAINT [PK_gender] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [menu] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [URL] nvarchar(1000) NOT NULL,
        CONSTRAINT [PK_menu] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [purchaseType] (
        [Id] int NOT NULL IDENTITY,
        [PurchaseTypeName] nvarchar(30) NOT NULL,
        CONSTRAINT [PK_purchaseType] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [product] (
        [Id] int NOT NULL IDENTITY,
        [ProductName] nvarchar(200) NOT NULL,
        [BrandId] int NOT NULL,
        CONSTRAINT [PK_product] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_product_brand_BrandId] FOREIGN KEY ([BrandId]) REFERENCES [brand] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        [ManagerId] nvarchar(450) NULL,
        [extendidentityuserId] nvarchar(450) NULL,
        [CityId] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUsers_city_CityId] FOREIGN KEY ([CityId]) REFERENCES [city] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUsers_AspNetUsers_extendidentityuserId] FOREIGN KEY ([extendidentityuserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [district] (
        [Id] int NOT NULL IDENTITY,
        [DistrictName] nvarchar(50) NOT NULL,
        [CityId] int NOT NULL,
        CONSTRAINT [PK_district] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_district_city_CityId] FOREIGN KEY ([CityId]) REFERENCES [city] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [roleMenu] (
        [Id] int NOT NULL IDENTITY,
        [MenuId] int NOT NULL,
        [UserRoleId] nvarchar(450) NULL,
        [extendidentityroleId] nvarchar(450) NULL,
        CONSTRAINT [PK_roleMenu] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_roleMenu_menu_MenuId] FOREIGN KEY ([MenuId]) REFERENCES [menu] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_roleMenu_AspNetRoles_extendidentityroleId] FOREIGN KEY ([extendidentityroleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [userBrand] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [extendidentityuserId] nvarchar(450) NULL,
        [BrandId] int NOT NULL,
        CONSTRAINT [PK_userBrand] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_userBrand_brand_BrandId] FOREIGN KEY ([BrandId]) REFERENCES [brand] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_userBrand_AspNetUsers_extendidentityuserId] FOREIGN KEY ([extendidentityuserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [account] (
        [Id] int NOT NULL IDENTITY,
        [AccountName] nvarchar(100) NOT NULL,
        [AccountTypeId] int NOT NULL,
        [Address] nvarchar(300) NOT NULL,
        [PhoneNumber] nvarchar(15) NULL,
        [DistrictId] int NOT NULL,
        [Email] nvarchar(50) NULL,
        [NumberOfDoctors] smallint NOT NULL,
        [PurchaseTypeId] int NOT NULL,
        [PaymentNote] nvarchar(1000) NULL,
        [BestTimeFrom] datetime2 NOT NULL,
        [BestTimeTo] datetime2 NOT NULL,
        [RelationshipNote] nvarchar(1000) NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_account] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_account_accountType_AccountTypeId] FOREIGN KEY ([AccountTypeId]) REFERENCES [accountType] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_account_category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [category] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_account_district_DistrictId] FOREIGN KEY ([DistrictId]) REFERENCES [district] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_account_purchaseType_PurchaseTypeId] FOREIGN KEY ([PurchaseTypeId]) REFERENCES [purchaseType] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [accountBrandPayment] (
        [Id] int NOT NULL IDENTITY,
        [AccountId] int NOT NULL,
        [BrandId] int NOT NULL,
        [Openning] money NOT NULL,
        [Collection] money NOT NULL,
        [Balance] money NOT NULL,
        [Total] money NOT NULL,
        CONSTRAINT [PK_accountBrandPayment] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_accountBrandPayment_account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [account] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_accountBrandPayment_brand_BrandId] FOREIGN KEY ([BrandId]) REFERENCES [brand] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [accountMedicalVisit] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NULL,
        [extendidentityuserId] nvarchar(450) NULL,
        [AccountId] int NOT NULL,
        [VisitDate] datetime2 NOT NULL,
        [VisitTime] datetime2 NOT NULL,
        [SubmittingDate] datetime2 NOT NULL DEFAULT (GETDATE()),
        [SubmittingTime] datetime2 NOT NULL DEFAULT (GETDATE()),
        [VisitNotes] nvarchar(1000) NULL,
        [AdditionalNotes] nvarchar(1000) NULL,
        CONSTRAINT [PK_accountMedicalVisit] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_accountMedicalVisit_account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [account] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_accountMedicalVisit_AspNetUsers_extendidentityuserId] FOREIGN KEY ([extendidentityuserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [accountSalesVisit] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NULL,
        [extendidentityuserId] nvarchar(450) NULL,
        [NameOfPerson] nvarchar(50) NOT NULL,
        [PersonPosition] nvarchar(30) NULL,
        [AccountId] int NOT NULL,
        [VisitDate] datetime2 NOT NULL,
        [VisitTime] datetime2 NOT NULL,
        [SubmittingDate] datetime2 NOT NULL DEFAULT (GETDATE()),
        [SubmittingTime] datetime2 NOT NULL DEFAULT (GETDATE()),
        [VisitNotes] nvarchar(1000) NULL,
        [PaymentNotes] nvarchar(1000) NULL,
        CONSTRAINT [PK_accountSalesVisit] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_accountSalesVisit_account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [account] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_accountSalesVisit_AspNetUsers_extendidentityuserId] FOREIGN KEY ([extendidentityuserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [contact] (
        [Id] int NOT NULL IDENTITY,
        [ContactName] nvarchar(100) NOT NULL,
        [GenderId] int NOT NULL,
        [DistrictId] int NULL,
        [Address] nvarchar(300) NULL,
        [LandLineNumber] nvarchar(15) NULL,
        [MobileNumber] nvarchar(15) NULL,
        [Email] nvarchar(50) NULL,
        [ContactTypeId] int NOT NULL,
        [PaymentNotes] nvarchar(1000) NULL,
        [RelationshipNote] nvarchar(1000) NULL,
        [BestTimeFrom] datetime2 NOT NULL,
        [BestTimeTo] datetime2 NOT NULL,
        [PurchaseTypeId] int NOT NULL,
        [AccountId] int NOT NULL,
        [CategoryId] int NULL,
        CONSTRAINT [PK_contact] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_contact_account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [account] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_contact_category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [category] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_contact_contactType_ContactTypeId] FOREIGN KEY ([ContactTypeId]) REFERENCES [contactType] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_contact_district_DistrictId] FOREIGN KEY ([DistrictId]) REFERENCES [district] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_contact_gender_GenderId] FOREIGN KEY ([GenderId]) REFERENCES [gender] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_contact_purchaseType_PurchaseTypeId] FOREIGN KEY ([PurchaseTypeId]) REFERENCES [purchaseType] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [userAccount] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NULL,
        [extendidentityuserId] nvarchar(450) NULL,
        [AccountId] int NOT NULL,
        CONSTRAINT [PK_userAccount] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_userAccount_account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [account] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_userAccount_AspNetUsers_extendidentityuserId] FOREIGN KEY ([extendidentityuserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [accountMedicalVisitProducts] (
        [Id] int NOT NULL IDENTITY,
        [AccountMeicalVisitId] int NOT NULL,
        [accountmedicalvisitId] int NULL,
        [ProductId] int NOT NULL,
        [BrandShare] tinyint NOT NULL,
        CONSTRAINT [PK_accountMedicalVisitProducts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_accountMedicalVisitProducts_product_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [product] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_accountMedicalVisitProducts_accountMedicalVisit_accountmedicalvisitId] FOREIGN KEY ([accountmedicalvisitId]) REFERENCES [accountMedicalVisit] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [accountSalesVisitBrand] (
        [Id] int NOT NULL IDENTITY,
        [AccountSalesVisitId] int NOT NULL,
        [BrandId] int NOT NULL,
        CONSTRAINT [PK_accountSalesVisitBrand] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_accountSalesVisitBrand_accountSalesVisit_AccountSalesVisitId] FOREIGN KEY ([AccountSalesVisitId]) REFERENCES [accountSalesVisit] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_accountSalesVisitBrand_brand_BrandId] FOREIGN KEY ([BrandId]) REFERENCES [brand] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [contactMedicalVisit] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NULL,
        [extendidentityuserId] nvarchar(450) NULL,
        [ContactId] int NOT NULL,
        [SalesAid] nvarchar(100) NULL,
        [VisitDate] datetime2 NOT NULL,
        [VisitTime] datetime2 NOT NULL,
        [SubmittingDate] datetime2 NOT NULL DEFAULT (GETDATE()),
        [SubmittingTime] datetime2 NOT NULL DEFAULT (GETDATE()),
        [VisitNotes] nvarchar(1000) NULL,
        [Requests] nvarchar(1000) NULL,
        CONSTRAINT [PK_contactMedicalVisit] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_contactMedicalVisit_contact_ContactId] FOREIGN KEY ([ContactId]) REFERENCES [contact] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_contactMedicalVisit_AspNetUsers_extendidentityuserId] FOREIGN KEY ([extendidentityuserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [userContact] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NULL,
        [extendidentityuserId] nvarchar(450) NULL,
        [ContactId] int NOT NULL,
        CONSTRAINT [PK_userContact] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_userContact_contact_ContactId] FOREIGN KEY ([ContactId]) REFERENCES [contact] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_userContact_AspNetUsers_extendidentityuserId] FOREIGN KEY ([extendidentityuserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE TABLE [contactMedicalVisitProduct] (
        [Id] int NOT NULL IDENTITY,
        [ContactMedicalVisitId] int NOT NULL,
        [ProductId] int NOT NULL,
        [NumberOfSamples] tinyint NOT NULL,
        [BrandShare] tinyint NOT NULL,
        CONSTRAINT [PK_contactMedicalVisitProduct] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_contactMedicalVisitProduct_contactMedicalVisit_ContactMedicalVisitId] FOREIGN KEY ([ContactMedicalVisitId]) REFERENCES [contactMedicalVisit] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_contactMedicalVisitProduct_product_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [product] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_account_AccountTypeId] ON [account] ([AccountTypeId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_account_CategoryId] ON [account] ([CategoryId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_account_DistrictId] ON [account] ([DistrictId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_account_PurchaseTypeId] ON [account] ([PurchaseTypeId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_accountBrandPayment_AccountId] ON [accountBrandPayment] ([AccountId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_accountBrandPayment_BrandId] ON [accountBrandPayment] ([BrandId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_accountMedicalVisit_AccountId] ON [accountMedicalVisit] ([AccountId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_accountMedicalVisit_extendidentityuserId] ON [accountMedicalVisit] ([extendidentityuserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_accountMedicalVisitProducts_ProductId] ON [accountMedicalVisitProducts] ([ProductId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_accountMedicalVisitProducts_accountmedicalvisitId] ON [accountMedicalVisitProducts] ([accountmedicalvisitId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_accountSalesVisit_AccountId] ON [accountSalesVisit] ([AccountId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_accountSalesVisit_extendidentityuserId] ON [accountSalesVisit] ([extendidentityuserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_accountSalesVisitBrand_AccountSalesVisitId] ON [accountSalesVisitBrand] ([AccountSalesVisitId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_accountSalesVisitBrand_BrandId] ON [accountSalesVisitBrand] ([BrandId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_AspNetUsers_CityId] ON [AspNetUsers] ([CityId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_AspNetUsers_extendidentityuserId] ON [AspNetUsers] ([extendidentityuserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_contact_AccountId] ON [contact] ([AccountId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_contact_CategoryId] ON [contact] ([CategoryId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_contact_ContactTypeId] ON [contact] ([ContactTypeId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_contact_DistrictId] ON [contact] ([DistrictId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_contact_GenderId] ON [contact] ([GenderId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_contact_PurchaseTypeId] ON [contact] ([PurchaseTypeId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_contactMedicalVisit_ContactId] ON [contactMedicalVisit] ([ContactId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_contactMedicalVisit_extendidentityuserId] ON [contactMedicalVisit] ([extendidentityuserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_contactMedicalVisitProduct_ContactMedicalVisitId] ON [contactMedicalVisitProduct] ([ContactMedicalVisitId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_contactMedicalVisitProduct_ProductId] ON [contactMedicalVisitProduct] ([ProductId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_district_CityId] ON [district] ([CityId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_product_BrandId] ON [product] ([BrandId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_roleMenu_MenuId] ON [roleMenu] ([MenuId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_roleMenu_extendidentityroleId] ON [roleMenu] ([extendidentityroleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_userAccount_AccountId] ON [userAccount] ([AccountId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_userAccount_extendidentityuserId] ON [userAccount] ([extendidentityuserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_userBrand_BrandId] ON [userBrand] ([BrandId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_userBrand_extendidentityuserId] ON [userBrand] ([extendidentityuserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_userContact_ContactId] ON [userContact] ([ContactId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    CREATE INDEX [IX_userContact_extendidentityuserId] ON [userContact] ([extendidentityuserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320173617_x1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210320173617_x1', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    ALTER TABLE [contact] DROP CONSTRAINT [FK_contact_purchaseType_PurchaseTypeId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[contactMedicalVisitProduct]') AND [c].[name] = N'BrandShare');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [contactMedicalVisitProduct] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [contactMedicalVisitProduct] DROP COLUMN [BrandShare];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[contactMedicalVisit]') AND [c].[name] = N'SalesAid');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [contactMedicalVisit] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [contactMedicalVisit] DROP COLUMN [SalesAid];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[accountSalesVisit]') AND [c].[name] = N'NameOfPerson');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [accountSalesVisit] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [accountSalesVisit] DROP COLUMN [NameOfPerson];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[accountSalesVisit]') AND [c].[name] = N'PersonPosition');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [accountSalesVisit] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [accountSalesVisit] DROP COLUMN [PersonPosition];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[accountMedicalVisitProducts]') AND [c].[name] = N'BrandShare');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [accountMedicalVisitProducts] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [accountMedicalVisitProducts] DROP COLUMN [BrandShare];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    ALTER TABLE [contactMedicalVisitProduct] ADD [ProductShare] tinyint NOT NULL DEFAULT CAST(0 AS tinyint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    ALTER TABLE [contactMedicalVisit] ADD [ContactMedicalVisitSalesAidId] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[contact]') AND [c].[name] = N'PurchaseTypeId');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [contact] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [contact] ALTER COLUMN [PurchaseTypeId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    ALTER TABLE [accountMedicalVisitProducts] ADD [ProductShare] tinyint NOT NULL DEFAULT CAST(0 AS tinyint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    CREATE TABLE [accountMedicalVisitPerson] (
        [Id] int NOT NULL IDENTITY,
        [PersonName] nvarchar(50) NOT NULL,
        [PersonPosition] nvarchar(30) NULL,
        [Gender] bit NOT NULL,
        [AccountMedicalVisitId] int NOT NULL,
        CONSTRAINT [PK_accountMedicalVisitPerson] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_accountMedicalVisitPerson_accountMedicalVisit_AccountMedicalVisitId] FOREIGN KEY ([AccountMedicalVisitId]) REFERENCES [accountMedicalVisit] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    CREATE TABLE [accountSalesVisitPerson] (
        [Id] int NOT NULL IDENTITY,
        [PersonName] nvarchar(50) NOT NULL,
        [PersonPosition] nvarchar(30) NULL,
        [Gender] bit NOT NULL,
        [AccountSalesVisitId] int NOT NULL,
        CONSTRAINT [PK_accountSalesVisitPerson] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_accountSalesVisitPerson_accountSalesVisit_AccountSalesVisitId] FOREIGN KEY ([AccountSalesVisitId]) REFERENCES [accountSalesVisit] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    CREATE TABLE [contactMedicalVisitSalesAid] (
        [Id] int NOT NULL IDENTITY,
        [SalesAidName] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_contactMedicalVisitSalesAid] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    CREATE INDEX [IX_contactMedicalVisit_ContactMedicalVisitSalesAidId] ON [contactMedicalVisit] ([ContactMedicalVisitSalesAidId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    CREATE INDEX [IX_accountMedicalVisitPerson_AccountMedicalVisitId] ON [accountMedicalVisitPerson] ([AccountMedicalVisitId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    CREATE INDEX [IX_accountSalesVisitPerson_AccountSalesVisitId] ON [accountSalesVisitPerson] ([AccountSalesVisitId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    ALTER TABLE [contact] ADD CONSTRAINT [FK_contact_purchaseType_PurchaseTypeId] FOREIGN KEY ([PurchaseTypeId]) REFERENCES [purchaseType] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    ALTER TABLE [contactMedicalVisit] ADD CONSTRAINT [FK_contactMedicalVisit_contactMedicalVisitSalesAid_ContactMedicalVisitSalesAidId] FOREIGN KEY ([ContactMedicalVisitSalesAidId]) REFERENCES [contactMedicalVisitSalesAid] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213349_x2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210320213349_x2', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213915_x3')
BEGIN
    ALTER TABLE [contact] DROP CONSTRAINT [FK_contact_gender_GenderId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213915_x3')
BEGIN
    DROP TABLE [gender];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213915_x3')
BEGIN
    DROP INDEX [IX_contact_GenderId] ON [contact];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213915_x3')
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[contact]') AND [c].[name] = N'GenderId');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [contact] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [contact] DROP COLUMN [GenderId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213915_x3')
BEGIN
    ALTER TABLE [contact] ADD [Gender] bit NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210320213915_x3')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210320213915_x3', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323163935_x7')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210323163935_x7', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323164540_x4')
BEGIN
    ALTER TABLE [contactMedicalVisit] DROP CONSTRAINT [FK_contactMedicalVisit_contactMedicalVisitSalesAid_ContactMedicalVisitSalesAidId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323164540_x4')
BEGIN
    DROP INDEX [IX_contactMedicalVisit_ContactMedicalVisitSalesAidId] ON [contactMedicalVisit];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323164540_x4')
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[contactMedicalVisit]') AND [c].[name] = N'ContactMedicalVisitSalesAidId');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [contactMedicalVisit] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [contactMedicalVisit] DROP COLUMN [ContactMedicalVisitSalesAidId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323164540_x4')
BEGIN
    CREATE TABLE [ContactSalesAid] (
        [Id] int NOT NULL IDENTITY,
        [ContactMedicalVisitId] int NOT NULL,
        [SalesAidId] int NOT NULL,
        CONSTRAINT [PK_ContactSalesAid] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ContactSalesAid_contactMedicalVisit_ContactMedicalVisitId] FOREIGN KEY ([ContactMedicalVisitId]) REFERENCES [contactMedicalVisit] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ContactSalesAid_contactMedicalVisitSalesAid_SalesAidId] FOREIGN KEY ([SalesAidId]) REFERENCES [contactMedicalVisitSalesAid] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323164540_x4')
BEGIN
    CREATE INDEX [IX_ContactSalesAid_ContactMedicalVisitId] ON [ContactSalesAid] ([ContactMedicalVisitId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323164540_x4')
BEGIN
    CREATE INDEX [IX_ContactSalesAid_SalesAidId] ON [ContactSalesAid] ([SalesAidId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323164540_x4')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210323164540_x4', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171347_x6')
BEGIN
    ALTER TABLE [contactSalesAid] DROP CONSTRAINT [FK_contactSalesAid_SalesAid_SalesAidId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171347_x6')
BEGIN
    ALTER TABLE [SalesAid] DROP CONSTRAINT [PK_SalesAid];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171347_x6')
BEGIN
    EXEC sp_rename N'[SalesAid]', N'salesAid';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171347_x6')
BEGIN
    ALTER TABLE [salesAid] ADD CONSTRAINT [PK_salesAid] PRIMARY KEY ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171347_x6')
BEGIN
    ALTER TABLE [contactSalesAid] ADD CONSTRAINT [FK_contactSalesAid_salesAid_SalesAidId] FOREIGN KEY ([SalesAidId]) REFERENCES [salesAid] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171347_x6')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210323171347_x6', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    ALTER TABLE [ContactSalesAid] DROP CONSTRAINT [FK_ContactSalesAid_contactMedicalVisit_ContactMedicalVisitId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    ALTER TABLE [ContactSalesAid] DROP CONSTRAINT [FK_ContactSalesAid_contactMedicalVisitSalesAid_SalesAidId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    ALTER TABLE [ContactSalesAid] DROP CONSTRAINT [PK_ContactSalesAid];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    ALTER TABLE [contactMedicalVisitSalesAid] DROP CONSTRAINT [PK_contactMedicalVisitSalesAid];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    EXEC sp_rename N'[ContactSalesAid]', N'contactSalesAid';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    EXEC sp_rename N'[contactMedicalVisitSalesAid]', N'SalesAid';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    EXEC sp_rename N'[contactSalesAid].[IX_ContactSalesAid_SalesAidId]', N'IX_contactSalesAid_SalesAidId', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    EXEC sp_rename N'[contactSalesAid].[IX_ContactSalesAid_ContactMedicalVisitId]', N'IX_contactSalesAid_ContactMedicalVisitId', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    ALTER TABLE [contactSalesAid] ADD CONSTRAINT [PK_contactSalesAid] PRIMARY KEY ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    ALTER TABLE [SalesAid] ADD CONSTRAINT [PK_SalesAid] PRIMARY KEY ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    ALTER TABLE [contactSalesAid] ADD CONSTRAINT [FK_contactSalesAid_contactMedicalVisit_ContactMedicalVisitId] FOREIGN KEY ([ContactMedicalVisitId]) REFERENCES [contactMedicalVisit] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    ALTER TABLE [contactSalesAid] ADD CONSTRAINT [FK_contactSalesAid_SalesAid_SalesAidId] FOREIGN KEY ([SalesAidId]) REFERENCES [SalesAid] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210323171748_x5')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210323171748_x5', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    ALTER TABLE [AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_AspNetUsers_extendidentityuserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[userContact]') AND [c].[name] = N'UserId');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [userContact] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [userContact] DROP COLUMN [UserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[userBrand]') AND [c].[name] = N'UserId');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [userBrand] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [userBrand] DROP COLUMN [UserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[userAccount]') AND [c].[name] = N'UserId');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [userAccount] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [userAccount] DROP COLUMN [UserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[roleMenu]') AND [c].[name] = N'UserRoleId');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [roleMenu] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [roleMenu] DROP COLUMN [UserRoleId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[contactMedicalVisit]') AND [c].[name] = N'UserId');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [contactMedicalVisit] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [contactMedicalVisit] DROP COLUMN [UserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'ManagerId');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [AspNetUsers] DROP COLUMN [ManagerId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[accountSalesVisit]') AND [c].[name] = N'UserId');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [accountSalesVisit] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [accountSalesVisit] DROP COLUMN [UserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[accountMedicalVisit]') AND [c].[name] = N'UserId');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [accountMedicalVisit] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [accountMedicalVisit] DROP COLUMN [UserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    EXEC sp_rename N'[AspNetUsers].[extendidentityuserId]', N'ManagerIdId', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    EXEC sp_rename N'[AspNetUsers].[IX_AspNetUsers_extendidentityuserId]', N'IX_AspNetUsers_ManagerIdId', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_AspNetUsers_ManagerIdId] FOREIGN KEY ([ManagerIdId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326185437_y1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210326185437_y1', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326191435_8')
BEGIN
    ALTER TABLE [contactSalesAid] DROP CONSTRAINT [FK_contactSalesAid_SalesAid_SalesAidId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326191435_8')
BEGIN
    ALTER TABLE [SalesAid] DROP CONSTRAINT [PK_SalesAid];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326191435_8')
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[accountMedicalVisitProducts]') AND [c].[name] = N'AccountMeicalVisitId');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [accountMedicalVisitProducts] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [accountMedicalVisitProducts] DROP COLUMN [AccountMeicalVisitId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326191435_8')
BEGIN
    EXEC sp_rename N'[SalesAid]', N'salesAid';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326191435_8')
BEGIN
    ALTER TABLE [salesAid] ADD CONSTRAINT [PK_salesAid] PRIMARY KEY ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326191435_8')
BEGIN
    ALTER TABLE [contactSalesAid] ADD CONSTRAINT [FK_contactSalesAid_salesAid_SalesAidId] FOREIGN KEY ([SalesAidId]) REFERENCES [salesAid] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210326191435_8')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210326191435_8', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329153723_ch')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210329153723_ch', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210331161852_c1')
BEGIN
    ALTER TABLE [AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_city_CityId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210331161852_c1')
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'CityId');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [AspNetUsers] ALTER COLUMN [CityId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210331161852_c1')
BEGIN
    ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_city_CityId] FOREIGN KEY ([CityId]) REFERENCES [city] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210331161852_c1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210331161852_c1', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210401165601_a')
BEGIN
    ALTER TABLE [AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_AspNetUsers_ManagerIdId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210401165601_a')
BEGIN
    EXEC sp_rename N'[AspNetUsers].[ManagerIdId]', N'extendidentityuserid', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210401165601_a')
BEGIN
    EXEC sp_rename N'[AspNetUsers].[IX_AspNetUsers_ManagerIdId]', N'IX_AspNetUsers_extendidentityuserid', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210401165601_a')
BEGIN
    ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_AspNetUsers_extendidentityuserid] FOREIGN KEY ([extendidentityuserid]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210401165601_a')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210401165601_a', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210404141319_new')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [FullName] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210404141319_new')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210404141319_new', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210405160300_ame1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210405160300_ame1', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210417175846_new1')
BEGIN
    ALTER TABLE [userBrand] DROP CONSTRAINT [FK_userBrand_AspNetUsers_extendidentityuserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210417175846_new1')
BEGIN
    EXEC sp_rename N'[userBrand].[extendidentityuserId]', N'extendidentityuserid', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210417175846_new1')
BEGIN
    EXEC sp_rename N'[userBrand].[IX_userBrand_extendidentityuserId]', N'IX_userBrand_extendidentityuserid', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210417175846_new1')
BEGIN
    ALTER TABLE [userBrand] ADD CONSTRAINT [FK_userBrand_AspNetUsers_extendidentityuserid] FOREIGN KEY ([extendidentityuserid]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210417175846_new1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210417175846_new1', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210417184558_new2')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Active] bit NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210417184558_new2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210417184558_new2', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210420182952_new3')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210420182952_new3', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210420183107_new4')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210420183107_new4', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210420184412_new5')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210420184412_new5', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210420184545_new6')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210420184545_new6', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210420184742_new8')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210420184742_new8', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [account] DROP CONSTRAINT [FK_account_accountType_AccountTypeId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [account] DROP CONSTRAINT [FK_account_category_CategoryId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [account] DROP CONSTRAINT [FK_account_district_DistrictId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [account] DROP CONSTRAINT [FK_account_purchaseType_PurchaseTypeId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [accountMedicalVisit] DROP CONSTRAINT [FK_accountMedicalVisit_AspNetUsers_extendidentityuserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [contact] DROP CONSTRAINT [FK_contact_account_AccountId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [contact] DROP CONSTRAINT [FK_contact_contactType_ContactTypeId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [userAccount] DROP CONSTRAINT [FK_userAccount_AspNetUsers_extendidentityuserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [userContact] DROP CONSTRAINT [FK_userContact_AspNetUsers_extendidentityuserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    EXEC sp_rename N'[userContact].[extendidentityuserId]', N'extendidentityuserid', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    EXEC sp_rename N'[userContact].[IX_userContact_extendidentityuserId]', N'IX_userContact_extendidentityuserid', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    EXEC sp_rename N'[userAccount].[extendidentityuserId]', N'extendidentityuserid', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    EXEC sp_rename N'[userAccount].[IX_userAccount_extendidentityuserId]', N'IX_userAccount_extendidentityuserid', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    EXEC sp_rename N'[accountMedicalVisit].[extendidentityuserId]', N'extendidentityuserid', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    EXEC sp_rename N'[accountMedicalVisit].[IX_accountMedicalVisit_extendidentityuserId]', N'IX_accountMedicalVisit_extendidentityuserid', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    DECLARE @var18 sysname;
    SELECT @var18 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[contact]') AND [c].[name] = N'ContactTypeId');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [contact] DROP CONSTRAINT [' + @var18 + '];');
    ALTER TABLE [contact] ALTER COLUMN [ContactTypeId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    DECLARE @var19 sysname;
    SELECT @var19 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[contact]') AND [c].[name] = N'AccountId');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [contact] DROP CONSTRAINT [' + @var19 + '];');
    ALTER TABLE [contact] ALTER COLUMN [AccountId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    DECLARE @var20 sysname;
    SELECT @var20 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[account]') AND [c].[name] = N'PurchaseTypeId');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [account] DROP CONSTRAINT [' + @var20 + '];');
    ALTER TABLE [account] ALTER COLUMN [PurchaseTypeId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    DECLARE @var21 sysname;
    SELECT @var21 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[account]') AND [c].[name] = N'NumberOfDoctors');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [account] DROP CONSTRAINT [' + @var21 + '];');
    ALTER TABLE [account] ALTER COLUMN [NumberOfDoctors] smallint NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    DECLARE @var22 sysname;
    SELECT @var22 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[account]') AND [c].[name] = N'DistrictId');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [account] DROP CONSTRAINT [' + @var22 + '];');
    ALTER TABLE [account] ALTER COLUMN [DistrictId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    DECLARE @var23 sysname;
    SELECT @var23 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[account]') AND [c].[name] = N'CategoryId');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [account] DROP CONSTRAINT [' + @var23 + '];');
    ALTER TABLE [account] ALTER COLUMN [CategoryId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    DECLARE @var24 sysname;
    SELECT @var24 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[account]') AND [c].[name] = N'BestTimeTo');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [account] DROP CONSTRAINT [' + @var24 + '];');
    ALTER TABLE [account] ALTER COLUMN [BestTimeTo] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    DECLARE @var25 sysname;
    SELECT @var25 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[account]') AND [c].[name] = N'BestTimeFrom');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [account] DROP CONSTRAINT [' + @var25 + '];');
    ALTER TABLE [account] ALTER COLUMN [BestTimeFrom] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    DECLARE @var26 sysname;
    SELECT @var26 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[account]') AND [c].[name] = N'AccountTypeId');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [account] DROP CONSTRAINT [' + @var26 + '];');
    ALTER TABLE [account] ALTER COLUMN [AccountTypeId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [account] ADD CONSTRAINT [FK_account_accountType_AccountTypeId] FOREIGN KEY ([AccountTypeId]) REFERENCES [accountType] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [account] ADD CONSTRAINT [FK_account_category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [category] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [account] ADD CONSTRAINT [FK_account_district_DistrictId] FOREIGN KEY ([DistrictId]) REFERENCES [district] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [account] ADD CONSTRAINT [FK_account_purchaseType_PurchaseTypeId] FOREIGN KEY ([PurchaseTypeId]) REFERENCES [purchaseType] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [accountMedicalVisit] ADD CONSTRAINT [FK_accountMedicalVisit_AspNetUsers_extendidentityuserid] FOREIGN KEY ([extendidentityuserid]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [contact] ADD CONSTRAINT [FK_contact_account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [account] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [contact] ADD CONSTRAINT [FK_contact_contactType_ContactTypeId] FOREIGN KEY ([ContactTypeId]) REFERENCES [contactType] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [userAccount] ADD CONSTRAINT [FK_userAccount_AspNetUsers_extendidentityuserid] FOREIGN KEY ([extendidentityuserid]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    ALTER TABLE [userContact] ADD CONSTRAINT [FK_userContact_AspNetUsers_extendidentityuserid] FOREIGN KEY ([extendidentityuserid]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503190225_a1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210503190225_a1', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191303_sa')
BEGIN
    ALTER TABLE [salesAid] ADD [show] bit NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191303_sa')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210503191303_sa', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191408_a2')
BEGIN
    ALTER TABLE [accountSalesVisit] DROP CONSTRAINT [FK_accountSalesVisit_AspNetUsers_extendidentityuserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191408_a2')
BEGIN
    EXEC sp_rename N'[accountSalesVisit].[extendidentityuserId]', N'extendidentityuserid', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191408_a2')
BEGIN
    EXEC sp_rename N'[accountSalesVisit].[IX_accountSalesVisit_extendidentityuserId]', N'IX_accountSalesVisit_extendidentityuserid', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191408_a2')
BEGIN
    ALTER TABLE [accountSalesVisit] ADD CONSTRAINT [FK_accountSalesVisit_AspNetUsers_extendidentityuserid] FOREIGN KEY ([extendidentityuserid]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191408_a2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210503191408_a2', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191948_om')
BEGIN
    ALTER TABLE [contactMedicalVisit] DROP CONSTRAINT [FK_contactMedicalVisit_AspNetUsers_extendidentityuserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191948_om')
BEGIN
    EXEC sp_rename N'[contactMedicalVisit].[extendidentityuserId]', N'extendidentityuserid', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191948_om')
BEGIN
    EXEC sp_rename N'[contactMedicalVisit].[IX_contactMedicalVisit_extendidentityuserId]', N'IX_contactMedicalVisit_extendidentityuserid', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191948_om')
BEGIN
    DECLARE @var27 sysname;
    SELECT @var27 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[contact]') AND [c].[name] = N'BestTimeTo');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [contact] DROP CONSTRAINT [' + @var27 + '];');
    ALTER TABLE [contact] ALTER COLUMN [BestTimeTo] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191948_om')
BEGIN
    DECLARE @var28 sysname;
    SELECT @var28 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[contact]') AND [c].[name] = N'BestTimeFrom');
    IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [contact] DROP CONSTRAINT [' + @var28 + '];');
    ALTER TABLE [contact] ALTER COLUMN [BestTimeFrom] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191948_om')
BEGIN
    ALTER TABLE [contactMedicalVisit] ADD CONSTRAINT [FK_contactMedicalVisit_AspNetUsers_extendidentityuserid] FOREIGN KEY ([extendidentityuserid]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503191948_om')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210503191948_om', N'2.1.14-servicing-32113');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503195115_mig')
BEGIN
    DECLARE @var29 sysname;
    SELECT @var29 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[accountBrandPayment]') AND [c].[name] = N'Total');
    IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [accountBrandPayment] DROP CONSTRAINT [' + @var29 + '];');
    ALTER TABLE [accountBrandPayment] DROP COLUMN [Total];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210503195115_mig')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210503195115_mig', N'2.1.14-servicing-32113');
END;

GO

