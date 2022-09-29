using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Authentication;

public static class AuthenticationClaims
{
    public const string OVERRIDE_USER_CLAIMS = "OVERRIDE_USER_CLAIMS";
    public const string MANAGE_USER_ROLES = "MANAGE_USER_ROLES";
    public const string MANAGE_USER_CLAIMS = "MANAGE_USER_CLAIMS";
    public const string MANAGE_ROLES = "MANAGE_ROLES";
    public const string MANAGE_CLAIMS = "MANAGE_CLAIMS";
    public const string CAN_OVERRIDE_CLAIM_PROTECTION = "CAN_OVERRIDE_CLAIM_PROTECTION";
}

public static class AuthenticationClaimDefinitions
{
    public static ClaimValue OVERRIDE_USER_CLAIMS => new ClaimValue(AuthenticationClaims.OVERRIDE_USER_CLAIMS, "Gerenciamento de permissões de acessos individuais");
    public static ClaimValue MANAGE_USER_ROLES => new ClaimValue(AuthenticationClaims.MANAGE_USER_ROLES, "Gerenciamento de regras de acesso de usuários");
    public static ClaimValue MANAGE_USER_CLAIMS => new ClaimValue(AuthenticationClaims.MANAGE_USER_CLAIMS, "Gerenciamento de permissões de acesso de usuários");
    public static ClaimValue MANAGE_ROLES => new ClaimValue(AuthenticationClaims.MANAGE_ROLES, "Gerenciamento de regras de acesso");    
    public static ClaimValue MANAGE_CLAIMS => new ClaimValue(AuthenticationClaims.MANAGE_CLAIMS, "Gerenciamento de permissões de acesso");
    public static ClaimValue CAN_OVERRIDE_CLAIM_PROTECTION => new ClaimValue(AuthenticationClaims.CAN_OVERRIDE_CLAIM_PROTECTION, "Proteger/desproteger permissões de acesso");
}

public class ClaimValue
{
    public ClaimValue(string name, string description)
    {
        Name = name;
        Description = description;
    }
    public string Name { get; set; }
    public string Description { get; set; }
}
