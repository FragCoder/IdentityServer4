namespace IdentityServer4.Models
{
    /// <summary>
    /// OpenID Connect subject types.
    /// </summary>
    public enum SubjectTypes
    {
        /// <summary>
        /// global - use the native subject id
        /// </summary>
        Global = 0,

        /// <summary>
        /// ppid - scope the subject id to the client
        /// </summary>
        Ppid = 1
    }
}