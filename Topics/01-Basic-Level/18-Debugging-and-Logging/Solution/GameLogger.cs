using System.Diagnostics;
using System.IO;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameLogger : MonoBehaviour
{
    private static GameLogger _instance;
    private string _logDirectory;
    private static readonly object _fileLock = new object();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _logDirectory = Application.persistentDataPath;
        Application.logMessageReceived += HandleLog;
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogGeneration(string message)
    {
        UnityEngine.Debug.Log($"[GEN] {message}");
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogCombat(string message)
    {
        UnityEngine.Debug.Log($"[COMBAT] {message}");
    }

    public static void LogCriticalError(string message)
    {
        UnityEngine.Debug.LogError($"[CRITICAL] {message}");

        if (_instance != null)
        {
            _instance.WriteCriticalErrorDirectly(message);
        }
    }

    private void WriteCriticalErrorDirectly(string message)
    {
        string path = Path.Combine(_logDirectory, "critical_errors.txt");
        string entry = $"[{System.DateTime.Now:HH:mm:ss}] [Critical] {message}\n";

        lock (_fileLock)
        {
            File.AppendAllText(path, entry);
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string entry = $"[{System.DateTime.Now:HH:mm:ss}] [{type}] {logString}\n";

        if (type == LogType.Error || type == LogType.Exception)
        {
            entry += $"STACK: {stackTrace}\n";
        }

        lock (_fileLock)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                string errorPath = Path.Combine(_logDirectory, "critical_errors.txt");
                File.AppendAllText(errorPath, entry);
            }

            if (UnityEngine.Debug.isDebugBuild || Application.isEditor)
            {
                string fullLogPath = Path.Combine(_logDirectory, "full_debug.log");
                File.AppendAllText(fullLogPath, entry);
            }
        }
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
}
