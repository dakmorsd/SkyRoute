using SkyRoute.Domain.Enums;

namespace SkyRoute.Domain.Services;

public sealed record DocumentRule(DocumentType Type, string Label, string Pattern);