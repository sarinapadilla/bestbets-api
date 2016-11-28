using System;

/// <summary>
/// Metadata for a CancerGov published content item.
/// </summary>
public class PublishedContentInfo
{
    /// <summary>
    /// Full path to the file, relative to server root.
    /// </summary>
    public string FullWebPath {get; set;}

    /// <summary>
    /// The name of the file with no path information
    /// </summary>
    public string FileName {get; set;}

    /// <summary>
    /// Date the file was created.
    /// </summary>
    public DateTime CreationTime {get; set;}

    /// <summary>
    /// Date the file was last updated.
    /// </summary>
    public DateTime LastWriteTime {get; set;}
}