# 🔐 Système d'Authentification JWT

## ✅ Implémentation Complète

### Fonctionnalités Implémentées

1. **Entités**
   - ✅ User (Utilisateur)
   - ✅ Role (Rôle)

2. **Authentification JWT**
   - ✅ Login avec génération de token
   - ✅ Register (réservé aux administrateurs)
   - ✅ Hashage des mots de passe avec BCrypt
   - ✅ Validation des tokens JWT

3. **Rôles Disponibles**
   - ✅ **Administrateur** - Accès complet
   - ✅ **Pharmacien** - Gestion opérationnelle
   - ✅ **Caissier** - Ventes uniquement

4. **Sécurité**
   - ✅ Mots de passe hashés (BCrypt)
   - ✅ Tokens JWT avec expiration (24h)
   - ✅ Refresh tokens
   - ✅ Middleware d'authentification
   - ✅ Politiques d'autorisation par rôle

5. **Interface**
   - ✅ Page de connexion (`/login.html`)
   - ✅ Vérification automatique de l'authentification
   - ✅ Affichage de l'utilisateur connecté
   - ✅ Bouton de déconnexion

## 🔑 Compte par Défaut

Lors de la première initialisation, un compte administrateur est créé automatiquement :

- **Username** : `admin`
- **Password** : `admin123`
- **Rôle** : Administrateur

⚠️ **Important** : Changez le mot de passe après la première connexion !

## 📋 Endpoints API

### Authentification

- `POST /api/auth/login` - Connexion
  ```json
  {
    "username": "admin",
    "password": "admin123"
  }
  ```

- `POST /api/auth/register` - Inscription (Admin uniquement)
  ```json
  {
    "username": "nouveau_user",
    "email": "user@example.com",
    "password": "motdepasse",
    "fullName": "Nom Complet",
    "roleId": 2
  }
  ```

- `GET /api/auth/roles` - Liste des rôles disponibles

- `POST /api/auth/refresh` - Rafraîchir le token

## 🔒 Protection des Endpoints

Les endpoints peuvent être protégés avec les attributs suivants :

```csharp
[Authorize] // Nécessite une authentification
[Authorize(Roles = "Administrateur")] // Admin uniquement
[Authorize(Roles = "Pharmacien,Administrateur")] // Pharmacien ou Admin
```

## 📝 Politiques d'Autorisation

Trois politiques sont configurées :

1. **AdminOnly** - Administrateur uniquement
2. **PharmacienOrAdmin** - Pharmacien ou Administrateur
3. **CaissierOrAbove** - Caissier, Pharmacien ou Administrateur

## 🚀 Utilisation

1. **Première connexion** :
   - Allez sur `/login.html`
   - Utilisez : `admin` / `admin123`

2. **Créer de nouveaux utilisateurs** :
   - Connectez-vous en tant qu'administrateur
   - Utilisez l'endpoint `/api/auth/register` via Swagger

3. **Protéger vos endpoints** :
   - Ajoutez `[Authorize]` ou `[Authorize(Roles = "...")]` aux contrôleurs

## 🔐 Configuration JWT

La configuration JWT se trouve dans `appsettings.json` :

```json
{
  "Jwt": {
    "Key": "VotreCleSecrete...",
    "Issuer": "PharmacyManagement",
    "Audience": "PharmacyManagement",
    "ExpirationHours": 24
  }
}
```

⚠️ **Important** : Changez la clé JWT en production !

## 📊 Base de Données

Deux nouvelles tables sont créées :

- **Users** - Utilisateurs avec hash de mot de passe
- **Roles** - Rôles disponibles

## 🔄 Prochaines Étapes

Pour compléter le système :

1. Implémenter le refresh token avec stockage en base
2. Ajouter la blacklist des tokens (logout)
3. Ajouter la gestion des permissions granulaires
4. Ajouter les logs d'activité
5. Ajouter la réinitialisation de mot de passe

