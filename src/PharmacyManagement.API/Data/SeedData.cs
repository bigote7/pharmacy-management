using BCrypt.Net;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.API.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Créer les catégories seulement si elles n'existent pas
        bool categoriesExist = context.Categories.Any();

        // Créer des catégories seulement si elles n'existent pas
        var categories = context.Categories.ToList();
        if (!categoriesExist)
        {
            var newCategories = new[]
            {
                new Categorie { Nom = "Antibiotiques", Description = "Médicaments antibiotiques" },
                new Categorie { Nom = "Antalgiques", Description = "Médicaments contre la douleur" },
                new Categorie { Nom = "Vitamines", Description = "Compléments vitaminiques" },
                new Categorie { Nom = "Antihistaminiques", Description = "Médicaments contre les allergies" },
                new Categorie { Nom = "Cardiovasculaires", Description = "Médicaments pour le cœur" }
            };

            context.Categories.AddRange(newCategories);
            context.SaveChanges();
            categories = context.Categories.ToList();
        }

        // Créer des médicaments seulement s'ils n'existent pas
        if (!context.Medicaments.Any())
        {
            var medicaments = new[]
        {
            new Medicament
            {
                Nom = "Paracétamol 500mg",
                CodeBarre = "1234567890123",
                Dosage = "500mg",
                Forme = "Comprimé",
                PrixAchat = 2.50m,
                PrixVente = 5.00m,
                StockMinimum = 50,
                RequiertPrescription = false,
                EstActif = true,
                CategorieId = categories[1].Id // Antalgiques
            },
            new Medicament
            {
                Nom = "Amoxicilline 500mg",
                CodeBarre = "1234567890124",
                Dosage = "500mg",
                Forme = "Gélule",
                PrixAchat = 8.00m,
                PrixVente = 15.00m,
                StockMinimum = 30,
                RequiertPrescription = true,
                EstActif = true,
                CategorieId = categories[0].Id // Antibiotiques
            },
            new Medicament
            {
                Nom = "Vitamine D3 1000 UI",
                CodeBarre = "1234567890125",
                Dosage = "1000 UI",
                Forme = "Gélule",
                PrixAchat = 3.00m,
                PrixVente = 6.00m,
                StockMinimum = 40,
                RequiertPrescription = false,
                EstActif = true,
                CategorieId = categories[2].Id // Vitamines
            },
            new Medicament
            {
                Nom = "Ibuprofène 400mg",
                CodeBarre = "1234567890126",
                Dosage = "400mg",
                Forme = "Comprimé",
                PrixAchat = 3.50m,
                PrixVente = 7.00m,
                StockMinimum = 50,
                RequiertPrescription = false,
                EstActif = true,
                CategorieId = categories[1].Id // Antalgiques
            }
        };

            context.Medicaments.AddRange(medicaments);
            context.SaveChanges();
        }

        // Créer des stocks seulement s'ils n'existent pas
        if (!context.Stocks.Any() && context.Medicaments.Any())
        {
            var medicamentsList = context.Medicaments.OrderBy(m => m.Id).Take(4).ToList();
            if (medicamentsList.Count >= 4)
            {
                var stocks = new[]
                {
                    new Stock
                    {
                        MedicamentId = medicamentsList[0].Id,
                Quantite = 100,
                DateEntree = DateTime.Now.AddDays(-30),
                DateExpiration = DateTime.Now.AddYears(2),
                NumeroLot = "LOT-001"
            },
                    new Stock
                    {
                        MedicamentId = medicamentsList[1].Id,
                        Quantite = 50,
                        DateEntree = DateTime.Now.AddDays(-20),
                        DateExpiration = DateTime.Now.AddYears(1),
                        NumeroLot = "LOT-002"
                    },
                    new Stock
                    {
                        MedicamentId = medicamentsList[2].Id,
                        Quantite = 80,
                        DateEntree = DateTime.Now.AddDays(-15),
                        DateExpiration = DateTime.Now.AddYears(3),
                        NumeroLot = "LOT-003"
                    },
                    new Stock
                    {
                        MedicamentId = medicamentsList[3].Id,
                        Quantite = 120,
                        DateEntree = DateTime.Now.AddDays(-10),
                        DateExpiration = DateTime.Now.AddYears(2),
                        NumeroLot = "LOT-004"
                    }
                };

                context.Stocks.AddRange(stocks);
                context.SaveChanges();
            }
        }

        // Créer des clients seulement s'ils n'existent pas
        if (!context.Clients.Any())
        {
        var clients = new[]
        {
            new Client
            {
                Nom = "Alami",
                Prenom = "Ahmed",
                Telephone = "0612345678",
                Email = "ahmed.alami@example.com",
                DateNaissance = new DateTime(1985, 5, 15),
                Adresse = "123 Rue Mohammed V, Casablanca"
            },
            new Client
            {
                Nom = "Benali",
                Prenom = "Fatima",
                Telephone = "0623456789",
                Email = "fatima.benali@example.com",
                DateNaissance = new DateTime(1990, 8, 22),
                Adresse = "456 Avenue Hassan II, Rabat"
            },
            new Client
            {
                Nom = "Idrissi",
                Prenom = "Mohammed",
                Telephone = "0634567890",
                Email = "mohammed.idrissi@example.com",
                DateNaissance = new DateTime(1978, 3, 10),
                Adresse = "789 Boulevard Zerktouni, Marrakech"
            }
        };

            context.Clients.AddRange(clients);
            context.SaveChanges();
        }

        // Créer un fournisseur seulement s'il n'existe pas
        if (!context.Fournisseurs.Any())
        {
        var fournisseur = new Fournisseur
        {
            Nom = "Pharma Distribution SARL",
            Telephone = "0522123456",
            Email = "contact@pharmadist.ma",
            Adresse = "100 Avenue Allal Ben Abdellah, Casablanca",
            EstActif = true
        };

            context.Fournisseurs.Add(fournisseur);
            context.SaveChanges();
        }

        // Créer les rôles
        var roles = new[]
        {
            new Role { Nom = "Administrateur", Description = "Accès complet à tous les modules" },
            new Role { Nom = "Pharmacien", Description = "Gestion des médicaments, prescriptions et stocks" },
            new Role { Nom = "Caissier", Description = "Gestion des ventes uniquement" }
        };

        foreach (var role in roles)
        {
            if (!context.Roles.Any(r => r.Nom == role.Nom))
            {
                context.Roles.Add(role);
            }
        }
        context.SaveChanges();

        // Créer les utilisateurs par défaut
        var adminRole = context.Roles.FirstOrDefault(r => r.Nom == "Administrateur");
        var pharmacienRole = context.Roles.FirstOrDefault(r => r.Nom == "Pharmacien");
        var caissierRole = context.Roles.FirstOrDefault(r => r.Nom == "Caissier");

        // Créer/réinitialiser le compte admin
        if (adminRole != null)
        {
            var existingAdmin = context.Users.FirstOrDefault(u => u.Username == "admin");
            if (existingAdmin == null)
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    Email = "admin@pharmacy.ma",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FullName = "Administrateur",
                    RoleId = adminRole.Id,
                    EstActif = true,
                    DateCreation = DateTime.Now
                });
            }
            else
            {
                existingAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
                existingAdmin.EstActif = true;
                existingAdmin.RoleId = adminRole.Id;
                context.Users.Update(existingAdmin);
            }
        }

        // Créer/réinitialiser le compte pharmacien
        if (pharmacienRole != null)
        {
            var existingPharmacien = context.Users.FirstOrDefault(u => u.Username == "pharmacien");
            if (existingPharmacien == null)
            {
                context.Users.Add(new User
                {
                    Username = "pharmacien",
                    Email = "pharmacien@pharmacy.ma",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("pharmacien123"),
                    FullName = "Pharmacien Test",
                    RoleId = pharmacienRole.Id,
                    EstActif = true,
                    DateCreation = DateTime.Now
                });
            }
            else
            {
                existingPharmacien.PasswordHash = BCrypt.Net.BCrypt.HashPassword("pharmacien123");
                existingPharmacien.EstActif = true;
                existingPharmacien.RoleId = pharmacienRole.Id;
                context.Users.Update(existingPharmacien);
            }
        }

        // Créer/réinitialiser le compte caissier
        if (caissierRole != null)
        {
            var existingCaissier = context.Users.FirstOrDefault(u => u.Username == "caissier");
            if (existingCaissier == null)
            {
                context.Users.Add(new User
                {
                    Username = "caissier",
                    Email = "caissier@pharmacy.ma",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("caissier123"),
                    FullName = "Caissier Test",
                    RoleId = caissierRole.Id,
                    EstActif = true,
                    DateCreation = DateTime.Now
                });
            }
            else
            {
                existingCaissier.PasswordHash = BCrypt.Net.BCrypt.HashPassword("caissier123");
                existingCaissier.EstActif = true;
                existingCaissier.RoleId = caissierRole.Id;
                context.Users.Update(existingCaissier);
            }
        }

        context.SaveChanges();
        Console.WriteLine("Données de test et utilisateurs par défaut créés/mis à jour avec succès !");
    }
}

