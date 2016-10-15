namespace IdentityServer4.Validation.Models
{
    public enum IntrospectionRequestValidationFailureReason
    {
        None,
        MissingToken,
        InvalidToken,
        InvalidScope
    }
}