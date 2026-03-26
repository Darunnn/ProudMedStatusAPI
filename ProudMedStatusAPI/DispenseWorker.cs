using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ProudMedStatusAPI
{
    /// <summary>
    /// Orchestrator: ประสาน DispenseRepository กับ DispenseApiClient
    /// ไม่มี SQL หรือ HttpClient อยู่ในไฟล์นี้
    /// </summary>
    public class DispenseWorker
    {
        private readonly Config _config;
        private readonly LogManager _log;
        private readonly DispenseRepository _repo;
        private readonly DispenseApiClient _apiClient;
        private System.Threading.Timer? _timer;

        // callback อัปเดต UI (pending, successToday)
        public Action<int, int>? OnStatsUpdated;

        private int _successToday;
        private DateTime _successDate = DateTime.Today;

        public DispenseWorker(Config config, LogManager log)
        {
            _config = config;
            _log = log;
            _repo = new DispenseRepository(new Database());
            _apiClient = new DispenseApiClient(config.DomainAPI, log);
        }

        // ---- Start / Stop ----

        public void Start()
        {
            int intervalMs = _config.TimeProcess * 1000;
            _timer = new System.Threading.Timer(
                callback: _ => RunOnce(),
                state: null,
                dueTime: 0,
                period: intervalMs
            );
            _log.Info($"Worker เริ่มทำงาน interval={_config.TimeProcess}s");
        }

        public void Stop()
        {
            _timer?.Dispose();
            _apiClient.Dispose();
            _log.Info("Worker หยุดทำงาน");
        }

        // ---- Main loop ----

        private void RunOnce()
        {
            try
            {
                ResetSuccessCounterIfNewDay();

                // 1. ดึงรายการรอส่งจาก DB (ReceiveStatus = '0')
                var pending = _repo.GetPendingItems();

                if (pending.Count == 0)
                {
                    NotifyUI(0, _successToday);
                    return;
                }

                _log.Info($"พบรายการรอส่ง {pending.Count} รายการ");

                // 2. รวม PrescriptionItemID คั่น ,
                string ids = string.Join(",", pending.Select(p => p.PrescriptionItemID));

                // 3. เรียก API POST /api/robot/updateDispense
                var apiResult = _apiClient.UpdateDispenseAsync(ids).GetAwaiter().GetResult();

                // 4. Update DB ตามผล API
                int successCount = ProcessResult(apiResult, pending);
                _successToday += successCount;

                // 5. ลบข้อมูลเก่า + clear log เก่า
                ClearOldData();
                _log.ClearOldLogs();

                // remaining pending = total - success
                NotifyUI(pending.Count - successCount, _successToday);
            }
            catch (Exception ex)
            {
                _log.Error($"RunOnce Exception: {ex}");
            }
        }

        // ---- Process API result → update DB ----

        private int ProcessResult(ApiResponse? result, List<DispenseItem> pending)
        {
            if (result == null)
            {
                _log.Error("ไม่ได้รับผลลัพธ์จาก API");
                return 0;
            }

            int successCount = 0;

            if (result.Message != null && result.Message.Count > 0)
            {
                // API ตอบ 201 พร้อม detail แต่ละรายการใน message[]
                var lookup = pending.ToDictionary(
                    p => p.PrescriptionItemID,
                    StringComparer.OrdinalIgnoreCase);

                foreach (var msg in result.Message)
                {
                    if (msg.Status && lookup.TryGetValue(msg.PrescriptionItemID, out var item))
                    {
                        _repo.MarkAsSuccess(item.ID);
                        successCount++;
                        _log.Info($"ส่งสำเร็จ: ID={item.ID} PrescriptionItemID={msg.PrescriptionItemID}");
                    }
                    else
                    {
                        _log.Error(
                            $"API status=false: PrescriptionItemID={msg.PrescriptionItemID}" +
                            $" message={msg.Message}");
                    }
                }
            }
            else if (string.Equals(result.Status, "OK", StringComparison.OrdinalIgnoreCase))
            {
                // API ตอบ OK แบบ bulk ไม่มี detail → mark ทุกรายการ
                foreach (var item in pending)
                {
                    _repo.MarkAsSuccess(item.ID);
                    successCount++;
                }
                _log.Info($"ส่งสำเร็จ (bulk): {successCount} รายการ");
            }
            else
            {
                _log.Error($"API ตอบกลับไม่ถูกต้อง status={result.Status}");
            }

            return successCount;
        }

        // ---- Helpers ----

        private void ClearOldData()
        {
            try
            {
                int rows = _repo.DeleteOldRecords(_config.DayClearData);
                if (rows > 0)
                    _log.Info($"ลบข้อมูลเก่า {rows} รายการ (เกิน {_config.DayClearData} วัน)");
            }
            catch (Exception ex)
            {
                _log.Error($"ClearOldData: {ex.Message}");
            }
        }

        private void ResetSuccessCounterIfNewDay()
        {
            if (DateTime.Today != _successDate)
            {
                _successToday = 0;
                _successDate = DateTime.Today;
            }
        }

        private void NotifyUI(int pending, int success) =>
            OnStatsUpdated?.Invoke(pending, success);
    }
}