# 🚀 Guide pour pousser le projet vers GitHub

Ce guide vous explique comment pousser votre projet .NET vers GitHub de manière professionnelle.

## 📋 Prérequis

1. Un compte GitHub (si vous n'en avez pas, créez-en un sur [github.com](https://github.com))
2. Git installé sur votre machine (déjà fait ✅)
3. Un dépôt Git initialisé (déjà fait ✅)

## 🎯 Étapes pour pousser vers GitHub

### Étape 1 : Créer un nouveau dépôt sur GitHub

1. Connectez-vous à votre compte GitHub
2. Cliquez sur le bouton **"+"** en haut à droite, puis sélectionnez **"New repository"**
3. Remplissez les informations :
   - **Repository name** : `pharmacy-management` (ou un nom de votre choix)
   - **Description** : `Système de gestion de pharmacie développé en .NET 8 avec architecture en couches`
   - **Visibility** : Choisissez **Public** (pour partager) ou **Private** (pour garder privé)
   - ⚠️ **IMPORTANT** : Ne cochez PAS "Initialize this repository with a README" car vous avez déjà un README local
   - Cliquez sur **"Create repository"**

### Étape 2 : Connecter votre dépôt local à GitHub

Une fois le dépôt créé sur GitHub, vous verrez une page avec des instructions. Copiez l'URL de votre dépôt (format : `https://github.com/votre-username/pharmacy-management.git`).

### Étape 3 : Ajouter le remote et pousser

Exécutez les commandes suivantes dans le terminal, dans le dossier du projet :

```bash
# Remplacer YOUR_USERNAME par votre nom d'utilisateur GitHub
# et REPO_NAME par le nom du dépôt que vous avez créé

cd "c:\Users\HP\Documents\.net prjt"

# Ajouter le dépôt GitHub comme remote (nommé "origin")
git remote add origin https://github.com/YOUR_USERNAME/REPO_NAME.git

# Renommer la branche principale en "main" (standard GitHub moderne)
git branch -M main

# Pousser le code vers GitHub
git push -u origin main
```

Si vous utilisez PowerShell et que vous avez des problèmes avec l'authentification, GitHub vous demandera vos identifiants.

### Étape 4 : Authentification GitHub

GitHub ne permet plus l'authentification par mot de passe via HTTPS. Vous avez deux options :

#### Option A : Utiliser un Personal Access Token (PAT)

1. Allez sur GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Cliquez sur **"Generate new token (classic)"**
3. Donnez un nom au token (ex: "Pharmacy Management Project")
4. Sélectionnez les scopes : **`repo`** (pour l'accès complet aux dépôts)
5. Cliquez sur **"Generate token"**
6. **Copiez le token** (vous ne pourrez plus le voir après !)
7. Lorsque Git vous demande le mot de passe, utilisez ce token au lieu de votre mot de passe GitHub

#### Option B : Utiliser SSH (recommandé pour un usage régulier)

```bash
# Générer une clé SSH (si vous n'en avez pas)
ssh-keygen -t ed25519 -C "votre_email@example.com"

# Copier la clé publique
cat ~/.ssh/id_ed25519.pub

# Ajouter la clé sur GitHub :
# GitHub → Settings → SSH and GPG keys → New SSH key
# Collez le contenu de la clé publique

# Changer l'URL du remote pour utiliser SSH
git remote set-url origin git@github.com:YOUR_USERNAME/REPO_NAME.git

# Pousser
git push -u origin main
```

## ✅ Vérification

Après avoir poussé avec succès, vous devriez voir :
- ✅ Tous vos fichiers sur GitHub
- ✅ Le README.md affiché sur la page principale du dépôt
- ✅ L'historique des commits

## 📝 Commandes Git utiles pour la suite

```bash
# Vérifier l'état des fichiers
git status

# Ajouter des fichiers modifiés
git add .

# Créer un commit
git commit -m "Description des modifications"

# Pousser vers GitHub
git push

# Voir l'historique des commits
git log --oneline

# Voir les remotes configurés
git remote -v
```

## 🔐 Sécurité

⚠️ **Important** : Vérifiez que ces fichiers sont bien exclus du dépôt :
- `appsettings.Development.json` (devrait être ignoré par .gitignore)
- Tous les fichiers contenant des mots de passe ou clés secrètes

Si vous avez accidentellement commité des fichiers sensibles, contactez-nous pour les retirer de l'historique.

## 🎉 Félicitations !

Votre projet est maintenant sur GitHub et prêt à être partagé avec le monde ! 🌟

---

**Besoin d'aide ?** Consultez la [documentation officielle de GitHub](https://docs.github.com/en/get-started/quickstart/create-a-repo).
