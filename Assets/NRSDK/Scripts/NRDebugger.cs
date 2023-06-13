/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System.Diagnostics;

    /// <summary> Values that represent log levels. </summary>
    public enum LogLevel
    {
        /// <summary> An enum constant representing all option. </summary>
        All = 0,
        /// <summary> An enum constant representing the debug option. </summary>
        Debug,
        /// <summary> An enum constant representing the Information option. </summary>
        Info,
        /// <summary> An enum constant representing the warning option. </summary>
        Warning,
        /// <summary> An enum constant representing the error option. </summary>
        Error,
        /// <summary> An enum constant representing the fatal option. </summary>
        Fatal,
        /// <summary> An enum constant representing the off option. </summary>
        Off
    }

    /// <summary> Interface for log helper. </summary>
    public interface ILogHelper
    {
        /// <summary> Logs. </summary>
        /// <param name="level">   The level.</param>
        /// <param name="message"> The message.</param>
        void Log(LogLevel level, object message);
    }

    /// <summary> A default log helper. </summary>
    public class DefaultLogHelper : ILogHelper
    {
        /// <summary> Logs. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        /// <param name="level">   The level.</param>
        /// <param name="message"> The message.</param>
        public void Log(LogLevel level, object message)
        {
            string log_info = string.Format("[{0}]{1}", level.ToString(), message.ToString());
            switch (level)
            {
                case LogLevel.Debug:
                    UnityEngine.Debug.Log(log_info);
                    break;
                case LogLevel.Info:
                    UnityEngine.Debug.Log(log_info);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(log_info);
                    break;
                case LogLevel.Error:
                    UnityEngine.Debug.LogError(log_info);
                    break;
                default:
                    throw new System.Exception(log_info);
            }
        }
    }

    /// <summary> A tool for log. </summary>
    public class NRDebugger
    {
        private static ILogHelper m_LogHelper = new DefaultLogHelper();

        /// <summary> The log level. </summary>
        private static LogLevel m_LogLevel = LogLevel.Warning;
        /// <summary> Gets or sets the log level. </summary>
        /// <value> The log level. </value>
        public static LogLevel logLevel
        {
            get { return m_LogLevel; }
            set { m_LogLevel = value; }
        }

        /// <summary> Set the log helper. </summary>
        /// <param name="logHelper"> The log helper.</param>
        public static void SetLogHelper(ILogHelper logHelper)
        {
            m_LogHelper = logHelper;
        }

        /// <summary> (Only available in DEBUG builds) debugs. </summary>
        /// <param name="message"> The message.</param>
        [Conditional("DEBUG")]
        public static void Debug(object message)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Debug)
            {
                m_LogHelper.Log(LogLevel.Debug, message);
            }
        }

        /// <summary> (Only available in DEBUG builds) debugs. </summary>
        /// <param name="message"> The message.</param>
        [Conditional("DEBUG")]
        public static void Debug(string message)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Debug)
            {
                m_LogHelper.Log(LogLevel.Debug, message);
            }
        }

        /// <summary> (Only available in DEBUG builds) debugs. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, object arg0)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Debug)
            {
                m_LogHelper.Log(LogLevel.Debug, StringUtility.Format(format, arg0));
            }
        }

        /// <summary> (Only available in DEBUG builds) debugs. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <param name="arg1">   The first argument.</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, object arg0, object arg1)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Debug)
            {
                m_LogHelper.Log(LogLevel.Debug, StringUtility.Format(format, arg0, arg1));
            }
        }

        /// <summary> (Only available in DEBUG builds) debugs. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <param name="arg1">   The first argument.</param>
        /// <param name="arg2">   The second argument.</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, object arg0, object arg1, object arg2)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Debug)
            {
                m_LogHelper.Log(LogLevel.Debug, StringUtility.Format(format, arg0, arg1, arg2));
            }
        }

        /// <summary> (Only available in DEBUG builds) debugs. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="args">   A variable-length parameters list containing arguments.</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, params object[] args)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Debug)
            {
                m_LogHelper.Log(LogLevel.Debug, StringUtility.Format(format, args));
            }
        }

        /// <summary> Infoes. </summary>
        /// <param name="message"> The message.</param>
        public static void Info(object message)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Info)
            {
                m_LogHelper.Log(LogLevel.Info, message);
            }
        }

        /// <summary> Infoes. </summary>
        /// <param name="message"> The message.</param>
        public static void Info(string message)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Info)
            {
                m_LogHelper.Log(LogLevel.Info, message);
            }
        }

        /// <summary> Infoes. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        public static void Info(string format, object arg0)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Info)
            {
                m_LogHelper.Log(LogLevel.Info, StringUtility.Format(format, arg0));
            }
        }

        /// <summary> Infoes. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <param name="arg1">   The first argument.</param>
        public static void Info(string format, object arg0, object arg1)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Info)
            {
                m_LogHelper.Log(LogLevel.Info, StringUtility.Format(format, arg0, arg1));
            }
        }

        /// <summary> Infoes. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <param name="arg1">   The first argument.</param>
        /// <param name="arg2">   The second argument.</param>
        public static void Info(string format, object arg0, object arg1, object arg2)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Info)
            {
                m_LogHelper.Log(LogLevel.Info, StringUtility.Format(format, arg0, arg1, arg2));
            }
        }

        /// <summary> Infoes. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="args">   A variable-length parameters list containing arguments.</param>
        public static void Info(string format, params object[] args)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Info)
            {
                m_LogHelper.Log(LogLevel.Info, StringUtility.Format(format, args));
            }
        }

        /// <summary> Warnings. </summary>
        /// <param name="message"> The message.</param>
        public static void Warning(object message)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Warning)
            {
                m_LogHelper.Log(LogLevel.Warning, message);
            }
        }

        /// <summary> Warnings. </summary>
        /// <param name="message"> The message.</param>
        public static void Warning(string message)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Warning)
            {
                m_LogHelper.Log(LogLevel.Warning, message);
            }
        }

        /// <summary> Warnings. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        public static void Warning(string format, object arg0)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Warning)
            {
                m_LogHelper.Log(LogLevel.Warning, StringUtility.Format(format, arg0));
            }
        }

        /// <summary> Warnings. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <param name="arg1">   The first argument.</param>
        public static void Warning(string format, object arg0, object arg1)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Warning)
            {
                m_LogHelper.Log(LogLevel.Warning, StringUtility.Format(format, arg0, arg1));
            }
        }

        /// <summary> Warnings. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <param name="arg1">   The first argument.</param>
        /// <param name="arg2">   The second argument.</param>
        public static void Warning(string format, object arg0, object arg1, object arg2)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Warning)
            {
                m_LogHelper.Log(LogLevel.Warning, StringUtility.Format(format, arg0, arg1, arg2));
            }
        }

        /// <summary> Warnings. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="args">   A variable-length parameters list containing arguments.</param>
        public static void Warning(string format, params object[] args)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Warning)
            {
                m_LogHelper.Log(LogLevel.Warning, StringUtility.Format(format, args));
            }
        }

        /// <summary> Errors. </summary>
        /// <param name="message"> The message.</param>
        public static void Error(object message)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Error)
            {
                m_LogHelper.Log(LogLevel.Error, message);
            }
        }

        /// <summary> Errors. </summary>
        /// <param name="message"> The message.</param>
        public static void Error(string message)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Error)
            {
                m_LogHelper.Log(LogLevel.Error, message);
            }
        }

        /// <summary> Errors. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        public static void Error(string format, object arg0)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Error)
            {
                m_LogHelper.Log(LogLevel.Error, StringUtility.Format(format, arg0));
            }
        }

        /// <summary> Errors. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <param name="arg1">   The first argument.</param>
        public static void Error(string format, object arg0, object arg1)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Error)
            {
                m_LogHelper.Log(LogLevel.Error, StringUtility.Format(format, arg0, arg1));
            }
        }

        /// <summary> Errors. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <param name="arg1">   The first argument.</param>
        /// <param name="arg2">   The second argument.</param>
        public static void Error(string format, object arg0, object arg1, object arg2)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Error)
            {
                m_LogHelper.Log(LogLevel.Error, StringUtility.Format(format, arg0, arg1, arg2));
            }
        }

        /// <summary> Errors. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="args">   A variable-length parameters list containing arguments.</param>
        public static void Error(string format, params object[] args)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Error)
            {
                m_LogHelper.Log(LogLevel.Error, StringUtility.Format(format, args));
            }
        }

        /// <summary> Fatals. </summary>
        /// <param name="message"> The message.</param>
        public static void Fatal(object message)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Fatal)
            {
                m_LogHelper.Log(LogLevel.Fatal, message);
            }
        }

        /// <summary> Fatals. </summary>
        /// <param name="message"> The message.</param>
        public static void Fatal(string message)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Fatal)
            {
                m_LogHelper.Log(LogLevel.Fatal, message);
            }
        }

        /// <summary> Fatals. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        public static void Fatal(string format, object arg0)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Fatal)
            {
                m_LogHelper.Log(LogLevel.Fatal, StringUtility.Format(format, arg0));
            }
        }

        /// <summary> Fatals. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <param name="arg1">   The first argument.</param>
        public static void Fatal(string format, object arg0, object arg1)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Fatal)
            {
                m_LogHelper.Log(LogLevel.Fatal, StringUtility.Format(format, arg0, arg1));
            }
        }

        /// <summary> Fatals. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <param name="arg1">   The first argument.</param>
        /// <param name="arg2">   The second argument.</param>
        public static void Fatal(string format, object arg0, object arg1, object arg2)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Fatal)
            {
                m_LogHelper.Log(LogLevel.Fatal, StringUtility.Format(format, arg0, arg1, arg2));
            }
        }

        /// <summary> Fatals. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="args">   A variable-length parameters list containing arguments.</param>
        public static void Fatal(string format, params object[] args)
        {
            if (m_LogHelper == null)
            {
                return;
            }

            if (m_LogLevel <= LogLevel.Fatal)
            {
                m_LogHelper.Log(LogLevel.Fatal, StringUtility.Format(format, args));
            }
        }
    }
}
