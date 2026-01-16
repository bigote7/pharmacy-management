-- ============================================
-- Script SQL pour le Module Caissier
-- Système de Gestion de Pharmacie
-- ============================================

-- Table Ventes (avec champs pour le module Caissier)
CREATE TABLE IF NOT EXISTS `Ventes` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `DateVente` DATETIME(6) NOT NULL,
    `MontantTotal` DECIMAL(18,2) NOT NULL,
    `MontantTVA` DECIMAL(18,2) NOT NULL,
    `NumeroFacture` VARCHAR(50) CHARACTER SET utf8mb4 NOT NULL,
    `Notes` LONGTEXT CHARACTER SET utf8mb4 NULL,
    `Statut` VARCHAR(50) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'Normal',
    `DateAnnulation` DATETIME(6) NULL,
    `RaisonAnnulation` LONGTEXT CHARACTER SET utf8mb4 NULL,
    `UserId` INT NULL,
    `ClientId` INT NULL,
    CONSTRAINT `PK_Ventes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Ventes_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Ventes_Clients_ClientId` FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `IX_Ventes_NumeroFacture` UNIQUE (`NumeroFacture`),
    INDEX `IX_Ventes_UserId` (`UserId`),
    INDEX `IX_Ventes_ClientId` (`ClientId`),
    INDEX `IX_Ventes_DateVente` (`DateVente`),
    INDEX `IX_Ventes_Statut` (`Statut`)
) CHARACTER SET=utf8mb4;

-- Table VenteDetails (Lignes de vente)
CREATE TABLE IF NOT EXISTS `VenteDetails` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `VenteId` INT NOT NULL,
    `MedicamentId` INT NOT NULL,
    `Quantite` INT NOT NULL,
    `PrixUnitaire` DECIMAL(18,2) NOT NULL,
    `SousTotal` DECIMAL(18,2) NOT NULL,
    CONSTRAINT `PK_VenteDetails` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_VenteDetails_Ventes_VenteId` FOREIGN KEY (`VenteId`) REFERENCES `Ventes` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_VenteDetails_Medicaments_MedicamentId` FOREIGN KEY (`MedicamentId`) REFERENCES `Medicaments` (`Id`) ON DELETE RESTRICT,
    INDEX `IX_VenteDetails_VenteId` (`VenteId`),
    INDEX `IX_VenteDetails_MedicamentId` (`MedicamentId`)
) CHARACTER SET=utf8mb4;

-- Index pour optimiser les requêtes du caissier
CREATE INDEX IF NOT EXISTS `IX_Ventes_User_Date` ON `Ventes` (`UserId`, `DateVente`);
CREATE INDEX IF NOT EXISTS `IX_Ventes_Statut_Date` ON `Ventes` (`Statut`, `DateVente`);

-- Vue pour statistiques caissier (optionnel)
CREATE OR REPLACE VIEW `VueStatistiquesCaissier` AS
SELECT 
    v.UserId,
    DATE(v.DateVente) AS DateVente,
    COUNT(*) AS NombreVentes,
    SUM(v.MontantTotal) AS ChiffreAffaires,
    COUNT(DISTINCT v.ClientId) AS NombreClients
FROM Ventes v
WHERE v.Statut = 'Normal' OR v.Statut IS NULL
GROUP BY v.UserId, DATE(v.DateVente);

-- ============================================
-- Requêtes utiles pour le module Caissier
-- ============================================

-- Ventes d'aujourd'hui par caissier
-- SELECT * FROM Ventes 
-- WHERE UserId = ? AND DATE(DateVente) = CURDATE() AND (Statut = 'Normal' OR Statut IS NULL);

-- Chiffre d'affaires du mois par caissier
-- SELECT SUM(MontantTotal) FROM Ventes 
-- WHERE UserId = ? AND MONTH(DateVente) = MONTH(CURDATE()) 
-- AND YEAR(DateVente) = YEAR(CURDATE()) AND (Statut = 'Normal' OR Statut IS NULL);

-- Médicaments les plus vendus par caissier
-- SELECT m.Nom, SUM(vd.Quantite) AS QuantiteVendue, SUM(vd.SousTotal) AS CA
-- FROM VenteDetails vd
-- INNER JOIN Ventes v ON vd.VenteId = v.Id
-- INNER JOIN Medicaments m ON vd.MedicamentId = m.Id
-- WHERE v.UserId = ? AND (v.Statut = 'Normal' OR v.Statut IS NULL)
-- GROUP BY m.Id, m.Nom
-- ORDER BY QuantiteVendue DESC
-- LIMIT 5;

