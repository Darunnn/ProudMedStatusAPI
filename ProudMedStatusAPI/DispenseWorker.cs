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


        private int _isRunning = 0;
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
            if (Interlocked.CompareExchange(ref _isRunning, 1, 0) != 0)
            {
                _log.Info("RunOnce skipped — รอบก่อนยังทำงานอยู่");
                return;
            }
            try
            {
                int successToday = _repo.CountSuccessToday(); // ประกาศครั้งแรก
                var pending = _repo.GetPendingItems();
                if (pending.Count == 0)
                {
                    NotifyUI(0, successToday);
                    return;
                }

                _log.Info($"พบรายการรอส่ง {pending.Count} รายการ");
                var distinctIds = pending
                                 .Select(p => p.PrescriptionItemID)
                                 .Distinct(StringComparer.OrdinalIgnoreCase)
                                 .ToList();
                _log.Info($"PrescriptionItemID unique ที่จะส่ง API: {distinctIds.Count} รายการ (จาก {pending.Count} รายการ)");
                string ids = string.Join(",", distinctIds);
                var apiResult = _apiClient.UpdateDispenseAsync(ids).GetAwaiter().GetResult();
                int successCount = ProcessResult(apiResult, pending);

                ClearOldData();
                _log.ClearOldLogs();

                successToday = _repo.CountSuccessToday(); // ✅ แค่ assign ใหม่ ไม่ต้อง int
                NotifyUI(pending.Count - successCount, successToday);
            }
            catch (Exception ex)
            {
                _log.Error($"RunOnce Exception: {ex}");
            }
            finally
            {
                Interlocked.Exchange(ref _isRunning, 0);
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
                var lookup = pending
            .GroupBy(p => p.PrescriptionItemID, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

                foreach (var msg in result.Message)
                {
                    if (msg.status && lookup.TryGetValue(msg.prescriptionItemID, out var items))
                    {
                        
                        foreach (var item in items)
                        {
                            _repo.MarkAsSuccess(item.ID);
                            successCount++;
                            _log.Info($"ส่งสำเร็จ: ID={item.ID} PrescriptionItemID={msg.prescriptionItemID}");
                        }
                    }
                    else
                    {
                        _log.Error(
                            $"API status=false: PrescriptionItemID={msg.prescriptionItemID}" +
                            $" message={msg.message}");
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

        

        private void NotifyUI(int pending, int success) =>
            OnStatsUpdated?.Invoke(pending, success);
    }
}