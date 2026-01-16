# 💊 Système de Gestion de Pharmacie

Une application complète de gestion de pharmacie développée en **.NET 8** avec une architecture en couches propre et professionnelle. Ce système permet de gérer efficacement tous les aspects opérationnels d'une pharmacie moderne, depuis la gestion des stocks jusqu'aux ventes et facturation.

## 📋 Table des Matières

- [Aperçu](#aperçu)
- [Fonctionnalités](#fonctionnalités)
- [Architecture](#architecture)
- [Technologies Utilisées](#technologies-utilisées)
- [Installation](#installation)
- [Configuration](#configuration)
- [Utilisation](#utilisation)
- [Documentation API](#documentation-api)
- [Structure du Projet](#structure-du-projet)

## 🎯 Aperçu

Ce système de gestion de pharmacie développée par LABIB LAYACHI  est conçu pour répondre aux besoins quotidiens des pharmaciens et de leur équipe. Il offre une interface intuitive pour gérer les médicaments, suivre les stocks, traiter les ventes, gérer les clients et fournisseurs, tout en respectant les normes pharmaceutiques en vigueur.

**Caractéristiques principales :**
- ✅ Gestion complète des médicaments et stocks
- ✅ Système de ventes avec facturation automatique
- ✅ Gestion des prescriptions médicales
- ✅ Suivi des dates d'expiration et alertes
- ✅ Authentification et autorisation sécurisées
- ✅ Dashboard analytique en temps réel
- ✅ API REST complète et documentée

## ✨ Fonctionnalités

### 📦 Gestion des Médicaments
- Création, modification et suppression de médicaments
- Recherche avancée par nom, code-barres ou catégorie
- Gestion des catégories et informations détaillées
- Alertes automatiques pour les stocks faibles
- Support des prescriptions obligatoires

### 👥 Gestion des Clients
- Base de données clients complète
- Historique des achats par client
- Recherche rapide de clients existants
- Gestion des informations personnelles et médicales

### 💰 Gestion des Ventes
- Création de ventes avec détails multiples
- Calcul automatique des totaux et TVA (20%)
- Génération automatique de numéros de facture
- Gestion des méthodes de paiement
- Traçabilité complète des transactions

### 📊 Gestion des Stocks
- Suivi en temps réel des quantités disponibles
- Gestion des dates d'expiration par lot
- Système FIFO (First In First Out) pour les sorties
- Alertes d'expiration proche
- Traçabilité par numéro de lot

### 📋 Gestion des Commandes
- Création de commandes fournisseurs
- Suivi des réceptions et livraisons
- Gestion des fournisseurs
- Historique des commandes

### 🔐 Sécurité et Authentification
- Système d'authentification JWT sécurisé
- Gestion des rôles (Admin, Caissier, Pharmacien)
- Middleware de gestion d'exceptions
- Protection des endpoints sensibles

### 📈 Dashboard Analytique
- Vue d'ensemble des ventes du jour
- Statistiques des médicaments les plus vendus
- Indicateurs de performance clés (KPI)
- Rapports de revenus

## 🏗️ Architecture

Le projet suit les principes de **Clean Architecture** avec une séparation claire des responsabilités :

```
┌─────────────────────────────────────────┐
│      PharmacyManagement.API             │  ← Couche Présentation
│  (Controllers, Middleware, Configuration)│
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│   PharmacyManagement.Application        │  ← Couche Application
│     (Services, DTOs, Interfaces)        │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│  PharmacyManagement.Infrastructure      │  ← Couche Infrastructure
│   (DbContext, Repositories, EF Core)    │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│     PharmacyManagement.Domain           │  ← Couche Domaine
│        (Entities, Value Objects)        │
└─────────────────────────────────────────┘
```

### Couches du Projet

- **Domain** : Contient les entités du domaine métier (Medicament, Client, Vente, Stock, etc.). Cette couche est indépendante et ne dépend d'aucune autre couche.

- **Infrastructure** : Gère l'accès aux données via Entity Framework Core, les repositories, et les migrations. Implémente les interfaces définies dans la couche Application.

- **Application** : Contient la logique métier, les services, les DTOs (Data Transfer Objects) et les interfaces. C'est le cœur fonctionnel de l'application.

- **API** : Couche de présentation avec les contrôleurs REST, la configuration de l'application, le middleware et les points d'entrée.

## 🛠️ Technologies Utilisées

- **.NET 8.0** - Framework principal
- **ASP.NET Core Web API** - Framework web pour l'API REST
- **Entity Framework Core 8.0** - ORM pour l'accès aux données
- **MySQL** - Base de données (configurable pour SQL Server)
- **Swagger/OpenAPI** - Documentation interactive de l'API
- **JWT (JSON Web Tokens)** - Authentification sécurisée
- **BCrypt** - Hachage des mots de passe
- **FluentValidation** - Validation des données (si utilisé)

## 📦 Installation

### Prérequis

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL Server 8.0+ (ou SQL Server)
- Visual Studio 2022 / Visual Studio Code / Rider (optionnel)

### Étapes d'installation

1. **Cloner le dépôt**
   ```bash
   git clone https://github.com/votre-username/pharmacy-management.git
   cd pharmacy-management
   ```

2. **Restaurer les dépendances NuGet**
   ```bash
   dotnet restore
   ```

3. **Configurer la base de données**
   - Modifiez la chaîne de connexion dans `src/PharmacyManagement.API/appsettings.json`
   - Assurez-vous que MySQL est installé et en cours d'exécution

4. **Appliquer les migrations**
   ```bash
   cd src/PharmacyManagement.API
   dotnet ef database update --project ../PharmacyManagement.Infrastructure
   ```

5. **Lancer l'application**
   ```bash
   dotnet run
   ```

L'API sera accessible sur :
- `https://localhost:5001` (HTTPS)
- `http://localhost:5000` (HTTP)

## ⚙️ Configuration

### Configuration de la Base de Données

Modifiez `appsettings.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=PharmacyManagementDb;User=root;Password=votre_mot_de_passe;"
  }
}
```

### Configuration JWT

Les paramètres JWT sont configurés dans `appsettings.json` :

```json
{
  "Jwt": {
    "Key": "VotreCleSecreteTresLongueEtSecuriseePourJWT",
    "Issuer": "PharmacyManagement",
    "Audience": "PharmacyManagement",
    "ExpirationHours": 24
  }
}
```

⚠️ **Important** : Changez la clé JWT en production avec une clé sécurisée et longue !

## 🚀 Utilisation

### Accéder à la Documentation API

Une fois l'application lancée, accédez à Swagger UI :
```
https://localhost:5001/swagger
```

### Premier Utilisateur (Admin)

Après la première migration, vous pouvez créer un utilisateur admin via l'endpoint `/api/setup` ou en utilisant les données de seed.

### Authentification

1. **Se connecter**
   ```http
   POST /api/auth/login
   Content-Type: application/json
   
   {
     "email": "admin@pharmacy.com",
     "password": "Admin123!"
   }
   ```

2. **Utiliser le token JWT**
   ```http
   Authorization: Bearer {votre_token_jwt}
   ```

## 📚 Documentation API

### Endpoints Principaux

#### 🔐 Authentification
- `POST /api/auth/login` - Connexion utilisateur
- `POST /api/auth/register` - Inscription (si activé)
- `GET /api/auth/me` - Informations de l'utilisateur connecté

#### 💊 Médicaments
- `GET /api/medicaments` - Liste tous les médicaments
- `GET /api/medicaments/{id}` - Détails d'un médicament
- `GET /api/medicaments/codebarre/{codeBarre}` - Recherche par code-barres
- `GET /api/medicaments/stock-faible` - Médicaments en stock faible
- `POST /api/medicaments` - Créer un médicament
- `PUT /api/medicaments/{id}` - Modifier un médicament
- `DELETE /api/medicaments/{id}` - Supprimer un médicament

#### 👥 Clients
- `GET /api/clients` - Liste tous les clients
- `GET /api/clients/{id}` - Détails d'un client
- `GET /api/clients/search/{searchTerm}` - Recherche de clients
- `POST /api/clients` - Créer un client
- `PUT /api/clients/{id}` - Modifier un client
- `DELETE /api/clients/{id}` - Supprimer un client

#### 💰 Ventes
- `GET /api/ventes` - Liste toutes les ventes
- `GET /api/ventes/{id}` - Détails d'une vente
- `POST /api/ventes` - Créer une vente
- `POST /api/ventes/calculer-total` - Calculer le total avant validation
- `POST /api/ventes/valider-stock` - Vérifier la disponibilité du stock

#### 📊 Stocks
- `GET /api/stocks` - Liste tous les stocks
- `GET /api/stocks/medicament/{medicamentId}` - Stocks d'un médicament
- `GET /api/stocks/expires` - Stocks expirés
- `GET /api/stocks/expire-bientot` - Stocks expirant bientôt
- `POST /api/stocks` - Créer un stock
- `PUT /api/stocks/{id}` - Modifier un stock
- `DELETE /api/stocks/{id}` - Supprimer un stock

#### 📋 Commandes
- `GET /api/commandes` - Liste toutes les commandes
- `GET /api/commandes/{id}` - Détails d'une commande
- `POST /api/commandes` - Créer une commande
- `PUT /api/commandes/{id}` - Modifier une commande

#### 📈 Dashboard
- `GET /api/dashboard/stats` - Statistiques générales
- `GET /api/dashboard/ventes-jour` - Ventes du jour
- `GET /api/dashboard/top-medicaments` - Médicaments les plus vendus

Pour la documentation complète et interactive, utilisez Swagger UI.

## 📁 Structure du Projet

```
PharmacyManagement.sln
│
├── src/
│   ├── PharmacyManagement.Domain/
│   │   ├── Entities/          # Entités du domaine
│   │   │   ├── Medicament.cs
│   │   │   ├── Client.cs
│   │   │   ├── Vente.cs
│   │   │   ├── Stock.cs
│   │   │   └── ...
│   │   └── PharmacyManagement.Domain.csproj
│   │
│   ├── PharmacyManagement.Infrastructure/
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── Configurations/    # Configurations EF Core
│   │   ├── Migrations/            # Migrations de base de données
│   │   ├── Repositories/          # Pattern Repository & Unit of Work
│   │   │   ├── IRepository.cs
│   │   │   ├── Repository.cs
│   │   │   ├── IUnitOfWork.cs
│   │   │   └── UnitOfWork.cs
│   │   └── PharmacyManagement.Infrastructure.csproj
│   │
│   ├── PharmacyManagement.Application/
│   │   ├── Services/              # Services métier
│   │   │   ├── IMedicamentService.cs
│   │   │   ├── MedicamentService.cs
│   │   │   ├── IClientService.cs
│   │   │   ├── ClientService.cs
│   │   │   └── ...
│   │   ├── DTOs/                  # Data Transfer Objects
│   │   │   ├── MedicamentDto.cs
│   │   │   ├── ClientDto.cs
│   │   │   └── ...
│   │   └── PharmacyManagement.Application.csproj
│   │
│   └── PharmacyManagement.API/
│       ├── Controllers/           # Contrôleurs REST
│       │   ├── MedicamentsController.cs
│       │   ├── ClientsController.cs
│       │   ├── VentesController.cs
│       │   └── ...
│       ├── Middleware/
│       │   └── ExceptionHandlingMiddleware.cs
│       ├── Data/
│       │   └── SeedData.cs       # Données initiales
│       ├── wwwroot/              # Fichiers statiques (frontend optionnel)
│       ├── Program.cs            # Point d'entrée
│       ├── appsettings.json      # Configuration
│       └── PharmacyManagement.API.csproj
│
├── .gitignore
├── README.md
└── PharmacyManagement.sln        # Solution Visual Studio
```

## 🔒 Sécurité

- Les mots de passe sont hashés avec BCrypt
- Authentification JWT avec expiration
- Middleware de gestion d'exceptions centralisé
- Validation des entrées utilisateur
- Protection CORS configurable

## 📝 Notes Importantes

- Les ventes utilisent un système **FIFO** (First In First Out) pour la gestion des stocks
- La **TVA est fixée à 20%** (configurable)
- Les numéros de facture sont générés automatiquement
- Les stocks expirés sont automatiquement exclus des calculs de disponibilité
- Les migrations Entity Framework sont versionnées dans le dépôt

## 🤝 Contribution

Les contributions sont les bienvenues ! N'hésitez pas à ouvrir une issue ou soumettre une pull request.

## 📄 Licence

Ce projet est sous licence MIT. Voir le fichier LICENSE pour plus de détails.

## 👨‍💻 Auteur

Développé avec ❤️ pour faciliter la gestion des pharmacies modernes.

---

**Note** : Ce projet est en développement actif. N'hésitez pas à signaler les bugs ou suggérer des améliorations !
