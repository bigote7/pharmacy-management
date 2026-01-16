using PharmacyManagement.Application.DTOs;

namespace PharmacyManagement.API.Validators;

public static class CreerMedicamentValidator
{
    public static List<string> Validate(CreerMedicamentDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Nom))
            errors.Add("Le nom du médicament est requis");

        if (string.IsNullOrWhiteSpace(dto.CodeBarre))
            errors.Add("Le code-barres est requis");

        if (dto.PrixAchat <= 0)
            errors.Add("Le prix d'achat doit être supérieur à 0");

        if (dto.PrixVente <= 0)
            errors.Add("Le prix de vente doit être supérieur à 0");

        // Note: On permet que le prix de vente soit inférieur au prix d'achat
        // pour permettre les promotions et les ventes à perte contrôlées

        if (dto.StockMinimum < 0)
            errors.Add("Le stock minimum ne peut pas être négatif");

        if (dto.CategorieId <= 0)
            errors.Add("La catégorie est requise");

        return errors;
    }
}

