using Furion.DependencyInjection;
using Hangfire.HttpJob.Client;
using Microsoft.Extensions.Logging;

namespace MyFurionApi.Core;

/// <summary>
/// Hanfire计划任务服务
/// </summary>
public class HangfireJobService : IHangfireJobService, ISingleton
{
    private readonly ILogger<HangfireJobService> _logger;
    private readonly IConfiguration _config;

    private readonly string ServerUrl;
    private readonly string BasicUserName;
    private readonly string BasicPassword;
    private readonly string NoticeMail;

    public HangfireJobService(ILogger<HangfireJobService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;

        ServerUrl = _config["HangfireTask:ServerUrl"];
        BasicUserName = _config["HangfireTask:BasicUserName"];
        BasicPassword = _config["HangfireTask:BasicPassword"];
        NoticeMail = _config["HangfireTask:NoticeMail"];
    }

    /// <summary>
    /// 添加一个一次性任务，指定执行时间
    /// </summary>
    /// <param name="model">任务名称</param>
    /// <returns>返回jobId，删除时需要</returns>
    /// <example>await _hangfireJobService.AddBackgroudJobRunAtAsync(new HanfireJobModel(jobName: "inquiry_expire_" + inquiryEntity.Id,url: $"{_config[AppSettingsConst.HanfireCallBackDomainUrl]}/api/inquiryContent/taskjob/expire?id={inquiryEntity.Id}",method: "Get",runAt: inquiryEntity.ExpireTime))</example>
    public async Task<string> AddBackgroudJobRunAtAsync(HanfireJobModel model)
    {
        var result = await AddBackgroundJobAsync(new BackgroundJob()
        {
            JobName = model.JobName,
            Url = model.Url,
            Method = model.Method,
            RunAt = model.RunAt,
        });
        if (result.IsSuccess)
        {
            return result.JobId;
        }
        else
        {
            _logger.LogError($"添加计划任务失败，消息：{result.ErrMessage}");
            return null;
        }
    }

    /// <summary>
    /// 删除一个一次性任务
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public async Task RemoveBackgroundJobAsync(string jobId)
    {
        if (jobId.IsNull()) return;

        await DeleteBackgroundJobAsync(jobId);
    }

    #region 简单封装

    /// <summary>
    /// 添加一个一次性任务
    /// </summary>
    /// <param name="job"></param>
    /// <returns>如果返回为空，则执行成功，否则返回异常消息</returns>
    private AddBackgroundHangfirJobResult AddBackgroundJob(BackgroundJob job)
    {
        if (job.SendFail || job.SendSuccess) job.Mail = NoticeMail.SplitWithSemicolon().ToList();

        //如果指定的时间小于现在，则置为一分钟后执行
        if (job.RunAt <= DateTime.Now) job.DelayFromMinutes = 1;

        return HangfireJobClient.AddBackgroundJob(ServerUrl, job, new HangfireServerPostOption { BasicUserName = BasicUserName, BasicPassword = BasicPassword });
    }

    /// <summary>
    /// 添加一个一次性任务
    /// </summary>
    /// <param name="job"></param>
    /// <returns>如果返回为空，则执行成功，否则返回异常消息</returns>
    private Task<AddBackgroundHangfirJobResult> AddBackgroundJobAsync(BackgroundJob job)
    {
        if (job.SendFail || job.SendSuccess) job.Mail = NoticeMail.SplitWithSemicolon().ToList();

        //如果指定的时间小于现在，则置为一分钟后执行
        if (job.RunAt <= DateTime.Now) job.DelayFromMinutes = 1;

        return HangfireJobClient.AddBackgroundJobAsync(ServerUrl, job, new HangfireServerPostOption { BasicUserName = BasicUserName, BasicPassword = BasicPassword });
    }

    /// <summary>
    /// 根据jobid删除一个一次性任务
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    private HangfirJobResult DeleteBackgroundJob(string jobId)
    {
        return HangfireJobClient.RemoveBackgroundJob(ServerUrl, jobId, new HangfireServerPostOption() { BasicUserName = BasicUserName, BasicPassword = BasicPassword });
    }

    /// <summary>
    /// 根据jobid删除一个一次性任务
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    private Task<HangfirJobResult> DeleteBackgroundJobAsync(string jobId)
    {
        return HangfireJobClient.RemoveBackgroundJobAsync(ServerUrl, jobId, new HangfireServerPostOption() { BasicUserName = BasicUserName, BasicPassword = BasicPassword });
    }

    /// <summary>
    ///  添加一个指定Corn的任务
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    private HangfirJobResult AddRecurringJob(RecurringJob job)
    {
        if (job.SendFail || job.SendSuccess) job.Mail = NoticeMail.SplitWithSemicolon().ToList();

        return HangfireJobClient.AddRecurringJob(ServerUrl, job, new HangfireServerPostOption { BasicUserName = BasicUserName, BasicPassword = BasicPassword });
    }

    /// <summary>
    ///  添加一个指定Corn的任务
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    private Task<HangfirJobResult> AddRecurringJobAsync(RecurringJob job)
    {
        if (job.SendFail || job.SendSuccess) job.Mail = NoticeMail.SplitWithSemicolon().ToList();

        return HangfireJobClient.AddRecurringJobAsync(ServerUrl, job, new HangfireServerPostOption { BasicUserName = BasicUserName, BasicPassword = BasicPassword });
    }

    /// <summary>
    /// 根据jobName删除周期任务
    /// </summary>
    /// <param name="jobName"></param>
    /// <returns></returns>
    private HangfirJobResult DeleteRecurringJob(string jobName)
    {
        return HangfireJobClient.RemoveRecurringJob(ServerUrl, jobName, new HangfireServerPostOption() { BasicUserName = BasicUserName, BasicPassword = BasicPassword });
    }

    /// <summary>
    /// 根据jobName删除周期任务
    /// </summary>
    /// <param name="jobName"></param>
    /// <returns></returns>
    private Task<HangfirJobResult> DeleteRecurringJobAsync(string jobName)
    {
        return HangfireJobClient.RemoveRecurringJobAsync(ServerUrl, jobName, new HangfireServerPostOption() { BasicUserName = BasicUserName, BasicPassword = BasicPassword });
    }

    #endregion
}
