# 📋 Documentation Technique - Module Caissier

## 🎯 Vue d'ensemble

Le module Caissier est un système de Point de Vente (POS) complet et professionnel pour la gestion des ventes dans une pharmacie. Il offre une interface moderne, intuitive et performante pour les opérations de caisse.

---

## 🏗️ Architecture

### Structure des Couches

```
PharmacyManagement.Application/
├── DTOs/
│   └── CaissierDto.cs          # ViewModels pour le module Caissier
├── Services/
│   ├── ICaissierService.cs     # Interface du service
│   └── CaissierService.cs      # Implémentation du service

PharmacyManagement.API/
├── Controllers/
│   └── CaissierController.cs   # Contrôleur REST API
└── wwwroot/caissier/
    ├── dashboard.html           # Dashboard caissier
    ├── vente.html              # Page POS (Point de Vente)
    ├── historique.html         # Historique des ventes
    ├── clients.html            # Gestion clients
    ├── alertes.html            # Alertes (lecture seule)
    ├── caissier.css            # Styles modernes
    └── caissier.js             # Utilitaires JavaScript
```

---

## 📦 Modèles et DTOs

### ViewModels Principaux

#### `VenteCaissierViewModel`
Représente une vente en cours de création par le caissier.

```csharp
public class VenteCaissierViewModel
{
    public int? ClientId { get; set; }
    public string? ClientNom { get; set; }
    public string? ClientTelephone { get; set; }
    public List<LigneVenteViewModel> Lignes { get; set; }
    public string? Notes { get; set; }
    public string ModePaiement { get; set; } // Cash, Carte, Assurance, Differe
    public decimal? MontantRecu { get; set; }
}
```

#### `LigneVenteViewModel`
Représente un article dans le panier.

```csharp
public class LigneVenteViewModel
{
    public int MedicamentId { get; set; }
    public string MedicamentNom { get; set; }
    public string CodeBarre { get; set; }
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal SousTotal { get; set; }
    public int StockDisponible { get; set; }
    public bool RequiertPrescription { get; set; }
}
```

#### `DashboardCaissierViewModel`
Données du dashboard spécifique au caissier.

```csharp
public class DashboardCaissierViewModel
{
    public int TotalVentesAujourdhui { get; set; }
    public decimal ChiffreAffairesAujourdhui { get; set; }
    public decimal ChiffreAffairesMois { get; set; }
    public int NombreClientsAujourdhui { get; set; }
    public List<MedicamentPopulaireDto> MedicamentsPlusVendus { get; set; }
    public List<AlerteStockFaibleDto> AlertesStockFaible { get; set; }
    public List<VenteRecenteDto> VentesRecentes { get; set; }
}
```

---

## 🔌 API Endpoints REST

### Base URL: `/api/caissier`

#### Dashboard
- **GET** `/dashboard` - Récupère les statistiques du caissier

#### Recherche Médicaments
- **GET** `/medicaments/rechercher?terme={terme}` - Recherche par nom/catégorie
- **GET** `/medicaments/codebarre/{codeBarre}` - Recherche par code-barres

#### Validation
- **POST** `/valider-stock` - Valide la disponibilité du stock
- **POST** `/calculer-total` - Calcule le total avec TVA

#### Ventes
- **POST** `/ventes` - Crée une nouvelle vente
- **GET** `/ventes/historique?dateDebut={date}&dateFin={date}` - Historique des ventes
- **GET** `/ventes/{id}` - Détails d'une vente
- **POST** `/ventes/{id}/annuler` - Annule une vente
- **GET** `/ventes/{id}/facture-pdf` - Génère la facture PDF
- **GET** `/ventes/{id}/ticket` - Génère le ticket 80mm

#### Clients
- **GET** `/clients/rechercher?terme={terme}` - Recherche de clients
- **GET** `/clients/{id}` - Détails d'un client

#### Alertes (Lecture seule)
- **GET** `/alertes/stock-faible` - Liste des médicaments en stock faible
- **GET** `/alertes/expiration` - Stocks expirant dans 30 jours

---

## 🔐 Sécurité

Tous les endpoints sont protégés par :
```csharp
[Authorize(Policy = "CaissierOrAbove")]
```

