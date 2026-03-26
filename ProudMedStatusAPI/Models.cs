using System;
using System.Collections.Generic;

namespace ProudMedStatusAPI
{
    // ---- ตาราง TPN_T_ReceiveDispense ----
    public class DispenseItem
    {
        public long ID { get; set; }
        public string PrescriptionItemID { get; set; } = "";
        public string PrescriptionNo { get; set; } = "";
        public string PrescriptionDate { get; set; } = "";
        public string HN { get; set; } = "";
        public string DrugCode { get; set; } = "";
    }

    // ---- API Response (201) ----
    public class ApiResponseMessage
    {
        public string PrescriptionItemID { get; set; } = "";
        public bool Status { get; set; }
        public string? Message { get; set; }
    }

    public class ApiResponse
    {
        public string? Status { get; set; }
        public List<ApiResponseMessage>? Message { get; set; }
    }
}