using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ProudMedStatusAPI
{
    /// <summary>
    /// รับผิดชอบ SQL ทั้งหมดกับตาราง TPN_T_ReceiveDispense
    /// </summary>
    public class DispenseRepository
    {
        private readonly Database _db;

        public DispenseRepository(Database db)
        {
            _db = db;
        }

        /// <summary>
        /// ดึงรายการที่ ReceiveStatus = '0' (รอส่ง)
        /// </summary>
        public List<DispenseItem> GetPendingItems()
        {
            const string sql = @"
                SELECT ID, PrescriptionItemID, PrescriptionNo,
                       PrescriptionDate, HN, DrugCode
                FROM   TPN_T_ReceiveDispense
                WHERE  ReceiveStatus = '0'
                ORDER  BY InsertDateTime";

            var dt = _db.ExecuteQuery(sql);
            var list = new List<DispenseItem>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DispenseItem
                {
                    ID = Convert.ToInt64(row["ID"]),
                    PrescriptionItemID = row["PrescriptionItemID"]?.ToString() ?? "",
                    PrescriptionNo = row["PrescriptionNo"]?.ToString() ?? "",
                    PrescriptionDate = row["PrescriptionDate"]?.ToString() ?? "",
                    HN = row["HN"]?.ToString() ?? "",
                    DrugCode = row["DrugCode"]?.ToString() ?? "",
                });
            }

            return list;
        }

        /// <summary>
        /// อัปเดต ReceiveStatus = '1' และ ReceiveDateTime = เวลาปัจจุบัน
        /// </summary>
        public void MarkAsSuccess(long id)
        {
            const string sql = @"
                UPDATE TPN_T_ReceiveDispense
                SET    ReceiveStatus   = '1',
                       ReceiveDateTime = @now
                WHERE  ID = @id";

            _db.ExecuteNonQuery(sql, new[]
            {
                new SqlParameter("@now", DateTime.Now),
                new SqlParameter("@id",  id),
            });
        }

        /// <summary>
        /// ลบข้อมูลเก่าที่ InsertDateTime เกินกว่า dayClear วัน
        /// </summary>
        public int DeleteOldRecords(int dayClear)
        {
            const string sql = @"
                DELETE FROM TPN_T_ReceiveDispense
                WHERE  DATEDIFF(DAY, InsertDateTime, GETDATE()) > @days";

            return _db.ExecuteNonQuery(sql, new[]
            {
                new SqlParameter("@days", dayClear),
            });
        }
    }
}