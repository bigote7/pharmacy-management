# Instructions pour les Migrations Entity Framework

## Prérequis

Assurez-vous d'avoir installé :
- .NET 8.0 SDK
- SQL Server (LocalDB ou SQL Server Express/Full)
- Entity Framework Core Tools

## Installation des outils EF Core

```bash
dotnet tool install --global dotnet-ef
```

## Créer une migration

```bash
dotnet ef migrations add NomDeLaMigration --project src/PharmacyManagement.Infrastructure --startup-project src/PharmacyManagement.API
```

## Appliquer les migrations à la base de données

```bash
dotnet ef database update --project src/PharmacyManagement.Infrastructure --startup-project src/PharmacyManagement.API
```

## Créer la base de données initiale

Pour créer la première migration et la base de données :

```bash
# 1. Créer la migration initiale
dotnet ef migrations add InitialCreate --project src/PharmacyManagement.Infrastructure --startup-project src/PharmacyManagement.API

# 2. Appliquer la migration
dotnet ef database update --project src/PharmacyManagement.Infrastructure --startup-project src/PharmacyManagement.API
```

## Supprimer la dernière migration

```bash
dotnet ef migrations remove --project src/PharmacyManagement.Infrastructure --startup-project src/PharmacyManagement.API
```

## Voir le script SQL généré

```bash
dotnet ef migrations script --project src/PharmacyManagement.Infrastructure --startup-project src/PharmacyManagement.API
```

## Notes

- Les migrations sont stockées dans `src/PharmacyManagement.Infrastructure/Migrations/`
- Modifiez la chaîne de connexion dans `appsettings.json` selon votre environnement
- Pour SQL Server LocalDB, la chaîne par défaut devrait fonctionner
- Pour PostgreSQL, modifiez le package NuGet et la chaîne de connexion

