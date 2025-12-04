namespace BudgetApp.Domain.Models;

/// <summary>
/// Represents the processing status of a transaction.
/// </summary>
public enum TransactionStatus
{
    /// <summary>
    /// Transaction is new and awaiting categorization.
    /// </summary>
    New,
    
    /// <summary>
    /// Transaction has been processed and categorized.
    /// </summary>
    Processed,
    
    /// <summary>
    /// Transaction requires manual review.
    /// </summary>
    PendingReview,
    
    /// <summary>
    /// Transaction has been manually verified.
    /// </summary>
    Verified
}


