using System.ComponentModel.DataAnnotations;
using System.Data;

namespace field_recording_api.Models.Payment
{
    public class tbt_dep_log_statement
    {
        [Key]
        public string st_hd_no                  { get; set; }
        public string contract_no               { get; set; }
        public string contract_no_format        { get; set; }
        public string recipient_name            { get; set; }
        public string recipient_address1        { get; set; }
        public string recipient_address2        { get; set; }
        public string recipient_address3        { get; set; }
        public string issue_date                { get; set; }
        public string product_type              { get; set; }
        public string sub_product_type          { get; set; }
        public string contract_balance          { get; set; }
        public string installment_amount        { get; set; }
        public string total_term                { get; set; }
        public string due_date                  { get; set; }
        public string next_due_date             { get; set; }
        public string end_due_date              { get; set; }
        public string license_no                { get; set; }
        public string chassis_no                { get; set; }
        public string machine_no                { get; set; }
        public string tax_exp                   { get; set; }
        public string insurance_policy_exp      { get; set; }
        public string ref_no                    { get; set; }
        public string payment_barcode           { get; set; }
        public string no_invoice                { get; set; }
        public string no_receipt                { get; set; }
        public string no_tax_invoice            { get; set; }
        public string statement_type            { get; set; }
        public string remark                    { get; set; }
        public string payment_qr_code           { get; set; }
        public string ads                       { get; set; }
        public string letter_type               { get; set; }
        public string send_letter_type          { get; set; }
        public string print_date                { get; set; }
        public string account_date              { get; set; }
        public string statement_channel { get; set; }
    }
}
