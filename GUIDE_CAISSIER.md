# 🎯 Guide d'Utilisation - Module Caissier

## 🚀 Démarrage Rapide

### 1. Accès au Module

1. **Se connecter** avec un compte Caissier :
   - Username: `caissier`
   - Password: `caissier123`

2. **Redirection automatique** vers `/caissier/dashboard.html`

### 2. Navigation

Le module dispose de 5 pages principales accessibles via la sidebar :

- 📊 **Dashboard** : Vue d'ensemble des statistiques
- 🛒 **Nouvelle Vente** : Point de vente (POS)
- 📜 **Historique** : Liste des ventes effectuées
- 👥 **Clients** : Recherche et gestion clients
- ⚠️ **Alertes** : Stock faible et expirations (lecture seule)

---

## 💰 Créer une Vente

### Étape 1 : Rechercher un Médicament

**Option A - Scan Code-barres :**
1. Cliquer dans le champ "Scanner Code-barres"
2. Scanner ou saisir le code-barres
3. Appuyer sur **Enter**
4. Le médicament est automatiquement ajouté au panier

**Option B - Recherche par Nom :**
1. Saisir le nom du médicament (minimum 2 caractères)
2. Sélectionner le médicament dans les résultats
3. Cliquer sur "Ajouter"

### Étape 2 : Gérer le Panier

- **Modifier quantité** : Boutons `-` et `+`
- **Retirer un article** : Bouton 🗑️
- **Vider le panier** : Bouton "Vider le Panier"

### Étape 3 : Sélectionner un Client (Optionnel)

1. Saisir le nom/téléphone du client
2. Sélectionner dans les résultats
3. Le client est associé à la vente

### Étape 4 : Choisir le Mode de Paiement

- **Cash** : Saisir le montant reçu → Monnaie calculée automatiquement
- **Carte** : Validation immédiate
- **Assurance** : Enregistrement sans paiement
- **Différé** : Créance client

### Étape 5 : Finaliser

1. Cliquer sur **"Finaliser la Vente"**
2. Confirmation affichée
3. Options : Imprimer facture / Nouvelle vente

---

## 📊 Dashboard

### Statistiques Affichées

- **Ventes Aujourd'hui** : Nombre de ventes effectuées aujourd'hui
- **Chiffre d'Affaires** : Total encaissé aujourd'hui
- **CA du Mois** : Chiffre d'affaires du mois en cours
- **Clients Servis** : Nombre de clients différents servis aujourd'hui

### Tableaux

- **Médicaments Plus Vendus** : Top 5 des médicaments vendus ce mois
- **Alertes Stock Faible** : Médicaments nécessitant un réapprovisionnement
- **Ventes Récentes** : 10 dernières ventes avec actions rapides

---

## 📜 Historique des Ventes

### Fonctionnalités

1. **Filtrage par Date**
   - Sélectionner date début et date fin
   - Cliquer sur "Filtrer"

2. **Actions sur une Vente**
   - 👁️ **Voir détails** : Affiche tous les détails de la vente
   - 🖨️ **Imprimer** : Génère la facture PDF
   - ❌ **Annuler** : Annule la vente (si statut Normal)

3. **Informations Affichées**
   - Numéro de facture
   - Date et heure
   - Client
   - Nombre d'articles
   - Montant total
   - Statut (Normal, Annulée, Retournée)

---

## 👥 Gestion Clients

### Recherche

- Recherche instantanée par :
  - Nom
  - Prénom
  - Téléphone
  - Email

### Utilisation

1. Rechercher un client
2. Cliquer sur **"Utiliser"**
3. Redirection vers la page de vente avec client pré-sélectionné

---

## ⚠️ Alertes

### Types d'Alertes

1. **Stock Faible**
   - Médicaments dont le stock ≤ stock minimum
   - Affichage : Nom, catégorie, stock disponible, stock minimum

2. **Expirations Proches**
   - Stocks expirant dans les 30 prochains jours
   - Affichage : Médicament, numéro de lot, quantité, date expiration

**Note** : Les alertes sont en **lecture seule** pour le caissier.

---

## 🔍 Fonctionnalités Avancées

### Recherche Instantanée

- Déclenchement automatique après 2 caractères
- Recherche dans : nom, code-barres, catégorie
- Résultats en temps réel

### Validation Automatique

- ✅ Vérification stock avant ajout
- ✅ Vérification avant finalisation
- ✅ Alerte si stock insuffisant
- ✅ Alerte si quantité invalide

### Calcul Automatique

- Sous-totaux par ligne
- Total HT
- TVA (20%)
- Total TTC
- Monnaie à rendre (mode Cash)

---

## ⌨️ Raccourcis Clavier

- **Enter** dans champ code-barres → Ajoute au panier
- **Tab** → Navigation entre champs
- **Escape** → Ferme les modals

---

## 🎨 Design

### Couleurs

- **Primaire** : Gradient violet-bleu (#667eea → #764ba2)
- **Succès** : Vert (#28a745)
- **Danger** : Rouge (#dc3545)
- **Warning** : Orange (#ffc107)

### Composants

- Cartes arrondies avec ombres
- Badges colorés pour statuts
- Transitions fluides
- Design responsive (mobile-friendly)

---

## ❓ FAQ

### Q: Comment annuler une vente ?
**R:** Aller dans Historique → Cliquer sur ❌ → Saisir la raison → Confirmer

### Q: Le stock est insuffisant, que faire ?
**R:** Le système bloque automatiquement. Contacter le pharmacien pour réapprovisionner.

### Q: Comment imprimer une facture ?
**R:** Après création de vente → Cliquer "Imprimer Facture" OU Historique → Voir détails → Imprimer

### Q: Puis-je modifier une vente déjà créée ?
**R:** Non, mais vous pouvez l'annuler et créer une nouvelle vente.

### Q: Comment voir mes statistiques personnelles ?
**R:** Le Dashboard affiche uniquement VOS ventes et statistiques.

---

## 🛠️ Dépannage

### Erreur "Stock insuffisant"
- Vérifier le stock disponible dans les alertes
- Contacter le pharmacien

### Erreur "Erreur 500"
- Vérifier que la migration a été appliquée : `dotnet ef database update`
- Vérifier la connexion à la base de données

### Page blanche
- Vérifier la console du navigateur (F12)
- Vérifier que le token JWT est valide
- Se reconnecter si nécessaire

---

## 📞 Support Technique

Pour toute assistance :
1. Vérifier les logs de l'application
2. Consulter `MODULE_CAISSIER.md` pour la documentation technique
3. Vérifier la console du navigateur (F12) pour les erreurs JavaScript

---

**Module développé avec ❤️ pour une expérience caissier optimale**

