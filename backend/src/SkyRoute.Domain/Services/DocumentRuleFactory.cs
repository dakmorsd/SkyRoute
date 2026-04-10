using SkyRoute.Domain.Enums;

namespace SkyRoute.Domain.Services;

public static class DocumentRuleFactory
{
    public const string PassportPattern = "^[A-Z0-9]{6,9}$";
    public const string NationalIdPattern = "^\\d{7,8}$";

    public static DocumentRule Create(RouteType routeType)
    {
        return routeType == RouteType.International
            ? new DocumentRule(DocumentType.Passport, "Passport Number", PassportPattern)
            : new DocumentRule(DocumentType.NationalId, "National ID", NationalIdPattern);
    }
}