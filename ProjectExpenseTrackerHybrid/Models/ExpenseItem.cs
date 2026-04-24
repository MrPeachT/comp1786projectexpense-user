using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectExpenseTrackerHybrid.Models
{
    public class ExpenseItem
    {
        public string ExpenseId { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public string DateOfExpense { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string ExpenseType { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string Claimant { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public long LastModified { get; set; }
        public int Synced { get; set; }
    }
}
