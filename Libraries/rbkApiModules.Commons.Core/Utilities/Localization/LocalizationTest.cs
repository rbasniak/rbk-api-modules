﻿using System.ComponentModel;

namespace rbkApiModules.Commons.Core.Utilities.Localization;

public interface ILocalizedResource
{

}

public class AuthenticationMessages : ILocalizedResource
{
    public enum Fields
    {
        [Description("User")] User,
        [Description("Role")] Role,
        [Description("Claim")] Claim,
        [Description("Name")] Name,
        [Description("Description")] Description,
        [Description("Identification")] Identification,
        [Description("Password")] Password,
        [Description("Destination domain")] DestinationDomain,
        [Description("Tenant")] Tenant,
        [Description("E-mail")] Email,
        [Description("Refresh token")] RefreshToken,
        [Description("Password reset code")] PasswordResetCode,
        [Description("Activation code")] ActivationCode,
        [Description("Tenant")] TenantAlias,
        [Description("Display name")] DisplayName,
    }

    public enum Emails
    {
        [Description("Registration confirmation")] RegistrationConfirmed,
        [Description("Welcome")] Welcome,
        [Description("Password reset")] PasswordReset,
        [Description("Password successfully reset")] PasswordSuccessfullyReset,
    }

    public enum Validations
    {
        [Description("Role not found")] RoleNotFound,
        [Description("Claim not found")] ClaimNotFound,
        [Description("Tenant not found")] TenantNotFound,
        [Description("User not found")] UserNotFound,
        [Description("Unauthorized access")] UnauthorizedAccess,
        [Description("The list of roles must have at least one item")] RoleListMustNotBeEmpty,
        [Description("Claim is not overrided in the user")] ClaimNotOverridedInUser,
        [Description("The list of claims must have at least one item")] ClaimListMustNotBeEmpty,
        [Description("Unknown claim in the list")] UnknownClaimInTheList,
        [Description("Name already used")] NameAlreadyUsed,
        [Description("Role associated with one or more users")] RoleIsBeingUsed,
        [Description("There is already a claim with this description")] ClaimDescriptionAlreadyUsed,
        [Description("Cannot remove a claim that is being used by any roles")] CannotRemoveClaimUsedByOtherRoles,
        [Description("Cannot remove a system protected claim")] CannotRemoveSystemProtectedClaims,
        [Description("Cannot remove a claim that is being used in any users")] CannotRemoveClaimAssociatedWithUsers,
        [Description("There is already a claim with this identification")] ClaimIdentificationAlreadyUsed,
        [Description("Invalid credentials")] InvalidCredentials,
        [Description("User not yet confirmed")] UserNotYetConfirmed,
        [Description("E-mail not found")] EmailNotFound,
        [Description("E-mail already confirmed")] EmailAlreadyConfirmed,
        [Description("E-mail not confirmed")] EmailNotYetConfirmed,
        [Description("Refresh token does not exist anymore")] RefreshTokenNotFound,
        [Description("Refresh token expired")] RefreshTokenExpired,
        [Description("The password reset is code expired or was already used")] PasswordResetCodeExpiredOrUsed,
        [Description("The password reset is code expired or was already used")] InvalidActivationCode,
        [Description("Admin user data is required")] AdminUserDataIsRequired,
        [Description("Tenant alias already used")] TenantAliasAlreadyUsed,
        [Description("Password and confirmation must match")] PasswordsMustBeTheSame,
        [Description("Old password does not match")] OldPasswordDoesNotMatch,
        [Description("User already exists")] UserAlreadyExists,
        [Description("E-mail already used")] EmailAlreadyUsed,
        [Description("Password is required")] PasswordIsRequired,
        [Description("User cannot delete itself")] UserCannotDeleteItselft,
        [Description("User cannot deactivate itself")] UserCannotDeactivateItselft,
        [Description("Account is deactivated")] AccountDeactivated,
        [Description("Invalid login mode")] InvalidLoginMode,
        [Description("Cannot rename the default user role when the application allow for automatic user creation")] CannotRenameDefaultUserRole,
        [Description("Cannot delete the default user role when the application allow for automatic user creation")] CannotDeleteDefaultUserRole,
    }

    public enum Erros
    {
        [Description("Cannot delete the tenant, probably it has data associated with it")] CannotDeleteUsedTenant,
        [Description("Could not find the user associate with that password reset code")] CouldNotFindTheUserAssociatedWithThePasswordResetCode,
        [Description("Could not delete the user, ensure it is not being used by any related entities")] CannotDeleteUser,
        [Description("Could not find the default role for a new user")] CannotFindDefaultRole
    }
}


public class CommentMessages : ILocalizedResource
{
    public enum Fields
    {
        [Description("Message")] Comment,
    }

    public enum Validation
    {
        [Description("Could not find parent comment")] CouldNotFindParentComment,
    }
}

public class FaqMessages : ILocalizedResource
{
    public enum Fields
    {
        [Description("Tag")] Tag,
        [Description("Question")] Question,
        [Description("Answer")] Answer,
    }

    public enum Validation
    {
        [Description("Entity not found")] EntityNotFound,
    }

    public enum Errors
    {
        [Description("Access denied")] AccessDenied,
    }
}

public class WorkflowMessages : ILocalizedResource
{
    public enum Validation
    {
        [Description("State group not found")] StateGroupNotFound,
    }
}



public class SharedValidationMessages: ILocalizedResource
{
    public enum Common
    {
        [Description("The field '{PropertyName}' cannot be empty")] FieldCannotBeEmpty,
        [Description("The field '{PropertyName}' already exists in the database")] FieldValueAlreadyUsedInDatabase,
        [Description("The field '{PropertyName}' cannot be null")] FieldCannotBeNull,
        [Description("The field '{PropertyName}' must have between {0} and {1} characters")] FieldMustHaveLengthBetweenMinAndMax,
        [Description("The field '{PropertyName}' must have {0} characters")] FieldMustHaveFixedLength,
        [Description("E-mail is invalid")] InvalidEmailFormat,
        [Description("Id is invalid")] InvalidIdFormat,
        [Description("'{PropertyName}' not found in database")] EntityNotFoundInDatabase,
        [Description("Request expected to require authentication")] RequestExpectedToRequireAuthentication,
        [Description("Request expected to be authenticated")] RequestExpectedToBeAuthenticated,
        [Description("Possible unauthorized access")] PossibleUnauthorizedAccess,
    }

    public enum Errors
    {
        [Description("Internal server error")] InternalServerError,
    }
}

public class SharedUiMessages: ILocalizedResource
{
    public enum Errors
    {
        [Description("SharedUI credentials are not properly setup in the server")] CredentialsNotProperlyConfigured,
        [Description("Invalid credentials")] InvalidCredentials,
    }
}

public class LiteDBCommonMessages : ILocalizedResource
{
    public enum Errors
    {
        [Description("Could not delete entity with id: {0}")] CannotDeleteEntity,
        [Description("Could not update entity with id: {0}")] CannotUpdateEntity,
    }
}