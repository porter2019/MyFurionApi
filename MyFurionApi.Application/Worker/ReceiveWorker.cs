using Furion.EventBus;
using Microsoft.Extensions.Hosting;

namespace MyFurionApi.Application;

/// <summary>
/// 后台持续运行的任务
/// </summary>
public class ReceiveWorker : BackgroundService
{
    private readonly ILogger<ReceiveWorker> _logger;
    private readonly IEventPublisher _publisher;

    private readonly int _delay;

    public ReceiveWorker(ILogger<ReceiveWorker> logger, IConfiguration config, IEventPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
        _delay = config.Get<int>("AppSettings:Interval");
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //在这里执行后台任务
        //1、每隔多久执行一次
        if (_delay < 1) return;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _publisher.PublishAsync("Pub:Event1", cancellationToken: stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("操作被取消.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发生错误.");
            }
            //TimeSpan.FromHours(_delay)
            //每隔几分钟就publish一个事件出去
            await Task.Delay(TimeSpan.FromMinutes(_delay), stoppingToken);
        }

        //2、持续运行，比如接收MQTT或串口数据
        //try
        //{
        //    var factory = new MqttFactory();
        //    _mqttClient = factory.CreateMqttClient();
        //    _mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
        //    await _mqttClient.ConnectAsync(_mqttClientOptions);

        //    if (_topic.IsNotNull())
        //    {
        //        await _mqttClient.SubscribeAsync(_topic);
        //        _logger.LogInformation($"MQTT > 订阅主题： {_topic}");
        //    }

        //    //阻塞线程，保持保证持续接受数据
        //    await Task.Delay(Timeout.Infinite, stoppingToken);
        //}
        //catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        //{
        //    // 服务被请求停止，这是正常的，可以忽略此异常
        //    //_logger.LogDebug($"{_logPrefix} 服务正在停止...");
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, $"MQ 订阅失败");
        //}
        //finally
        //{
        //    if (_mqttClient != null && _mqttClient.IsConnected)
        //    {
        //        await _mqttClient.DisconnectAsync();
        //    }
        //}
    }

    ///// <summary>
    ///// 处理消息
    ///// </summary>
    ///// <param name="arg"></param>
    ///// <returns></returns>
    //private async Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    //{
    //    var payload = new EventSiloCurPayload();
    //    payload.Toppic = arg.ApplicationMessage.Topic;
    //    payload.Now = DateTime.Now;

    //    if (payload.Type.Equals("DTU"))
    //    {
    //        byte[] data = [.. arg.ApplicationMessage.PayloadSegment];
    //        if (IsCrcValid(data))
    //        {
    //            var val = ParseModbusRTU(data);
    //            payload.DTUAddress = val.Item1;
    //            payload.DTUValue = val.Item2;
    //            await _publisher.PublishAsync("Silo:CurValue", payload);
    //        }
    //    }
    //    else if (payload.Type.Equals("MCGS"))
    //    {
    //        var json = System.Text.Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);
    //        payload.MCGSJson = JsonHelper.Deserialize<EventSiloCurPayload.MCGSJsonObj>(json);
    //        await _publisher.PublishAsync("Silo:CurValue", payload);
    //    }
    //}

    public override void Dispose()
    {
        base.Dispose();
    }

    #region 私有方法

    ///// <summary>
    ///// 解析ModbusRTU数据
    ///// </summary>
    ///// <param name="receivedData"></param>
    ///// <returns>int是仪表地址，double是重量</returns>
    //static Tuple<int, double> ParseModbusRTU(byte[] receivedData)
    //{
    //    // 输入数据
    //    //byte[] receivedData = new byte[] { 1, 2, 4, 0, 242, 0, 1, 154, 0 };

    //    var address = (int)receivedData[0];

    //    // 解析 Weight
    //    int weight = (short)((receivedData[3] << 8) | receivedData[4]); // 16 位有符号整数

    //    // 解析 Point
    //    int point = (receivedData[5] << 8) | receivedData[6];
    //    point = Math.Min(point, 3); //最多支持3位小数

    //    // 计算实际重量
    //    var val = weight / Math.Pow(10, point);

    //    return Tuple.Create(address, val);
    //}

    ///// <summary>
    ///// CRC校验
    ///// </summary>
    ///// <param name="receivedData"></param>
    ///// <returns></returns>
    //static bool IsCrcValid(byte[] receivedData)
    //{
    //    // 检查输入长度是否符合最小 Modbus 数据包要求
    //    if (receivedData == null || receivedData.Length < 3)
    //    {
    //        //throw new ArgumentException("接收到的数据长度不足以校验 CRC。");
    //        return false;
    //    }

    //    // CRC 计算函数
    //    static ushort CalculateCRC(byte[] data, int length)
    //    {
    //        ushort crc = 0xFFFF; // 初始值

    //        for (int i = 0; i < length; i++)
    //        {
    //            crc ^= data[i]; // 与当前字节异或

    //            for (int j = 0; j < 8; j++) // 每个字节的每一位处理
    //            {
    //                if ((crc & 0x0001) != 0) // 检查最低位
    //                {
    //                    crc >>= 1;
    //                    crc ^= 0xA001; // 与多项式异或
    //                }
    //                else
    //                {
    //                    crc >>= 1;
    //                }
    //            }
    //        }

    //        return crc; // 返回最终的 CRC 值
    //    }

    //    // 去除 CRC 部分的数据
    //    byte[] dataToCheck = new byte[receivedData.Length - 2];
    //    Array.Copy(receivedData, 0, dataToCheck, 0, dataToCheck.Length);

    //    // 调用 CRC 计算函数
    //    ushort crcCalculated = CalculateCRC(dataToCheck, dataToCheck.Length);

    //    // 提取接收到的 CRC，低字节在前，高字节在后
    //    ushort crcReceived = (ushort)((receivedData[receivedData.Length - 2]) | (receivedData[receivedData.Length - 1] << 8));

    //    // 比较 CRC
    //    return crcCalculated == crcReceived;
    //}

    #endregion
}
