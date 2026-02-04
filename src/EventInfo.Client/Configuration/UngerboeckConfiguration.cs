namespace EventInfo.Client.Configuration;

/// <summary>
/// Configuration settings for connecting to the Ungerboeck API
/// </summary>
public class UngerboeckConfiguration
{
    /// <summary>
    /// The base URL of your Ungerboeck instance (e.g., https://yoursite.ungerboeck.com)
    /// </summary>
    public string UngerboeckUri { get; set; } = string.Empty;

    /// <summary>
    /// The API User ID from Ungerboeck Main Menu -> Api Users
    /// </summary>
    public string ApiUserId { get; set; } = string.Empty;

    /// <summary>
    /// The Secret value (GUID) from the API User details window
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// The Key value (GUID) from the API User details window
    /// </summary>
    public string Key { get; set; } = string.Empty;
}
