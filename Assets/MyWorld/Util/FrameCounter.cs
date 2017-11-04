using UnityEngine;
using System.Collections;

public class FrameCounter : MonoBehaviour
{
    /// <summary>
    /// fps更新间隔
    /// </summary>
    [Label("FPS更新间隔")]
    public float updateInterval = 0.5f;
    /// <summary>
    /// 计时自动关闭 0为不开启
    /// </summary>
    [Label("统计平均帧数时长")]
    public int meanFrameTime;
    private float _meanFrameCount;
    /// <summary>
    /// fps累积
    /// </summary>
    private float _accum;
    /// <summary>
    /// 帧数累积
    /// </summary>
    private int _frames;
    /// <summary>
    /// 当前更新间隔的计时
    /// </summary>
    private float _timeLeft;

    private float _fps;
    //private float _timeFrame;

    // 计算评价帧数
    private float _totalFps;
    private float _fpsCount;

    private void Awake()
    {
        _timeLeft = updateInterval;
        StartCoroutine(UpdateCount());
    }

    private bool _start = true;
    private IEnumerator UpdateCount()
    {
        while (_start)
        {
            CountFps();
            yield return null;
        }
    }

    private void CountFps()
    {
        _timeLeft -= Time.deltaTime;
        _accum += (Time.timeScale / Time.deltaTime);
        ++_frames;

        _meanFrameCount += Time.deltaTime;

        // 到达更新间隔
        if (_timeLeft <= 0.0f) 
        {
            // 计算fps
            _fps = _accum / _frames;
            // 计算处理一帧需要的时间
            //_timeFrame = 1000.0f / _fps;

            // 计算平均帧数
            _totalFps += _fps;
            _fpsCount++;

            // 重置数据，准备下一次的计算
            _timeLeft = updateInterval;
            _accum = 0.0f;
            _frames = 0;
        }

        if (meanFrameTime != 0 && _meanFrameCount >= meanFrameTime)
        {
            print(">>>>>>>>>>>>>>> mean fps: " + _totalFps / _fpsCount);
            _start = false;
        }
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(20, 20, 100, 20), "FPS:" + _fps);
    }
}