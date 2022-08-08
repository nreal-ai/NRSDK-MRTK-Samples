using System;

namespace NRKernal
{
    public enum Level
    {
        High,
        Normal,
    }

    /// <summary> A nr kernal error. </summary>
    public class NRKernalError : ApplicationException
    {
        public Level level;
        /// <summary> The error message. </summary>
        protected string msg;
        /// <summary> The inner exception. </summary>
        protected Exception innerException;

        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRKernalError(string msg, Level level = Level.Normal, Exception innerException = null) : base(msg)
        {
            this.innerException = innerException;
            this.msg = msg;
            this.level = level;
        }
        /// <summary> Gets the error. </summary>
        /// <returns> The error. </returns>
        virtual public string GetErrorMsg()
        {
            return msg;
        }
    }

    #region native error
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// /// internal errors
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary> A nr kernal error. </summary>
    public class NRNativeError : NRKernalError
    {
        public NativeResult result;
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRNativeError(NativeResult result, string msg, Level level = Level.Normal, Exception innerException = null) : base(msg, level, innerException)
        {
            this.result = result;
        }
        /// <summary> Gets the error. </summary>
        /// <returns> The error. </returns>
        override public string GetErrorMsg()
        {
            return string.Format("Error Code-{0}: {1}", (int)result, msg);
        }
    }

    /// <summary> A nr invalid argument error. </summary>
    public class NRInvalidArgumentError : NRNativeError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRInvalidArgumentError(NativeResult result, string msg, Exception innerException = null) : base(result, msg, Level.Normal, innerException)
        {
        }
    }

    /// <summary> A nr not enough memory error. </summary>
    public class NRNotEnoughMemoryError : NRNativeError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRNotEnoughMemoryError(NativeResult result, string msg, Exception innerException = null) : base(result, msg, Level.High, innerException)
        {
        }
    }

    /// <summary> A nr sdcard permission deny error. </summary>
    public class NRSdcardPermissionDenyError : NRNativeError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRSdcardPermissionDenyError(NativeResult result, string msg, Exception innerException = null) : base(result, msg, Level.High, innerException)
        {
        }
    }

    /// <summary> A nr un supported error. </summary>
    public class NRUnSupportedError : NRNativeError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRUnSupportedError(NativeResult result, string msg, Exception innerException = null) : base(result, msg, Level.High, innerException)
        {
        }
    }

    /// <summary> A nr glasses connect error. </summary>
    public class NRGlassesConnectError : NRNativeError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRGlassesConnectError(NativeResult result, string msg, Exception innerException = null) : base(result, msg, Level.High, innerException)
        {
        }
    }

    /// <summary> A nr sdk version mismatch error. </summary>
    public class NRSdkVersionMismatchError : NRNativeError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRSdkVersionMismatchError(NativeResult result, string msg, Exception innerException = null) : base(result, msg, Level.High, innerException)
        {
        }
    }

    /// <summary> A nr rgb camera device not find error. </summary>
    public class NRRGBCameraDeviceNotFindError : NRNativeError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRRGBCameraDeviceNotFindError(NativeResult result, string msg, Exception innerException = null) : base(result, msg, Level.Normal, innerException)
        {
        }
    }

    /// <summary> Display device not find error. </summary>
    public class NRDPDeviceNotFindError : NRNativeError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRDPDeviceNotFindError(NativeResult result, string msg, Exception innerException = null) : base(result, msg, Level.High, innerException)
        {
        }
    }

    /// <summary> MRSpace display device not find error. </summary>
    public class NRGetDisplayFailureError : NRNativeError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRGetDisplayFailureError(NativeResult result, string msg, Exception innerException = null) : base(result, msg, Level.High, innerException)
        {
        }
    }

    /// <summary> Display Mode mismatch error, as MRSpace mode is needed. </summary>
    public class NRDisplayModeMismatchError : NRNativeError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRDisplayModeMismatchError(NativeResult result, string msg, Exception innerException = null) : base(result, msg, Level.High, innerException)
        {
        }
    }

    /// <summary> A device not support hand tracking calculation error. </summary>
    public class NRUnSupportedHandtrackingCalculationError : NRNativeError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRUnSupportedHandtrackingCalculationError(NativeResult result, string msg, Exception innerException = null) : base(result, msg, Level.Normal, innerException)
        {
        }
    }
    #endregion

    #region internal error
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// /// internal errors
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary> A nr internal error. </summary>
    public class NRInternalError : NRKernalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRInternalError(string msg, Level level = Level.Normal, Exception innerException = null) : base(msg, level, innerException)
        {
        }
    }


    public class NRMissingKeyComponentError : NRInternalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRMissingKeyComponentError(string msg, Exception innerException = null) : base(msg, Level.High, innerException)
        {
        }
    }

    public class NRPermissionDenyError : NRInternalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRPermissionDenyError(string msg, Exception innerException = null) : base(msg, Level.Normal, innerException)
        {
        }
    }


    public class NRUnSupportDeviceError : NRInternalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRUnSupportDeviceError(string msg, Exception innerException = null) : base(msg, Level.High, innerException)
        {
        }
    }

    /// <summary> A nr glasses not available error. </summary>
    public class NRGlassesNotAvailbleError : NRInternalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRGlassesNotAvailbleError(string msg, Exception innerException = null) : base(msg, Level.High, innerException)
        {
        }
    }
    #endregion
}
