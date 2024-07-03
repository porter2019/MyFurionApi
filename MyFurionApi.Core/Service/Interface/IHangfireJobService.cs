namespace MyFurionApi.Core;

/// <summary>
/// Hanfire计划任务服务
/// </summary>
public interface IHangfireJobService
{
    Task<string> AddBackgroudJobRunAtAsync(HanfireJobModel model);
    Task RemoveBackgroundJobAsync(string jobId);
}