Le contrôleur vérifie automatiquement l'identité de l'utilisateur via JWT et filtre les données selon le `UserId`.

---

## 💼 Logique Métier

### Calcul TVA
- **Taux TVA**: 20%
- **Formule**: `MontantHT * 0.20 = MontantTVA`
- **Total TTC**: `MontantHT + MontantTVA`

### Validation Stock
- Vérification en temps réel avant ajout au panier
- Vérification avant finalisation de la vente
- Alerte si stock insuffisant
- Alerte si stock < stock minimum

### Gestion des Ventes
1. **Création**: Validation stock → Calcul totaux → Enregistrement → Mise à jour stock
2. **Annulation**: Restauration stock → Marquage "Annulee" → Enregistrement raison
3. **Retour**: Restauration partielle stock → Marquage "Retournee"

### Modes de Paiement
- **Cash**: Calcul automatique de la monnaie à rendre
- **Carte**: Validation immédiate
- **Assurance**: Enregistrement sans paiement immédiat
- **Différé**: Créance client

---

## 🎨 Interface Utilisateur

### Design System
- **Couleurs**: Gradient violet-bleu (#667eea → #764ba2)
- **Typographie**: Segoe UI, Tahoma
- **Composants**: Bootstrap 5.3.0
- **Icônes**: Bootstrap Icons 1.11.0
- **Style**: Cartes arrondies, ombres douces, transitions fluides

### Pages

#### 1. Dashboard (`dashboard.html`)
- 4 cartes statistiques (gradients colorés)
- Tableau médicaments plus vendus
- Tableau alertes stock faible
- Liste ventes récentes

#### 2. Point de Vente (`vente.html`)
- **Layout**: 2 colonnes (Recherche | Panier)
- **Recherche**: Code-barres + Recherche texte
- **Panier**: Items avec quantité modifiable
- **Total**: HT, TVA, TTC calculés automatiquement
- **Paiement**: Sélection mode + calcul monnaie

#### 3. Historique (`historique.html`)
- Filtres par date
- Tableau des ventes avec statut
- Actions: Voir détails, Annuler, Imprimer

#### 4. Clients (`clients.html`)
- Recherche instantanée
- Affichage carte client
- Sélection rapide pour vente

#### 5. Alertes (`alertes.html`)
- Stock faible (rouge)
- Expirations proches (orange)
- Lecture seule

---

## 🔍 Fonctionnalités Avancées

### Recherche Instantanée
- Recherche par nom, code-barres, catégorie
- Résultats en temps réel (déclenchement après 2 caractères)
- Affichage: Nom, catégorie, stock, prix

### Scan Code-barres
- Champ dédié avec focus automatique
- Validation au Enter
- Ajout automatique au panier si trouvé

### Validation en Temps Réel
- Vérification stock avant ajout
- Vérification avant finalisation
- Messages d'erreur clairs

### Calcul Automatique
- Sous-totaux par ligne
- Total HT
- TVA (20%)
- Total TTC
- Monnaie à rendre (si cash)

---

## 📊 Base de Données

### Tables Utilisées

#### `Ventes`
```sql
CREATE TABLE Ventes (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    DateVente DATETIME NOT NULL,
    MontantTotal DECIMAL(18,2) NOT NULL,
    MontantTVA DECIMAL(18,2) NOT NULL,
    NumeroFacture VARCHAR(50) UNIQUE NOT NULL,
    Notes TEXT,
    Statut VARCHAR(50) DEFAULT 'Normal',
    DateAnnulation DATETIME NULL,
    RaisonAnnulation TEXT NULL,
    UserId INT NULL,
    ClientId INT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (ClientId) REFERENCES Clients(Id)
);
```

#### `VenteDetails`
```sql
CREATE TABLE VenteDetails (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    VenteId INT NOT NULL,
    MedicamentId INT NOT NULL,
    Quantite INT NOT NULL,
    PrixUnitaire DECIMAL(18,2) NOT NULL,
    SousTotal DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (VenteId) REFERENCES Ventes(Id) ON DELETE CASCADE,
    FOREIGN KEY (MedicamentId) REFERENCES Medicaments(Id)
);
```

---

## ✅ Validations Métier

### Stock
- ❌ Quantité > Stock disponible → **Erreur**
- ❌ Quantité <= 0 → **Erreur**
- ✅ Vérification avant chaque ajout
- ✅ Vérification avant finalisation

### Paiement Cash
- ❌ Montant reçu < Total → **Erreur**
- ✅ Calcul automatique monnaie à rendre

### Vente
- ❌ Panier vide → **Erreur**
- ✅ Client optionnel
- ✅ Notes optionnelles

---

## 🖨️ Impression

### Facture PDF (À implémenter)
- Format A4
- En-tête pharmacie
- Détails client
- Liste articles
- Totaux (HT, TVA, TTC)
- Pied de page avec signature

### Ticket 80mm (À implémenter)
- Format thermique
- Informations essentielles
- Code-barres facture
- QR Code (optionnel)

---

## 🚀 Utilisation

### Accès au Module
1. Se connecter avec un compte **Caissier**
2. Rediriger vers `/caissier/dashboard.html`
3. Navigation via sidebar

### Créer une Vente
1. Aller sur "Nouvelle Vente"
2. Rechercher/Scanner médicament
3. Ajouter au panier
4. (Optionnel) Sélectionner client
5. Choisir mode paiement
6. Finaliser

### Consulter Historique
1. Aller sur "Historique"
2. Filtrer par dates
3. Voir détails / Imprimer / Annuler

---

## 📝 Messages d'Erreur UX

- **Stock insuffisant**: "Stock insuffisant pour [Médicament]"
- **Panier vide**: "Le panier ne peut pas être vide"
- **Montant insuffisant**: "Le montant reçu doit être supérieur ou égal au total"
- **Médicament non trouvé**: "Aucun médicament trouvé"
- **Erreur serveur**: "Une erreur s'est produite. Veuillez réessayer."

---

## 🔄 Flux de Données

### Création Vente
```
Interface → API POST /ventes
    ↓
CaissierService.CreerVenteAsync()
    ↓
VenteService.CreateVenteAsync()
    ↓
Validation Stock → Création Vente → Mise à jour Stock
    ↓
Retour VenteCompleteViewModel
```

### Recherche Médicament
```
Interface → API GET /medicaments/rechercher
    ↓
CaissierService.RechercherMedicamentAsync()
    ↓
Query DB → Retour RechercheMedicamentDto
```

---

## 🎓 Points Techniques pour Jury

1. **Architecture Clean**: Séparation claire des responsabilités
2. **Sécurité**: JWT + Rôles + Filtrage par utilisateur
3. **Performance**: Requêtes optimisées, indexation DB
4. **UX**: Interface moderne, responsive, intuitive
5. **Validation**: Multi-niveaux (client, serveur, base)
6. **Extensibilité**: Facile d'ajouter modes paiement, impressions
7. **Maintenabilité**: Code structuré, documenté, testable

---

## 📚 Technologies Utilisées

- **Backend**: .NET 8.0, ASP.NET Core Web API
- **ORM**: Entity Framework Core 8.0
- **Base de données**: MySQL (Pomelo.EntityFrameworkCore.MySql)
- **Frontend**: HTML5, CSS3, JavaScript ES6+, Bootstrap 5.3
- **Authentification**: JWT Bearer
- **Architecture**: Repository Pattern, Unit of Work, Service Layer

---

## 🔮 Améliorations Futures

- [ ] Impression PDF avec QuestPDF ou iTextSharp
- [ ] Impression ticket thermique 80mm
- [ ] Mode hors-ligne avec synchronisation
- [ ] Suggestion médicaments similaires
- [ ] Points de fidélité clients
- [ ] Statistiques avancées par caissier
- [ ] Export Excel des ventes
- [ ] Notifications temps réel (SignalR)

---

## 📞 Support

Pour toute question technique, consulter :
- Code source dans `src/PharmacyManagement.API/Controllers/CaissierController.cs`
- Service métier dans `src/PharmacyManagement.Application/Services/CaissierService.cs`
- Interface dans `src/PharmacyManagement.API/wwwroot/caissier/`

---

**Module développé avec ❤️ pour un système de gestion de pharmacie professionnel**

